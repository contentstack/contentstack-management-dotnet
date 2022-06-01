using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Abstractions;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class Extension
    {
        internal Stack stack;
        public string Uid { get; set; }

        internal string resourcePath;

        internal Extension(Stack stack, string uid = null)
        {
            stack.ThrowIfAPIKeyEmpty();

            this.stack = stack;
            Uid = uid;
            resourcePath = uid == null ? "/assets" : $"/assets/{uid}";
        }

        /// <summary>
        /// The Query on Extension will allow to fetch details of all Extensions.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Queryable.Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Upload request is used to upload a new custom-field, custom-widget, dashboard widget to the Stack.
        /// </summary>
        /// <param name="model"> <see cref="Abstractions.IExtensionInterface"/> Model with details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// CustomField model = new CustomField(&quot;FILE_PATH&quot;, &quot;FILE_CONTENT_TYPE&quot;, &quot;TITLE&quot;, &quot;DATA_TYPE&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension().Upload(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Upload(IExtensionInterface model)
        {
            ThrowIfUidNotEmpty();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Upload request is used to upload a new custom-field, custom-widget, dashboard widget to the Stack.
        /// </summary>
        /// <param name="model"><see cref="Abstractions.IExtensionInterface"/> with details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// CustomFieldModel model = new CustomFieldModel(&quot;FILE_PATH&quot;, &quot;FILE_CONTENT_TYPE&quot;, &quot;TITLE&quot;, &quot;DATA_TYPE&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension().UploadAsync(model);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public virtual Task<ContentstackResponse> UploadAsync(IExtensionInterface model)
        {
            ThrowIfUidNotEmpty();
            stack.ThrowIfNotLoggedIn();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model);
            return stack.client.InvokeAsync<UploadService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Create is used to create a custom field, custom-widget, dashboard widget to the Stack.
        /// </summary>
        /// <param name="model"><see cref="ExtensionModel"/> with details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ExtensionModel model = new ExtensionModel(); 
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension().Create(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Create(ExtensionModel model)
        {
            ThrowIfUidNotEmpty();

            var service = new CreateUpdateService<ExtensionModel>(stack.client.serializer, stack, resourcePath, model, "extension");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Create is used to create a custom field, custom-widget, dashboard widget to the Stack.
        /// </summary>
        /// <param name="model"><see cref="ExtensionModel"/> with details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ExtensionModel model = new ExtensionModel();
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public virtual Task<ContentstackResponse> CreateAsync(ExtensionModel model)
        {
            ThrowIfUidNotEmpty();
            stack.ThrowIfNotLoggedIn();

            var service = new CreateUpdateService<ExtensionModel>(stack.client.serializer, stack, resourcePath, model, "extension");
            return stack.client.InvokeAsync<CreateUpdateService<ExtensionModel>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Update extension call will update an existing extension on the stack.
        /// </summary>
        /// <param name="model"><see cref="ExtensionModel"/> with details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ExtensionModel model = new ExtensionModel();
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension(&quot;&lt;EXTENSION_UID&gt;&quot;).Update(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Update(ExtensionModel model)
        {
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<ExtensionModel>(stack.client.serializer, stack, resourcePath, model, "extension", "PUT");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Update extension call will update an existing extension on the stack.
        /// </summary>
        /// <param name="model"><see cref="ExtensionModel"/> with details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ExtensionModel model = new ExtensionModel();
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension(&quot;&lt;EXTENSION_UID&gt;&quot;).UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> UpdateAsync(ExtensionModel model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<ExtensionModel>(stack.client.serializer, stack, resourcePath, model, "extension", "PUT");
            return stack.client.InvokeAsync<CreateUpdateService<ExtensionModel>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Get an extension call returns comprehensive information about a specific extension of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension(&quot;&lt;EXTENSION_UID&gt;&quot;).Fetch();
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
        /// The Get an extension call returns comprehensive information about a specific extension of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension(&quot;&lt;EXTENSION_UID&gt;&quot;).FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Delete extension call will delete an existing extension from the stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension(&quot;&lt;EXTENSION_UID&gt;&quot;).Delete();
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
        /// The Delete extension call will delete an existing extension from the stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Extension(&quot;&lt;EXTENSION_UID&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> DeleteAsync()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE");
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
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
