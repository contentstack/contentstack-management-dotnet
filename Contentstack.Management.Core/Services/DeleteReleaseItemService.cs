using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Services
{
    internal class DeleteReleaseItemService : ContentstackService
    {
        #region Internal
        internal List<string> _items;

        internal DeleteReleaseItemService(JsonSerializer serializer, Core.Models.Stack stack, string releaseUID, List<string> items)
            : base(serializer, stack: stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", "Should have API Key to perform this operation.");
            }
            if (releaseUID == null)
            {
                throw new ArgumentNullException("releaseUID", "Should have release UID for service.");
            }
            if (items == null)
            {
                throw new ArgumentNullException("items", "Should release items for service.");
            }
            this.ResourcePath = $"/releases/{releaseUID}/item";
            this.HttpMethod = "DELETE";
            _items = items;
        }
        #endregion

        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);

                Serializer.Serialize(writer, _items);
                string snippet = $"{{\"items\": {stringWriter.ToString()}}}";
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
    }
}