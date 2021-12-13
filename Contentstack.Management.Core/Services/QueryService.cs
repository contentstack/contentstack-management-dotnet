using System;
using Newtonsoft.Json;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Services
{
    internal class QueryService: ContentstackService
    {
        #region Internal

        internal QueryService(Core.Models.Stack stack, ParameterCollection collection, string resourcePath)
            : base(stack.client.serializer, stack, collection)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                throw new ArgumentNullException("resourcePath");
            }
            this.ResourcePath = resourcePath;

            if (string.IsNullOrEmpty(stack.APIKey))
            {
                throw new ArgumentNullException("Stack API Key");
            }
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }
        #endregion
    }
}
