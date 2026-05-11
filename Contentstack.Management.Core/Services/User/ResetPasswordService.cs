using System;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.User
{
    internal class ResetPasswordService: ContentstackService
    {
        private readonly string _resetPasswordToken;
        private readonly string _password;
        private readonly string _confirmPassword;

        internal ResetPasswordService(JsonSerializerOptions serializerOptions, string resetPasswordToken, string password, string confirmPassword) : base(serializerOptions)
        {
            if (string.IsNullOrEmpty(resetPasswordToken))
            {
                throw new ArgumentNullException("resetPasswordToken", CSConstants.ResetPasswordTokenRequired);
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password", CSConstants.NewPasswordRequired);
            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                throw new ArgumentNullException("confirmPassword", CSConstants.PasswordMismatch);
            }
            _resetPasswordToken = resetPasswordToken;
            _password = password;
            _confirmPassword = confirmPassword;
            this.HttpMethod = "POST";
            this.ResourcePath = "user/reset_password";
        }


        public override void ContentBody()
        {
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("user");
                writer.WriteStartObject();
                writer.WriteString("reset_password_token", _resetPasswordToken);
                writer.WriteString("password", _password);
                writer.WriteString("password_confirmation", _confirmPassword);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }
    }
}
