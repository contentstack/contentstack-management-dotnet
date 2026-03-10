# Test report artifacts – do not commit

## Why

TRX files and HTML reports (EnhancedTestReport, ReportGenerator coverage) are built from test runs that capture **stdout**. Integration tests using `TestReportHelper` emit JSON that includes:

- HTTP request URLs, query parameters, headers, and bodies  
- HTTP response headers and bodies  
- Assertion details  

Those outputs can contain **management tokens, API keys, stack API URLs, org identifiers**, and other sensitive data. **Treat every generated report as confidential.**

## What is ignored

The repository `.gitignore` is configured so that:

| Pattern | Purpose |
|--------|--------|
| `*.trx` | VSTest TRX may embed full StdOut from each test. |
| `**/TestResults/` | Default location for TRX, Cobertura, and report HTML. |
| `**/EnhancedReport*.html` | Single-file enhanced reports (if written outside `TestResults` by mistake). |
| `**/Coverage-*/**/*.html` | ReportGenerator output trees. |

## Good practice

1. **Write reports only under `TestResults`**  
   Scripts should pass `--output` / targetdir under each test project’s `TestResults` folder so one ignore rule covers everything.

2. **CI only**  
   Upload reports as **artifacts** (`actions/upload-artifact`) with `if: success() || failure()` for debugging—never commit them back to the branch.

3. **Never `git add -f`** on ignored report paths  
   If a file was tracked before it was ignored, remove it from the index:  
   `git rm -r --cached path/to/TestResults`

4. **Optional local hook**  
   To block accidental commits, install a pre-commit hook that rejects staged paths under `TestResults/` or matching `EnhancedReport*.html`. See `Scripts/pre-commit-no-test-artifacts.sample`.

## Related

- **EnhancedTestReport** tool: `tools/EnhancedTestReport/` – run after `dotnet test` via `Scripts/run-unit-test-case.sh` or `Scripts/run-test-case.sh`. Output is always under `**/TestResults/` (ignored by git).
- Test pipeline overview: see internal `TEST_REPORT_LOGIC.md` (or your team’s copy) for TRX + Cobertura + EnhancedTestReport flow.
