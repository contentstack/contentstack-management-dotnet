using System;
using Newtonsoft.Json;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

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
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
            }
            this.ResourcePath = resourcePath;

            if (string.IsNullOrEmpty(stack.APIKey))
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }
        #endregion
    }
}
