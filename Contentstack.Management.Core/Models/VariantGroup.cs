using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Variant groups allow you to group related variants together for better organization and management.
    /// </summary>
    public class VariantGroup
    {
        internal Stack stack;
        internal string resourcePath;

        /// <summary>
        /// Gets the UID of the variant group.
        /// </summary>
        public string Uid { get; private set; }

        #region Constructor
        internal VariantGroup(Stack stack, string uid = null)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack", "Stack cannot be null.");
            }

            stack.ThrowIfAPIKeyEmpty();

            this.stack = stack;
            this.Uid = uid;
            this.resourcePath = uid == null ? "/variant_groups" : $"/variant_groups/{uid}";
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// The Find call fetches all the variant groups in a stack.
        /// </summary>
        /// <param name="collection">Query parameters for filtering results.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// ContentstackResponse response = client.Stack("&lt;API_KEY&gt;").VariantGroup().Find();
        /// </code></pre>
        /// </example>
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
        /// The Find call fetches all the variant groups in a stack.
        /// </summary>
        /// <param name="collection">Query parameters for filtering results.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// ContentstackResponse response = await client.Stack("&lt;API_KEY&gt;").VariantGroup().FindAsync();
        /// </code></pre>
        /// </example>
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
        /// The Link Content Types call associates one or more content types with a variant group.
        /// </summary>
        /// <param name="contentTypeUids">List of content type UIDs to link to the variant group.</param>
        /// <param name="collection">Query parameters.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// List&lt;string&gt; contentTypeUids = new List&lt;string&gt; { "content_type_uid_1", "content_type_uid_2" };
        /// ContentstackResponse response = client.Stack("&lt;API_KEY&gt;").VariantGroup("variant_group_uid").LinkContentTypes(contentTypeUids);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse LinkContentTypes(
            List<string> contentTypeUids,
            ParameterCollection collection = null
        )
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new VariantContentTypeLinkService(
                stack.client.serializer,
                stack,
                $"{resourcePath}/variants",
                contentTypeUids,
                this.Uid,
                true,
                collection
            );

            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Link Content Types call associates one or more content types with a variant group.
        /// </summary>
        /// <param name="contentTypeUids">List of content type UIDs to link to the variant group.</param>
        /// <param name="collection">Query parameters.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// List&lt;string&gt; contentTypeUids = new List&lt;string&gt; { "content_type_uid_1", "content_type_uid_2" };
        /// ContentstackResponse response = await client.Stack("&lt;API_KEY&gt;").VariantGroup("variant_group_uid").LinkContentTypesAsync(contentTypeUids);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> LinkContentTypesAsync(
            List<string> contentTypeUids,
            ParameterCollection collection = null
        )
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new VariantContentTypeLinkService(
                stack.client.serializer,
                stack,
                $"{resourcePath}/variants",
                contentTypeUids,
                this.Uid,
                true,
                collection
            );

            return stack.client.InvokeAsync<VariantContentTypeLinkService, ContentstackResponse>(
                service
            );
        }

        /// <summary>
        /// The Unlink Content Types call removes the association between one or more content types and a variant group.
        /// </summary>
        /// <param name="contentTypeUids">List of content type UIDs to unlink from the variant group.</param>
        /// <param name="collection">Query parameters.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// List&lt;string&gt; contentTypeUids = new List&lt;string&gt; { "content_type_uid_1", "content_type_uid_2" };
        /// ContentstackResponse response = client.Stack("&lt;API_KEY&gt;").VariantGroup("variant_group_uid").UnlinkContentTypes(contentTypeUids);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse UnlinkContentTypes(
            List<string> contentTypeUids,
            ParameterCollection collection = null
        )
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new VariantContentTypeLinkService(
                stack.client.serializer,
                stack,
                $"{resourcePath}/variants",
                contentTypeUids,
                this.Uid,
                false,
                collection
            );

            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Unlink Content Types call removes the association between one or more content types and a variant group.
        /// </summary>
        /// <param name="contentTypeUids">List of content type UIDs to unlink from the variant group.</param>
        /// <param name="collection">Query parameters.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// List&lt;string&gt; contentTypeUids = new List&lt;string&gt; { "content_type_uid_1", "content_type_uid_2" };
        /// ContentstackResponse response = await client.Stack("&lt;API_KEY&gt;").VariantGroup("variant_group_uid").UnlinkContentTypesAsync(contentTypeUids);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> UnlinkContentTypesAsync(
            List<string> contentTypeUids,
            ParameterCollection collection = null
        )
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new VariantContentTypeLinkService(
                stack.client.serializer,
                stack,
                $"{resourcePath}/variants",
                contentTypeUids,
                this.Uid,
                false,
                collection
            );

            return stack.client.InvokeAsync<VariantContentTypeLinkService, ContentstackResponse>(
                service
            );
        }

        #endregion

        #region Private Methods

        internal void ThrowIfUidNotEmpty()
        {
            if (!string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("Operation not allowed when UID is specified.");
            }
        }

        internal void ThrowIfUidEmpty()
        {
            if (string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException(
                    "Variant group UID is required for this operation."
                );
            }
        }

        #endregion
    }
}
