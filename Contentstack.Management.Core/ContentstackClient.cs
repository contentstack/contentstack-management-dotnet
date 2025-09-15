using System;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Contentstack.Management.Core.Utils;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Attributes;
using Contentstack.Management.Core.Abstractions;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Runtime.Pipeline;
using Contentstack.Management.Core.Services.User;
using Contentstack.Management.Core.Queryable;
using Environment = System.Environment;
using System.Collections.Generic;
using Contentstack.Management.Core.Runtime.Pipeline.RetryHandler;

namespace Contentstack.Management.Core
{
    /// <summary>
    /// Contentstack Client for interacting with Contentstack Management API.
    /// </summary>
    public class ContentstackClient : IContentstackClient
    {
        internal ContentstackRuntimePipeline ContentstackPipeline { get; set; }
        internal ContentstackClientOptions contentstackOptions;
        internal JsonSerializer serializer => JsonSerializer.Create(SerializerSettings);

        #region Private
        private HttpClient _httpClient;
        private bool _disposed = false;

        private string Version => "0.4.0";
        private string xUserAgent => $"contentstack-management-dotnet/{Version}";
        
        // OAuth token storage
        private readonly Dictionary<string, OAuthTokens> _oauthTokens = new Dictionary<string, OAuthTokens>();
        
        private bool _isRefreshingToken = false;
        #endregion


        #region Public

        public LogManager LogManager { get; set; }
        /// <summary>
        /// Get and Set method for deserialization.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings();

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes new instance of the <see cref="ContentstackClient"/> class.
        /// </summary>
        /// <param name="contentstackOptions">The <see cref="ContentstackClientOptions"/> used for this client.</param>
        /// <example>
        /// <pre><code>
        /// var options = new ContentstackClientOptions()
        /// {
        ///       Host = "<API_HOST>",
        ///       Authtoken = "<AUTHTOKEN>"
        /// }
        /// ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
        /// </code></pre>
        /// </example>
        public ContentstackClient(IOptions<ContentstackClientOptions> contentstackOptions)
        {
            this.contentstackOptions = contentstackOptions.Value;
            Initialize();
            BuildPipeline();
        }


        /// <summary>
        /// Initializes new instance of the <see cref="ContentstackClient"/> class with custom HttpClient.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> user can pass.</param>
        /// <example>
        /// <pre><code>
        /// HttpClientHandler httpClientHandler = new HttpClientHandler
        /// {
        ///     Proxy = contentstackOptions.GetWebProxy()
        /// };

        /// _httpClient = new HttpClient(httpClientHandler);
        /// ContentstackClient client = new ContentstackClient(_httpClient);
        /// </code></pre>
        /// </example>
        public ContentstackClient(HttpClient httpClient, ContentstackClientOptions contentstackOptions) 
        {
            this.contentstackOptions = contentstackOptions;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Initialize(_httpClient);
            BuildPipeline();
        }

        public ContentstackClient(ContentstackClientOptions contentstackOptions) :
        this(new OptionsWrapper<ContentstackClientOptions>(contentstackOptions))
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="ContentstackClient"/> class.
        /// </summary>
        /// <param name="authtoken">The optional Authtoken for making CMA call</param>
        /// <param name="host">The optional host name for the API.</param>
        /// <param name="port">The optional port for the API</param>
        /// <param name="version">The optional version for the API</param>
        /// <param name="disableLogging">The optional to disable or enable logs.</param>
        /// <param name="maxResponseContentBufferSize">The optional maximum number of bytes to buffer when reading the response content</param>
        /// <param name="timeout">The optional timespan to wait before the request times out.</param>
        /// <param name="retryOnError">The optional retry condition for retrying on error.</param>
        /// <param name="proxyHost">Host to use with a proxy.</param>
        /// <param name="proxyPort">Port to use with a proxy.</param>
        /// <param name="proxyCredentials">Credentials to use with a proxy.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// </code></pre>
        /// </example>
        public ContentstackClient(
            string authtoken = null,
            string host = "api.contentstack.io",
            int port = 443,
            string version = "v3",
            bool disableLogging = false,
            long maxResponseContentBufferSize = CSConstants.ContentBufferSize,
            int timeout = 30,
            bool retryOnError = true,
            string proxyHost = null,
            int proxyPort = -1,
            ICredentials proxyCredentials = null
            ) :
        this(new OptionsWrapper<ContentstackClientOptions>(new ContentstackClientOptions()
        {
            Authtoken = authtoken,
            Host = host,
            Port = port,
            Version = version,
            DisableLogging = disableLogging,
            MaxResponseContentBufferSize = maxResponseContentBufferSize,
            Timeout = TimeSpan.FromSeconds(timeout),
            RetryOnError = retryOnError,
            ProxyHost = proxyHost,
            ProxyPort = proxyPort,
            ProxyCredentials = proxyCredentials
        }
        ))
        { }
        #endregion

        protected void Initialize(HttpClient httpClient = null)
        {
            if (httpClient != null)
            {
                _httpClient = httpClient;
            }
            else
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler
                {
                    Proxy = contentstackOptions.GetWebProxy()
                };

                _httpClient = new HttpClient(httpClientHandler);
            }

            _httpClient.DefaultRequestHeaders.Add(HeadersKey.XUserAgentHeader, $"{xUserAgent}");

            if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
            {
                _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("contentstack-management-dotnet", Version));
                _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DotNet", Environment.Version.ToString()));
            }

            if (contentstackOptions != null)
            {
                _httpClient.Timeout = contentstackOptions.Timeout;
                _httpClient.MaxResponseContentBufferSize = contentstackOptions.MaxResponseContentBufferSize;
                LogManager = contentstackOptions.DisableLogging ? LogManager.EmptyLogger : LogManager.GetLogManager(GetType());

                if (contentstackOptions.EarlyAccess != null) {
                    _httpClient.DefaultRequestHeaders.Add(HeadersKey.EarlyAccessHeader, string.Join(",", contentstackOptions.EarlyAccess));
                }
            }

            SerializerSettings.DateParseHandling = DateParseHandling.None;
            SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            foreach (Type t in CsmJsonConverterAttribute.GetCustomAttribute(typeof(CsmJsonConverterAttribute)))
            {
                SerializerSettings.Converters.Add((JsonConverter)Activator.CreateInstance(t));
            }
            SerializerSettings.Converters.Add(new NodeJsonConverter());
            SerializerSettings.Converters.Add(new TextNodeJsonConverter());
        }

        protected void BuildPipeline()
        {
            HttpHandler httpClientHandler = new HttpHandler(_httpClient);

            RetryPolicy retryPolicy = contentstackOptions.RetryPolicy ?? new DefaultRetryPolicy(contentstackOptions.RetryLimit, contentstackOptions.RetryDelay);
            ContentstackPipeline = new ContentstackRuntimePipeline(new List<IPipelineHandler>()
            {
                httpClientHandler,
                new RetryHandler(retryPolicy)
            }, LogManager);
        }

        internal ContentstackResponse InvokeSync<TRequest>(TRequest request, bool addAcceptMediaHeader = false, string apiVersion = null) where TRequest : IContentstackService
        {
            ThrowIfDisposed();

            // OAuth token validation is handled in the async method

            ExecutionContext context = new ExecutionContext(
                new RequestContext()
                {
                    config = contentstackOptions,
                    service = request
                },
                new ResponseContext());

            return (ContentstackResponse)ContentstackPipeline.InvokeSync(context, addAcceptMediaHeader, apiVersion).httpResponse;
        }

        internal async Task<TResponse> InvokeAsync<TRequest, TResponse>(TRequest request, bool addAcceptMediaHeader = false, string apiVersion = null)
            where TRequest : IContentstackService
            where TResponse : ContentstackResponse
        {
            ThrowIfDisposed();

            // Check and refresh OAuth tokens if needed before making API calls
            if (contentstackOptions.IsOAuthToken && !string.IsNullOrEmpty(contentstackOptions.Authtoken))
            {
                await EnsureOAuthTokenIsValidAsync();
            }

            ExecutionContext context = new ExecutionContext(
              new RequestContext()
              {
                  config = contentstackOptions,
                  service = request
              },
              new ResponseContext());
            return await ContentstackPipeline.InvokeAsync<TResponse>(context, addAcceptMediaHeader, apiVersion);
        }

        #region Dispose methods
        /// <summary>
        /// Wrapper for HttpClient Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _httpClient.Dispose();
            }
            ContentstackPipeline?.Dispose();

            _disposed = true;

        }

        private void ThrowIfDisposed()
        {
            //_httpClient.SendAsync
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
        #endregion

        /// <summary>
        /// <see cref="Models.User" /> session consists of calls that will help you to update user of your Contentstack account.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// User user = client.User();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Models.User" />.</returns>
        public User User()
        {
            return new User(this);
        }

        /// <summary>
        /// <see cref="Models.Organization" /> the top-level entity in the hierarchy of Contentstack, consisting of stacks and stack resources, and users.
        /// <see cref="Models.Organization" />  allows easy management of projects as well as users within the Organization.
        /// </summary>
        /// <param name="uid">Organization uid.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Organization organization = client.Organization("<ORG_UID>");
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Models.Organization" />.</returns>
        public Organization Organization(string uid = null)
        {
            return new Organization(this, uid);
        }

        /// <summary>
        /// <see cref="Models.Stack" /> is a space that stores the content of a project (a web or mobile property).
        /// Within a stack, you can create content structures, content entries, users, etc. related to the project. 
        /// </summary>
        /// <param name="apiKey">Stack API key</param>
        /// <param name="managementToken">Stack Management token </param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack Stack = client.Stack("<API_KEY>");
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Models.Stack" />.</returns>
        public Stack Stack(string apiKey = null, string managementToken = null, string branchUid = null)
        {
            return new Stack(this, apiKey, managementToken, branchUid);
        }

        #region LoginMethod
        /// <summary>
        /// The Log in to your account request is used to sign in to your Contentstack account and obtain the authtoken.
        /// </summary>
        /// <param name="credentials">User credentials for login.</param>
        /// <param name="token">The optional 2FA token.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// NetworkCredential credentials = new NetworkCredential("<EMAIL>", "<PASSWORD>");
        /// ContentstackResponse contentstackResponse = client.Login(credentials);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse" /></returns>
        public ContentstackResponse Login(ICredentials credentials, string token = null)
        {
            ThrowIfAlreadyLoggedIn();
            LoginService Login = new LoginService(serializer, credentials, token);

            return InvokeSync(Login);
        }

        /// <summary>
        /// The Log in to your account request is used to sign in to your Contentstack account and obtain the authtoken.
        /// </summary>
        /// <param name="credentials">User credentials for login.</param>
        /// <param name="token">The optional 2FA token.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// NetworkCredential credentials = new NetworkCredential("<EMAIL>", "<PASSWORD>");
        /// ContentstackResponse contentstackResponse = await client.LoginAsync(credentials);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> LoginAsync(ICredentials credentials, string token = null)
        {
            ThrowIfAlreadyLoggedIn();
            LoginService Login = new LoginService(serializer, credentials, token);

            return InvokeAsync<LoginService, ContentstackResponse>(Login);
        }
        #endregion

        #region Throw Error

        internal void ThrowIfAlreadyLoggedIn()
        {
            if (!string.IsNullOrEmpty(contentstackOptions.Authtoken))
            {
                throw new InvalidOperationException(CSConstants.YouAreLoggedIn);
            }
        }

        internal void ThrowIfNotLoggedIn()
        {
            if (string.IsNullOrEmpty(contentstackOptions.Authtoken))
            {
                throw new InvalidOperationException(CSConstants.YouAreNotLoggedIn);
            }
        }
        #endregion

        #region LogoutMethod
        /// <summary>
        /// The Log out of your account call is used to sign out the user of Contentstack account
        /// </summary>
        /// <param name="authtoken">The optional authroken in case user want to logout.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Logout();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse" /></returns>
        public ContentstackResponse Logout(string authtoken = null)
        {
            string token = authtoken ?? contentstackOptions.Authtoken;
            LogoutService logout = new LogoutService(serializer, token);

            return InvokeSync(logout);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authtoken">The optional authroken in case user want to logout.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.LogoutAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> LogoutAsync(string authtoken = null)
        {
            string token = authtoken ?? contentstackOptions.Authtoken;
            LogoutService logout = new LogoutService(serializer, token);

            return InvokeAsync<LogoutService, ContentstackResponse>(logout);
        }
        #endregion

        #region OAuth Methods
        /// <summary>
        /// Creates an OAuth handler for OAuth 2.0 authentication flow.
        /// This method allows you to use OAuth instead of traditional authtoken authentication.
        /// </summary>
        /// <param name="options">The OAuth configuration options.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient();
        /// var oauthOptions = new OAuthOptions
        /// {
        ///     AppId = "your-app-id",
        ///     ClientId = "your-client-id",
        ///     RedirectUri = "http://localhost:8184"
        /// };
        /// OAuthHandler oauthHandler = client.OAuth(oauthOptions);
        /// 
        /// // Get authorization URL
        /// string authUrl = oauthHandler.GetAuthorizationUrl();
        /// 
        /// // After user authorization, exchange code for tokens
        /// var tokens = await oauthHandler.ExchangeCodeForTokenAsync("authorization_code");
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="OAuthHandler" /> for managing OAuth flow.</returns>
        public OAuthHandler OAuth(OAuthOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options), "OAuth options cannot be null.");

            return new OAuthHandler(this, options);
        }

        /// <summary>
        /// Creates an OAuth handler with default OAuth options.
        /// Uses the default AppId, ClientId, and RedirectUri.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient();
        /// OAuthHandler oauthHandler = client.OAuth();
        /// 
        /// // Get authorization URL with default options
        /// string authUrl = oauthHandler.GetAuthorizationUrl();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="OAuthHandler" /> with default OAuth options.</returns>
        public OAuthHandler OAuth()
        {
            var defaultOptions = new OAuthOptions();
            return new OAuthHandler(this, defaultOptions);
        }

        /// <summary>
        /// Sets OAuth tokens for the client to use for authenticated requests.
        /// This method is called internally by the OAuthHandler after successful token exchange or refresh.
        /// </summary>
        /// <param name="tokens">The OAuth tokens to use for authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when tokens is null.</exception>
        internal void SetOAuthTokens(OAuthTokens tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens), "OAuth tokens cannot be null.");

            if (string.IsNullOrEmpty(tokens.AccessToken))
                throw new ArgumentException("Access token cannot be null or empty.", nameof(tokens));

            // Store the access token in the client options for use in HTTP requests
            // This will be used by the HTTP pipeline to inject the Bearer token
            // Note: We need both IsOAuthToken=true AND Authtoken=AccessToken because
            // the HTTP pipeline only has access to ContentstackClientOptions, not the full client
            contentstackOptions.Authtoken = tokens.AccessToken;
            contentstackOptions.IsOAuthToken = true;
        }

        /// <summary>
        /// Gets the current OAuth tokens for the specified client ID.
        /// This method allows other SDKs (like contentstack-model-generator) to access OAuth tokens.
        /// </summary>
        /// <param name="clientId">The OAuth client ID to get tokens for.</param>
        /// <returns>The OAuth tokens if available, null otherwise.</returns>
        public OAuthTokens GetOAuthTokens(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));

            return GetStoredOAuthTokens(clientId);
        }

        /// <summary>
        /// Checks if OAuth tokens are available for the specified client ID (regardless of validity).
        /// </summary>
        /// <param name="clientId">The OAuth client ID to check tokens for.</param>
        /// <returns>True if tokens are available, false otherwise.</returns>
        public bool HasOAuthTokens(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                return false;

            return HasStoredOAuthTokens(clientId);
        }

        /// <summary>
        /// Checks if valid OAuth tokens are available for the specified client ID.
        /// </summary>
        /// <param name="clientId">The OAuth client ID to check tokens for.</param>
        /// <returns>True if valid tokens are available, false otherwise.</returns>
        public bool HasValidOAuthTokens(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                return false;

            var tokens = GetStoredOAuthTokens(clientId);
            return tokens?.IsValid == true;
        }

        /// <summary>
        /// Clears OAuth tokens and resets the client to use traditional authentication.
        /// This method should be called when logging out or switching authentication methods.
        /// </summary>
        /// <param name="clientId">The OAuth client ID to clear tokens for.</param>
        public void ClearOAuthTokens(string clientId = null)
        {
            if (!string.IsNullOrEmpty(clientId))
            {
                ClearStoredOAuthTokens(clientId);
            }
            else
            {
                _oauthTokens.Clear();
            }

            // Reset OAuth flag and clear authtoken if it was an OAuth token
            if (contentstackOptions.IsOAuthToken)
            {
                contentstackOptions.IsOAuthToken = false;
                contentstackOptions.Authtoken = null;
            }
        }
        #endregion

        #region Internal OAuth Token Management
        /// <summary>
        /// Stores OAuth tokens for the specified client ID.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <param name="tokens">The OAuth tokens to store.</param>
        internal void StoreOAuthTokens(string clientId, OAuthTokens tokens)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));

            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            _oauthTokens[clientId] = tokens;
        }

        /// <summary>
        /// Gets OAuth tokens for the specified client ID.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <returns>The OAuth tokens if found, null otherwise.</returns>
        internal OAuthTokens GetStoredOAuthTokens(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                return null;

            return _oauthTokens.TryGetValue(clientId, out var tokens) ? tokens : null;
        }

        /// <summary>
        /// Checks if OAuth tokens exist for the specified client ID.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <returns>True if tokens exist, false otherwise.</returns>
        internal bool HasStoredOAuthTokens(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                return false;

            return _oauthTokens.ContainsKey(clientId);
        }

        /// <summary>
        /// Removes OAuth tokens for the specified client ID.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        internal void ClearStoredOAuthTokens(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                return;

            _oauthTokens.Remove(clientId);
        }

        /// <summary>
        /// Clears all OAuth tokens (useful for cleanup).
        /// </summary>
        internal void ClearAllOAuthTokens()
        {
            _oauthTokens.Clear();
        }
        #endregion

        /// <summary>
        /// The Get user call returns comprehensive information of an existing user account.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.GetUser();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse GetUser(ParameterCollection collection = null)
        {
            ThrowIfNotLoggedIn();

            GetLoggedInUserService getUser = new GetLoggedInUserService(serializer, collection);

            return InvokeSync(getUser);
        }

        /// <summary>
        /// The Get user call returns comprehensive information of an existing user account.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.GetUserAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> GetUserAsync(ParameterCollection collection = null)
        {
            ThrowIfNotLoggedIn();

            GetLoggedInUserService getUser = new GetLoggedInUserService(serializer, collection);

            return InvokeAsync<GetLoggedInUserService, ContentstackResponse>(getUser);
        }

        /// <summary>
        /// Ensures that the current OAuth token is valid and refreshes it if needed.
        /// This method is called before each API request to automatically handle token refresh.
        /// </summary>
        private async Task EnsureOAuthTokenIsValidAsync()
        {
            // Prevent recursive calls
            if (_isRefreshingToken)
            {
                return;
            }

            try
            {
                // Find the OAuth tokens that match the current access token
                foreach (var kvp in _oauthTokens)
                {
                    var clientId = kvp.Key;
                    var tokens = kvp.Value;

                    if (tokens?.AccessToken == contentstackOptions.Authtoken && tokens.NeedsRefresh)
                    {
                        // Set flag to prevent recursive calls
                        _isRefreshingToken = true;

                        try
                        {
                            // Get the OAuth handler for this client
                            var oauthHandler = OAuth(new Models.OAuthOptions
                            {
                                ClientId = clientId,
                                AppId = tokens.AppId
                            });

                            // Refresh the tokens
                            var refreshedTokens = await oauthHandler.RefreshTokenAsync(tokens.RefreshToken);
                            
                            if (refreshedTokens != null)
                            {
                                // Update the stored tokens
                                StoreOAuthTokens(clientId, refreshedTokens);
                                
                                // Update the client's current authentication
                                contentstackOptions.Authtoken = refreshedTokens.AccessToken;
                                contentstackOptions.IsOAuthToken = true; // Ensure OAuth flag is maintained
                            }
                        }
                        catch (Exception ex)
                        {
                            // Wrap any exception in OAuth exception with context
                            throw new Exceptions.OAuthException(
                                $"OAuth token refresh failed for client '{clientId}': {ex.Message}", ex);
                        }
                        finally
                        {
                            _isRefreshingToken = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Wrap any exception in OAuth exception with context
                throw new Exceptions.OAuthException(
                    $"OAuth token validation failed: {ex.Message}", ex);
            }
        }
    }
}

