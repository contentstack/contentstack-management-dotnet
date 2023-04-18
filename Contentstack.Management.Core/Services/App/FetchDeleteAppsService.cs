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
	internal class FetchDeleteAppsService: ContentstackService
	{
        #region Internal
        internal FetchDeleteAppsService(JsonSerializer serializer, string orgUid, string resourcePath, string httpMethod = "GET", ParameterCollection collection = null) : base(serializer, collection:collection)
        {
            if (orgUid == null)
            {
                throw new ArgumentNullException("Organization Uid", "Should have Organization Uid.");
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("Resource path", "Should have resource path.");
            }
            
            this.ResourcePath = resourcePath;
            this.HttpMethod = httpMethod;
            this.versionStrategy = Http.VersionStrategy.None;
            Headers["organization_uid"] = orgUid;
            
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }
        #endregion

    }
}

