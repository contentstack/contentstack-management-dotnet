using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class LocaleService: ContentstackService
    {
        internal LocaleService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath = null)
           : base(serializer, stack: stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("Should have API Key to perform this operation.");
            }
            
            this.ResourcePath = resourcePath != null ? $"{resourcePath}/locales" : "locales";
            this.HttpMethod = "GET";
        }
    }
}
