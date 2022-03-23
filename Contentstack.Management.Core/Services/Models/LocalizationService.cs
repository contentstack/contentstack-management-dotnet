using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class LocalizationService<T> : ContentstackService
    {
        private readonly T _typedModel;
        private readonly string _fieldName;
        private readonly bool _shouldUnlocalize;
        #region Internal

        internal LocalizationService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, T dataModel, string fieldName, ParameterCollection collection = null, bool shouldUnlocalize = false)
            : base(serializer, stack: stack, collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", "Should have API Key to perform this operation.");
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", "Should resource path for service.");
            }
            if (!shouldUnlocalize && dataModel == null)
            {
                throw new ArgumentNullException("dataModel", "Data model is mandatory for service");
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName", "Name mandatory for service");
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
                using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    JsonWriter writer = new JsonTextWriter(stringWriter);

                    Serializer.Serialize(writer, _typedModel);
                    string snippet = $"{{\"{_fieldName}\": {stringWriter.ToString()}}}";
                    this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
                }
            }
            
        }
    }
}
