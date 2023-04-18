using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace contentstack.management.core.Services.App
{
	internal class CreateUpdateAppsService: ContentstackService
	{
        #region private
        private readonly JObject _typedModel;
        #endregion
        #region Internal
        internal CreateUpdateAppsService(JsonSerializer serializer, string orgUid, string resourcePath, JObject dataModel, string httpMethod = "POST", ParameterCollection collection = null) : base(serializer, collection:collection)
        {
            if (orgUid == null)
            {
                throw new ArgumentNullException("Organization Uid", "Should have Organization Uid.");
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("Resource path", "Should have resource path.");
            }
            if (dataModel == null)
            {
                throw new ArgumentNullException("Json Object", "Should have data.");
            }
            this.ResourcePath = resourcePath;
            this.HttpMethod = httpMethod;
            this.versionStrategy = Http.VersionStrategy.None;
            Headers["organization_uid"] = orgUid;
            
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
            _typedModel = dataModel;
        }
        #endregion

        #region Public
        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);

                Serializer.Serialize(writer, _typedModel);
                string snippet = stringWriter.ToString();
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
        #endregion
    }
}

