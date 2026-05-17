using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;
using System.Text.Json;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services.User
{
    internal class ForgotPasswordService : ContentstackService
    {
        private readonly string _email;

        internal ForgotPasswordService(Newtonsoft.Json.JsonSerializer serializer, string email, JsonSerializerOptions stjOptions = null, SerializationMode serializationMode = SerializationMode.Newtonsoft): base (serializer, stjOptions: stjOptions, serializationMode: serializationMode)
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
            var forgotPasswordRequest = new
            {
                user = new
                {
                    email = _email
                }
            };

            var mode = GetSerializationMode();
            WriteObjectWithBothEngines(forgotPasswordRequest, mode, GetSerializerSettings(), GetStjOptions(), out byte[] content);
            this.ByteContent = content;
        }
    }
}
