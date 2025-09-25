using System;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Configuration options for OAuth authentication.
    /// </summary>
    public class OAuthOptions
    {
        /// <summary>
        /// The OAuth application ID. Defaults to the Contentstack app ID.
        /// </summary>
        public string AppId { get; set; } = "6400aa06db64de001a31c8a9";

        /// <summary>
        /// The OAuth client ID. Defaults to the Contentstack client ID.
        /// </summary>
        public string ClientId { get; set; } = "Ie0FEfTzlfAHL4xM";

        /// <summary>
        /// The redirect URI for OAuth callbacks. Defaults to localhost:8184.
        /// </summary>
        public string RedirectUri { get; set; } = "http://localhost:8184";

        /// <summary>
        /// The OAuth client secret. If provided, PKCE flow will be skipped.
        /// If null or empty, PKCE flow will be used for enhanced security.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The OAuth response type. Defaults to "code" for authorization code flow.
        /// </summary>
        public string ResponseType { get; set; } = "code";

        /// <summary>
        /// The OAuth scopes to request. Optional array of permission scopes.
        /// </summary>
        public string[] Scope { get; set; }

        /// <summary>
        /// Indicates whether PKCE (Proof Key for Code Exchange) flow should be used.
        /// This is automatically determined based on whether ClientSecret is provided.
        /// </summary>
        public bool UsePkce => string.IsNullOrEmpty(ClientSecret);

        /// <summary>
        /// Validates the OAuth options configuration.
        /// </summary>
        /// <returns>True if the configuration is valid, false otherwise.</returns>
        public bool IsValid()
        {
            return IsValid(out _);
        }

        /// <summary>
        /// Validates the OAuth options configuration and provides detailed error information.
        /// </summary>
        /// <param name="errorMessage">The validation error message if validation fails.</param>
        /// <returns>True if the configuration is valid, false otherwise.</returns>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(AppId))
            {
                errorMessage = "AppId is required for OAuth configuration.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ClientId))
            {
                errorMessage = "ClientId is required for OAuth configuration.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(RedirectUri))
            {
                errorMessage = "RedirectUri is required for OAuth configuration.";
                return false;
            }

            if (!Uri.TryCreate(RedirectUri, UriKind.Absolute, out var redirectUri))
            {
                errorMessage = "RedirectUri must be a valid absolute URI.";
                return false;
            }

            if (redirectUri.Scheme != "http" && redirectUri.Scheme != "https")
            {
                errorMessage = "RedirectUri must use http or https scheme.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ResponseType))
            {
                errorMessage = "ResponseType is required for OAuth configuration.";
                return false;
            }

            if (ResponseType != "code")
            {
                errorMessage = "ResponseType must be 'code' for authorization code flow.";
                return false;
            }

            // For traditional OAuth flow (non-PKCE), client secret is required
            if (!UsePkce && string.IsNullOrWhiteSpace(ClientSecret))
            {
                errorMessage = "ClientSecret is required for traditional OAuth flow. Use PKCE flow (leave ClientSecret empty) for public clients.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the OAuth options configuration and throws an exception if invalid.
        /// </summary>
        /// <exception cref="OAuthConfigurationException">Thrown when the configuration is invalid.</exception>
        public void Validate()
        {
            if (!IsValid(out var errorMessage))
            {
                throw new Exceptions.OAuthConfigurationException(errorMessage);
            }
        }

        /// <summary>
        /// Gets a string representation of the OAuth options for debugging.
        /// </summary>
        /// <returns>A string representation of the OAuth options.</returns>
        public override string ToString()
        {
            return $"OAuthOptions: AppId={AppId}, ClientId={ClientId}, RedirectUri={RedirectUri}, " +
                   $"ResponseType={ResponseType}, UsePkce={UsePkce}, HasScope={Scope?.Length > 0}";
        }
    }
}
