using System;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.User
{
    internal class ForgotPasswordService : ContentstackService
    {
        private readonly string _email;

        internal ForgotPasswordService(JsonSerializerOptions serializerOptions, string email): base (serializerOptions)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email", CSConstants.EmailRequired);
            }

            _email = email;
            this.HttpMethod = "POST";
            this.ResourcePath = "user/forgot_password";
        }

        public override void ContentBody()
        {
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("user");
                writer.WriteStartObject();
                writer.WriteString("email", _email);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }
    }
}
