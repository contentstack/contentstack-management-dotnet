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
        public override Task<T> InvokeAsync<T>(IExecutionContext executionContext, bool addAcceptMediaHeader = false)
        {
            return base.InvokeAsync<T>(executionContext, addAcceptMediaHeader);
        }

        public override void InvokeSync(IExecutionContext executionContext, bool addAcceptMediaHeader = false)
        {
            base.InvokeSync(executionContext, addAcceptMediaHeader);
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

        public async Task<T> InvokeAsync<T>(IExecutionContext executionContext, bool addAcceptMediaHeader = false) 
        {
            executionContext.ResponseContext.httpResponse = _response;

            if (executionContext.RequestContext.service != null)
            {
                executionContext.RequestContext.service.OnResponse(_response, executionContext.RequestContext.config);
            }
            return await Task.FromResult<T>((T)executionContext.ResponseContext.httpResponse);
        }

        public void InvokeSync(IExecutionContext executionContext, bool addAcceptMediaHeader = false)
        {
            executionContext.ResponseContext.httpResponse = _response;
            if (executionContext.RequestContext.service != null)
            {
                executionContext.RequestContext.service.OnResponse(_response, executionContext.RequestContext.config);
            }
        }
    }

    public class MockHttpHandlerDisposable : MockHttpHandler, IDisposable
    {
        public bool disposed;

        public MockHttpHandlerDisposable(ContentstackResponse response) : base(response) { }

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
