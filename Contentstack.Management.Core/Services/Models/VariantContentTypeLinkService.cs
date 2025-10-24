using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class VariantContentTypeLinkService : ContentstackService
    {
        private readonly List<string> _contentTypeUids;
        private readonly bool _isLink;

        internal VariantContentTypeLinkService(
            JsonSerializer serializer,
            Core.Models.Stack stack,
            string resourcePath,
            List<string> contentTypeUids,
            bool isLink,
            ParameterCollection collection = null
        )
            : base(serializer, stack: stack, collection: collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException(
                    "stack",
                    "Should have API Key to perform this operation."
                );
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException(
                    "resourcePath",
                    "Should have resource path for service."
                );
            }
            if (contentTypeUids == null || contentTypeUids.Count == 0)
            {
                throw new ArgumentNullException(
                    "contentTypeUids",
                    "Content type UIDs are required for this operation."
                );
            }

            this.ResourcePath = resourcePath;
            this.HttpMethod = "POST";
            _contentTypeUids = contentTypeUids;
            _isLink = isLink;

            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }

        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("content_types");
                writer.WriteStartArray();

                foreach (var uid in _contentTypeUids)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("uid");
                    writer.WriteValue(uid);
                    writer.WritePropertyName("status");
                    writer.WriteValue(_isLink ? "linked" : "unlinked");
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();

                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(stringWriter.ToString());
            }
        }
    }
}
