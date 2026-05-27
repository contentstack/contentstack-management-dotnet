using System;
using System.Text.Json;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class FetchReferencesService : ContentstackService
    {
        internal FetchReferencesService(Core.Models.Stack stack, string? resourcePath = null, ParameterCollection? collection = null, JsonSerializerOptions? stjOptions = null)
           : base(stjOptions ?? stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack: stack, collection)
        {
            if (stack!.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
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
