using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using Contentstack.Management.Core.Http;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;
using Contentstack.Management.Core.Queryable;
using System.Net.Http.Headers;

namespace Contentstack.Management.Core.Services
{
    internal class ContentstackService : IContentstackService
    {

        #region
        internal readonly ParameterCollection collection;
        readonly IDictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly IDictionary<string, string> pathResources = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly IDictionary<string, string> queryResources = new Dictionary<string, string>(StringComparer.Ordinal);

        private bool _useQueryString = false;

        private bool _disposed = false;
        private JsonSerializer _serializer { get; set; }

        #endregion

        #region Constructor
        internal ContentstackService(JsonSerializer serializer, Core.Models.Stack stack = null, ParameterCollection collection = null)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            if (stack != null)
            {
                if (!string.IsNullOrEmpty(stack.APIKey))
                {
                    this.Headers.Add("api_key", stack.APIKey);
                }

                if (!string.IsNullOrEmpty(stack.BranchUid))
                {
                    Headers.Add("branch", stack.BranchUid);
                }

                this.ManagementToken = stack.ManagementToken;
            }else
            {
                this.ManagementToken = null;
            }
            
            this.collection = collection ?? new ParameterCollection();
            _serializer = serializer;
        }
        #endregion

        public JsonSerializer Serializer
        {
            get
            {
                return _serializer;
            }
        }

        public bool UseQueryString
        {
            get
            {
                if (HttpMethod == "GET")
                    return true;
                return _useQueryString;
            }
            set
            {
                _useQueryString = value;
            }
        }

        public ParameterCollection Parameters
        {
            get
            {
                return collection;
            }
        }

        public IDictionary<string, string> Headers
        {
            get
            {
                return headers;
            }
        }

        public IDictionary<string, string> QueryResources
        {
            get
            {
                return queryResources;
            }
        }

        public IDictionary<string, string> PathResources
        {
            get
            {
                return pathResources;
            }
        }

        public string ResourcePath { get; set; }
        public byte[] ByteContent { get; set; }
        public HttpContent Content { get; set; }
        public string HttpMethod { get; set; } = "GET";
        public string ManagementToken { get; set; }

        public void AddQueryResource(string queryResource, string value)
        {
            ThrowIfDisposed();
            QueryResources.Add(queryResource, value);
        }

        public void AddPathResource(string key, string value)
        {
            ThrowIfDisposed();

            PathResources.Add(key, value);
        }

        public string GetHeaderValue(string headerName)
        {
            ThrowIfDisposed();

            string headerValue;
            if (headers.TryGetValue(headerName, out headerValue))
                return headerValue;

            return string.Empty;
        }

        /// <summary>
        /// Returns true if the request has a body, else false.
        /// </summary>
        /// <returns>Returns true if the request has a body, else false.</returns>
        public bool HasRequestBody()
        {
            return HttpMethod == "POST" || HttpMethod == "PUT" || HttpMethod == "PATCH" || HttpMethod == "DELETE";
        }

        public virtual IHttpRequest CreateHttpRequest(HttpClient httpClient, ContentstackClientOptions config, bool addAcceptMediaHeader = false, string apiVersion = null)
        {
            ThrowIfDisposed();

            if (addAcceptMediaHeader)
            {
                httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("image/jpeg"));
            }
            Uri requestUri = ContentstackUtilities.ComposeUrI(config.GetUri(), this);
            Headers["Content-Type"] = "application/json";

            if (!string.IsNullOrEmpty(this.ManagementToken))
            {
                Headers["authorization"] = this.ManagementToken;
            }
            else if (!string.IsNullOrEmpty(config.Authtoken))
            {
                Headers["authtoken"] = config.Authtoken;
            }

            if (!string.IsNullOrEmpty(apiVersion))
            {
                Headers["api_version"] = apiVersion;
            }
            var contentstackHttpRequest = new ContentstackHttpRequest(httpClient, _serializer);
            contentstackHttpRequest.Method = new HttpMethod(HttpMethod);
            contentstackHttpRequest.RequestUri = requestUri;

            ContentBody();
            WriteContentByte();

            contentstackHttpRequest.SetRequestHeaders(Headers);
            return contentstackHttpRequest;
        }

        public virtual void ContentBody(){}

        internal void WriteContentByte()
        {
            if (ByteContent != null && ByteContent.Length > 0)
            {
                Content = new ByteArrayContent(ByteContent);
                Content.Headers.ContentLength = ByteContent.Length;
            }
        }

        public virtual void OnResponse(IResponse httpResponse, ContentstackClientOptions config) { }

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
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
        #endregion

    }
}