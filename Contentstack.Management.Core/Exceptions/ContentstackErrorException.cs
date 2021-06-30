using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public HttpStatusCode StatusCode;

        /// <summary>
        /// This is http response Header of REST request to Contentstack.
        /// </summary>
        public HttpResponseHeaders Header;

        /// <summary>
        /// This is http response phrase code of REST request to Contentstack.
        /// </summary>
        public string ReasonPhrase;

        /// <summary>
        /// This is error message.
        /// </summary>
        public new string Message;

        /// <summary>
        /// This is error message.
        /// </summary>
        [JsonProperty("error_message")]
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
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Set of errors in detail.
        /// </summary>
        [JsonProperty("errors")]
        public Dictionary<string, object> Errors { get; set; }
        #endregion
        public static ContentstackErrorException CreateException(HttpResponseMessage response)
        {
            var stringResponse = response.Content.ReadAsStringAsync().Result;
            ContentstackErrorException exception = null;
            if (!string.IsNullOrEmpty(stringResponse))
            {
                exception = JObject.Parse(stringResponse).ToObject<ContentstackErrorException>();
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
