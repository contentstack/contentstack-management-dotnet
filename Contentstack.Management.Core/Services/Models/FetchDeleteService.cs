using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;

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
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
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
