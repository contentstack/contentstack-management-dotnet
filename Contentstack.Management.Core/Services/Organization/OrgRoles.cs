﻿using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class OrganizationRolesService : ContentstackService
    {
        #region Internal

        internal OrganizationRolesService(JsonSerializer serializer, string uid, ParameterCollection collection) : base(serializer, collection)
        {
            if (uid == null)
            {
                throw new ArgumentNullException("uid");
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