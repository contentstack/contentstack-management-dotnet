using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core
{
    /// <summary>
    /// Abstract class for Response objects 
    /// </summary>
    public class ContentstackResponse : IResponse, IDisposable
    {
        private bool _disposed = false;

        string[] _headerNames = null!;
        Dictionary<string, string> _headers = null!;
        HashSet<string> _headerNamesSet = null!;
        private readonly HttpResponseMessage _response;
        private readonly JsonSerializerOptions _serializerOptions;

        #region Public
        /// <summary>
        /// Returns the content length of the HTTP response.
        /// </summary>
        public long ContentLength { get; private set; }

        /// <summary>
        /// Gets the property ContentType. 
        /// </summary>
        public string? ContentType { get; private set; }

        /// <summary>
        /// The HTTP status code from the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether the HTTP response was successful.
        /// </summary>
        public bool IsSuccessStatusCode { get; private set; }

        /// <summary>
        /// The entire response body from the HTTP response.
        /// </summary>
        public HttpResponseMessage ResponseBody {
            get
            {
                return _response;
            }
        }

        /// <summary>
        /// Gets the header names from HTTP response headers.
        /// </summary>
        /// <returns>The string Array</returns>
        public string[] GetHeaderNames()
        {
            return _headerNames;
        }

        /// <summary>
        /// Gets the value for the header name from HTTP response headers.
        /// </summary>
        /// <param name="headerName">Header name for which value is needed</param>
        /// <returns>The string</returns>
        public string GetHeaderValue(string headerName)
        {
            string? headerValue;
            if (_headers.TryGetValue(headerName, out headerValue))
                return headerValue;

            return string.Empty;
        }

        /// <summary>
        /// Return true if header name present in HTTP response headers.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns>The bool</returns>
        public bool IsHeaderPresent(string headerName)
        {
            return _headerNamesSet.Contains(headerName);
        }
        #endregion

        #region Private
        private string? GetFirstHeaderValue(HttpHeaders headers, string key)
        {
            IEnumerable<string>? headerValues = null;
            if (headers.TryGetValues(key, out headerValues))
                return headerValues.FirstOrDefault();

            return string.Empty;
        }

        private void CopyHeaderValues(HttpResponseMessage response)
        {
            List<string> headerNames = new List<string>();
            _headers = new Dictionary<string, string>(10, StringComparer.OrdinalIgnoreCase);

            foreach (string key in response.Headers.Select((kvp) => kvp.Key))
            {
                headerNames.Add(key);
                var headerValue = GetFirstHeaderValue(response.Headers, key);
                _headers.Add(key, headerValue ?? string.Empty);
            }

            if (response.Content != null)
            {
                foreach (var key in response.Content.Headers.Select((kvp) => kvp.Key))
                {
                    if (!headerNames.Contains(key))
                    {
                        headerNames.Add(key);
                        var headerValue = GetFirstHeaderValue(response.Content.Headers, key);
                        _headers.Add(key, headerValue ?? string.Empty);
                    }
                }
            }
            _headerNames = headerNames.ToArray();
            _headerNamesSet = new HashSet<string>(_headerNames, StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        internal ContentstackResponse(HttpResponseMessage response, JsonSerializerOptions serializerOptions)
        {
            _response = response;
            _serializerOptions = serializerOptions;

            this.StatusCode = response.StatusCode;
            this.IsSuccessStatusCode = response.IsSuccessStatusCode;
            this.ContentLength = response.Content.Headers.ContentLength ?? 0;

            if (response.Content.Headers.ContentType != null)
            {
                this.ContentType = response.Content.Headers.ContentType.MediaType ?? string.Empty;
            }
            CopyHeaderValues(response);
            
        }

        /// <summary>
        /// Json Object format response.
        /// </summary>
        /// <returns>The JsonObject.</returns>
        public JsonObject OpenJsonObjectResponse()
        {
            ThrowIfDisposed();
            return JsonNode.Parse(OpenResponse())!.AsObject();
        }

        /// <summary>
        /// Backward compatibility method for non-migrated models. Will be removed in future versions.
        /// </summary>
        /// <returns>The JObject (Newtonsoft.Json).</returns>
        [Obsolete("Use OpenJsonObjectResponse() instead. This method will be removed in future versions.")]
        public JObject OpenJObjectResponse()
        {
            ThrowIfDisposed();
            return JObject.Parse(OpenResponse());
        }

        /// <summary>
        /// String format response.
        /// </summary>
        /// <returns>The string.</returns>
        public string OpenResponse()
        {
            ThrowIfDisposed();
            return _response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Type response to serialize the response.
        /// </summary>
        /// <typeparam name="TResponse">The type to serialize the response into.</typeparam>
        /// <returns></returns>
        public TResponse? OpenTResponse<TResponse>()
        {
            ThrowIfDisposed();
            string json = OpenResponse();
            return JsonSerializer.Deserialize<TResponse>(json, _serializerOptions);
        }


        #region Dispose method
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
            if (this._disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
        #endregion
    }
}
