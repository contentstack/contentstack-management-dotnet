using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Runtime.Contexts;

namespace Contentstack.Management.Core.Runtime.Pipeline.RetryHandler
{
    public class RetryHandler : PipelineHandler
    {
        public RetryPolicy RetryPolicy { get; private set; }

        public RetryHandler(RetryPolicy retryPolicy)
        {
            this.RetryPolicy = retryPolicy;
        }
        public override async Task<T> InvokeAsync<T>(IExecutionContext executionContext, bool addAcceptMediaHeader = false)
        {
            var requestContext = executionContext.RequestContext;
            var responseContext = executionContext.ResponseContext;
            bool shouldRetry = false;
            do
            {
                try
                {
                    var response =  await base.InvokeAsync<T>(executionContext, addAcceptMediaHeader);
                    return response;
                }
                catch (Exception exception)
                {
                    shouldRetry = this.RetryPolicy.Retry(executionContext, exception);
                    if (!shouldRetry)
                    {
                        LogForError(requestContext, exception);
                        throw;
                    }
                    else
                    {
                        requestContext.Retries++;
                        LogForRetry(requestContext, exception);
                    }
                }

                this.RetryPolicy.WaitBeforeRetry(executionContext);

            } while (shouldRetry == true);
            throw new ContentstackException("No response was return nor exception was thrown");
        }

        public override void InvokeSync(IExecutionContext executionContext, bool addAcceptMediaHeader = false)
        {
            var requestContext = executionContext.RequestContext;
            bool shouldRetry = false;
            do
            {
                try
                {
                    base.InvokeSync(executionContext, addAcceptMediaHeader);
                    return;
                }
                catch (Exception exception)
                {
                    shouldRetry = this.RetryPolicy.Retry(executionContext, exception);
                    if (!shouldRetry)
                    {
                        LogForError(requestContext, exception);
                        throw;
                    }
                    else
                    {
                        requestContext.Retries++;
                        LogForRetry(requestContext, exception);
                    }

                }

                this.RetryPolicy.WaitBeforeRetry(executionContext);

            } while (shouldRetry == true);
        }

        private void LogForError(IRequestContext requestContext, Exception exception)
        {
            LogManager.InfoFormat("{0} making request {1}. Attempt {2} of {3}.",
                          exception.GetType().Name,
                          requestContext.service.ResourcePath,
                          requestContext.Retries + 1,
                          RetryPolicy.RetryLimit);
        }

        private void LogForRetry(IRequestContext requestContext, Exception exception)
        {
            LogManager.Error(exception, "{0} making request {1}. Attempt {2} of {3}.",
                          exception.GetType().Name,
                          requestContext.service.ResourcePath,
                          requestContext.Retries,
                          RetryPolicy.RetryLimit);
        }
    }
}
