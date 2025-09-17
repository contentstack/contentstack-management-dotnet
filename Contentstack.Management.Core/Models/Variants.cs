using System;
using System.Linq;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Models
{
    public class Variants
    {
        internal Stack stack;
        public string Uid { get; set; }

        internal string resourcePath;

        internal Variants(Stack stack, string uid = null)
        {
            stack.ThrowIfAPIKeyEmpty();

            this.stack = stack;
            Uid = uid;
            resourcePath = uid == null ? "/variants" : $"/variants/{uid}";
        }

        /// <summary>
        /// The Delete variants call is used to delete a specific variants.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Variants("<Variants_UID>").Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Delete()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Delete variants call is used to delete a specific variants.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Variants("<Variants_UID>").DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> DeleteAsync()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE");
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service, true);
        }

        /// <summary>
        /// Retrieves a specific variant by UID.
        /// </summary>
        /// <param name="collection">Optional query parameters.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Variants("<Variants_UID>").Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>Variant data in <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Asynchronously retrieves a specific variant by UID.
        /// </summary>
        /// <param name="collection">Optional query parameters.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Variants("<Variants_UID>").FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>Task containing variant data in <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }
        
        /// <summary>
        /// The Create is used to create an entry variant in the stack.
        /// </summary>
        /// <param name="model"><see cref="VariantsModel"/> with details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// VariantsModel model = new VariantsModel(); 
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Variants().Create(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Create(VariantsModel model)
        {
            ThrowIfUidNotEmpty();

            var service = new CreateUpdateService<VariantsModel>(stack.client.serializer, stack, resourcePath, model, "entry");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Create is used to create an entry variant in the stack.
        /// </summary>
        /// <param name="model"><see cref="VariantsModel"/> with details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// VariantsModel model = new VariantsModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Variants().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public virtual Task<ContentstackResponse> CreateAsync(VariantsModel model)
        {
            ThrowIfUidNotEmpty();
            stack.ThrowIfNotLoggedIn();

            var service = new CreateUpdateService<VariantsModel>(stack.client.serializer, stack, resourcePath, model, "entry");
            return stack.client.InvokeAsync<CreateUpdateService<VariantsModel>, ContentstackResponse>(service);
        }

        /// <summary>
        /// Retrieves multiple variants by their UIDs.
        /// </summary>
        /// <param name="uids">Array of variant UIDs to fetch.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// string[] variantUids = {"uid1", "uid2", "uid3"};
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Variants().FetchByUid(variantUids);
        /// </code></pre>
        /// </example>
        /// <returns>Variants data in <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse FetchByUid(string[] uids)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();

            if (uids == null || uids.Length == 0)
            {
                throw new ArgumentException("UIDs array cannot be null or empty.", nameof(uids));
            }

            var collection = new ParameterCollection();
            collection.Add("uid", uids.ToList());

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Asynchronously retrieves multiple variants by their UIDs.
        /// </summary>
        /// <param name="uids">Array of variant UIDs to fetch.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// string[] variantUids = {"uid1", "uid2", "uid3"};
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Variants().FetchByUidAsync(variantUids);
        /// </code></pre>
        /// </example>
        /// <returns>Task containing variants data in <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> FetchByUidAsync(string[] uids)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();

            if (uids == null || uids.Length == 0)
            {
                throw new ArgumentException("UIDs array cannot be null or empty.", nameof(uids));
            }

            var collection = new ParameterCollection();
            collection.Add("uid", uids.ToList());

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        #region Internal Validation

        /// <summary>Validates no UID is set for collection operations.</summary>
        internal void ThrowIfUidNotEmpty()
        {
            if (!string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("Operation not allowed.");
            }
        }

        /// <summary>Validates UID is set for specific variant operations.</summary>
        internal void ThrowIfUidEmpty()
        {
            if (string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("Uid can not be empty.");
            }
        }
        #endregion
    }
}
