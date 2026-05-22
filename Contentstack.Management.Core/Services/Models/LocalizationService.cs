using System;
using System.Collections.Generic;
using System.Text.Json;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class LocalizationService<T> : ContentstackService
    {
        private readonly T _typedModel;
        private readonly string _fieldName;
        private readonly bool _shouldUnlocalize;
        #region Internal

        internal LocalizationService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, string resourcePath, T dataModel, string fieldName, ParameterCollection collection = null, bool shouldUnlocalize = false)
            : base(serializerOptions, stack: stack, collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
            }
            if (!shouldUnlocalize && dataModel == null)
            {
                throw new ArgumentNullException("dataModel", CSConstants.DataModelRequired);
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName", CSConstants.FieldNameRequired);
            }
            this.ResourcePath = shouldUnlocalize ? $"{resourcePath}/unlocalize" : resourcePath;
            this.HttpMethod = shouldUnlocalize ? "POST": "PUT";
            _fieldName = fieldName;
            _shouldUnlocalize = shouldUnlocalize;
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
            _typedModel = dataModel;
        }
        #endregion

        public override void ContentBody()
        {
            if (!_shouldUnlocalize)
            {
                var wrapper = new Dictionary<string, T> { { _fieldName, _typedModel } };
                this.ByteContent = JsonSerializer.SerializeToUtf8Bytes(wrapper, SerializerOptions);
            }
        }
    }
}
