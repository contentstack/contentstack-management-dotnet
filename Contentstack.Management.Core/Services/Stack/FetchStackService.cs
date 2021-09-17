using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class FetchStackService : ContentstackService
    {
        #region Internal

        internal FetchStackService(JsonSerializer serializer, ParameterCollection collection, string apiKey = null)
            : base(serializer, collection, apiKey: apiKey)
        {
            this.ResourcePath = "stacks";

            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }
        #endregion
    }
}