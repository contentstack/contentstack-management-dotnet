using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class FetchDeleteService: ContentstackService
    {
        #region Internal

        internal FetchDeleteService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, string httpMethod = "GET", ParameterCollection collection = null)
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
            this.ResourcePath = resourcePath;
            this.HttpMethod = httpMethod;

            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }
        #endregion
    }
}
