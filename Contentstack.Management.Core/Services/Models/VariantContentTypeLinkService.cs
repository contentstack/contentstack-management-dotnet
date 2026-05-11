using System;
using System.Collections.Generic;
using System.IO;
using Contentstack.Management.Core.Queryable;
using System.Text.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class VariantContentTypeLinkService : ContentstackService
    {
        private readonly List<string> _contentTypeUids;
        private readonly string _variantGroupUid;
        private readonly bool _isLink;

        internal VariantContentTypeLinkService(
            JsonSerializerOptions serializerOptions,
            Core.Models.Stack stack,
            string resourcePath,
            List<string> contentTypeUids,
            string variantGroupUid,
            bool isLink,
            ParameterCollection collection = null
        )
            : base(serializerOptions, stack: stack, collection: collection)
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
            this.HttpMethod = "PUT";
            _contentTypeUids = contentTypeUids;
            _variantGroupUid = variantGroupUid;
            _isLink = isLink;

            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }

        public override void ContentBody()
        {
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("content_types");
                writer.WriteStartArray();
                foreach (var uid in _contentTypeUids)
                {
                    writer.WriteStartObject();
                    writer.WriteString("uid", uid);
                    writer.WriteString("status", _isLink ? "linked" : "unlinked");
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteString("uid", _variantGroupUid);
                writer.WritePropertyName("branches");
                writer.WriteStartArray();
                writer.WriteStringValue("main");
                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }
    }
}
