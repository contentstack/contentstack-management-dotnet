using System;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.User
{
    internal class LogoutService : ContentstackService
    {
        private readonly string _authtoken;

        #region Constructor
        public LogoutService(JsonSerializer serializer, string authtoken): base(serializer)
        {
            this.HttpMethod = "DELETE";
            this.ResourcePath = "user-session";

            if (string.IsNullOrEmpty(authtoken))
            {
                throw new ArgumentNullException("authtoken", CSConstants.AuthenticationTokenRequired);
            }

            _authtoken = authtoken;
        }
        #endregion

        public override void ContentBody()
        {
            Headers["authtoken"] = _authtoken;
        }

        public override void OnResponse(IResponse httpResponse, ContentstackClientOptions config)
        {
            base.OnResponse(httpResponse, config);
            if (httpResponse.IsSuccessStatusCode && config.Authtoken == _authtoken)
            {
                config.Authtoken = null;
            }
        }
    }
}
