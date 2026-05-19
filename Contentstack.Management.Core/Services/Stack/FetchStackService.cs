using System;
using Contentstack.Management.Core.Queryable;
using System.Text.Json;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class FetchStackService : ContentstackService
    {
        #region Internal

        internal FetchStackService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, ParameterCollection collection = null)
            : base(serializerOptions, stack, collection)
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