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

        private string Version => "0.1.13";
        private string xUserAgent => $"contentstack-management-dotnet/{Version}";
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

            ExecutionContext context = new ExecutionContext(
                new RequestContext()
                {
                    config = contentstackOptions,
                    service = request
                },
                new ResponseContext());

            return (ContentstackResponse)ContentstackPipeline.InvokeSync(context, addAcceptMediaHeader, apiVersion).httpResponse;
        }

        internal Task<TResponse> InvokeAsync<TRequest, TResponse>(TRequest request, bool addAcceptMediaHeader = false, string apiVersion = null)
            where TRequest : IContentstackService
            where TResponse : ContentstackResponse
        {
            ThrowIfDisposed();

            ExecutionContext context = new ExecutionContext(
              new RequestContext()
              {
                  config = contentstackOptions,
                  service = request
              },
              new ResponseContext());
            return ContentstackPipeline.InvokeAsync<TResponse>(context, addAcceptMediaHeader, apiVersion);
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
    }
}
