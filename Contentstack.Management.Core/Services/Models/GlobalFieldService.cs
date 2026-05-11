using System;
using System.Text;
using System.Text.Json;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    /// <summary>
    /// Specialized service for GlobalField Create/Update operations that handles api_version header cleanup.
    /// </summary>
    internal class GlobalFieldService : ContentstackService
    {
        private readonly ContentModelling _typedModel;
        private readonly string fieldName;
        private readonly string _apiVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalFieldService"/> class.
        /// </summary>
        internal GlobalFieldService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, string resourcePath, ContentModelling dataModel, string fieldName, string apiVersion, string httpMethod = "POST", ParameterCollection collection = null)
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
            this._apiVersion = apiVersion;
            
            if (!string.IsNullOrEmpty(apiVersion))
            {
                this.Headers["api_version"] = apiVersion;
            }
            
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
            _typedModel = dataModel;
        }

        public override void ContentBody()
        {
            var inner = JsonSerializer.Serialize(_typedModel, SerializerOptions);
            var snippet = $"{{\"{fieldName}\": {inner}}}";
            this.ByteContent = Encoding.UTF8.GetBytes(snippet);
        }

        public override void OnResponse(IResponse httpResponse, ContentstackClientOptions config)
        {
            base.OnResponse(httpResponse, config);
            
            if (httpResponse != null && httpResponse.IsSuccessStatusCode && !string.IsNullOrEmpty(_apiVersion))
            {
                if (Headers.ContainsKey("api_version"))
                {
                    Headers.Remove("api_version");
                }
            }
        }
    }
}
