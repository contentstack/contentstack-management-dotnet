using System;
using System.Collections.Generic;
using System.Text.Json;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class CreateUpdateService<T> : ContentstackService
    {
        private readonly T _typedModel;
        private readonly string fieldName;
        #region Internal

        internal CreateUpdateService(Core.Models.Stack stack, string resourcePath, T dataModel, string fieldName, string httpMethod = "POST", ParameterCollection collection = null, JsonSerializerOptions stjOptions = null)
            : base(stjOptions ?? stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack: stack, collection: collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
            }
            if (dataModel == null)
            {
                throw new ArgumentNullException("dataModel", CSConstants.DataModelRequired);
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName", CSConstants.FieldNameRequired);
            }
            this.ResourcePath = resourcePath;
            this.HttpMethod = httpMethod;
            this.fieldName = fieldName;
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
            _typedModel = dataModel;
        }
        #endregion

        public override void ContentBody()
        {
            var requestData = new Dictionary<string, object>
            {
                { fieldName, _typedModel }
            };

            string jsonString = JsonSerializer.Serialize(requestData, SerializerOptions);
            this.ByteContent = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }
    }
}
