using System;
using System.IO;
using Contentstack.Management.Core.Models;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class PublishUnpublishService : ContentstackService
    {
        internal string locale;
        internal string fieldName;
        internal PublishUnpublishDetails details;
        internal PublishUnpublishService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, PublishUnpublishDetails details, string resourcePath, string fieldName, string locale = null)
           : base(serializerOptions, stack: stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }

            if (details == null)
            {
                throw new ArgumentNullException("details", CSConstants.PublishDetailsRequired);
            }

            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
            }

            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName", CSConstants.FieldNameRequired);
            }

            this.ResourcePath = resourcePath;
            this.HttpMethod = "POST";
            this.details = details;
            this.locale = locale;
            this.fieldName = fieldName;
        }

        public override void ContentBody()
        {
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WritePropertyName(fieldName);
                writer.WriteStartObject();

                if (details.Locales != null && details.Locales.Count > 0)
                {
                    writer.WritePropertyName("locales");
                    writer.WriteStartArray();
                    foreach (string code in details.Locales)
                        writer.WriteStringValue(code);
                    writer.WriteEndArray();
                }

                if (details.Environments != null && details.Environments.Count > 0)
                {
                    writer.WritePropertyName("environments");
                    writer.WriteStartArray();
                    foreach (string environment in details.Environments)
                        writer.WriteStringValue(environment);
                    writer.WriteEndArray();
                }

                if (details.Variants != null && details.Variants.Count > 0)
                {
                    writer.WritePropertyName("variants");
                    writer.WriteStartArray();
                    foreach (var variant in details.Variants)
                    {
                        writer.WriteStartObject();
                        if (variant.Uid != null)
                            writer.WriteString("uid", variant.Uid);
                        if (variant.Version.HasValue)
                            writer.WriteNumber("version", variant.Version.Value);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }

                if (details.VariantRules != null)
                {
                    writer.WritePropertyName("variant_rules");
                    writer.WriteStartObject();
                    if (details.VariantRules.PublishLatestBase.HasValue)
                        writer.WriteBoolean("publish_latest_base", details.VariantRules.PublishLatestBase.Value);
                    if (details.VariantRules.PublishLatestBaseConditionally.HasValue)
                        writer.WriteBoolean("publish_latest_base_conditionally", details.VariantRules.PublishLatestBaseConditionally.Value);
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();

                if (details.Version != null)
                    writer.WriteNumber("version", details.Version.Value);

                if (!string.IsNullOrEmpty(locale))
                    writer.WriteString("locale", locale);

                if (!string.IsNullOrEmpty(details.ScheduledAt))
                    writer.WriteString("scheduled_at", details.ScheduledAt);

                writer.WriteEndObject();
            }

            ByteContent = ms.ToArray();
        }
    }
}
