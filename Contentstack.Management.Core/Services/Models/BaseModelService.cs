using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class BaseModelService: ContentstackService
    {
        #region Internal

        internal BaseModelService(JsonSerializer serializer, Core.Models.Stack stack, ParameterCollection collection)
            : base(serializer, stack: stack, collection)
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
