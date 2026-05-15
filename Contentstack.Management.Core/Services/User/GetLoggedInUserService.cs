using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.User
{
    internal class GetLoggedInUserService: ContentstackService
    {
        public GetLoggedInUserService(JsonSerializer serializer, ParameterCollection collection): base(serializer, collection: collection)
        {
            this.ResourcePath = "user";
        }
    }
}
