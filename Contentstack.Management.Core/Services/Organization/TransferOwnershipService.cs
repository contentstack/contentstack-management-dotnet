using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;
using System.Text.Json;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class TransferOwnershipService : ContentstackService
    {
        private readonly string _email;

        #region Internal
        internal TransferOwnershipService(Newtonsoft.Json.JsonSerializer serializer, string uid, string email, JsonSerializerOptions stjOptions = null, SerializationMode serializationMode = SerializationMode.Newtonsoft) : base(serializer, stjOptions: stjOptions, serializationMode: serializationMode)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid", CSConstants.OrganizationUIDRequired);
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email", CSConstants.EmailRequired);
            }
            this._email = email;
            this.ResourcePath = "/organizations/{organization_uid}/transfer-ownership";
            this.HttpMethod = "POST";
            this.AddPathResource("{organization_uid}", uid);
        }
        #endregion

        public override void ContentBody()
        {
            var transferRequest = new { transfer_to = _email };
            var mode = GetSerializationMode();
            WriteObjectWithBothEngines(transferRequest, mode, GetSerializerSettings(), GetStjOptions(), out byte[] content);
            this.ByteContent = content;
        }
    }
}