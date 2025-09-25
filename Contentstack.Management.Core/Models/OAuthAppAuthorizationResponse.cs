using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents the response from the OAuth app authorization API.
    /// </summary>
    public class OAuthAppAuthorizationResponse
    {
        
        [JsonProperty("data")]
        public OAuthAppAuthorizationData[] Data { get; set; }
    }

    /// <summary>
    /// Represents OAuth app authorization data.
    /// </summary>
    public class OAuthAppAuthorizationData
    {
        
        [JsonProperty("authorization_uid")]
        public string AuthorizationUid { get; set; }

        
        [JsonProperty("user")]
        public OAuthUser User { get; set; }
    }

    
    public class OAuthUser
    {
        
        [JsonProperty("uid")]
        public string Uid { get; set; }
    }
}



