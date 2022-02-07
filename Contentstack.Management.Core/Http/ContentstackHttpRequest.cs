using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Contentstack.Management.Core.Utils;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Http
{
    internal class ContentstackHttpRequest: IHttpRequest
    {
        #region Private
        private bool _disposed = false;
        private HttpClient _httpClient;
        private HttpRequestMessage _request;
        private JsonSerializer _serializer;
        #endregion

        #region Public
        /// <summary>
        /// The HTTP method or verb.
        /// </summary>
        public HttpMethod Method
        {
            get { return _request.Method; }
            set { _request.Method = value; }
        }

        /// <summary>
        /// The request URI.
        /// </summary>
        public Uri RequestUri { get; set; }

        /// <summary>
        /// The underlying HttpClient
        /// </summary>
        public HttpClient HttpClient
        {
            get { return _httpClient; }
        }

        /// <summary>
        /// The underlying HTTP web request.
        /// </summary>
        public HttpRequestMessage Request
        {
            get { return _request; }
        }

        #endregion

        #region Constructor
        internal ContentstackHttpRequest(HttpClient httpClient, JsonSerializer serializer)
        {
            _httpClient = httpClient;
            _serializer = serializer;
            _request = new HttpRequestMessage();
        }
        #endregion

        #region Dispose methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _disposed = true;
                if (_request != null)
                {
                    _request.Dispose();
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion

        /// <summary>
        /// Returns the HTTP response.
        /// </summary>
        /// <returns></returns>
        public IResponse GetResponse()
        {
            ThrowIfDisposed();
            try
            {
                return this.GetResponseAsync().Result;
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Returns the HTTP response.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IResponse> GetResponseAsync()
        {
            ThrowIfDisposed();
            try
            {
                _request.RequestUri = this.RequestUri;

                var responseMessage = await _httpClient.SendAsync(_request, HttpCompletionOption.ResponseHeadersRead)
        .ConfigureAwait(continueOnCapturedContext: false);

                if (responseMessage.StatusCode >= HttpStatusCode.Ambiguous &&
                    responseMessage.StatusCode < HttpStatusCode.BadRequest)
                    return new ContentstackResponse(responseMessage, _serializer);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw ContentstackErrorException.CreateException(responseMessage);
                }

                return new ContentstackResponse(responseMessage, _serializer);
            }
            catch (HttpRequestException httpException)
            {
                if (httpException.InnerException != null)
                {
                    if (httpException.InnerException is IOException)
                    {
                        throw httpException.InnerException;
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// Sets the headers on the request.
        /// </summary>
        /// <param name="headers">A dictionary of header names and values.</param>
        public void SetRequestHeaders(IDictionary<string, string> headers)
        {
            ThrowIfDisposed();
            foreach (var kvp in headers)
            {
                _request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Writes a stream to the request body.
        /// </summary>
        /// <param name="content">The destination where the content stream is written.</param>
        public void WriteToRequestBody(HttpContent content)
        {
            _request.Content = content;
        }

        /// <summary>
        /// Gets a handle to the request content.
        /// </summary>
        /// <returns>The <see cref="HttpContent"/>.</returns>
        public HttpContent GetRequestContent()
        {
            ThrowIfDisposed();
            return System.Threading.Tasks.Task.FromResult(_request.Content).Result;
        }

        /// <summary>
        /// Writes a stream to the request body.
        /// </summary>
        /// <param name="content">The destination where the content stream is written.</param>
        /// <param name="contentHeaders">A dictionary of header names and values.</param>
        public void WriteToRequestBody(HttpContent content, IDictionary<string, string> contentHeaders)
        {
            ThrowIfDisposed();
            WriteToRequestBody(content);
            WriteContentHeaders(contentHeaders);
        }

        private void WriteContentHeaders(IDictionary<string, string> contentHeaders)
        {
            _request.Content.Headers.ContentType =
                MediaTypeHeaderValue.Parse(contentHeaders[HeadersKey.ContentTypeHeader]);
        }
    }
}
