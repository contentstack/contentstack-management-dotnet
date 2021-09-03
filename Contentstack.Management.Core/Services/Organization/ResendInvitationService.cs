using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class ResendInvitationService : ContentstackService
    {
      
        #region Internal
        internal ResendInvitationService(JsonSerializer serializer, string uid, string shareUid) : base(serializer)
        {
            if (shareUid == null)
            {
                throw new ArgumentNullException("uid");
            }
            if (shareUid == null)
            {
                throw new ArgumentNullException("shareUid");
            }
            this.ResourcePath = "/organizations/{organization_uid}/share/{share_uid}/resend_invitation";
            this.AddPathResource("{organization_uid}", uid);
            this.AddPathResource("{share_uid}", shareUid);
        }

        #endregion
    }
}
