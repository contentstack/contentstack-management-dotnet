using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class ReleaseItem {

        internal Stack stack;
        internal string resourcePath;
        internal string releaseUID;

        internal ReleaseItem(Stack stack, string releaseUID)
        {
            this.stack = stack;
            this.releaseUID = releaseUID;
            resourcePath = $"/releases/{releaseUID}/items";
        }

        /// <summary>
        /// The Get all request retrieves a list of all items (entries and assets) that are part of a specific Release.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack().Relase(&quot;&lt;RELEASE_UID&gt;&quot;).Item().GetAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse GetAll(ParameterCollection parameters = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, this.stack, resourcePath, collection: parameters);

            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get all request retrieves a list of all items (entries and assets) that are part of a specific Release.
        /// </summary>
        /// <param name="collection">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack().Relase(&quot;&lt;RELEASE_UID&gt;&quot;).Item().GetAllAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> GetAllAsync(ParameterCollection parameters = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, this.stack, resourcePath, collection: parameters);

            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Create request allows you to add an item (entry or asset) to a Release.
        /// To add entries/assets to a Release, you need to provide the UIDs of the entries/assets in ‘items’ in the request body.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ReleaseItemModel model = new ReleaseItemModel();
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Relase(&quot;&lt;RELEASE_UID&gt;&quot;).Item().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">ReleaseItem Model for creating ReleaseItem.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Create(ReleaseItemModel model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<ReleaseItemModel>(stack.client.serializer, stack, resourcePath, model, "item", collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Create request allows you to add an item (entry or asset) to a Release.
        /// To add entries/assets to a Release, you need to provide the UIDs of the entries/assets in ‘items’ in the request body.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ReleaseItemModel model = new ReleaseItemModel();
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Relase(&quot;&lt;RELEASE_UID&gt;&quot;).Item().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">ReleaseItem Model for creating ReleaseItem.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> CreateAsync(ReleaseItemModel model, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            stack.ThrowIfNotLoggedIn();

            var service = new CreateUpdateService<ReleaseItemModel>(stack.client.serializer, stack, resourcePath, model, "item", collection: collection);

            return stack.client.InvokeAsync<CreateUpdateService<ReleaseItemModel>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Create request allows you to add multiple items (entries and/or assets) to a Release.
        /// To add entries/assets to a Release, you need to provide the UIDs of the entries/assets in ‘items’ in the request body.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ReleaseItemModel model = new ReleaseItemModel();
        /// List&lt;ReleaseItemModel&gt; models = new List&lt;ReleaseItemModel&gt;()
        /// {
        ///     model,
        /// };
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Relase(&quot;&lt;RELEASE_UID&gt;&quot;).Item().CreateMultiple(models);
        /// </code></pre>
        /// </example>
        /// <param name="model">ReleaseItem Model for creating ReleaseItem.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse CreateMultiple(List<ReleaseItemModel> models, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<List<ReleaseItemModel>>(stack.client.serializer, stack, resourcePath, models, "items", collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Create request allows you to add multiple items (entries and/or assets) to a Release.
        /// To add entries/assets to a Release, you need to provide the UIDs of the entries/assets in ‘items’ in the request body.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Relase(&quot;&lt;RELEASE_UID&gt;&quot;).Item().CreateMultipleAsync(models);
        /// ReleaseItemModel model = new ReleaseItemModel();
        /// List&lt;ReleaseItemModel&gt; models = new List&lt;ReleaseItemModel&gt;()
        /// {
        ///     model,
        /// };
        /// </code></pre>
        /// </example>
        /// <param name="model">ReleaseItem Model for creating ReleaseItem.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> CreateMultipleAsync(List<ReleaseItemModel> model, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            stack.ThrowIfNotLoggedIn();

            var service = new CreateUpdateService<List<ReleaseItemModel>>(stack.client.serializer, stack, resourcePath, model, "items", collection: collection);

            return stack.client.InvokeAsync<CreateUpdateService<List<ReleaseItemModel>>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Update Release items to their latest versions request let you update all the release items (entries and assets) to their latest versions before deployment.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// List&lt;string&gt; items = new List&lt;string&gt;(){
        /// &quot;&lt;$all&gt;&quot;
        /// }
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Release(&quot;&lt;RELEASE_UID&gt;&quot;).Item().UpdateReleaseItem(items);
        /// </code></pre>
        /// </example>
        /// <param name="items">Release items to update or &quot;$all&quot; for updating all release items</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse UpdateReleaseItem(List<string> items)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new CreateUpdateService<List<string>>(stack.client.serializer, stack, $"/releases/{releaseUID}/update_items", items, "items", "PUT");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Update Release items to their latest versions request let you update all the release items (entries and assets) to their latest versions before deployment.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// List&lt;string&gt; items = new List&lt;string&gt;(){
        /// &quot;&lt;$all&gt;&quot;
        /// }
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Release(&quot;&lt;RELEASE_UID&gt;&quot;).Item().UpdateReleaseItemAsync(items);
        /// </code></pre>
        /// </example>
        /// <param name="items">Release items to update or &quot;$all&quot; for updating all release items</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> UpdateReleaseItemAsync(List<string> items)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            var service = new CreateUpdateService<List<string>>(stack.client.serializer, stack, $"/releases/{releaseUID}/update_items", items, "items", "PUT");
            return stack.client.InvokeAsync<CreateUpdateService<List<string>>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Delete request deletes one or more items (entries and/or assets) from a specific Release.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ReleaseItemModel model = new ReleaseItemModel();
        /// List&lt;ReleaseItemModel&gt; models = new List&lt;ReleaseItemModel&gt;()
        /// {
        ///     model,
        /// };
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Relase(&quot;&lt;RELEASE_UID&gt;&quot;).Item(&quot;&lt;RELEASE_ITEM_UID&gt;&quot;).Delete(models);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Delete(List<ReleaseItemModel> models, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new DeleteService<List<ReleaseItemModel>>(stack.client.serializer, stack, resourcePath, "items", models, collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Delete request deletes one or more items (entries and/or assets) from a specific Release.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ReleaseItemModel model = new ReleaseItemModel();
        /// List&lt;ReleaseItemModel&gt; models = new List&lt;ReleaseItemModel&gt;()
        /// {
        ///     model,
        /// };
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Relase(&quot;&lt;RELEASE_UID&gt;&quot;).Item(&quot;&lt;RELEASE_ITEM_UID&gt;&quot;).DeleteAsync(models);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> DeleteAsync(List<ReleaseItemModel> models, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new DeleteService<List<ReleaseItemModel>>(stack.client.serializer, stack, resourcePath, "items", models, collection);

            return stack.client.InvokeAsync<DeleteService<List<ReleaseItemModel>>, ContentstackResponse>(service);
        }

        #region Throw Error

        internal void ThrowIfUidEmpty()
        {
            if (string.IsNullOrEmpty(this.releaseUID))
            {
                throw new InvalidOperationException("Uid can not be empty.");
            }
        }
        #endregion
    }
}
