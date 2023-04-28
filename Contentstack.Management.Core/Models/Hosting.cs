using System;
using contentstack.management.core.Services.App;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Newtonsoft.Json.Linq;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
	public class Hosting
	{
        #region Internal
        internal string orgUid;
        internal string appUid;
        internal ContentstackClient client;
        internal string resourcePath;
        #endregion

        #region Constructor
        internal Hosting(ContentstackClient contentstackClient, string orgUid, string uid)
        {
            this.orgUid = orgUid;
            this.appUid = uid;
            this.client = contentstackClient;
            
            if (uid != null)
            {
                resourcePath = $"/manifests/{uid}/hosting";
            }
        }
        #endregion
        #region Public
        /// <summary>
        /// The get hosting call is used to fetch to know whether the hosting is enabled or not.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().IsEnable();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse IsEnable(ParameterCollection collection = null)
        {
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The get hosting call is used to fetch to know whether the hosting is enabled or not.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().IsEnableAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> IsEnableAsync(ParameterCollection collection = null)
        {
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath,collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The toggle hosting call is used to enable the hosting of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Enable();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Enable(ParameterCollection collection = null)
        {
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/enable", new JObject(), "PUT", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The toggle hosting call is used to enable the hosting of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().EnableAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> EnableAsync(ParameterCollection collection = null)
        {
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/enable", new JObject(), "PUT", collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The toggle hosting call is used to enable the hosting of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Disable();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Disable(ParameterCollection collection = null)
        {
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/disable", new JObject(), "PUT", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The toggle hosting call is used to enable the hosting of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().DisableAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> DisableAsync(ParameterCollection collection = null)
        {
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/disable", new JObject(), "PUT", collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The create signed upload url call is used to create an signed upload url for the files in hosting.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().CreateUploadUrl();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse CreateUploadUrl(ParameterCollection collection = null)
        {
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/signedUploadUrl", new JObject(), collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The create signed upload url call is used to create an signed upload url for the files in hosting.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().CreateUploadUrlAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> CreateUploadUrlAsync(ParameterCollection collection = null)
        {
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/signedUploadUrl", new JObject(), collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The GET latest live deployment call is used to get details of latest deployment of the source file.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().LatestLiveDeployment();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse LatestLiveDeployment(ParameterCollection collection = null)
        {
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/latestLiveDeployment", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The GET latest live deployment call is used to get details of latest deployment of the source file.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().LatestLiveDeploymentAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> LatestLiveDeploymentAsync(ParameterCollection collection = null)
        {
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/latestLiveDeployment", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The Deployment allows you to deploy app.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Deployment"/></returns>
        public Deployment Deployment(string uid = null)
        {
            return new Deployment(client, orgUid, this.appUid, uid);
        }
        #endregion
    }
}

