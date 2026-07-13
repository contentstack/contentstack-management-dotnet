using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class PublishUnpublishService : ContentstackService
    {
        internal string? locale;
        internal string fieldName = null!;
        internal PublishUnpublishDetails details = null!;
        internal PublishUnpublishService(Core.Models.Stack stack, PublishUnpublishDetails details, string resourcePath, string fieldName, string? locale = null, JsonSerializerOptions? stjOptions = null)
           : base(stjOptions ?? stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack: stack)
        {
            if (stack!.APIKey == null)
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
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            
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
                    {
                        writer.WritePropertyName("uid");
                        writer.WriteStringValue(variant.Uid);
                    }
                    if (variant.Version.HasValue)
                    {
                        writer.WritePropertyName("version");
                        writer.WriteNumberValue(variant.Version.Value);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }

            if (details.VariantRules != null)
            {
                writer.WritePropertyName("variant_rules");
                writer.WriteStartObject();
                if (details.VariantRules.PublishLatestBase.HasValue)
                {
                    writer.WritePropertyName("publish_latest_base");
                    writer.WriteBooleanValue(details.VariantRules.PublishLatestBase.Value);
                }
                if (details.VariantRules.PublishLatestBaseConditionally.HasValue)
                {
                    writer.WritePropertyName("publish_latest_base_conditionally");
                    writer.WriteBooleanValue(details.VariantRules.PublishLatestBaseConditionally.Value);
                }
                writer.WriteEndObject();
            }

            writer.WriteEndObject();

            if (details.Version != null)
            {
                writer.WritePropertyName("version");
                writer.WriteNumberValue(details.Version.Value);
            }

            if (!string.IsNullOrEmpty(locale))
            {
                writer.WritePropertyName("locale");
                writer.WriteStringValue(locale);
            }
            
            if (!string.IsNullOrEmpty(details.ScheduledAt))
            {
                writer.WritePropertyName("scheduled_at");
                writer.WriteStringValue(details.ScheduledAt);
            }
            
            writer.WriteEndObject();
            writer.Flush();

            ByteContent = stream.ToArray();
        }
    }
}
