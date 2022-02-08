using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class PublishUnpublishService : ContentstackService
    {
        internal string locale;
        internal string fieldName;
        internal PublishUnpublishDetails details;
        internal PublishUnpublishService(JsonSerializer serializer, Core.Models.Stack stack, PublishUnpublishDetails details, string resourcePath, string fieldName, string locale = null)
           : base(serializer, stack: stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("Should have API Key to perform this operation.");
            }

            if (details == null)
            {
                throw new ArgumentNullException("Should publish details for service.");
            }

            if (resourcePath == null)
            {
                throw new ArgumentNullException("Should resource path for service.");
            }

            if (fieldName == null)
            {
                throw new ArgumentNullException("Should field name for service.");
            }

            this.ResourcePath = resourcePath;
            this.HttpMethod = "POST";
            this.details = details;
            this.locale = locale;
            this.fieldName = fieldName;
        }

        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName(fieldName);
                writer.WriteStartObject();
                
                if (details.Locales != null && details.Locales.Count > 0)
                {
                    writer.WritePropertyName("locales");
                    writer.WriteStartArray();
                    foreach (string locale in details.Locales)
                        writer.WriteValue(locale);

                    writer.WriteEndArray();
                }
                if (details.Environments != null && details.Environments.Count > 0)
                {
                    writer.WritePropertyName("environments");
                    writer.WriteStartArray();
                    foreach (string environment in details.Environments)
                        writer.WriteValue(environment);

                    writer.WriteEndArray();
                }
                writer.WriteEndObject();

                if (!string.IsNullOrEmpty(details.Version))
                {
                    writer.WritePropertyName("version");
                    writer.WriteValue(details.Version);

                }

                if (!string.IsNullOrEmpty(locale))
                {
                    writer.WritePropertyName("locale");
                    writer.WriteValue(locale);

                }
                if (!string.IsNullOrEmpty(details.Version))
                {
                    writer.WritePropertyName("scheduled_at");
                    writer.WriteValue(details.ScheduledAt);
                }
                writer.WriteEndObject();
                string snippet = stringWriter.ToString();
                ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
    }
}
