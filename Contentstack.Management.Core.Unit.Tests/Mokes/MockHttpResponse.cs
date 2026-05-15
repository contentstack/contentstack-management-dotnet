using System;
using System.Collections.Generic;
using System.Net;
using Contentstack.Management.Core;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Mock HTTP response for unit testing.
    /// </summary>
    public class MockHttpResponse : IResponse
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;
        private readonly Dictionary<string, string> _headers;
        private readonly string[] _headerNames;

        public MockHttpResponse(int statusCode, string responseContent = null, Dictionary<string, string> headers = null)
        {
            _statusCode = (HttpStatusCode)statusCode;
            _responseContent = responseContent ?? string.Empty;
            _headers = headers ?? new Dictionary<string, string>();
            _headerNames = new string[_headers.Count];
            _headers.Keys.CopyTo(_headerNames, 0);
        }

        public long ContentLength => _responseContent?.Length ?? 0;

        public string ContentType => "application/json";

        public HttpStatusCode StatusCode => _statusCode;

        public bool IsSuccessStatusCode => (int)_statusCode >= 200 && (int)_statusCode < 300;

        public string GetHeaderValue(string headerName)
        {
            _headers.TryGetValue(headerName, out string value);
            return value ?? string.Empty;
        }

        public string[] GetHeaderNames()
        {
            return _headerNames;
        }

        public bool IsHeaderPresent(string headerName)
        {
            return _headers.ContainsKey(headerName);
        }

        public JObject OpenJObjectResponse()
        {
            if (string.IsNullOrEmpty(_responseContent))
                return new JObject();
            
            try
            {
                return JObject.Parse(_responseContent);
            }
            catch
            {
                // Return empty JObject if parsing fails
                return new JObject();
            }
        }

        public string OpenResponse()
        {
            return _responseContent;
        }

        public TResponse OpenTResponse<TResponse>()
        {
            if (string.IsNullOrEmpty(_responseContent))
                return default(TResponse);

            try
            {
                var jObject = OpenJObjectResponse();
                return jObject.ToObject<TResponse>();
            }
            catch
            {
                return default(TResponse);
            }
        }
    }
} 