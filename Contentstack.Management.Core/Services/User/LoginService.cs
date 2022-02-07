using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Contentstack.Management.Core.Http;

namespace Contentstack.Management.Core.Services.User
{
    internal class LoginService : ContentstackService
    {
        #region Private
        private ICredentials _credentials;
        private string _token;
        #endregion

        #region Constructor
        internal LoginService(JsonSerializer serializer, ICredentials credentials, string token = null): base(serializer)
        {
            this.HttpMethod = "POST";
            this.ResourcePath = "user-session";

            if (credentials == null)
            {
                throw new ArgumentNullException("credentials");
            }

            _credentials = credentials;
            _token = token;
        }
        #endregion

        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                var credential = _credentials as NetworkCredential;
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("user");
                writer.WriteStartObject();
                writer.WritePropertyName("email");
                writer.WriteValue(credential.UserName);
                writer.WritePropertyName("password");
                writer.WriteValue(credential.Password);
                if (_token != null)
                {
                    writer.WritePropertyName("tfa_token");
                    writer.WriteValue(_token);
                }
                writer.WriteEndObject();
                writer.WriteEndObject();

                string snippet = stringWriter.ToString();
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }

        public override void OnResponse(IResponse httpResponse, ContentstackClientOptions config)
        {
            if (httpResponse.IsSuccessStatusCode)
            {
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
