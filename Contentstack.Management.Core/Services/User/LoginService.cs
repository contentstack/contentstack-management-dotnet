using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;
using OtpNet;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Utils;
using System.Text.Json;
using System.Text.Json.Nodes;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services.User
{
    internal class LoginService : ContentstackService
    {
        #region Private
        private readonly ICredentials _credentials;
        private readonly string _token;
        #endregion

        #region Constructor
        internal LoginService(Newtonsoft.Json.JsonSerializer serializer, ICredentials credentials, string token = null, string mfaSecret = null, JsonSerializerOptions stjOptions = null, SerializationMode serializationMode = SerializationMode.Newtonsoft): base(serializer, stjOptions: stjOptions, serializationMode: serializationMode)
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
            
            // Create login request object - conditionally include tfa_token
            object loginRequest;
            if (_token != null)
            {
                loginRequest = new
                {
                    user = new
                    {
                        email = credential.UserName,
                        password = credential.Password,
                        tfa_token = _token
                    }
                };
            }
            else
            {
                loginRequest = new
                {
                    user = new
                    {
                        email = credential.UserName,
                        password = credential.Password
                    }
                };
            }

            // Use dual serialization based on mode
            var mode = GetSerializationMode();
            WriteObjectWithBothEngines(loginRequest, mode, GetSerializerSettings(), GetStjOptions(), out byte[] content);
            this.ByteContent = content;
        }

        public override void OnResponse(IResponse httpResponse, ContentstackClientOptions config)
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                // Try STJ first if available, fallback to Newtonsoft
                try
                {
                    if (GetSerializationMode() == SerializationMode.SystemTextJson)
                    {
                        var jsonObject = httpResponse.OpenJsonObjectResponse();
                        var user = jsonObject["user"];
                        if (user != null)
                        {
                            var userObj = user.AsObject();
                            var authtoken = userObj["authtoken"];
                            if (authtoken != null)
                            {
                                config.Authtoken = authtoken.ToString();
                            }
                        }
                    }
                    else
                    {
                        // Use existing Newtonsoft implementation
                        JObject jObject = httpResponse.OpenJObjectResponse();
                        var user = jObject.GetValue("user");
                        if (user != null && user.GetType() == typeof(JObject))
                        {
                            JObject userObj = (JObject)user;
                            var authtoken = userObj.GetValue("authtoken");
                            if (authtoken != null)
                            {
                                config.Authtoken = (string)authtoken;
                            }
                        }
                    }
                }
                catch
                {
                    // Always fallback to Newtonsoft for safety
                    JObject jObject = httpResponse.OpenJObjectResponse();
                    var user = jObject.GetValue("user");
                    if (user != null && user.GetType() == typeof(JObject))
                    {
                        JObject userObj = (JObject)user;
                        var authtoken = userObj.GetValue("authtoken");
                        if (authtoken != null)
                        {
                            config.Authtoken = (string)authtoken;
                        }
                    }
                }
            }
        }
    }
}
