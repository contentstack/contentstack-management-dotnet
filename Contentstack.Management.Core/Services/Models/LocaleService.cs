using System;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class LocaleService: ContentstackService
    {
        internal LocaleService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, string? resourcePath = null)
           : base(serializerOptions, stack: stack)
        {
            if (stack!.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            
            this.ResourcePath = resourcePath != null ? $"{resourcePath}/locales" : "locales";
            this.HttpMethod = "GET";
        }
    }
}
