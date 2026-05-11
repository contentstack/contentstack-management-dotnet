using System;
using Contentstack.Management.Core.Queryable;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class OrganizationRolesService : ContentstackService
    {
        #region Internal

        internal OrganizationRolesService(JsonSerializerOptions serializerOptions, string uid, ParameterCollection collection) : base(serializerOptions, collection: collection)
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
