using System;
using System.Collections.Generic;
using System.Text.Json;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class DeleteService<T>: ContentstackService
    {
        internal string fieldName;
        internal T model;

        internal DeleteService(Core.Models.Stack stack, string resourcePath, string fieldName, T model, ParameterCollection? collection = null, JsonSerializerOptions? stjOptions = null)
            : base(stjOptions ?? stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack: stack, collection: collection)
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
            var requestData = new Dictionary<string, object?>
            {
                { fieldName, model }
            };

            string jsonString = JsonSerializer.Serialize(requestData, SerializerOptions);
            this.ByteContent = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }
    }
}
