using System;
using System.Net.Http;
using Contentstack.Management.Core.Services;

namespace Contentstack.Management.Core.Runtime.Contexts
{
    internal class RequestContext : IRequestContext
    {
        public IContentstackService service { get; set; }

        public ContentstackClientOptions config { get; set; }
        public int Retries { get; set; }
    }
}
