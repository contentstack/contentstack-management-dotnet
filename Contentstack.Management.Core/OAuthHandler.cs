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

        private string codeVerifier = "";

        private string codeChallenge = "";
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
        public string ClientId => _clientId;

        public string AppId => _options.AppId;
        public string RedirectUri => _options.RedirectUri;
        public bool UsePkce => _options.UsePkce;
        public string[] Scope => _options.Scope;
        #endregion

        #region Helper Methods
        private OAuthTokens ValidateAndGetTokens()
        {
            var tokens = GetCurrentTokens();
            if (tokens == null)
            {
                throw new Exceptions.OAuthException("No OAuth tokens found. Please authenticate first.");
            }
            return tokens;
        }

        private void SetClientOAuthTokens(OAuthTokens tokens)
        {
            GetClient().contentstackOptions.Authtoken = tokens.AccessToken;
            GetClient().contentstackOptions.IsOAuthToken = true;
        }

        private void UpdateTokenProperty<T>(Action<OAuthTokens, T> setter, T value)
        {
            var tokens = GetCurrentTokens();
            if (tokens == null)
            {
                tokens = new OAuthTokens { ClientId = _clientId };
            }
            setter(tokens, value);
            StoreTokens(tokens);
        }

        private string LogoutInternal()
        {
            var currentTokens = ValidateAndGetTokens();

            // Try to revoke the OAuth app authorization (optional - if it fails, we still clear tokens)
            // Only attempt revocation if we have valid tokens
            if (!string.IsNullOrEmpty(currentTokens.AccessToken))
            {
                try
                {
                    var authorizationId = GetOauthAppAuthorizationAsync().GetAwaiter().GetResult();
                    RevokeOauthAppAuthorizationAsync(authorizationId).GetAwaiter().GetResult();
                }
                catch
                {
                    // If revocation fails, continue with logout
                    // This is common in OAuth implementations where revocation is optional
                }
            }

            // Clear tokens from memory store
            ClearTokens();

            // Return success message
            return "Logged out successfully";
        }

        private Exceptions.OAuthException HandleOAuthException(Exception ex, string operation)
        {
            if (ex is Exceptions.OAuthException)
                return (Exceptions.OAuthException)ex;
            
            return new Exceptions.OAuthException($"Failed to {operation}: {ex.Message}", ex);
        }
        #endregion

        #region Public Methods
        public OAuthTokens GetCurrentTokens()
        {
            return _client.GetStoredOAuthTokens(_clientId);
        }

        public bool HasValidTokens()
        {
            var tokens = _client.GetStoredOAuthTokens(_clientId);
            return tokens?.IsValid == true;
        }

        public bool HasTokens()
        {
            return _client.HasStoredOAuthTokens(_clientId);
        }

        public void ClearTokens()
        {
            _client.ClearStoredOAuthTokens(_clientId);
        }

        private void StoreTokens(OAuthTokens tokens)
        {
            _client.StoreOAuthTokens(_clientId, tokens);
        }

        public override string ToString()
        {
            return $"OAuthHandler: ClientId={_clientId}, AppId={_options.AppId}, UsePkce={_options.UsePkce}, HasTokens={HasTokens()}";
        }

        #region Token Getter Methods
        public string GetAccessToken()
        {
            var tokens = GetCurrentTokens();
            return tokens?.AccessToken;
        }

        public string GetRefreshToken()
        {
            var tokens = GetCurrentTokens();
            return tokens?.RefreshToken;
        }

        public string GetOrganizationUID()
        {
            var tokens = GetCurrentTokens();
            return tokens?.OrganizationUid;
        }

        public string GetUserUID()
        {
            var tokens = GetCurrentTokens();
            return tokens?.UserUid;
        }

        public DateTime? GetTokenExpiryTime()
        {
            var tokens = GetCurrentTokens();
            return tokens?.ExpiresAt;
        }
        #endregion

        #region Token Setter Methods
        public void SetAccessToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Access token is required.", nameof(token));

            UpdateTokenProperty((t, v) => t.AccessToken = v, token);
        }

        public void SetRefreshToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Refresh token is required.", nameof(token));

            UpdateTokenProperty((t, v) => t.RefreshToken = v, token);
        }

        public void SetOrganizationUID(string organizationUID)
        {
            if (string.IsNullOrEmpty(organizationUID))
                throw new ArgumentException("Organization UID is required.", nameof(organizationUID));

            UpdateTokenProperty((t, v) => t.OrganizationUid = v, organizationUID);
        }

        public void SetUserUID(string userUID)
        {
            if (string.IsNullOrEmpty(userUID))
                throw new ArgumentException("User UID is required.", nameof(userUID));

            UpdateTokenProperty((t, v) => t.UserUid = v, userUID);
        }

        public void SetTokenExpiryTime(DateTime expiryTime)
        {
            if (expiryTime == default(DateTime))
                throw new ArgumentException("Token expiry time is required.", nameof(expiryTime));

            UpdateTokenProperty((t, v) => t.ExpiresAt = v, expiryTime);
        }
        #endregion

        #endregion

        #region Protected Methods
        protected ContentstackClient GetClient()
        {
            return _client;
        }

        protected OAuthOptions GetOptions()
        {
            return _options;
        }
        #endregion

        #region OAuth Flow Methods
        /// <summary>
        /// Generates the OAuth authorization URL for user authentication.
        /// </summary>
        public async Task<string> AuthorizeAsync()
        {
            // AppId validation is now handled by OAuthOptions.Validate() in constructor

            try
            {
                // Build the base authorization URL using the correct OAuth hostname
                // Transform api.contentstack.io -> app.contentstack.com for OAuth authorization
                var oauthHost = GetOAuthHost(GetClient().contentstackOptions.Host);
                var baseUrl = $"https://{oauthHost}/#!/apps/{_options.AppId}/authorize";
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
                    this.codeVerifier  = PkceHelper.GenerateCodeVerifier();
                    this.codeChallenge  = PkceHelper.GenerateCodeChallenge(codeVerifier);

                    // Add PKCE parameters
                    queryParams.Add($"code_challenge={Uri.EscapeDataString(codeChallenge)}");
                    queryParams.Add("code_challenge_method=S256");
                }
                // Traditional OAuth flow - no additional parameters needed

                // Build the complete URL
                authUrl.Query = string.Join("&", queryParams);
                return await Task.FromResult(authUrl.ToString());
            }
            catch (Exception ex) when (!(ex is Exceptions.OAuthException))
            {
                throw new Exceptions.OAuthAuthorizationException($"Failed to generate OAuth authorization URL: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exchanges an authorization code for OAuth access and refresh tokens.
        /// </summary>
        public async Task<OAuthTokens> ExchangeCodeForTokenAsync(string authorizationCode)
        {
            if (string.IsNullOrEmpty(authorizationCode))
            {
                throw new ArgumentException("Authorization code is required.", nameof(authorizationCode));
            }

            try
            {
                // Create the OAuth token service for authorization code exchange
                OAuthTokenService tokenService;

                if (_options.UsePkce && !string.IsNullOrEmpty(this.codeVerifier) )
                {
                    tokenService = OAuthTokenService.CreateForAuthorizationCode(
                        serializer: GetClient().serializer,
                        authorizationCode: authorizationCode,
                        clientId: _options.ClientId,
                        redirectUri: _options.RedirectUri,
                        codeVerifier: this.codeVerifier
                    );
                }
                else
                {
                    if (string.IsNullOrEmpty(_options.ClientSecret))
                    {
                        throw new Exceptions.OAuthConfigurationException(
                            "Client secret is required for traditional OAuth flow. Set ClientSecret in OAuth options or use PKCE flow.");
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
                    ClientId = _clientId,
                    AppId = _options.AppId
                };

                // Store tokens in memory for future use
                StoreTokens(tokens);

                // Set OAuth tokens in the client for authenticated requests
                SetClientOAuthTokens(tokens);

                return tokens;
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is Exceptions.OAuthException))
            {
                throw HandleOAuthException(ex, "exchange authorization code for tokens");
            }
        }

        /// <summary>
        /// Refreshes the OAuth access token using the refresh token.
        /// </summary>
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
                        "No refresh token available. Please authenticate first.");
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
                            "Client secret is required for traditional OAuth flow.");
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
                    ClientId = _clientId,
                    AppId = _options.AppId
                };

                // Store the new tokens in memory
                StoreTokens(newTokens);

                // Set OAuth tokens in the client for authenticated requests
                SetClientOAuthTokens(newTokens);

                return newTokens;
            }
            catch (Exception ex) when (!(ex is Exceptions.OAuthException))
            {
                throw HandleOAuthException(ex, "refresh OAuth tokens");
            }
        }

        /// <summary>
        /// Logs out the user by clearing OAuth tokens.
        /// </summary>
        public async Task<string> LogoutAsync()
        {
            try
            {
                var currentTokens = ValidateAndGetTokens();

                // Try to revoke the OAuth app authorization (optional - if it fails, we still clear tokens)
                // Only attempt revocation if we have valid tokens
                if (!string.IsNullOrEmpty(currentTokens.AccessToken))
                {
                    try
                    {
                        var authorizationId = await GetOauthAppAuthorizationAsync();
                        await RevokeOauthAppAuthorizationAsync(authorizationId);
                    }
                    catch
                    {
                        // If revocation fails, continue with logout
                        // This is common in OAuth implementations where revocation is optional
                    }
                }

                // Clear tokens from memory store
                ClearTokens();

                // Return success message
                return "Logged out successfully";
            }
            catch (Exception ex) when (!(ex is Exceptions.OAuthException))
            {
                throw new InvalidOperationException($"Failed to logout: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Logs out the user synchronously by clearing OAuth tokens.
        /// </summary>
        public string Logout()
        {
            try
            {
                return LogoutInternal();
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                throw new InvalidOperationException($"Failed to logout: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Handles the redirect URL after OAuth authorization and exchanges the authorization code for tokens.
        /// </summary>
        public async Task HandleRedirectAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Redirect URL is required.", nameof(url));

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
                throw HandleOAuthException(ex, "handle redirect URL");
            }
        }
        #endregion

        #region Private Methods
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

        private async Task<string> GetOauthAppAuthorizationAsync()
        {
            var tokens = ValidateAndGetTokens();

            try
            {
                // Create a service to get OAuth app authorizations
                var service = new OAuthAppAuthorizationService(
                    GetClient().serializer,
                    _options.AppId,
                    tokens.OrganizationUid
                );

                SetClientOAuthTokens(tokens);
                
                try
                {
                    // Make the API call to get authorizations
                    var response = await GetClient().InvokeAsync<OAuthAppAuthorizationService, ContentstackResponse>(service);
                    
                    var authResponse = response.OpenTResponse<OAuthAppAuthorizationResponse>();

                    if (authResponse?.Data?.Length > 0)
                    {
                        var userUid = tokens.UserUid;
                        var currentUserAuthorization = authResponse.Data.FirstOrDefault(auth => auth.User?.Uid == userUid);
                        
                        if (currentUserAuthorization == null)
                        {
                            throw new Exceptions.OAuthException("No authorizations found for current user.");
                        }
                        
                        return currentUserAuthorization.AuthorizationUid;
                    }
                    else
                    {
                        throw new Exceptions.OAuthException("No authorizations found for the app.");
                    }
                }
                catch (Exception ex) when (!(ex is Exceptions.OAuthException))
                {
                    throw HandleOAuthException(ex, "get OAuth app authorization");
                }
            }
            catch (Exception ex) when (!(ex is Exceptions.OAuthException))
            {
                throw HandleOAuthException(ex, "get OAuth app authorization");
            }
        }

        private async Task RevokeOauthAppAuthorizationAsync(string authorizationId)
        {
            if (string.IsNullOrEmpty(authorizationId))
            {
                throw new ArgumentException("Authorization ID is required.", nameof(authorizationId));
            }

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
                
                SetClientOAuthTokens(tokens);
                
                try
                {
                    // Make the API call to revoke authorization
                    var response = await GetClient().InvokeAsync<OAuthAppRevocationService, ContentstackResponse>(service);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    // Clear OAuth tokens after successful revocation (for logout scenario)
                    GetClient().contentstackOptions.Authtoken = null;
                    GetClient().contentstackOptions.IsOAuthToken = false;
                }
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is Exceptions.OAuthException))
            {
                throw HandleOAuthException(ex, "revoke OAuth app authorization");
            }
        }

        #endregion
        
    }
}
