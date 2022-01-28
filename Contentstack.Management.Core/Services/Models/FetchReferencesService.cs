using System;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Services.Models
{
    internal class FetchReferencesService : ContentstackService
    {
        internal FetchReferencesService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath = null)
           : base(serializer, stack: stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("Should have API Key to perform this operation.");
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("Should resource path for service.");
            }
            this.ResourcePath = $"{resourcePath}/references";
            this.HttpMethod = "GET";
        }
    }
}
