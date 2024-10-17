using System;
using System.Collections.Generic;
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

        public ContentstackRuntimePipeline(List<IPipelineHandler> handlers, ILogManager logManager)
        {
            if (handlers == null || handlers.Count == 0)
                throw new ArgumentNullException("handler");

            if (logManager == null)
                throw new ArgumentNullException("logManager");

            foreach (IPipelineHandler handler in handlers)
            {
                AddHanlder(handler);
            }
            _handler.LogManager = logManager;

            _logManager = logManager;
        }

        public void AddLogger(Logger logger)
        {
            _logManager.AddLogger(logger);
        }

        public void AddHanlder(IPipelineHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            ThrowIfDisposed();

            var currentHanler = handler;
            while (currentHanler.InnerHandler != null)
            {
                currentHanler = currentHanler.InnerHandler;
            }

            if (_handler != null)
            {
                currentHanler.InnerHandler = _handler;
            }

            _handler = currentHanler;
        }

        public System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext, bool addAcceptMediaHeader = false)
        {
            ThrowIfDisposed();

            return _handler.InvokeAsync<T>(executionContext, addAcceptMediaHeader);
        }

        public IResponseContext InvokeSync(IExecutionContext executionContext, bool addAcceptMediaHeader = false)
        {
            ThrowIfDisposed();

            _handler.InvokeSync(executionContext, addAcceptMediaHeader);
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
                    var innerHandler = handler.InnerHandler;
                    var disposable = handler as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                    handler = innerHandler;
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
