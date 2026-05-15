using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents the Entry Variant sub-resource.
    /// </summary>
    public class EntryVariant
    {
        internal Stack stack;
        internal string resourcePath;

        /// <summary>
        /// Gets the UID of the variant.
        /// </summary>
        public string Uid { get; private set; }

        #region Constructor
        internal EntryVariant(Stack stack, string contentTypeUid, string entryUid, string uid = null)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack", "Stack cannot be null.");
            }

            stack.ThrowIfAPIKeyEmpty();

            this.stack = stack;
            this.Uid = uid;

            string basePath = $"/content_types/{contentTypeUid}/entries/{entryUid}/variants";
            this.resourcePath = uid == null ? basePath : $"{basePath}/{uid}";
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Finds all variants for an entry.
        /// </summary>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Find(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();

            var service = new QueryService(
                stack,
                collection ?? new ParameterCollection(),
                resourcePath
            );
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Finds all variants for an entry asynchronously.
        /// </summary>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> FindAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();

            var service = new QueryService(
                stack,
                collection ?? new ParameterCollection(),
                resourcePath
            );
            return stack.client.InvokeAsync<QueryService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Creates a variant for an entry.
        /// </summary>
        /// <param name="model">The variant entry data including _variant metadata.</param>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Create(object model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<object>(stack.client.serializer, stack, resourcePath, model, "entry", "PUT", collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Creates a variant for an entry asynchronously.
        /// </summary>
        /// <param name="model">The variant entry data including _variant metadata.</param>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> CreateAsync(object model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<object>(stack.client.serializer, stack, resourcePath, model, "entry", "PUT", collection: collection);
            return stack.client.InvokeAsync<CreateUpdateService<object>, ContentstackResponse>(service);
        }

        /// <summary>
        /// Updates a variant for an entry.
        /// </summary>
        /// <param name="model">The variant entry data including _variant metadata.</param>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Update(object model, ParameterCollection collection = null)
        {
            return Create(model, collection);
        }

        /// <summary>
        /// Updates a variant for an entry asynchronously.
        /// </summary>
        /// <param name="model">The variant entry data including _variant metadata.</param>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> UpdateAsync(object model, ParameterCollection collection = null)
        {
            return CreateAsync(model, collection);
        }

        /// <summary>
        /// Fetches a specific variant.
        /// </summary>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Fetches a specific variant asynchronously.
        /// </summary>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Deletes a specific variant.
        /// </summary>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Delete(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE", collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Deletes a specific variant asynchronously.
        /// </summary>
        /// <param name="collection">Query parameters.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE", collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        internal void ThrowIfUidNotEmpty()
        {
            if (!string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("Operation not allowed with a specified UID.");
            }
        }

        internal void ThrowIfUidEmpty()
        {
            if (string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("UID is required for this operation.");
            }
        }
        #endregion
    }
}