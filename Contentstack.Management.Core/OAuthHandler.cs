using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Utils;
using Contentstack.Management.Core.Services.OAuth;

namespace Contentstack.Management.Core
{
    /// <summary>
    /// Handles OAuth 2.0 authentication flow for Contentstack Management API.
    /// Supports both traditional OAuth flow (with client secret) and PKCE flow (without client secret).
    /// </summary>
    public class OAuthHandler
    {
        #region Private Fields
        private readonly ContentstackClient _client;
        private readonly OAuthOptions _options;
        private readonly string _clientId;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the OAuthHandler class.
        /// </summary>
        /// <param name="client">The Contentstack client instance.</param>
        /// <param name="options">The OAuth configuration options.</param>
        /// <exception cref="ArgumentNullException">Thrown when client or options is null.</exception>
        /// <exception cref="ArgumentException">Thrown when options are invalid.</exception>
        public OAuthHandler(ContentstackClient client, OAuthOptions options)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client), "Contentstack client cannot be null.");

            if (options == null)
                throw new ArgumentNullException(nameof(options), "OAuth options cannot be null.");

            // Validate OAuth options and throw specific exception if invalid
            options.Validate();

            _client = client;
            _options = options;
            _clientId = options.ClientId;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the OAuth client ID.
        /// </summary>
        public string ClientId => _clientId;

        /// <summary>
        /// Gets the OAuth application ID.
        /// </summary>
        public string AppId => _options.AppId;

        /// <summary>
        /// Gets the redirect URI for OAuth callbacks.
        /// </summary>
        public string RedirectUri => _options.RedirectUri;

        /// <summary>
        /// Gets a value indicating whether PKCE flow is being used.
        /// </summary>
        public bool UsePkce => _options.UsePkce;

        /// <summary>
        /// Gets the OAuth scopes.
        /// </summary>
        public string[] Scope => _options.Scope;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the current OAuth tokens for this client.
        /// </summary>
        /// <returns>The OAuth tokens if available, null otherwise.</returns>
        public OAuthTokens GetCurrentTokens()
        {
            return _client.GetStoredOAuthTokens(_clientId);
        }

        /// <summary>
        /// Checks if valid OAuth tokens are available.
        /// </summary>
        /// <returns>True if valid tokens are available, false otherwise.</returns>
        public bool HasValidTokens()
        {
            var tokens = _client.GetStoredOAuthTokens(_clientId);
            return tokens?.IsValid == true;
        }

        /// <summary>
        /// Checks if OAuth tokens exist (regardless of validity).
        /// </summary>
        /// <returns>True if tokens exist, false otherwise.</returns>
        public bool HasTokens()
        {
            return _client.HasStoredOAuthTokens(_clientId);
        }

        /// <summary>
        /// Clears the OAuth tokens for this client.
        /// </summary>
        public void ClearTokens()
        {
            _client.ClearStoredOAuthTokens(_clientId);
        }

        /// <summary>
        /// Stores OAuth tokens in the client.
        /// </summary>
        /// <param name="tokens">The OAuth tokens to store.</param>
        private void StoreTokens(OAuthTokens tokens)
        {
            _client.StoreOAuthTokens(_clientId, tokens);
        }

        /// <summary>
        /// Gets a string representation of the OAuth handler for debugging.
        /// </summary>
        /// <returns>A string representation of the OAuth handler.</returns>
        public override string ToString()
        {
            return $"OAuthHandler: ClientId={_clientId}, AppId={_options.AppId}, UsePkce={_options.UsePkce}, HasTokens={HasTokens()}";
        }

        #region Token Getter Methods
        /// <summary>
        /// Gets the current access token.
        /// </summary>
        /// <returns>The access token if available, null otherwise.</returns>
        public string GetAccessToken()
        {
            var tokens = GetCurrentTokens();
            return tokens?.AccessToken;
        }

        /// <summary>
        /// Gets the current refresh token.
        /// </summary>
        /// <returns>The refresh token if available, null otherwise.</returns>
        public string GetRefreshToken()
        {
            var tokens = GetCurrentTokens();
            return tokens?.RefreshToken;
        }

        /// <summary>
        /// Gets the current organization UID.
        /// </summary>
        /// <returns>The organization UID if available, null otherwise.</returns>
        public string GetOrganizationUID()
        {
            var tokens = GetCurrentTokens();
            return tokens?.OrganizationUid;
        }

        /// <summary>
        /// Gets the current user UID.
        /// </summary>
        /// <returns>The user UID if available, null otherwise.</returns>
        public string GetUserUID()
        {
            var tokens = GetCurrentTokens();
            return tokens?.UserUid;
        }

        /// <summary>
        /// Gets the current token expiry time.
        /// </summary>
        /// <returns>The token expiry time if available, null otherwise.</returns>
        public DateTime? GetTokenExpiryTime()
        {
            var tokens = GetCurrentTokens();
            return tokens?.ExpiresAt;
        }
        #endregion

        #region Token Setter Methods
        /// <summary>
        /// Sets the access token in the stored OAuth tokens.
        /// </summary>
        /// <param name="token">The access token to set.</param>
        /// <exception cref="ArgumentException">Thrown when the token is null or empty.</exception>
        public void SetAccessToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Access token cannot be null or empty.", nameof(token));

            var tokens = GetCurrentTokens();
            if (tokens == null)
            {
                tokens = new OAuthTokens { ClientId = _clientId };
            }
            tokens.AccessToken = token;
            StoreTokens(tokens);
        }

        /// <summary>
        /// Sets the refresh token in the stored OAuth tokens.
        /// </summary>
        /// <param name="token">The refresh token to set.</param>
        /// <exception cref="ArgumentException">Thrown when the token is null or empty.</exception>
        public void SetRefreshToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Refresh token cannot be null or empty.", nameof(token));

            var tokens = GetCurrentTokens();
            if (tokens == null)
            {
                tokens = new OAuthTokens { ClientId = _clientId };
            }
            tokens.RefreshToken = token;
            StoreTokens(tokens);
        }

        /// <summary>
        /// Sets the organization UID in the stored OAuth tokens.
        /// </summary>
        /// <param name="organizationUID">The organization UID to set.</param>
        /// <exception cref="ArgumentException">Thrown when the organization UID is null or empty.</exception>
        public void SetOrganizationUID(string organizationUID)
        {
            if (string.IsNullOrEmpty(organizationUID))
                throw new ArgumentException("Organization UID cannot be null or empty.", nameof(organizationUID));

            var tokens = GetCurrentTokens();
            if (tokens == null)
            {
                tokens = new OAuthTokens { ClientId = _clientId };
            }
            tokens.OrganizationUid = organizationUID;
            StoreTokens(tokens);
        }

        /// <summary>
        /// Sets the user UID in the stored OAuth tokens.
        /// </summary>
        /// <param name="userUID">The user UID to set.</param>
        /// <exception cref="ArgumentException">Thrown when the user UID is null or empty.</exception>
        public void SetUserUID(string userUID)
        {
            if (string.IsNullOrEmpty(userUID))
                throw new ArgumentException("User UID cannot be null or empty.", nameof(userUID));

            var tokens = GetCurrentTokens();
            if (tokens == null)
            {
                tokens = new OAuthTokens { ClientId = _clientId };
            }
            tokens.UserUid = userUID;
            StoreTokens(tokens);
        }

        /// <summary>
        /// Sets the token expiry time in the stored OAuth tokens.
        /// </summary>
        /// <param name="expiryTime">The token expiry time to set.</param>
        /// <exception cref="ArgumentException">Thrown when the expiry time is not provided.</exception>
        public void SetTokenExpiryTime(DateTime expiryTime)
        {
            if (expiryTime == default(DateTime))
                throw new ArgumentException("Token expiry time cannot be default value.", nameof(expiryTime));

            var tokens = GetCurrentTokens();
            if (tokens == null)
            {
                tokens = new OAuthTokens { ClientId = _clientId };
            }
            tokens.ExpiresAt = expiryTime;
            StoreTokens(tokens);
        }
        #endregion

        #endregion

        #region Protected Methods
        /// <summary>
        /// Gets the Contentstack client instance.
        /// </summary>
        /// <returns>The Contentstack client instance.</returns>
        protected ContentstackClient GetClient()
        {
            return _client;
        }

        /// <summary>
        /// Gets the OAuth options.
        /// </summary>
        /// <returns>The OAuth options.</returns>
        protected OAuthOptions GetOptions()
        {
            return _options;
        }
        #endregion

        #region OAuth Flow Methods
        /// <summary>
        /// Generates the OAuth authorization URL for user authentication (async version).
        /// This URL should be opened in a browser to start the OAuth flow.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the authorization URL.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the OAuth configuration is invalid.</exception>
        /// <exception cref="CryptographicException">Thrown when PKCE code generation fails.</exception>
        public async Task<string> AuthorizeAsync()
        {
            return await Task.FromResult(GetAuthorizationUrl());
        }

        /// <summary>
        /// Generates the OAuth authorization URL for user authentication.
        /// This URL should be opened in a browser to start the OAuth flow.
        /// </summary>
        /// <returns>The authorization URL.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the OAuth configuration is invalid.</exception>
        /// <exception cref="CryptographicException">Thrown when PKCE code generation fails.</exception>
        [Obsolete("Use AuthorizeAsync()")]
        public string GetAuthorizationUrl()
        {
            // AppId validation is now handled by OAuthOptions.Validate() in constructor

            try
            {
                // Build the base authorization URL using the correct OAuth hostname
                // Transform api.contentstack.io -> app.contentstack.com for OAuth authorization
                var oauthHost = GetOAuthHost(GetClient().contentstackOptions.Host);
                var baseUrl = $"{oauthHost}/#!/apps/{_options.AppId}/authorize";
                var authUrl = new UriBuilder(baseUrl);

                // Add required OAuth parameters
                var queryParams = new List<string>
                {
                    $"response_type={Uri.EscapeDataString(_options.ResponseType)}",
                    $"client_id={Uri.EscapeDataString(_options.ClientId)}",
                    $"redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}"
                };

                // Add scopes if provided
                if (_options.Scope != null && _options.Scope.Length > 0)
                {
                    var scopeString = string.Join(" ", _options.Scope);
                    queryParams.Add($"scope={Uri.EscapeDataString(scopeString)}");
                }

                // Handle PKCE vs Traditional OAuth flow
                if (_options.UsePkce)
                {
                    // PKCE flow - generate code verifier and challenge
                    var codeVerifier = PkceHelper.GenerateCodeVerifier();
                    var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

                    // Add PKCE parameters
                    queryParams.Add($"code_challenge={Uri.EscapeDataString(codeChallenge)}");
                    queryParams.Add("code_challenge_method=S256");

                    // Store code verifier temporarily for later use in token exchange
                    var tempTokens = new OAuthTokens
                    {
                        ClientId = _clientId,
                        AccessToken = codeVerifier // Temporarily store code verifier
                    };
                    StoreTokens(tempTokens);
                }
                // Traditional OAuth flow - no additional parameters needed

                // Build the complete URL
                authUrl.Query = string.Join("&", queryParams);
                return authUrl.ToString();
            }
            catch (Exception ex) when (!(ex is Exceptions.OAuthException))
            {
                throw new Exceptions.OAuthAuthorizationException($"Failed to generate OAuth authorization URL: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exchanges an authorization code for OAuth access and refresh tokens.
        /// This method should be called after the user completes the OAuth authorization flow.
        /// </summary>
        /// <param name="authorizationCode">The authorization code received from the OAuth provider.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the OAuth tokens.</returns>
        /// <exception cref="ArgumentException">Thrown when the authorization code is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the OAuth configuration is invalid or code verifier is missing.</exception>
        /// <exception cref="HttpRequestException">Thrown when the token exchange request fails.</exception>
        public async Task<OAuthTokens> ExchangeCodeForTokenAsync(string authorizationCode)
        {
            if (string.IsNullOrEmpty(authorizationCode))
            {
                throw new ArgumentException("Authorization code cannot be null or empty.", nameof(authorizationCode));
            }

            try
            {
                // Create the OAuth token service for authorization code exchange
                OAuthTokenService tokenService;

                if (_options.UsePkce)
                {
                    // PKCE flow - get stored code verifier
                    var storedTokens = GetCurrentTokens();
                    if (storedTokens?.AccessToken == null)
                    {
                        throw new Exceptions.OAuthTokenException(
                            "Code verifier not found. Please call GetAuthorizationUrl() first to generate the authorization URL and code verifier.");
                    }

                    var codeVerifier = storedTokens.AccessToken; // This is the stored code verifier
                    tokenService = OAuthTokenService.CreateForAuthorizationCode(
                        serializer: GetClient().serializer,
                        authorizationCode: authorizationCode,
                        clientId: _options.ClientId,
                        redirectUri: _options.RedirectUri,
                        codeVerifier: codeVerifier
                    );
                }
                else
                {
                    // Traditional OAuth flow - use client secret
                    if (string.IsNullOrEmpty(_options.ClientSecret))
                    {
                        throw new Exceptions.OAuthConfigurationException(
                            "Client secret is required for traditional OAuth flow. Please set the ClientSecret in OAuth options or use PKCE flow.");
                    }

                    tokenService = OAuthTokenService.CreateForAuthorizationCode(
                        serializer: GetClient().serializer,
                        authorizationCode: authorizationCode,
                        clientId: _options.ClientId,
                        redirectUri: _options.RedirectUri,
                        clientSecret: _options.ClientSecret
                    );
                }

                // Make the token exchange request
                var response = await GetClient().InvokeAsync<OAuthTokenService, ContentstackResponse>(tokenService);

                // Parse the OAuth response from the ContentstackResponse
                var oauthResponse = response.OpenTResponse<OAuthResponse>();

                // Create OAuth tokens from the response
                var tokens = new OAuthTokens
                {
                    AccessToken = oauthResponse.AccessToken,
                    RefreshToken = oauthResponse.RefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(oauthResponse.ExpiresIn - 60), // 60 second buffer
                    OrganizationUid = oauthResponse.OrganizationUid,
                    UserUid = oauthResponse.UserUid,
                    ClientId = _clientId
                };

                // Store tokens in memory for future use
                StoreTokens(tokens);

                // Set OAuth tokens in the client for authenticated requests
                GetClient().SetOAuthTokens(tokens);

                return tokens;
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is Exceptions.OAuthException))
            {
                throw new Exceptions.OAuthTokenException($"Failed to exchange authorization code for tokens: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Refreshes the OAuth access token using the refresh token.
        /// This method automatically handles token refresh and updates the stored tokens.
        /// </summary>
        /// <param name="refreshToken">The refresh token to use. If null, uses the stored refresh token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the new OAuth tokens.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no refresh token is available or the refresh request fails.</exception>
        /// <exception cref="HttpRequestException">Thrown when the token refresh request fails.</exception>
        public async Task<OAuthTokens> RefreshTokenAsync(string refreshToken = null)
        {
            // Get the refresh token to use
            string tokenToUse = refreshToken;
            
            if (string.IsNullOrEmpty(tokenToUse))
            {
                // Get refresh token from stored tokens
                var storedTokens = GetCurrentTokens();
                if (storedTokens?.RefreshToken == null)
                {
                    throw new Exceptions.OAuthTokenRefreshException(
                        "No refresh token available. Please provide a refresh token or ensure tokens are stored from a previous OAuth flow.");
                }
                tokenToUse = storedTokens.RefreshToken;
            }

            try
            {
                // Create the OAuth token service for token refresh
                OAuthTokenService tokenService;

                if (_options.UsePkce)
                {
                    // PKCE flow - no client secret needed
                    tokenService = OAuthTokenService.CreateForRefreshToken(
                        serializer: GetClient().serializer,
                        refreshToken: tokenToUse,
                        clientId: _options.ClientId
                    );
                }
                else
                {
                    // Traditional OAuth flow - use client secret
                    if (string.IsNullOrEmpty(_options.ClientSecret))
                    {
                        throw new Exceptions.OAuthConfigurationException(
                            "Client secret is required for traditional OAuth flow. Please set the ClientSecret in OAuth options or use PKCE flow.");
                    }

                    tokenService = OAuthTokenService.CreateForRefreshToken(
                        serializer: GetClient().serializer,
                        refreshToken: tokenToUse,
                        clientId: _options.ClientId,
                        clientSecret: _options.ClientSecret
                    );
                }

                // Make the token refresh request
                var response = await GetClient().InvokeAsync<OAuthTokenService, ContentstackResponse>(tokenService);

                // Parse the OAuth response from the ContentstackResponse
                var oauthResponse = response.OpenTResponse<OAuthResponse>();

                // Create new OAuth tokens from the response
                var newTokens = new OAuthTokens
                {
                    AccessToken = oauthResponse.AccessToken,
                    RefreshToken = oauthResponse.RefreshToken ?? tokenToUse, // Keep existing refresh token if not provided
                    ExpiresAt = DateTime.UtcNow.AddSeconds(oauthResponse.ExpiresIn - 60), // 60 second buffer
                    OrganizationUid = oauthResponse.OrganizationUid,
                    UserUid = oauthResponse.UserUid,
                    ClientId = _clientId
                };

                // Store the new tokens in memory
                StoreTokens(newTokens);

                // Set OAuth tokens in the client for authenticated requests
                GetClient().SetOAuthTokens(newTokens);

                return newTokens;
            }
            catch (Exception ex) when (!(ex is Exceptions.OAuthException))
            {
                throw new Exceptions.OAuthTokenRefreshException($"Failed to refresh OAuth tokens: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Logs out the user by clearing OAuth tokens and resetting the client authentication state.
        /// This method clears the stored tokens and resets the client to use traditional authentication.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a success message.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no OAuth tokens are available to logout.</exception>
        public async Task<string> LogoutAsync()
        {
            try
            {
                // Check if we have tokens to logout
                var currentTokens = GetCurrentTokens();
                if (currentTokens == null)
                {
                    throw new Exceptions.OAuthException("No OAuth tokens found. User is not logged in via OAuth.");
                }

                // Try to revoke the OAuth app authorization (optional - if it fails, we still clear tokens)
                // Only attempt revocation if we have valid tokens
                if (currentTokens != null && !string.IsNullOrEmpty(currentTokens.AccessToken))
                {
                    try
                    {
                        var authorizationId = await GetOauthAppAuthorizationAsync();
                        await RevokeOauthAppAuthorizationAsync(authorizationId);
                    }
                    catch (Exception ex)
                    {
                        // Log the revocation failure but don't fail the logout
                        // This is common in OAuth implementations where revocation is optional
                        System.Diagnostics.Debug.WriteLine($"OAuth authorization revocation failed (non-critical): {ex.Message}");
                    }
                }

                // Clear tokens from memory store
                ClearTokens();

                // Clear OAuth tokens from the client
                GetClient().ClearOAuthTokens(_clientId);

                // Return success message
                return "Logged out successfully";
            }
            catch (Exception ex) when (!(ex is Exceptions.OAuthException))
            {
                throw new InvalidOperationException($"Failed to logout: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Logs out the user synchronously by clearing OAuth tokens and resetting the client authentication state.
        /// This method clears the stored tokens and resets the client to use traditional authentication.
        /// </summary>
        /// <returns>A success message.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no OAuth tokens are available to logout.</exception>
        public string Logout()
        {
            try
            {
                // Check if we have tokens to logout
                var currentTokens = GetCurrentTokens();
                if (currentTokens == null)
                {
                    throw new Exceptions.OAuthException("No OAuth tokens found. User is not logged in via OAuth.");
                }

                // Clear tokens from memory store
                ClearTokens();

                // Clear OAuth tokens from the client
                GetClient().ClearOAuthTokens(_clientId);

                // Return success message
                return "Logged out successfully";
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                throw new InvalidOperationException($"Failed to logout: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Handles the redirect URL after OAuth authorization and exchanges the authorization code for tokens.
        /// </summary>
        /// <param name="url">The redirect URL containing the authorization code.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Thrown when the URL is null or empty.</exception>
        /// <exception cref="Exceptions.OAuthException">Thrown when the authorization code is not found in the URL.</exception>
        /// <exception cref="Exceptions.OAuthTokenException">Thrown when token exchange fails.</exception>
        public async Task HandleRedirectAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Redirect URL cannot be null or empty.", nameof(url));

            try
            {
                // Parse the URL to extract the authorization code
                var uri = new Uri(url);
                var query = uri.Query.TrimStart('?');
                var queryParams = new Dictionary<string, string>();
                
                if (!string.IsNullOrEmpty(query))
                {
                    foreach (var param in query.Split('&'))
                    {
                        var parts = param.Split('=');
                        if (parts.Length == 2)
                        {
                            queryParams[Uri.UnescapeDataString(parts[0])] = Uri.UnescapeDataString(parts[1]);
                        }
                    }
                }
                
                var code = queryParams.ContainsKey("code") ? queryParams["code"] : null;

                if (string.IsNullOrEmpty(code))
                {
                    throw new Exceptions.OAuthException("Authorization code not found in redirect URL.");
                }

                // Exchange the authorization code for tokens
                await ExchangeCodeForTokenAsync(code);
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is Exceptions.OAuthException))
            {
                throw new Exceptions.OAuthTokenException($"Failed to handle redirect URL: {ex.Message}", ex);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Transforms the base hostname to the appropriate OAuth hostname.
        /// </summary>
        /// <param name="baseHost">The base hostname (e.g., api.contentstack.io)</param>
        /// <returns>The transformed OAuth hostname (e.g., app.contentstack.com)</returns>
        private static string GetOAuthHost(string baseHost)
        {
            if (string.IsNullOrEmpty(baseHost))
                return baseHost;

            // Transform api.contentstack.io -> app.contentstack.com
            var oauthHost = baseHost;
            
            // Replace .io with .com
            if (oauthHost.EndsWith(".io"))
            {
                oauthHost = oauthHost.Replace(".io", ".com");
            }
            
            // Replace 'api' with 'app'
            if (oauthHost.Contains("api."))
            {
                oauthHost = oauthHost.Replace("api.", "app.");
            }
            
            return oauthHost;
        }

        /// <summary>
        /// Gets the OAuth app authorization for the current user.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the authorization ID.</returns>
        /// <exception cref="Exceptions.OAuthException">Thrown when no authorization is found.</exception>
        private async Task<string> GetOauthAppAuthorizationAsync()
        {
            var tokens = GetCurrentTokens();
            if (tokens == null)
            {
                throw new Exceptions.OAuthException("No OAuth tokens found. User is not logged in via OAuth.");
            }

            try
            {
                // Create a service to get OAuth app authorizations
                var service = new Services.OAuth.OAuthAppAuthorizationService(
                    GetClient().serializer,
                    _options.AppId,
                    tokens.OrganizationUid
                );

                // Configure the client with OAuth access token for this request
                var originalAuthtoken = GetClient().contentstackOptions.Authtoken;
                var originalIsOAuthToken = GetClient().contentstackOptions.IsOAuthToken;
                
                GetClient().contentstackOptions.Authtoken = tokens.AccessToken;
                GetClient().contentstackOptions.IsOAuthToken = true;
                
                try
                {
                    // Make the API call to get authorizations
                    var response = await GetClient().InvokeAsync<Services.OAuth.OAuthAppAuthorizationService, ContentstackResponse>(service);
                    var authResponse = response.OpenTResponse<Models.OAuthAppAuthorizationResponse>();

                    if (authResponse?.Data?.Length > 0)
                    {
                        var userUid = tokens.UserUid;
                        var currentUserAuthorization = authResponse.Data.FirstOrDefault(auth => auth.User?.Uid == userUid);
                        
                        if (currentUserAuthorization == null)
                        {
                            throw new Exceptions.OAuthException("No authorizations found for current user!");
                        }
                        
                        return currentUserAuthorization.AuthorizationUid;
                    }
                    else
                    {
                        throw new Exceptions.OAuthException("No authorizations found for the app!");
                    }
                }
                finally
                {
                    // Restore original client configuration
                    GetClient().contentstackOptions.Authtoken = originalAuthtoken;
                    GetClient().contentstackOptions.IsOAuthToken = originalIsOAuthToken;
                }
            }
            catch (Exception ex) when (!(ex is Exceptions.OAuthException))
            {
                throw new Exceptions.OAuthException($"Failed to get OAuth app authorization: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Revokes the OAuth app authorization for the current user.
        /// </summary>
        /// <param name="authorizationId">The authorization ID to revoke.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Thrown when the authorization ID is null or empty.</exception>
        /// <exception cref="Exceptions.OAuthException">Thrown when revocation fails.</exception>
        private async Task RevokeOauthAppAuthorizationAsync(string authorizationId)
        {
            if (string.IsNullOrEmpty(authorizationId))
                throw new ArgumentException("Authorization ID cannot be null or empty.", nameof(authorizationId));

            try
            {
                // Get current tokens to access organization UID
                var tokens = GetCurrentTokens();
                var organizationUid = tokens?.OrganizationUid;

                // Create a service to revoke OAuth app authorization
                var service = new Services.OAuth.OAuthAppRevocationService(
                    GetClient().serializer,
                    _options.AppId,
                    authorizationId,
                    organizationUid
                );

                // Configure the client with OAuth access token for this request
                var originalAuthtoken = GetClient().contentstackOptions.Authtoken;
                var originalIsOAuthToken = GetClient().contentstackOptions.IsOAuthToken;
                
                GetClient().contentstackOptions.Authtoken = tokens.AccessToken;
                GetClient().contentstackOptions.IsOAuthToken = true;
                
                try
                {
                    // Make the API call to revoke authorization
                    await GetClient().InvokeAsync<Services.OAuth.OAuthAppRevocationService, ContentstackResponse>(service);
                }
                finally
                {
                    // Restore original client configuration
                    GetClient().contentstackOptions.Authtoken = originalAuthtoken;
                    GetClient().contentstackOptions.IsOAuthToken = originalIsOAuthToken;
                }
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is Exceptions.OAuthException))
            {
                throw new Exceptions.OAuthException($"Failed to revoke OAuth app authorization: {ex.Message}", ex);
            }
        }
        #endregion

        #region Future Method Placeholders
        // These methods will be implemented in subsequent phases:
        // - GetValidTokensAsync()
        #endregion
    }
}
