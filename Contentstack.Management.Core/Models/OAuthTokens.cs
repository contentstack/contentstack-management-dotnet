using System;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents OAuth tokens stored in memory for cross-SDK access.
    /// This class enables sharing OAuth tokens between the Management SDK and other SDKs like Model Generator.
    /// </summary>
    public class OAuthTokens
    {
        /// <summary>
        /// Gets or sets the access token used for API authentication.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token used to obtain new access tokens.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the access token expires.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the organization UID associated with the OAuth tokens.
        /// </summary>
        public string OrganizationUid { get; set; }

        /// <summary>
        /// Gets or sets the user UID associated with the OAuth tokens.
        /// </summary>
        public string UserUid { get; set; }

        /// <summary>
        /// Gets or sets the OAuth client ID associated with these tokens.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the access token has expired.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        /// <summary>
        /// Gets a value indicating whether the access token needs to be refreshed.
        /// Tokens are considered to need refresh if they expire within 5 minutes or are already expired.
        /// </summary>
        public bool NeedsRefresh => DateTime.UtcNow >= ExpiresAt.AddMinutes(-5) || IsExpired;

        /// <summary>
        /// Gets a value indicating whether the OAuth tokens are valid for use.
        /// Tokens are valid if they have an access token and are not expired.
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(AccessToken) && !IsExpired;
    }
}

