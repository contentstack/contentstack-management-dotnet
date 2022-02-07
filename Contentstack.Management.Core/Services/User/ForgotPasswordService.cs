using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.User
{
    internal class ForgotPasswordService : ContentstackService
    {
        private readonly string _email;

        internal ForgotPasswordService(JsonSerializer serializer, string email): base (serializer)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            _email = email;
            this.HttpMethod = "POST";
            this.ResourcePath = "user/forgot_password";
        }

        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("user");
                    writer.WriteStartObject();
                    writer.WritePropertyName("email");
                    writer.WriteValue(_email);
                    writer.WriteEndObject();
                writer.WriteEndObject();

                string snippet = stringWriter.ToString();
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
    }
}
