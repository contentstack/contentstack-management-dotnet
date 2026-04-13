using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    /// <summary>
    /// Workflow integration tests covering CRUD operations, publish rules, and error scenarios.
    /// API requires 2–20 workflow stages per workflow; helpers enforce that minimum.
    /// Tests are independent with unique naming to avoid conflicts. Cleanup is best-effort to maintain stack state.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class Contentstack020_WorkflowTest
    {
        private static ContentstackClient _client;
        private Stack _stack;
        
        // Test resource tracking for cleanup
        private List<string> _createdWorkflowUids = new List<string>();
        private List<string> _createdPublishRuleUids = new List<string>();
        private string _testEnvironmentUid;

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
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Best-effort cleanup of created resources
            CleanupCreatedResources();
        }

        /// <summary>
        /// Fails the test with a clear message from ContentstackErrorException or generic exception.
        /// </summary>
        private static void FailWithError(string operation, Exception ex)
        {
            if (ex is ContentstackErrorException cex)
                AssertLogger.Fail($"{operation} failed. HTTP {(int)cex.StatusCode} ({cex.StatusCode}). ErrorCode: {cex.ErrorCode}. Message: {cex.ErrorMessage ?? cex.Message}");
            else
                AssertLogger.Fail($"{operation} failed: {ex.Message}");
        }

        /// <summary>
        /// API may return 404 or 422 for missing or invalid workflow UIDs depending on endpoint.
        /// </summary>
        private static void AssertMissingWorkflowStatus(HttpStatusCode statusCode, string assertionName)
        {
            AssertLogger.IsTrue(
                statusCode == HttpStatusCode.NotFound || statusCode == HttpStatusCode.UnprocessableEntity,
                $"Expected 404 or 422 for missing workflow, got {(int)statusCode} ({statusCode})",
                assertionName);
        }

        /// <summary>
        /// Creates a test workflow model with specified name and stage count.
        /// Contentstack API requires between 2 and 20 workflow stages.
        /// </summary>
        private WorkflowModel CreateTestWorkflowModel(string name, int stageCount = 2)
        {
            if (stageCount < 2 || stageCount > 20)
                throw new ArgumentOutOfRangeException(nameof(stageCount), "API requires workflow_stages count between 2 and 20.");
            var stages = GenerateTestStages(stageCount);
            return new WorkflowModel
            {
                Name = name,
                Enabled = true,
                Branches = new List<string> { "main" },
                ContentTypes = new List<string> { "$all" },
                AdminUsers = new Dictionary<string, object> { ["users"] = new List<object>() },
                WorkflowStages = stages
            };
        }

        /// <summary>
        /// Generates test workflow stages with unique names and standard configurations.
        /// </summary>
        private List<WorkflowStage> GenerateTestStages(int count)
        {
            var stages = new List<WorkflowStage>();
            var colors = new[] { "#fe5cfb", "#3688bf", "#28a745", "#ffc107", "#dc3545" };
            
            for (int i = 0; i < count; i++)
            {
                var sysAcl = new Dictionary<string, object>
                {
                    ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                    ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                    ["others"] = new Dictionary<string, object>()
                };

                stages.Add(new WorkflowStage
                {
                    Name = $"Test Stage {i + 1}",
                    Color = colors[i % colors.Length],
                    SystemACL = sysAcl,
                    NextAvailableStages = new List<string> { "$all" },
                    AllStages = true,
                    AllUsers = true,
                    SpecificStages = false,
                    SpecificUsers = false,
                    EntryLock = "$none"
                });
            }
            return stages;
        }

        /// <summary>
        /// Creates a test publish rule model for the given workflow and stage UIDs.
        /// </summary>
        private PublishRuleModel CreateTestPublishRuleModel(string workflowUid, string stageUid, string environmentUid)
        {
            return new PublishRuleModel
            {
                WorkflowUid = workflowUid,
                WorkflowStageUid = stageUid,
                Environment = environmentUid,
                Branches = new List<string> { "main" },
                ContentTypes = new List<string> { "$all" },
                Locales = new List<string> { "en-us" },
                Actions = new List<string>(),
                Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() },
                DisableApproval = false
            };
        }

        /// <summary>
        /// Ensures a test environment exists for publish rule tests.
        /// </summary>
        private async Task EnsureTestEnvironmentAsync()
        {
            if (!string.IsNullOrEmpty(_testEnvironmentUid))
                return;

            try
            {
                // Try to find existing environments first
                ContentstackResponse envResponse = _stack.Environment().Query().Find();
                if (envResponse.IsSuccessStatusCode)
                {
                    var envJson = envResponse.OpenJObjectResponse();
                    var environments = envJson["environments"] as JArray;
                    if (environments != null && environments.Count > 0)
                    {
                        _testEnvironmentUid = environments[0]["uid"]?.ToString();
                        return;
                    }
                }

                // Create test environment if none exist
                var environmentModel = new EnvironmentModel
                {
                    Name = $"test_workflow_env_{Guid.NewGuid():N}",
                    Urls = new List<LocalesUrl>
                    {
                        new LocalesUrl
                        {
                            Url = "https://test-workflow-environment.example.com",
                            Locale = "en-us"
                        }
                    }
                };

                ContentstackResponse response = _stack.Environment().Create(environmentModel);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = response.OpenJObjectResponse();
                    _testEnvironmentUid = responseJson["environment"]?["uid"]?.ToString();
                }
            }
            catch (Exception)
            {
                // Environment creation failed - tests will skip or use fallback
            }
        }

        /// <summary>
        /// Returns the UID of the first content type on the stack, or null if the query fails.
        /// GetPublishRule(contentType) requires a real content-type UID; <c>$all</c> is valid on workflows/publish rules but not in that path.
        /// </summary>
        private string TryGetFirstContentTypeUidFromStack()
        {
            try
            {
                ContentstackResponse response = _stack.ContentType().Query().Find();
                if (!response.IsSuccessStatusCode)
                    return null;
                var model = response.OpenTResponse<ContentTypesModel>();
                return model?.Modellings?.FirstOrDefault()?.Uid;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Best-effort cleanup of created test resources.
        /// </summary>
        private void CleanupCreatedResources()
        {
            // Cleanup publish rules first (they depend on workflows)
            foreach (var ruleUid in _createdPublishRuleUids.ToList())
            {
                try
                {
                    _stack.Workflow().PublishRule(ruleUid).Delete();
                    _createdPublishRuleUids.Remove(ruleUid);
                }
                catch
                {
                    // Ignore cleanup failures
                }
            }

            // Then cleanup workflows
            foreach (var workflowUid in _createdWorkflowUids.ToList())
            {
                try
                {
                    _stack.Workflow(workflowUid).Delete();
                    _createdWorkflowUids.Remove(workflowUid);
                }
                catch
                {
                    // Ignore cleanup failures
                }
            }
        }

        // ==== HAPPY PATH TESTS (001-015) ====

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Workflow_With_Minimum_Required_Stages()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateWorkflowWithMinimumRequiredStages");
            try
            {
                // Arrange — API enforces min 2 stages (max 20)
                string workflowName = $"test_min_stages_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);

                // Act
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowCreateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow create failed with status {(int)response.StatusCode}", "workflowCreateSuccess");
                AssertLogger.IsNotNull(responseJson["workflow"], "workflowObject");
                AssertLogger.IsNotNull(responseJson["workflow"]["uid"], "workflowUid");
                
                string workflowUid = responseJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);
                TestOutputLogger.LogContext("WorkflowUid", workflowUid);
                
                var stages = responseJson["workflow"]["workflow_stages"] as JArray;
                AssertLogger.AreEqual(2, stages?.Count, "Expected exactly 2 stages (API minimum)", "stageCount");
                AssertLogger.AreEqual(workflowName, responseJson["workflow"]["name"]?.ToString(), "workflowName");
            }
            catch (Exception ex)
            {
                FailWithError("Create workflow with minimum required stages", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Create_Workflow_With_Multiple_Stages()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateWorkflowWithMultipleStages");
            try
            {
                // Arrange
                string workflowName = $"test_multi_stage_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 3);

                // Act
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowCreateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow create failed with status {(int)response.StatusCode}", "workflowCreateSuccess");
                AssertLogger.IsNotNull(responseJson["workflow"], "workflowObject");
                
                string workflowUid = responseJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);
                TestOutputLogger.LogContext("WorkflowUid", workflowUid);
                
                var stages = responseJson["workflow"]["workflow_stages"] as JArray;
                AssertLogger.AreEqual(3, stages?.Count, "Expected exactly 3 stages", "stageCount");
                
                // Verify all stages were created with correct names
                for (int i = 0; i < 3; i++)
                {
                    AssertLogger.AreEqual($"Test Stage {i + 1}", stages[i]["name"]?.ToString(), $"stage{i + 1}Name");
                }
            }
            catch (Exception ex)
            {
                FailWithError("Create workflow with multiple stages", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_Single_Workflow_By_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchSingleWorkflowByUid");
            try
            {
                // Arrange - Create a workflow first
                string workflowName = $"test_fetch_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Act
                ContentstackResponse response = _stack.Workflow(workflowUid).Fetch();
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowFetchResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow fetch failed with status {(int)response.StatusCode}", "workflowFetchSuccess");
                AssertLogger.IsNotNull(responseJson["workflow"], "workflowObject");
                AssertLogger.AreEqual(workflowUid, responseJson["workflow"]["uid"]?.ToString(), "workflowUid");
                AssertLogger.AreEqual(workflowName, responseJson["workflow"]["name"]?.ToString(), "workflowName");
                
                var stages = responseJson["workflow"]["workflow_stages"] as JArray;
                AssertLogger.AreEqual(2, stages?.Count, "Expected 2 stages", "stageCount");
                TestOutputLogger.LogContext("FetchedWorkflowUid", workflowUid);
            }
            catch (Exception ex)
            {
                FailWithError("Fetch single workflow by UID", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Fetch_All_Workflows()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAllWorkflows");
            try
            {
                // Act
                ContentstackResponse response = _stack.Workflow().FindAll();
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowFindAllResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow FindAll failed with status {(int)response.StatusCode}", "workflowFindAllSuccess");
                
                // Response should contain workflows array (even if empty)
                var workflows = (responseJson["workflows"] as JArray) ?? (responseJson["workflow"] as JArray);
                AssertLogger.IsNotNull(workflows, "workflowsArray");
                
                TestOutputLogger.LogContext("WorkflowCount", workflows.Count.ToString());
            }
            catch (Exception ex)
            {
                FailWithError("Fetch all workflows", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_Workflow_Properties()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateWorkflowProperties");
            try
            {
                // Arrange - Create a workflow first
                string originalName = $"test_update_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(originalName, 2);
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Prepare update
                string updatedName = $"updated_workflow_{Guid.NewGuid():N}";
                workflowModel.Name = updatedName;
                workflowModel.Enabled = false; // Change enabled status

                // Act
                ContentstackResponse response = _stack.Workflow(workflowUid).Update(workflowModel);
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowUpdateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow update failed with status {(int)response.StatusCode}", "workflowUpdateSuccess");
                AssertLogger.IsNotNull(responseJson["workflow"], "workflowObject");
                AssertLogger.AreEqual(updatedName, responseJson["workflow"]["name"]?.ToString(), "updatedWorkflowName");
                AssertLogger.AreEqual(false, responseJson["workflow"]["enabled"]?.Value<bool>(), "updatedEnabledStatus");
                
                TestOutputLogger.LogContext("UpdatedWorkflowUid", workflowUid);
            }
            catch (Exception ex)
            {
                FailWithError("Update workflow properties", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Add_New_Stage_To_Existing_Workflow()
        {
            TestOutputLogger.LogContext("TestScenario", "AddNewStageToExistingWorkflow");
            try
            {
                // Arrange - Create with 2 stages (API minimum), then add a third
                string workflowName = $"test_add_stage_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                workflowModel.WorkflowStages = GenerateTestStages(3);

                // Act
                ContentstackResponse response = _stack.Workflow(workflowUid).Update(workflowModel);
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowUpdateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow update failed with status {(int)response.StatusCode}", "workflowUpdateSuccess");
                
                var stages = responseJson["workflow"]["workflow_stages"] as JArray;
                AssertLogger.AreEqual(3, stages?.Count, "Expected 3 stages after update", "stageCount");
                
                TestOutputLogger.LogContext("WorkflowWithNewStageUid", workflowUid);
            }
            catch (Exception ex)
            {
                FailWithError("Add new stage to existing workflow", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Enable_Workflow_Successfully()
        {
            TestOutputLogger.LogContext("TestScenario", "EnableWorkflowSuccessfully");
            try
            {
                // Arrange - Create a disabled workflow
                string workflowName = $"test_enable_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.Enabled = false;
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Act
                ContentstackResponse response = _stack.Workflow(workflowUid).Enable();

                // Assert
                AssertLogger.IsNotNull(response, "workflowEnableResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow enable failed with status {(int)response.StatusCode}", "workflowEnableSuccess");
                
                TestOutputLogger.LogContext("EnabledWorkflowUid", workflowUid);
            }
            catch (Exception ex)
            {
                FailWithError("Enable workflow", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Disable_Workflow_Successfully()
        {
            TestOutputLogger.LogContext("TestScenario", "DisableWorkflowSuccessfully");
            try
            {
                // Arrange - Create an enabled workflow
                string workflowName = $"test_disable_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.Enabled = true;
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Act
                ContentstackResponse response = _stack.Workflow(workflowUid).Disable();

                // Assert
                AssertLogger.IsNotNull(response, "workflowDisableResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow disable failed with status {(int)response.StatusCode}", "workflowDisableSuccess");
                
                TestOutputLogger.LogContext("DisabledWorkflowUid", workflowUid);
            }
            catch (Exception ex)
            {
                FailWithError("Disable workflow", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Create_Publish_Rule_For_Workflow_Stage()
        {
            TestOutputLogger.LogContext("TestScenario", "CreatePublishRuleForWorkflowStage");
            try
            {
                // Arrange - Create workflow and ensure environment exists
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required for publish rule tests", "testEnvironmentUid");

                string workflowName = $"test_publish_rule_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JArray;
                string stageUid = stages[1]["uid"].ToString(); // Use second stage

                // Create publish rule
                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);

                // Act
                ContentstackResponse response = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "publishRuleCreateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Publish rule create failed with status {(int)response.StatusCode}", "publishRuleCreateSuccess");
                AssertLogger.IsNotNull(responseJson["publishing_rule"], "publishingRuleObject");
                
                string publishRuleUid = responseJson["publishing_rule"]["uid"].ToString();
                _createdPublishRuleUids.Add(publishRuleUid);
                
                AssertLogger.AreEqual(workflowUid, responseJson["publishing_rule"]["workflow"]?.ToString(), "publishRuleWorkflowUid");
                AssertLogger.AreEqual(stageUid, responseJson["publishing_rule"]["workflow_stage"]?.ToString(), "publishRuleStageUid");
                
                TestOutputLogger.LogContext("PublishRuleUid", publishRuleUid);
            }
            catch (Exception ex)
            {
                FailWithError("Create publish rule for workflow stage", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Fetch_All_Publish_Rules()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAllPublishRules");
            try
            {
                // Act
                ContentstackResponse response = _stack.Workflow().PublishRule().FindAll();
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "publishRuleFindAllResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Publish rule FindAll failed with status {(int)response.StatusCode}", "publishRuleFindAllSuccess");
                
                // Response should contain publishing_rules array (even if empty)
                var rules = (responseJson["publishing_rules"] as JArray) ?? (responseJson["publishing_rule"] as JArray);
                AssertLogger.IsNotNull(rules, "publishingRulesArray");
                
                TestOutputLogger.LogContext("PublishRuleCount", rules.Count.ToString());
            }
            catch (Exception ex)
            {
                FailWithError("Fetch all publish rules", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test011_Should_Get_Publish_Rules_By_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "GetPublishRulesByContentType");
            try
            {
                // Arrange - Create workflow and publish rule first
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required", "testEnvironmentUid");

                string contentTypeUid = TryGetFirstContentTypeUidFromStack();
                AssertLogger.IsFalse(string.IsNullOrEmpty(contentTypeUid), "Stack must expose at least one content type for GetPublishRule by content type", "contentTypeUid");

                string workflowName = $"test_content_type_rule_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                publishRuleModel.ContentTypes = new List<string> { contentTypeUid };

                ContentstackResponse ruleResponse = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var ruleJson = ruleResponse.OpenJObjectResponse();
                string publishRuleUid = ruleJson["publishing_rule"]["uid"].ToString();
                _createdPublishRuleUids.Add(publishRuleUid);

                // Act
                var collection = new ParameterCollection();
                ContentstackResponse response = _stack.Workflow(workflowUid).GetPublishRule(contentTypeUid, collection);

                // Assert
                AssertLogger.IsNotNull(response, "getPublishRuleResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Get publish rule by content type failed with status {(int)response.StatusCode}", "getPublishRuleSuccess");
                
                TestOutputLogger.LogContext("ContentTypeFilter", contentTypeUid);
            }
            catch (Exception ex)
            {
                FailWithError("Get publish rules by content type", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test012_Should_Update_Publish_Rule()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdatePublishRule");
            try
            {
                // Arrange - Create workflow and publish rule first
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required", "testEnvironmentUid");

                string workflowName = $"test_update_rule_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                ContentstackResponse ruleResponse = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var ruleJson = ruleResponse.OpenJObjectResponse();
                string publishRuleUid = ruleJson["publishing_rule"]["uid"].ToString();
                _createdPublishRuleUids.Add(publishRuleUid);

                // Update the publish rule (locales must exist on the stack; integration stack typically has en-us)
                publishRuleModel.DisableApproval = true;
                publishRuleModel.Locales = new List<string> { "en-us" };

                // Act
                ContentstackResponse response = _stack.Workflow().PublishRule(publishRuleUid).Update(publishRuleModel);
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "publishRuleUpdateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Publish rule update failed with status {(int)response.StatusCode}", "publishRuleUpdateSuccess");
                AssertLogger.AreEqual(true, responseJson["publishing_rule"]["disable_approver_publishing"]?.Value<bool>(), "updatedDisableApproval");
                
                TestOutputLogger.LogContext("UpdatedPublishRuleUid", publishRuleUid);
            }
            catch (Exception ex)
            {
                FailWithError("Update publish rule", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Fetch_Workflows_With_Include_Parameters()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchWorkflowsWithIncludeParameters");
            try
            {
                // Act
                var collection = new ParameterCollection();
                collection.Add("include_count", "true");
                collection.Add("include_publish_details", "true");
                
                ContentstackResponse response = _stack.Workflow().FindAll(collection);
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowFindAllWithIncludeResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow FindAll with include failed with status {(int)response.StatusCode}", "workflowFindAllWithIncludeSuccess");
                
                var workflows = (responseJson["workflows"] as JArray) ?? (responseJson["workflow"] as JArray);
                AssertLogger.IsNotNull(workflows, "workflowsArray");
                
                TestOutputLogger.LogContext("IncludeParameters", "include_count,include_publish_details");
            }
            catch (Exception ex)
            {
                FailWithError("Fetch workflows with include parameters", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Fetch_Workflows_With_Pagination()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchWorkflowsWithPagination");
            try
            {
                // Act
                var collection = new ParameterCollection();
                collection.Add("limit", "5");
                collection.Add("skip", "0");
                
                ContentstackResponse response = _stack.Workflow().FindAll(collection);
                var responseJson = response.OpenJObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowPaginationResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow FindAll with pagination failed with status {(int)response.StatusCode}", "workflowPaginationSuccess");
                
                var workflows = (responseJson["workflows"] as JArray) ?? (responseJson["workflow"] as JArray);
                AssertLogger.IsNotNull(workflows, "workflowsArray");
                
                TestOutputLogger.LogContext("PaginationParams", "limit=5,skip=0");
            }
            catch (Exception ex)
            {
                FailWithError("Fetch workflows with pagination", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test015_Should_Delete_Publish_Rule_Successfully()
        {
            TestOutputLogger.LogContext("TestScenario", "DeletePublishRuleSuccessfully");
            try
            {
                // Arrange - Create workflow and publish rule first
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required", "testEnvironmentUid");

                string workflowName = $"test_delete_rule_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                ContentstackResponse ruleResponse = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var ruleJson = ruleResponse.OpenJObjectResponse();
                string publishRuleUid = ruleJson["publishing_rule"]["uid"].ToString();

                // Act
                ContentstackResponse response = _stack.Workflow().PublishRule(publishRuleUid).Delete();

                // Assert
                AssertLogger.IsNotNull(response, "publishRuleDeleteResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Publish rule delete failed with status {(int)response.StatusCode}", "publishRuleDeleteSuccess");
                
                TestOutputLogger.LogContext("DeletedPublishRuleUid", publishRuleUid);
                
                // Remove from cleanup list since it's already deleted
                _createdPublishRuleUids.Remove(publishRuleUid);
            }
            catch (Exception ex)
            {
                FailWithError("Delete publish rule", ex);
            }
        }

        // ==== NEGATIVE PATH TESTS (101-110) ====

        [TestMethod]
        [DoNotParallelize]
        public void Test101_Should_Fail_Create_Workflow_With_Missing_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithMissingName");
            try
            {
                // Arrange - Create workflow model without name
                var workflowModel = new WorkflowModel
                {
                    Name = null, // Missing required field
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    WorkflowStages = GenerateTestStages(2)
                };

                // Act & Assert
                AssertLogger.ThrowsException<ContentstackErrorException>(() =>
                {
                    ContentstackResponse response = _stack.Workflow().Create(workflowModel);
                    if (!response.IsSuccessStatusCode)
                    {
                        // Parse error details and throw exception for validation
                        throw new ContentstackErrorException { StatusCode = response.StatusCode, ErrorMessage = "Validation failed" };
                    }
                }, "createWorkflowWithMissingName");
                
                TestOutputLogger.LogContext("ValidationError", "MissingName");
            }
            catch (ContentstackErrorException cex)
            {
                // Expected validation error
                AssertLogger.IsTrue((int)cex.StatusCode >= 400 && (int)cex.StatusCode < 500, "Expected 4xx status code for validation error", "validationErrorStatusCode");
                TestOutputLogger.LogContext("ExpectedValidationError", cex.Message);
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for missing workflow name", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test102_Should_Fail_Create_Workflow_With_Invalid_Stage_Data()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithInvalidStageData");
            try
            {
                // Arrange - Create workflow with invalid stage configuration
                var workflowModel = new WorkflowModel
                {
                    Name = $"test_invalid_stage_workflow_{Guid.NewGuid():N}",
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    WorkflowStages = new List<WorkflowStage>
                    {
                        new WorkflowStage
                        {
                            Name = null, // Invalid: missing stage name
                            Color = "invalid_color", // Invalid color format
                            SystemACL = null // Missing ACL
                        }
                    }
                };

                // Act
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);

                // Assert - Should fail with validation error
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Expected workflow creation to fail with invalid stage data", "invalidStageCreationFailed");
                AssertLogger.IsTrue((int)response.StatusCode >= 400 && (int)response.StatusCode < 500, "Expected 4xx status code", "validationErrorStatusCode");
                
                TestOutputLogger.LogContext("ValidationError", "InvalidStageData");
            }
            catch (Exception ex)
            {
                // Some validation errors might be thrown as exceptions
                if (ex is ContentstackErrorException cex)
                {
                    AssertLogger.IsTrue((int)cex.StatusCode >= 400 && (int)cex.StatusCode < 500, "Expected 4xx status code for validation error", "validationErrorStatusCode");
                    TestOutputLogger.LogContext("ExpectedValidationError", cex.Message);
                }
                else
                {
                    FailWithError("Expected validation error for invalid stage data", ex);
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test103_Should_Fail_Create_Duplicate_Workflow_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateDuplicateWorkflowName");
            try
            {
                // Arrange - Create first workflow
                string duplicateName = $"test_duplicate_workflow_{Guid.NewGuid():N}";
                var workflowModel1 = CreateTestWorkflowModel(duplicateName, 2);
                
                ContentstackResponse response1 = _stack.Workflow().Create(workflowModel1);
                AssertLogger.IsTrue(response1.IsSuccessStatusCode, "First workflow creation should succeed", "firstWorkflowCreated");
                
                var responseJson1 = response1.OpenJObjectResponse();
                string workflowUid1 = responseJson1["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid1);

                // Create second workflow with same name
                var workflowModel2 = CreateTestWorkflowModel(duplicateName, 2);

                // Act & assert — duplicate name may return non-success response or throw ContentstackErrorException (422)
                try
                {
                    ContentstackResponse response2 = _stack.Workflow().Create(workflowModel2);
                    AssertLogger.IsFalse(response2.IsSuccessStatusCode, "Expected duplicate workflow creation to fail", "duplicateWorkflowCreationFailed");
                    AssertLogger.IsTrue((int)response2.StatusCode == 409 || (int)response2.StatusCode == 422, "Expected 409 Conflict or 422 Unprocessable Entity", "conflictErrorStatusCode");
                }
                catch (ContentstackErrorException cex)
                {
                    AssertLogger.IsTrue((int)cex.StatusCode == 409 || (int)cex.StatusCode == 422, "Expected 409 Conflict or 422 Unprocessable Entity", "conflictErrorStatusCode");
                }
                
                TestOutputLogger.LogContext("ConflictError", "DuplicateWorkflowName");
            }
            catch (Exception ex)
            {
                FailWithError("Expected conflict error for duplicate workflow name", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test104_Should_Fail_Fetch_NonExistent_Workflow()
        {
            TestOutputLogger.LogContext("TestScenario", "FailFetchNonExistentWorkflow");
            try
            {
                // Arrange
                string nonExistentUid = $"non_existent_workflow_{Guid.NewGuid():N}";

                // Act
                ContentstackResponse response = _stack.Workflow(nonExistentUid).Fetch();

                // Assert — API often returns 422 for invalid/missing workflow UID
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Expected fetch to fail for non-existent workflow", "fetchNonExistentFailed");
                AssertMissingWorkflowStatus(response.StatusCode, "missingWorkflowStatusCode");
                
                TestOutputLogger.LogContext("NotFoundOrUnprocessable", nonExistentUid);
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingWorkflowStatus(cex.StatusCode, "missingWorkflowStatusCode");
                TestOutputLogger.LogContext("ExpectedNotFoundError", cex.Message);
            }
            catch (Exception ex)
            {
                FailWithError("Expected error for non-existent workflow fetch", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test105_Should_Fail_Update_NonExistent_Workflow()
        {
            TestOutputLogger.LogContext("TestScenario", "FailUpdateNonExistentWorkflow");
            try
            {
                // Arrange
                string nonExistentUid = $"non_existent_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel("update_test", 2);

                // Act
                ContentstackResponse response = _stack.Workflow(nonExistentUid).Update(workflowModel);

                // Assert
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Expected update to fail for non-existent workflow", "updateNonExistentFailed");
                AssertMissingWorkflowStatus(response.StatusCode, "missingWorkflowStatusCode");
                
                TestOutputLogger.LogContext("NotFoundOrUnprocessable", nonExistentUid);
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingWorkflowStatus(cex.StatusCode, "missingWorkflowStatusCode");
                TestOutputLogger.LogContext("ExpectedNotFoundError", cex.Message);
            }
            catch (Exception ex)
            {
                FailWithError("Expected error for non-existent workflow update", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test106_Should_Fail_Enable_NonExistent_Workflow()
        {
            TestOutputLogger.LogContext("TestScenario", "FailEnableNonExistentWorkflow");
            try
            {
                // Arrange
                string nonExistentUid = $"non_existent_workflow_{Guid.NewGuid():N}";

                // Act
                ContentstackResponse response = _stack.Workflow(nonExistentUid).Enable();

                // Assert
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Expected enable to fail for non-existent workflow", "enableNonExistentFailed");
                AssertMissingWorkflowStatus(response.StatusCode, "missingWorkflowStatusCode");
                
                TestOutputLogger.LogContext("NotFoundOrUnprocessable", nonExistentUid);
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingWorkflowStatus(cex.StatusCode, "missingWorkflowStatusCode");
                TestOutputLogger.LogContext("ExpectedNotFoundError", cex.Message);
            }
            catch (Exception ex)
            {
                FailWithError("Expected error for non-existent workflow enable", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test107_Should_Fail_Create_Publish_Rule_Invalid_Workflow_Reference()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreatePublishRuleInvalidWorkflowReference");
            try
            {
                // Arrange
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required", "testEnvironmentUid");

                string invalidWorkflowUid = $"invalid_workflow_{Guid.NewGuid():N}";
                string invalidStageUid = $"invalid_stage_{Guid.NewGuid():N}";
                
                var publishRuleModel = CreateTestPublishRuleModel(invalidWorkflowUid, invalidStageUid, _testEnvironmentUid);

                // Act
                ContentstackResponse response = _stack.Workflow().PublishRule().Create(publishRuleModel);

                // Assert
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Expected publish rule creation to fail with invalid workflow reference", "invalidReferenceCreationFailed");
                AssertLogger.IsTrue((int)response.StatusCode >= 400 && (int)response.StatusCode < 500, "Expected 4xx status code", "validationErrorStatusCode");
                
                TestOutputLogger.LogContext("ValidationError", "InvalidWorkflowReference");
            }
            catch (Exception ex)
            {
                if (ex is ContentstackErrorException cex)
                {
                    AssertLogger.IsTrue((int)cex.StatusCode >= 400 && (int)cex.StatusCode < 500, "Expected 4xx status code for validation error", "validationErrorStatusCode");
                    TestOutputLogger.LogContext("ExpectedValidationError", cex.Message);
                }
                else
                {
                    FailWithError("Expected validation error for invalid workflow reference", ex);
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test108_Should_Allow_Delete_Workflow_With_Active_Publish_Rules()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteWorkflowWithActivePublishRules");
            try
            {
                // Arrange - Create workflow and publish rule
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required", "testEnvironmentUid");

                string workflowName = $"test_delete_with_rules_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                ContentstackResponse ruleResponse = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var ruleJson = ruleResponse.OpenJObjectResponse();
                string publishRuleUid = ruleJson["publishing_rule"]["uid"].ToString();
                _createdPublishRuleUids.Add(publishRuleUid);

                // Act — Management API allows deleting the workflow while publish rules still reference it; cleanup removes rules first
                ContentstackResponse response = _stack.Workflow(workflowUid).Delete();

                // Assert
                AssertLogger.IsNotNull(response, "workflowDeleteResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow delete failed with status {(int)response.StatusCode}", "workflowDeleteSuccess");
                _createdWorkflowUids.Remove(workflowUid);
                
                TestOutputLogger.LogContext("DeletedWorkflowWithPublishRules", workflowUid);
            }
            catch (Exception ex)
            {
                FailWithError("Delete workflow with active publish rules", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test109_Should_Fail_Workflow_Operations_Without_Authentication()
        {
            TestOutputLogger.LogContext("TestScenario", "FailWorkflowOperationsWithoutAuthentication");
            try
            {
                // Arrange - Create unauthenticated client
                var unauthenticatedClient = new ContentstackClient();
                var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");

                // Act & Assert — SDK throws InvalidOperationException when not logged in (before HTTP)
                AssertLogger.ThrowsException<InvalidOperationException>(() =>
                {
                    unauthenticatedStack.Workflow().FindAll();
                }, "unauthenticatedWorkflowOperation");
                
                TestOutputLogger.LogContext("AuthenticationError", "NotLoggedIn");
            }
            catch (Exception ex)
            {
                FailWithError("Unauthenticated workflow operation", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test110_Should_Delete_Workflow_Successfully_After_Cleanup()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteWorkflowSuccessfullyAfterCleanup");
            try
            {
                // Arrange - Create a simple workflow
                string workflowName = $"test_final_delete_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();

                // Act
                ContentstackResponse response = _stack.Workflow(workflowUid).Delete();

                // Assert
                AssertLogger.IsNotNull(response, "workflowDeleteResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow delete failed with status {(int)response.StatusCode}", "workflowDeleteSuccess");
                
                TestOutputLogger.LogContext("DeletedWorkflowUid", workflowUid);
                
                // Verify deletion — fetch may return error response or throw ContentstackErrorException (e.g. 422)
                try
                {
                    ContentstackResponse fetchResponse = _stack.Workflow(workflowUid).Fetch();
                    AssertMissingWorkflowStatus(fetchResponse.StatusCode, "workflowNotFoundAfterDelete");
                }
                catch (ContentstackErrorException cex)
                {
                    AssertMissingWorkflowStatus(cex.StatusCode, "workflowNotFoundAfterDelete");
                }
            }
            catch (Exception ex)
            {
                FailWithError("Delete workflow after cleanup", ex);
            }
        }
    }
}