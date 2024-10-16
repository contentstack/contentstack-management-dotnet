using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Unit.Tests.Utils;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    internal class MockService : IContentstackService
    {
        public MockService(ParameterCollection pairs = null)
        {
            parameters = pairs;
            _serializer = JsonSerializer.Create(new JsonSerializerSettings());
        }
        #region
        readonly ParameterCollection parameters = new ParameterCollection();
        readonly IDictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly IDictionary<string, string> pathResources = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly IDictionary<string, string> queryResources = new Dictionary<string, string>(StringComparer.Ordinal);

        string resourcePath;
        byte[] content;
        HttpContent httpContent;
        string httpMethod = "GET";
        string managementToken = null;
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

        public ParameterCollection Parameters
        {
            get
            {
                return parameters;
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

        public byte[] ByteContent
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

        public HttpContent Content
        {
            get
            {
                return httpContent;
            }
            set
            {
                httpContent = value;
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

        public string ManagementToken
        {
            get
            {
                return managementToken;
            }
            set
            {
                managementToken = value;
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

        public virtual IHttpRequest CreateHttpRequest(HttpClient httpClient, ContentstackClientOptions config, bool addAcceptMediaHeader = false, string apiVersion = null)
        {
            var contentstackHttpRequest = new ContentstackHttpRequest(httpClient, _serializer);
            contentstackHttpRequest.Method = new HttpMethod(HttpMethod);
            contentstackHttpRequest.RequestUri = new Uri("https://localhost.com");
            ContentBody();
            WriteContentByte();
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
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
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

        protected virtual void Dispose(bool disposing) { }
        #endregion

    }
}