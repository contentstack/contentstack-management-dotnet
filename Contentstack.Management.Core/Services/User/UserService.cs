using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.User
{
    internal class UserService: ContentstackService
    {
       internal UserService(JsonSerializer serializer): base(serializer)
        {
            this.ResourcePath = "user";
        }
    }
}
