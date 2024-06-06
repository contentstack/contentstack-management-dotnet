using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Runtime.Contexts;

namespace Contentstack.Management.Core.Runtime.Pipeline
{
    public class PipelineHandler : IPipelineHandler
    {
        public ILogManager LogManager { get; set; }

        public IPipelineHandler InnerHandler { get; set; }

        public virtual Task<T> InvokeAsync<T>(IExecutionContext executionContext, bool addAcceptMediaHeader = false)
        {
            if (InnerHandler != null)
            {
                return InnerHandler.InvokeAsync<T>(executionContext, addAcceptMediaHeader);
            }
            throw new InvalidOperationException("Cannot invoke InnerHandler. InnerHandler is not set.");
        }

        public virtual void InvokeSync(IExecutionContext executionContext)
        {
            if (this.InnerHandler != null)
            {
                InnerHandler.InvokeSync(executionContext);
                return;
            }
            throw new InvalidOperationException("Cannot invoke InnerHandler. InnerHandler is not set.");
        }
    }
}
