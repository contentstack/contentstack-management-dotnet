using System;
using contentstack.management.core.Services.App;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Models
{
	public class Installation
	{
        #region Internal
        internal string orgUid;
        internal string appUid;
        internal string uid;
        internal ContentstackClient client;
        internal string resourcePath;
        internal string resourcePathOAuth;

        #endregion

        #region Constructor
        internal Installation(ContentstackClient contentstackClient, string orgUid, string appUid = null, string uid = null)
        {
            this.orgUid = orgUid;
            this.appUid = appUid;
            this.uid = uid;
            this.client = contentstackClient;
        }
        #endregion

        #region Public
        /// <summary>
        /// The GET installation call is used to retrieve all installations of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation().FindAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse FindAll(ParameterCollection collection = null)
        {
            ThrowIfAppUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"/manifests/{this.appUid}/installations", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The GET installation call is used to retrieve all installations of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation().FindAllAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> FindAllAsync(ParameterCollection collection = null)
        {
            ThrowIfAppUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"/manifests/{this.appUid}/installations", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        #endregion

        #region Throw Error

        internal void ThrowIfUidNotEmpty()
        {
            if (!string.IsNullOrEmpty(this.uid))
            {
                throw new InvalidOperationException("Operation not allowed.");
            }
        }

        internal void ThrowIfUidEmpty()
        {
            if (string.IsNullOrEmpty(this.uid))
            {
                throw new InvalidOperationException("Uid can not be empty.");
            }
        }
        internal void ThrowIfAppUidEmpty()
        {
            if (string.IsNullOrEmpty(this.appUid))
            {
                throw new InvalidOperationException("App uid can not be empty.");
            }
        }

        private void ThrowIfOrganizationUidNull()
        {
            if (string.IsNullOrEmpty(this.orgUid))
            {
                throw new InvalidOperationException("Org uid can not be empty.");
            }
        }
        #endregion
    }
}

