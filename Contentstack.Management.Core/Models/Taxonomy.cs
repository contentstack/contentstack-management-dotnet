using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Taxonomy allows you to organize and categorize content using a hierarchical structure of terms.
    /// </summary>
    public class Taxonomy : BaseModel<TaxonomyModel>
    {
        internal Taxonomy(Stack stack, string uid = null)
            : base(stack, "taxonomy", uid)
        {
            resourcePath = uid == null ? "/taxonomies" : $"/taxonomies/{uid}";
        }

        /// <summary>
        /// Query taxonomies. Fetches all taxonomies with optional filters.
        /// </summary>
        /// <example>
        /// <code>
        /// ContentstackResponse response = stack.Taxonomy().Query().Find();
        /// </code>
        /// </example>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// Create a taxonomy.
        /// </summary>
        public override ContentstackResponse Create(TaxonomyModel model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// Create a taxonomy asynchronously.
        /// </summary>
        public override Task<ContentstackResponse> CreateAsync(TaxonomyModel model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// Update an existing taxonomy.
        /// </summary>
        public override ContentstackResponse Update(TaxonomyModel model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// Update an existing taxonomy asynchronously.
        /// </summary>
        public override Task<ContentstackResponse> UpdateAsync(TaxonomyModel model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// Fetch a single taxonomy.
        /// </summary>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// Fetch a single taxonomy asynchronously.
        /// </summary>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// Delete a taxonomy.
        /// </summary>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// Delete a taxonomy asynchronously.
        /// </summary>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }

        /// <summary>
        /// Export taxonomy. GET {resourcePath}/export with optional query parameters.
        /// </summary>
        public ContentstackResponse Export(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/export", "GET", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Export taxonomy asynchronously.
        /// </summary>
        public Task<ContentstackResponse> ExportAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/export", "GET", collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Get taxonomy locales. GET {resourcePath}/locales.
        /// </summary>
        public ContentstackResponse Locales(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/locales", "GET", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Get taxonomy locales asynchronously.
        /// </summary>
        public Task<ContentstackResponse> LocalesAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/locales", "GET", collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Localize taxonomy. POST to resourcePath with body { taxonomy: model } and query params (e.g. locale).
        /// </summary>
        public ContentstackResponse Localize(TaxonomyModel model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new CreateUpdateService<TaxonomyModel>(stack.client.serializer, stack, resourcePath, model, "taxonomy", "POST", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Localize taxonomy asynchronously.
        /// </summary>
        public Task<ContentstackResponse> LocalizeAsync(TaxonomyModel model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new CreateUpdateService<TaxonomyModel>(stack.client.serializer, stack, resourcePath, model, "taxonomy", "POST", collection);
            return stack.client.InvokeAsync<CreateUpdateService<TaxonomyModel>, ContentstackResponse>(service);
        }

        /// <summary>
        /// Import taxonomy. POST /taxonomies/import with multipart form (taxonomy file).
        /// </summary>
        public ContentstackResponse Import(TaxonomyImportModel model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();
            var path = resourcePath + "/import";
            var service = new UploadService(stack.client.serializer, stack, path, model, "POST", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Import taxonomy asynchronously.
        /// </summary>
        public Task<ContentstackResponse> ImportAsync(TaxonomyImportModel model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();
            var path = resourcePath + "/import";
            var service = new UploadService(stack.client.serializer, stack, path, model, "POST", collection);
            return stack.client.InvokeAsync<UploadService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Get Terms instance for this taxonomy. When termUid is provided, returns a single-term context; otherwise collection for query/create.
        /// </summary>
        /// <param name="termUid">Optional term UID. If null, returns Terms for querying all terms or creating.</param>
        /// <example>
        /// <code>
        /// stack.Taxonomy("taxonomy_uid").Terms().Query().Find();
        /// stack.Taxonomy("taxonomy_uid").Terms("term_uid").Fetch();
        /// </code>
        /// </example>
        public Term Terms(string termUid = null)
        {
            ThrowIfUidEmpty();
            return new Term(stack, Uid, termUid);
        }
    }
}
