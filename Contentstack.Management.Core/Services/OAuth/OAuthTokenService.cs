using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Services.OAuth
{
    /// <summary>
    /// Service class for OAuth token operations including token exchange and refresh.
    /// </summary>
    internal class OAuthTokenService : ContentstackService
    {
        #region Private Fields
        private readonly Dictionary<string, string> _requestBody;
        
        // Constants for OAuth grant types
        private const string AuthorizationCodeGrantType = "authorization_code";
        private const string RefreshTokenGrantType = "refresh_token";
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the OAuthTokenService class.
        /// </summary>
        /// <param name="serializer">The JSON serializer to use.</param>
        /// <param name="requestBody">The request body parameters for the OAuth token request.</param>
        /// <exception cref="ArgumentNullException">Thrown when serializer or requestBody is null.</exception>
        internal OAuthTokenService(JsonSerializer serializer, Dictionary<string, string> requestBody) 
            : base(serializer)
        {
            if (requestBody == null)
                throw new ArgumentNullException(nameof(requestBody), "Request body cannot be null.");

            _requestBody = requestBody;
            HttpMethod = "POST";
            ResourcePath = "token";
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the content body for the OAuth token request.
        /// The body is formatted as application/x-www-form-urlencoded as required by OAuth 2.0.
        /// </summary>
        public override void ContentBody()
        {
            if (_requestBody == null || _requestBody.Count == 0)
            {
                throw new InvalidOperationException("Request body cannot be null or empty for OAuth token requests.");
            }

            // Create form-encoded data as required by OAuth 2.0 specification
            var formData = string.Join("&", _requestBody.Select(kvp => 
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));

            ByteContent = Encoding.UTF8.GetBytes(formData);
        }

        /// <summary>
        /// Creates the HTTP request for OAuth token operations.
        /// Overrides the base implementation to set the correct content type and URL for OAuth requests.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use for the request.</param>
        /// <param name="config">The Contentstack client configuration.</param>
        /// <param name="addAcceptMediaHeader">Whether to add accept media headers.</param>
        /// <param name="apiVersion">The API version to use.</param>
        /// <returns>The HTTP request for OAuth token operations.</returns>
        public override IHttpRequest CreateHttpRequest(System.Net.Http.HttpClient httpClient, ContentstackClientOptions config, bool addAcceptMediaHeader = false, string apiVersion = null)
        {
            // Create a custom config with Developer Hub hostname for OAuth token operations
            // OAuth token endpoints don't use API versioning, so we set Version to empty
            var devHubConfig = new ContentstackClientOptions
            {
                Host = GetDeveloperHubHostname(config.Host),
                Port = config.Port,
                Version = "", // OAuth endpoints don't use versioning
            };
            
            var request = base.CreateHttpRequest(httpClient, devHubConfig, addAcceptMediaHeader, apiVersion);
            
            // OAuth token requests require application/x-www-form-urlencoded content type
            Headers["Content-Type"] = "application/x-www-form-urlencoded";
            
            return request;
        }

        /// <summary>
        /// Transforms the base hostname to the Developer Hub API hostname.
        /// </summary>
        /// <param name="baseHost">The base hostname (e.g., api.contentstack.io)</param>
        /// <returns>The transformed Developer Hub hostname (e.g., developerhub-api.contentstack.com)</returns>
        private static string GetDeveloperHubHostname(string baseHost)
        {
            if (string.IsNullOrEmpty(baseHost))
                return baseHost;

            // Transform api.contentstack.io -> developerhub-api.contentstack.com
            var devHubHost = baseHost;
            
            // Replace 'api' with 'developerhub-api'
            if (devHubHost.Contains("api."))
            {
                devHubHost = devHubHost.Replace("api.", "developerhub-api.");
            }
            
            // Replace .io with .com
            if (devHubHost.EndsWith(".io"))
            {
                devHubHost = devHubHost.Replace(".io", ".com");
            }
            
            // Ensure https:// protocol
            if (!devHubHost.StartsWith("http"))
            {
                devHubHost = "https://" + devHubHost;
            }
            
            return devHubHost;
        }

        /// <summary>
        /// Handles the response from OAuth token operations.
        /// This method is called after the HTTP request completes.
        /// </summary>
        /// <param name="httpResponse">The HTTP response from the OAuth token request.</param>
        /// <param name="config">The Contentstack client configuration.</param>
        public override void OnResponse(IResponse httpResponse, ContentstackClientOptions config)
        {
            // OAuth token service doesn't need to modify the client configuration
            // The response handling is done by the OAuthHandler class
            // This method is provided for future extensibility
        }
        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Creates an OAuth token service for authorization code exchange.
        /// </summary>
        /// <param name="serializer">The JSON serializer to use.</param>
        /// <param name="authorizationCode">The authorization code received from the OAuth provider.</param>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <param name="redirectUri">The redirect URI used in the authorization request.</param>
        /// <param name="clientSecret">The OAuth client secret (optional, for traditional OAuth flow).</param>
        /// <param name="codeVerifier">The PKCE code verifier (optional, for PKCE flow).</param>
        /// <returns>An OAuth token service configured for authorization code exchange.</returns>
        public static OAuthTokenService CreateForAuthorizationCode(
            JsonSerializer serializer,
            string authorizationCode,
            string clientId,
            string redirectUri,
            string clientSecret = null,
            string codeVerifier = null)
        {
            if (string.IsNullOrEmpty(authorizationCode))
                throw new ArgumentException("Authorization code cannot be null or empty.", nameof(authorizationCode));
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));
            if (string.IsNullOrEmpty(redirectUri))
                throw new ArgumentException("Redirect URI cannot be null or empty.", nameof(redirectUri));

            var requestBody = new Dictionary<string, string>
            {
                ["grant_type"] = AuthorizationCodeGrantType,
                ["code"] = authorizationCode,
                ["redirect_uri"] = redirectUri,
                ["client_id"] = clientId
            };

            // Add either client_secret (traditional OAuth) or code_verifier (PKCE)
            if (!string.IsNullOrEmpty(clientSecret))
            {
                requestBody["client_secret"] = clientSecret;
            }
            else if (!string.IsNullOrEmpty(codeVerifier))
            {
                requestBody["code_verifier"] = codeVerifier;
            }
            else
            {
                throw new ArgumentException("Either client_secret or code_verifier must be provided.");
            }

            return new OAuthTokenService(serializer, requestBody);
        }

        /// <summary>
        /// Creates an OAuth token service for token refresh.
        /// </summary>
        /// <param name="serializer">The JSON serializer to use.</param>
        /// <param name="refreshToken">The refresh token to use for obtaining new access tokens.</param>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <param name="redirectUri">The redirect URI used in the original authorization request.</param>
        /// <returns>An OAuth token service configured for token refresh.</returns>
        public static OAuthTokenService CreateForTokenRefresh(
            JsonSerializer serializer,
            string refreshToken,
            string clientId,
            string redirectUri)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException("Refresh token cannot be null or empty.", nameof(refreshToken));
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));
            if (string.IsNullOrEmpty(redirectUri))
                throw new ArgumentException("Redirect URI cannot be null or empty.", nameof(redirectUri));

            var requestBody = new Dictionary<string, string>
            {
                ["grant_type"] = RefreshTokenGrantType,
                ["refresh_token"] = refreshToken,
                ["client_id"] = clientId,
                ["redirect_uri"] = redirectUri
            };

            return new OAuthTokenService(serializer, requestBody);
        }

        /// <summary>
        /// Creates an OAuth token service for token refresh with optional client secret.
        /// This method supports both PKCE flow (without client secret) and traditional OAuth flow (with client secret).
        /// </summary>
        /// <param name="serializer">The JSON serializer to use.</param>
        /// <param name="refreshToken">The refresh token to use for obtaining new access tokens.</param>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <param name="clientSecret">The OAuth client secret (optional, for traditional OAuth flow).</param>
        /// <returns>An OAuth token service configured for token refresh.</returns>
        public static OAuthTokenService CreateForRefreshToken(
            JsonSerializer serializer,
            string refreshToken,
            string clientId,
            string clientSecret = null)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException("Refresh token cannot be null or empty.", nameof(refreshToken));
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));

            var requestBody = new Dictionary<string, string>
            {
                ["grant_type"] = RefreshTokenGrantType,
                ["refresh_token"] = refreshToken,
                ["client_id"] = clientId
            };

            // Add client secret for traditional OAuth flow
            if (!string.IsNullOrEmpty(clientSecret))
            {
                requestBody["client_secret"] = clientSecret;
            }

            return new OAuthTokenService(serializer, requestBody);
        }
        #endregion
    }
}
