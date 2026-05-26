using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Contentstack.Management.Core;

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

        public JsonObject OpenJsonObjectResponse()
        {
            if (string.IsNullOrEmpty(_responseContent))
                return new JsonObject();
            
            try
            {
                return JsonNode.Parse(_responseContent)!.AsObject();
            }
            catch
            {
                // Return empty JsonObject if parsing fails
                return new JsonObject();
            }
        }

        [Obsolete("Use OpenJsonObjectResponse() instead.")]
        public JObject OpenJObjectResponse()
        {
            return null;
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
                return JsonSerializer.Deserialize<TResponse>(_responseContent);
            }
            catch
            {
                return default(TResponse);
            }
        }
    }
} 