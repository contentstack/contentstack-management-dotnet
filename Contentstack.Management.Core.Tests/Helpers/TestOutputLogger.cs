using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Tests.Helpers
{
    public static class TestOutputLogger
    {
        private const string START_MARKER = "###TEST_OUTPUT_START###";
        private const string END_MARKER = "###TEST_OUTPUT_END###";

        public static void LogAssertion(string assertionName, object expected, object actual, bool passed)
        {
            Emit(new Dictionary<string, object>
            {
                { "type", "ASSERTION" },
                { "assertionName", assertionName },
                { "expected", expected?.ToString() ?? "null" },
                { "actual", actual?.ToString() ?? "null" },
                { "passed", passed }
            });
        }

        public static void LogHttpRequest(string method, string url,
            IDictionary<string, string> headers, string body,
            string curlCommand, string sdkMethod)
        {
            Emit(new Dictionary<string, object>
            {
                { "type", "HTTP_REQUEST" },
                { "method", method ?? "" },
                { "url", url ?? "" },
                { "headers", headers ?? new Dictionary<string, string>() },
                { "body", body ?? "" },
                { "curlCommand", curlCommand ?? "" },
                { "sdkMethod", sdkMethod ?? "" }
            });
        }

        public static void LogHttpResponse(int statusCode, string statusText,
            IDictionary<string, string> headers, string body)
        {
            Emit(new Dictionary<string, object>
            {
                { "type", "HTTP_RESPONSE" },
                { "statusCode", statusCode },
                { "statusText", statusText ?? "" },
                { "headers", headers ?? new Dictionary<string, string>() },
                { "body", body ?? "" }
            });
        }

        public static void LogContext(string key, string value)
        {
            Emit(new Dictionary<string, object>
            {
                { "type", "CONTEXT" },
                { "key", key ?? "" },
                { "value", value ?? "" }
            });
        }

        private static void Emit(object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, Formatting.None);
                Console.Write(START_MARKER);
                Console.Write(json);
                Console.WriteLine(END_MARKER);
            }
            catch
            {
                // Never let logging break a test
            }
        }
    }
}
