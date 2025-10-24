using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Runtime.Pipeline;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    public class MockRetryHadler : PipelineHandler
    {
        public override Task<T> InvokeAsync<T>(
            IExecutionContext executionContext,
            bool addAcceptMediaHeader = false,
            string apiVersion = null
        )
        {
            return base.InvokeAsync<T>(executionContext, addAcceptMediaHeader);
        }

        public override void InvokeSync(
            IExecutionContext executionContext,
            bool addAcceptMediaHeader = false,
            string apiVersion = null
        )
        {
            base.InvokeSync(executionContext, addAcceptMediaHeader, apiVersion);
        }
    }

    public class MockHttpHandler : IPipelineHandler
    {
        private ContentstackResponse _response;

        public MockHttpHandler(ContentstackResponse response)
        {
            _response = response;
        }

        public ILogManager LogManager { get; set; }
        public IPipelineHandler InnerHandler { get; set; }
        public Uri LastRequestUri { get; private set; }

        public async Task<T> InvokeAsync<T>(
            IExecutionContext executionContext,
            bool addAcceptMediaHeader = false,
            string apiVersion = null
        )
        {
            // Capture the request URI
            if (executionContext.RequestContext.service != null)
            {
                var httpRequest = executionContext.RequestContext.service.CreateHttpRequest(
                    null,
                    executionContext.RequestContext.config
                );
                LastRequestUri = httpRequest.RequestUri;
            }

            executionContext.ResponseContext.httpResponse = _response;

            if (executionContext.RequestContext.service != null)
            {
                executionContext.RequestContext.service.OnResponse(
                    _response,
                    executionContext.RequestContext.config
                );
            }
            return await Task.FromResult<T>((T)executionContext.ResponseContext.httpResponse);
        }

        public void InvokeSync(
            IExecutionContext executionContext,
            bool addAcceptMediaHeader = false,
            string apiVersion = null
        )
        {
            // Capture the request URI
            if (executionContext.RequestContext.service != null)
            {
                var httpRequest = executionContext.RequestContext.service.CreateHttpRequest(
                    null,
                    executionContext.RequestContext.config
                );
                LastRequestUri = httpRequest.RequestUri;
            }

            executionContext.ResponseContext.httpResponse = _response;
            if (executionContext.RequestContext.service != null)
            {
                executionContext.RequestContext.service.OnResponse(
                    _response,
                    executionContext.RequestContext.config
                );
            }
        }
    }

    public class MockHttpHandlerDisposable : MockHttpHandler, IDisposable
    {
        public bool disposed;

        public MockHttpHandlerDisposable(ContentstackResponse response)
            : base(response) { }

        #region Dispose methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                disposed = true;
            }
        }
        #endregion
    }
}
