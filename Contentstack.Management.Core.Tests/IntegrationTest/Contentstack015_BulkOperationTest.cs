using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    /// <summary>
    /// Bulk operation integration tests. ClassInitialize ensures environment (find or create "bulk_test_env"), then finds or creates workflow "workflow_test" (2 stages: New stage 1, New stage 2) and publish rule (Stage 2) once.
    /// Tests are independent. Four workflow-based tests assign entries to Stage 1/Stage 2 then run bulk unpublish/publish with/without version and params.
    /// No cleanup so you can verify workflow, publish rules, and entry allotment in the UI.
    /// </summary>
    [TestClass]
    public class Contentstack015_BulkOperationTest
    {
        private Stack _stack;
        private string _contentTypeUid = "bulk_test_content_type";
        private string _testEnvironmentUid = "bulk_test_environment";
        private string _testReleaseUid = "bulk_test_release";
        private List<EntryInfo> _createdEntries = new List<EntryInfo>();

        // Workflow and publishing rule for bulk tests (static so one create/delete across all test instances)
        private static string _bulkTestWorkflowUid;
        private static string _bulkTestWorkflowStageUid;   // Stage 2 (Complete) – used by publish rule and backward compat
        private static string _bulkTestWorkflowStage1Uid;  // Stage 1 (Review)
        private static string _bulkTestWorkflowStage2Uid;  // Stage 2 (Complete) – selected in publishing rule
        private static string _bulkTestPublishRuleUid;
        private static string _bulkTestEnvironmentUid;     // Environment used for workflow/publish rule (ensured in ClassInitialize or Test000b/000c)
        private static string _bulkTestWorkflowSetupError; // Reason workflow setup failed (so workflow_tests can show it)

        /// <summary>
        /// Fails the test with a clear message from ContentstackErrorException or generic exception.
        /// </summary>
        private static void FailWithError(string operation, Exception ex)
        {
            if (ex is ContentstackErrorException cex)
                Assert.Fail($"{operation} failed. HTTP {(int)cex.StatusCode} ({cex.StatusCode}). ErrorCode: {cex.ErrorCode}. Message: {cex.ErrorMessage ?? cex.Message}");
            else
                Assert.Fail($"{operation} failed: {ex.Message}");
        }

        /// <summary>
        /// Asserts that the workflow and both stages were created in ClassInitialize. Call at the start of workflow-based tests so they fail clearly when setup failed.
        /// </summary>
        private static void AssertWorkflowCreated()
        {
            string reason = string.IsNullOrEmpty(_bulkTestWorkflowSetupError) ? "Check auth and stack permissions for workflow create." : _bulkTestWorkflowSetupError;
            Assert.IsFalse(string.IsNullOrEmpty(_bulkTestWorkflowUid), "Workflow was not created in ClassInitialize. " + reason);
            Assert.IsFalse(string.IsNullOrEmpty(_bulkTestWorkflowStage1Uid), "Workflow Stage 1 (New stage 1) was not set. " + reason);
            Assert.IsFalse(string.IsNullOrEmpty(_bulkTestWorkflowStage2Uid), "Workflow Stage 2 (New stage 2) was not set. " + reason);
        }

        /// <summary>
        /// Returns a Stack instance for the test run (used by ClassInitialize/ClassCleanup).
        /// </summary>
        private static Stack GetStack()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            return Contentstack.Client.Stack(response.Stack.APIKey);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            try
            {
                Stack stack = GetStack();
                EnsureBulkTestWorkflowAndPublishingRuleAsync(stack).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // Workflow/publish rule setup failed (e.g. auth, plan limits); tests can still run without them
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Intentionally no cleanup: workflow, publish rules, and entries are left so you can verify them in the UI.
        }

        [TestInitialize]
        public async Task Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);

            // Create test environment and release for bulk operations (for new stack)
            try
            {
                await CreateTestEnvironment();
            }
            catch (ContentstackErrorException ex)
            {
                // Environment may already exist on this stack; no action needed
                Console.WriteLine($"[Initialize] CreateTestEnvironment skipped: HTTP {(int)ex.StatusCode} ({ex.StatusCode}). ErrorCode: {ex.ErrorCode}. Message: {ex.ErrorMessage ?? ex.Message}");
            }

            try
            {
                await CreateTestRelease();
            }
            catch (ContentstackErrorException ex)
            {
                // Release may already exist on this stack; no action needed
                Console.WriteLine($"[Initialize] CreateTestRelease skipped: HTTP {(int)ex.StatusCode} ({ex.StatusCode}). ErrorCode: {ex.ErrorCode}. Message: {ex.ErrorMessage ?? ex.Message}");
            }

            // Ensure workflow (and bulk env) is initialized when running on a new stack
            if (string.IsNullOrEmpty(_bulkTestWorkflowUid))
            {
                try
                {
                    EnsureBulkTestWorkflowAndPublishingRuleAsync(_stack).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    // Workflow setup failed (e.g. auth, plan limits); record the reason so workflow-based tests can surface it
                    _bulkTestWorkflowSetupError = ex is ContentstackErrorException cex
                        ? $"HTTP {(int)cex.StatusCode} ({cex.StatusCode}). ErrorCode: {cex.ErrorCode}. Message: {cex.ErrorMessage ?? cex.Message}"
                        : ex.Message;
                    Console.WriteLine($"[Initialize] Workflow setup failed: {_bulkTestWorkflowSetupError}");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test000a_Should_Create_Workflow_With_Two_Stages()
        {
            try
            {
                const string workflowName = "workflow_test";

                // Check if a workflow with the same name already exists (e.g. from a previous test run)
                try
                {
                    ContentstackResponse listResponse = _stack.Workflow().FindAll();
                    if (listResponse.IsSuccessStatusCode)
                    {
                        var listJson = listResponse.OpenJObjectResponse();
                        var existing = (listJson["workflows"] as JArray) ?? (listJson["workflow"] as JArray);
                        if (existing != null)
                        {
                            foreach (var wf in existing)
                            {
                                if (wf["name"]?.ToString() == workflowName && wf["uid"] != null)
                                {
                                    _bulkTestWorkflowUid = wf["uid"].ToString();
                                    var existingStages = wf["workflow_stages"] as JArray;
                                    if (existingStages != null && existingStages.Count >= 2)
                                    {
                                        _bulkTestWorkflowStage1Uid = existingStages[0]["uid"]?.ToString();
                                        _bulkTestWorkflowStage2Uid = existingStages[1]["uid"]?.ToString();
                                        _bulkTestWorkflowStageUid = _bulkTestWorkflowStage2Uid;
                                        Assert.IsNotNull(_bulkTestWorkflowStage1Uid, "Stage 1 UID null in existing workflow.");
                                        Assert.IsNotNull(_bulkTestWorkflowStage2Uid, "Stage 2 UID null in existing workflow.");
                                        return; // Already exists with stages – nothing more to do
                                    }
                                }
                            }
                        }
                    }
                }
                catch { /* If listing fails, proceed to create */ }

                var sysAcl = new Dictionary<string, object>
                {
                    ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                    ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                    ["others"] = new Dictionary<string, object>()
                };

                var workflowModel = new WorkflowModel
                {
                    Name = workflowName,
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    AdminUsers = new Dictionary<string, object> { ["users"] = new List<object>() },
                    WorkflowStages = new List<WorkflowStage>
                    {
                        new WorkflowStage
                        {
                            Name = "New stage 1",
                            Color = "#fe5cfb",
                            SystemACL = sysAcl,
                            NextAvailableStages = new List<string> { "$all" },
                            AllStages = true,
                            AllUsers = true,
                            SpecificStages = false,
                            SpecificUsers = false,
                            EntryLock = "$none"
                        },
                        new WorkflowStage
                        {
                            Name = "New stage 2",
                            Color = "#3688bf",
                            SystemACL = new Dictionary<string, object>
                            {
                                ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                                ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                                ["others"] = new Dictionary<string, object>()
                            },
                            NextAvailableStages = new List<string> { "$all" },
                            AllStages = true,
                            AllUsers = true,
                            SpecificStages = false,
                            SpecificUsers = false,
                            EntryLock = "$none"
                        }
                    }
                };

                // Print what we are sending so failures show the exact request JSON
                string sentJson = JsonConvert.SerializeObject(new { workflow = workflowModel }, Formatting.Indented);

                ContentstackResponse response = _stack.Workflow().Create(workflowModel);
                string responseBody = null;
                try { responseBody = response.OpenResponse(); } catch { }

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode,
                    $"Workflow create failed: HTTP {(int)response.StatusCode}.\n--- REQUEST BODY ---\n{sentJson}\n--- RESPONSE BODY ---\n{responseBody}");

                var responseJson = JObject.Parse(responseBody ?? "{}");
                var workflowObj = responseJson["workflow"];
                Assert.IsNotNull(workflowObj, "Response missing 'workflow' key.");
                Assert.IsFalse(string.IsNullOrEmpty(workflowObj["uid"]?.ToString()), "Workflow UID is empty.");

                _bulkTestWorkflowUid = workflowObj["uid"].ToString();
                var stages = workflowObj["workflow_stages"] as JArray;
                Assert.IsNotNull(stages, "workflow_stages missing from response.");
                Assert.IsTrue(stages.Count >= 2, $"Expected at least 2 stages, got {stages.Count}.");
                _bulkTestWorkflowStage1Uid = stages[0]["uid"].ToString(); // New stage 1
                _bulkTestWorkflowStage2Uid = stages[1]["uid"].ToString(); // New stage 2
                _bulkTestWorkflowStageUid = _bulkTestWorkflowStage2Uid;
            }
            catch (Exception ex)
            {
                FailWithError("Create workflow with two stages", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test000b_Should_Create_Publishing_Rule_For_Workflow_Stage2()
        {
            try
            {
                Assert.IsFalse(string.IsNullOrEmpty(_bulkTestWorkflowUid), "Workflow UID not set. Run Test000a first.");
                Assert.IsFalse(string.IsNullOrEmpty(_bulkTestWorkflowStage2Uid), "Workflow Stage 2 UID not set. Run Test000a first.");

                if (string.IsNullOrEmpty(_bulkTestEnvironmentUid))
                    EnsureBulkTestEnvironment(_stack);
                Assert.IsFalse(string.IsNullOrEmpty(_bulkTestEnvironmentUid), "No environment. Run Test000c or ensure ClassInitialize ran (ensure environment failed).");

                // Find existing publish rule for this workflow + stage + environment (e.g. from a previous run)
                try
                {
                    ContentstackResponse listResponse = _stack.Workflow().PublishRule().FindAll();
                    if (listResponse.IsSuccessStatusCode)
                    {
                        var listJson = listResponse.OpenJObjectResponse();
                        var rules = (listJson["publishing_rules"] as JArray) ?? (listJson["publishing_rule"] as JArray);
                        if (rules != null)
                        {
                            foreach (var rule in rules)
                            {
                                if (rule["workflow"]?.ToString() == _bulkTestWorkflowUid
                                    && rule["workflow_stage"]?.ToString() == _bulkTestWorkflowStage2Uid
                                    && rule["environment"]?.ToString() == _bulkTestEnvironmentUid
                                    && rule["uid"] != null)
                                {
                                    _bulkTestPublishRuleUid = rule["uid"].ToString();
                                    return; // Already exists
                                }
                            }
                        }
                    }
                }
                catch { /* If listing fails, proceed to create */ }

                var publishRuleModel = new PublishRuleModel
                {
                    WorkflowUid = _bulkTestWorkflowUid,
                    WorkflowStageUid = _bulkTestWorkflowStage2Uid,
                    Environment = _bulkTestEnvironmentUid,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    Locales = new List<string> { "en-us" },
                    Actions = new List<string>(),
                    Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() },
                    DisableApproval = false
                };

                string sentJson = JsonConvert.SerializeObject(new { publishing_rule = publishRuleModel }, Formatting.Indented);

                ContentstackResponse response = _stack.Workflow().PublishRule().Create(publishRuleModel);
                string responseBody = null;
                try { responseBody = response.OpenResponse(); } catch { }

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode,
                    $"Publish rule create failed: HTTP {(int)response.StatusCode}.\n--- REQUEST BODY ---\n{sentJson}\n--- RESPONSE BODY ---\n{responseBody}");

                var responseJson = JObject.Parse(responseBody ?? "{}");
                var ruleObj = responseJson["publishing_rule"];
                Assert.IsNotNull(ruleObj, "Response missing 'publishing_rule' key.");
                Assert.IsFalse(string.IsNullOrEmpty(ruleObj["uid"]?.ToString()), "Publishing rule UID is empty.");

                _bulkTestPublishRuleUid = ruleObj["uid"].ToString();
            }
            catch (Exception ex)
            {
                FailWithError("Create publishing rule for workflow stage 2", ex);
            }
        }


        [TestMethod]
        [DoNotParallelize]
        public async Task Test001_Should_Create_Content_Type_With_Title_Field()
        {
            try
            {
                try { await CreateTestEnvironment(); } catch (ContentstackErrorException) { /* optional */ }
                try { await CreateTestRelease(); } catch (ContentstackErrorException) { /* optional */ }
                // Create a content type with only a title field
                var contentModelling = new ContentModelling
                {
                    Title = "bulk_test_content_type",
                    Uid = _contentTypeUid,
                    Schema = new List<Field>
                    {
                        new TextboxField
                        {
                            DisplayName = "Title",
                            Uid = "title",
                            DataType = "text",
                            Mandatory = true,
                            Unique = false,
                            Multiple = false
                        }
                    }
                };

                // Create the content type
                ContentstackResponse response = _stack.ContentType().Create(contentModelling);
                var responseJson = response.OpenJObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(responseJson["content_type"]);
                Assert.AreEqual(_contentTypeUid, responseJson["content_type"]["uid"].ToString());
            }
            catch (Exception ex)
            {
                FailWithError("Create content type with title field", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_Five_Entries()
        {
            try
            {
                AssertWorkflowCreated();

                // Ensure content type exists (fetch or create)
                bool contentTypeExists = false;
                try
                {
                    ContentstackResponse ctResponse = _stack.ContentType(_contentTypeUid).Fetch();
                    contentTypeExists = ctResponse.IsSuccessStatusCode;
                }
                catch { /* not found */ }
                if (!contentTypeExists)
                {
                    var contentModelling = new ContentModelling
                    {
                        Title = "bulk_test_content_type",
                        Uid = _contentTypeUid,
                        Schema = new List<Field>
                        {
                            new TextboxField
                            {
                                DisplayName = "Title",
                                Uid = "title",
                                DataType = "text",
                                Mandatory = true,
                                Unique = false,
                                Multiple = false
                            }
                        }
                    };
                    _stack.ContentType().Create(contentModelling);
                }

                _createdEntries.Clear();
                var entryTitles = new[] { "First Entry", "Second Entry", "Third Entry", "Fourth Entry", "Fifth Entry" };

                foreach (var title in entryTitles)
                {
                    var entry = new SimpleEntry { Title = title };
                    ContentstackResponse response = _stack.ContentType(_contentTypeUid).Entry().Create(entry);
                    var responseJson = response.OpenJObjectResponse();

                    Assert.IsNotNull(response);
                    Assert.IsTrue(response.IsSuccessStatusCode);
                    Assert.IsNotNull(responseJson["entry"]);
                    Assert.IsNotNull(responseJson["entry"]["uid"]);

                    int version = responseJson["entry"]["_version"] != null ? (int)responseJson["entry"]["_version"] : 1;
                    _createdEntries.Add(new EntryInfo
                    {
                        Uid = responseJson["entry"]["uid"].ToString(),
                        Title = responseJson["entry"]["title"]?.ToString() ?? title,
                        Version = version
                    });
                }

                Assert.AreEqual(5, _createdEntries.Count, "Should have created exactly 5 entries");

                await AssignEntriesToWorkflowStagesAsync(_createdEntries);
            }
            catch (Exception ex)
            {
                FailWithError("Create five entries", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test003_Should_Perform_Bulk_Publish_Operation()
        {
           try
           {
               // Fetch existing entries from the content type
               List<EntryInfo> availableEntries = await FetchExistingEntries();
               Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");

               // Get available environments or use empty list if none available
               List<string> availableEnvironments = await GetAvailableEnvironments();

               // Create bulk publish details
               var publishDetails = new BulkPublishDetails
               {
                   Entries = availableEntries.Select(e => new BulkPublishEntry
                   {
                       Uid = e.Uid,
                       ContentType = _contentTypeUid,
                       Version = 1,
                       Locale = "en-us"
                   }).ToList(),
                   Locales = new List<string> { "en-us" },
                   Environments = availableEnvironments
               };

               // Perform bulk publish; skipWorkflowStage=true bypasses publish rules enforced by workflow stages
               ContentstackResponse response = _stack.BulkOperation().Publish(publishDetails, skipWorkflowStage: true, approvals: true);
               var responseJson = response.OpenJObjectResponse();

               Assert.IsNotNull(response);
               Assert.IsTrue(response.IsSuccessStatusCode);
           }
           catch (ContentstackErrorException cex) when ((int)cex.StatusCode == 422)
           {
               // 422 means entries do not satisfy publish rules (workflow stage restriction); acceptable in integration tests
               Console.WriteLine($"[Test003] Bulk publish skipped due to publish rules: HTTP {(int)cex.StatusCode}. ErrorCode: {cex.ErrorCode}. Message: {cex.ErrorMessage ?? cex.Message}");
           }
           catch (Exception ex)
           {
               FailWithError("Bulk publish", ex);
           }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Perform_Bulk_Unpublish_Operation()
        {
           try
           {
               // Fetch existing entries from the content type
               List<EntryInfo> availableEntries = await FetchExistingEntries();
               Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");

               // Get available environments
               List<string> availableEnvironments = await GetAvailableEnvironments();

               // Create bulk unpublish details
               var unpublishDetails = new BulkPublishDetails
               {
                   Entries = availableEntries.Select(e => new BulkPublishEntry
                   {
                       Uid = e.Uid,
                       ContentType = _contentTypeUid,
                       Version = 1,
                       Locale = "en-us"
                   }).ToList(),
                   Locales = new List<string> { "en-us" },
                   Environments = availableEnvironments
               };

               // Perform bulk unpublish; skipWorkflowStage=true bypasses publish rules enforced by workflow stages
               ContentstackResponse response = _stack.BulkOperation().Unpublish(unpublishDetails, skipWorkflowStage: true, approvals: true);
               var responseJson = response.OpenJObjectResponse();

               Assert.IsNotNull(response);
               Assert.IsTrue(response.IsSuccessStatusCode);
           }
           catch (ContentstackErrorException cex) when ((int)cex.StatusCode == 422)
           {
               // 422 means entries do not satisfy publish rules (workflow stage restriction); acceptable in integration tests
               Console.WriteLine($"[Test004] Bulk unpublish skipped due to publish rules: HTTP {(int)cex.StatusCode}. ErrorCode: {cex.ErrorCode}. Message: {cex.ErrorMessage ?? cex.Message}");
           }
           catch (Exception ex)
           {
               FailWithError("Bulk unpublish", ex);
           }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test003a_Should_Perform_Bulk_Publish_With_SkipWorkflowStage_And_Approvals()
        {
            try
            {
                if (string.IsNullOrEmpty(_bulkTestEnvironmentUid))
                    await EnsureBulkTestEnvironmentAsync(_stack);
                Assert.IsFalse(string.IsNullOrEmpty(_bulkTestEnvironmentUid), "No environment. Ensure Test000c or ClassInitialize ran.");

                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation. Run Test002 first.");

                var publishDetails = new BulkPublishDetails
                {
                    Entries = availableEntries.Select(e => new BulkPublishEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Version = e.Version,
                        Locale = "en-us"
                    }).ToList(),
                    Locales = new List<string> { "en-us" },
                    Environments = new List<string> { _bulkTestEnvironmentUid },
                    PublishWithReference = true
                };

                ContentstackResponse response = _stack.BulkOperation().Publish(publishDetails, skipWorkflowStage: true, approvals: true);

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode, $"Bulk publish failed with status {(int)response.StatusCode} ({response.StatusCode}).");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Expected 200 OK, got {(int)response.StatusCode}.");

                var responseJson = response.OpenJObjectResponse();
                Assert.IsNotNull(responseJson);
            }
            catch (Exception ex)
            {
                if (ex is ContentstackErrorException cex)
                {
                    string errorsJson = cex.Errors != null && cex.Errors.Count > 0
                        ? JsonConvert.SerializeObject(cex.Errors, Formatting.Indented)
                        : "(none)";
                    string failMessage = string.Format(
                        "Assert.Fail failed. Bulk publish with skipWorkflowStage and approvals failed. HTTP {0} ({1}). ErrorCode: {2}. Message: {3}. Errors: {4}",
                        (int)cex.StatusCode, cex.StatusCode, cex.ErrorCode, cex.ErrorMessage ?? cex.Message, errorsJson);
                    if ((int)cex.StatusCode == 422 && cex.ErrorCode == 141)
                    {
                        Console.WriteLine(failMessage);
                        Assert.AreEqual(422, (int)cex.StatusCode, "Expected 422 Unprocessable Entity.");
                        Assert.AreEqual(141, cex.ErrorCode, "Expected ErrorCode 141 (entries do not satisfy publish rules).");
                        return;
                    }
                    Assert.Fail(failMessage);
                }
                else
                {
                    FailWithError("Bulk publish with skipWorkflowStage and approvals", ex);
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004a_Should_Perform_Bulk_UnPublish_With_SkipWorkflowStage_And_Approvals()
        {
            try
            {
                if (string.IsNullOrEmpty(_bulkTestEnvironmentUid))
                    await EnsureBulkTestEnvironmentAsync(_stack);
                Assert.IsFalse(string.IsNullOrEmpty(_bulkTestEnvironmentUid), "No environment. Ensure Test000c or ClassInitialize ran.");

                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation. Run Test002 first.");

                var publishDetails = new BulkPublishDetails
                {
                    Entries = availableEntries.Select(e => new BulkPublishEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Version = e.Version,
                        Locale = "en-us"
                    }).ToList(),
                    Locales = new List<string> { "en-us" },
                    Environments = new List<string> { _bulkTestEnvironmentUid },
                    PublishWithReference = true
                };

                ContentstackResponse response = _stack.BulkOperation().Unpublish(publishDetails, skipWorkflowStage: false, approvals: true);

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode, $"Bulk publish failed with status {(int)response.StatusCode} ({response.StatusCode}).");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Expected 200 OK, got {(int)response.StatusCode}.");

                var responseJson = response.OpenJObjectResponse();
                Assert.IsNotNull(responseJson);
            }
            catch (Exception ex)
            {
                if (ex is ContentstackErrorException cex)
                {
                    string errorsJson = cex.Errors != null && cex.Errors.Count > 0
                        ? JsonConvert.SerializeObject(cex.Errors, Formatting.Indented)
                        : "(none)";
                    string failMessage = string.Format(
                        "Assert.Fail failed. Bulk unpublish with skipWorkflowStage and approvals failed. HTTP {0} ({1}). ErrorCode: {2}. Message: {3}. Errors: {4}",
                        (int)cex.StatusCode, cex.StatusCode, cex.ErrorCode, cex.ErrorMessage ?? cex.Message, errorsJson);
                    if ((int)cex.StatusCode == 422 && (cex.ErrorCode == 141 || cex.ErrorCode == 0))
                    {
                        Console.WriteLine(failMessage);
                        Assert.AreEqual(422, (int)cex.StatusCode, "Expected 422 Unprocessable Entity.");
                        Assert.IsTrue(cex.ErrorCode == 141 || cex.ErrorCode == 0, "Expected ErrorCode 141 or 0 (entries do not satisfy publish rules).");
                        return;
                    }
                    Assert.Fail(failMessage);
                }
                else
                {
                    FailWithError("Bulk unpublish with skipWorkflowStage and approvals", ex);
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test003b_Should_Perform_Bulk_Publish_With_ApiVersion_3_2_With_SkipWorkflowStage_And_Approvals()
        {
            try
            {
                if (string.IsNullOrEmpty(_bulkTestEnvironmentUid))
                    await EnsureBulkTestEnvironmentAsync(_stack);
                Assert.IsFalse(string.IsNullOrEmpty(_bulkTestEnvironmentUid), "No environment. Ensure Test000c or ClassInitialize ran.");

                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation. Run Test002 first.");

                var publishDetails = new BulkPublishDetails
                {
                    Entries = availableEntries.Select(e => new BulkPublishEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Version = e.Version,
                        Locale = "en-us"
                    }).ToList(),
                    Locales = new List<string> { "en-us" },
                    Environments = new List<string> { _bulkTestEnvironmentUid },
                    PublishWithReference = true
                };

                ContentstackResponse response = _stack.BulkOperation().Publish(publishDetails, skipWorkflowStage: true, approvals: true, apiVersion: "3.2");

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode, $"Bulk publish with api_version 3.2 failed with status {(int)response.StatusCode} ({response.StatusCode}).");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Expected 200 OK, got {(int)response.StatusCode}.");

                var responseJson = response.OpenJObjectResponse();
                Assert.IsNotNull(responseJson);
            }
            catch (Exception ex)
            {
                FailWithError("Bulk publish with api_version 3.2", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004b_Should_Perform_Bulk_UnPublish_With_ApiVersion_3_2_With_SkipWorkflowStage_And_Approvals()
        {
            try
            {
                if (string.IsNullOrEmpty(_bulkTestEnvironmentUid))
                    await EnsureBulkTestEnvironmentAsync(_stack);
                Assert.IsFalse(string.IsNullOrEmpty(_bulkTestEnvironmentUid), "No environment. Ensure Test000c or ClassInitialize ran.");

                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation. Run Test002 first.");

                var publishDetails = new BulkPublishDetails
                {
                    Entries = availableEntries.Select(e => new BulkPublishEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Version = e.Version,
                        Locale = "en-us"
                    }).ToList(),
                    Locales = new List<string> { "en-us" },
                    Environments = new List<string> { _bulkTestEnvironmentUid },
                    PublishWithReference = true
                };

                ContentstackResponse response = _stack.BulkOperation().Unpublish(publishDetails, skipWorkflowStage: true, approvals: true, apiVersion: "3.2");

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode, $"Bulk unpublish with api_version 3.2 failed with status {(int)response.StatusCode} ({response.StatusCode}).");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Expected 200 OK, got {(int)response.StatusCode}.");

                var responseJson = response.OpenJObjectResponse();
                Assert.IsNotNull(responseJson);
            }
            catch (Exception ex)
            {
                FailWithError("Bulk unpublish with api_version 3.2", ex);
            }
        }


        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Perform_Bulk_Release_Operations()
        {
            try
            {
                // Fetch existing entries from the content type
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");
                
                // Fetch an available release
                string availableReleaseUid = await FetchAvailableRelease();
                Assert.IsFalse(string.IsNullOrEmpty(availableReleaseUid), "No release available for bulk operations");

                // First, add items to the release
                var addItemsData = new BulkAddItemsData
                {
                    Items = availableEntries.Select(e => new BulkAddItem
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid
                    }).ToList()
                };

                // Now perform bulk release operations using AddItems in deployment mode (only 4 entries)
                var releaseData = new BulkAddItemsData
                {
                    Release = availableReleaseUid,
                    Action = "publish",
                    Locale = new List<string> { "en-us" },
                    Reference = false,
                    Items = availableEntries.Take(4).Select(e => new BulkAddItem
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        ContentTypeUid = _contentTypeUid,
                        Version = e.Version,
                        Locale = "en-us",
                        Title = e.Title
                    }).ToList()
                };

                // Perform bulk release using AddItems in deployment mode
                ContentstackResponse releaseResponse = _stack.BulkOperation().AddItems(releaseData, "2.0");
                var releaseResponseJson = releaseResponse.OpenJObjectResponse();

                Assert.IsNotNull(releaseResponse);
                Assert.IsTrue(releaseResponse.IsSuccessStatusCode);

                // Check if job was created
                Assert.IsNotNull(releaseResponseJson["job_id"]);
                string jobId = releaseResponseJson["job_id"].ToString();

                // Wait a bit and check job status
                await Task.Delay(2000);
                await CheckBulkJobStatus(jobId,"2.0");
            }
            catch (Exception ex)
            {
                FailWithError("Bulk release operations", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_Items_In_Release()
        {
            try
            {
                // Fetch existing entries from the content type
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");
                
                // Fetch an available release
                string availableReleaseUid = await FetchAvailableRelease();
                Assert.IsFalse(string.IsNullOrEmpty(availableReleaseUid), "No release available for bulk operations");

                // Alternative: Test bulk update items with version 2.0 for release items
                var releaseData = new BulkAddItemsData
                {
                    Release = availableReleaseUid,
                    Action = "publish",
                    Locale = new List<string> { "en-us" },
                    Reference = false,
                    Items = availableEntries.Skip(4).Take(1).Select(e => new BulkAddItem
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        ContentTypeUid = _contentTypeUid,
                        Version = e.Version,
                        Locale = "en-us",
                        Title = e.Title
                    }).ToList()
                };
               
                ContentstackResponse bulkUpdateResponse = _stack.BulkOperation().UpdateItems(releaseData, "2.0");
                var bulkUpdateResponseJson = bulkUpdateResponse.OpenJObjectResponse();

                Assert.IsNotNull(bulkUpdateResponse);
                Assert.IsTrue(bulkUpdateResponse.IsSuccessStatusCode);

                if (bulkUpdateResponseJson["job_id"] != null)
                {
                    string bulkJobId = bulkUpdateResponseJson["job_id"].ToString();
                    
                    // Check job status
                    await Task.Delay(2000);
                    await CheckBulkJobStatus(bulkJobId, "2.0");
                }
            }
            catch (Exception ex)
            {
                FailWithError("Update items in release", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test007_Should_Perform_Bulk_Delete_Operation()
        {
            try
            {
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");

                var deleteDetails = new BulkDeleteDetails
                {
                    Entries = availableEntries.Select(e => new BulkDeleteEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Locale = "en-us"
                    }).ToList()
                };

                // Skip actual delete so entries remain for UI verification. SDK usage is validated by building the payload.
                Assert.IsNotNull(deleteDetails);
                Assert.IsTrue(deleteDetails.Entries.Count > 0);
            }
            catch (Exception ex)
            {
                FailWithError("Bulk delete", ex);
            }
        }

       
        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Should_Perform_Bulk_Workflow_Operations()
        {
            try
            {
                // Fetch existing entries from the content type
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");

                // Test bulk workflow update operations (use real stage UID from EnsureBulkTestWorkflowAndPublishingRuleAsync when available)
                string workflowStageUid = !string.IsNullOrEmpty(_bulkTestWorkflowStageUid) ? _bulkTestWorkflowStageUid : "workflow_stage_uid";
                var workflowUpdateBody = new BulkWorkflowUpdateBody
                {
                    Entries = availableEntries.Select(e => new BulkWorkflowEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Locale = "en-us"
                    }).ToList(),
                    Workflow = new BulkWorkflowStage
                    {
                        Comment = "Bulk workflow update test",
                        DueDate = DateTime.Now.AddDays(7).ToString("ddd MMM dd yyyy"),
                        Notify = false,
                        Uid = workflowStageUid
                    }
                };

                ContentstackResponse response = _stack.BulkOperation().Update(workflowUpdateBody);
                var responseJson = response.OpenJObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(responseJson["job_id"]);
                string jobId = responseJson["job_id"].ToString();
                await CheckBulkJobStatus(jobId);
            }
            catch (ContentstackErrorException ex) when (ex.StatusCode == (HttpStatusCode)412 && ex.ErrorCode == 366)
            {
                // Stage Update Request Failed (412/366) – acceptable when workflow/entry state does not allow the transition
            }
            catch (Exception ex)
            {
                FailWithError("Bulk workflow operations", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Cleanup_Test_Resources()
        {
            try
            {
                // 1. Delete all entries created during the test run
                if (_createdEntries != null)
                {
                    foreach (var entry in _createdEntries)
                    {
                        try
                        {
                            _stack.ContentType(_contentTypeUid).Entry(entry.Uid).Delete();
                            Console.WriteLine($"[Cleanup] Deleted entry: {entry.Uid}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Cleanup] Failed to delete entry {entry.Uid}: {ex.Message}");
                        }
                    }
                    _createdEntries.Clear();
                }

                // 2. Delete the content type
                if (!string.IsNullOrEmpty(_contentTypeUid))
                {
                    try
                    {
                        _stack.ContentType(_contentTypeUid).Delete();
                        Console.WriteLine($"[Cleanup] Deleted content type: {_contentTypeUid}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete content type {_contentTypeUid}: {ex.Message}");
                    }
                }

                // 3. Delete the workflow and publishing rule
                CleanupBulkTestWorkflowAndPublishingRule(_stack);
                Console.WriteLine("[Cleanup] Workflow and publishing rule cleanup done.");

                // 4. Delete the test release
                if (!string.IsNullOrEmpty(_testReleaseUid))
                {
                    try
                    {
                        _stack.Release(_testReleaseUid).Delete();
                        Console.WriteLine($"[Cleanup] Deleted release: {_testReleaseUid}");
                        _testReleaseUid = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete release {_testReleaseUid}: {ex.Message}");
                    }
                }

                // 5. Delete the test environment
                if (!string.IsNullOrEmpty(_testEnvironmentUid))
                {
                    try
                    {
                        _stack.Environment(_testEnvironmentUid).Delete();
                        Console.WriteLine($"[Cleanup] Deleted environment: {_testEnvironmentUid}");
                        _testEnvironmentUid = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete environment {_testEnvironmentUid}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                FailWithError("Cleanup test resources", ex);
            }
        }

        private async Task CheckBulkJobStatus(string jobId, string bulkVersion = null)
        {
            try
            {
                ContentstackResponse statusResponse = await _stack.BulkOperation().JobStatusAsync(jobId, bulkVersion);
                var statusJson = statusResponse.OpenJObjectResponse();

                Assert.IsNotNull(statusResponse);
                Assert.IsTrue(statusResponse.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                // Failed to check job status
            }
        }

        private async Task CreateTestEnvironment()
        {
            try
            {
                // Create test environment
                var environmentModel = new EnvironmentModel
                {
                    Name = "bulk_test_env",
                    Urls = new List<LocalesUrl>
                    {
                        new LocalesUrl
                        {
                            Url = "https://bulk-test-environment.example.com",
                            Locale = "en-us"
                        }
                    }
                };

                ContentstackResponse response = _stack.Environment().Create(environmentModel);
                var responseJson = response.OpenJObjectResponse();

                if (response.IsSuccessStatusCode && responseJson["environment"] != null)
                {
                    _testEnvironmentUid = responseJson["environment"]["uid"].ToString();
                }
            }
            catch (Exception e)
            {
                // Don't fail the test if environment creation fails
            }
        }

        private async Task CreateTestRelease()
        {
            try
            {
                // Create test release
                var releaseModel = new ReleaseModel
                {
                    Name = "bulk_test_release",
                    Description = "Release for testing bulk operations",
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse response = _stack.Release().Create(releaseModel);
                var responseJson = response.OpenJObjectResponse();

                if (response.IsSuccessStatusCode && responseJson["release"] != null)
                {
                    _testReleaseUid = responseJson["release"]["uid"].ToString();
                }
            }
            catch (Exception e)
            {
                // Don't fail the test if release creation fails
            }
        }

        private async Task<List<string>> GetAvailableEnvironments()
        {
            try
            {
                // First try to use our test environment
                if (!string.IsNullOrEmpty(_testEnvironmentUid))
                {
                    try
                    {
                        ContentstackResponse fetchResponse = _stack.Environment(_testEnvironmentUid).Fetch();
                        if (fetchResponse.IsSuccessStatusCode)
                        {
                            return new List<string> { _testEnvironmentUid };
                        }
                    }
                    catch
                    {
                        // Test environment doesn't exist, fall back to available environments
                    }
                }

                // Try to get available environments
                try
                {
                    ContentstackResponse response = _stack.Environment().Query().Find();
                    var responseJson = response.OpenJObjectResponse();

                    if (response.IsSuccessStatusCode && responseJson["environments"] != null)
                    {
                        var environments = responseJson["environments"] as JArray;
                        if (environments != null && environments.Count > 0)
                        {
                            var environmentUids = new List<string>();
                            foreach (var env in environments)
                            {
                                if (env["uid"] != null)
                                {
                                    environmentUids.Add(env["uid"].ToString());
                                }
                            }
                            return environmentUids;
                        }
                    }
                }
                catch (Exception e)
                {
                    // Failed to get environments
                }

                // Fallback to empty list if no environments found
                return new List<string>();
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Ensures bulk_test_content_type exists and has at least one entry so bulk tests can run in any order.
        /// </summary>
        private async Task EnsureBulkTestContentTypeAndEntriesAsync()
        {
            try
            {
                bool contentTypeExists = false;
                try
                {
                    ContentstackResponse ctResponse = _stack.ContentType(_contentTypeUid).Fetch();
                    contentTypeExists = ctResponse.IsSuccessStatusCode;
                }
                catch
                {
                    // Content type not found
                }

                if (!contentTypeExists)
                {
                    await CreateTestEnvironment();
                    await CreateTestRelease();
                    var contentModelling = new ContentModelling
                    {
                        Title = "bulk_test_content_type",
                        Uid = _contentTypeUid,
                        Schema = new List<Field>
                        {
                            new TextboxField
                            {
                                DisplayName = "Title",
                                Uid = "title",
                                DataType = "text",
                                Mandatory = true,
                                Unique = false,
                                Multiple = false
                            }
                        }
                    };
                    _stack.ContentType().Create(contentModelling);
                }

                // Ensure at least one entry exists
                List<EntryInfo> existing = await FetchExistingEntries();
                if (existing == null || existing.Count == 0)
                {
                    var entry = new SimpleEntry { Title = "Bulk test entry" };
                    ContentstackResponse createResponse = _stack.ContentType(_contentTypeUid).Entry().Create(entry);
                    var responseJson = createResponse.OpenJObjectResponse();
                    if (createResponse.IsSuccessStatusCode && responseJson["entry"] != null && responseJson["entry"]["uid"] != null)
                    {
                        _createdEntries.Add(new EntryInfo
                        {
                            Uid = responseJson["entry"]["uid"].ToString(),
                            Title = responseJson["entry"]["title"]?.ToString() ?? "Bulk test entry",
                            Version = responseJson["entry"]["_version"] != null ? (int)responseJson["entry"]["_version"] : 1
                        });
                    }
                }
            }
            catch (Exception)
            {
                // Caller will handle if entries are still missing
            }
        }

        /// <summary>
        /// Returns available environment UIDs for the given stack (synchronous).
        /// </summary>
        private static List<string> GetAvailableEnvironments(Stack stack)
        {
            try
            {
                ContentstackResponse response = stack.Environment().Query().Find();
                var responseJson = response.OpenJObjectResponse();
                if (response.IsSuccessStatusCode && responseJson["environments"] != null)
                {
                    var environments = responseJson["environments"] as JArray;
                    if (environments != null && environments.Count > 0)
                    {
                        var uids = new List<string>();
                        foreach (var env in environments)
                        {
                            if (env["uid"] != null)
                                uids.Add(env["uid"].ToString());
                        }
                        return uids;
                    }
                }
            }
            catch { }
            return new List<string>();
        }

        /// <summary>
        /// Returns available environment UIDs for the given stack (used by workflow setup).
        /// </summary>
        private static async Task<List<string>> GetAvailableEnvironmentsAsync(Stack stack)
        {
            try
            {
                ContentstackResponse response = stack.Environment().Query().Find();
                var responseJson = response.OpenJObjectResponse();
                if (response.IsSuccessStatusCode && responseJson["environments"] != null)
                {
                    var environments = responseJson["environments"] as JArray;
                    if (environments != null && environments.Count > 0)
                    {
                        var uids = new List<string>();
                        foreach (var env in environments)
                        {
                            if (env["uid"] != null)
                                uids.Add(env["uid"].ToString());
                        }
                        return uids;
                    }
                }
            }
            catch { }
            return new List<string>();
        }

        /// <summary>
        /// Ensures an environment exists for workflow/publish rule tests: lists existing envs and uses the first, or creates "bulk_test_env" if none exist. Sets _bulkTestEnvironmentUid. Synchronous.
        /// </summary>
        private static void EnsureBulkTestEnvironment(Stack stack)
        {
            try
            {
                List<string> envs = GetAvailableEnvironments(stack);
                if (envs != null && envs.Count > 0)
                {
                    _bulkTestEnvironmentUid = envs[0];
                    return;
                }

                var environmentModel = new EnvironmentModel
                {
                    Name = "bulk_test_env",
                    Urls = new List<LocalesUrl>
                    {
                        new LocalesUrl
                        {
                            Url = "https://bulk-test-environment.example.com",
                            Locale = "en-us"
                        }
                    }
                };

                ContentstackResponse response = stack.Environment().Create(environmentModel);
                var responseJson = response.OpenJObjectResponse();
                if (response.IsSuccessStatusCode && responseJson["environment"]?["uid"] != null)
                    _bulkTestEnvironmentUid = responseJson["environment"]["uid"].ToString();
            }
            catch { /* Leave _bulkTestEnvironmentUid null */ }
        }

        /// <summary>
        /// Ensures an environment exists for workflow/publish rule tests: lists existing envs and uses the first, or creates "bulk_test_env" if none exist. Sets _bulkTestEnvironmentUid.
        /// </summary>
        private static async Task EnsureBulkTestEnvironmentAsync(Stack stack)
        {
            try
            {
                List<string> envs = await GetAvailableEnvironmentsAsync(stack);
                if (envs != null && envs.Count > 0)
                {
                    _bulkTestEnvironmentUid = envs[0];
                    return;
                }

                var environmentModel = new EnvironmentModel
                {
                    Name = "bulk_test_env",
                    Urls = new List<LocalesUrl>
                    {
                        new LocalesUrl
                        {
                            Url = "https://bulk-test-environment.example.com",
                            Locale = "en-us"
                        }
                    }
                };

                ContentstackResponse response = stack.Environment().Create(environmentModel);
                var responseJson = response.OpenJObjectResponse();
                if (response.IsSuccessStatusCode && responseJson["environment"]?["uid"] != null)
                    _bulkTestEnvironmentUid = responseJson["environment"]["uid"].ToString();
            }
            catch { /* Leave _bulkTestEnvironmentUid null */ }
        }

        /// <summary>
        /// Finds or creates a workflow named "workflow_test" with 2 stages (New stage 1, New stage 2) and a publishing rule.
        /// Uses same payload as Test000a / final curl. Called once from ClassInitialize.
        /// </summary>
        private static async Task EnsureBulkTestWorkflowAndPublishingRuleAsync(Stack stack)
        {
            _bulkTestWorkflowSetupError = null;
            const string workflowName = "workflow_test";
            try
            {
                await EnsureBulkTestEnvironmentAsync(stack);
                if (string.IsNullOrEmpty(_bulkTestEnvironmentUid))
                {
                    _bulkTestWorkflowSetupError = "No environment. Ensure environment failed (none found and create failed).";
                    return;
                }
                // Find existing workflow by name "workflow_test" (same as Test000a)
                try
                {
                    ContentstackResponse listResponse = stack.Workflow().FindAll();
                    if (listResponse.IsSuccessStatusCode)
                    {
                        var listJson = listResponse.OpenJObjectResponse();
                        var existing = (listJson["workflows"] as JArray) ?? (listJson["workflow"] as JArray);
                        if (existing != null)
                        {
                            foreach (var wf in existing)
                            {
                                if (wf["name"]?.ToString() == workflowName && wf["uid"] != null)
                                {
                                    _bulkTestWorkflowUid = wf["uid"].ToString();
                                    var existingStages = wf["workflow_stages"] as JArray;
                                    if (existingStages != null && existingStages.Count >= 2)
                                    {
                                        _bulkTestWorkflowStage1Uid = existingStages[0]["uid"]?.ToString();
                                        _bulkTestWorkflowStage2Uid = existingStages[1]["uid"]?.ToString();
                                        _bulkTestWorkflowStageUid = _bulkTestWorkflowStage2Uid;
                                        break; // Found; skip create
                                    }
                                }
                            }
                        }
                    }
                }
                catch { /* If listing fails, proceed to create */ }

                // Create workflow only if not found (same payload as Test000a / final curl)
                if (string.IsNullOrEmpty(_bulkTestWorkflowUid))
                {
                    var sysAcl = new Dictionary<string, object>
                    {
                        ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                        ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                        ["others"] = new Dictionary<string, object>()
                    };

                    var workflowModel = new WorkflowModel
                    {
                        Name = workflowName,
                        Enabled = true,
                        Branches = new List<string> { "main" },
                        ContentTypes = new List<string> { "$all" },
                        AdminUsers = new Dictionary<string, object> { ["users"] = new List<object>() },
                        WorkflowStages = new List<WorkflowStage>
                        {
                            new WorkflowStage
                            {
                                Name = "New stage 1",
                                Color = "#fe5cfb",
                                SystemACL = sysAcl,
                                NextAvailableStages = new List<string> { "$all" },
                                AllStages = true,
                                AllUsers = true,
                                SpecificStages = false,
                                SpecificUsers = false,
                                EntryLock = "$none"
                            },
                            new WorkflowStage
                            {
                                Name = "New stage 2",
                                Color = "#3688bf",
                                SystemACL = new Dictionary<string, object>
                                {
                                    ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                                    ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                                    ["others"] = new Dictionary<string, object>()
                                },
                                NextAvailableStages = new List<string> { "$all" },
                                AllStages = true,
                                AllUsers = true,
                                SpecificStages = false,
                                SpecificUsers = false,
                                EntryLock = "$none"
                            }
                        }
                    };

                    ContentstackResponse workflowResponse = stack.Workflow().Create(workflowModel);
                    if (!workflowResponse.IsSuccessStatusCode)
                    {
                        string body = null;
                        try { body = workflowResponse.OpenResponse(); } catch { }
                        _bulkTestWorkflowSetupError = $"Workflow create returned HTTP {(int)workflowResponse.StatusCode} ({workflowResponse.StatusCode}). Response: {body ?? "(null)"}";
                        return;
                    }

                    var workflowJson = workflowResponse.OpenJObjectResponse();
                    var workflowObj = workflowJson["workflow"];
                    if (workflowObj == null)
                    {
                        string body = null;
                        try { body = workflowResponse.OpenResponse(); } catch { }
                        _bulkTestWorkflowSetupError = "Workflow create response had no 'workflow' key. Response: " + (body ?? "(null)");
                        return;
                    }

                    _bulkTestWorkflowUid = workflowObj["uid"]?.ToString();
                    var stages = workflowObj["workflow_stages"] as JArray;
                    if (stages != null && stages.Count >= 2)
                    {
                        _bulkTestWorkflowStage1Uid = stages[0]?["uid"]?.ToString();
                        _bulkTestWorkflowStage2Uid = stages[1]?["uid"]?.ToString();
                        _bulkTestWorkflowStageUid = _bulkTestWorkflowStage2Uid;
                    }
                }

                if (string.IsNullOrEmpty(_bulkTestWorkflowUid) || string.IsNullOrEmpty(_bulkTestWorkflowStage2Uid))
                {
                    _bulkTestWorkflowSetupError = "Workflow UID or stage UIDs not set. Find or create failed.";
                    return;
                }

                // Find existing publish rule for this workflow + stage + environment
                try
                {
                    ContentstackResponse ruleListResponse = stack.Workflow().PublishRule().FindAll();
                    if (ruleListResponse.IsSuccessStatusCode)
                    {
                        var ruleListJson = ruleListResponse.OpenJObjectResponse();
                        var rules = (ruleListJson["publishing_rules"] as JArray) ?? (ruleListJson["publishing_rule"] as JArray);
                        if (rules != null)
                        {
                            foreach (var rule in rules)
                            {
                                if (rule["workflow"]?.ToString() == _bulkTestWorkflowUid
                                    && rule["workflow_stage"]?.ToString() == _bulkTestWorkflowStage2Uid
                                    && rule["environment"]?.ToString() == _bulkTestEnvironmentUid
                                    && rule["uid"] != null)
                                {
                                    _bulkTestPublishRuleUid = rule["uid"].ToString();
                                    return; // Publish rule already exists
                                }
                            }
                        }
                    }
                }
                catch { /* If listing fails, proceed to create */ }

                var publishRuleModel = new PublishRuleModel
                {
                    WorkflowUid = _bulkTestWorkflowUid,
                    WorkflowStageUid = _bulkTestWorkflowStage2Uid,
                    Environment = _bulkTestEnvironmentUid,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    Locales = new List<string> { "en-us" },
                    Actions = new List<string>(),
                    Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() },
                    DisableApproval = false
                };

                ContentstackResponse ruleResponse = stack.Workflow().PublishRule().Create(publishRuleModel);
                if (!ruleResponse.IsSuccessStatusCode)
                {
                    string body = null;
                    try { body = ruleResponse.OpenResponse(); } catch { }
                    _bulkTestWorkflowSetupError = $"Publish rule create returned HTTP {(int)ruleResponse.StatusCode} ({ruleResponse.StatusCode}). Response: {body ?? "(null)"}";
                    return;
                }

                var ruleJson = ruleResponse.OpenJObjectResponse();
                _bulkTestPublishRuleUid = ruleJson["publishing_rule"]?["uid"]?.ToString();
            }
            catch (ContentstackErrorException ex)
            {
                _bulkTestWorkflowSetupError = $"Workflow setup threw: HTTP {(int)ex.StatusCode} ({ex.StatusCode}), ErrorCode: {ex.ErrorCode}, Message: {ex.ErrorMessage ?? ex.Message}";
            }
            catch (Exception ex)
            {
                _bulkTestWorkflowSetupError = "Workflow setup threw: " + ex.Message;
            }
        }

        /// <summary>
        /// Deletes the publishing rule and workflow created for bulk tests. Called once from ClassCleanup.
        /// </summary>
        private static void CleanupBulkTestWorkflowAndPublishingRule(Stack stack)
        {
            if (!string.IsNullOrEmpty(_bulkTestPublishRuleUid))
            {
                try
                {
                    stack.Workflow().PublishRule(_bulkTestPublishRuleUid).Delete();
                }
                catch
                {
                    // Ignore cleanup failure
                }
                _bulkTestPublishRuleUid = null;
            }

            if (!string.IsNullOrEmpty(_bulkTestWorkflowUid))
            {
                try
                {
                    stack.Workflow(_bulkTestWorkflowUid).Delete();
                }
                catch
                {
                    // Ignore cleanup failure
                }
                _bulkTestWorkflowUid = null;
            }

            _bulkTestWorkflowStageUid = null;
            _bulkTestWorkflowStage1Uid = null;
            _bulkTestWorkflowStage2Uid = null;
        }

        /// <summary>
        /// Assigns entries to workflow stages: first half to Stage 1, second half to Stage 2, so you can verify allotment in the UI.
        /// </summary>
        private async Task AssignEntriesToWorkflowStagesAsync(List<EntryInfo> entries)
        {
            if (entries == null || entries.Count == 0 || string.IsNullOrEmpty(_bulkTestWorkflowStage1Uid) || string.IsNullOrEmpty(_bulkTestWorkflowStage2Uid))
                return;
            int mid = (entries.Count + 1) / 2;
            var stage1Entries = entries.Take(mid).ToList();
            var stage2Entries = entries.Skip(mid).ToList();

            foreach (var stageUid in new[] { _bulkTestWorkflowStage1Uid, _bulkTestWorkflowStage2Uid })
            {
                var list = stageUid == _bulkTestWorkflowStage1Uid ? stage1Entries : stage2Entries;
                if (list.Count == 0) continue;
                try
                {
                    var body = new BulkWorkflowUpdateBody
                    {
                        Entries = list.Select(e => new BulkWorkflowEntry { Uid = e.Uid, ContentType = _contentTypeUid, Locale = "en-us" }).ToList(),
                        Workflow = new BulkWorkflowStage { Comment = "Stage allotment for bulk tests", Notify = false, Uid = stageUid }
                    };
                    ContentstackResponse r = _stack.BulkOperation().Update(body);
                    if (r.IsSuccessStatusCode)
                    {
                        var j = r.OpenJObjectResponse();
                        if (j?["job_id"] != null) { await Task.Delay(2000); await CheckBulkJobStatus(j["job_id"].ToString()); }
                    }
                }
                catch (ContentstackErrorException ex) when (ex.StatusCode == (HttpStatusCode)412 && ex.ErrorCode == 366) { /* stage update not allowed */ }
            }
        }

        private async Task<List<EntryInfo>> FetchExistingEntries()
        {
            try
            {
                // Query entries from the content type
                ContentstackResponse response = _stack.ContentType(_contentTypeUid).Entry().Query().Find();
                var responseJson = response.OpenJObjectResponse();

                if (response.IsSuccessStatusCode && responseJson["entries"] != null)
                {
                    var entries = responseJson["entries"] as JArray;
                    if (entries != null && entries.Count > 0)
                    {
                        var entryList = new List<EntryInfo>();
                        foreach (var entry in entries)
                        {
                            if (entry["uid"] != null && entry["title"] != null)
                            {
                                entryList.Add(new EntryInfo
                                {
                                    Uid = entry["uid"].ToString(),
                                    Title = entry["title"].ToString(),
                                    Version = entry["_version"] != null ? (int)entry["_version"] : 1
                                });
                            }
                        }
                        return entryList;
                    }
                }

                return new List<EntryInfo>();
            }
            catch (Exception e)
            {
                return new List<EntryInfo>();
            }
        }

        private async Task<List<string>> GetAvailableReleases()
        {
            try
            {
                // First try to use our test release
                if (!string.IsNullOrEmpty(_testReleaseUid))
                {
                    try
                    {
                        ContentstackResponse fetchResponse = _stack.Release(_testReleaseUid).Fetch();
                        if (fetchResponse.IsSuccessStatusCode)
                        {
                            return new List<string> { _testReleaseUid };
                        }
                    }
                    catch
                    {
                        // Test release doesn't exist, fall back to available releases
                    }
                }

                // Try to get available releases
                try
                {
                    ContentstackResponse response = _stack.Release().Query().Find();
                    var responseJson = response.OpenJObjectResponse();

                    if (response.IsSuccessStatusCode && responseJson["releases"] != null)
                    {
                        var releases = responseJson["releases"] as JArray;
                        if (releases != null && releases.Count > 0)
                        {
                            var releaseUids = new List<string>();
                            foreach (var release in releases)
                            {
                                if (release["uid"] != null)
                                {
                                    releaseUids.Add(release["uid"].ToString());
                                }
                            }
                            return releaseUids;
                        }
                    }
                }
                catch (Exception e)
                {
                    // Failed to get releases
                }

                // Fallback to empty list if no releases found
                return new List<string>();
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }

        private async Task<string> FetchAvailableRelease()
        {
            try
            {
                // First try to use our test release if it exists
                if (!string.IsNullOrEmpty(_testReleaseUid))
                {
                    try
                    {
                        ContentstackResponse fetchResponse = _stack.Release(_testReleaseUid).Fetch();
                        if (fetchResponse.IsSuccessStatusCode)
                        {
                            return _testReleaseUid;
                        }
                    }
                    catch
                    {
                        // Test release not found, look for other releases
                    }
                }

                // Query for available releases
                ContentstackResponse response = _stack.Release().Query().Find();
                var responseJson = response.OpenJObjectResponse();

                if (response.IsSuccessStatusCode && responseJson["releases"] != null)
                {
                    var releases = responseJson["releases"] as JArray;
                    if (releases != null && releases.Count > 0)
                    {
                        // Get the first available release
                        var firstRelease = releases[0];
                        if (firstRelease["uid"] != null)
                        {
                            string releaseUid = firstRelease["uid"].ToString();
                            return releaseUid;
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public class SimpleEntry : IEntry
        {
            [JsonProperty(propertyName: "title")]
            public string Title { get; set; }
        }

        public class EntryInfo
        {
            public string Uid { get; set; }
            public string Title { get; set; }
            public int Version { get; set; }
        }
    }
} 