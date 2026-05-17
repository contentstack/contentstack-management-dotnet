using System;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;
using System.Text.Json;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class ResendInvitationService : ContentstackService
    {
      
        #region Internal
        internal ResendInvitationService(Newtonsoft.Json.JsonSerializer serializer, string uid, string shareUid, JsonSerializerOptions stjOptions = null, SerializationMode serializationMode = SerializationMode.Newtonsoft) : base(serializer, stjOptions: stjOptions, serializationMode: serializationMode)
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
