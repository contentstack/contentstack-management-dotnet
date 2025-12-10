using System;
using System.Net;
using Contentstack.Management.Core;

namespace Contentstack.Management.Core.Runtime.Pipeline.RetryHandler
{
    /// <summary>
    /// Configuration for retry behavior, supporting both network and HTTP error retries.
    /// </summary>
    public class RetryConfiguration
    {
        /// <summary>
        /// When set to true, the client will retry requests.
        /// When set to false, the client will not retry request.
        /// The default value is true.
        /// </summary>
        public bool RetryOnError { get; set; } = true;

        /// <summary>
        /// Returns the flag indicating how many retry HTTP requests an SDK should
        /// make for a single SDK operation invocation before giving up.
        /// The default value is 5.
        /// </summary>
        public int RetryLimit { get; set; } = 5;

        /// <summary>
        /// Returns the flag indicating delay in retrying HTTP requests.
        /// The default value is 300ms.
        /// </summary>
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(300);

        /// <summary>
        /// When set to true, the client will retry on network failures.
        /// The default value is true.
        /// </summary>
        public bool RetryOnNetworkFailure { get; set; } = true;

        /// <summary>
        /// When set to true, the client will retry on DNS failures.
        /// The default value is true.
        /// </summary>
        public bool RetryOnDnsFailure { get; set; } = true;

        /// <summary>
        /// When set to true, the client will retry on socket failures.
        /// The default value is true.
        /// </summary>
        public bool RetryOnSocketFailure { get; set; } = true;

        /// <summary>
        /// When set to true, the client will retry on HTTP server errors (5xx).
        /// The default value is true.
        /// </summary>
        public bool RetryOnHttpServerError { get; set; } = true;

        /// <summary>
        /// Maximum number of network retry attempts.
        /// The default value is 3.
        /// </summary>
        public int MaxNetworkRetries { get; set; } = 3;

        /// <summary>
        /// Base delay for network retries.
        /// The default value is 100ms.
        /// </summary>
        public TimeSpan NetworkRetryDelay { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Backoff strategy for network retries.
        /// The default value is Exponential.
        /// </summary>
        public BackoffStrategy NetworkBackoffStrategy { get; set; } = BackoffStrategy.Exponential;

        /// <summary>
        /// Custom function to determine if a status code should be retried.
        /// If null, default retry condition is used (429, 500, 502, 503, 504).
        /// </summary>
        public Func<HttpStatusCode, bool>? RetryCondition { get; set; }

        /// <summary>
        /// Options for retry delay calculation.
        /// </summary>
        public RetryDelayOptions RetryDelayOptions { get; set; } = new RetryDelayOptions();

        /// <summary>
        /// Creates a RetryConfiguration from ContentstackClientOptions.
        /// </summary>
        public static RetryConfiguration FromOptions(ContentstackClientOptions options)
        {
            return new RetryConfiguration
            {
                RetryOnError = options.RetryOnError,
                RetryLimit = options.RetryLimit,
                RetryDelay = options.RetryDelay,
                RetryOnNetworkFailure = options.RetryOnNetworkFailure,
                RetryOnDnsFailure = options.RetryOnDnsFailure,
                RetryOnSocketFailure = options.RetryOnSocketFailure,
                RetryOnHttpServerError = options.RetryOnHttpServerError,
                MaxNetworkRetries = options.MaxNetworkRetries,
                NetworkRetryDelay = options.NetworkRetryDelay,
                NetworkBackoffStrategy = options.NetworkBackoffStrategy,
                RetryCondition = options.RetryCondition,
                RetryDelayOptions = options.RetryDelayOptions ?? new RetryDelayOptions
                {
                    Base = options.RetryDelay
                }
            };
        }
    }

    /// <summary>
    /// Options for retry delay calculation.
    /// </summary>
    public class RetryDelayOptions
    {
        /// <summary>
        /// Base delay for retries.
        /// </summary>
        public TimeSpan Base { get; set; } = TimeSpan.FromMilliseconds(300);

        /// <summary>
        /// Custom backoff function. Parameters: retryCount, exception.
        /// Return TimeSpan.Zero or negative TimeSpan to disable retry for that attempt.
        /// </summary>
        public Func<int, Exception?, TimeSpan>? CustomBackoff { get; set; }
    }
}

