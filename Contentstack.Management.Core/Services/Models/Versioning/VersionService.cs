using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models.Versioning
{
    internal class VersionService : ContentstackService
    {
        #region Internal
        internal string fieldName;

        internal string name;
        internal string locale;
        internal bool force;

        internal VersionService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, string httpMethod, string fieldName, ParameterCollection collection = null)
            : base(serializer, stack: stack, collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("Should have API Key to perform this operation.");
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("Should resource path for service.");
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("Should resource path for service.");
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
                using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    JsonWriter writer = new JsonTextWriter(stringWriter);
                    writer.WriteStartObject();
                    writer.WritePropertyName(fieldName);
                    writer.WriteStartObject();
                    if (name != null)
                    {
                        writer.WritePropertyName("_version_name");
                        writer.WriteValue(name);
                    }

                    if (locale != null)
                    {
                        writer.WritePropertyName("locale");
                        writer.WriteValue(locale);
                    }

                    if (force == true)
                    {
                        writer.WritePropertyName("force");
                        writer.WriteValue(force);
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                    string snippet = stringWriter.ToString();
                    this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
                }
            }
            
        }
    }
}