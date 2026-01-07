using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class OrganizationRolesService : ContentstackService
    {
        #region Internal

        internal OrganizationRolesService(JsonSerializer serializer, string uid, ParameterCollection collection) : base(serializer, collection: collection)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid", CSConstants.OrganizationUIDRequired);
            }

            this.ResourcePath = "organizations/{organization_uid}/roles";
            this.AddPathResource("{organization_uid}", uid);

            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }
        #endregion
    }
}
