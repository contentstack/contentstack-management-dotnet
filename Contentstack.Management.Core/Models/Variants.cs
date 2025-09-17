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
        /// The Delete call is used to delete a specific variant.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Variants("<VARIANT_UID>").Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/> containing the deletion result.</returns>
        public ContentstackResponse Delete()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The DeleteAsync call is used to asynchronously delete a specific variant.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Variants("<VARIANT_UID>").DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task containing <see cref="ContentstackResponse"/> with the deletion result.</returns>
        public Task<ContentstackResponse> DeleteAsync()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE");
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service, true);
        }

        /// <summary>
        /// The Fetch call retrieves a specific variant by UID.
        /// </summary>
        /// <param name="collection">Optional query parameters.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Variants("<VARIANT_UID>").Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/> containing the variant data.</returns>
        public ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The FetchAsync call asynchronously retrieves a specific variant by UID.
        /// </summary>
        /// <param name="collection">Optional query parameters.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Variants("<VARIANT_UID>").FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task containing <see cref="ContentstackResponse"/> with the variant data.</returns>
        public Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }
        
        /// <summary>
        /// The Create call is used to create an entry variant in the stack.
        /// </summary>
        /// <param name="model">The <see cref="VariantsModel"/> containing variant details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// VariantsModel model = new VariantsModel(); 
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Variants().Create(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/> containing the created variant data.</returns>
        public ContentstackResponse Create(VariantsModel model)
        {
            ThrowIfUidNotEmpty();

            var service = new CreateUpdateService<VariantsModel>(stack.client.serializer, stack, resourcePath, model, "entry");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The CreateAsync call is used to asynchronously create an entry variant in the stack.
        /// </summary>
        /// <param name="model">The <see cref="VariantsModel"/> containing variant details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// VariantsModel model = new VariantsModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Variants().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <returns>The Task containing <see cref="ContentstackResponse"/> with the created variant data.</returns>
        public Task<ContentstackResponse> CreateAsync(VariantsModel model)
        {
            ThrowIfUidNotEmpty();
            stack.ThrowIfNotLoggedIn();

            var service = new CreateUpdateService<VariantsModel>(stack.client.serializer, stack, resourcePath, model, "entry");
            return stack.client.InvokeAsync<CreateUpdateService<VariantsModel>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The FetchByUid call retrieves multiple variants by passing an array of their UIDs.
        /// This method allows you to fetch multiple variants in a single API call.
        /// </summary>
        /// <param name="uids">Array of variant UIDs to fetch. Cannot be null or empty.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// string[] variantUids = {"bltvariant123", "bltvariant456", "bltvariant789"};
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Variants().FetchByUid(variantUids);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/> containing the requested variants data.</returns>
        public ContentstackResponse FetchByUid(string[] uids)
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
        /// The FetchByUidAsync call asynchronously retrieves multiple variants by passing an array of their UIDs.
        /// This method allows you to fetch multiple variants in a single API call asynchronously.
        /// </summary>
        /// <param name="uids">Array of variant UIDs to fetch. Cannot be null or empty.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// string[] variantUids = {"bltvariant123", "bltvariant456", "bltvariant789"};
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Variants().FetchByUidAsync(variantUids);
        /// </code></pre>
        /// </example>
        /// <returns>The Task containing <see cref="ContentstackResponse"/> with the requested variants data.</returns>
        public Task<ContentstackResponse> FetchByUidAsync(string[] uids)
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
