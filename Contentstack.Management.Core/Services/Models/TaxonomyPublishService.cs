using System;
using System.Text.Json;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class TaxonomyPublishService : ContentstackService
    {
        private readonly TaxonomyPublishDetails _details;

        internal TaxonomyPublishService(Core.Models.Stack stack, TaxonomyPublishDetails details, string resourcePath, ParameterCollection? collection = null, JsonSerializerOptions? stjOptions = null)
            : base(stjOptions ?? stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack: stack, collection: collection)
        {
            if (stack!.APIKey == null)
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            if (details == null)
                throw new ArgumentNullException("details", CSConstants.PublishDetailsRequired);
            if (resourcePath == null)
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);

            this.ResourcePath = resourcePath;
            this.HttpMethod = "POST";
            _details = details;
        }

        public override void ContentBody()
        {
            string jsonString = JsonSerializer.Serialize(_details, SerializerOptions);
            this.ByteContent = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }
    }
}
