using System;
namespace Contentstack.Management.Core.Runtime.Contexts
{
    public interface IExecutionContext
    {
        IResponseContext ResponseContext { get; }
        IRequestContext RequestContext { get; }
    }
}
