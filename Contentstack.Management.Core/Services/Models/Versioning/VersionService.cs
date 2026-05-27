using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models.Versioning
{
    internal class VersionService : ContentstackService
    {
        #region Internal
        internal string fieldName = null!;

        internal string? name;
        internal string? locale;
        internal bool force;

        internal VersionService(Core.Models.Stack stack, string resourcePath, string httpMethod, string fieldName, ParameterCollection? collection = null, JsonSerializerOptions? stjOptions = null)
            : base(stjOptions ?? stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack: stack, collection)
        {
            if (stack!.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName", CSConstants.FieldNameRequired);
            }
            this.fieldName = fieldName;
            this.ResourcePath = resourcePath;
            this.HttpMethod = httpMethod;
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }

        #endregion

        public override void ContentBody()
        {
            if (HttpMethod != "GET")
            {
                using var stream = new MemoryStream();
                using var writer = new Utf8JsonWriter(stream);
                
                writer.WriteStartObject();
                writer.WritePropertyName(fieldName);
                writer.WriteStartObject();
                
                if (name != null)
                {
                    writer.WritePropertyName("_version_name");
                    writer.WriteStringValue(name);
                }

                if (locale != null)
                {
                    writer.WritePropertyName("locale");
                    writer.WriteStringValue(locale);
                }

                if (force)
                {
                    writer.WritePropertyName("force");
                    writer.WriteBooleanValue(force);
                }
                
                writer.WriteEndObject();
                writer.WriteEndObject();
                writer.Flush();

                this.ByteContent = stream.ToArray();
            }
        }
    }
}