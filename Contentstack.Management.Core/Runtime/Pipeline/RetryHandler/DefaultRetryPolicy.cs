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

        public override bool CanRetry(IExecutionContext executionContext)
        {
            if (retryConfiguration != null)
            {
                return retryConfiguration.RetryOnError;
            }
            return RetryOnError;
        }

        public override bool RetryForException(
            IExecutionContext executionContext,
            Exception exception
        )
        {
            if (retryConfiguration == null)
            {
                // Fallback to old behavior if no configuration provided
                return false;
            }

            var requestContext = executionContext.RequestContext;

            // Check for HTTP errors (ContentstackErrorException) FIRST
            // This must come before network error check because ContentstackErrorException with 5xx
            // can be detected as network errors, but we want to use HTTP retry logic and limits
            if (exception is ContentstackErrorException contentstackException)
            {
                // Check if HTTP retry limit exceeded first (applies to all HTTP errors)
                // HttpRetryCount is the number of retries already attempted
                // RetryLimit is the maximum number of retries allowed
                // So when HttpRetryCount >= RetryLimit, we've reached or exceeded the limit
                // IMPORTANT: Check this BEFORE checking if it's a retryable error to ensure limit is respected
                // This matches the pattern used in ShouldRetryHttpStatusCode method
                if (requestContext.HttpRetryCount >= this.RetryLimit)
                {
                    return false;
                }

                // Check if it's a server error (5xx) that should be retried
                if (contentstackException.StatusCode >= HttpStatusCode.InternalServerError &&
                    contentstackException.StatusCode <= HttpStatusCode.GatewayTimeout)
                {
                    if (retryConfiguration.RetryOnHttpServerError)
                    {
                        return true;
                    }
                    // If RetryOnHttpServerError is false, fall through to check custom retry condition
                }

                // Check custom retry condition
                if (delayCalculator.ShouldRetryHttpStatusCode(contentstackException.StatusCode, retryConfiguration))
                {
                    return true;
                }
            }

            // Check for network errors (only if not already handled as HTTP error)
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

            return false;
        }

        public override bool RetryLimitExceeded(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;

            if (retryConfiguration != null)
            {
                // Check both network and HTTP retry limits
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
        public TimeSpan GetHttpRetryDelay(IRequestContext requestContext, Exception exception, HttpResponseHeaders? responseHeaders = null)
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

