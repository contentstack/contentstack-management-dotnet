using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;

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
            if (model == null)
            {
                throw new ArgumentNullException("model", CSConstants.ModelRequired);
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
