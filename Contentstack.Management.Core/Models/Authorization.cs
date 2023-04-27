using System;
using contentstack.management.core.Services.App;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Queryable;
using System.Net;
using System.Security.Cryptography;

namespace Contentstack.Management.Core.Models
{
	public class Authorization
	{

        #region Internal
        internal string orgUid;
        internal string appUid;
        internal ContentstackClient client;
        internal string resourcePath;
        #endregion

        #region Constructor
        internal Authorization(ContentstackClient contentstackClient, string orgUid, string uid)
        {
            this.orgUid = orgUid;
            this.appUid = uid;
            this.client = contentstackClient;
            if (uid != null)
            {
                resourcePath = $"/manifests/{uid}/authorizations";
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// List all user authorizations made to an authorized app under a particular organization
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Authorization().FindAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse FindAll(ParameterCollection collection = null)
        {
            ThrowIfAppUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// List all user authorizations made to an authorized app under a particular organization
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Authorization().FindAllAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> FindAllAsync(ParameterCollection collection = null)
        {
            ThrowIfAppUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Revoke user token issued to an authorized app for the particular organization
        /// </summary>
        /// <param name="authorizationUid"> Authorization uid to revoke.
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Authorization().Revoke(&quot;&lt;AUTHORIZATION_UID&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Revoke(string authorizationUid, ParameterCollection collection = null)
        {
            ThrowIfAppUidEmpty();
            if (string.IsNullOrEmpty(authorizationUid))
            {
                throw new InvalidOperationException("Authorization uid can not be empty.");
            }
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/{authorizationUid}", "DELETE", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// Revoke user token issued to an authorized app for the particular organization
        /// </summary>
        /// <param name="authorizationUid"> Authorization uid to revoke.
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Authorization().RevokeAsync(&quot;&lt;AUTHORIZATION_UID&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> RevokeAsync(string authorizationUid, ParameterCollection collection = null)
        {
            ThrowIfAppUidEmpty();
            if (string.IsNullOrEmpty(authorizationUid))
            {
                throw new InvalidOperationException("Authorization uid can not be empty.");
            }
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/{authorizationUid}", "DELETE", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// Revoke user token issued to an authorized app for the particular organization
        /// </summary>
        /// <param name="authorizationUid"> Authorization uid to revoke.
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Authorization().RevokeAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse RevokeAll(ParameterCollection collection = null)
        {
            ThrowIfAppUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, "DELETE", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// Revoke user token issued to an authorized app for the particular organization
        /// </summary>
        /// <param name="authorizationUid"> Authorization uid to revoke.
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Authorization().RevokeAllAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> RevokeAllAsync(ParameterCollection collection = null)
        {
            ThrowIfAppUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, "DELETE", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }

        #endregion
        #region Throw Error
        internal void ThrowIfAppUidEmpty()
        {
            if (string.IsNullOrEmpty(this.appUid))
            {
                throw new InvalidOperationException("App uid can not be empty.");
            }
        }
        #endregion
    }
}

