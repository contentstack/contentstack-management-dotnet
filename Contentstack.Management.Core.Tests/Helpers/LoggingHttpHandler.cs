using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contentstack.Management.Core.Tests.Helpers
{
    public class LoggingHttpHandler : DelegatingHandler
    {
        public LoggingHttpHandler() : base(new HttpClientHandler()) { }
        public LoggingHttpHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

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

            var response = await base.SendAsync(request, cancellationToken);

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
            var headers = new Dictionary<string, string>();
            foreach (var h in request.Headers)
                headers[h.Key] = string.Join(", ", h.Value);

            string body = null;
            if (request.Content != null)
            {
                foreach (var h in request.Content.Headers)
                    headers[h.Key] = string.Join(", ", h.Value);

                await request.Content.LoadIntoBufferAsync();
                body = await request.Content.ReadAsStringAsync();
            }

            var curl = BuildCurl(request.Method.ToString(), request.RequestUri?.ToString(), headers, body);

            TestOutputLogger.LogHttpRequest(
                method: request.Method.ToString(),
                url: request.RequestUri?.ToString() ?? "",
                headers: headers,
                body: body ?? "",
                curlCommand: curl,
                sdkMethod: ""
            );
        }

        private async Task CaptureResponse(HttpResponseMessage response)
        {
            var headers = new Dictionary<string, string>();
            foreach (var h in response.Headers)
                headers[h.Key] = string.Join(", ", h.Value);

            string body = null;
            if (response.Content != null)
            {
                foreach (var h in response.Content.Headers)
                    headers[h.Key] = string.Join(", ", h.Value);

                await response.Content.LoadIntoBufferAsync();
                body = await response.Content.ReadAsStringAsync();
            }

            TestOutputLogger.LogHttpResponse(
                statusCode: (int)response.StatusCode,
                statusText: response.ReasonPhrase ?? response.StatusCode.ToString(),
                headers: headers,
                body: body ?? ""
            );
        }

        private static string BuildCurl(string method, string url,
            IDictionary<string, string> headers, string body)
        {
            var sb = new StringBuilder();
            sb.Append($"curl -X {method} \\\n");
            sb.Append($"  '{url}' \\\n");
            foreach (var kv in headers)
                sb.Append($"  -H '{kv.Key}: {kv.Value}' \\\n");
            if (!string.IsNullOrEmpty(body))
            {
                var escaped = body.Replace("'", "'\\''");
                sb.Append($"  -d '{escaped}'");
            }
            return sb.ToString().TrimEnd('\\', '\n', ' ');
        }
    }
}
