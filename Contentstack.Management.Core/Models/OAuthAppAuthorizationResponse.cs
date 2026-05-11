using System;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents the response from the OAuth app authorization API.
    /// </summary>
    public class OAuthAppAuthorizationResponse
    {
        
        [JsonPropertyName("data")]
        public OAuthAppAuthorizationData[] Data { get; set; }
    }

    /// <summary>
    /// Represents OAuth app authorization data.
    /// </summary>
    public class OAuthAppAuthorizationData
    {
        
        [JsonPropertyName("authorization_uid")]
        public string AuthorizationUid { get; set; }

        
        [JsonPropertyName("user")]
        public OAuthUser User { get; set; }
    }

    
    public class OAuthUser
    {
        
        [JsonPropertyName("uid")]
        public string Uid { get; set; }
    }
}



