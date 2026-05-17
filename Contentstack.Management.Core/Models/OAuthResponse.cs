using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents the response from OAuth token exchange operations.
    /// </summary>
    public class OAuthResponse
    {
       
        [JsonProperty("access_token")]
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

       
        [JsonProperty("refresh_token")]
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        
        [JsonProperty("expires_in")]
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

       
        [JsonProperty("organization_uid")]
        [JsonPropertyName("organization_uid")]
        public string OrganizationUid { get; set; }

        
        [JsonProperty("user_uid")]
        [JsonPropertyName("user_uid")]
        public string UserUid { get; set; }
    }
}


