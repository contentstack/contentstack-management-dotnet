using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;
using System.Text.Json;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services.User
{
    internal class ResetPasswordService: ContentstackService
    {
        private readonly string _resetPasswordToken;
        private readonly string _password;
        private readonly string _confirmPassword;

        internal ResetPasswordService(Newtonsoft.Json.JsonSerializer serializer, string resetPasswordToken, string password, string confirmPassword, JsonSerializerOptions stjOptions = null, SerializationMode serializationMode = SerializationMode.Newtonsoft) : base(serializer, stjOptions: stjOptions, serializationMode: serializationMode)
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
            var resetPasswordRequest = new
            {
                user = new
                {
                    reset_password_token = _resetPasswordToken,
                    password = _password,
                    password_confirmation = _confirmPassword
                }
            };

            var mode = GetSerializationMode();
            WriteObjectWithBothEngines(resetPasswordRequest, mode, GetSerializerSettings(), GetStjOptions(), out byte[] content);
            this.ByteContent = content;
        }
    }
}
