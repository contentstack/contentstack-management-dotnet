using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents the response from the OAuth app authorization API.
    /// </summary>
    public class OAuthAppAuthorizationResponse
    {
        
        [JsonProperty("data")]
        [JsonPropertyName("data")]
        public OAuthAppAuthorizationData[] Data { get; set; }
    }

    /// <summary>
    /// Represents OAuth app authorization data.
    /// </summary>
    public class OAuthAppAuthorizationData
    {
        
        [JsonProperty("authorization_uid")]
        [JsonPropertyName("authorization_uid")]
        public string AuthorizationUid { get; set; }

        
        [JsonProperty("user")]
        [JsonPropertyName("user")]
        public OAuthUser User { get; set; }
    }

    
    public class OAuthUser
    {
        
        [JsonProperty("uid")]
        [JsonPropertyName("uid")]
        public string Uid { get; set; }
    }
}



