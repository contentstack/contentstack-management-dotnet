using System;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Collections.Generic;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Unit.Tests.Utils;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    public class MockResponse
    {
        public static ContentstackResponse CreateContentstackResponse(string resourceName)
        {
            HttpResponseMessage httpResponseMessage = CreateFromResource(resourceName);
            return new ContentstackResponse(httpResponseMessage, JsonSerializer.Create(new JsonSerializerSettings()));
        }

        public static HttpResponseMessage CreateFromResource(string resourceName)
        {
            var rawResponse = Utilities.GetResourceText(resourceName);

            var response = ParseRawReponse(rawResponse);
            var statusCode = ParseStatusCode(response.StatusLine);

            return Create(statusCode, response.Headers, response.Body);
        }


        public static HttpResponseMessage Create(HttpStatusCode statusCode,
            IDictionary<string, string> headers, string body = null)
        {
            var type = typeof(HttpResponseMessage);
            var assembly = Assembly.GetAssembly(type);
            var obj = assembly.CreateInstance("System.Net.Http.HttpResponseMessage");

            var responseHeader = (obj as HttpResponseMessage).Headers;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    responseHeader.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            HttpContent responseBodyContent = null;
            body = body ?? string.Empty;
            responseBodyContent = Utilities.CreateStreamFromString(body);

            (obj as HttpResponseMessage).StatusCode = statusCode;
            (obj as HttpResponseMessage).Content = responseBodyContent;
            (obj as HttpResponseMessage).StatusCode = statusCode;

            return obj as HttpResponseMessage;
        }

        static HttpResponse ParseRawReponse(string rawResponse)
        {
            var response = new HttpResponse();
            var responseLines = rawResponse.Split('\n');

            if (responseLines.Count() == 0)
                throw new ArgumentException("The resource does not contain a valid HTTP response.",
                    "resourceName");

            response.StatusLine = responseLines[0];
            var currentLine = responseLines[0];
            var statusCode = ParseStatusCode(currentLine);

            var lineIndex = 0;
            if (responseLines.Count() > 1)
            {
                for (lineIndex = 1; lineIndex < responseLines.Count(); lineIndex++)
                {
                    currentLine = responseLines[lineIndex];
                    if (currentLine.Trim() == string.Empty)
                    {
                        currentLine = responseLines[lineIndex - 1];
                        break;
                    }

                    var index = currentLine.IndexOf(":");
                    if (index != -1)
                    {
                        var headerKey = currentLine.Substring(0, index);
                        var headerValue = currentLine.Substring(index + 1);
                        response.Headers.Add(headerKey.Trim(), headerValue.Trim());
                    }
                }
            }

            var startOfBody = rawResponse.IndexOf(currentLine) + currentLine.Length;
            response.Body = rawResponse.Substring(startOfBody).Trim();
            return response;
        }

        private static HttpStatusCode ParseStatusCode(string statusLine)
        {
            var statusCode = string.Empty;
            try
            {
                statusCode = statusLine.Split(' ')[1];
                return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), statusCode);
            }
            catch (Exception exception)
            {
                throw new ArgumentException("Invalid HTTP status line.", exception);
            }
        }

        class HttpResponse
        {
            public HttpResponse()
            {
                this.Headers = new Dictionary<string, string>();
            }
            public string StatusLine { get; set; }
            public IDictionary<string, string> Headers { get; private set; }
            public string Body { get; set; }
        }
    }
}
