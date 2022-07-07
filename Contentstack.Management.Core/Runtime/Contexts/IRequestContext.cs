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
    }

}
