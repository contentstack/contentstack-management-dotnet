using System;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents OAuth tokens stored in memory for cross-SDK access.
    /// This class enables sharing OAuth tokens between the Management SDK and other SDKs
    /// </summary>
    public class OAuthTokens
    {
 
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string OrganizationUid { get; set; }

        public string UserUid { get; set; }

        public string ClientId { get; set; }

        public string AppId { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public bool NeedsRefresh => DateTime.UtcNow >= ExpiresAt.AddMinutes(-5) || IsExpired;

        public bool IsValid => !string.IsNullOrEmpty(AccessToken) && !IsExpired;
    }
}

