using System;
using System.IO;
using System.Net.Http;
using Contentstack.Management.Core.Abstractions;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class UploadService: ContentstackService
    {
        private readonly IUploadInterface _uploadInterface;
        
        internal UploadService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, IUploadInterface uploadInterface, string httpMethod = "POST")
            : base(serializer, stack: stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", "Should have API Key to perform this operation.");
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", "Should have resource path for service.");
            }
            if (uploadInterface == null)
            {
                throw new ArgumentNullException("uploadInterface", "Should have multipart content for service.");
            }
            this.ResourcePath = resourcePath;
            this.HttpMethod = httpMethod;
            _uploadInterface = uploadInterface;
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
