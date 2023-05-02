using System;
using System.Threading.Tasks;
using contentstack.management.core.Services.App;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Models
{
	public class App
	{
        #region Internal
        internal string orgUid;
        internal string uid;
        internal ContentstackClient client;
        internal string resourcePath;
        internal string resourcePathOAuth;

        #endregion

        #region Constructor
        internal App(ContentstackClient contentstackClient, string orgUid, string uid = null)
		{
			this.orgUid = orgUid;
            this.uid = uid;
            this.client = contentstackClient;
            resourcePath = uid == null ? "/manifests" : $"/manifests/{uid}";
            if (uid != null)
            {
                resourcePathOAuth = $"/manifests/{uid}/oauth";
            }
		}
        #endregion

        #region Public
        /// <summary>
        /// The create manifest call is used for creating a new app/manifest in your Contentstack organization.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Create(content);
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
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).CreateAsync(content);
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
        /// The update manifest call is used to update the app details such as name, description, icon, and so on.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Update(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Update(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, resourcePath, jObject, "PUT", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The update manifest call is used to update the app details such as name, description, icon, and so on.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).UpdateAsync(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> UpdateAsync(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, resourcePath, jObject, "PUT", collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The list manifests call is used to fetch details of all apps in a particular organization.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).FindAll();
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
        /// The list manifests call is used to fetch details of all apps in a particular organization.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).FindAllAsync();
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
        /// The get manifest call is used to fetch details of a particular app with its ID.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Fetch();
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
        ///The get manifest call is used to fetch details of a particular app with its ID.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).FetchAsync();
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
        /// The delete an app call is used to delete the app.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Delete(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, "DELETE", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The delete an app call is used to delete the app.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePath, "DELETE", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The update oauth configuration call is used to update the OAuth details, (redirect url and permission scope) of an app.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).UpdateOAuth(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse UpdateOAuth(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, resourcePathOAuth, jObject, "PUT", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The update oauth configuration call is used to update the OAuth details, (redirect url and permission scope) of an app.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).UpdateOAuthAsync(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> UpdateOAuthAsync(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, resourcePathOAuth, jObject, "PUT", collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The get oauth call is used to fetch the OAuth details of the app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).FetchOAuth();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse FetchOAuth(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePathOAuth, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The get oauth call is used to fetch the OAuth details of the app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).FetchOAuthAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> FetchOAuthAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, resourcePathOAuth, collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Create installation call is used to initiate the installation of the app
        /// </summary>
        /// <param name="jObject"> Json Object for app installation config</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Install(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Install(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/install", jObject, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The Create installation call is used to initiate the installation of the app
        /// </summary>
        /// <param name="jObject"> Json Object for app installation config</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).InstallAsync(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> InstallAsync(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/install", jObject, collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The Reinstall call is used to upgrade the installation of an app.
        /// </summary>
        /// <param name="jObject"> Json Object for app installation config</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Reinstall(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Reinstall(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/reinstall", jObject, collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The Reinstall call is used to upgrade the installation of an app.
        /// </summary>
        /// <param name="jObject"> Json Object for app installation config</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).ReinstallAsync(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> ReinstallAsync(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/reinstall", jObject, collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// App installation allow to set installation, configuration for apps.
        /// </summary>
        /// <param name="uid"> Installation uid.
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Installation"/></returns>
        public Installation Installation(string uid = null)
        {
            ThrowIfUidEmpty();
            return new Installation(client, orgUid, this.uid, uid);
        }
        /// <summary>
        /// The Authorization allows you to get and revoke app authorization.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Authorization();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Authorization"/></returns>
        public Authorization Authorization()
        {
            ThrowIfUidEmpty();
            return new Authorization(client, orgUid, this.uid);
        }
        /// <summary>
        /// The Hosting allows you to host app.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Hosting();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Hosting"/></returns>
        public Hosting Hosting()
        {
            ThrowIfUidEmpty();
            return new Hosting(client, orgUid, this.uid);
        }
        /// <summary>
        /// To get the apps list of authorized apps for the particular organization.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Authorize();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Authorize(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/authorize", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// To get the apps list of authorized apps for the particular organization.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).AuthorizeAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> AuthorizeAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/authorize", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The GET app requests of an app call is used to retrieve all requests of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).GetRequests();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse GetRequests(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/requests", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// The GET app requests of an app call is used to retrieve all requests of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).GetRequestsAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> GetRequestsAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/requests", collection: collection);
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
        #endregion
    }
}

