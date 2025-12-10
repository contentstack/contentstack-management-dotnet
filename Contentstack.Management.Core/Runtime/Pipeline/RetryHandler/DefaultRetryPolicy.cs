using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Runtime.Contexts;

namespace Contentstack.Management.Core.Runtime.Pipeline.RetryHandler
{
    public partial class DefaultRetryPolicy : RetryPolicy
    {
        protected TimeSpan retryDelay { get; set; }
        protected RetryConfiguration retryConfiguration;
        protected NetworkErrorDetector networkErrorDetector;
        protected RetryDelayCalculator delayCalculator;

        protected ICollection<HttpStatusCode> statusCodesToRetryOn = new HashSet<HttpStatusCode>
        {
            HttpStatusCode.InternalServerError,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.BadGateway,
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.RequestTimeout,
            (HttpStatusCode)429,
            HttpStatusCode.Unauthorized
        };

        internal DefaultRetryPolicy(int retryLimit, TimeSpan delay)
        {
            RetryLimit = retryLimit;
            retryDelay = delay;
            networkErrorDetector = new NetworkErrorDetector();
            delayCalculator = new RetryDelayCalculator();
        }

        internal DefaultRetryPolicy(RetryConfiguration config)
        {
            retryConfiguration = config ?? throw new ArgumentNullException(nameof(config));
            RetryLimit = config.RetryLimit;
            retryDelay = config.RetryDelay;
            networkErrorDetector = new NetworkErrorDetector();
            delayCalculator = new RetryDelayCalculator();
        }

        protected override bool CanRetry(IExecutionContext executionContext)
        {
            if (retryConfiguration != null)
            {
                return retryConfiguration.RetryOnError;
            }
            return RetryOnError;
        }

        protected override bool RetryForException(IExecutionContext executionContext, Exception exception)
        {
            if (retryConfiguration == null)
            {
                // Fallback to old behavior if no configuration provided
                return false;
            }

            var requestContext = executionContext.RequestContext;

            // Check for network errors
            var networkErrorInfo = networkErrorDetector.IsTransientNetworkError(exception);
            if (networkErrorInfo != null)
            {
                if (networkErrorDetector.ShouldRetryNetworkError(networkErrorInfo, retryConfiguration))
                {
                    // Check if network retry limit exceeded
                    if (requestContext.NetworkRetryCount >= retryConfiguration.MaxNetworkRetries)
                    {
                        return false;
                    }
                    return true;
                }
            }

            // Check for HTTP errors (ContentstackErrorException)
            if (exception is ContentstackErrorException contentstackException)
            {
                // Check if it's a server error (5xx) that should be retried
                if (contentstackException.StatusCode >= HttpStatusCode.InternalServerError &&
                    contentstackException.StatusCode <= HttpStatusCode.GatewayTimeout)
                {
                    if (retryConfiguration.RetryOnHttpServerError)
                    {
                        // Check if HTTP retry limit exceeded
                        // If HttpRetryCount >= RetryLimit, we've already exhausted retries
                        if (requestContext.HttpRetryCount >= retryConfiguration.RetryLimit)
                        {
                            return false;
                        }
                        return true;
                    }
                    return false;
                }

                // Check custom retry condition (for non-5xx errors like 429)
                if (delayCalculator.ShouldRetryHttpStatusCode(contentstackException.StatusCode, retryConfiguration))
                {
                    // Check if HTTP retry limit exceeded
                    // If HttpRetryCount >= RetryLimit, we've already exhausted retries
                    if (requestContext.HttpRetryCount >= retryConfiguration.RetryLimit)
                    {
                        return false;
                    }
                    return true;
                }
            }

            return false;
        }

        protected override bool RetryLimitExceeded(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;

            if (retryConfiguration != null)
            {
                // Return true only if BOTH limits are exceeded (meaning we've exhausted all retry options)
                // Individual limits are checked in RetryForException
                return requestContext.NetworkRetryCount >= retryConfiguration.MaxNetworkRetries &&
                       requestContext.HttpRetryCount >= retryConfiguration.RetryLimit;
            }

            // Fallback to old behavior
            return requestContext.Retries >= this.RetryLimit;
        }

        internal override void WaitBeforeRetry(IExecutionContext executionContext)
        {
            if (retryConfiguration == null)
            {
                // Fallback to old behavior
                System.Threading.Tasks.Task.Delay(retryDelay.Milliseconds).Wait();
                return;
            }

            var requestContext = executionContext.RequestContext;
            TimeSpan delay;

            // Determine delay based on error type
            // We need to check the last exception, but we don't have it here
            // So we'll use a heuristic: if network retries > 0, use network delay
            if (requestContext.NetworkRetryCount > 0)
            {
                delay = delayCalculator.CalculateNetworkRetryDelay(
                    requestContext.NetworkRetryCount,
                    retryConfiguration);
            }
            else
            {
                // HTTP retry - we'll use the last exception if available
                // For now, use base delay with exponential backoff
                delay = delayCalculator.CalculateHttpRetryDelay(
                    requestContext.HttpRetryCount,
                    retryConfiguration,
                    null);
            }

            System.Threading.Tasks.Task.Delay(delay).Wait();
        }

        /// <summary>
        /// Determines if an HTTP status code should be retried.
        /// </summary>
        public bool ShouldRetryHttpStatusCode(HttpStatusCode statusCode, IRequestContext requestContext)
        {
            if (retryConfiguration == null)
            {
                return statusCodesToRetryOn.Contains(statusCode);
            }

            if (requestContext.HttpRetryCount >= retryConfiguration.RetryLimit)
            {
                return false;
            }

            return delayCalculator.ShouldRetryHttpStatusCode(statusCode, retryConfiguration);
        }

        /// <summary>
        /// Gets the retry delay for an HTTP error.
        /// </summary>
        public TimeSpan GetHttpRetryDelay(IRequestContext requestContext, Exception exception, HttpResponseHeaders responseHeaders = null)
        {
            if (retryConfiguration == null)
            {
                return retryDelay;
            }

            return delayCalculator.CalculateHttpRetryDelay(
                requestContext.HttpRetryCount,
                retryConfiguration,
                exception,
                responseHeaders);
        }

        /// <summary>
        /// Gets the retry delay for a network error.
        /// </summary>
        public TimeSpan GetNetworkRetryDelay(IRequestContext requestContext)
        {
            if (retryConfiguration == null)
            {
                return retryDelay;
            }

            return delayCalculator.CalculateNetworkRetryDelay(
                requestContext.NetworkRetryCount,
                retryConfiguration);
        }
    }
}

