using System;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents the response from OAuth token exchange operations.
    /// </summary>
    public class OAuthResponse
    {
       
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

       
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

       
        [JsonPropertyName("organization_uid")]
        public string OrganizationUid { get; set; }

        
        [JsonPropertyName("user_uid")]
        public string UserUid { get; set; }
    }
}


