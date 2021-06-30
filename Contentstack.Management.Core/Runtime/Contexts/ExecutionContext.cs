using System;
namespace Contentstack.Management.Core.Runtime.Contexts
{
    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(IRequestContext requestContext, IResponseContext responseContext)
        {
            this.RequestContext = requestContext;
            this.ResponseContext = responseContext;
        }

        public IResponseContext ResponseContext { get; }

        public IRequestContext RequestContext { get; }
    }
}
