using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using OtpNet;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.User
{
    internal class LoginService : ContentstackService
    {
        #region Private
        private readonly ICredentials _credentials;
        private readonly string _token;
        #endregion

        #region Constructor
        internal LoginService(JsonSerializerOptions serializerOptions, ICredentials credentials, string token = null, string mfaSecret = null): base(serializerOptions)
        {
            this.HttpMethod = "POST";
            this.ResourcePath = "user-session";

            if (credentials == null)
            {
                throw new ArgumentNullException("credentials", CSConstants.LoginCredentialsRequired);
            }

            _credentials = credentials;

            if (string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(mfaSecret))
            {
                var secretBytes = Base32Encoding.ToBytes(mfaSecret);

                var totp = new Totp(secretBytes);
                _token = totp.ComputeTotp();
            }
            else
            {
                _token = token;
            }
        }
        #endregion

        public override void ContentBody()
        {
            var credential = _credentials as NetworkCredential;
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("user");
                writer.WriteStartObject();
                writer.WriteString("email", credential.UserName);
                writer.WriteString("password", credential.Password);
                if (_token != null)
                    writer.WriteString("tfa_token", _token);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }

        public override void OnResponse(IResponse httpResponse, ContentstackClientOptions config)
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                var root = httpResponse.OpenJsonObjectResponse();
                if (root.TryGetPropertyValue("user", out var userNode) && userNode is JsonObject userObj)
                {
                    if (userObj.TryGetPropertyValue("authtoken", out var at) && at != null)
                        config.Authtoken = at.GetValue<string>();
                }
            }

        }
    }
}
