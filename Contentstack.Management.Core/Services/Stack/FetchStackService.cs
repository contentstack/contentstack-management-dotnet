using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class FetchStackService : ContentstackService
    {
        #region Internal

        internal FetchStackService(JsonSerializer serializer, Core.Models.Stack stack, ParameterCollection collection = null)
            : base(serializer, stack, collection)
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