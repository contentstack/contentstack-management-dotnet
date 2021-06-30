using System;
using Contentstack.Management.Core.Http;

namespace Contentstack.Management.Core.Runtime.Contexts
{
    public interface IResponseContext
    {
        IResponse httpResponse { get; set; }
    }
}
