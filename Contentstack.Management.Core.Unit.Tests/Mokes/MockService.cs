using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Unit.Tests.Utils;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    internal class MockService : IContentstackService
    {
        public MockService()
        {
            _serializer = JsonSerializer.Create(new JsonSerializerSettings());
        }
        #region
        readonly IDictionary<string, string> parametersFacade = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly IDictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly IDictionary<string, string> pathResources = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly IDictionary<string, string> queryResources = new Dictionary<string, string>(StringComparer.Ordinal);

        string resourcePath;
        byte[] content;
        string httpMethod = "GET";
        bool useQueryString = false;

        private JsonSerializer _serializer { get; set; }

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

        public IDictionary<string, string> Parameters
        {
            get
            {
                return parametersFacade;
            }
        }

        public IDictionary<string, string> Headers
        {
            get
            {
                return headers;
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

        public IDictionary<string, string> QueryResources
        {
            get
            {
                return queryResources;
            }
        }

        public void AddQueryResource(string queryResource, string value)
        {
            QueryResources.Add(queryResource, value);
        }


        public void AddPathResource(string key, string value)
        {
            PathResources.Add(key, value);
        }

        public string GetHeaderValue(string headerName)
        {
            string headerValue;
            if (headers.TryGetValue(headerName, out headerValue))
                return headerValue;

            return string.Empty;
        }

        public bool HasRequestBody()
        {
            return HttpMethod == "POST" || HttpMethod == "PUT" || HttpMethod == "PATCH";
        }

        public virtual IHttpRequest CreateHttpRequest(HttpClient httpClient, ContentstackClientOptions config)
        {
            var contentstackHttpRequest = new ContentstackHttpRequest(httpClient, _serializer);
            contentstackHttpRequest.Method = new HttpMethod(HttpMethod);
            contentstackHttpRequest.RequestUri = new Uri("https://localhost.com");
            ContentBody();
            return contentstackHttpRequest;
        }

        internal virtual void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("user");
                writer.WriteValue("test_Mock_user");
                writer.WriteEndObject();

                string snippet = stringWriter.ToString();
                this.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
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

        protected virtual void Dispose(bool disposing) { }
        #endregion

    }
}