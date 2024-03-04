using System;
using Newtonsoft.Json;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Services.Models
{
    internal class FetchReferencesService : ContentstackService
    {
        internal FetchReferencesService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath = null, ParameterCollection collection = null)
           : base(serializer, stack: stack, collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", "Should have API Key to perform this operation.");
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", "Should resource path for service.");
            }
            this.ResourcePath = $"{resourcePath}/references";
            this.HttpMethod = "GET";

            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }
    }
}
