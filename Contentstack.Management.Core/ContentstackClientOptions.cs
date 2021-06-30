﻿using System;
using System.ComponentModel;
using System.Net;
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
        /// The Host used to set host url for the Contentstack Management API.
        /// </summary>
        public string Host { get; set; } = "api.contentstack.io";


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
        /// </summary>
        public TimeSpan Timeout { get; set; } = CSConstants.Timeout;

        /// <summary>
        /// When set to true, the client will retry requests.
        /// When set to false, the client will not retry request.
        /// The default value is true
        /// </summary>
        public bool RetryOnError { get; set; } = true;

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