using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace EnhancedTestReport;

static class Program
{
    static int Main(string[] args)
    {
        var trxPaths = new List<string>();
        var coberturaPaths = new List<string>();
        string? outputPath = null;
        string? trxDir = null;
        string? coberturaDir = null;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--trx" when i + 1 < args.Length:
                    trxPaths.Add(args[++i]);
                    break;
                case "--trx-dir" when i + 1 < args.Length:
                    trxDir = args[++i];
                    break;
                case "--cobertura" when i + 1 < args.Length:
                    coberturaPaths.Add(args[++i]);
                    break;
                case "--cobertura-dir" when i + 1 < args.Length:
                    coberturaDir = args[++i];
                    break;
                case "--output" when i + 1 < args.Length:
                    outputPath = args[++i];
                    break;
                case "--help":
                case "-h":
                    PrintUsage();
                    return 0;
            }
        }

        if (trxDir != null)
            trxPaths.AddRange(GlobFiles(trxDir, "*.trx"));
        if (coberturaDir != null)
            coberturaPaths.AddRange(GlobFiles(coberturaDir, "coverage.cobertura.xml"));

        trxPaths = trxPaths.Where(File.Exists).Distinct().ToList();
        coberturaPaths = coberturaPaths.Where(File.Exists).Distinct().ToList();

        if (trxPaths.Count == 0 && coberturaPaths.Count == 0)
        {
            Console.Error.WriteLine("No TRX or Cobertura files found. Use --trx, --trx-dir, --cobertura, or --cobertura-dir.");
            return 1;
        }

        outputPath ??= "EnhancedTestReport.html";

        var testData = ParseTrxFiles(trxPaths);
        EnrichResults(testData);
        var coverageData = ParseCoberturaFiles(coberturaPaths);
        var html = GenerateHtml(testData, coverageData);
        var dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(outputPath, html);
        Console.WriteLine($"Report written to {Path.GetFullPath(outputPath)}");
        return 0;
    }

    static void PrintUsage()
    {
        Console.WriteLine(@"EnhancedTestReport - Generate HTML test report from TRX and Cobertura.

Usage:
  EnhancedTestReport [options]

Options:
  --trx <path>         Add a TRX file (can be repeated).
  --trx-dir <dir>      Glob *.trx in directory (recursive).
  --cobertura <path>   Add a Cobertura XML file (can be repeated).
  --cobertura-dir <dir> Glob coverage.cobertura.xml in directory (recursive).
  --output <path>      Output HTML path (default: EnhancedTestReport.html).
  --help, -h           Show this help.
");
    }

    static List<string> GlobFiles(string dir, string pattern)
    {
        if (!Directory.Exists(dir)) return new List<string>();
        var list = new List<string>();
        foreach (var f in Directory.GetFiles(dir, pattern, SearchOption.AllDirectories))
            list.Add(Path.GetFullPath(f));
        return list;
    }

    static TestReportData ParseTrxFiles(List<string> paths)
    {
        var results = new List<UnitTestResult>();
        int total = 0, passed = 0, failed = 0, skipped = 0;
        var totalDuration = TimeSpan.Zero;
        var byAssembly = new Dictionary<string, List<UnitTestResult>>(StringComparer.OrdinalIgnoreCase);
        var byClass = new Dictionary<string, List<UnitTestResult>>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in paths)
        {
            try
            {
                var doc = XDocument.Load(path);
                var ns = doc.Root?.Name.Namespace ?? XNamespace.None;

                // Build testId → className (short name) from TestDefinitions
                var testIdToClassName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var ut in doc.Descendants().Where(e => e.Name.LocalName == "UnitTest"))
                {
                    var id = (string?)ut.Attribute("id");
                    var testMethod = ut.Descendants().FirstOrDefault(e => e.Name.LocalName == "TestMethod");
                    var fullClass = (string?)testMethod?.Attribute("className");
                    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(fullClass))
                    {
                        var shortName = fullClass.Split('.').LastOrDefault() ?? fullClass;
                        testIdToClassName[id] = shortName;
                    }
                }

                foreach (var er in doc.Descendants().Where(e => e.Name.LocalName == "UnitTestResult"))
                {
                    var outcome = (string?)er.Attribute("outcome") ?? "";
                    var testId = (string?)er.Attribute("testId") ?? "";
                    var testName = (string?)er.Attribute("testName") ?? testId;
                    var durationStr = (string?)er.Attribute("duration");
                    TimeSpan duration = TimeSpan.Zero;
                    if (!string.IsNullOrEmpty(durationStr))
                        TimeSpan.TryParse(durationStr, CultureInfo.InvariantCulture, out duration);

                    var output = er.Element(ns + "Output");
                    var err = output?.Element(ns + "ErrorInfo");
                    var message = (string?)err?.Element(ns + "Message")?.Value ?? "";
                    var stack = (string?)err?.Element(ns + "StackTrace")?.Value ?? "";
                    var stdOut = (string?)output?.Element(ns + "StdOut")?.Value ?? "";
                    var stdErr = (string?)output?.Element(ns + "StdErr")?.Value ?? "";
                    var debugTrace = (string?)output?.Element(ns + "DebugTrace")?.Value ?? "";

                    var assembly = Path.GetFileNameWithoutExtension(path).Replace("Report-", "").Replace(".trx", "");
                    if (string.IsNullOrEmpty(assembly)) assembly = "Tests";

                    var className = testIdToClassName.TryGetValue(testId, out var cn) ? cn : "";

                    var r = new UnitTestResult
                    {
                        TestName = testName,
                        ClassName = className,
                        Outcome = outcome,
                        Duration = duration,
                        Message = message,
                        StackTrace = stack,
                        StdOut = stdOut?.Trim() ?? "",
                        StdErr = stdErr?.Trim() ?? "",
                        DebugTrace = debugTrace?.Trim() ?? "",
                        Assembly = assembly
                    };
                    results.Add(r);
                    total++;
                    totalDuration += duration;
                    if (outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase)) passed++;
                    else if (outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase)) failed++;
                    else skipped++;

                    if (!byAssembly.TryGetValue(assembly, out var list))
                    {
                        list = new List<UnitTestResult>();
                        byAssembly[assembly] = list;
                    }
                    list.Add(r);

                    var classKey = string.IsNullOrEmpty(className) ? "(Unknown)" : className;
                    if (!byClass.TryGetValue(classKey, out var classList))
                    {
                        classList = new List<UnitTestResult>();
                        byClass[classKey] = classList;
                    }
                    classList.Add(r);
                }

                var counters = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Counters");
                if (counters != null && total == 0)
                {
                    total = GetIntAttr(counters, "total", 0);
                    passed = GetIntAttr(counters, "passed", 0);
                    failed = GetIntAttr(counters, "failed", 0);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading {path}: {ex.Message}");
            }
        }

        if (results.Count > 0 && total == 0)
        {
            total = results.Count;
            passed = results.Count(r => r.Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase));
            failed = results.Count(r => r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase));
            skipped = total - passed - failed;
        }

        return new TestReportData
        {
            Results = results,
            ByAssembly = byAssembly,
            ByClass = byClass,
            Total = total,
            Passed = passed,
            Failed = failed,
            Skipped = skipped,
            TotalDuration = totalDuration
        };
    }

    static int GetIntAttr(XElement el, string localName, int defaultValue)
    {
        var a = el.Attributes().FirstOrDefault(x => x.Name.LocalName == localName);
        return a != null && int.TryParse(a.Value, out var v) ? v : defaultValue;
    }

    static CoverageReportData ParseCoberturaFiles(List<string> paths)
    {
        var files = new List<CoverageFileRow>();
        double sumStmts = 0, sumBranch = 0, sumFuncs = 0, sumLines = 0;
        int countStmts = 0, countBranch = 0, countFuncs = 0, countLines = 0;

        foreach (var path in paths)
        {
            try
            {
                var doc = XDocument.Load(path);
                foreach (var package in doc.Descendants().Where(e => e.Name.LocalName == "package"))
                {
                    var pkgName = (string?)package.Attribute("name") ?? "";
                    foreach (var classEl in package.Descendants().Where(e => e.Name.LocalName == "class"))
                    {
                        var fileName = (string?)classEl.Attribute("filename") ?? (string?)classEl.Attribute("name") ?? "";
                        var lineRate = GetDoubleAttr(classEl, "line-rate", 0);
                        var branchRate = GetDoubleAttr(classEl, "branch-rate", 0);

                        var methods = classEl.Descendants().Where(x => x.Name.LocalName == "method").ToList();
                        var methodCount = methods.Count;
                        var methodCovered = methods.Count(m => GetDoubleAttr(m, "line-rate", 0) > 0);
                        var funcRate = methodCount > 0 ? (double)methodCovered / methodCount : 1.0;

                        var lines = classEl.Descendants().Where(x => x.Name.LocalName == "line").ToList();
                        var uncovered = new List<int>();
                        foreach (var line in lines)
                        {
                            var num = GetIntAttr(line, "number", 0);
                            var hits = GetIntAttr(line, "hits", 0);
                            if (num > 0 && hits == 0)
                                uncovered.Add(num);
                        }
                        uncovered.Sort();
                        var uncoveredStr = FormatUncoveredLines(uncovered);

                        var displayName = string.IsNullOrEmpty(fileName) ? (string?)classEl.Attribute("name") ?? "?" : fileName;
                        if (!string.IsNullOrEmpty(pkgName) && !displayName.StartsWith(pkgName, StringComparison.Ordinal))
                            displayName = pkgName + "/" + displayName.TrimStart('/');

                        var row = new CoverageFileRow
                        {
                            File = displayName,
                            PctStmts = lineRate * 100,
                            PctBranch = branchRate * 100,
                            PctFuncs = funcRate * 100,
                            PctLines = lineRate * 100,
                            UncoveredLines = uncoveredStr
                        };
                        files.Add(row);

                        sumStmts += row.PctStmts; countStmts++;
                        sumBranch += row.PctBranch; countBranch++;
                        sumFuncs += row.PctFuncs; countFuncs++;
                        sumLines += row.PctLines; countLines++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading Cobertura {path}: {ex.Message}");
            }
        }

        return new CoverageReportData
        {
            Files = files,
            SummaryStmts = countStmts > 0 ? sumStmts / countStmts : 0,
            SummaryBranch = countBranch > 0 ? sumBranch / countBranch : 0,
            SummaryFuncs = countFuncs > 0 ? sumFuncs / countFuncs : 0,
            SummaryLines = countLines > 0 ? sumLines / countLines : 0
        };
    }

    static double GetDoubleAttr(XElement el, string localName, double defaultValue)
    {
        var a = el.Attributes().FirstOrDefault(x => x.Name.LocalName == localName);
        return a != null && double.TryParse(a.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : defaultValue;
    }

    static string FormatUncoveredLines(List<int> lines)
    {
        if (lines.Count == 0) return "";
        var ranges = new List<string>();
        int start = lines[0], prev = lines[0];
        for (var i = 1; i < lines.Count; i++)
        {
            if (lines[i] == prev + 1) { prev = lines[i]; continue; }
            ranges.Add(start == prev ? start.ToString() : $"{start}-{prev}");
            start = lines[i];
            prev = lines[i];
        }
        ranges.Add(start == prev ? start.ToString() : $"{start}-{prev}");
        return string.Join(", ", ranges);
    }

    static void EnrichResults(TestReportData testData)
    {
        foreach (var r in testData.Results)
        {
            EnrichOne(r);
        }
    }

    static void EnrichOne(UnitTestResult r)
    {
        // ── Structured parse: look for ##TEST_REPORT_START## / ##TEST_REPORT_END## in StdOut ──
        var stdOut = r.StdOut ?? "";
        const string startMarker = "##TEST_REPORT_START##";
        const string endMarker   = "##TEST_REPORT_END##";
        var sIdx = stdOut.IndexOf(startMarker, StringComparison.Ordinal);
        var eIdx = stdOut.IndexOf(endMarker,   StringComparison.Ordinal);
        if (sIdx >= 0 && eIdx > sIdx)
        {
            var jsonLine = stdOut.Substring(sIdx + startMarker.Length, eIdx - sIdx - startMarker.Length).Trim();
            try
            {
                var block = System.Text.Json.JsonSerializer.Deserialize<TestBlockPayload>(jsonLine,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (block != null)
                {
                    if (block.Assertions != null)
                        foreach (var a in block.Assertions)
                            r.Assertions.Add(new AssertionRecord
                            {
                                Passed        = a.Passed,
                                Name          = a.Name          ?? "",
                                Expected      = a.Expected      ?? "",
                                Actual        = a.Actual        ?? "",
                                AssertionType = a.AssertionType ?? "Assert"
                            });

                    if (block.HttpRequests != null)
                        foreach (var req in block.HttpRequests)
                        {
                            var requestUrl = NormalizeRequestUrl(req.RequestUrl ?? "");
                            var curlCommand = req.CurlCommand ?? "";
                            if (string.IsNullOrEmpty(curlCommand) && !string.IsNullOrEmpty(requestUrl))
                                curlCommand = BuildCurlCommand(req.HttpMethod ?? "GET", requestUrl, req.QueryParams ?? new(), req.Headers ?? new(), req.Body ?? "");
                            r.HttpRequests.Add(new HttpRequestRecord
                            {
                                SdkMethod   = req.SdkMethod  ?? "",
                                HttpMethod  = req.HttpMethod  ?? "GET",
                                RequestUrl  = requestUrl,
                                QueryParams = req.QueryParams ?? new(),
                                Headers     = req.Headers     ?? new(),
                                Body        = req.Body        ?? "",
                                CurlCommand = curlCommand
                            });
                        }

                    if (block.HttpResponses != null)
                        foreach (var resp in block.HttpResponses)
                            r.HttpResponses.Add(new HttpResponseRecord
                            {
                                StatusCode     = resp.StatusCode,
                                StatusText     = resp.StatusText     ?? "",
                                ResponseTimeMs = resp.ResponseTimeMs ?? "",
                                Headers        = resp.Headers        ?? new(),
                                Body           = resp.Body           ?? "",
                                PayloadSize    = resp.PayloadSize    ?? ""
                            });

                    if (block.Context != null)
                        r.Context = new TestContextRecord
                        {
                            Environment    = block.Context.Environment    ?? "",
                            SdkVersion     = block.Context.SdkVersion     ?? "",
                            BuildNumber    = block.Context.BuildNumber    ?? "",
                            CommitSha      = block.Context.CommitSha      ?? "",
                            TestDataSource = block.Context.TestDataSource ?? "",
                            Locale         = block.Context.Locale         ?? ""
                        };

                    // Derive exception type from TRX message even for structured results
                    var isFailed2 = r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase);
                    if (isFailed2 && string.IsNullOrEmpty(r.ExceptionType) && !string.IsNullOrEmpty(r.Message))
                        r.ExceptionType = ExtractExceptionType(r.Message);

                    return; // structured data wins — skip heuristics
                }
            }
            catch { /* fall through to heuristic parsing */ }
        }

        // ── Heuristic fallback (for tests not yet instrumented) ──────────────
        var isFailed = r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase);

        if (r.Assertions.Count == 0 && isFailed && !string.IsNullOrEmpty(r.Message))
        {
            var assertion = DeriveAssertionFromMessage(r.Message);
            if (assertion != null)
                r.Assertions.Add(assertion);
        }

        if (r.HttpRequests.Count == 0 && !string.IsNullOrEmpty(r.StdOut))
        {
            var requests = ParseRequestsFromStdOut(r.StdOut);
            foreach (var req in requests)
                r.HttpRequests.Add(req);
        }

        if (r.HttpResponses.Count == 0 && r.HttpRequests.Count > 0 && isFailed && !string.IsNullOrEmpty(r.Message))
        {
            var resp = DeriveResponseFromMessage(r.Message, r.StdOut);
            if (resp != null)
                r.HttpResponses.Add(resp);
        }

        if (isFailed && string.IsNullOrEmpty(r.ExceptionType) && !string.IsNullOrEmpty(r.Message))
            r.ExceptionType = ExtractExceptionType(r.Message);

        if (string.IsNullOrEmpty(r.Context.TestDataSource))
        {
            r.Context.SdkVersion = "—";
            r.Context.BuildNumber = "—";
            r.Context.CommitSha = "—";
            r.Context.Environment = "—";
            r.Context.Locale = "—";
            r.Context.TestDataSource = "TRX";
        }
    }

    static AssertionRecord? DeriveAssertionFromMessage(string message)
    {
        var m = message.Trim();
        if (string.IsNullOrEmpty(m)) return null;
        var passed = false;
        string expected = "", actual = "", type = "Fail";
        var expectedMatch = Regex.Match(m, @"Expected:\s*(.+?)(?:\.|\,|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        var actualMatch = Regex.Match(m, @"Actual:\s*(.+?)(?:\.|\,|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (expectedMatch.Success) expected = expectedMatch.Groups[1].Value.Trim();
        if (actualMatch.Success) actual = actualMatch.Groups[1].Value.Trim();
        if (m.StartsWith("Assert.", StringComparison.OrdinalIgnoreCase))
        {
            if (m.Contains("Assert.Fail", StringComparison.OrdinalIgnoreCase)) type = "Fail";
            else if (m.Contains("Assert.AreEqual") || m.Contains("Assert.Equals")) type = "Equals";
            else if (m.Contains("Assert.IsNotNull")) type = "NotNull";
            else if (m.Contains("Assert.IsNull")) type = "IsNull";
            else if (m.Contains("Assert.Contains")) type = "Contains";
            else if (m.Contains("Assert.IsTrue")) type = "IsTrue";
            else if (m.Contains("Assert.IsFalse")) type = "IsFalse";
        }
        return new AssertionRecord
        {
            Passed = passed,
            Name = m.Length > 120 ? m.Substring(0, 117) + "..." : m,
            Expected = expected,
            Actual = actual,
            AssertionType = type
        };
    }

    /// <summary>Normalize request URL: trim trailing sentence period, ensure path has leading slash.</summary>
    static string NormalizeRequestUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return "";
        var u = url.Trim();
        // Trim trailing dot from log lines like "making request /environments."
        if (u.Length > 1 && u[u.Length - 1] == '.' && u.IndexOf(' ') < 0)
            u = u.Substring(0, u.Length - 1);
        // Path-only (no scheme): ensure leading slash for proper cURL
        if (u.Length > 0 && !u.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !u.StartsWith("/"))
            u = "/" + u;
        return u;
    }

    static string BuildCurlCommand(string httpMethod, string requestUrl)
    {
        var url = NormalizeRequestUrl(requestUrl ?? "");
        if (string.IsNullOrEmpty(url)) return "";
        var method = (httpMethod ?? "GET").ToUpperInvariant();
        return $"curl -X {method} \"{url}\"";
    }

    /// <summary>Builds a full cURL command with URL (including query), headers, and body.</summary>
    static string BuildCurlCommand(string httpMethod, string requestUrl, Dictionary<string, string> queryParams, Dictionary<string, string> headers, string body)
    {
        var fullUrl = BuildFullRequestUrl(NormalizeRequestUrl(requestUrl ?? ""), queryParams);
        if (string.IsNullOrEmpty(fullUrl)) return "";
        var method = (httpMethod ?? "GET").ToUpperInvariant();
        var sb = new System.Text.StringBuilder();
        sb.Append("curl -X ").Append(method).Append(" ");
        sb.Append("\"").Append(EscapeCurlUrl(fullUrl)).Append("\"");
        foreach (var h in headers.Where(x => !string.IsNullOrEmpty(x.Key)))
            sb.Append(" -H \"").Append(EscapeCurlHeader(h.Key)).Append(": ").Append(EscapeCurlHeader(h.Value)).Append("\"");
        if (!string.IsNullOrEmpty(body))
            sb.Append(" -d '").Append(EscapeCurlBody(body)).Append("'");
        return sb.ToString();
    }

    static string BuildFullRequestUrl(string requestUrl, Dictionary<string, string> queryParams)
    {
        var baseUrl = requestUrl.Split('?')[0].Trim();
        if (string.IsNullOrEmpty(baseUrl)) return requestUrl;
        var existingQuery = requestUrl.Contains("?") ? requestUrl.Substring(requestUrl.IndexOf('?') + 1) : "";
        var paramList = new List<string>();
        if (!string.IsNullOrEmpty(existingQuery))
            paramList.Add(existingQuery);
        foreach (var kv in queryParams.Where(x => !string.IsNullOrEmpty(x.Key)))
            paramList.Add($"{System.Uri.EscapeDataString(kv.Key)}={System.Uri.EscapeDataString(kv.Value ?? "")}");
        return paramList.Count == 0 ? baseUrl : baseUrl + "?" + string.Join("&", paramList);
    }

    static string EscapeCurlUrl(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    static string EscapeCurlHeader(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    static string EscapeCurlBody(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Replace("'", "'\\''");
    }

    static List<HttpRequestRecord> ParseRequestsFromStdOut(string stdOut)
    {
        var list = new List<HttpRequestRecord>();
        var regex = new Regex(@"making request\s+([^\s\n\r]+)", RegexOptions.IgnoreCase);
        foreach (Match match in regex.Matches(stdOut))
        {
            var path = NormalizeRequestUrl(match.Groups[1].Value);
            if (string.IsNullOrEmpty(path)) continue;
            list.Add(new HttpRequestRecord
            {
                SdkMethod = "—",
                HttpMethod = "GET",
                RequestUrl = path,
                CurlCommand = BuildCurlCommand("GET", path)
            });
        }
        return list;
    }

    static HttpResponseRecord? DeriveResponseFromMessage(string message, string stdOut)
    {
        var codeMatch = Regex.Match(message, @"HTTP\s+(\d+)\s*\(([^)]*)\)", RegexOptions.IgnoreCase);
        if (!codeMatch.Success)
            codeMatch = Regex.Match(message, @"(\d{3})\s+(\w+)", RegexOptions.IgnoreCase);
        int code = 0;
        var text = "";
        if (codeMatch.Success)
        {
            int.TryParse(codeMatch.Groups[1].Value, out code);
            text = codeMatch.Groups.Count > 2 ? codeMatch.Groups[2].Value.Trim() : "";
        }
        return new HttpResponseRecord
        {
            StatusCode = code > 0 ? code : 0,
            StatusText = text,
            Body = message.Length > 500 ? message.Substring(0, 497) + "..." : message
        };
    }

    static string ExtractExceptionType(string message)
    {
        var match = Regex.Match(message, @"([a-zA-Z0-9_.]+Exception)(?:\s*:|\s+was thrown|\.)", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : "—";
    }

    static string GenerateHtml(TestReportData testData, CoverageReportData coverageData)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append(GetReportCss());
        sb.Append("<body><div class=\"container\">");
        sb.Append("<h1>Contentstack Management .NET SDK – Enhanced Test Report</h1>");

        if (testData.Results.Count > 0)
        {
            sb.Append("<h2 id=\"test-summary\" class=\"section-head\">Test results</h2>");
            sb.Append("<div class=\"summary\">");
            sb.Append($"<div class=\"summary-item\"><strong>Total</strong><span>{testData.Total}</span></div>");
            sb.Append($"<div class=\"summary-item passed\"><strong>Passed</strong><span>{testData.Passed}</span></div>");
            sb.Append($"<div class=\"summary-item failed\"><strong>Failed</strong><span>{testData.Failed}</span></div>");
            sb.Append($"<div class=\"summary-item skipped\"><strong>Skipped</strong><span>{testData.Skipped}</span></div>");
            sb.Append($"<div class=\"summary-item\"><strong>Duration</strong><span>{FormatDuration(testData.TotalDuration)}</span></div>");
            sb.Append("</div>");

            if (coverageData.Files.Count > 0)
            {
                sb.Append(@"<div class=""coverage-all-files coverage-at-top"">
  <span class=""coverage-all-label"">All files</span>
  <div class=""coverage-all-metrics"">
    <div class=""coverage-metric""><span class=""coverage-metric-label"">Statements</span><span class=""coverage-metric-value"">");
                sb.Append($"{coverageData.SummaryStmts:F1}%</span></div>");
                sb.Append(@"<div class=""coverage-metric""><span class=""coverage-metric-label"">Branches</span><span class=""coverage-metric-value"">");
                sb.Append($"{coverageData.SummaryBranch:F1}%</span></div>");
                sb.Append(@"<div class=""coverage-metric""><span class=""coverage-metric-label"">Functions</span><span class=""coverage-metric-value"">");
                sb.Append($"{coverageData.SummaryFuncs:F1}%</span></div>");
                sb.Append(@"<div class=""coverage-metric""><span class=""coverage-metric-label"">Lines</span><span class=""coverage-metric-value"">");
                sb.Append($"{coverageData.SummaryLines:F1}%</span></div>");
                sb.Append(@"</div>
</div>");
            }

            var useByClass = testData.ByClass.Count > 0;
            var groupDict = useByClass ? testData.ByClass : testData.ByAssembly;
            foreach (var kv in groupDict.OrderBy(x => x.Key))
            {
                var sectionTitle = useByClass ? kv.Key + ".cs" : kv.Key;
                var list = kv.Value;
                var failedCount = list.Count(r => r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase));
                var badgeClass = failedCount > 0 ? "failed" : "passed";
                var groupId = Regex.Replace(EscapeAttr(kv.Key), @"[^a-zA-Z0-9_-]", "-");
                sb.Append($@"<div class=""test-group"">
<h2 class=""toggler section-head"" data-target=""group-{groupId}""><span class=""chevron"">▶</span> {Escape(sectionTitle)} <span class=""badge {badgeClass}"">{list.Count} tests</span></h2>
<div id=""group-{groupId}"" class=""collapsible group-content"">");

                foreach (var r in list.OrderBy(x => x.Outcome).ThenBy(x => x.TestName))
                {
                    AppendTestCase(sb, r);
                }
                sb.Append("</div></div>");
            }
        }
        else
        {
            sb.Append("<div class=\"summary\"><p class=\"empty-msg\">No test results (TRX) provided.</p></div>");
            if (coverageData.Files.Count > 0)
            {
                sb.Append(@"<div class=""coverage-all-files coverage-at-top"">
  <span class=""coverage-all-label"">All files</span>
  <div class=""coverage-all-metrics"">
    <div class=""coverage-metric""><span class=""coverage-metric-label"">Statements</span><span class=""coverage-metric-value"">");
                sb.Append($"{coverageData.SummaryStmts:F1}%</span></div>");
                sb.Append(@"<div class=""coverage-metric""><span class=""coverage-metric-label"">Branches</span><span class=""coverage-metric-value"">");
                sb.Append($"{coverageData.SummaryBranch:F1}%</span></div>");
                sb.Append(@"<div class=""coverage-metric""><span class=""coverage-metric-label"">Functions</span><span class=""coverage-metric-value"">");
                sb.Append($"{coverageData.SummaryFuncs:F1}%</span></div>");
                sb.Append(@"<div class=""coverage-metric""><span class=""coverage-metric-label"">Lines</span><span class=""coverage-metric-value"">");
                sb.Append($"{coverageData.SummaryLines:F1}%</span></div>");
                sb.Append(@"</div>
</div>");
            }
        }

        if (coverageData.Files.Count > 0)
        {
            sb.Append(@"<h2 id=""coverage-summary"" class=""section-head"">Code coverage</h2>
<div class=""coverage-section"">
<p class=""coverage-table-caption"">Per-file coverage</p>
<table class=""coverage-table"">
<thead><tr><th>File</th><th class=""num"">Statements</th><th class=""num"">Branches</th><th class=""num"">Functions</th><th class=""num"">Lines</th><th>Uncovered line #s</th></tr></thead>
<tbody>");
            foreach (var row in coverageData.Files.OrderBy(x => x.File))
            {
                var pctClassStmts = row.PctStmts < 50 ? " low" : row.PctStmts < 80 ? " mid" : "";
                var pctClassBranch = row.PctBranch < 50 ? " low" : row.PctBranch < 80 ? " mid" : "";
                var pctClassFuncs = row.PctFuncs < 50 ? " low" : row.PctFuncs < 80 ? " mid" : "";
                var pctClassLines = row.PctLines < 50 ? " low" : row.PctLines < 80 ? " mid" : "";
                sb.Append($"<tr><td class=\"file-cell\">{Escape(row.File)}</td><td class=\"num pct{pctClassStmts}\">{row.PctStmts:F1}%</td><td class=\"num pct{pctClassBranch}\">{row.PctBranch:F1}%</td><td class=\"num pct{pctClassFuncs}\">{row.PctFuncs:F1}%</td><td class=\"num pct{pctClassLines}\">{row.PctLines:F1}%</td><td class=\"uncovered-cell\">{Escape(row.UncoveredLines)}</td></tr>");
            }
            sb.Append("</tbody></table></div>");
        }
        else
        {
            sb.Append("<h2 id=\"coverage-summary\" class=\"section-head\">Code coverage</h2><div class=\"no-coverage\">Coverage not collected (no Cobertura XML provided).</div>");
        }

        sb.Append(GetReportScript());
        sb.Append("</div></body></html>");
        return sb.ToString();
    }

    static string GetReportCss()
    {
        return @"<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>Contentstack Management .NET SDK - Enhanced Test Report</title>
<style>
:root {
  --bg: #0f1419;
  --surface: #1a2332;
  --surface2: #243044;
  --border: #2d3d52;
  --text: #e6edf3;
  --text-muted: #8b9cb3;
  --accent: #58a6ff;
  --accent-dim: #388bfd26;
  --pass: #3fb950;
  --fail: #f85149;
  --warn: #d29922;
  --font: 'JetBrains Mono', 'SF Mono', 'Fira Code', monospace;
}
* { margin: 0; padding: 0; box-sizing: border-box; }
body { font-family: var(--font); background: var(--bg); color: var(--text); padding: 24px; line-height: 1.5; font-size: 14px; }
.container { max-width: 1100px; margin: 0 auto; background: var(--surface); border-radius: 12px; box-shadow: 0 8px 32px rgba(0,0,0,0.4); overflow: hidden; border: 1px solid var(--border); }
h1 { background: linear-gradient(135deg, #1f6feb 0%, #388bfd 100%); color: #fff; padding: 24px 28px; font-size: 1.35rem; font-weight: 600; letter-spacing: -0.02em; }
.section-head { padding: 14px 24px; background: var(--surface2); border-bottom: 1px solid var(--border); font-size: 1rem; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 10px; user-select: none; }
.section-head:hover { background: #2a3850; }
.section-head .chevron { display: inline-block; transition: transform 0.2s ease; color: var(--accent); }
.section-head .chevron.open { transform: rotate(90deg); }
.section-head .badge { font-size: 0.7rem; padding: 3px 10px; border-radius: 12px; font-weight: 500; }
.section-head .badge.passed { background: rgba(63,185,80,0.2); color: var(--pass); }
.section-head .badge.failed { background: rgba(248,81,73,0.2); color: var(--fail); }
.summary { display: flex; flex-wrap: wrap; gap: 14px; padding: 20px 24px; background: var(--surface2); border-bottom: 1px solid var(--border); }
.summary-item { background: var(--surface); padding: 14px 20px; border-radius: 10px; border: 1px solid var(--border); min-width: 100px; }
.summary-item strong { display: block; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.05em; color: var(--text-muted); margin-bottom: 4px; }
.summary-item span { font-size: 1.25rem; font-weight: 600; }
.summary-item.passed span { color: var(--pass); }
.summary-item.failed span { color: var(--fail); }
.summary-item.skipped span { color: var(--text-muted); }
.collapsible { display: none; }
.collapsible.active { display: block; }
.group-content { padding: 0; }
.test-case { border-bottom: 1px solid var(--border); }
.test-case:last-child { border-bottom: none; }
.test-row { padding: 14px 24px; display: flex; align-items: center; gap: 14px; flex-wrap: wrap; cursor: pointer; transition: background 0.15s; }
.test-row:hover { background: var(--surface2); }
.test-row.failed { background: rgba(248,81,73,0.06); }
.test-name { flex: 1 1 280px; font-weight: 500; }
.test-row .outcome { font-weight: 600; padding: 4px 10px; border-radius: 6px; font-size: 0.8rem; }
.test-row .outcome.Passed { background: rgba(63,185,80,0.2); color: var(--pass); }
.test-row .outcome.Failed { background: rgba(248,81,73,0.2); color: var(--fail); }
.test-row .outcome.Skipped { background: rgba(139,156,179,0.2); color: var(--text-muted); }
.test-row .duration { color: var(--text-muted); font-size: 0.85rem; }
.test-row .detail-toggle { color: var(--accent); transition: transform 0.2s; }
.test-row .detail-toggle.open { transform: rotate(90deg); }
.test-details { display: none; padding: 0 24px 20px; }
.test-details.open { display: block; }
.detail-panel { background: var(--surface2); border-radius: 10px; border: 1px solid var(--border); margin-top: 12px; overflow: hidden; }
.detail-section { border-bottom: 1px solid var(--border); }
.detail-section:last-child { border-bottom: none; }
.detail-section-title { padding: 12px 16px; font-weight: 600; font-size: 0.9rem; display: flex; align-items: center; gap: 8px; cursor: pointer; user-select: none; }
.detail-section-title:hover { background: rgba(255,255,255,0.03); }
.detail-section-title .icon { opacity: 0.9; }
.detail-section-content { padding: 14px 16px; }
.detail-section-content.collapsed { display: none; }
.detail-section-content .sub { margin: 8px 0; font-size: 0.9rem; }
.detail-section-content .sub label { color: var(--text-muted); display: inline-block; min-width: 120px; }
.detail-section-content pre, .detail-section-content code { background: var(--bg); padding: 12px; border-radius: 8px; font-size: 0.85rem; overflow-x: auto; display: block; white-space: pre-wrap; word-break: break-all; border: 1px solid var(--border); }
.detail-section-content pre { margin: 8px 0; }
.assertion-card { background: var(--surface); border: 1px solid var(--border); border-radius: 8px; padding: 12px 14px; margin: 8px 0; }
.assertion-card .status { font-size: 1rem; }
.assertion-card.fail { border-left: 3px solid var(--fail); }
.assertion-card.pass { border-left: 3px solid var(--pass); }
.req-resp-row { margin: 6px 0; }
.req-resp-row .k { color: var(--text-muted); }
.copy-btn { background: var(--accent); color: #fff; border: none; padding: 6px 12px; border-radius: 6px; font-size: 0.8rem; cursor: pointer; font-family: inherit; margin-top: 8px; }
.copy-btn:hover { filter: brightness(1.1); }
.empty-section { color: var(--text-muted); font-style: italic; padding: 12px 0; }
.coverage-section { padding: 24px; }
.coverage-all-files { background: var(--surface2); border: 1px solid var(--border); border-radius: 10px; padding: 20px 24px; margin-bottom: 24px; }
.coverage-at-top { margin-bottom: 20px; }
.coverage-all-label { display: block; font-weight: 600; font-size: 1rem; color: var(--text); margin-bottom: 14px; }
.coverage-all-metrics { display: flex; flex-wrap: wrap; gap: 20px; }
.coverage-metric { background: var(--surface); border: 1px solid var(--border); border-radius: 8px; padding: 14px 20px; min-width: 100px; }
.coverage-metric-label { display: block; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.04em; color: var(--text-muted); margin-bottom: 4px; }
.coverage-metric-value { font-size: 1.35rem; font-weight: 600; color: var(--text); }
.coverage-table-caption { font-size: 0.9rem; font-weight: 600; color: var(--text-muted); margin-bottom: 12px; }
.coverage-table { width: 100%; border-collapse: collapse; font-size: 0.9rem; border: 1px solid var(--border); border-radius: 8px; overflow: hidden; }
.coverage-table th, .coverage-table td { padding: 14px 16px; text-align: left; border-bottom: 1px solid var(--border); }
.coverage-table th { background: var(--surface2); font-weight: 600; color: var(--text-muted); font-size: 0.8rem; text-transform: uppercase; letter-spacing: 0.03em; }
.coverage-table tbody tr:hover { background: var(--surface2); }
.coverage-table tbody tr:last-child td { border-bottom: none; }
.coverage-table .num { text-align: right; font-variant-numeric: tabular-nums; }
.coverage-table .file-cell { word-break: break-all; max-width: 280px; }
.coverage-table .uncovered-cell { font-size: 0.85rem; color: var(--text-muted); word-break: break-all; max-width: 200px; }
.coverage-table .pct.low { color: var(--fail); font-weight: 600; }
.coverage-table .pct.mid { color: var(--warn); }
.no-coverage { padding: 24px; color: var(--text-muted); text-align: center; }
.empty-msg { color: var(--text-muted); }
</style>
</head>
";
    }

    static void AppendTestCase(System.Text.StringBuilder sb, UnitTestResult r)
    {
        var outcomeClass = r.Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase) ? "Passed" : r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase) ? "Failed" : "Skipped";
        var testId = "test-" + Guid.NewGuid().ToString("N")[..8];
        sb.Append($"<div class=\"test-case\">");
        sb.Append($"<div class=\"test-row {outcomeClass.ToLowerInvariant()}\" data-detail=\"{testId}\" role=\"button\" tabindex=\"0\">");
        sb.Append($"<span class=\"detail-toggle\">▶</span>");
        sb.Append($"<span class=\"outcome {outcomeClass}\">{Escape(r.Outcome)}</span>");
        sb.Append($"<span class=\"duration\">{FormatDuration(r.Duration)}</span>");
        sb.Append($"<span class=\"test-name\">{Escape(r.TestName)}</span>");
        sb.Append("</div>");
        sb.Append($"<div id=\"{testId}\" class=\"test-details\">");
        AppendDetailSections(sb, r);
        sb.Append("</div>");
        sb.Append("</div>");
    }

    static void AppendDetailSections(System.Text.StringBuilder sb, UnitTestResult r)
    {
        sb.Append("<div class=\"detail-panel\">");

        sb.Append("<div class=\"detail-section\"><div class=\"detail-section-title\"><span class=\"icon\">✓</span> Assertions</div><div class=\"detail-section-content\">");
        if (r.Assertions.Count == 0)
            sb.Append("<div class=\"empty-section\">No assertion details in TRX. Failed message is shown in Error Details.</div>");
        else
        {
            for (var i = 0; i < r.Assertions.Count; i++)
            {
                var a = r.Assertions[i];
                var cardClass = a.Passed ? "pass" : "fail";
                sb.Append($"<div class=\"assertion-card {cardClass}\">");
                sb.Append($"<span class=\"status\">{(a.Passed ? "✅" : "❌")}</span> ");
                sb.Append($"<strong>{Escape(a.Name)}</strong>");
                if (!string.IsNullOrEmpty(a.AssertionType)) sb.Append($" <span class=\"sub\">Type: {Escape(a.AssertionType)}</span>");
                if (!string.IsNullOrEmpty(a.Expected)) sb.Append($"<div class=\"sub\"><label>Expected:</label> {Escape(a.Expected)}</div>");
                if (!string.IsNullOrEmpty(a.Actual)) sb.Append($"<div class=\"sub\"><label>Actual:</label> {Escape(a.Actual)}</div>");
                sb.Append("</div>");
            }
        }
        sb.Append("</div></div>");

        sb.Append("<div class=\"detail-section\"><div class=\"detail-section-title\"><span class=\"icon\">🌐</span> HTTP Requests</div><div class=\"detail-section-content\">");
        if (r.HttpRequests.Count == 0)
            sb.Append("<div class=\"empty-section\">No request data captured. Emit structured data from tests to populate.</div>");
        else
        {
            foreach (var req in r.HttpRequests)
            {
                sb.Append("<div class=\"req-resp-row\"><span class=\"k\">SDK Method:</span> ").Append(Escape(req.SdkMethod)).Append("</div>");
                sb.Append("<div class=\"req-resp-row\"><span class=\"k\">HTTP Method:</span> ").Append(Escape(req.HttpMethod)).Append("</div>");
                sb.Append("<div class=\"req-resp-row\"><span class=\"k\">Request URL:</span> ").Append(Escape(req.RequestUrl)).Append("</div>");
                if (req.QueryParams.Count > 0)
                {
                    sb.Append("<div class=\"req-resp-row\"><span class=\"k\">Query Parameters:</span></div>");
                    foreach (var q in req.QueryParams)
                        sb.Append($"<div class=\"sub\"> {Escape(q.Key)} = {Escape(q.Value)}</div>");
                }
                if (req.Headers.Count > 0)
                {
                    sb.Append("<div class=\"req-resp-row\"><span class=\"k\">Request Headers:</span></div>");
                    var headerLines = string.Join("\n", req.Headers.Select(h => $"{h.Key}: {h.Value}"));
                    sb.Append($"<pre class=\"headers-block\">{Escape(headerLines)}</pre>");
                }
                if (!string.IsNullOrEmpty(req.Body))
                    sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Request Body:</span></div><pre>{Escape(req.Body)}</pre>");
                if (!string.IsNullOrEmpty(req.CurlCommand))
                {
                    sb.Append("<div class=\"req-resp-row\"><span class=\"k\">cURL:</span></div><pre class=\"curl-block\">").Append(Escape(req.CurlCommand)).Append("</pre>");
                    sb.Append($"<button type=\"button\" class=\"copy-btn\" data-copy=\"{System.Net.WebUtility.HtmlEncode(req.CurlCommand ?? "")}\">Copy cURL</button>");
                }
            }
        }
        sb.Append("</div></div>");

        sb.Append("<div class=\"detail-section\"><div class=\"detail-section-title\"><span class=\"icon\">📥</span> HTTP Responses</div><div class=\"detail-section-content\">");
        if (r.HttpResponses.Count == 0)
            sb.Append("<div class=\"empty-section\">No response data captured.</div>");
        else
        {
            foreach (var res in r.HttpResponses)
            {
                if (res.StatusCode > 0) sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Status:</span> {res.StatusCode} {Escape(res.StatusText)}</div>");
                if (!string.IsNullOrEmpty(res.ResponseTimeMs)) sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Response Time:</span> {Escape(res.ResponseTimeMs)}</div>");
                if (res.Headers.Count > 0)
                {
                    sb.Append("<div class=\"req-resp-row\"><span class=\"k\">Response Headers:</span></div>");
                    var headerLines = string.Join("\n", res.Headers.Select(h => $"{h.Key}: {h.Value}"));
                    sb.Append($"<pre class=\"headers-block\">{Escape(headerLines)}</pre>");
                }
                if (!string.IsNullOrEmpty(res.Body)) sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Response Body:</span></div><pre>{Escape(res.Body)}</pre>");
                if (!string.IsNullOrEmpty(res.PayloadSize)) sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Payload Size:</span> {Escape(res.PayloadSize)}</div>");
            }
        }
        sb.Append("</div></div>");

        var isFailed = r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase);
        if (isFailed)
        {
            sb.Append("<div class=\"detail-section\"><div class=\"detail-section-title\"><span class=\"icon\">⚠️</span> Error Details</div><div class=\"detail-section-content\">");
            if (!string.IsNullOrEmpty(r.Message)) sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Error Message:</span></div><pre>{Escape(r.Message)}</pre>");
            if (!string.IsNullOrEmpty(r.ExceptionType)) sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Exception Type:</span> {Escape(r.ExceptionType)}</div>");
            if (!string.IsNullOrEmpty(r.StackTrace)) sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Stack Trace:</span></div><pre>{Escape(r.StackTrace)}</pre>");
            if (r.Assertions.Count > 0) sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Failed Assertion:</span> See Assertions #1 above.</div>");
            if (r.RetryCount.HasValue) sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Retry Count:</span> {r.RetryCount.Value}</div>");
            sb.Append("</div></div>");
        }

        sb.Append("<div class=\"detail-section\"><div class=\"detail-section-title\"><span class=\"icon\">ℹ️</span> Test Context</div><div class=\"detail-section-content\">");
        var ctx = r.Context;
        sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Environment:</span> {Escape(ctx.Environment)}</div>");
        sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Locale:</span> {Escape(ctx.Locale)}</div>");
        sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">SDK Version:</span> {Escape(ctx.SdkVersion)}</div>");
        sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Build Number:</span> {Escape(ctx.BuildNumber)}</div>");
        sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Commit SHA:</span> {Escape(ctx.CommitSha)}</div>");
        sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">Test Data Source:</span> {Escape(ctx.TestDataSource)}</div>");
        foreach (var kv in ctx.Keys)
            sb.Append($"<div class=\"req-resp-row\"><span class=\"k\">{Escape(kv.Key)}:</span> {Escape(kv.Value)}</div>");
        sb.Append("</div></div>");

        sb.Append("</div>");
    }

    static string GetReportScript()
    {
        return @"
<script>
(function() {
  function byClass(c) { return document.getElementsByClassName(c); }
  function byId(id) { return document.getElementById(id); }
  function qs(s, r) { return (r || document).querySelector(s); }
  function qsAll(s, r) { return (r || document).querySelectorAll(s); }
  qsAll('.toggler').forEach(function(el) {
    el.addEventListener('click', function() {
      var id = el.getAttribute('data-target');
      var target = id && byId(id);
      if (target) {
        target.classList.toggle('active');
        var chev = el.querySelector('.chevron');
        if (chev) chev.classList.toggle('open');
      }
    });
  });
  qsAll('.section-head').forEach(function(h) {
    var content = h.nextElementSibling;
    if (content && content.classList.contains('group-content')) {
      content.classList.add('active');
      var chev = h.querySelector('.chevron');
      if (chev) chev.classList.add('open');
    }
  });
  qsAll('.test-row').forEach(function(row) {
    row.addEventListener('click', function() {
      var id = row.getAttribute('data-detail');
      if (!id) return;
      var detail = byId(id);
      if (detail) {
        detail.classList.toggle('open');
        var toggle = row.querySelector('.detail-toggle');
        if (toggle) toggle.classList.toggle('open');
      }
    });
  });
  qsAll('.copy-btn').forEach(function(btn) {
    btn.addEventListener('click', function() {
      var text = btn.getAttribute('data-copy');
      if (text && navigator.clipboard) {
        navigator.clipboard.writeText(text).then(function() { btn.textContent = 'Copied!'; setTimeout(function() { btn.textContent = 'Copy cURL'; }, 1500); });
      }
    });
  });
})();
</script>";
    }

    static string FormatDuration(TimeSpan d) => d.TotalMilliseconds < 1000 ? $"{d.TotalMilliseconds:F0} ms" : $"{d.TotalSeconds:F2} s";
    static string Escape(string? s) => string.IsNullOrEmpty(s) ? "" : System.Net.WebUtility.HtmlEncode(s);
    static string EscapeAttr(string? s) => Escape(s)?.Replace("\"", "&quot;") ?? "";

    private sealed class AssertionRecord
    {
        public bool Passed { get; set; }
        public string Name { get; set; } = "";
        public string Expected { get; set; } = "";
        public string Actual { get; set; } = "";
        public string AssertionType { get; set; } = "";
    }

    private sealed class HttpRequestRecord
    {
        public string SdkMethod { get; set; } = "";
        public string HttpMethod { get; set; } = "";
        public string RequestUrl { get; set; } = "";
        public Dictionary<string, string> QueryParams { get; set; } = new();
        public Dictionary<string, string> Headers { get; set; } = new();
        public string Body { get; set; } = "";
        public string CurlCommand { get; set; } = "";
    }

    private sealed class HttpResponseRecord
    {
        public int StatusCode { get; set; }
        public string StatusText { get; set; } = "";
        public string ResponseTimeMs { get; set; } = "";
        public Dictionary<string, string> Headers { get; set; } = new();
        public string Body { get; set; } = "";
        public string PayloadSize { get; set; } = "";
    }

    private sealed class TestContextRecord
    {
        public Dictionary<string, string> Keys { get; set; } = new();
        public string Environment { get; set; } = "";
        public string Locale { get; set; } = "";
        public string SdkVersion { get; set; } = "";
        public string BuildNumber { get; set; } = "";
        public string CommitSha { get; set; } = "";
        public string TestDataSource { get; set; } = "";
    }

    private sealed class UnitTestResult
    {
        public string TestName { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string Outcome { get; set; } = "";
        public TimeSpan Duration { get; set; }
        public string Message { get; set; } = "";
        public string StackTrace { get; set; } = "";
        public string StdOut { get; set; } = "";
        public string StdErr { get; set; } = "";
        public string DebugTrace { get; set; } = "";
        public string Assembly { get; set; } = "";
        public List<AssertionRecord> Assertions { get; set; } = new();
        public List<HttpRequestRecord> HttpRequests { get; set; } = new();
        public List<HttpResponseRecord> HttpResponses { get; set; } = new();
        public TestContextRecord Context { get; set; } = new();
        public string ExceptionType { get; set; } = "";
        public int? RetryCount { get; set; }
    }

    private sealed class CoverageFileRow
    {
        public string File { get; set; } = "";
        public double PctStmts { get; set; }
        public double PctBranch { get; set; }
        public double PctFuncs { get; set; }
        public double PctLines { get; set; }
        public string UncoveredLines { get; set; } = "";
    }

    private sealed class TestReportData
    {
        public List<UnitTestResult> Results { get; set; } = new();
        public Dictionary<string, List<UnitTestResult>> ByAssembly { get; set; } = new();
        public Dictionary<string, List<UnitTestResult>> ByClass { get; set; } = new();
        public int Total { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public TimeSpan TotalDuration { get; set; }
    }

    private sealed class CoverageReportData
    {
        public List<CoverageFileRow> Files { get; set; } = new();
        public double SummaryStmts { get; set; }
        public double SummaryBranch { get; set; }
        public double SummaryFuncs { get; set; }
        public double SummaryLines { get; set; }
    }

    // ── POCOs that mirror the JSON written by TestReportHelper.Flush() ────────
    private sealed class TestBlockPayload
    {
        public List<AssertionP>  Assertions   { get; set; }
        public List<RequestP>    HttpRequests  { get; set; }
        public List<ResponseP>   HttpResponses { get; set; }
        public ContextP          Context       { get; set; }
    }
    private sealed class AssertionP
    {
        public bool   Passed        { get; set; }
        public string Name          { get; set; }
        public string Expected      { get; set; }
        public string Actual        { get; set; }
        public string AssertionType { get; set; }
    }
    private sealed class RequestP
    {
        public string SdkMethod   { get; set; }
        public string HttpMethod  { get; set; }
        public string RequestUrl  { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }
        public Dictionary<string, string> Headers     { get; set; }
        public string Body        { get; set; }
        public string CurlCommand { get; set; }
    }
    private sealed class ResponseP
    {
        public int    StatusCode     { get; set; }
        public string StatusText     { get; set; }
        public string ResponseTimeMs { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body           { get; set; }
        public string PayloadSize    { get; set; }
    }
    private sealed class ContextP
    {
        public string Environment    { get; set; }
        public string SdkVersion     { get; set; }
        public string BuildNumber    { get; set; }
        public string CommitSha      { get; set; }
        public string TestDataSource { get; set; }
        public string Locale         { get; set; }
    }
}
