using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Runtime.Pipeline;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Mock HTTP handler that can simulate failures and successes for retry testing.
    /// </summary>
    public class MockHttpHandlerWithRetries : IPipelineHandler
    {
        private readonly Queue<Func<IExecutionContext, IResponse>> _responseQueue;
        private readonly Queue<Exception> _exceptionQueue;
        private int _callCount = 0;

        public ILogManager LogManager { get; set; }
        public IPipelineHandler InnerHandler { get; set; }
        public int CallCount => _callCount;

        public MockHttpHandlerWithRetries()
        {
            _responseQueue = new Queue<Func<IExecutionContext, IResponse>>();
            _exceptionQueue = new Queue<Exception>();
        }

        /// <summary>
        /// Adds a response that will be returned on the next call.
        /// </summary>
        public void AddResponse(HttpStatusCode statusCode, string body = null)
        {
            _responseQueue.Enqueue((context) =>
            {
                var response = new HttpResponseMessage(statusCode);
                if (body != null)
                {
                    response.Content = new StringContent(body);
                }
                return new ContentstackResponse(response, JsonSerializer.Create(new JsonSerializerSettings()));
            });
        }

        /// <summary>
        /// Adds a successful response (200 OK).
        /// </summary>
        public void AddSuccessResponse(string body = "{\"success\": true}")
        {
            AddResponse(HttpStatusCode.OK, body);
        }

        /// <summary>
        /// Adds an exception that will be thrown on the next call.
        /// </summary>
        public void AddException(Exception exception)
        {
            _exceptionQueue.Enqueue(exception);
        }

        /// <summary>
        /// Adds multiple failures followed by a success.
        /// </summary>
        public void AddFailuresThenSuccess(int failureCount, Exception failureException, string successBody = "{\"success\": true}")
        {
            for (int i = 0; i < failureCount; i++)
            {
                AddException(failureException);
            }
            AddSuccessResponse(successBody);
        }

        /// <summary>
        /// Adds multiple HTTP error responses followed by a success.
        /// </summary>
        public void AddHttpErrorsThenSuccess(int errorCount, HttpStatusCode errorStatusCode, string successBody = "{\"success\": true}")
        {
            for (int i = 0; i < errorCount; i++)
            {
                AddResponse(errorStatusCode);
            }
            AddSuccessResponse(successBody);
        }

        public async System.Threading.Tasks.Task<T> InvokeAsync<T>(
            IExecutionContext executionContext,
            bool addAcceptMediaHeader = false,
            string apiVersion = null)
        {
            _callCount++;

            // Check for exceptions first
            if (_exceptionQueue.Count > 0)
            {
                var exception = _exceptionQueue.Dequeue();
                throw exception;
            }

            // Check for responses
            if (_responseQueue.Count > 0)
            {
                var responseFactory = _responseQueue.Dequeue();
                var response = responseFactory(executionContext);
                executionContext.ResponseContext.httpResponse = response;
                return await System.Threading.Tasks.Task.FromResult<T>((T)response);
            }

            // Default: return success
            var defaultResponse = new HttpResponseMessage(HttpStatusCode.OK);
            defaultResponse.Content = new StringContent("{\"success\": true}");
            var contentstackResponse = new ContentstackResponse(defaultResponse, JsonSerializer.Create(new JsonSerializerSettings()));
            executionContext.ResponseContext.httpResponse = contentstackResponse;
            return await System.Threading.Tasks.Task.FromResult<T>((T)(IResponse)contentstackResponse);
        }

        public void InvokeSync(
            IExecutionContext executionContext,
            bool addAcceptMediaHeader = false,
            string apiVersion = null)
        {
            _callCount++;

            // Check for exceptions first
            if (_exceptionQueue.Count > 0)
            {
                var exception = _exceptionQueue.Dequeue();
                throw exception;
            }

            // Check for responses
            if (_responseQueue.Count > 0)
            {
                var responseFactory = _responseQueue.Dequeue();
                var response = responseFactory(executionContext);
                executionContext.ResponseContext.httpResponse = response;
                return;
            }

            // Default: return success
            var defaultResponse = new HttpResponseMessage(HttpStatusCode.OK);
            defaultResponse.Content = new StringContent("{\"success\": true}");
            var contentstackResponse = new ContentstackResponse(defaultResponse, JsonSerializer.Create(new JsonSerializerSettings()));
            executionContext.ResponseContext.httpResponse = contentstackResponse;
        }
    }
}

