using System;
using contentstack.management.core.Services.App;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

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
            resourcePath = "/installations";
            if (uid != null)
            {
                resourcePath = $"/installations/{uid}";
            }
            
        }
        #endregion

        #region Public

        /// <summary>
        /// The update manifest call is used to update the app details such as name, description, icon, and so on.
        /// </summary>
        /// <param name="jObject"> Json Object for the app to be created</param>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).Update(content);
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
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).UpdateAsync(content);
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
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation().FindAll();
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
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation().FindAllAsync();
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
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).Fetch();
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
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).FetchAsync();
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
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).Delete();
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
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).DeleteAsync();
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
        /// The GET installation call is used to retrieve all installations of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation().AppInstallations();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse AppInstallations(ParameterCollection collection = null)
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
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation().AppInstallationsAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> AppInstallationsAsync(ParameterCollection collection = null)
        {
            ThrowIfAppUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"/manifests/{this.appUid}/installations", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The GET installation call is used to retrieve all installations of an app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).Configuration();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse Configuration(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/configuration", collection: collection);
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
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).ConfigurationAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> ConfigurationAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/configuration", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// To update organization level app configuration. 
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).SetConfiguration(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse SetConfiguration(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/configuration", jObject, "PUT", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// To update organization level app configuration. 
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).SetConfigurationAsync(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> SetConfigurationAsync(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/configuration", jObject, "PUT", collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// To fetch server side organization level config required for the app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).ServerConfiguration();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse ServerConfiguration(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/server-configuration", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// To fetch server side organization level config required for the app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).ServerConfigurationAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> ServerConfigurationAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/server-configuration", collection: collection);
            return client.InvokeAsync<FetchDeleteAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// To update server side organization level config required for the app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).SetServerConfiguration(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse SetServerConfiguration(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/server-configuration", jObject, "PUT", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// To update server side organization level config required for the app.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).SetServerConfigurationAsync(content);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> SetServerConfigurationAsync(JObject jObject, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new CreateUpdateAppsService(client.serializer, orgUid, $"{resourcePath}/server-configuration", jObject, "PUT", collection: collection);
            return client.InvokeAsync<CreateUpdateAppsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// To fetch installation data of an app configuration.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).InstallationData();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public virtual ContentstackResponse InstallationData(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/installationData", collection: collection);
            return client.InvokeSync(service);
        }
        /// <summary>
        /// To fetch installation data of an app configuration.
        /// </summary>
        /// <param name="collection"> Query Parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.App(&quot;&lt;APP_UID&gt;&quot;).Installation(&quot;&lt;INSTALLATION_UID&gt;&quot;).InstallationDataAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Task"/></returns>
        public virtual Task<ContentstackResponse> InstallationDataAsync(ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();
            var service = new FetchDeleteAppsService(client.serializer, orgUid, $"{resourcePath}/installationData", collection: collection);
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

