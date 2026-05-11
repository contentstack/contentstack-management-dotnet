using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Queryable;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class DeleteService<T>: ContentstackService
    {
        internal string fieldName;
        internal T model;

        internal DeleteService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, string resourcePath, string fieldName, T model, ParameterCollection collection = null)
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
            var inner = JsonSerializer.Serialize(model, SerializerOptions);
            var snippet = $"{{\"{fieldName}\": {inner}}}";
            this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
        }
    }
}
