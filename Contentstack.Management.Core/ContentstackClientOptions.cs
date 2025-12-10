using System;
using System.ComponentModel;
using System.Net;
using Contentstack.Management.Core.Runtime.Pipeline.RetryHandler;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core
{
    /// <summary>
    /// ContentstackConfig class is base class for Contentstack Configuration
    /// </summary>
    public class ContentstackClientOptions
    {
        #region public
        /// <summary>
        /// An Authtoken is a read-write token used to make authorized CMA requests.
        /// </summary>
        public string Authtoken { get; set; }

        /// <summary>
        /// Indicates whether the current authtoken is an OAuth Bearer token.
        /// When true, the authtoken will be sent as "Authorization: Bearer {token}" header.
        /// When false, the authtoken will be sent as "authtoken: {token}" header.
        /// </summary>
        public bool IsOAuthToken { get; set; } = false;

        /// <summary>
        /// The Host used to set host url for the Contentstack Management API.
        /// </summary>
        public string Host { get; set; } = "api.contentstack.io";

        /// <summary>
        /// The EarlyAccess used to set early access headers for the Contentstack Management API.
        /// </summary>
        public string[] EarlyAccess { get; set; }

        /// <summary>
        /// The Host used to set host url for the Contentstack Management API.
        /// </summary>
        public int Port { get; set; } = 443;

        /// <summary>
        /// The Host used to set host url for the Contentstack Management API.
        /// </summary>
        public string Version { get; set; } = "v3";

        /// <summary>
        /// Gets or sets the DisableLogging. When set to true, the logging of the client is disabled.
        /// The default value is false.
        /// </summary>
        public bool DisableLogging { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of bytes to buffer when reading the response content.
        /// The default value for this property is 1 gigabytes.
        /// </summary>
        public long MaxResponseContentBufferSize { get; set; } = CSConstants.ContentBufferSize;

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// The default value for time out is 30 seconds.
        /// </summary>
        public TimeSpan Timeout { get; set; } = CSConstants.Timeout;

        /// <summary>
        /// When set to true, the client will retry requests.
        /// When set to false, the client will not retry request.
        /// The default value is true
        /// </summary>
        public bool RetryOnError { get; set; } = true;

        /// <summary>
        /// Returns the flag indicating how many retry HTTP requests an SDK should
        /// make for a single SDK operation invocation before giving up.
        /// The default value is 5.
        /// </summary>
        public int RetryLimit { get; set; } = 5;

        /// <summary>
        /// Returns the flag indicating delay in retrying HTTP requests.
        /// The default value is 300ms.
        /// </summary>
        public TimeSpan RetryDelay { get; set; } = CSConstants.Delay;

        /// <summary>
        /// The retry policy which specifies when 
        /// a retry should be performed.
        /// </summary>
        public RetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// When set to true, the client will retry on network failures.
        /// The default value is true.
        /// </summary>
        public bool RetryOnNetworkFailure { get; set; } = true;

        /// <summary>
        /// When set to true, the client will retry on DNS failures.
        /// The default value is true.
        /// </summary>
        public bool RetryOnDnsFailure { get; set; } = true;

        /// <summary>
        /// When set to true, the client will retry on socket failures.
        /// The default value is true.
        /// </summary>
        public bool RetryOnSocketFailure { get; set; } = true;

        /// <summary>
        /// When set to true, the client will retry on HTTP server errors (5xx).
        /// The default value is true.
        /// </summary>
        public bool RetryOnHttpServerError { get; set; } = true;

        /// <summary>
        /// Maximum number of network retry attempts.
        /// The default value is 3.
        /// </summary>
        public int MaxNetworkRetries { get; set; } = 3;

        /// <summary>
        /// Base delay for network retries.
        /// The default value is 100ms.
        /// </summary>
        public TimeSpan NetworkRetryDelay { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Backoff strategy for network retries.
        /// The default value is Exponential.
        /// </summary>
        public BackoffStrategy NetworkBackoffStrategy { get; set; } = BackoffStrategy.Exponential;

        /// <summary>
        /// Custom function to determine if a status code should be retried.
        /// If null, default retry condition is used (429, 500, 502, 503, 504).
        /// </summary>
        public Func<HttpStatusCode, bool>? RetryCondition { get; set; }

        /// <summary>
        /// Options for retry delay calculation.
        /// </summary>
        public RetryDelayOptions RetryDelayOptions { get; set; }

        /// <summary>
        /// Host for the Proxy.
        /// </summary>
        public string ProxyHost { get; set; }

        /// <summary>
        /// Port for the Proxy.
        /// </summary>
        public int ProxyPort { get; set; } = -1;

        /// <summary>
        /// Credentials to use with a proxy.
        /// </summary>
        public ICredentials ProxyCredentials { get; set; }

        /// <summary>
        /// Returns a WebProxy instance configured to match the proxy settings
        /// in the configuration.
        /// </summary>
        /// <returns></returns>
        public IWebProxy GetWebProxy()
        {
            const string httpPrefix = "http://";

            WebProxy webProxy = null;
            if (!string.IsNullOrEmpty(ProxyHost) && ProxyPort != -1)
            {
                var host = ProxyHost.StartsWith(httpPrefix, StringComparison.OrdinalIgnoreCase)
                               ? ProxyHost.Substring(httpPrefix.Length)
                               : ProxyHost;
                webProxy = new WebProxy(host, ProxyPort);

                if (ProxyCredentials != null)
                {
                    webProxy.Credentials = ProxyCredentials;
                }
            }

            return webProxy;
        }

        public Uri GetUri ()
        {
            UriBuilder uriBuilder = new UriBuilder(this.Host);
            uriBuilder.Port = this.Port;
            uriBuilder.Path = this.Version;
            uriBuilder.Scheme = "https";
            return uriBuilder.Uri;
        }
        #endregion
    }
}