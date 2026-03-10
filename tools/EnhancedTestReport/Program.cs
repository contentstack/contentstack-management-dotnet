using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

const string StartMarker = "##TEST_REPORT_START##";
const string EndMarker = "##TEST_REPORT_END##";

var trxPaths = new List<string>();
var coberturaPaths = new List<string>();
string? outputPath = null;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--trx":
            if (i + 1 < args.Length) trxPaths.Add(args[++i]);
            break;
        case "--trx-dir":
            if (i + 1 < args.Length)
                foreach (var p in Directory.GetFiles(args[++i], "*.trx", SearchOption.AllDirectories))
                    trxPaths.Add(p);
            break;
        case "--cobertura":
            if (i + 1 < args.Length) coberturaPaths.Add(args[++i]);
            break;
        case "--cobertura-dir":
            if (i + 1 < args.Length)
                foreach (var p in Directory.GetFiles(args[++i], "coverage.cobertura.xml", SearchOption.AllDirectories))
                    coberturaPaths.Add(p);
            break;
        case "--output":
            if (i + 1 < args.Length) outputPath = args[++i];
            break;
        case "--help":
        case "-h":
            PrintHelp();
            return 0;
    }
}

trxPaths = trxPaths.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
coberturaPaths = coberturaPaths.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

if (trxPaths.Count == 0 && coberturaPaths.Count == 0)
{
    Console.Error.WriteLine("No TRX and no Cobertura files found. Use --trx / --trx-dir and/or --cobertura / --cobertura-dir.");
    PrintHelp();
    return 1;
}

outputPath ??= "EnhancedTestReport.html";

var results = new List<TestResultRow>();
foreach (var trx in trxPaths)
{
    if (!File.Exists(trx)) continue;
    try { results.AddRange(ParseTrx(trx)); }
    catch (Exception ex) { Console.Error.WriteLine($"WARN: TRX {trx}: {ex.Message}"); }
}

var coverageRows = new List<CoverageFileRow>();
foreach (var cob in coberturaPaths)
{
    if (!File.Exists(cob)) continue;
    try { coverageRows.AddRange(ParseCobertura(cob)); }
    catch (Exception ex) { Console.Error.WriteLine($"WARN: Cobertura {cob}: {ex.Message}"); }
}

var html = GenerateHtml(results, coverageRows, trxPaths, coberturaPaths);
var outDir = Path.GetDirectoryName(Path.GetFullPath(outputPath));
if (!string.IsNullOrEmpty(outDir))
    Directory.CreateDirectory(outDir);
File.WriteAllText(outputPath, html, Encoding.UTF8);
Console.WriteLine($"Wrote {outputPath}");
return 0;

static void PrintHelp()
{
    Console.WriteLine("""
        EnhancedTestReport [options]

          --trx <path>           Add a TRX file (repeatable).
          --trx-dir <dir>        Recursively glob *.trx under dir.
          --cobertura <path>     Add a Cobertura XML file (repeatable).
          --cobertura-dir <dir> Recursively glob coverage.cobertura.xml under dir.
          --output <path>        Output HTML (default: EnhancedTestReport.html).
          --help, -h
        """);
}

static IEnumerable<TestResultRow> ParseTrx(string path)
{
    var doc = XDocument.Load(path);
    var results = doc.Descendants().Where(e => e.Name.LocalName == "UnitTestResult");
    foreach (var el in results)
    {
        var testName = (string?)el.Attribute("testName") ?? "(unknown)";
        var outcome = (string?)el.Attribute("outcome") ?? "";
        var duration = (string?)el.Attribute("duration") ?? "";
        var outputEl = el.Descendants().FirstOrDefault(e => e.Name.LocalName == "Output");
        var stdOut = outputEl?.Descendants().FirstOrDefault(e => e.Name.LocalName == "StdOut")?.Value ?? "";
        var errorInfo = outputEl?.Descendants().FirstOrDefault(e => e.Name.LocalName == "ErrorInfo");
        var message = errorInfo?.Descendants().FirstOrDefault(e => e.Name.LocalName == "Message")?.Value;
        var stackTrace = errorInfo?.Descendants().FirstOrDefault(e => e.Name.LocalName == "StackTrace")?.Value;

        var enrich = EnrichFromStdOut(stdOut, message, stackTrace, outcome);
        yield return new TestResultRow(path, testName, outcome, duration, stdOut, message, stackTrace, enrich);
    }
}

static EnrichPayload EnrichFromStdOut(string stdOut, string? message, string? stackTrace, string outcome)
{
    var payload = new EnrichPayload();
    var start = stdOut.IndexOf(StartMarker, StringComparison.Ordinal);
    var end = stdOut.IndexOf(EndMarker, StringComparison.Ordinal);
    if (start >= 0 && end > start)
    {
        var json = stdOut.AsSpan(start + StartMarker.Length, end - start - StartMarker.Length).ToString().Trim();
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var parsed = JsonSerializer.Deserialize<EnrichPayload>(json, options);
            if (parsed != null)
                return parsed;
        }
        catch { /* fallback below */ }
    }

    // Heuristic: assertions from failure message Expected/Actual
    if (outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(message))
    {
        var m = Regex.Match(message, @"Expected\s*:\s*(.+?)(?:\r|\n|$)", RegexOptions.Singleline);
        var a = Regex.Match(message, @"Actual\s*:\s*(.+?)(?:\r|\n|$)", RegexOptions.Singleline);
        if (m.Success || a.Success)
        {
            payload.Assertions ??= new List<AssertionDto>();
            payload.Assertions.Add(new AssertionDto
            {
                Passed = false,
                Name = "Assertion (from message)",
                Expected = m.Success ? m.Groups[1].Value.Trim() : null,
                Actual = a.Success ? a.Groups[1].Value.Trim() : null,
                AssertionType = "Fail"
            });
        }
    }

    // Heuristic: lines like "making request /path" -> GET
    foreach (Match match in Regex.Matches(stdOut, @"making request\s+(\S+)", RegexOptions.IgnoreCase))
    {
        payload.HttpRequests ??= new List<HttpRequestDto>();
        payload.HttpRequests.Add(new HttpRequestDto
        {
            HttpMethod = "GET",
            RequestUrl = match.Groups[1].Value
        });
    }

    // Heuristic: HTTP status in failure text
    if (!string.IsNullOrEmpty(message))
    {
        var statusMatch = Regex.Match(message, @"\b(\d{3})\s+([A-Za-z ]+)?");
        if (statusMatch.Success && int.TryParse(statusMatch.Groups[1].Value, out var code) && code >= 400)
        {
            payload.HttpResponses ??= new List<HttpResponseDto>();
            payload.HttpResponses.Add(new HttpResponseDto
            {
                StatusCode = code,
                StatusText = statusMatch.Groups[2].Value.Trim()
            });
        }
    }

    return payload;
}

static IEnumerable<CoverageFileRow> ParseCobertura(string path)
{
    var doc = XDocument.Load(path);
    foreach (var cls in doc.Descendants().Where(e => e.Name.LocalName == "class"))
    {
        var filename = (string?)cls.Attribute("filename");
        if (string.IsNullOrEmpty(filename)) continue;
        var lineRate = (string?)cls.Attribute("line-rate");
        var branchRate = (string?)cls.Attribute("branch-rate");
        var lines = cls.Descendants().Where(e => e.Name.LocalName == "line");
        var total = 0;
        var covered = 0;
        foreach (var line in lines)
        {
            total++;
            if (int.TryParse((string?)line.Attribute("hits"), out var hits) && hits > 0)
                covered++;
        }
        yield return new CoverageFileRow(filename, lineRate, branchRate, total, covered);
    }
}

static string GenerateHtml(List<TestResultRow> results, List<CoverageFileRow> coverage,
    List<string> trxPaths, List<string> coberturaPaths)
{
    var passed = results.Count(r => r.Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase));
    var failed = results.Count - passed;
    var failedList = results.Where(r => !r.Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase)).ToList();
    var passedList = results.Where(r => r.Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase)).ToList();
    var sb = new StringBuilder();
    sb.Append("<!DOCTYPE html><html lang='en'><head><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'>");
    sb.Append("<title>Enhanced Test Report – Contentstack .NET</title><style>").Append(GetCss()).Append("</style></head><body>");
    sb.Append("<div class='layout'><aside class='sidebar'>");
    sb.Append("<div class='brand'>CMA SDK</div><div class='brand-sub'>.NET Test Report</div>");
    sb.Append("<nav class='nav'><a href='#overview' class='nav-item active' data-tab='overview'>Overview</a>");
    sb.Append("<a href='#tests' class='nav-item' data-tab='tests'>Tests <span class='badge'>").Append(results.Count).Append("</span></a>");
    if (failed > 0)
        sb.Append("<a href='#failed' class='nav-item nav-fail' data-tab='failed'>Failed <span class='badge fail'>").Append(failed).Append("</span></a>");
    sb.Append("<a href='#coverage' class='nav-item' data-tab='coverage'>Coverage</a>");
    sb.Append("<a href='#sources' class='nav-item' data-tab='sources'>Sources</a></nav>");
    sb.Append("<div class='sidebar-footer'>").Append(Escape(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"))).Append(" UTC</div></aside>");
    sb.Append("<main class='main'><header class='topbar'><h1>Test run summary</h1>");
    sb.Append("<input type='search' id='testFilter' class='search' placeholder='Filter tests by name…' autocomplete='off'></header>");

    // Overview tab
    sb.Append("<section id='panel-overview' class='panel active'>");
    sb.Append("<div class='cards'>");
    sb.Append("<div class='card'><div class='card-label'>Total</div><div class='card-value'>").Append(results.Count).Append("</div></div>");
    sb.Append("<div class='card card-pass'><div class='card-label'>Passed</div><div class='card-value'>").Append(passed).Append("</div></div>");
    sb.Append("<div class='card card-fail'><div class='card-label'>Failed</div><div class='card-value'>").Append(failed).Append("</div></div>");
    var passRate = results.Count > 0 ? (100.0 * passed / results.Count).ToString("F1") : "0";
    sb.Append("<div class='card'><div class='card-label'>Pass rate</div><div class='card-value'>").Append(passRate).Append("%</div></div>");
    sb.Append("</div>");
    if (failedList.Count > 0)
    {
        sb.Append("<h2 class='section-title'>Failed tests</h2><div class='test-list'>");
        foreach (var r in failedList)
            sb.Append(AppendTestCard(r, true));
        sb.Append("</div>");
    }
    sb.Append("</section>");

    // Failed-only tab
    sb.Append("<section id='panel-failed' class='panel");
    if (failed == 0) sb.Append(" empty-state");
    sb.Append("'>");
    if (failed == 0)
        sb.Append("<p class='empty'>No failed tests.</p>");
    else
    {
        sb.Append("<div class='test-list'>");
        foreach (var r in failedList)
            sb.Append(AppendTestCard(r, true));
        sb.Append("</div>");
    }
    sb.Append("</section>");

    // All tests tab – tree: Test Name, Status Badge, Duration, expandable Test Details
    sb.Append("<section id='panel-tests' class='panel'><div class='test-table-wrap'><table class='test-table' id='testTable'><thead><tr>");
    sb.Append("<th>Test name</th><th>Status</th><th>Duration</th></tr></thead><tbody>");
    foreach (var r in results)
    {
        var badge = StatusBadge(r.Outcome);
        var isPass = badge == "PASSED";
        var isSkip = badge == "SKIPPED";
        var rowClass = isPass ? "row-pass" : (isSkip ? "row-skip" : "row-fail");
        sb.Append("<tr class='test-row ").Append(rowClass).Append("' data-name='").Append(EscapeAttr(r.TestName)).Append("'>");
        sb.Append("<td class='name'><div class='tree-test-case'>");
        sb.Append("<div class='tree-test-name'>").Append(Escape(r.TestName)).Append("</div>");
        sb.Append("<details class='tree-details").Append(!isPass ? " open" : "").Append("'><summary class='tree-details-summary'>Test Details</summary>");
        sb.Append("<div class='tree-details-body'>").Append(AppendTestDetailsTree(r)).Append("</div></details></div></td>");
        sb.Append("<td><span class='pill pill-").Append(badge == "PASSED" ? "pass" : (badge == "SKIPPED" ? "skip" : "fail"))
            .Append("'>").Append(badge).Append("</span></td>");
        sb.Append("<td class='dur'>").Append(Escape(r.Duration)).Append("</td></tr>");
    }
    sb.Append("</tbody></table></div></section>");

    // Coverage tab
    sb.Append("<section id='panel-coverage' class='panel");
    if (coverage.Count == 0)
    {
        sb.Append(" empty-state'><p class='empty'>No Cobertura data.</p></section>");
    }
    else
    {
        sb.Append("'><h2 class='section-title'>Coverage (Cobertura)</h2>");
        sb.Append("<div class='table-scroll'><table class='data-table'><thead><tr><th>File</th><th>Line rate</th><th>Branch rate</th><th>Lines hit</th></tr></thead><tbody>");
        foreach (var row in coverage.OrderByDescending(c => c.TotalLines).Take(500))
        {
            sb.Append("<tr><td class='mono'>").Append(Escape(row.Filename)).Append("</td><td>")
                .Append(Escape(row.LineRate ?? "–")).Append("</td><td>").Append(Escape(row.BranchRate ?? "–"))
                .Append("</td><td>").Append(row.CoveredLines).Append(" / ").Append(row.TotalLines).Append("</td></tr>");
        }
        if (coverage.Count > 500)
            sb.Append("<tr><td colspan='4' class='muted'>… ").Append(coverage.Count - 500).Append(" more files omitted</td></tr>");
        sb.Append("</tbody></table></div></section>");
    }

    // Sources tab
    sb.Append("<section id='panel-sources' class='panel'><h2 class='section-title'>Input sources</h2>");
    sb.Append("<div class='source-block'><h3>TRX</h3><ul class='mono-list'>");
    foreach (var p in trxPaths) sb.Append("<li>").Append(Escape(p)).Append("</li>");
    sb.Append("</ul><h3>Cobertura</h3><ul class='mono-list'>");
    foreach (var p in coberturaPaths) sb.Append("<li>").Append(Escape(p)).Append("</li>");
    sb.Append("</ul></div></section>");

    sb.Append("</main></div><script>").Append(GetScript()).Append("</script></body></html>");
    return sb.ToString();
}

static string AppendTestCard(TestResultRow r, bool open)
{
    var badge = StatusBadge(r.Outcome);
    var cardClass = badge == "PASSED" ? "pass" : (badge == "SKIPPED" ? "skip" : "fail");
    var sb = new StringBuilder();
    sb.Append("<article class='test-card ").Append(cardClass).Append("'><details").Append(open ? " open" : "").Append(">");
    sb.Append("<summary class='test-summary'><span class='tree-test-name'>").Append(Escape(r.TestName)).Append("</span>");
    sb.Append("<span class='pill pill-").Append(badge == "PASSED" ? "pass" : (badge == "SKIPPED" ? "skip" : "fail")).Append("'>").Append(badge).Append("</span>");
    sb.Append("<span class='test-dur'>").Append(Escape(r.Duration)).Append("</span></summary>");
    sb.Append("<div class='test-body'><div class='tree-details-body'>").Append(AppendTestDetailsTree(r)).Append("</div></div></details></article>");
    return sb.ToString();
}

static string AppendTestDetailsTree(TestResultRow r)
{
    var sb = new StringBuilder();
    var e = r.Enrich;
    var isFailed = !r.Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase);

    // 1. ✓ Assertions
    if (e.Assertions?.Count > 0)
    {
        sb.Append("<div class='tree-section'><div class='tree-section-title'>✓ Assertions</div><ul class='tree-list'>");
        var idx = 0;
        foreach (var a in e.Assertions)
        {
            idx++;
            sb.Append("<li class='tree-assertion'><details class='tree-collapse'><summary>Assertion #").Append(idx)
                .Append(" — ").Append(a.Passed ? "✅" : "❌").Append(" ").Append(Escape(a.Name ?? "Assertion")).Append("</summary>");
            sb.Append("<div class='tree-kv'><div class='tree-kv-row'><span class='tree-kv-k'>Status</span><span>").Append(a.Passed ? "✅ Passed" : "❌ Failed").Append("</span></div>");
            sb.Append("<div class='tree-kv-row'><span class='tree-kv-k'>Assertion Name</span><span>").Append(Escape(a.Name ?? "—")).Append("</span></div>");
            sb.Append("<div class='tree-kv-row'><span class='tree-kv-k'>Expected</span><pre class='pre-inline'>").Append(Escape(a.Expected ?? "—")).Append("</pre></div>");
            sb.Append("<div class='tree-kv-row'><span class='tree-kv-k'>Actual</span><pre class='pre-inline'>").Append(Escape(a.Actual ?? "—")).Append("</pre></div>");
            sb.Append("<div class='tree-kv-row'><span class='tree-kv-k'>Assertion Type</span><span>").Append(Escape(a.AssertionType ?? "—")).Append("</span></div>");
            sb.Append("</div></details></li>");
        }
        sb.Append("</ul></div>");
    }

    // 2. 🌐 HTTP Requests
    if (e.HttpRequests?.Count > 0)
    {
        sb.Append("<div class='tree-section'><div class='tree-section-title'>🌐 HTTP Requests</div><ul class='tree-list'>");
        foreach (var h in e.HttpRequests)
        {
            sb.Append("<li class='tree-request'><div class='tree-node'><span class='tree-kv-k'>SDK Method</span><span>").Append(Escape(h.SdkMethod ?? "—")).Append("</span></div>");
            sb.Append("<div class='tree-node'><span class='tree-kv-k'>HTTP Method</span><span>").Append(Escape(h.HttpMethod ?? "—")).Append("</span></div>");
            sb.Append("<div class='tree-node'><span class='tree-kv-k'>Request URL</span><span class='mono'>").Append(Escape(h.RequestUrl ?? "—")).Append("</span></div>");
            if (!string.IsNullOrEmpty(h.QueryParams))
            {
                sb.Append("<details class='tree-collapse'><summary>Query Parameters</summary><pre class='pre pre-inline-block'>").Append(Escape(h.QueryParams)).Append("</pre></details>");
            }
            if (!string.IsNullOrEmpty(h.Headers))
            {
                sb.Append("<details class='tree-collapse'><summary>Request Headers</summary><pre class='pre pre-inline-block'>").Append(Escape(h.Headers)).Append("</pre></details>");
            }
            if (!string.IsNullOrEmpty(h.Body))
            {
                sb.Append("<div class='tree-node'><span class='tree-kv-k'>Request Body</span><pre class='pre pre-inline-block'>").Append(Escape(Truncate(h.Body, 4000))).Append("</pre></div>");
            }
            if (!string.IsNullOrEmpty(h.CurlCommand))
            {
                sb.Append("<div class='tree-node'><span class='tree-kv-k'>cURL Command</span>");
                sb.Append("<div class='curl-wrap'><pre class='pre curl-pre' data-curl='").Append(EscapeAttr(h.CurlCommand)).Append("'>").Append(Escape(Truncate(h.CurlCommand, 2000))).Append("</pre>");
                sb.Append("<button type='button' class='btn-copy' data-copy-target='").Append(EscapeAttr(h.CurlCommand)).Append("' title='Copy cURL'>Copy</button></div></div>");
            }
            sb.Append("</li>");
        }
        sb.Append("</ul></div>");
    }

    // 3. 📥 HTTP Responses
    if (e.HttpResponses?.Count > 0)
    {
        sb.Append("<div class='tree-section'><div class='tree-section-title'>📥 HTTP Responses</div><ul class='tree-list'>");
        foreach (var h in e.HttpResponses)
        {
            sb.Append("<li class='tree-response'><div class='tree-node'><span class='tree-kv-k'>Status Line</span><span>");
            sb.Append(h.StatusCode ?? 0).Append(" ").Append(Escape(h.StatusText ?? "")).Append("</span></div>");
            if (h.ResponseTimeMs.HasValue)
                sb.Append("<div class='tree-node'><span class='tree-kv-k'>Response Time</span><span>").Append(h.ResponseTimeMs).Append(" ms</span></div>");
            if (!string.IsNullOrEmpty(h.Headers))
                sb.Append("<details class='tree-collapse'><summary>Response Headers</summary><pre class='pre pre-inline-block'>").Append(Escape(h.Headers)).Append("</pre></details>");
            if (!string.IsNullOrEmpty(h.Body))
                sb.Append("<div class='tree-node'><span class='tree-kv-k'>Response Body</span><pre class='pre pre-inline-block'>").Append(Escape(Truncate(h.Body, 4000))).Append("</pre></div>");
            if (!string.IsNullOrEmpty(h.PayloadSize))
                sb.Append("<div class='tree-node'><span class='tree-kv-k'>Payload Size</span><span>").Append(Escape(h.PayloadSize)).Append("</span></div>");
            sb.Append("</li>");
        }
        sb.Append("</ul></div>");
    }

    // 4. ⚠️ Error Details (Only If Failed)
    if (isFailed && (!string.IsNullOrEmpty(r.Message) || !string.IsNullOrEmpty(r.StackTrace) || e.ErrorDetails != null))
    {
        sb.Append("<div class='tree-section tree-section-error'><div class='tree-section-title'>⚠️ Error Details</div><ul class='tree-list'>");
        var err = e.ErrorDetails;
        if (!string.IsNullOrEmpty(r.Message))
            sb.Append("<li class='tree-node'><span class='tree-kv-k'>Error Message</span><pre class='pre error'>").Append(Escape(r.Message)).Append("</pre></li>");
        if (err != null && !string.IsNullOrEmpty(err.ExceptionType))
            sb.Append("<li class='tree-node'><span class='tree-kv-k'>Exception Type</span><span>").Append(Escape(err.ExceptionType)).Append("</span></li>");
        if (!string.IsNullOrEmpty(r.StackTrace))
            sb.Append("<li class='tree-node'><span class='tree-kv-k'>Stack Trace</span><pre class='pre stack'>").Append(Escape(r.StackTrace)).Append("</pre></li>");
        if (err != null && !string.IsNullOrEmpty(err.FailedAssertionReference))
            sb.Append("<li class='tree-node'><span class='tree-kv-k'>Failed Assertion Reference</span><span>").Append(Escape(err.FailedAssertionReference)).Append("</span></li>");
        if (err != null && err.RetryCount.HasValue)
            sb.Append("<li class='tree-node'><span class='tree-kv-k'>Retry Count</span><span>").Append(err.RetryCount).Append("</span></li>");
        sb.Append("</ul></div>");
    }

    // 5. ℹ️ Test Context
    if (e.Context != null)
    {
        var ctx = e.Context;
        var hasContext = (ctx.ContextKeys?.Count > 0) || !string.IsNullOrEmpty(ctx.Environment) || !string.IsNullOrEmpty(ctx.Locale)
            || !string.IsNullOrEmpty(ctx.SdkVersion) || !string.IsNullOrEmpty(ctx.BuildNumber) || !string.IsNullOrEmpty(ctx.CommitSha) || !string.IsNullOrEmpty(ctx.TestDataSource);
        if (hasContext)
        {
            sb.Append("<div class='tree-section'><div class='tree-section-title'>ℹ️ Test Context</div><ul class='tree-list'>");
            if (ctx.ContextKeys != null)
                foreach (var kv in ctx.ContextKeys)
                    if (!string.IsNullOrEmpty(kv.Value))
                        sb.Append("<li class='tree-node'><span class='tree-kv-k'>").Append(Escape(kv.Key)).Append("</span><span>").Append(Escape(kv.Value)).Append("</span></li>");
            void CtxRow(string k, string? v) { if (!string.IsNullOrEmpty(v)) sb.Append("<li class='tree-node'><span class='tree-kv-k'>").Append(Escape(k)).Append("</span><span>").Append(Escape(v)).Append("</span></li>"); }
            CtxRow("Environment", ctx.Environment);
            CtxRow("Locale", ctx.Locale);
            CtxRow("SDK Version", ctx.SdkVersion);
            CtxRow("Build Number", ctx.BuildNumber);
            CtxRow("Commit SHA", ctx.CommitSha);
            CtxRow("Test Data Source", ctx.TestDataSource);
            sb.Append("</ul></div>");
        }
    }

    if (!string.IsNullOrEmpty(r.StdOut) && (r.StdOut.IndexOf(StartMarker, StringComparison.Ordinal) < 0 || r.StdOut.IndexOf(EndMarker, StringComparison.Ordinal) < 0))
        sb.Append("<div class='tree-section'><div class='tree-section-title'>StdOut (raw)</div><pre class='pre stdout'>").Append(Escape(Truncate(r.StdOut, 8000))).Append("</pre></div>");
    return sb.ToString();
}

/// <summary>Maps TRX outcome to tree status: PASSED, FAILED, or SKIPPED.</summary>
static string StatusBadge(string outcome)
{
    if (string.IsNullOrEmpty(outcome)) return "SKIPPED";
    if (outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase)) return "PASSED";
    if (outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase)) return "FAILED";
    return "SKIPPED"; // NotExecuted, Skipped, etc.
}

static string EscapeAttr(string? s)
{
    if (string.IsNullOrEmpty(s)) return "";
    return Escape(s).Replace("'", "&#39;");
}

static string Truncate(string s, int max)
{
    if (s.Length <= max) return s;
    return s.Substring(0, max) + "\n… (truncated)";
}

static string Escape(string? s)
{
    if (string.IsNullOrEmpty(s)) return "";
    return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
}

static string GetCss() => """
    :root {
      --bg: #0f1419;
      --surface: #1a2332;
      --surface2: #243044;
      --border: #2d3a4f;
      --text: #e6edf3;
      --text-muted: #8b9cb3;
      --accent: #3b82f6;
      --pass: #22c55e;
      --pass-bg: rgba(34,197,94,.12);
      --fail: #ef4444;
      --fail-bg: rgba(239,68,68,.12);
      --radius: 10px;
      --font: 'Segoe UI', system-ui, -apple-system, sans-serif;
    }
    * { box-sizing: border-box; }
    body { margin: 0; font-family: var(--font); background: var(--bg); color: var(--text); font-size: 14px; line-height: 1.5; }
    .layout { display: flex; min-height: 100vh; }
    .sidebar {
      width: 240px; flex-shrink: 0; background: var(--surface); border-right: 1px solid var(--border);
      display: flex; flex-direction: column; padding: 1.25rem 0;
    }
    .brand { font-weight: 700; font-size: 1.1rem; padding: 0 1.25rem; color: var(--text); }
    .brand-sub { font-size: .75rem; color: var(--text-muted); padding: 0 1.25rem .75rem; text-transform: uppercase; letter-spacing: .04em; }
    .nav { display: flex; flex-direction: column; gap: 2px; padding: 1rem 0; flex: 1; }
    .nav-item {
      display: flex; align-items: center; justify-content: space-between; padding: .6rem 1.25rem;
      color: var(--text-muted); text-decoration: none; font-size: .9rem; border-left: 3px solid transparent;
    }
    .nav-item:hover { background: var(--surface2); color: var(--text); }
    .nav-item.active { background: var(--surface2); color: var(--accent); border-left-color: var(--accent); }
    .nav-fail.active { color: var(--fail); border-left-color: var(--fail); }
    .badge { background: var(--surface2); padding: .15rem .5rem; border-radius: 999px; font-size: .75rem; }
    .badge.fail { background: var(--fail-bg); color: var(--fail); }
    .sidebar-footer { padding: 1rem 1.25rem; font-size: .75rem; color: var(--text-muted); border-top: 1px solid var(--border); }
    .main { flex: 1; overflow: auto; padding: 0 1.5rem 2rem; }
    .topbar {
      position: sticky; top: 0; z-index: 10; background: var(--bg); padding: 1.25rem 0 1rem;
      display: flex; flex-wrap: wrap; align-items: center; gap: 1rem; border-bottom: 1px solid var(--border);
    }
    .topbar h1 { margin: 0; font-size: 1.35rem; font-weight: 600; }
    .search {
      flex: 1; min-width: 200px; max-width: 420px; padding: .5rem .85rem; border-radius: var(--radius);
      border: 1px solid var(--border); background: var(--surface); color: var(--text); font-size: .9rem;
    }
    .search::placeholder { color: var(--text-muted); }
    .panel { display: none; padding-top: 1.5rem; }
    .panel.active { display: block; }
    .panel.empty-state { padding-top: 3rem; text-align: center; }
    .empty { color: var(--text-muted); }
    .cards { display: grid; grid-template-columns: repeat(auto-fill, minmax(140px, 1fr)); gap: 1rem; margin-bottom: 2rem; }
    .card {
      background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius);
      padding: 1rem 1.15rem;
    }
    .card-pass { border-color: rgba(34,197,94,.35); background: var(--pass-bg); }
    .card-fail { border-color: rgba(239,68,68,.35); background: var(--fail-bg); }
    .card-label { font-size: .75rem; color: var(--text-muted); text-transform: uppercase; letter-spacing: .05em; }
    .card-value { font-size: 1.75rem; font-weight: 700; margin-top: .25rem; }
    .card-pass .card-value { color: var(--pass); }
    .card-fail .card-value { color: var(--fail); }
    .section-title { font-size: 1rem; font-weight: 600; margin: 0 0 1rem; color: var(--text-muted); }
    .test-list { display: flex; flex-direction: column; gap: .75rem; }
    .test-card { background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius); overflow: hidden; }
    .test-card.fail { border-left: 4px solid var(--fail); }
    .test-card.pass { border-left: 4px solid var(--pass); }
    .test-card.skip { border-left: 4px solid #94a3b8; }
    .test-summary {
      display: flex; align-items: center; gap: .75rem; padding: .85rem 1rem; cursor: pointer; list-style: none;
      font-weight: 500;
    }
    .test-summary::-webkit-details-marker { display: none; }
    .test-title { flex: 1; word-break: break-all; font-size: .9rem; }
    .test-dur { font-size: .8rem; color: var(--text-muted); font-family: ui-monospace, monospace; }
    .test-body { padding: 0 1rem 1rem; border-top: 1px solid var(--border); }
    .pill { font-size: .65rem; font-weight: 700; padding: .2rem .5rem; border-radius: 4px; letter-spacing: .03em; }
    .pill-pass { background: var(--pass-bg); color: var(--pass); }
    .pill-fail { background: var(--fail-bg); color: var(--fail); }
    .pill-skip { background: rgba(148,163,184,.2); color: #94a3b8; }
    .meta-row { display: flex; flex-wrap: wrap; gap: .5rem 1.5rem; padding: .75rem 0; font-size: .85rem; }
    .meta-row .label { color: var(--text-muted); }
    .block { margin-top: 1rem; }
    .block-title { font-size: .75rem; text-transform: uppercase; color: var(--text-muted); margin-bottom: .5rem; letter-spacing: .04em; }
    .pre {
      margin: 0; padding: .85rem 1rem; border-radius: 8px; overflow: auto; max-height: 22rem;
      font-size: .8rem; line-height: 1.45; background: #0a0e14; border: 1px solid var(--border); white-space: pre-wrap; word-break: break-word;
    }
    .pre.error { background: rgba(239,68,68,.08); border-color: rgba(239,68,68,.25); }
    .pre.stack { max-height: 18rem; }
    .pre.http { max-height: 16rem; }
    .pre.stdout { max-height: 14rem; }
    .pre-inline { margin: 0; white-space: pre-wrap; word-break: break-word; font-size: .8rem; }
    .kv-table { width: 100%; border-collapse: collapse; font-size: .85rem; }
    .kv-table th { text-align: left; color: var(--text-muted); font-weight: 500; padding: .4rem .75rem .4rem 0; width: 140px; vertical-align: top; }
    .kv-table td { padding: .4rem 0; word-break: break-all; }
    .data-table { width: 100%; border-collapse: collapse; font-size: .85rem; }
    .data-table th, .data-table td { border: 1px solid var(--border); padding: .5rem .65rem; text-align: left; }
    .data-table th { background: var(--surface2); color: var(--text-muted); font-weight: 600; }
    .data-table.compact td pre { max-height: 8rem; }
    .table-scroll { overflow: auto; border: 1px solid var(--border); border-radius: var(--radius); }
    .table-scroll .data-table { margin: 0; }
    .table-scroll .data-table th { position: sticky; top: 0; }
    .mono { font-family: ui-monospace, monospace; font-size: .8rem; word-break: break-all; }
    .muted { color: var(--text-muted); }
    .test-table-wrap { overflow: auto; border: 1px solid var(--border); border-radius: var(--radius); max-height: 70vh; }
    .test-table { width: 100%; border-collapse: collapse; font-size: .85rem; }
    .test-table th { position: sticky; top: 0; background: var(--surface2); z-index: 1; padding: .65rem .75rem; text-align: left; border-bottom: 1px solid var(--border); }
    .test-table td { padding: .5rem .75rem; border-bottom: 1px solid var(--border); vertical-align: top; }
    .test-table tr.hidden { display: none; }
    .test-table .name { max-width: 0; }
    .test-table .dur { white-space: nowrap; color: var(--text-muted); font-family: ui-monospace, monospace; font-size: .8rem; }
    .test-table .row-skip { border-left-color: #94a3b8; }
    .tree-test-case { display: flex; flex-direction: column; gap: .35rem; }
    .tree-test-name { font-weight: 600; word-break: break-word; }
    .tree-details { margin-top: .25rem; }
    .tree-details-summary { cursor: pointer; font-size: .8rem; color: var(--text-muted); list-style: none; }
    .tree-details-summary::-webkit-details-marker { display: none; }
    .tree-details-body { margin-top: .5rem; padding-top: .5rem; border-top: 1px solid var(--border); }
    .tree-section { margin-bottom: 1rem; }
    .tree-section:last-child { margin-bottom: 0; }
    .tree-section-title { font-size: .8rem; font-weight: 600; color: var(--text-muted); margin-bottom: .5rem; }
    .tree-section-error .tree-section-title { color: var(--fail); }
    .tree-list { list-style: none; margin: 0; padding: 0; }
    .tree-list > li { margin-bottom: .5rem; padding-left: .5rem; border-left: 2px solid var(--border); }
    .tree-node { margin-bottom: .35rem; font-size: .85rem; }
    .tree-node .tree-kv-k { color: var(--text-muted); margin-right: .5rem; }
    .tree-kv { margin-top: .35rem; }
    .tree-kv-row { margin-bottom: .25rem; font-size: .85rem; }
    .tree-kv-row .tree-kv-k { display: inline-block; min-width: 7rem; color: var(--text-muted); }
    .tree-collapse { margin-top: .25rem; }
    .tree-collapse summary { cursor: pointer; font-size: .85rem; color: var(--text-muted); }
    .tree-collapse summary::-webkit-details-marker { display: none; }
    .pre-inline-block { display: block; margin: .25rem 0 0; }
    .curl-wrap { position: relative; }
    .curl-pre { margin-right: 3rem; }
    .btn-copy {
      position: absolute; top: .5rem; right: .5rem; padding: .25rem .5rem; font-size: .7rem;
      background: var(--surface2); border: 1px solid var(--border); border-radius: 4px; color: var(--text); cursor: pointer;
    }
    .btn-copy:hover { background: var(--accent); border-color: var(--accent); }
    .source-block { background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius); padding: 1rem 1.25rem; }
    .source-block h3 { margin: 1rem 0 .5rem; font-size: .85rem; color: var(--text-muted); }
    .source-block h3:first-child { margin-top: 0; }
    .mono-list { margin: 0; padding-left: 1.25rem; font-family: ui-monospace, monospace; font-size: .8rem; word-break: break-all; }
    """;

static string GetScript() => """
    (function(){
      var tabs = document.querySelectorAll('.nav-item[data-tab]');
      var panels = document.querySelectorAll('.panel');
      function show(tab) {
        panels.forEach(function(p){ p.classList.remove('active'); });
        tabs.forEach(function(t){ t.classList.remove('active'); });
        var panel = document.getElementById('panel-' + tab);
        if (panel) panel.classList.add('active');
        var nav = document.querySelector('.nav-item[data-tab="' + tab + '"]');
        if (nav) nav.classList.add('active');
        if (history.replaceState) history.replaceState(null, '', '#' + tab);
      }
      tabs.forEach(function(el){
        el.addEventListener('click', function(e){
          e.preventDefault();
          show(el.getAttribute('data-tab'));
        });
      });
      var hash = (location.hash || '#overview').slice(1);
      if (document.getElementById('panel-' + hash)) show(hash); else show('overview');
      var filter = document.getElementById('testFilter');
      if (filter) {
        filter.addEventListener('input', function(){
          var q = (filter.value || '').toLowerCase();
          document.querySelectorAll('.test-row').forEach(function(row){
            var name = (row.getAttribute('data-name') || '').toLowerCase();
            row.classList.toggle('hidden', q && name.indexOf(q) < 0);
          });
        });
      }
      document.querySelectorAll('.btn-copy').forEach(function(btn){
        btn.addEventListener('click', function(){
          var text = btn.getAttribute('data-copy-target') || '';
          if (!text && btn.previousElementSibling && btn.previousElementSibling.getAttribute('data-curl'))
            text = btn.previousElementSibling.getAttribute('data-curl');
          if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(text).then(function(){ btn.textContent = 'Copied!'; setTimeout(function(){ btn.textContent = 'Copy'; }, 1500); });
          } else {
            var ta = document.createElement('textarea'); ta.value = text; document.body.appendChild(ta); ta.select(); document.execCommand('copy'); document.body.removeChild(ta);
            btn.textContent = 'Copied!'; setTimeout(function(){ btn.textContent = 'Copy'; }, 1500);
          }
        });
      });
    })();
    """;

record TestResultRow(string TrxPath, string TestName, string Outcome, string Duration, string StdOut,
    string? Message, string? StackTrace, EnrichPayload Enrich);

record CoverageFileRow(string Filename, string? LineRate, string? BranchRate, int TotalLines, int CoveredLines);

class EnrichPayload
{
    public List<AssertionDto>? Assertions { get; set; }
    public List<HttpRequestDto>? HttpRequests { get; set; }
    public List<HttpResponseDto>? HttpResponses { get; set; }
    public ContextDto? Context { get; set; }
    public ErrorDetailsDto? ErrorDetails { get; set; }
}

class AssertionDto
{
    public bool Passed { get; set; }
    public string? Name { get; set; }
    public string? Expected { get; set; }
    public string? Actual { get; set; }
    [JsonPropertyName("assertionType")]
    public string? AssertionType { get; set; }
}

class HttpRequestDto
{
    public string? SdkMethod { get; set; }
    public string? HttpMethod { get; set; }
    public string? RequestUrl { get; set; }
    public string? QueryParams { get; set; }
    public string? Headers { get; set; }
    public string? Body { get; set; }
    public string? CurlCommand { get; set; }
}

class HttpResponseDto
{
    public int? StatusCode { get; set; }
    public string? StatusText { get; set; }
    public int? ResponseTimeMs { get; set; }
    public string? Headers { get; set; }
    public string? Body { get; set; }
    public string? PayloadSize { get; set; }
}

class ContextDto
{
    public string? Environment { get; set; }
    public string? SdkVersion { get; set; }
    public string? BuildNumber { get; set; }
    public string? CommitSha { get; set; }
    public string? TestDataSource { get; set; }
    public string? Locale { get; set; }
    [JsonPropertyName("contextKeys")]
    public Dictionary<string, string?>? ContextKeys { get; set; }
}

class ErrorDetailsDto
{
    public string? ErrorMessage { get; set; }
    public string? ExceptionType { get; set; }
    public string? StackTrace { get; set; }
    public string? FailedAssertionReference { get; set; }
    public int? RetryCount { get; set; }
}
