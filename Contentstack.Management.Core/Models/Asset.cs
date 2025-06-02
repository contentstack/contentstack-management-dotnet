using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class Asset
    {
        internal Stack stack;
        public string Uid { get; set; }

        internal string resourcePath;

        internal Asset(Stack stack, string uid = null)
        {
            stack.ThrowIfAPIKeyEmpty();
            
            this.stack = stack;
            Uid = uid;
            resourcePath = uid == null ? "/assets" : $"/assets/{uid}";
        }

        /// <summary>
        /// The Query on Asset will allow to fetch details of all Assets.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Queryable.Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Folder allows to fetch and create folders in assets.
        /// </summary>
        /// <param name="uid"></param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset().Folder().Create();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Models.Folder"/></returns>
        public Folder Folder(string uid = null)
        {
            ThrowIfUidNotEmpty();
            return new Folder(stack, uid);
        }

        /// <summary>
        /// The Versioning on Asset will allow to fetch all version, delete specific version or naming the asset version.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset("<ASSET_UID>").Version().GetAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Models.Version"/></returns>
        public Version Version(int? versionNumber = null)
        {
            ThrowIfUidEmpty();
            return new Version(stack, resourcePath, "asset", versionNumber);
        }
        /// <summary>
        /// The Upload asset request uploads an asset file to your stack.
        /// </summary>
        /// <param name="model">Asset Model with details.</param>
        /// <param name="collection">Query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// AssetModel model = new AssetModel("ASSET_NAME", "FILE_PATH", "FILE_CONTENT_TYPE");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset().Create(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Create(AssetModel model, ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Upload asset request uploads an asset file to your stack.
        /// </summary>
        /// <param name="model">Asset Model with details.</param>
        /// <param name="collection">Query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// AssetModel model = new AssetModel("ASSET_NAME", "FILE_PATH", "FILE_CONTENT_TYPE");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Asset().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public virtual Task<ContentstackResponse> CreateAsync(AssetModel model, ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();
            stack.ThrowIfNotLoggedIn();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model);
            return stack.client.InvokeAsync<UploadService, ContentstackResponse>(service, true);
        }

        /// <summary>
        /// The Replace asset call will replace an existing asset with another file on the stack.
        /// </summary>
        /// <param name="model">Asset Model with details.</param>
        /// <param name="collection">Query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// AssetModel model = new AssetModel("ASSET_NAME", "FILE_PATH", "FILE_CONTENT_TYPE");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset("<ASSET_UID>").Update(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Update(AssetModel model, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model, "PUT");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Replace asset call will replace an existing asset with another file on the stack.
        /// </summary>
        /// <param name="model">Asset Model with details.</param>
        /// <param name="collection">Query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// AssetModel model = new AssetModel("ASSET_NAME", "FILE_PATH", "FILE_CONTENT_TYPE");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Asset("<ASSET_UID>").UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> UpdateAsync(AssetModel model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model, "PUT");
            return stack.client.InvokeAsync<UploadService, ContentstackResponse>(service, true);
        }

        /// <summary>
        /// The Get an asset call returns comprehensive information about a specific version of an asset of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset("<ASSET_UID>").Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get an asset call returns comprehensive information about a specific version of an asset of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Asset("<ASSET_UID>").FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service, true);
        }

        /// <summary>
        /// The Delete asset call will delete an existing asset from the stack
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset("<ASSET_UID>").Delete();
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
        /// The Delete asset call will delete an existing asset from the stack
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Asset("<ASSET_UID>").DeleteAsync();
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
        /// The Publish an asset call is used to publish a specific version of an asset on the desired environment either immediately or at a later date/time.
        /// </summary>
        /// <param name="details">Publish details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset("<ASSET_UID>").Publish(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Publish(PublishUnpublishDetails details, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/publish", "asset", apiVersion);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Publish an asset call is used to publish a specific version of an asset on the desired environment either immediately or at a later date/time.
        /// </summary>
        /// <param name="details">Publish details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Asset("<ASSET_UID>").PublishAsync(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> PublishAsync(PublishUnpublishDetails details, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/publish", "asset", apiVersion);
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Unpublish an asset call is used to unpublish a specific version of an asset from a desired environment.
        /// </summary>
        /// <param name="details">Publish details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset("<ASSET_UID>").Unpublish(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Unpublish(PublishUnpublishDetails details, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/unpublish", "asset", apiVersion);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Unpublish an asset call is used to unpublish a specific version of an asset from a desired environment.
        /// </summary>
        /// <param name="details">Publish details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Asset("<ASSET_UID>").UnpublishAsync(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> UnpublishAsync(PublishUnpublishDetails details, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/unpublish", "asset", apiVersion);
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The References request returns the details of the entries and the content types in which the specified asset is referenced.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Asset("<ASSET_UID>").References();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse References(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchReferencesService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The ReferencesAsync request returns the details of the entries and the content types in which the specified asset is referenced.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Asset("<ASSET_UID>").ReferencesAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> ReferencesAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchReferencesService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchReferencesService, ContentstackResponse>(service);
        }

        #region Throw Error

        internal void ThrowIfUidNotEmpty()
        {
            if (!string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("Operation not allowed.");
            }
        }

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

