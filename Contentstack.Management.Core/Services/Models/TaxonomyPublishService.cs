using System;
using System.Text;
using System.Text.Json;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    /// <summary>
    /// Service for taxonomy publish/unpublish. Serializes the model directly as the root body
    /// without a field-name wrapper (CreateUpdateService always wraps as {"field": model}).
    /// </summary>
    internal class TaxonomyPublishService<T> : ContentstackService
    {
        private readonly T _model;

        internal TaxonomyPublishService(Core.Models.Stack stack, string resourcePath, T model, ParameterCollection? collection = null)
            : base(stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack: stack, collection: collection)
        {
            if (stack!.APIKey == null)
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            if (resourcePath == null)
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
            if (model == null)
                throw new ArgumentNullException("model", CSConstants.DataModelRequired);

            ResourcePath = resourcePath;
            HttpMethod = "POST";
            if (collection != null && collection.Count > 0)
                UseQueryString = true;
            _model = model;
        }

        public override void ContentBody()
        {
            ByteContent = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_model, SerializerOptions));
        }
    }
}
