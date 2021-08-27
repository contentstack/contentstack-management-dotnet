using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using Contentstack.Management.Core.Http;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Services
{
    internal class ContentstackService : IContentstackService
    {

        #region
        internal readonly ParameterCollection collection;
        readonly IDictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly IDictionary<string, string> pathResources = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly IDictionary<string, string> queryResources = new Dictionary<string, string>(StringComparer.Ordinal);

        string resourcePath;
        byte[] content;
        //Stream contentStream;
        string httpMethod = "GET";
        bool useQueryString = false;

        private bool _disposed = false;
        private JsonSerializer _serializer { get; set; }

        #endregion

        #region Constructor
        internal ContentstackService(JsonSerializer serializer, ParameterCollection collection = null)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
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
                return useQueryString;
            }
            set
            {
                useQueryString = value;
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

        public string ResourcePath
        {
            get
            {
                return resourcePath;
            }
            set
            {
                resourcePath = value;
            }
        }

        public IDictionary<string, string> PathResources
        {
            get
            {
                return pathResources;
            }
        }

        public byte[] Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }
        //public bool SetContentFromParameters { get; set; }
        //public Stream ContentStream
        //{
        //    get { return contentStream; }
        //    set
        //    {
        //        contentStream = value;
        //    }
        //}
        public string HttpMethod
        {
            get
            {
                return httpMethod;
            }
            set
            {
                httpMethod = value;
            }
        }

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
            return HttpMethod == "POST" || HttpMethod == "PUT" || HttpMethod == "PATCH";
        }

        public virtual IHttpRequest CreateHttpRequest(HttpClient httpClient, ContentstackClientOptions config)
        {
            ThrowIfDisposed();

            Uri requestUri = ContentstackUtilities.ComposeUrI(config.GetUri(), this);
            Headers["Content-Type"] = "application/json";

            if (!string.IsNullOrEmpty(config.Authtoken))
            {
                Headers["authtoken"] = config.Authtoken;
            }

            var contentstackHttpRequest = new ContentstackHttpRequest(httpClient, _serializer);
            contentstackHttpRequest.Method = new HttpMethod(HttpMethod);
            contentstackHttpRequest.RequestUri = requestUri;

            ContentBody();

            contentstackHttpRequest.SetRequestHeaders(Headers);
            return contentstackHttpRequest;
        }

        public virtual void ContentBody(){}

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