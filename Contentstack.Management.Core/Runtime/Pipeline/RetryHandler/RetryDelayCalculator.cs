using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Runtime.Pipeline.RetryHandler
{
    /// <summary>
    /// Utility for calculating retry delays with various backoff strategies.
    /// </summary>
    public class RetryDelayCalculator
    {
        private static readonly Random _random = new Random();
        private const int MaxJitterMs = 100;

        /// <summary>
        /// Calculates the delay for a network retry attempt.
        /// </summary>
        /// <param name="attempt">The current retry attempt number (1-based).</param>
        /// <param name="config">The retry configuration.</param>
        /// <returns>The delay to wait before retrying.</returns>
        public TimeSpan CalculateNetworkRetryDelay(int attempt, RetryConfiguration config)
        {
            TimeSpan baseDelay = config.NetworkRetryDelay;
            TimeSpan calculatedDelay;

            switch (config.NetworkBackoffStrategy)
            {
                case BackoffStrategy.Exponential:
                    // Exponential: baseDelay * 2^(attempt-1)
                    double exponentialDelay = baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1);
                    calculatedDelay = TimeSpan.FromMilliseconds(exponentialDelay);
                    break;

                case BackoffStrategy.Fixed:
                default:
                    calculatedDelay = baseDelay;
                    break;
            }

            // Add jitter (random 0-100ms)
            int jitterMs = _random.Next(0, MaxJitterMs);
            return calculatedDelay.Add(TimeSpan.FromMilliseconds(jitterMs));
        }

        /// <summary>
        /// Calculates the delay for an HTTP retry attempt.
        /// </summary>
        /// <param name="retryCount">The current retry count (0-based).</param>
        /// <param name="config">The retry configuration.</param>
        /// <param name="error">The exception that triggered the retry, if any.</param>
        /// <param name="responseHeaders">The HTTP response headers, if available.</param>
        /// <returns>The delay to wait before retrying. Returns TimeSpan.Zero or negative to disable retry.</returns>
        public TimeSpan CalculateHttpRetryDelay(int retryCount, RetryConfiguration config, Exception? error, HttpResponseHeaders? responseHeaders = null)
        {
            // Check for Retry-After header (for 429 Too Many Requests)
            if (responseHeaders != null && responseHeaders.RetryAfter != null)
            {
                var retryAfter = responseHeaders.RetryAfter;
                if (retryAfter.Delta.HasValue)
                {
                    return retryAfter.Delta.Value;
                }
                if (retryAfter.Date.HasValue)
                {
                    var delay = retryAfter.Date.Value - DateTimeOffset.UtcNow;
                    if (delay > TimeSpan.Zero)
                    {
                        return delay;
                    }
                }
            }

            // Use custom backoff function if provided
            if (config.RetryDelayOptions.CustomBackoff != null)
            {
                var customDelay = config.RetryDelayOptions.CustomBackoff(retryCount, error);
                // Negative or zero delay means don't retry
                if (customDelay <= TimeSpan.Zero)
                {
                    return customDelay;
                }
                return customDelay;
            }

            // Default: use base delay with exponential backoff
            TimeSpan baseDelay = config.RetryDelayOptions.Base;
            if (baseDelay == TimeSpan.Zero)
            {
                baseDelay = config.RetryDelay;
            }

            // Exponential backoff: baseDelay * 2^retryCount
            double exponentialDelay = baseDelay.TotalMilliseconds * Math.Pow(2, retryCount);
            TimeSpan calculatedDelay = TimeSpan.FromMilliseconds(exponentialDelay);

            // Add jitter (random 0-100ms)
            int jitterMs = _random.Next(0, MaxJitterMs);
            return calculatedDelay.Add(TimeSpan.FromMilliseconds(jitterMs));
        }

        /// <summary>
        /// Determines if an HTTP status code should be retried based on configuration.
        /// </summary>
        public bool ShouldRetryHttpStatusCode(HttpStatusCode statusCode, RetryConfiguration config)
        {
            if (config.RetryCondition != null)
            {
                return config.RetryCondition(statusCode);
            }

            // Default retry condition: 429, 500, 502, 503, 504
            return statusCode == HttpStatusCode.TooManyRequests ||
                   statusCode == HttpStatusCode.InternalServerError ||
                   statusCode == HttpStatusCode.BadGateway ||
                   statusCode == HttpStatusCode.ServiceUnavailable ||
                   statusCode == HttpStatusCode.GatewayTimeout;
        }
    }
}

