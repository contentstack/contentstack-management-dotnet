using System;
using System.IO;
using Contentstack.Management.Core.Queryable;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models.Versioning
{
    internal class VersionService : ContentstackService
    {
        #region Internal
        internal string fieldName;

        internal string name;
        internal string locale;
        internal bool force;

        internal VersionService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, string resourcePath, string httpMethod, string fieldName, ParameterCollection collection = null)
            : base(serializerOptions, stack: stack, collection: collection)
        {
            if (stack.APIKey == null)
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
                using var ms = new MemoryStream();
                using (var writer = new Utf8JsonWriter(ms))
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(fieldName);
                    writer.WriteStartObject();
                    if (name != null)
                        writer.WriteString("_version_name", name);
                    if (locale != null)
                        writer.WriteString("locale", locale);
                    if (force)
                        writer.WriteBoolean("force", force);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                this.ByteContent = ms.ToArray();
            }
        }
    }
}