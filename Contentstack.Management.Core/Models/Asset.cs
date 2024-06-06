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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Query().Find();
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Folder().Create();
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).Version().GetAll();
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// AssetModel model = new AssetModel(&quot;ASSET_NAME&quot;, &quot;FILE_PATH&quot;, &quot;FILE_CONTENT_TYPE&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Create(model);
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// AssetModel model = new AssetModel(&quot;ASSET_NAME&quot;, &quot;FILE_PATH&quot;, &quot;FILE_CONTENT_TYPE&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().CreateAsync(model);
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// AssetModel model = new AssetModel(&quot;ASSET_NAME&quot;, &quot;FILE_PATH&quot;, &quot;FILE_CONTENT_TYPE&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).Update(model);
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// AssetModel model = new AssetModel(&quot;ASSET_NAME&quot;, &quot;FILE_PATH&quot;, &quot;FILE_CONTENT_TYPE&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).UpdateAsync(model);
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).Fetch();
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).FetchAsync();
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).Delete();
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).DeleteAsync();
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
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).Publish(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Publish(PublishUnpublishDetails details)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/publish", "asset");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Publish an asset call is used to publish a specific version of an asset on the desired environment either immediately or at a later date/time.
        /// </summary>
        /// <param name="details">Publish details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).PublishAsync(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> PublishAsync(PublishUnpublishDetails details)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/publish", "asset");
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Unpublish an asset call is used to unpublish a specific version of an asset from a desired environment.
        /// </summary>
        /// <param name="details">Publish details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).Unpublish(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Unpublish(PublishUnpublishDetails details)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/unpublish", "asset");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Unpublish an asset call is used to unpublish a specific version of an asset from a desired environment.
        /// </summary>
        /// <param name="details">Publish details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset(&quot;&lt;ASSET_UID&gt;&quot;).UnpublishAsync(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> UnpublishAsync(PublishUnpublishDetails details)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/unpublish", "asset");
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service);
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

