using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
using System.Text.Json;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services.User
{
    internal class GetLoggedInUserService: ContentstackService
    {
        public GetLoggedInUserService(Newtonsoft.Json.JsonSerializer serializer, ParameterCollection collection, JsonSerializerOptions stjOptions = null, SerializationMode serializationMode = SerializationMode.Newtonsoft): base(serializer, collection: collection, stjOptions: stjOptions, serializationMode: serializationMode)
        {
            this.ResourcePath = "user";
        }
    }
}
