using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents the response from OAuth token exchange operations.
    /// </summary>
    public class OAuthResponse
    {
        /// <summary>
        /// The access token used for API authentication.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token used to obtain new access tokens.
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// The number of seconds until the access token expires.
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// The organization UID associated with the OAuth tokens.
        /// </summary>
        [JsonProperty("organization_uid")]
        public string OrganizationUid { get; set; }

        /// <summary>
        /// The user UID associated with the OAuth tokens.
        /// </summary>
        [JsonProperty("user_uid")]
        public string UserUid { get; set; }

        /// <summary>
        /// The type of authorization (e.g., "oauth").
        /// </summary>
        [JsonProperty("authorization_type")]
        public string AuthorizationType { get; set; }

        /// <summary>
        /// The stack API key associated with the OAuth tokens.
        /// </summary>
        [JsonProperty("stack_api_key")]
        public string StackApiKey { get; set; }
    }
}


