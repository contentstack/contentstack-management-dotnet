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
        internal string contentTypeUid;
        internal string entryUid;
        internal string branchUid;

        /// <summary>
        /// Gets the UID of the variant.
        /// </summary>
        public string Uid { get; private set; }

        #region Constructor
        internal EntryVariant(Stack stack, string contentTypeUid, string entryUid, string uid = null, string branchUid = null)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack", "Stack cannot be null.");
            }

            stack.ThrowIfAPIKeyEmpty();

            this.stack = stack;
            this.Uid = uid;
            this.contentTypeUid = contentTypeUid;
            this.entryUid = entryUid;
            this.branchUid = branchUid;

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
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
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
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
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
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
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
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
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
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
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
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
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
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
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
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
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

        /// <summary>
        /// The Publish an entry variant request lets you publish an entry variant either immediately or schedule it for a later date/time.
        /// </summary>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <param name="apiVersion">API version</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Publish(PublishUnpublishDetails details, string locale = null, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            if (details != null)
            {
                if (details.Variants == null)
                {
                    details.Variants = new System.Collections.Generic.List<PublishVariant>();
                }

                if (!details.Variants.Exists(v => v.Uid == this.Uid))
                {
                    details.Variants.Add(new PublishVariant { Uid = this.Uid, Version = details.Version });
                }
            }

            string publishPath = $"/content_types/{this.contentTypeUid}/entries/{this.entryUid}/publish";
            var service = new PublishUnpublishService(stack.client.serializer, stack, details, publishPath, "entry", locale);
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
            return stack.client.InvokeSync(service, apiVersion: apiVersion);
        }

        /// <summary>
        /// The Publish an entry variant request lets you publish an entry variant either immediately or schedule it for a later date/time.
        /// </summary>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <param name="apiVersion">API version</param>
        /// <returns>The Task.</returns>
        public virtual Task<ContentstackResponse> PublishAsync(PublishUnpublishDetails details, string locale = null, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            if (details != null)
            {
                if (details.Variants == null)
                {
                    details.Variants = new System.Collections.Generic.List<PublishVariant>();
                }

                if (!details.Variants.Exists(v => v.Uid == this.Uid))
                {
                    details.Variants.Add(new PublishVariant { Uid = this.Uid, Version = details.Version });
                }
            }

            string publishPath = $"/content_types/{this.contentTypeUid}/entries/{this.entryUid}/publish";
            var service = new PublishUnpublishService(stack.client.serializer, stack, details, publishPath, "entry", locale);
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service, apiVersion: apiVersion);
        }

        /// <summary>
        /// The Unpublish an entry variant call will unpublish an entry variant at once, and also, gives you the provision to unpublish an entry variant automatically at a later date/time.
        /// </summary>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <param name="apiVersion">API version</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Unpublish(PublishUnpublishDetails details, string locale = null, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            if (details != null)
            {
                if (details.Variants == null)
                {
                    details.Variants = new System.Collections.Generic.List<PublishVariant>();
                }

                if (!details.Variants.Exists(v => v.Uid == this.Uid))
                {
                    details.Variants.Add(new PublishVariant { Uid = this.Uid, Version = details.Version });
                }
            }

            string unpublishPath = $"/content_types/{this.contentTypeUid}/entries/{this.entryUid}/unpublish";
            var service = new PublishUnpublishService(stack.client.serializer, stack, details, unpublishPath, "entry", locale);
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
            return stack.client.InvokeSync(service, apiVersion: apiVersion);
        }

        /// <summary>
        /// The Unpublish an entry variant call will unpublish an entry variant at once, and also, gives you the provision to unpublish an entry variant automatically at a later date/time.
        /// </summary>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <param name="apiVersion">API version</param>
        /// <returns>The Task.</returns>
        public virtual Task<ContentstackResponse> UnpublishAsync(PublishUnpublishDetails details, string locale = null, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            if (details != null)
            {
                if (details.Variants == null)
                {
                    details.Variants = new System.Collections.Generic.List<PublishVariant>();
                }

                if (!details.Variants.Exists(v => v.Uid == this.Uid))
                {
                    details.Variants.Add(new PublishVariant { Uid = this.Uid, Version = details.Version });
                }
            }

            string unpublishPath = $"/content_types/{this.contentTypeUid}/entries/{this.entryUid}/unpublish";
            var service = new PublishUnpublishService(stack.client.serializer, stack, details, unpublishPath, "entry", locale);
            if (!string.IsNullOrWhiteSpace(this.branchUid)) { service.Headers["branch"] = this.branchUid; }
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service, apiVersion: apiVersion);
        }
        #endregion
    }
}