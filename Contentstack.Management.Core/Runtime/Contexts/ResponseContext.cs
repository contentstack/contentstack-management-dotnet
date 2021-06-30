using System;
using Contentstack.Management.Core.Http;

namespace Contentstack.Management.Core.Runtime.Contexts
{
    internal class ResponseContext : IResponseContext
    {
        public IResponse httpResponse { get; set; }
    }
}
