using System;
using Contentstack.Management.Core.Queryable;
using System.Text.Json;

namespace Contentstack.Management.Core.Services.User
{
    internal class GetLoggedInUserService: ContentstackService
    {
        public GetLoggedInUserService(JsonSerializerOptions serializerOptions, ParameterCollection collection): base(serializerOptions, collection: collection)
        {
            this.ResourcePath = "user";
        }
    }
}
