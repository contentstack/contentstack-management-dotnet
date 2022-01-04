using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class FetchDeleteService: ContentstackService
    {
        #region Internal

        internal FetchDeleteService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, string httpMethod = "GET")
            : base(serializer, stack: stack)
        {
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
