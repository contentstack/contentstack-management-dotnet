﻿using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class Folder
    {
        internal Stack stack;
        public string Uid;

        internal string resourcePath;

        internal Folder(Stack stack, string uid = null)
        {
            stack.ThrowIfAPIKeyEmpty();

            this.stack = stack;
            Uid = uid;
            resourcePath = uid == null ? "/assets/folders" : $"/assets/folders/{uid}";
        }

        /// <summary>
        /// The Create a folder call is used to create an asset folder and/or add a parent folder to it.
        /// </summary>
        /// <param name="name">Name for the folder.</param>
        /// <param name="parentUid">Parent Uid for folder to place this folder within another folder</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Folder().Create(&quot;&lt;FOLDER_NAME&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Create(string name, string parentUid = null)
        {
            ThrowIfUidNotEmpty();

            var service = new CreateUpdateFolderService(stack.client.serializer, stack, name, null, parentUid);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Create a folder call is used to create an asset folder and/or add a parent folder to it.
        /// </summary>
        /// <param name="name">Name for the folder.</param>
        /// <param name="parentUid">Parent Uid for folder to place this folder within another folder</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Folder().CreateAsync(&quot;&lt;FOLDER_NAME&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public virtual Task<ContentstackResponse> CreateAsync(string name, string parentUid = null)
        {
            ThrowIfUidNotEmpty();
            stack.client.ThrowIfNotLoggedIn();

            var service = new CreateUpdateFolderService(stack.client.serializer, stack, name, null, parentUid);
            return stack.client.InvokeAsync<CreateUpdateFolderService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Update or move folder request can be used either to update the details of a folder or set the parent folder if you want to move a folder under another folder.
        /// </summary>
        /// <param name="name">Name for the folder.</param>
        /// <param name="parentUid">Parent Uid for folder to place this folder within another folder</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Folder(&quot;&lt;ASSET_UID&gt;&quot;).Update(&quot;&lt;FOLDER_NAME&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Update(string name, string parentUid = null)
        {
            ThrowIfUidEmpty();

            var service = new CreateUpdateFolderService(stack.client.serializer, stack, name, null, parentUid);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Update or move folder request can be used either to update the details of a folder or set the parent folder if you want to move a folder under another folder.
        /// </summary>
        /// <param name="name">Name for the folder.</param>
        /// <param name="parentUid">Parent Uid for folder to place this folder within another folder</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Folder(&quot;&lt;ASSET_UID&gt;&quot;).UpdateAsync(&quot;&lt;FOLDER_NAME&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> UpdateAsync(string name, string parentUid = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new CreateUpdateFolderService(stack.client.serializer, stack, name, null, parentUid);
            return stack.client.InvokeAsync<CreateUpdateFolderService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Get a single folder call gets the comprehensive details of a specific asset folder by means of folder UID
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Folder(&quot;&lt;ASSET_UID&gt;&quot;).Fetch(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get a single folder call gets the comprehensive details of a specific asset folder by means of folder UID.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Folder(&quot;&lt;ASSET_UID&gt;&quot;).FetchAsync(model);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Delete a folder call is used to delete an asset folder along with all the assets within that folder.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Folder(&quot;&lt;ASSET_UID&gt;&quot;).Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Delete()
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Delete a folder call is used to delete an asset folder along with all the assets within that folder.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Asset().Folder(&quot;&lt;ASSET_UID&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> DeleteAsync()
        {
            stack.client.ThrowIfNotLoggedIn();
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