using System;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class TransferOwnershipService : ContentstackService
    {
        private readonly string _email;

        #region Internal
        internal TransferOwnershipService(JsonSerializerOptions serializerOptions, string uid, string email) : base(serializerOptions)
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
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WriteString("transfer_to", _email);
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }
    }
}
