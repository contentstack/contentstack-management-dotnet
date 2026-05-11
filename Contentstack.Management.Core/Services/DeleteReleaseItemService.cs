using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Queryable;
using System.Text.Json;
using Contentstack.Management.Core.Utils;
namespace Contentstack.Management.Core.Services
{
    internal class DeleteReleaseItemService : ContentstackService
    {
        #region Internal
        internal List<string> _items;

        internal DeleteReleaseItemService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, string releaseUID, List<string> items)
            : base(serializerOptions, stack: stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (releaseUID == null)
            {
                throw new ArgumentNullException("releaseUID", CSConstants.ReleaseUIDRequired);
            }
            if (items == null)
            {
                throw new ArgumentNullException("items", CSConstants.ReleaseItemsRequired);
            }
            this.ResourcePath = $"/releases/{releaseUID}/item";
            this.HttpMethod = "DELETE";
            _items = items;
        }
        #endregion

        public override void ContentBody()
        {
            var inner = JsonSerializer.Serialize(_items, SerializerOptions);
            var snippet = $"{{\"items\": {inner}}}";
            this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
        }
    }
}