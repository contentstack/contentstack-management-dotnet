using System;
using System.Net.Http;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Runtime.Contexts;

namespace Contentstack.Management.Core.Runtime.Pipeline.RetryHandler
{
    public class RetryHandler : PipelineHandler
    {
        public RetryPolicy RetryPolicy { get; private set; }
        private readonly NetworkErrorDetector networkErrorDetector;

        public RetryHandler(RetryPolicy retryPolicy)
        {
            this.RetryPolicy = retryPolicy;
            this.networkErrorDetector = new NetworkErrorDetector();
        }

        public override async Task<T> InvokeAsync<T>(IExecutionContext executionContext, bool addAcceptMediaHeader = false, string apiVersion = null)
        {
            var requestContext = executionContext.RequestContext;
            var responseContext = executionContext.ResponseContext;
            bool shouldRetry = false;
            Exception lastException = null;

            do
            {
                try
                {
                    var response = await base.InvokeAsync<T>(executionContext, addAcceptMediaHeader, apiVersion);
                    
                    // Check if response is an HTTP error that should be retried
                    if (response is ContentstackResponse contentstackResponse && 
                        !contentstackResponse.IsSuccessStatusCode)
                    {
                        var defaultRetryPolicy = RetryPolicy as DefaultRetryPolicy;
                        if (defaultRetryPolicy != null && 
                            defaultRetryPolicy.ShouldRetryHttpStatusCode(contentstackResponse.StatusCode, requestContext))
                        {
                            requestContext.HttpRetryCount++;
                            shouldRetry = true;
                            LogForHttpRetry(requestContext, contentstackResponse.StatusCode);
                            
                            // Get delay and wait
                            var delay = defaultRetryPolicy.GetHttpRetryDelay(
                                requestContext, 
                                null, 
                                contentstackResponse.ResponseBody?.Headers);
                            await Task.Delay(delay);
                            continue;
                        }
                        else
                        {
                            // Retry limit exceeded or not retryable - throw exception
                            throw ContentstackErrorException.CreateException(contentstackResponse.ResponseBody);
                        }
                    }

                    return response;
                }
                catch (Exception exception)
                {
                    lastException = exception;
                    shouldRetry = this.RetryPolicy.Retry(executionContext, exception);
                    
                    if (!shouldRetry)
                    {
                        LogForError(requestContext, exception);
                        throw;
                    }
                    else
                    {
                        // Classify error and increment appropriate counter
                        var networkErrorInfo = networkErrorDetector.IsTransientNetworkError(exception);
                        if (networkErrorInfo != null)
                        {
                            requestContext.NetworkRetryCount++;
                            LogForNetworkRetry(requestContext, exception, networkErrorInfo);
                        }
                        else if (exception is ContentstackErrorException)
                        {
                            requestContext.HttpRetryCount++;
                            LogForHttpRetry(requestContext, ((ContentstackErrorException)exception).StatusCode);
                        }
                        else
                        {
                            requestContext.Retries++;
                            LogForRetry(requestContext, exception);
                        }
                    }
                }

                this.RetryPolicy.WaitBeforeRetry(executionContext);

            } while (shouldRetry == true);

            throw new ContentstackException("No response was return nor exception was thrown");
        }

        public override void InvokeSync(IExecutionContext executionContext, bool addAcceptMediaHeader = false, string apiVersion = null)
        {
            var requestContext = executionContext.RequestContext;
            bool shouldRetry = false;
            Exception lastException = null;

            do
            {
                try
                {
                    base.InvokeSync(executionContext, addAcceptMediaHeader, apiVersion);
                    
                    // Check if response is an HTTP error that should be retried
                    var response = executionContext.ResponseContext.httpResponse;
                    if (response is ContentstackResponse contentstackResponse && 
                        !contentstackResponse.IsSuccessStatusCode)
                    {
                        var defaultRetryPolicy = RetryPolicy as DefaultRetryPolicy;
                        if (defaultRetryPolicy != null && 
                            defaultRetryPolicy.ShouldRetryHttpStatusCode(contentstackResponse.StatusCode, requestContext))
                        {
                            requestContext.HttpRetryCount++;
                            shouldRetry = true;
                            LogForHttpRetry(requestContext, contentstackResponse.StatusCode);
                            
                            // Get delay and wait
                            var delay = defaultRetryPolicy.GetHttpRetryDelay(
                                requestContext, 
                                null, 
                                contentstackResponse.ResponseBody?.Headers);
                            System.Threading.Tasks.Task.Delay(delay).Wait();
                            continue;
                        }
                        else
                        {
                            // Retry limit exceeded or not retryable - throw exception
                            throw ContentstackErrorException.CreateException(contentstackResponse.ResponseBody);
                        }
                    }

                    return;
                }
                catch (Exception exception)
                {
                    lastException = exception;
                    shouldRetry = this.RetryPolicy.Retry(executionContext, exception);
                    
                    if (!shouldRetry)
                    {
                        LogForError(requestContext, exception);
                        throw;
                    }
                    else
                    {
                        // Classify error and increment appropriate counter
                        var networkErrorInfo = networkErrorDetector.IsTransientNetworkError(exception);
                        if (networkErrorInfo != null)
                        {
                            requestContext.NetworkRetryCount++;
                            LogForNetworkRetry(requestContext, exception, networkErrorInfo);
                        }
                        else if (exception is ContentstackErrorException)
                        {
                            requestContext.HttpRetryCount++;
                            LogForHttpRetry(requestContext, ((ContentstackErrorException)exception).StatusCode);
                        }
                        else
                        {
                            requestContext.Retries++;
                            LogForRetry(requestContext, exception);
                        }
                    }
                }

                this.RetryPolicy.WaitBeforeRetry(executionContext);

            } while (shouldRetry == true);
        }

        private void LogForError(IRequestContext requestContext, Exception exception)
        {
            var totalAttempts = requestContext.NetworkRetryCount + requestContext.HttpRetryCount + requestContext.Retries + 1;
            LogManager.InfoFormat(
                "[RequestId: {0}] {1} making request {2}. Final attempt {3} failed. Network retries: {4}, HTTP retries: {5}.",
                requestContext.RequestId,
                exception.GetType().Name,
                requestContext.service.ResourcePath,
                totalAttempts,
                requestContext.NetworkRetryCount,
                requestContext.HttpRetryCount);
        }

        private void LogForRetry(IRequestContext requestContext, Exception exception)
        {
            var totalAttempts = requestContext.NetworkRetryCount + requestContext.HttpRetryCount + requestContext.Retries;
            LogManager.InfoFormat(
                "[RequestId: {0}] {1} making request {2}. Retrying (attempt {3}). Network retries: {4}, HTTP retries: {5}.",
                requestContext.RequestId,
                exception.GetType().Name,
                requestContext.service.ResourcePath,
                totalAttempts,
                requestContext.NetworkRetryCount,
                requestContext.HttpRetryCount);
        }

        private void LogForNetworkRetry(IRequestContext requestContext, Exception exception, NetworkErrorInfo errorInfo)
        {
            var totalAttempts = requestContext.NetworkRetryCount + requestContext.HttpRetryCount + requestContext.Retries;
            LogManager.InfoFormat(
                "[RequestId: {0}] Network error ({1}) making request {2}. Retrying (attempt {3}, network retry {4}).",
                requestContext.RequestId,
                errorInfo.ErrorType,
                requestContext.service.ResourcePath,
                totalAttempts,
                requestContext.NetworkRetryCount);
        }

        private void LogForHttpRetry(IRequestContext requestContext, System.Net.HttpStatusCode statusCode)
        {
            var totalAttempts = requestContext.NetworkRetryCount + requestContext.HttpRetryCount + requestContext.Retries;
            LogManager.InfoFormat(
                "[RequestId: {0}] HTTP error ({1}) making request {2}. Retrying (attempt {3}, HTTP retry {4}).",
                requestContext.RequestId,
                statusCode,
                requestContext.service.ResourcePath,
                totalAttempts,
                requestContext.HttpRetryCount);
        }
    }
}
