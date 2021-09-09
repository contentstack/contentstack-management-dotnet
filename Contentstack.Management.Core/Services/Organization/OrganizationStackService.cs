using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class OrganizationStackService : ContentstackService
    {

        #region Internal
        internal OrganizationStackService(JsonSerializer serializer, string uid, ParameterCollection collection = null) : base(serializer, collection)
        {

            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }
            this.ResourcePath = "/organizations/{organization_uid}/stacks";
            this.AddPathResource("{organization_uid}", uid);
        }

        #endregion
    }
}
