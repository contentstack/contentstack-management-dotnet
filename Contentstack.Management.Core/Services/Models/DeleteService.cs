using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class DeleteService<T>: ContentstackService
    {
        internal string fieldName;
        internal T model;

        internal DeleteService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, string fieldName, T model, ParameterCollection collection = null)
            : base(serializer, stack: stack, collection: collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", "Should have API Key to perform this operation.");
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", "Should have resource path for service.");
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName", "Should have field name for service.");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model", "Should have model for service.");
            }
            this.ResourcePath = resourcePath;
            this.HttpMethod = "DELETE";
            this.UseQueryString = true;
            this.fieldName = fieldName;
            this.model = model;
        }

        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);

                Serializer.Serialize(writer, model);
                string snippet = $"{{\"{fieldName}\": {stringWriter.ToString()}}}";
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
    }
}
