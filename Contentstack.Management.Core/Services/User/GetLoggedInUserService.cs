using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.User
{
    internal class GetLoggedInUserService: ContentstackService
    {
        public GetLoggedInUserService(JsonSerializer serializer): base(serializer)
        {
            this.ResourcePath = "user";
        }
    }
}
