using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Tests
{
    /// <summary>
    /// Writes a structured JSON block to stdout so EnhancedTestReport can fully populate
    /// Assertions, HTTP Requests, HTTP Responses, and Test Context sections.
    /// Call Begin() at the start of every test, accumulate with LogRequest/LogResponse/LogAssertion,
    /// then always call Flush() in a finally block.
    /// </summary>
    public static class TestReportHelper
    {
        private static readonly string _env        = Environment.GetEnvironmentVariable("TEST_ENV")     ?? "integration";
        private static readonly string _sdkVersion = Environment.GetEnvironmentVariable("SDK_VERSION")  ?? "—";
        private static readonly string _buildNum   = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "local";
        private static readonly string _commitSha  = Environment.GetEnvironmentVariable("COMMIT_SHA")   ?? "—";

        [ThreadStatic]
        private static TestBlock _current;

        public static void Begin(string testDataSource = "appsettings.json", string locale = "en-us")
        {
            _current = new TestBlock
            {
                Context = new ContextPayload
                {
                    Environment    = _env,
                    SdkVersion     = _sdkVersion,
                    BuildNumber    = _buildNum,
                    CommitSha      = _commitSha,
                    TestDataSource = testDataSource,
                    Locale         = locale
                }
            };
        }

        public static void LogRequest(
            string sdkMethod,
            string httpMethod,
            string requestUrl,
            Dictionary<string, string> headers     = null,
            string body                            = null,
            Dictionary<string, string> queryParams = null)
        {
            if (_current == null) return;
            var baseUrl = requestUrl?.Split('?')[0] ?? "";
            var curlParts = $"curl -X {httpMethod.ToUpperInvariant()} \"{baseUrl}\"";
            if (headers != null)
                foreach (var h in headers)
                    curlParts += $" -H \"{h.Key}: {h.Value}\"";
            if (!string.IsNullOrEmpty(body))
                curlParts += $" -d '{body}'";
            _current.HttpRequests.Add(new RequestPayload
            {
                SdkMethod   = sdkMethod,
                HttpMethod  = httpMethod.ToUpperInvariant(),
                RequestUrl  = requestUrl ?? "",
                QueryParams = queryParams ?? new Dictionary<string, string>(),
                Headers     = headers    ?? new Dictionary<string, string>(),
                Body        = body       ?? "",
                CurlCommand = curlParts
            });
        }

        public static void LogResponse(
            int statusCode,
            string statusText,
            long responseTimeMs,
            string body                            = "",
            Dictionary<string, string> headers     = null)
        {
            if (_current == null) return;
            var size = System.Text.Encoding.UTF8.GetByteCount(body ?? "");
            _current.HttpResponses.Add(new ResponsePayload
            {
                StatusCode     = statusCode,
                StatusText     = statusText ?? "",
                ResponseTimeMs = responseTimeMs.ToString(),
                Headers        = headers ?? new Dictionary<string, string>(),
                Body           = body    ?? "",
                PayloadSize    = $"{size} B"
            });
        }

        public static void LogAssertion(
            bool passed,
            string name,
            string expected        = "",
            string actual          = "",
            string type            = "Assert")
        {
            if (_current == null) return;
            _current.Assertions.Add(new AssertionPayload
            {
                Passed        = passed,
                Name          = name ?? "",
                Expected      = expected ?? "",
                Actual        = actual   ?? "",
                AssertionType = type     ?? "Assert"
            });
        }

        /// <summary>Always call in a finally block at the end of every test method.</summary>
        public static void Flush()
        {
            var block = _current;
            _current = null;
            if (block == null) return;
            try
            {
                var json = JsonSerializer.Serialize(block, new JsonSerializerOptions
                {
                    WriteIndented        = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
                Console.WriteLine("##TEST_REPORT_START##");
                Console.WriteLine(json);
                Console.WriteLine("##TEST_REPORT_END##");
            }
            catch
            {
                // Never throw from Flush — test result must not be affected
            }
        }

        // ── Payload POCOs ────────────────────────────────────────────────────────

        private class TestBlock
        {
            [JsonPropertyName("assertions")]
            public List<AssertionPayload> Assertions   { get; set; } = new();
            [JsonPropertyName("httpRequests")]
            public List<RequestPayload>  HttpRequests  { get; set; } = new();
            [JsonPropertyName("httpResponses")]
            public List<ResponsePayload> HttpResponses { get; set; } = new();
            [JsonPropertyName("context")]
            public ContextPayload Context { get; set; }
        }

        private class AssertionPayload
        {
            [JsonPropertyName("passed")]        public bool   Passed        { get; set; }
            [JsonPropertyName("name")]          public string Name          { get; set; }
            [JsonPropertyName("expected")]      public string Expected      { get; set; }
            [JsonPropertyName("actual")]        public string Actual        { get; set; }
            [JsonPropertyName("assertionType")] public string AssertionType { get; set; }
        }

        private class RequestPayload
        {
            [JsonPropertyName("sdkMethod")]   public string SdkMethod   { get; set; }
            [JsonPropertyName("httpMethod")]  public string HttpMethod   { get; set; }
            [JsonPropertyName("requestUrl")]  public string RequestUrl   { get; set; }
            [JsonPropertyName("queryParams")] public Dictionary<string, string> QueryParams { get; set; }
            [JsonPropertyName("headers")]     public Dictionary<string, string> Headers     { get; set; }
            [JsonPropertyName("body")]        public string Body         { get; set; }
            [JsonPropertyName("curlCommand")] public string CurlCommand  { get; set; }
        }

        private class ResponsePayload
        {
            [JsonPropertyName("statusCode")]     public int    StatusCode     { get; set; }
            [JsonPropertyName("statusText")]     public string StatusText     { get; set; }
            [JsonPropertyName("responseTimeMs")] public string ResponseTimeMs { get; set; }
            [JsonPropertyName("headers")]        public Dictionary<string, string> Headers { get; set; }
            [JsonPropertyName("body")]           public string Body           { get; set; }
            [JsonPropertyName("payloadSize")]    public string PayloadSize    { get; set; }
        }

        private class ContextPayload
        {
            [JsonPropertyName("environment")]    public string Environment    { get; set; }
            [JsonPropertyName("sdkVersion")]     public string SdkVersion     { get; set; }
            [JsonPropertyName("buildNumber")]    public string BuildNumber    { get; set; }
            [JsonPropertyName("commitSha")]      public string CommitSha      { get; set; }
            [JsonPropertyName("testDataSource")] public string TestDataSource { get; set; }
            [JsonPropertyName("locale")]         public string Locale         { get; set; }
        }
    }
}
