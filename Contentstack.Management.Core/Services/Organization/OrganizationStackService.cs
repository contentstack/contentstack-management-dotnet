using System;
using Contentstack.Management.Core.Queryable;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class OrganizationStackService : ContentstackService
    {

        #region Internal
        internal OrganizationStackService(JsonSerializerOptions serializerOptions, string uid, ParameterCollection collection = null) : base(serializerOptions, collection: collection)
        {

            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid", CSConstants.OrganizationUIDRequired);
            }
            this.ResourcePath = "/organizations/{organization_uid}/stacks";
            this.AddPathResource("{organization_uid}", uid);
        }

        #endregion
    }
}
