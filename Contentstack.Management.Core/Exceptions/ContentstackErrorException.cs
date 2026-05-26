using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Exceptions
{
    /// <summary>
    /// A base exception for Contentstack API.
    /// </summary>
    public class ContentstackErrorException: Exception
    {
        #region Private Variables
        private string _ErrorMessage = string.Empty;
        #endregion

        #region Public Variables
        /// <summary>
        /// This is http response status code of REST request to Contentstack.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// This is http response Header of REST request to Contentstack.
        /// </summary>
        public HttpResponseHeaders Header { get; set; }

        /// <summary>
        /// This is http response phrase code of REST request to Contentstack.
        /// </summary>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// This is error message.
        /// </summary>
        public new string Message { get; set; }

        /// <summary>
        /// This is error message.
        /// </summary>
        [JsonPropertyName("error_message")]
        public string ErrorMessage
        {
            get
            {
                return this._ErrorMessage;
            }
            set
            {
                this._ErrorMessage = value;
                this.Message = value;
            }
        }

        /// <summary>
        /// This is error code.
        /// </summary>
        [JsonPropertyName("error_code")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Set of errors in detail.
        /// </summary>
        [JsonPropertyName("errors")]
        public Dictionary<string, object> Errors { get; set; }

        /// <summary>
        /// Number of retry attempts made before this exception was thrown.
        /// </summary>
        public int RetryAttempts { get; set; }

        /// <summary>
        /// The original exception that caused this error, if this is a network error wrapped in an HTTP exception.
        /// </summary>
        public Exception OriginalError { get; set; }

        /// <summary>
        /// Indicates whether this error originated from a network failure.
        /// </summary>
        public bool IsNetworkError { get; set; }
        #endregion
        public static ContentstackErrorException CreateException(HttpResponseMessage response)
        {
            var stringResponse = response.Content.ReadAsStringAsync().Result;
            ContentstackErrorException exception = null;
            if (!string.IsNullOrEmpty(stringResponse))
            {
                try
                {
                    exception = JsonSerializer.Deserialize<ContentstackErrorException>(stringResponse);
                }
                catch (JsonException)
                {
                    // Handle HTML error responses or other non-JSON content
                    exception = new ContentstackErrorException();
                    
                    // Extract meaningful error message from HTML if possible
                    if (stringResponse.Contains("Cannot GET") || stringResponse.Contains("Cannot POST") || stringResponse.Contains("Cannot PUT") || stringResponse.Contains("Cannot DELETE"))
                    {
                        // Extract the endpoint path from HTML error message
                        var startIndex = stringResponse.IndexOf("Cannot");
                        var endIndex = stringResponse.IndexOf("</pre>", startIndex);
                        if (startIndex >= 0 && endIndex > startIndex)
                        {
                            var errorMessage = stringResponse.Substring(startIndex, endIndex - startIndex).Trim();
                            exception.ErrorMessage = $"API endpoint error: {errorMessage}";
                        }
                        else
                        {
                            exception.ErrorMessage = "API endpoint not found or not supported";
                        }
                    }
                    else
                    {
                        exception.ErrorMessage = "Invalid response format received from server";
                    }
                }
                catch (Exception ex)
                {
                    // Handle any other JSON parsing issues
                    exception = new ContentstackErrorException();
                    exception.ErrorMessage = $"Failed to parse server response: {ex.Message}";
                }
            }
            else
            {
                exception = new ContentstackErrorException();
            }

            exception.StatusCode = response.StatusCode;
            exception.Header = response.Headers;
            exception.ReasonPhrase = response.ReasonPhrase;
            return exception;
        }
        public ContentstackErrorException() { }
    }
}
