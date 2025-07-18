using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

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
        internal GlobalFieldService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, ContentModelling dataModel, string fieldName, string apiVersion, string httpMethod = "POST", ParameterCollection collection = null)
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
            if (dataModel == null)
            {
                throw new ArgumentNullException("dataModel", "Data model is mandatory for service");
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName", "Name mandatory for service");
            }
            
            this.ResourcePath = resourcePath;
            this.HttpMethod = httpMethod;
            this.fieldName = fieldName;
            this._apiVersion = apiVersion;
            
            // Set api_version header if provided
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
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);

                Serializer.Serialize(writer, _typedModel);
                string snippet = $"{{\"{fieldName}\": {stringWriter.ToString()}}}";
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet); 
            }
        }

        /// <summary>
        /// Handles response processing with api_version header cleanup for GlobalField operations.
        /// This matches the JavaScript SDK parseData function where api_version is removed from stackHeaders.
        /// </summary>
        public override void OnResponse(IResponse httpResponse, ContentstackClientOptions config)
        {
            base.OnResponse(httpResponse, config);
            
            // Clean up api_version header after successful GlobalField operation 
            // (matching JavaScript SDK parseData function behavior)
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