using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Contentstack.Management.Core.Abstractions;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class UploadService: ContentstackService
    {
        private readonly IUploadInterface _uploadInterface;
        
        internal UploadService(Core.Models.Stack stack, string resourcePath, IUploadInterface uploadInterface, string httpMethod = "POST", ParameterCollection collection = null, JsonSerializerOptions stjOptions = null)
            : base(stjOptions ?? stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack: stack, collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
            }
            if (uploadInterface == null)
            {
                throw new ArgumentNullException("uploadInterface", CSConstants.UploadContentRequired);
            }
            this.ResourcePath = resourcePath;
            this.HttpMethod = httpMethod;
            _uploadInterface = uploadInterface;
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }

        public override void ContentBody()
        {
            HttpContent content = _uploadInterface.GetHttpContent();

            Headers.Remove("Content-Type");
            this.Headers.Add("Content-Type", content.Headers.ContentType.ToString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(_uploadInterface.ContentType);
            this.Content = content;
        }
    }
}
