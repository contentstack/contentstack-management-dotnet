using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Term represents a single node in a taxonomy hierarchy. Terms can have parent/child relationships.
    /// </summary>
    public class Term : BaseModel<TermModel>
    {
        private readonly string _taxonomyUid;

        internal Term(Stack stack, string taxonomyUid, string termUid = null)
            : base(stack, "term", termUid)
        {
            _taxonomyUid = taxonomyUid ?? throw new ArgumentNullException(nameof(taxonomyUid));
            resourcePath = $"/taxonomies/{_taxonomyUid}/terms";
            if (!string.IsNullOrEmpty(termUid))
                resourcePath += $"/{termUid}";
        }

        /// <summary>
        /// Query terms in this taxonomy. Call only when no specific term UID is set (collection).
        /// </summary>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// Create a term in this taxonomy.
        /// </summary>
        public override ContentstackResponse Create(TermModel model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// Create a term asynchronously.
        /// </summary>
        public override Task<ContentstackResponse> CreateAsync(TermModel model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// Update an existing term.
        /// </summary>
        public override ContentstackResponse Update(TermModel model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// Update an existing term asynchronously.
        /// </summary>
        public override Task<ContentstackResponse> UpdateAsync(TermModel model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// Fetch a single term.
        /// </summary>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// Fetch a single term asynchronously.
        /// </summary>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// Delete a term.
        /// </summary>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// Delete a term asynchronously.
        /// </summary>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }

        /// <summary>
        /// Get ancestor terms of this term. GET {resourcePath}/ancestors.
        /// </summary>
        public ContentstackResponse Ancestors(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/ancestors", "GET", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Get ancestor terms asynchronously.
        /// </summary>
        public Task<ContentstackResponse> AncestorsAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/ancestors", "GET", collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Get descendant terms of this term. GET {resourcePath}/descendants.
        /// </summary>
        public ContentstackResponse Descendants(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/descendants", "GET", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Get descendant terms asynchronously.
        /// </summary>
        public Task<ContentstackResponse> DescendantsAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/descendants", "GET", collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Move term to a new parent and/or order. PUT {resourcePath}/move with body { term: moveModel }.
        /// </summary>
        public ContentstackResponse Move(TermMoveModel moveModel, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new CreateUpdateService<TermMoveModel>(stack.client.serializer, stack, resourcePath + "/move", moveModel, "term", "PUT", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Move term asynchronously.
        /// </summary>
        public Task<ContentstackResponse> MoveAsync(TermMoveModel moveModel, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new CreateUpdateService<TermMoveModel>(stack.client.serializer, stack, resourcePath + "/move", moveModel, "term", "PUT", collection);
            return stack.client.InvokeAsync<CreateUpdateService<TermMoveModel>, ContentstackResponse>(service);
        }

        /// <summary>
        /// Get term locales. GET {resourcePath}/locales.
        /// </summary>
        public ContentstackResponse Locales(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/locales", "GET", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Get term locales asynchronously.
        /// </summary>
        public Task<ContentstackResponse> LocalesAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath + "/locales", "GET", collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Localize term. POST to resourcePath with body { term: model } and query params (e.g. locale).
        /// </summary>
        public ContentstackResponse Localize(TermModel model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new CreateUpdateService<TermModel>(stack.client.serializer, stack, resourcePath, model, "term", "POST", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Localize term asynchronously.
        /// </summary>
        public Task<ContentstackResponse> LocalizeAsync(TermModel model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new CreateUpdateService<TermModel>(stack.client.serializer, stack, resourcePath, model, "term", "POST", collection);
            return stack.client.InvokeAsync<CreateUpdateService<TermModel>, ContentstackResponse>(service);
        }

        /// <summary>
        /// Search terms across all taxonomies. GET /taxonomies/$all/terms with typeahead query param. Callable only when no specific term UID is set.
        /// </summary>
        /// <param name="typeahead">Search string for typeahead.</param>
        /// <param name="collection">Optional additional query parameters.</param>
        public ContentstackResponse Search(string typeahead, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();
            var coll = collection ?? new ParameterCollection();
            coll.Add("typeahead", typeahead ?? string.Empty);
            var service = new FetchDeleteService(stack.client.serializer, stack, "/taxonomies/$all/terms", "GET", coll);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Search terms across all taxonomies asynchronously.
        /// </summary>
        public Task<ContentstackResponse> SearchAsync(string typeahead, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();
            var coll = collection ?? new ParameterCollection();
            coll.Add("typeahead", typeahead ?? string.Empty);
            var service = new FetchDeleteService(stack.client.serializer, stack, "/taxonomies/$all/terms", "GET", coll);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }
    }
}
