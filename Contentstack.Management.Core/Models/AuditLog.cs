using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class AuditLog
    {
        internal Stack stack;
        public string Uid { get; set; }

        internal string resourcePath;

        internal AuditLog(Stack stack, string uid = null)
        {
            stack.ThrowIfAPIKeyEmpty();

            this.stack = stack;
            Uid = uid;
            resourcePath = uid == null ? "/audit-logs" : $"/audit-logs/{uid}";
        }

        /// <summary>
        /// The Get audit log request is used to retrieve the audit log of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).AuditLog().FindAll();
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
        /// The Get audit log request is used to retrieve the audit log of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).AuditLog().FindAllAsync();
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
        /// The Get audit log item request is used to retrieve a specific item from the audit log of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).AuditLog(&quot;&lt;AUDITLOG_UID&gt;&quot;).Fetch();
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
        /// The Get audit log item request is used to retrieve a specific item from the audit log of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).AuditLog(&quot;&lt;AUDITLOG_UID&gt;&quot;).FetchAsync();
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
