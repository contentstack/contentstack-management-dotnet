using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Tests.Helpers
{
    public class MockHttpStatusHandler : LoggingHttpHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _errorMessage;
        private readonly int _errorCode;
        private readonly Dictionary<string, object> _additionalFields;

        public MockHttpStatusHandler(HttpStatusCode statusCode, string errorMessage = null, 
            int errorCode = 0, Dictionary<string, object> additionalFields = null)
        {
            _statusCode = statusCode;
            _errorMessage = errorMessage ?? GetDefaultErrorMessage(statusCode);
            _errorCode = errorCode > 0 ? errorCode : GetDefaultErrorCode(statusCode);
            _additionalFields = additionalFields ?? new Dictionary<string, object>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                await CaptureRequest(request);
            }
            catch
            {
                // Never let logging break the request
            }

            var errorResponse = new
            {
                error_message = _errorMessage,
                error_code = _errorCode,
                errors = _additionalFields
            };

            var jsonContent = JsonConvert.SerializeObject(errorResponse);
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                ReasonPhrase = GetReasonPhrase(_statusCode)
            };

            try
            {
                await CaptureResponse(response);
            }
            catch
            {
                // Never let logging break the response
            }

            return response;
        }

        private async Task CaptureRequest(HttpRequestMessage request)
        {
            try
            {
                TestOutputLogger.LogHttpRequest(
                    method: request.Method.ToString(),
                    url: request.RequestUri?.ToString() ?? "",
                    headers: new Dictionary<string, string>(),
                    body: request.Content != null ? await request.Content.ReadAsStringAsync() : "",
                    curlCommand: $"curl -X {request.Method} '{request.RequestUri}'",
                    sdkMethod: $"MockStatus:{_statusCode}"
                );
            }
            catch
            {
                // Never let logging break the request
            }
        }

        private async Task CaptureResponse(HttpResponseMessage response)
        {
            try
            {
                TestOutputLogger.LogHttpResponse(
                    statusCode: (int)response.StatusCode,
                    statusText: response.ReasonPhrase ?? response.StatusCode.ToString(),
                    headers: new Dictionary<string, string>
                    {
                        ["Content-Type"] = "application/json"
                    },
                    body: response.Content != null ? await response.Content.ReadAsStringAsync() : ""
                );
            }
            catch
            {
                // Never let logging break the response
            }
        }

        private static string GetDefaultErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.Unauthorized => "Authentication failed. Please check your credentials.",
                HttpStatusCode.Forbidden => "Access denied. Insufficient permissions.",
                HttpStatusCode.TooManyRequests => "Rate limit exceeded. Please try again later.",
                HttpStatusCode.InternalServerError => "Internal server error occurred.",
                HttpStatusCode.BadGateway => "Bad gateway. Upstream server error.",
                HttpStatusCode.ServiceUnavailable => "Service temporarily unavailable.",
                HttpStatusCode.RequestTimeout => "Request timeout occurred.",
                HttpStatusCode.Conflict => "Conflict occurred while processing request.",
                HttpStatusCode.Gone => "The requested resource is no longer available.",
                HttpStatusCode.BadRequest => "Bad request. Invalid input provided.",
                _ => $"HTTP {(int)statusCode} error occurred."
            };
        }

        private static int GetDefaultErrorCode(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.Unauthorized => 401,
                HttpStatusCode.Forbidden => 403,
                HttpStatusCode.TooManyRequests => 429,
                HttpStatusCode.InternalServerError => 500,
                HttpStatusCode.BadGateway => 502,
                HttpStatusCode.ServiceUnavailable => 503,
                HttpStatusCode.RequestTimeout => 408,
                HttpStatusCode.Conflict => 409,
                HttpStatusCode.Gone => 410,
                HttpStatusCode.BadRequest => 400,
                _ => (int)statusCode
            };
        }

        private static string GetReasonPhrase(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.Unauthorized => "Unauthorized",
                HttpStatusCode.Forbidden => "Forbidden",
                HttpStatusCode.TooManyRequests => "Too Many Requests",
                HttpStatusCode.InternalServerError => "Internal Server Error",
                HttpStatusCode.BadGateway => "Bad Gateway",
                HttpStatusCode.ServiceUnavailable => "Service Unavailable",
                HttpStatusCode.RequestTimeout => "Request Timeout",
                HttpStatusCode.Conflict => "Conflict",
                HttpStatusCode.Gone => "Gone",
                HttpStatusCode.BadRequest => "Bad Request",
                _ => statusCode.ToString()
            };
        }
    }

    public class MockMalformedResponseHandler : LoggingHttpHandler
    {
        private readonly string _responseContent;
        private readonly HttpStatusCode _statusCode;

        public MockMalformedResponseHandler(string responseContent, 
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responseContent = responseContent;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                await CaptureRequest(request);
            }
            catch
            {
                // Never let logging break the request
            }

            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };

            try
            {
                await CaptureResponse(response);
            }
            catch
            {
                // Never let logging break the response
            }

            return response;
        }

        private async Task CaptureRequest(HttpRequestMessage request)
        {
            try
            {
                TestOutputLogger.LogHttpRequest(
                    method: request.Method.ToString(),
                    url: request.RequestUri?.ToString() ?? "",
                    headers: new Dictionary<string, string>(),
                    body: request.Content != null ? await request.Content.ReadAsStringAsync() : "",
                    curlCommand: $"curl -X {request.Method} '{request.RequestUri}'",
                    sdkMethod: $"MockMalformed:{_statusCode}"
                );
            }
            catch
            {
                // Never let logging break the request
            }
        }

        private async Task CaptureResponse(HttpResponseMessage response)
        {
            try
            {
                TestOutputLogger.LogHttpResponse(
                    statusCode: (int)response.StatusCode,
                    statusText: response.ReasonPhrase ?? response.StatusCode.ToString(),
                    headers: new Dictionary<string, string>
                    {
                        ["Content-Type"] = "application/json"
                    },
                    body: _responseContent
                );
            }
            catch
            {
                // Never let logging break the response
            }
        }
    }
}