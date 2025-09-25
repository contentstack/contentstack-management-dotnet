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

        public bool IsExpired => ExpiresAt == DateTime.MinValue || DateTime.UtcNow >= ExpiresAt;

        public bool NeedsRefresh 
        {
            get
            {
                // If ExpiresAt is not set or is MinValue, consider it expired
                if (ExpiresAt == DateTime.MinValue)
                    return true;
                
                try
                {
                    // Check if we need to refresh (5 minutes before expiration)
                    var refreshTime = ExpiresAt.AddMinutes(-5);
                    return DateTime.UtcNow >= refreshTime || IsExpired;
                }
                catch (ArgumentOutOfRangeException)
                {
                    // If the calculation results in an unrepresentable DateTime, consider it expired
                    return true;
                }
            }
        }

        public bool IsValid => !string.IsNullOrEmpty(AccessToken) && !IsExpired;
    }
}

