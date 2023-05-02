using System;
using contentstack.management.core.Services.App;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
	public class AppRequest
	{
        #region Internal
        internal string orgUid;
        internal ContentstackClient client;
        internal string resourcePath;

        #endregion

        #region Constructor
        internal AppRequest(ContentstackClient contentstackClient, string orgUid)
        {
            this.orgUid = orgUid;
            this.client = contentstackClient;
            resourcePath = "/requests";
        }
        #endregion

        #region Public
        /// <summary>
        /// The Create call is used to create a app request for an app.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.AppRquest().Create(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Create(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, resourcePath, jObject, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The Create call is used to create a app request for an app.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.AppRquest().CreateAsync(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> CreateAsync(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, resourcePath, jObject, collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The Delete app request call is used to delete an app request of an app in target_uid.
        /// </summary>
        /// <param name="requestUid"> The ID of the request to be deleted </param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.AppRquest().Delete(&quot;&lt;APP_REQUEST_UID&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Delete(string requestUid, ParameterCollection collection = null)
        {
            if (string.IsNullOrEmpty(requestUid))
            {
                throw new InvalidOperationException("Request uid can not be empty.");
            }
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/{requestUid}", "DELETE", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The delete an app call is used to delete the app.
        /// </summary>
        /// <param name="requestUid"> The ID of the request to be deleted </param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.AppRquest().DeleteAsync(&quot;&lt;APP_REQUEST_UID&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> DeleteAsync(string requestUid, ParameterCollection collection = null)
        {
            if (string.IsNullOrEmpty(requestUid))
            {
                throw new InvalidOperationException("Request uid can not be empty.");
            }
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/{requestUid}", "DELETE", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The list manifests call is used to fetch details of all apps in a particular organization.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.AppRquest().FindAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse FindAll(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The list manifests call is used to fetch details of all apps in a particular organization.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.AppRquest().FindAllAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> FindAllAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        #endregion

        #region Throw Error

        internal void ThrowIfUidEmpty()
        {
            if (string.IsNullOrEmpty(this.orgUid))
            {
                throw new InvalidOperationException("Operation not allowed.");
            }
        }
        #endregion
    }
}

