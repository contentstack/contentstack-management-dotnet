using System;
using System.Collections.Generic;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services
{
    internal class DeleteReleaseItemService : ContentstackService
    {
        internal List<string> _items;

        internal DeleteReleaseItemService(Core.Models.Stack stack, string releaseUID, List<string> items, JsonSerializerOptions? stjOptions = null)
            : base(stjOptions ?? stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack: stack)
        {
            if (stack.APIKey == null)
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            if (releaseUID == null)
                throw new ArgumentNullException("releaseUID", CSConstants.ReleaseUIDRequired);
            if (items == null)
                throw new ArgumentNullException("items", CSConstants.ReleaseItemsRequired);

            this.ResourcePath = $"/releases/{releaseUID}/item";
            this.HttpMethod = "DELETE";
            _items = items;
        }

        public override void ContentBody()
        {
            var requestData = new Dictionary<string, object> { { "items", _items } };
            string jsonString = JsonSerializer.Serialize(requestData, SerializerOptions);
            this.ByteContent = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }
    }
}
