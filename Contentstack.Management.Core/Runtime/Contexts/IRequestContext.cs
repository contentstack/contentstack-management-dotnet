using System;
using System.Net.Http;
using Contentstack.Management.Core.Services;

namespace Contentstack.Management.Core.Runtime.Contexts
{
    public interface IRequestContext
    {
        IContentstackService service { get; set; }
        ContentstackClientOptions config { get; set; }
        int Retries { get; set; }

        /// <summary>
        /// Number of network retry attempts made.
        /// </summary>
        int NetworkRetryCount { get; set; }

        /// <summary>
        /// Number of HTTP retry attempts made.
        /// </summary>
        int HttpRetryCount { get; set; }

        /// <summary>
        /// Unique identifier for this request, used for correlation in logs.
        /// </summary>
        Guid RequestId { get; set; }
    }

}
