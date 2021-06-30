using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.User
{
    internal class ResetPasswordService: ContentstackService
    {
        private readonly string _resetPasswordToken;
        private readonly string _password;
        private readonly string _confirmPassword;

        internal ResetPasswordService(JsonSerializer serializer, string resetPasswordToken, string password, string confirmPassword) : base(serializer)
        {
            if (string.IsNullOrEmpty(resetPasswordToken))
            {
                throw new ArgumentNullException("resetPasswordToken");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                throw new ArgumentNullException("confirmPassword");
            }
            _resetPasswordToken = resetPasswordToken;
            _password = password;
            _confirmPassword = confirmPassword;
            this.HttpMethod = "POST";
            this.ResourcePath = "user/reset_password";
        }


        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("user");
                    writer.WriteStartObject();
                    writer.WritePropertyName("reset_password_token");
                    writer.WriteValue(_resetPasswordToken);
                    writer.WritePropertyName("password");
                    writer.WriteValue(_password);
                    writer.WritePropertyName("password_confirmation");
                    writer.WriteValue(_confirmPassword);
                    writer.WriteEndObject();
                writer.WriteEndObject();

                string snippet = stringWriter.ToString();
                this.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
    }
}
