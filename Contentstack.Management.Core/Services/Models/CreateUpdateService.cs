using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class CreateUpdateService<T> : ContentstackService
    {
        private readonly T _typedModel;
        private readonly string fieldName;
        #region Internal

        internal CreateUpdateService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, T dataModel, string fieldName, string httpMethod = "POST", ParameterCollection collection = null)
            : base(serializer, stack: stack, collection)
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
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);

                Serializer.Serialize(writer, _typedModel);
                string snippet = $"{{\"{fieldName}\": {stringWriter.ToString()}}}";
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet); 
            }
        }
    }
}
