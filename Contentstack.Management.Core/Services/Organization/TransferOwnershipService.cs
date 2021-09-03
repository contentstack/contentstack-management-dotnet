using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class TransferOwnershipService : ContentstackService
    {
        private string _email;

        #region Internal
        internal TransferOwnershipService(JsonSerializer serializer, string uid, string email) : base(serializer)
        {
            if (uid == null)
            {
                throw new ArgumentNullException("uid");
            }

            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            this.ResourcePath = "/organizations/{organization_uid}/transfer-ownership";
            this.HttpMethod = "POST";
            this.AddPathResource("{organization_uid}", uid);
        }
        #endregion
    }
}