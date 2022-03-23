using System;
using System.Net.Http;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Runtime.Contexts;

namespace Contentstack.Management.Core.Runtime.Pipeline
{
    public class HttpHandler: IPipelineHandler
    {
        #region Private
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        internal HttpHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Public
        public ILogManager LogManager { get; set; }

        public async System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            IHttpRequest httpRequest = null;
            try
            {
                var requestContext = executionContext.RequestContext;

                httpRequest = requestContext.service.CreateHttpRequest(_httpClient, requestContext.config);

                if (requestContext.service.HasRequestBody() && requestContext.service.Content != null)
                {
                    httpRequest.WriteToRequestBody(requestContext.service.Content, requestContext.service.Headers);
                }

                executionContext.ResponseContext.httpResponse = await httpRequest.GetResponseAsync().ConfigureAwait(false);
                executionContext.RequestContext.service.OnResponse(executionContext.ResponseContext.httpResponse, requestContext.config);

                return await System.Threading.Tasks.Task.FromResult<T>((T)executionContext.ResponseContext.httpResponse);
            }
            catch (Exception e)
            {
                if (LogManager != null)
                {
                    LogManager.Error(e, "Http request error", new object());
                }
                throw;
            }
            finally
            {
                if (httpRequest != null)
                    httpRequest.Dispose();
            }
        }

        public void InvokeSync(IExecutionContext executionContext)
        {
            IHttpRequest httpRequest = null;
            try
            {
                var requestContext = executionContext.RequestContext;

                httpRequest = requestContext.service.CreateHttpRequest(_httpClient, requestContext.config);

                if (requestContext.service.HasRequestBody() && requestContext.service.Content != null)
                {
                    httpRequest.WriteToRequestBody(requestContext.service.Content, requestContext.service.Headers);
                }

                executionContext.ResponseContext.httpResponse =  httpRequest.GetResponse();

                executionContext.RequestContext.service.OnResponse(executionContext.ResponseContext.httpResponse, requestContext.config);
            }
            catch (Exception e)
            {
                if (LogManager != null)
                {
                    LogManager.Error(e, "Http request error", new object());
                }
                throw;
            }
            finally
            {
                if (httpRequest != null)
                    httpRequest.Dispose();
            }
        }
        #endregion
    }
}
