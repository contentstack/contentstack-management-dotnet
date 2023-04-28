using System;
using contentstack.management.core.Services.App;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Newtonsoft.Json.Linq;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
	public class Deployment
	{
        #region Internal
        internal string orgUid;
        internal string appUid;
        internal string uid;
        internal ContentstackClient client;
        internal string resourcePath;
        #endregion

        #region Constructor
        internal Deployment(ContentstackClient contentstackClient, string orgUid, string appUid, string uid = null)
        {
            this.orgUid = orgUid;
            this.appUid = appUid;
            this.uid = uid;
            this.client = contentstackClient;
            resourcePath = uid == null ? $"/manifests/{appUid}/hosting/deployments" : $"/manifests/{appUid}/hosting/deployments/{uid}";
            
        }
        #endregion
        #region Public
        /// <summary>
        /// The create hosting deployments call is used to deploy the uploaded file in hosting
        /// </summary>
        /// <param name="jObject"> Json Object for the deployment to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment().Create(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Create(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, resourcePath, jObject, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The create manifest call is used for creating a new app/manifest in your Contentstack organization.
        /// </summary>
        /// <param name="jObject"> Json Object for the deployment to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment().CreateAsync(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> CreateAsync(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, resourcePath, jObject, collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        ///  The list deployments call is used to get all the available deployments made for an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment().FindAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse FindAll(ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        ///  The list deployments call is used to get all the available deployments made for an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment().FindAllAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> FindAllAsync(ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The GET deployment call is used to get all the details of an deployment of an app
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment(&quot;&lt;DEPLOYMENT_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The GET deployment call is used to get all the details of an deployment of an app
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment(&quot;&lt;DEPLOYMENT_UID&gt;&quot;).FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The list deployment logs call is used to list logs of a deployment.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment(&quot;&lt;DEPLOYMENT_UID&gt;&quot;).Logs();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Logs(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The list deployment logs call is used to list logs of a deployment.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment(&quot;&lt;DEPLOYMENT_UID&gt;&quot;).LogsAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> LogsAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The create hosting deployments call is used to deploy the uploaded file in hosting
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment(&quot;&lt;DEPLOYMENT_UID&gt;&quot;).SignedDownloadUrl();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse SignedDownloadUrl(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/signedDownloadUrl", new JObject(), collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The create manifest call is used for creating a new app/manifest in your Contentstack organization.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting().Deployment(&quot;&lt;DEPLOYMENT_UID&gt;&quot;).SignedDownloadUrlAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> SignedDownloadUrlAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/signedDownloadUrl", new JObject(), collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
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
        #endregion
    }
}

