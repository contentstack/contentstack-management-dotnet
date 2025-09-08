using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents the response from the OAuth app authorization API.
    /// </summary>
    public class OAuthAppAuthorizationResponse
    {
        /// <summary>
        /// Array of OAuth app authorization data.
        /// </summary>
        [JsonProperty("data")]
        public OAuthAppAuthorizationData[] Data { get; set; }
    }

    /// <summary>
    /// Represents OAuth app authorization data.
    /// </summary>
    public class OAuthAppAuthorizationData
    {
        /// <summary>
        /// The authorization UID.
        /// </summary>
        [JsonProperty("authorization_uid")]
        public string AuthorizationUid { get; set; }

        /// <summary>
        /// The user information.
        /// </summary>
        [JsonProperty("user")]
        public OAuthUser User { get; set; }
    }

    /// <summary>
    /// Represents OAuth user information.
    /// </summary>
    public class OAuthUser
    {
        /// <summary>
        /// The user UID.
        /// </summary>
        [JsonProperty("uid")]
        public string Uid { get; set; }
    }
}



