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

        /// <summary>
        /// Number of network retry attempts made.
        /// </summary>
        public int NetworkRetryCount { get; set; }

        /// <summary>
        /// Number of HTTP retry attempts made.
        /// </summary>
        public int HttpRetryCount { get; set; }

        /// <summary>
        /// Unique identifier for this request, used for correlation in logs.
        /// </summary>
        public Guid RequestId { get; set; } = Guid.NewGuid();
    }
}
