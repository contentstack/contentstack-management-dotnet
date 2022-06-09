using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class PublishQueue
    {
        internal Stack stack;
        public string Uid { get; set; }

        internal string resourcePath;

        internal PublishQueue(Stack stack, string uid = null)
        {
            stack.ThrowIfAPIKeyEmpty();

            this.stack = stack;
            Uid = uid;
            resourcePath = uid == null ? "/publish-queue" : $"/publish-queue/{uid}";
        }

        /// <summary>
        /// The Get publish queue request returns comprehensive information on activities such as publish, unpublish, and delete that have performed on entries and/or assets.
        /// This request also includes the details of the release deployments in the response body.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).PublishQueue().FindAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse FindAll(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get publish queue request returns comprehensive information on activities such as publish, unpublish, and delete that have performed on entries and/or assets.
        /// This request also includes the details of the release deployments in the response body.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).PublishQueue().FindAllAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> FindAllAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Get publish queue activity request returns comprehensive information on a specific publish, unpublish, or delete action that was performed on an entry and/or asset.
        /// You can also retrieve details of a specific release deployment.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).PublishQueue(&quot;&lt;PUBLISH_QUEUE_UID&gt;&quot;).Fetch();
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
        /// The Get publish queue activity request returns comprehensive information on a specific publish, unpublish, or delete action that was performed on an entry and/or asset.
        /// You can also retrieve details of a specific release deployment.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).PublishQueue(&quot;&lt;PUBLISH_QUEUE_UID&gt;&quot;).FetchAsync();
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
        /// The Cancel Scheduled Action request will allow you to cancel any scheduled publishing or unpublishing activity of entries and/or assets and also cancel the deployment of releases.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).PublishQueue(&quot;&lt;PUBLISH_QUEUE_UID&gt;&quot;).Cancel();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Cancel(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"{resourcePath}/unschedule", collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Cancel Scheduled Action request will allow you to cancel any scheduled publishing or unpublishing activity of entries and/or assets and also cancel the deployment of releases.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).PublishQueue(&quot;&lt;PUBLISH_QUEUE_UID&gt;&quot;).CancelAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> CancelAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"{resourcePath}/unschedule", collection: collection);
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
