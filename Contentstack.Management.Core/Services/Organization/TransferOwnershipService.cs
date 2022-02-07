using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class TransferOwnershipService : ContentstackService
    {
        private string _email;

        #region Internal
        internal TransferOwnershipService(JsonSerializer serializer, string uid, string email) : base(serializer)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }
            this._email = email;
            this.ResourcePath = "/organizations/{organization_uid}/transfer-ownership";
            this.HttpMethod = "POST";
            this.AddPathResource("{organization_uid}", uid);
        }
        #endregion

        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("transfer_to");
                writer.WriteValue(_email);
                writer.WriteEndObject();
                string snippet = stringWriter.ToString();
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
    }
}