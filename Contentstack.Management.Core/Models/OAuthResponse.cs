using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents the response from OAuth token exchange operations.
    /// </summary>
    public class OAuthResponse
    {
       
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

       
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

       
        [JsonProperty("organization_uid")]
        public string OrganizationUid { get; set; }

        
        [JsonProperty("user_uid")]
        public string UserUid { get; set; }
    }
}


