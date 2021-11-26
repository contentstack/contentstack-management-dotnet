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
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Services.User;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core
{
    /// <summary>
    /// Contentstack Client for interacting with Contentstack Management API.
    /// </summary>
    public class ContentstackClient: IContentstackClient
    {
        internal ContentstackRuntimePipeline ContentstackPipeline { get; set; }
        internal ContentstackClientOptions contentstackOptions;
        internal JsonSerializer serializer => JsonSerializer.Create(SerializerSettings);

        #region Private
        private HttpClient _httpClient;
        private bool _disposed = false;

        private string Version => "0.1.0";
        private string xUserAgent { get => $"contentstack-management-dotnet/{Version}"; }
        #endregion


        #region Public

        public LogManager LogManager;
        /// <summary>
        /// Get and Set method for deserialization.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings();

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes new instance of the <see cref="contentstackOptions"/> class.
        /// </summary>
        /// <param name="contentstackOptions">The <see cref="ContentstackClientOptions"/> used for this client.</param>
        public ContentstackClient(IOptions<ContentstackClientOptions> contentstackOptions)
        {
            this.contentstackOptions = contentstackOptions.Value;
            Initialize();
            BuildPipeline();
        }

        public ContentstackClient(ContentstackClientOptions contentstackOptions) :
        this(new OptionsWrapper<ContentstackClientOptions>(contentstackOptions)){}

        /// <summary>
        /// Initializes new instance of the <see cref="contentstackOptions"/> class.
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
        public ContentstackClient(
            string authtoken=null,
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
        )){}
        #endregion

        protected virtual void Initialize()
        {
            var httpClientHandler = new HttpClientHandler();

            httpClientHandler.Proxy = contentstackOptions.GetWebProxy();

            _httpClient = new HttpClient(httpClientHandler);

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
            }

            if (contentstackOptions.DisableLogging)
            {
                LogManager = LogManager.EmptyLogger;
            }
            else
            {
                LogManager = LogManager.GetLogManager(this.GetType());
            }

            this.SerializerSettings.DateParseHandling = DateParseHandling.None;
            this.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            this.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            this.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            foreach (Type t in CSMJsonConverterAttribute.GetCustomAttribute(typeof(CSMJsonConverterAttribute)))
            {
                SerializerSettings.Converters.Add((JsonConverter)Activator.CreateInstance(t));
            }
        }

        protected void BuildPipeline()
        {
            HttpHandler httpClientHandler = new HttpHandler(_httpClient);

            this.ContentstackPipeline = new ContentstackRuntimePipeline(httpClientHandler, LogManager);
        }

        internal ContentstackResponse InvokeSync<TRequest>(TRequest request) where TRequest : IContentstackService
        {
            ThrowIfDisposed();

            var context = new ExecutionContext(
                new RequestContext()
                {
                    config = this.contentstackOptions,
                    service = request
                },
                new ResponseContext());

            return (ContentstackResponse)this.ContentstackPipeline.InvokeSync(context).httpResponse;
        }

        internal Task<TResponse> InvokeAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IContentstackService
            where TResponse : ContentstackResponse
        {
            ThrowIfDisposed();

            var context = new ExecutionContext(
              new RequestContext()
              {
                  config = this.contentstackOptions,
                  service = request
              },
              new ResponseContext());
            return this.ContentstackPipeline.InvokeAsync<TResponse>(context);
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
                return;

            if (disposing)
            {
                _httpClient.Dispose();
            }
            if (ContentstackPipeline != null)
                ContentstackPipeline.Dispose();

            _disposed = true;

        }

        private void ThrowIfDisposed()
        {
            //_httpClient.SendAsync
            if (this._disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
        #endregion

        /// <summary>
        /// <see cref="Models.User" /> session consists of calls that will help you to update user of your Contentstack account.
        /// </summary>
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
        /// <returns>The <see cref="Models.User" />.</returns>
        public Organization Organization(string uid = null)
        {
            return new Organization(this, uid);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey">Stack API key</param>
        /// <param name="managementToken">Stack Management token </param>
        /// <returns></returns>
        public Stack Stack(string apiKey = null, string managementToken = null)
        {
            return new Stack(this, apiKey, managementToken);
        }

        #region LoginMethod
        /// <summary>
        /// The Log in to your account request is used to sign in to your Contentstack account and obtain the authtoken.
        /// </summary>
        /// <param name="credentials">User credentials for login.</param>
        /// <param name="token">The optional 2FA token.</param>
        /// <returns>The <see cref="ContentstackResponse" /></returns>
        public ContentstackResponse Login(ICredentials credentials, string token = null)
        {
            ThrowIfAlreadyLoggedIn();
            var Login = new LoginService(this.serializer, credentials, token);

            return this.InvokeSync(Login);
        }

        /// <summary>
        /// The Log in to your account request is used to sign in to your Contentstack account and obtain the authtoken.
        /// </summary>
        /// <param name="credentials">User credentials for login.</param>
        /// <param name="token">The optional 2FA token.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> LoginAsync(ICredentials credentials, string token = null)
        {
            ThrowIfAlreadyLoggedIn();
            var Login = new LoginService(this.serializer, credentials, token);

            return this.InvokeAsync<LoginService, ContentstackResponse>(Login);
        }
        #endregion

        #region Throw Error

        internal void ThrowIfAlreadyLoggedIn()
        {
            if (!string.IsNullOrEmpty(this.contentstackOptions.Authtoken))
            {
                throw new InvalidOperationException(CSConstants.YouAreLoggedIn);
            }
        }

        internal void ThrowIfNotLoggedIn()
        {
            if (string.IsNullOrEmpty(this.contentstackOptions.Authtoken))
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
        /// <returns>The <see cref="ContentstackResponse" /></returns>
        public ContentstackResponse Logout(string authtoken = null)
        {
            var token = authtoken ?? this.contentstackOptions.Authtoken;
            var logout = new LogoutService(this.serializer, token);

            return this.InvokeSync(logout);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authtoken">The optional authroken in case user want to logout.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> LogoutAsync(string authtoken = null)
        {
            var token = authtoken ?? this.contentstackOptions.Authtoken;
            var logout = new LogoutService(this.serializer, token);

            return this.InvokeAsync<LogoutService, ContentstackResponse>(logout);
        }
        #endregion

        /// <summary>
        /// The Get user call returns comprehensive information of an existing user account.
        /// </summary>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse GetUser(ParameterCollection collection = null)
        {
            ThrowIfNotLoggedIn();

            var getUser = new GetLoggedInUserService(this.serializer, collection);

            return this.InvokeSync(getUser);
        }

        /// <summary>
        /// The Get user call returns comprehensive information of an existing user account.
        /// </summary>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> GetUserAsync(ParameterCollection collection = null)
        {
            ThrowIfNotLoggedIn();

            var getUser = new GetLoggedInUserService(this.serializer, collection);

            return this.InvokeAsync<GetLoggedInUserService, ContentstackResponse>(getUser);
        }
    }
}
