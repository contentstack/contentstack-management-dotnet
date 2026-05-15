using System;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class ResendInvitationService : ContentstackService
    {
      
        #region Internal
        internal ResendInvitationService(JsonSerializer serializer, string uid, string shareUid) : base(serializer)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid", CSConstants.OrganizationUIDRequired);
            }
            if (string.IsNullOrEmpty(shareUid))
            {
                throw new ArgumentNullException("shareUid", CSConstants.ShareUIDRequired);
            }
            this.ResourcePath = "/organizations/{organization_uid}/share/{share_uid}/resend_invitation";
            this.AddPathResource("{organization_uid}", uid);
            this.AddPathResource("{share_uid}", shareUid);
        }

        #endregion
    }
}
