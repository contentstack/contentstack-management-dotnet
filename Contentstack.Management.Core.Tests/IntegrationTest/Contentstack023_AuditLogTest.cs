using System;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    [DoNotParallelize]
    public class Contentstack023_AuditLogTest
    {
        private static ContentstackClient _client;
        private Stack _stack;

        // Shared state populated in Test001 / Test002 and used by fetch tests
        private static string _firstLogUid;
        private static string _secondLogUid;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            try { _client?.Logout(); } catch { }
            _client = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);
        }

        // ─────────────────────────────────────────────────────────────
        // Test001 — List (sync)
        // ─────────────────────────────────────────────────────────────
        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_AuditLog_List()
        {
            TestOutputLogger.LogContext("TestScenario", "AuditLog_List_Sync");

            ContentstackResponse response = _stack.AuditLog().FindAll();

            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsTrue(response.IsSuccessStatusCode,
                $"FindAll should succeed, got {(int)response.StatusCode}: {response.OpenResponse()}",
                "FindAllSucceeded");

            var jObject = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jObject, "jObject");

            var logs = jObject["logs"] as JArray;
            AssertLogger.IsNotNull(logs, "logs array");

            // Audit log MUST have entries because tests 001–022 ran operations before this
            AssertLogger.IsTrue(logs.Count > 0,
                "Audit log should contain at least one entry — empty log indicates no operations were recorded",
                "LogsNotEmpty");

            // Capture UIDs for subsequent single-fetch tests
            if (logs.Count > 0)
            {
                _firstLogUid = logs[0]["uid"]?.ToString();
                TestOutputLogger.LogContext("FirstLogUid", _firstLogUid ?? "(null)");
            }

            if (logs.Count > 1)
            {
                _secondLogUid = logs[1]["uid"]?.ToString();
                TestOutputLogger.LogContext("SecondLogUid", _secondLogUid ?? "(null)");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // Test002 — List (async)
        // ─────────────────────────────────────────────────────────────
        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Return_AuditLog_List_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "AuditLog_List_Async");

            ContentstackResponse response = await _stack.AuditLog().FindAllAsync();

            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsTrue(response.IsSuccessStatusCode,
                $"FindAllAsync should succeed, got {(int)response.StatusCode}: {response.OpenResponse()}",
                "FindAllAsyncSucceeded");

            var jObject = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jObject, "jObject");

            var logs = jObject["logs"] as JArray;
            AssertLogger.IsNotNull(logs, "logs array");

            AssertLogger.IsTrue(logs.Count > 0,
                "Async audit log list should have at least one entry",
                "AsyncLogsNotEmpty");

            // Back-fill UIDs if Test001 did not run first in isolation
            if (_firstLogUid == null && logs.Count > 0)
            {
                _firstLogUid = logs[0]["uid"]?.ToString();
            }

            if (_secondLogUid == null && logs.Count > 1)
            {
                _secondLogUid = logs[1]["uid"]?.ToString();
            }
        }

        // ─────────────────────────────────────────────────────────────
        // Test003 — Limit parameter
        // ─────────────────────────────────────────────────────────────
        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Support_Limit_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "AuditLog_Limit_Parameter");

            var collection = new ParameterCollection();
            collection.Add("limit", 5);

            ContentstackResponse response = _stack.AuditLog().FindAll(collection);

            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsTrue(response.IsSuccessStatusCode,
                $"FindAll with limit should succeed, got {(int)response.StatusCode}: {response.OpenResponse()}",
                "LimitCallSucceeded");

            var logs = response.OpenJObjectResponse()?["logs"] as JArray;
            AssertLogger.IsNotNull(logs, "logs array");

            // SDK must honour the limit — if it returns more than 5 the parameter is being silently ignored
            AssertLogger.IsTrue(logs.Count <= 5,
                $"Response returned {logs.Count} entries but limit was 5 — SDK is ignoring the limit parameter",
                "LimitRespected");

            // The stack has prior operations so at least one entry is expected
            AssertLogger.IsTrue(logs.Count > 0,
                "Audit log with limit=5 should still return at least one entry",
                "LimitResultNotEmpty");
        }

        // ─────────────────────────────────────────────────────────────
        // Test004 — Skip + Limit, field presence
        // ─────────────────────────────────────────────────────────────
        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Support_Skip_And_Limit()
        {
            TestOutputLogger.LogContext("TestScenario", "AuditLog_Skip_And_Limit");

            var collection = new ParameterCollection();
            collection.Add("skip", 0);
            collection.Add("limit", 1);

            ContentstackResponse response = _stack.AuditLog().FindAll(collection);

            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsTrue(response.IsSuccessStatusCode,
                $"FindAll skip=0 limit=1 should succeed, got {(int)response.StatusCode}",
                "SkipLimitSucceeded");

            var logs = response.OpenJObjectResponse()?["logs"] as JArray;
            AssertLogger.IsNotNull(logs, "logs array");

            // Exactly one entry expected — catches off-by-one in pagination handling
            AssertLogger.AreEqual(1, logs.Count,
                "Expected exactly 1 entry with skip=0 limit=1 — pagination is broken if count differs",
                "ExactlyOneEntry");

            var entry = logs[0] as JObject;
            AssertLogger.IsNotNull(entry, "first log entry");

            string uid = entry["uid"]?.ToString();
            string action = entry["action"]?.ToString();
            string module = entry["module"]?.ToString();

            AssertLogger.IsTrue(!string.IsNullOrEmpty(uid),
                "Log entry uid must not be empty", "EntryUidPresent");
            AssertLogger.IsTrue(!string.IsNullOrEmpty(action),
                "Log entry action must not be empty", "EntryActionPresent");
            AssertLogger.IsTrue(!string.IsNullOrEmpty(module),
                "Log entry module must not be empty", "EntryModulePresent");
        }

        // ─────────────────────────────────────────────────────────────
        // Test005 — Fetch single entry by UID (sync)
        // ─────────────────────────────────────────────────────────────
        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Fetch_Single_AuditLog_Entry_By_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "AuditLog_Fetch_Single_Sync");

            if (string.IsNullOrEmpty(_firstLogUid))
            {
                Assert.Inconclusive("_firstLogUid is not set — ensure Test001 or Test002 ran first.");
                return;
            }

            ContentstackResponse response = _stack.AuditLog(_firstLogUid).Fetch();

            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsTrue(response.IsSuccessStatusCode,
                $"Fetch single entry should succeed, got {(int)response.StatusCode}: {response.OpenResponse()}",
                "FetchSingleSucceeded");

            var jObject = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jObject, "jObject");

            // Response key is "log" (singular), not "logs"
            var log = jObject["log"] as JObject;
            AssertLogger.IsNotNull(log, "log object in response");

            string returnedUid = log["uid"]?.ToString();
            AssertLogger.IsNotNull(returnedUid, "returned uid");

            // Round-trip check: the uid returned by detail must match the one used in the request
            AssertLogger.AreEqual(_firstLogUid, returnedUid,
                "Fetched log uid does not match the requested uid — uid mismatch between list and detail endpoint",
                "UidRoundTrip");
        }

        // ─────────────────────────────────────────────────────────────
        // Test006 — Fetch single entry by UID (async)
        // ─────────────────────────────────────────────────────────────
        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Fetch_AuditLog_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "AuditLog_Fetch_Single_Async");

            string uidToFetch = _secondLogUid ?? _firstLogUid;

            if (string.IsNullOrEmpty(uidToFetch))
            {
                Assert.Inconclusive("No log UID available — ensure Test001 or Test002 ran first.");
                return;
            }

            ContentstackResponse response = await _stack.AuditLog(uidToFetch).FetchAsync();

            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsTrue(response.IsSuccessStatusCode,
                $"FetchAsync should succeed, got {(int)response.StatusCode}: {response.OpenResponse()}",
                "FetchAsyncSucceeded");

            var jObject = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jObject, "jObject");

            var log = jObject["log"] as JObject;
            AssertLogger.IsNotNull(log, "log object in async response");

            string returnedUid = log["uid"]?.ToString();
            AssertLogger.AreEqual(uidToFetch, returnedUid,
                "Async fetch returned a different uid than requested",
                "AsyncUidRoundTrip");
        }

        // ─────────────────────────────────────────────────────────────
        // Test007 — Non-existent UID must throw 404/422
        // ─────────────────────────────────────────────────────────────
        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Throw_On_Fetch_NonExistent_Log_Entry()
        {
            TestOutputLogger.LogContext("TestScenario", "AuditLog_Fetch_NonExistent");

            const string fakeUid = "blt_nonexistent_log_uid_xyz";

            try
            {
                _stack.AuditLog(fakeUid).Fetch();

                // If no exception was thrown the API accepted a bogus UID — fail the test
                AssertLogger.Fail(
                    "Expected ContentstackErrorException for a non-existent audit log UID but no exception was thrown",
                    "NonExistentUidShouldThrow");
            }
            catch (ContentstackErrorException ex)
            {
                bool isExpectedStatus =
                    ex.StatusCode == HttpStatusCode.NotFound ||
                    ex.StatusCode == (HttpStatusCode)422 ||
                    ex.StatusCode == HttpStatusCode.UnprocessableEntity;

                AssertLogger.IsTrue(isExpectedStatus,
                    $"Expected 404 or 422 for non-existent uid, got {(int)ex.StatusCode} ({ex.StatusCode})",
                    "NonExistentUidStatusCode");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // Test008 — Required fields present on every entry
        // ─────────────────────────────────────────────────────────────
        [TestMethod]
        [DoNotParallelize]
        public void Test008_AuditLog_Entries_Have_Required_Fields()
        {
            TestOutputLogger.LogContext("TestScenario", "AuditLog_RequiredFields");

            // Retrieve the first few entries to validate schema
            var collection = new ParameterCollection();
            collection.Add("limit", 5);

            ContentstackResponse response = _stack.AuditLog().FindAll(collection);

            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsTrue(response.IsSuccessStatusCode,
                $"FindAll for schema check should succeed, got {(int)response.StatusCode}",
                "SchemaCheckSucceeded");

            var logs = response.OpenJObjectResponse()?["logs"] as JArray;
            AssertLogger.IsNotNull(logs, "logs array");
            AssertLogger.IsTrue(logs.Count > 0, "logs must not be empty for field validation", "LogsForSchemaCheck");

            for (int i = 0; i < logs.Count; i++)
            {
                var entry = logs[i] as JObject;
                AssertLogger.IsNotNull(entry, $"entry[{i}]");

                string uid = entry["uid"]?.ToString();
                string action = entry["action"]?.ToString();
                string module = entry["module"]?.ToString();
                string createdAt = entry["created_at"]?.ToString();

                // uid must be a non-empty string
                AssertLogger.IsTrue(!string.IsNullOrEmpty(uid),
                    $"Entry[{i}].uid is missing or empty — schema regression detected",
                    $"Entry{i}_UidPresent");

                // action must be a non-empty string (e.g. "create", "update", "delete")
                AssertLogger.IsTrue(!string.IsNullOrEmpty(action),
                    $"Entry[{i}].action is missing or empty — schema regression detected",
                    $"Entry{i}_ActionPresent");

                // module must be a non-empty string (e.g. "content_type", "asset")
                AssertLogger.IsTrue(!string.IsNullOrEmpty(module),
                    $"Entry[{i}].module is missing or empty — schema regression detected",
                    $"Entry{i}_ModulePresent");

                // created_at must exist and must be parseable as a DateTime
                AssertLogger.IsTrue(!string.IsNullOrEmpty(createdAt),
                    $"Entry[{i}].created_at is missing — schema regression detected",
                    $"Entry{i}_CreatedAtPresent");

                bool parsedDate = DateTime.TryParse(createdAt, out _);
                AssertLogger.IsTrue(parsedDate,
                    $"Entry[{i}].created_at value '{createdAt}' cannot be parsed as DateTime",
                    $"Entry{i}_CreatedAtParseable");
            }
        }
    }
}
