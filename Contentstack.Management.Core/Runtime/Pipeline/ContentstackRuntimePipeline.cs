using System;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Log;
using Contentstack.Management.Core.Runtime.Contexts;

namespace Contentstack.Management.Core.Runtime.Pipeline
{
    public partial class ContentstackRuntimePipeline : IDisposable
    {
        #region Private members

        bool _disposed;
        readonly ILogManager _logManager;

        // The top-most handler in the pipeline.
        IPipelineHandler _handler;

        /// <summary>
        /// The top-most handler in the pipeline.
        /// </summary>
        public IPipelineHandler Handler
        {
            get { return _handler; }
        }
        #endregion

        public ContentstackRuntimePipeline(IPipelineHandler handler, ILogManager logManager)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (logManager == null)
                throw new ArgumentNullException("logManager");

            _handler = handler;
            _handler.LogManager = logManager;

            _logManager = logManager;
        }

        public void AddLogger(Logger logger)
        {
            _logManager.AddLogger(logger);
        }

        public System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            ThrowIfDisposed();

            return _handler.InvokeAsync<T>(executionContext);
        }

        public IResponseContext InvokeSync(IExecutionContext executionContext)
        {
            ThrowIfDisposed();

            _handler.InvokeSync(executionContext);
            return executionContext.ResponseContext;
        }

        public void ReplaceHandler(IPipelineHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            // TODO to add Multiple Handlers
            _handler = handler;
        }

        #region Dispose methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                var handler = this.Handler;
                while (handler != null)
                {
                    //TODO to add multiple Handler Dispose
                    var disposable = handler as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                    handler = null;
                }

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion
    }
}
