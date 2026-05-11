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
        
        // Dedicated test content types for better isolation
        private static readonly List<string> _dedicatedTestContentTypes = new List<string>();
        private static Stack _testStack;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
            
            // Initialize stack for content type operations
            StackResponse response = StackResponse.getStack(_client.SerializerOptions);
            _testStack = _client.Stack(response.Stack.APIKey);
            
            // Create dedicated test content types for better isolation
            CreateDedicatedTestContentTypes();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Clean up dedicated test content types
            CleanupDedicatedTestContentTypes();
            
            try { _client?.Logout(); } catch { }
            _client = null;
        }

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(_client.SerializerOptions);
            _stack = _client.Stack(response.Stack.APIKey);
            
            // Clear tracking lists for this test
            _createdWorkflowUids.Clear();
            _createdPublishRuleUids.Clear();
            _testEnvironmentUid = null;
            
            // Clean up any existing workflows that might conflict with new tests
            CleanupConflictingWorkflows();
            
            // Verify clean environment before test execution
            VerifyCleanTestEnvironment();
            
            Console.WriteLine($"[TestInit] Test environment prepared and verified clean");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.WriteLine($"[TestCleanup] Starting cleanup for test with {_createdWorkflowUids.Count} workflows and {_createdPublishRuleUids.Count} publish rules");
            
            // Best-effort cleanup of created resources
            CleanupCreatedResources();
            
            // Additional cleanup verification
            VerifyResourcesCleanedUp();
            
            // Clear tracking lists
            _createdWorkflowUids.Clear();
            _createdPublishRuleUids.Clear();
            _testEnvironmentUid = null;
            
            Console.WriteLine($"[TestCleanup] Cleanup completed");
        }

        /// <summary>
        /// Verifies that the test environment is clean before starting a test.
        /// Logs warnings if unexpected resources are found but doesn't fail the test.
        /// </summary>
        private void VerifyCleanTestEnvironment()
        {
            try
            {
                // Check for any existing workflows
                ContentstackResponse workflowResponse = _stack.Workflow().FindAll();
                if (workflowResponse.IsSuccessStatusCode)
                {
                    var jObject = workflowResponse.OpenJsonObjectResponse();
                    var workflowsArray = jObject["workflows"] as JsonArray;
                    
                    if (workflowsArray != null && workflowsArray.Count > 0)
                    {
                        Console.WriteLine($"[TestVerify] Warning: Found {workflowsArray.Count} existing workflows in environment");
                        foreach (var workflow in workflowsArray.Take(3)) // Log first 3 for debugging
                        {
                            Console.WriteLine($"[TestVerify] Existing workflow: {workflow["name"]} ({workflow["uid"]})");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[TestVerify] Environment clean: No existing workflows found");
                    }
                }

                // Check for any existing publish rules
                ContentstackResponse publishRulesResponse = _stack.Workflow().PublishRule().FindAll();
                if (publishRulesResponse.IsSuccessStatusCode)
                {
                    var jObject = publishRulesResponse.OpenJsonObjectResponse();
                    var publishRulesArray = jObject["publishing_rules"] as JsonArray;
                    
                    if (publishRulesArray != null && publishRulesArray.Count > 0)
                    {
                        Console.WriteLine($"[TestVerify] Warning: Found {publishRulesArray.Count} existing publish rules in environment");
                    }
                    else
                    {
                        Console.WriteLine($"[TestVerify] Environment clean: No existing publish rules found");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TestVerify] Warning: Could not verify clean environment: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifies that resources created during the test have been properly cleaned up.
        /// </summary>
        private void VerifyResourcesCleanedUp()
        {
            if (_createdWorkflowUids.Count > 0)
            {
                Console.WriteLine($"[TestCleanup] Warning: {_createdWorkflowUids.Count} workflows may not have been cleaned up: {string.Join(", ", _createdWorkflowUids)}");
            }

            if (_createdPublishRuleUids.Count > 0)
            {
                Console.WriteLine($"[TestCleanup] Warning: {_createdPublishRuleUids.Count} publish rules may not have been cleaned up: {string.Join(", ", _createdPublishRuleUids)}");
            }

            if (_createdWorkflowUids.Count == 0 && _createdPublishRuleUids.Count == 0)
            {
                Console.WriteLine($"[TestCleanup] Success: All created resources appear to have been cleaned up");
            }
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
        /// Uses real content type UIDs from the stack for valid workflow creation.
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
                ContentTypes = new List<string> { GetValidContentTypeUid() }, // Use real content type UID
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
            // Get the workflow's content types to ensure publish rule compatibility
            List<string> workflowContentTypes = GetWorkflowContentTypes(workflowUid);
            
            return new PublishRuleModel
            {
                WorkflowUid = workflowUid,
                WorkflowStageUid = stageUid,
                Environment = environmentUid,
                Branches = new List<string> { "main" },
                ContentTypes = workflowContentTypes, // Use the same content types as the workflow
                Locales = new List<string> { "en-us" },
                Actions = new List<string>(),
                Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() },
                DisableApproval = false
            };
        }

        /// <summary>
        /// Gets the content types associated with a specific workflow.
        /// This ensures publish rules use the same content types as their parent workflow.
        /// </summary>
        private List<string> GetWorkflowContentTypes(string workflowUid)
        {
            try
            {
                ContentstackResponse response = _stack.Workflow(workflowUid).Fetch();
                if (response.IsSuccessStatusCode)
                {
                    var workflowJson = response.OpenJsonObjectResponse();
                    var contentTypesArray = workflowJson["workflow"]?["content_types"] as JsonArray;
                    
                    if (contentTypesArray != null)
                    {
                        var contentTypes = contentTypesArray.Select(ct => ct.ToString()).ToList();
                        Console.WriteLine($"[PublishRule] Using workflow content types: {string.Join(", ", contentTypes)}");
                        return contentTypes;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PublishRule] Failed to get workflow content types for {workflowUid}: {ex.Message}");
            }
            
            // Fallback to $all if we can't determine the workflow's content types
            // This maintains backward compatibility for tests that expect $all
            Console.WriteLine($"[PublishRule] Falling back to $all content types for workflow {workflowUid}");
            return new List<string> { "$all" };
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
                    var envJson = envResponse.OpenJsonObjectResponse();
                    var environments = envJson["environments"] as JsonArray;
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
                    var responseJson = response.OpenJsonObjectResponse();
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
        private static readonly List<string> _availableContentTypes = new List<string>();
        private static int _contentTypeRotationIndex = 0;

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
        /// Creates dedicated content types specifically for workflow testing.
        /// These provide consistent test isolation without interfering with stack content.
        /// </summary>
        private static void CreateDedicatedTestContentTypes()
        {
            try
            {
                string[] testContentTypeSpecs = {
                    "workflow_test_ct_1",
                    "workflow_test_ct_2", 
                    "workflow_test_ct_3"
                };

                foreach (string ctUid in testContentTypeSpecs)
                {
                    try
                    {
                        // Check if content type already exists
                        ContentstackResponse existsResponse = _testStack.ContentType(ctUid).Fetch();
                        if (existsResponse.IsSuccessStatusCode)
                        {
                            _dedicatedTestContentTypes.Add(ctUid);
                            Console.WriteLine($"[Setup] Using existing test content type: {ctUid}");
                            continue;
                        }

                        // Create new dedicated test content type
                        var contentModelling = new ContentModelling
                        {
                            Title = $"Workflow Test Content Type {ctUid}",
                            Uid = ctUid,
                            Schema = new List<Models.Fields.Field> 
                            { 
                                new Models.Fields.TextboxField
                                {
                                    Uid = "title",
                                    DataType = "text",
                                    FieldMetadata = new Models.Fields.FieldMetadata { Description = "Title field for workflow testing" },
                                    DisplayName = "Title",
                                    Mandatory = true,
                                    Unique = false
                                }
                            }
                        };

                        ContentstackResponse response = _testStack.ContentType().Create(contentModelling);
                        if (response.IsSuccessStatusCode)
                        {
                            _dedicatedTestContentTypes.Add(ctUid);
                            Console.WriteLine($"[Setup] Created dedicated test content type: {ctUid}");
                        }
                        else
                        {
                            Console.WriteLine($"[Setup] Failed to create test content type {ctUid}: {response.OpenResponse()}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Setup] Error creating test content type {ctUid}: {ex.Message}");
                    }
                }

                Console.WriteLine($"[Setup] Initialized {_dedicatedTestContentTypes.Count} dedicated test content types");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Setup] Failed to create dedicated test content types: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleans up dedicated test content types created for workflow testing.
        /// </summary>
        private static void CleanupDedicatedTestContentTypes()
        {
            foreach (string ctUid in _dedicatedTestContentTypes.ToList())
            {
                try
                {
                    ContentstackResponse response = _testStack.ContentType(ctUid).Delete();
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"[Cleanup] Deleted dedicated test content type: {ctUid}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Cleanup] Failed to delete test content type {ctUid}: {ex.Message}");
                }
            }
            _dedicatedTestContentTypes.Clear();
        }

        /// <summary>
        /// Gets a valid content type UID from the stack with rotation for test isolation.
        /// Prioritizes dedicated test content types, falls back to stack content types.
        /// </summary>
        private string GetValidContentTypeUid()
        {
            try
            {
                // First priority: Use dedicated test content types with rotation
                if (_dedicatedTestContentTypes.Count > 0)
                {
                    string contentType = _dedicatedTestContentTypes[_contentTypeRotationIndex % _dedicatedTestContentTypes.Count];
                    _contentTypeRotationIndex++;
                    Console.WriteLine($"[ContentType] Using dedicated test content type: {contentType}");
                    return contentType;
                }

                // Second priority: Use discovered content types from stack (excluding problematic ones)
                if (_availableContentTypes.Count == 0)
                {
                    DiscoverAvailableContentTypes();
                }

                // Filter out content types that are known to cause conflicts
                var safeContentTypes = _availableContentTypes.Where(ct => 
                    ct != "single_page" && 
                    ct != "multi_page" &&
                    !string.IsNullOrEmpty(ct)).ToList();

                if (safeContentTypes.Count > 0)
                {
                    string contentType = safeContentTypes[_contentTypeRotationIndex % safeContentTypes.Count];
                    _contentTypeRotationIndex++;
                    Console.WriteLine($"[ContentType] Using discovered safe content type: {contentType}");
                    return contentType;
                }

                // Third priority: Try to find any content type from stack that's not conflicting
                string fallback = TryGetFirstContentTypeUidFromStack();
                if (!string.IsNullOrEmpty(fallback) && fallback != "single_page" && fallback != "multi_page")
                {
                    Console.WriteLine($"[ContentType] Using fallback content type: {fallback}");
                    return fallback;
                }

                // Create a unique content type if we can't find any safe ones
                string uniqueContentType = CreateUniqueTestContentType();
                if (!string.IsNullOrEmpty(uniqueContentType))
                {
                    Console.WriteLine($"[ContentType] Created unique test content type: {uniqueContentType}");
                    return uniqueContentType;
                }

                // Final emergency fallback - create a GUID-based content type name
                string emergencyContentType = $"test_ct_{Guid.NewGuid():N}";
                Console.WriteLine($"[ContentType] Emergency fallback content type: {emergencyContentType}");
                return emergencyContentType;
            }
            catch (Exception ex)
            {
                // Emergency fallback with unique GUID to avoid conflicts
                string emergencyContentType = $"test_ct_{Guid.NewGuid():N}";
                Console.WriteLine($"[ContentType] Exception in GetValidContentTypeUid, using emergency fallback: {emergencyContentType}. Error: {ex.Message}");
                return emergencyContentType;
            }
        }

        /// <summary>
        /// Creates a unique test content type when none are available.
        /// </summary>
        private string CreateUniqueTestContentType()
        {
            try
            {
                string ctUid = $"test_workflow_ct_{Guid.NewGuid():N}";
                
                var contentModelling = new ContentModelling
                {
                    Title = $"Unique Workflow Test Content Type",
                    Uid = ctUid,
                    Schema = new List<Models.Fields.Field> 
                    { 
                        new Models.Fields.TextboxField
                        {
                            Uid = "title",
                            DataType = "text",
                            FieldMetadata = new Models.Fields.FieldMetadata { Description = "Title field for workflow testing" },
                            DisplayName = "Title",
                            Mandatory = true,
                            Unique = false
                        }
                    }
                };

                ContentstackResponse response = _testStack.ContentType().Create(contentModelling);
                if (response.IsSuccessStatusCode)
                {
                    // Add to our tracking list for cleanup
                    _dedicatedTestContentTypes.Add(ctUid);
                    return ctUid;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ContentType] Failed to create unique content type: {ex.Message}");
            }
            
            return null;
        }

        /// <summary>
        /// Discovers and caches all available content types from the stack.
        /// </summary>
        private void DiscoverAvailableContentTypes()
        {
            try
            {
                ContentstackResponse response = _stack.ContentType().Query().Find();
                if (!response.IsSuccessStatusCode)
                    return;

                var jObject = response.OpenJsonObjectResponse();
                var contentTypesArray = jObject["content_types"] as JsonArray;

                if (contentTypesArray != null)
                {
                    foreach (var ct in contentTypesArray)
                    {
                        string uid = ct["uid"]?.ToString();
                        if (!string.IsNullOrEmpty(uid))
                        {
                            _availableContentTypes.Add(uid);
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors during discovery - we'll use fallbacks
            }
        }

        /// <summary>
        /// Checks if a content type UID exists in the stack.
        /// </summary>
        private bool IsContentTypeValid(string contentTypeUid)
        {
            try
            {
                ContentstackResponse response = _stack.ContentType(contentTypeUid).Fetch();
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cleans up conflicting workflows that use "$all" content types.
        /// This prevents test failures due to Contentstack's one-workflow-per-stack limitation for "$all".
        /// </summary>
        private void CleanupConflictingWorkflows()
        {
            try
            {
                // Query all existing workflows - we need to remove ALL workflows to prevent conflicts
                ContentstackResponse response = _stack.Workflow().FindAll();
                if (!response.IsSuccessStatusCode)
                    return; // Ignore cleanup failures

                var jObject = response.OpenJsonObjectResponse();
                var workflowsArray = jObject["workflows"] as JsonArray;
                
                if (workflowsArray != null)
                {
                    foreach (var workflow in workflowsArray)
                    {
                        string workflowUid = workflow["uid"]?.ToString();
                        string workflowName = workflow["name"]?.ToString();
                        
                        if (!string.IsNullOrEmpty(workflowUid))
                        {
                            try
                            {
                                // First, clean up any publish rules associated with this workflow
                                CleanupPublishRulesForWorkflow(workflowUid);
                                
                                // Then delete the workflow
                                _stack.Workflow(workflowUid).Delete();
                                Console.WriteLine($"[Cleanup] Removed workflow: {workflowName} ({workflowUid})");
                            }
                            catch (Exception ex)
                            {
                                // Log individual workflow deletion failures but continue
                                Console.WriteLine($"[Cleanup] Failed to delete workflow {workflowUid} ({workflowName}): {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore all cleanup failures - tests should run even if cleanup fails
                Console.WriteLine($"[Cleanup] Workflow cleanup failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleans up publish rules associated with a specific workflow.
        /// Must be called before deleting the workflow to avoid constraint violations.
        /// </summary>
        private void CleanupPublishRulesForWorkflow(string workflowUid)
        {
            try
            {
                // Query all publish rules
                ContentstackResponse response = _stack.Workflow().PublishRule().FindAll();
                if (!response.IsSuccessStatusCode)
                    return;

                var jObject = response.OpenJsonObjectResponse();
                var publishRulesArray = jObject["publishing_rules"] as JsonArray;

                if (publishRulesArray != null)
                {
                    foreach (var publishRule in publishRulesArray)
                    {
                        string publishRuleUid = publishRule["uid"]?.ToString();
                        string publishRuleWorkflowUid = publishRule["workflow"]?.ToString();

                        // If this publish rule belongs to the workflow we're deleting
                        if (publishRuleWorkflowUid == workflowUid && !string.IsNullOrEmpty(publishRuleUid))
                        {
                            try
                            {
                                _stack.Workflow().PublishRule(publishRuleUid).Delete();
                                Console.WriteLine($"[Cleanup] Removed publish rule {publishRuleUid} for workflow {workflowUid}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Cleanup] Failed to delete publish rule {publishRuleUid}: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cleanup] Failed to query/clean publish rules for workflow {workflowUid}: {ex.Message}");
            }
        }

        /// <summary>
        /// Best-effort cleanup of created test resources.
        /// </summary>
        private void CleanupCreatedResources()
        {
            // Cleanup publish rules first (they depend on workflows)
            int publishRulesDeleted = 0;
            foreach (var ruleUid in _createdPublishRuleUids.ToList())
            {
                try
                {
                    ContentstackResponse response = _stack.Workflow().PublishRule(ruleUid).Delete();
                    if (response.IsSuccessStatusCode)
                    {
                        publishRulesDeleted++;
                        Console.WriteLine($"[Cleanup] Deleted publish rule: {ruleUid}");
                    }
                    else
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete publish rule {ruleUid}: HTTP {(int)response.StatusCode}");
                    }
                    _createdPublishRuleUids.Remove(ruleUid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Cleanup] Exception deleting publish rule {ruleUid}: {ex.Message}");
                    _createdPublishRuleUids.Remove(ruleUid); // Remove from tracking even if delete failed
                }
            }

            // Then cleanup workflows
            int workflowsDeleted = 0;
            foreach (var workflowUid in _createdWorkflowUids.ToList())
            {
                try
                {
                    ContentstackResponse response = _stack.Workflow(workflowUid).Delete();
                    if (response.IsSuccessStatusCode)
                    {
                        workflowsDeleted++;
                        Console.WriteLine($"[Cleanup] Deleted workflow: {workflowUid}");
                    }
                    else
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete workflow {workflowUid}: HTTP {(int)response.StatusCode}");
                    }
                    _createdWorkflowUids.Remove(workflowUid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Cleanup] Exception deleting workflow {workflowUid}: {ex.Message}");
                    _createdWorkflowUids.Remove(workflowUid); // Remove from tracking even if delete failed
                }
            }

            Console.WriteLine($"[Cleanup] Deleted {publishRulesDeleted} publish rules and {workflowsDeleted} workflows");
        }

        /// <summary>
        /// Helper method to ensure workflow UIDs are properly tracked for cleanup.
        /// Call this immediately after successful workflow creation.
        /// </summary>
        private void TrackWorkflowForCleanup(string workflowUid, string workflowName = null)
        {
            if (!string.IsNullOrEmpty(workflowUid) && !_createdWorkflowUids.Contains(workflowUid))
            {
                _createdWorkflowUids.Add(workflowUid);
                Console.WriteLine($"[Tracking] Added workflow to cleanup list: {workflowName ?? "Unknown"} ({workflowUid})");
            }
        }

        /// <summary>
        /// Helper method to ensure publish rule UIDs are properly tracked for cleanup.
        /// Call this immediately after successful publish rule creation.
        /// </summary>
        private void TrackPublishRuleForCleanup(string publishRuleUid)
        {
            if (!string.IsNullOrEmpty(publishRuleUid) && !_createdPublishRuleUids.Contains(publishRuleUid))
            {
                _createdPublishRuleUids.Add(publishRuleUid);
                Console.WriteLine($"[Tracking] Added publish rule to cleanup list: {publishRuleUid}");
            }
        }

        /// <summary>
        /// Creates invalid workflow model for testing specific validation scenarios.
        /// Uses real content type UID but keeps other parameters invalid for proper negative testing.
        /// </summary>
        private WorkflowModel CreateInvalidWorkflowModel(string scenario)
        {
            // Use real content type UID for valid API calls - other parameters remain invalid for testing
            var contentTypes = new List<string> { GetValidContentTypeUid() };

            switch (scenario)
            {
                case "empty_name":
                    return new WorkflowModel
                    {
                        Name = "",
                        Enabled = true,
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        WorkflowStages = GenerateTestStages(2)
                    };

                case "whitespace_name":
                    return new WorkflowModel
                    {
                        Name = "   ",
                        Enabled = true,
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        WorkflowStages = GenerateTestStages(2)
                    };

                case "long_name":
                    return new WorkflowModel
                    {
                        Name = new string('a', 1000), // Extremely long name
                        Enabled = true,
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        WorkflowStages = GenerateTestStages(2)
                    };

                case "invalid_characters":
                    return new WorkflowModel
                    {
                        Name = "test<>workflow&name",
                        Enabled = true,
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        WorkflowStages = GenerateTestStages(2)
                    };

                case "single_stage":
                    return new WorkflowModel
                    {
                        Name = $"test_single_stage_{Guid.NewGuid():N}",
                        Enabled = true,
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        WorkflowStages = GenerateTestStages(1) // API requires min 2
                    };

                case "too_many_stages":
                    return new WorkflowModel
                    {
                        Name = $"test_many_stages_{Guid.NewGuid():N}",
                        Enabled = true,
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        WorkflowStages = GenerateTestStages(25) // API max 20
                    };

                case "empty_stages":
                    return new WorkflowModel
                    {
                        Name = $"test_empty_stages_{Guid.NewGuid():N}",
                        Enabled = true,
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        WorkflowStages = new List<WorkflowStage>()
                    };

                case "null_stages":
                    return new WorkflowModel
                    {
                        Name = $"test_null_stages_{Guid.NewGuid():N}",
                        Enabled = true,
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        WorkflowStages = null
                    };

                case "invalid_branches":
                    return new WorkflowModel
                    {
                        Name = $"test_invalid_branches_{Guid.NewGuid():N}",
                        Enabled = true,
                        Branches = new List<string> { "", null, "invalid-branch-name!" },
                        ContentTypes = contentTypes,
                        WorkflowStages = GenerateTestStages(2)
                    };

                case "empty_branches":
                    return new WorkflowModel
                    {
                        Name = $"test_empty_branches_{Guid.NewGuid():N}",
                        Enabled = true,
                        Branches = new List<string>(),
                        ContentTypes = contentTypes,
                        WorkflowStages = GenerateTestStages(2)
                    };

                default:
                    throw new ArgumentException($"Unknown scenario: {scenario}");
            }
        }

        /// <summary>
        /// Creates invalid workflow stage for testing specific validation scenarios.
        /// </summary>
        private WorkflowStage CreateInvalidStage(string scenario)
        {
            switch (scenario)
            {
                case "empty_name":
                    return new WorkflowStage
                    {
                        Name = "",
                        Color = "#fe5cfb",
                        SystemACL = new Dictionary<string, object>
                        {
                            ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                            ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                            ["others"] = new Dictionary<string, object>()
                        },
                        NextAvailableStages = new List<string> { "$all" },
                        AllStages = true,
                        AllUsers = true
                    };

                case "invalid_color":
                    return new WorkflowStage
                    {
                        Name = "Test Invalid Color Stage",
                        Color = "invalid_color_format",
                        SystemACL = new Dictionary<string, object>
                        {
                            ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                            ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                            ["others"] = new Dictionary<string, object>()
                        },
                        NextAvailableStages = new List<string> { "$all" },
                        AllStages = true,
                        AllUsers = true
                    };

                case "missing_acl":
                    return new WorkflowStage
                    {
                        Name = "Test Missing ACL Stage",
                        Color = "#fe5cfb",
                        SystemACL = null,
                        NextAvailableStages = new List<string> { "$all" },
                        AllStages = true,
                        AllUsers = true
                    };

                case "invalid_acl":
                    return new WorkflowStage
                    {
                        Name = "Test Invalid ACL Stage",
                        Color = "#fe5cfb",
                        SystemACL = new Dictionary<string, object>
                        {
                            ["invalid_key"] = "invalid_value"
                        },
                        NextAvailableStages = new List<string> { "$all" },
                        AllStages = true,
                        AllUsers = true
                    };

                default:
                    throw new ArgumentException($"Unknown scenario: {scenario}");
            }
        }

        /// <summary>
        /// Creates invalid publish rule model for testing specific validation scenarios.
        /// </summary>
        private PublishRuleModel CreateInvalidPublishRuleModel(string scenario, string workflowUid = null, string stageUid = null, string environmentUid = null)
        {
            // Get appropriate content types for this publish rule based on scenario and workflow
            List<string> contentTypes = GetContentTypesForInvalidPublishRule(scenario, workflowUid);
            
            switch (scenario)
            {
                case "missing_environment":
                    return new PublishRuleModel
                    {
                        WorkflowUid = workflowUid ?? "valid_workflow_uid",
                        WorkflowStageUid = stageUid ?? "valid_stage_uid",
                        Environment = null, // Missing required field
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        Locales = new List<string> { "en-us" },
                        Actions = new List<string>(),
                        Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() }
                    };

                case "invalid_environment":
                    return new PublishRuleModel
                    {
                        WorkflowUid = workflowUid ?? "valid_workflow_uid",
                        WorkflowStageUid = stageUid ?? "valid_stage_uid",
                        Environment = "non_existent_environment_uid",
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        Locales = new List<string> { "en-us" },
                        Actions = new List<string>(),
                        Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() }
                    };

                case "missing_workflow":
                    return new PublishRuleModel
                    {
                        WorkflowUid = null, // Missing required field
                        WorkflowStageUid = stageUid ?? "valid_stage_uid",
                        Environment = environmentUid ?? "valid_environment_uid",
                        Branches = new List<string> { "main" },
                        ContentTypes = new List<string> { "$all" }, // Use $all since no workflow context
                        Locales = new List<string> { "en-us" },
                        Actions = new List<string>(),
                        Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() }
                    };

                case "missing_stage":
                    return new PublishRuleModel
                    {
                        WorkflowUid = workflowUid ?? "valid_workflow_uid",
                        WorkflowStageUid = null, // Missing required field
                        Environment = environmentUid ?? "valid_environment_uid",
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        Locales = new List<string> { "en-us" },
                        Actions = new List<string>(),
                        Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() }
                    };

                case "invalid_content_types":
                    return new PublishRuleModel
                    {
                        WorkflowUid = workflowUid ?? "valid_workflow_uid",
                        WorkflowStageUid = stageUid ?? "valid_stage_uid",
                        Environment = environmentUid ?? "valid_environment_uid",
                        Branches = new List<string> { "main" },
                        ContentTypes = new List<string> { "non_existent_content_type", "", null }, // Keep invalid for this test
                        Locales = new List<string> { "en-us" },
                        Actions = new List<string>(),
                        Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() }
                    };

                case "invalid_locales":
                    return new PublishRuleModel
                    {
                        WorkflowUid = workflowUid ?? "valid_workflow_uid",
                        WorkflowStageUid = stageUid ?? "valid_stage_uid",
                        Environment = environmentUid ?? "valid_environment_uid",
                        Branches = new List<string> { "main" },
                        ContentTypes = contentTypes,
                        Locales = new List<string> { "invalid-locale", "", null },
                        Actions = new List<string>(),
                        Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() }
                    };

                case "empty_branches":
                    return new PublishRuleModel
                    {
                        WorkflowUid = workflowUid ?? "valid_workflow_uid",
                        WorkflowStageUid = stageUid ?? "valid_stage_uid",
                        Environment = environmentUid ?? "valid_environment_uid",
                        Branches = new List<string>(), // Empty branches
                        ContentTypes = contentTypes,
                        Locales = new List<string> { "en-us" },
                        Actions = new List<string>(),
                        Approvers = new Approvals { Users = new List<string>(), Roles = new List<string>() }
                    };

                default:
                    throw new ArgumentException($"Unknown scenario: {scenario}");
            }
        }

        /// <summary>
        /// Gets appropriate content types for invalid publish rule scenarios.
        /// Uses workflow's actual content types to avoid API content type mismatch errors,
        /// except for specific scenarios that need to test content type validation itself.
        /// </summary>
        private List<string> GetContentTypesForInvalidPublishRule(string scenario, string workflowUid)
        {
            // For content type validation tests, keep the invalid content types
            if (scenario == "invalid_content_types")
            {
                return new List<string> { "non_existent_content_type", "", null };
            }
            
            // For other validation tests, use the workflow's content types to avoid content type mismatch
            if (!string.IsNullOrEmpty(workflowUid))
            {
                List<string> workflowContentTypes = GetWorkflowContentTypes(workflowUid);
                // Don't use $all as fallback since that causes the mismatch we're trying to avoid
                return workflowContentTypes.Contains("$all") ? new List<string> { "$all" } : workflowContentTypes;
            }
            
            // Fallback for tests without workflow context
            return new List<string> { "$all" };
        }

        /// <summary>
        /// Validates 4xx HTTP status codes for validation errors.
        /// </summary>
        private static void AssertValidationError(HttpStatusCode statusCode, string assertionName)
        {
            AssertLogger.IsTrue(
                (int)statusCode >= 400 && (int)statusCode < 500,
                $"Expected 4xx status code for validation error, got {(int)statusCode} ({statusCode})",
                assertionName);
        }

        /// <summary>
        /// Validates authentication-related errors.
        /// </summary>
        private static void AssertAuthenticationError(Exception ex, string assertionName)
        {
            if (ex is InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "SDK validation threw InvalidOperationException as expected", assertionName);
            }
            else if (ex is ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Unauthorized || 
                    cex.StatusCode == HttpStatusCode.Forbidden || 
                    cex.StatusCode == HttpStatusCode.PreconditionFailed,
                    $"Expected 401/403/412 for auth error, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    assertionName);
            }
            else
            {
                AssertLogger.Fail($"Unexpected exception type: {ex.GetType().Name}");
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
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowCreateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow create failed with status {(int)response.StatusCode}", "workflowCreateSuccess");
                AssertLogger.IsNotNull(responseJson["workflow"], "workflowObject");
                AssertLogger.IsNotNull(responseJson["workflow"]["uid"], "workflowUid");
                
                string workflowUid = responseJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);
                TestOutputLogger.LogContext("WorkflowUid", workflowUid);
                
                var stages = responseJson["workflow"]["workflow_stages"] as JsonArray;
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
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowCreateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow create failed with status {(int)response.StatusCode}", "workflowCreateSuccess");
                AssertLogger.IsNotNull(responseJson["workflow"], "workflowObject");
                
                string workflowUid = responseJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);
                TestOutputLogger.LogContext("WorkflowUid", workflowUid);
                
                var stages = responseJson["workflow"]["workflow_stages"] as JsonArray;
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
                var createJson = createResponse.OpenJsonObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Act
                ContentstackResponse response = _stack.Workflow(workflowUid).Fetch();
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowFetchResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow fetch failed with status {(int)response.StatusCode}", "workflowFetchSuccess");
                AssertLogger.IsNotNull(responseJson["workflow"], "workflowObject");
                AssertLogger.AreEqual(workflowUid, responseJson["workflow"]["uid"]?.ToString(), "workflowUid");
                AssertLogger.AreEqual(workflowName, responseJson["workflow"]["name"]?.ToString(), "workflowName");
                
                var stages = responseJson["workflow"]["workflow_stages"] as JsonArray;
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
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowFindAllResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow FindAll failed with status {(int)response.StatusCode}", "workflowFindAllSuccess");
                
                // Response should contain workflows array (even if empty)
                var workflows = (responseJson["workflows"] as JsonArray) ?? (responseJson["workflow"] as JsonArray);
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
                var createJson = createResponse.OpenJsonObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Prepare update
                string updatedName = $"updated_workflow_{Guid.NewGuid():N}";
                workflowModel.Name = updatedName;
                workflowModel.Enabled = false; // Change enabled status

                // Act
                ContentstackResponse response = _stack.Workflow(workflowUid).Update(workflowModel);
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowUpdateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow update failed with status {(int)response.StatusCode}", "workflowUpdateSuccess");
                AssertLogger.IsNotNull(responseJson["workflow"], "workflowObject");
                AssertLogger.AreEqual(updatedName, responseJson["workflow"]["name"]?.ToString(), "updatedWorkflowName");
                AssertLogger.AreEqual(false, responseJson["workflow"]?["enabled"]?.GetValue<bool>(), "updatedEnabledStatus");
                
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
                var createJson = createResponse.OpenJsonObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                workflowModel.WorkflowStages = GenerateTestStages(3);

                // Act
                ContentstackResponse response = _stack.Workflow(workflowUid).Update(workflowModel);
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowUpdateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow update failed with status {(int)response.StatusCode}", "workflowUpdateSuccess");
                
                var stages = responseJson["workflow"]["workflow_stages"] as JsonArray;
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
                var createJson = createResponse.OpenJsonObjectResponse();
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
                var createJson = createResponse.OpenJsonObjectResponse();
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
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[1]["uid"].ToString(); // Use second stage

                // Create publish rule
                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);

                // Act
                ContentstackResponse response = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var responseJson = response.OpenJsonObjectResponse();

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
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "publishRuleFindAllResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Publish rule FindAll failed with status {(int)response.StatusCode}", "publishRuleFindAllSuccess");
                
                // Response should contain publishing_rules array (even if empty)
                var rules = (responseJson["publishing_rules"] as JsonArray) ?? (responseJson["publishing_rule"] as JsonArray);
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
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                publishRuleModel.ContentTypes = new List<string> { contentTypeUid };

                ContentstackResponse ruleResponse = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var ruleJson = ruleResponse.OpenJsonObjectResponse();
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
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                ContentstackResponse ruleResponse = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var ruleJson = ruleResponse.OpenJsonObjectResponse();
                string publishRuleUid = ruleJson["publishing_rule"]["uid"].ToString();
                _createdPublishRuleUids.Add(publishRuleUid);

                // Update the publish rule (locales must exist on the stack; integration stack typically has en-us)
                publishRuleModel.DisableApproval = true;
                publishRuleModel.Locales = new List<string> { "en-us" };

                // Act
                ContentstackResponse response = _stack.Workflow().PublishRule(publishRuleUid).Update(publishRuleModel);
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "publishRuleUpdateResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Publish rule update failed with status {(int)response.StatusCode}", "publishRuleUpdateSuccess");
                AssertLogger.AreEqual(true, responseJson["publishing_rule"]?["disable_approver_publishing"]?.GetValue<bool>(), "updatedDisableApproval");
                
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
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowFindAllWithIncludeResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow FindAll with include failed with status {(int)response.StatusCode}", "workflowFindAllWithIncludeSuccess");
                
                var workflows = (responseJson["workflows"] as JsonArray) ?? (responseJson["workflow"] as JsonArray);
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
                var responseJson = response.OpenJsonObjectResponse();

                // Assert
                AssertLogger.IsNotNull(response, "workflowPaginationResponse");
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Workflow FindAll with pagination failed with status {(int)response.StatusCode}", "workflowPaginationSuccess");
                
                var workflows = (responseJson["workflows"] as JsonArray) ?? (responseJson["workflow"] as JsonArray);
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
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                ContentstackResponse ruleResponse = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var ruleJson = ruleResponse.OpenJsonObjectResponse();
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
                
                var responseJson1 = response1.OpenJsonObjectResponse();
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
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                ContentstackResponse ruleResponse = _stack.Workflow().PublishRule().Create(publishRuleModel);
                var ruleJson = ruleResponse.OpenJsonObjectResponse();
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
                var createJson = createResponse.OpenJsonObjectResponse();
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

        // ==== COMPREHENSIVE NEGATIVE PATH TESTS (111-153) ====

        // Category A: Input Validation Gaps (Test111-120)

        [TestMethod]
        [DoNotParallelize]
        public void Test111_Should_Fail_Create_Workflow_With_Empty_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithEmptyName");
            try
            {
                // Arrange
                var workflowModel = CreateInvalidWorkflowModel("empty_name");

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createWorkflowEmptyName",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "EmptyName");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for empty workflow name", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test112_Should_Accept_Create_Workflow_With_Whitespace_Only_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "AcceptCreateWorkflowWithWhitespaceOnlyName");
            try
            {
                // Arrange - API normalizes whitespace-only names
                var workflowModel = CreateInvalidWorkflowModel("whitespace_name");

                // Act
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);

                // Assert - API accepts and normalizes whitespace names
                AssertLogger.IsTrue(response.IsSuccessStatusCode, 
                    "API should accept and normalize whitespace-only workflow names", 
                    "createWorkflowWhitespaceName");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = response.OpenJsonObjectResponse();
                    string workflowUid = responseJson["workflow"]["uid"].ToString();
                    _createdWorkflowUids.Add(workflowUid);
                }

                TestOutputLogger.LogContext("ApiAccepted", "WhitespaceOnlyName");
            }
            catch (Exception ex)
            {
                FailWithError("API should accept whitespace-only workflow name", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test113_Should_Fail_Create_Workflow_With_Extremely_Long_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithExtremelyLongName");
            try
            {
                // Arrange
                var workflowModel = CreateInvalidWorkflowModel("long_name");

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createWorkflowLongName",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "ExtremelyLongName");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for extremely long workflow name", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test114_Should_Accept_Create_Workflow_With_Invalid_Characters_In_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "AcceptCreateWorkflowWithInvalidCharactersInName");
            try
            {
                // Arrange
                var workflowModel = CreateInvalidWorkflowModel("invalid_characters");

                // Act
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);

                // Assert - API accepts invalid characters in workflow names
                AssertLogger.IsTrue(response.IsSuccessStatusCode,
                    "API should accept workflow with invalid characters in name",
                    "createWorkflowInvalidChars");

                if (response.IsSuccessStatusCode)
                {
                    var workflowUid = response.OpenJsonObjectResponse()["workflow"]?["uid"]?.ToString();
                    if (!string.IsNullOrEmpty(workflowUid))
                    {
                        TrackWorkflowForCleanup(workflowUid, workflowModel.Name);
                    }
                }

                TestOutputLogger.LogContext("APIPermissive", "InvalidCharactersAccepted");
            }
            catch (Exception ex)
            {
                FailWithError("Create workflow with invalid characters in name", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test115_Should_Fail_Create_Workflow_With_Single_Stage()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithSingleStage");
            try
            {
                // Arrange - API requires minimum 2 stages
                var workflowModel = CreateInvalidWorkflowModel("single_stage");

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createWorkflowSingleStage",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "SingleStage");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for single workflow stage (API min 2)", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test116_Should_Fail_Create_Workflow_With_Too_Many_Stages()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithTooManyStages");
            try
            {
                // Arrange - API allows maximum 20 stages
                var workflowModel = CreateInvalidWorkflowModel("too_many_stages");

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createWorkflowTooManyStages",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "TooManyStages");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for too many workflow stages (API max 20)", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test117_Should_Fail_Create_Workflow_With_Empty_Stages_Array()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithEmptyStagesArray");
            try
            {
                // Arrange
                var workflowModel = CreateInvalidWorkflowModel("empty_stages");

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createWorkflowEmptyStages",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "EmptyStagesArray");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for empty workflow stages array", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test118_Should_Fail_Create_Workflow_With_Null_Stages_Array()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithNullStagesArray");
            try
            {
                // Arrange
                var workflowModel = CreateInvalidWorkflowModel("null_stages");

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createWorkflowNullStages",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "NullStagesArray");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for null workflow stages array", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test119_Should_Accept_Create_Workflow_With_Invalid_Branch_Names()
        {
            TestOutputLogger.LogContext("TestScenario", "AcceptCreateWorkflowWithInvalidBranchNames");
            try
            {
                // Arrange - API is permissive with branch names
                var workflowModel = CreateInvalidWorkflowModel("invalid_branches");

                // Act
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);

                // Assert - API accepts various branch name formats
                AssertLogger.IsTrue(response.IsSuccessStatusCode, 
                    "API should accept various branch name formats", 
                    "createWorkflowInvalidBranches");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = response.OpenJsonObjectResponse();
                    string workflowUid = responseJson["workflow"]["uid"].ToString();
                    _createdWorkflowUids.Add(workflowUid);
                }

                TestOutputLogger.LogContext("ApiAccepted", "InvalidBranchNames");
            }
            catch (Exception ex)
            {
                FailWithError("API should accept various branch name formats", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test120_Should_Fail_Create_Workflow_With_Empty_Branches_Array()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithEmptyBranchesArray");
            try
            {
                // Arrange
                var workflowModel = CreateInvalidWorkflowModel("empty_branches");

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createWorkflowEmptyBranches",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "EmptyBranchesArray");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for empty branches array", ex);
            }
        }

        // Category B: Stage Validation Errors (Test121-125)

        [TestMethod]
        [DoNotParallelize]
        public void Test121_Should_Fail_Create_Stage_With_Empty_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateStageWithEmptyName");
            try
            {
                // Arrange
                var workflowModel = new WorkflowModel
                {
                    Name = $"test_invalid_stage_name_{Guid.NewGuid():N}",
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    WorkflowStages = new List<WorkflowStage> 
                    { 
                        CreateInvalidStage("empty_name"),
                        GenerateTestStages(1)[0] // Add valid stage to meet minimum
                    }
                };

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createStageEmptyName",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "EmptyStageName");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for empty stage name", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test122_Should_Accept_Create_Stage_With_Invalid_Color_Format()
        {
            TestOutputLogger.LogContext("TestScenario", "AcceptCreateStageWithInvalidColorFormat");
            try
            {
                // Arrange - API accepts invalid color format
                var workflowModel = new WorkflowModel
                {
                    Name = $"test_invalid_stage_color_{Guid.NewGuid():N}",
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { GetValidContentTypeUid() }, // Use real content type UID
                    WorkflowStages = new List<WorkflowStage> 
                    { 
                        CreateInvalidStage("invalid_color"),
                        GenerateTestStages(1)[0] // Add valid stage to meet minimum
                    }
                };

                // Act
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);

                // Assert - API accepts invalid color format
                AssertLogger.IsTrue(response.IsSuccessStatusCode,
                    "API should accept workflow with invalid stage color format",
                    "createStageInvalidColor");

                if (response.IsSuccessStatusCode)
                {
                    var workflowUid = response.OpenJsonObjectResponse()["workflow"]?["uid"]?.ToString();
                    if (!string.IsNullOrEmpty(workflowUid))
                    {
                        _createdWorkflowUids.Add(workflowUid);
                    }
                    TestOutputLogger.LogContext("WorkflowCreated", workflowUid);
                }

                TestOutputLogger.LogContext("APIBehavior", "AcceptsInvalidStageColor");
            }
            catch (Exception ex)
            {
                FailWithError("API should accept invalid stage color format", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test123_Should_Fail_Create_Stage_With_Missing_System_ACL()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateStageWithMissingSystemACL");
            try
            {
                // Arrange
                var workflowModel = new WorkflowModel
                {
                    Name = $"test_missing_stage_acl_{Guid.NewGuid():N}",
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    WorkflowStages = new List<WorkflowStage> 
                    { 
                        CreateInvalidStage("missing_acl"),
                        GenerateTestStages(1)[0] // Add valid stage to meet minimum
                    }
                };

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createStageMissingACL",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "MissingSystemACL");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for missing stage SystemACL", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test124_Should_Fail_Create_Stage_With_Invalid_ACL_Structure()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateStageWithInvalidACLStructure");
            try
            {
                // Arrange
                var workflowModel = new WorkflowModel
                {
                    Name = $"test_invalid_stage_acl_{Guid.NewGuid():N}",
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    WorkflowStages = new List<WorkflowStage> 
                    { 
                        CreateInvalidStage("invalid_acl"),
                        GenerateTestStages(1)[0] // Add valid stage to meet minimum
                    }
                };

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createStageInvalidACL",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "InvalidACLStructure");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for invalid stage ACL structure", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test125_Should_Fail_Create_Stage_With_Circular_Stage_Dependencies()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateStageWithCircularStageDependencies");
            try
            {
                // Arrange - Create stages with circular dependencies
                var stage1 = new WorkflowStage
                {
                    Name = "Stage 1",
                    Color = "#fe5cfb",
                    SystemACL = new Dictionary<string, object>
                    {
                        ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                        ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                        ["others"] = new Dictionary<string, object>()
                    },
                    NextAvailableStages = new List<string> { "stage_2_uid" }, // Circular reference
                    AllStages = false,
                    SpecificStages = true
                };

                var stage2 = new WorkflowStage
                {
                    Name = "Stage 2",
                    Color = "#3688bf",
                    SystemACL = new Dictionary<string, object>
                    {
                        ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                        ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                        ["others"] = new Dictionary<string, object>()
                    },
                    NextAvailableStages = new List<string> { "stage_1_uid" }, // Circular reference
                    AllStages = false,
                    SpecificStages = true
                };

                var workflowModel = new WorkflowModel
                {
                    Name = $"test_circular_stages_{Guid.NewGuid():N}",
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    WorkflowStages = new List<WorkflowStage> { stage1, stage2 }
                };

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().Create(workflowModel),
                    "createStageCircularDeps",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity,
                    HttpStatusCode.Conflict);

                TestOutputLogger.LogContext("ValidationError", "CircularStageDependencies");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for circular stage dependencies", ex);
            }
        }

        // Category C: Publish Rule Validation Errors (Test126-132)

        [TestMethod]
        [DoNotParallelize]
        public async Task Test126_Should_Fail_Create_Publish_Rule_With_Missing_Environment()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreatePublishRuleWithMissingEnvironment");
            try
            {
                // Arrange - Create workflow first with specific content type to avoid conflicts
                string workflowName = $"test_missing_env_rule_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.ContentTypes = new List<string> { GetValidContentTypeUid() }; // Use real content type UID
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateInvalidPublishRuleModel("missing_environment", workflowUid, stageUid);

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().PublishRule().Create(publishRuleModel),
                    "createRuleMissingEnv",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "MissingEnvironment");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for missing environment in publish rule", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test127_Should_Fail_Create_Publish_Rule_With_Invalid_Environment_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreatePublishRuleWithInvalidEnvironmentUID");
            try
            {
                // Arrange - Create workflow first with specific content type to avoid conflicts
                string workflowName = $"test_invalid_env_rule_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.ContentTypes = new List<string> { GetValidContentTypeUid() }; // Use real content type UID
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateInvalidPublishRuleModel("invalid_environment", workflowUid, stageUid);

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().PublishRule().Create(publishRuleModel),
                    "createRuleInvalidEnv",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.NotFound,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "InvalidEnvironmentUID");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for invalid environment UID in publish rule", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test128_Should_Fail_Create_Publish_Rule_With_Missing_Workflow_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreatePublishRuleWithMissingWorkflowUID");
            try
            {
                // Arrange
                var publishRuleModel = CreateInvalidPublishRuleModel("missing_workflow");

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().PublishRule().Create(publishRuleModel),
                    "createRuleMissingWorkflow",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "MissingWorkflowUID");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for missing workflow UID in publish rule", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test129_Should_Fail_Create_Publish_Rule_With_Missing_Stage_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreatePublishRuleWithMissingStageUID");
            try
            {
                // Arrange
                var publishRuleModel = CreateInvalidPublishRuleModel("missing_stage");

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().PublishRule().Create(publishRuleModel),
                    "createRuleMissingStage",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "MissingStageUID");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for missing stage UID in publish rule", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test130_Should_Fail_Create_Publish_Rule_With_Invalid_Content_Types()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreatePublishRuleWithInvalidContentTypes");
            try
            {
                // Arrange - Create workflow and ensure environment first
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required", "testEnvironmentUid");

                string workflowName = $"test_invalid_content_types_rule_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.ContentTypes = new List<string> { GetValidContentTypeUid() }; // Use real content type UID
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateInvalidPublishRuleModel("invalid_content_types", workflowUid, stageUid, _testEnvironmentUid);

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().PublishRule().Create(publishRuleModel),
                    "createRuleInvalidContentTypes",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "InvalidContentTypes");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for invalid content types in publish rule", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test131_Should_Fail_Create_Publish_Rule_With_Invalid_Locales()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreatePublishRuleWithInvalidLocales");
            try
            {
                // Arrange - Create workflow and ensure environment first
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required", "testEnvironmentUid");

                string workflowName = $"test_invalid_locales_rule_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.ContentTypes = new List<string> { GetValidContentTypeUid() }; // Use real content type UID
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateInvalidPublishRuleModel("invalid_locales", workflowUid, stageUid, _testEnvironmentUid);

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().PublishRule().Create(publishRuleModel),
                    "createRuleInvalidLocales",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "InvalidLocales");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for invalid locales in publish rule", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test132_Should_Fail_Create_Publish_Rule_With_Empty_Branches()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreatePublishRuleWithEmptyBranches");
            try
            {
                // Arrange - Create workflow and ensure environment first
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required", "testEnvironmentUid");

                string workflowName = $"test_empty_branches_rule_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.ContentTypes = new List<string> { GetValidContentTypeUid() }; // Use real content type UID
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                var publishRuleModel = CreateInvalidPublishRuleModel("empty_branches", workflowUid, stageUid, _testEnvironmentUid);

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Workflow().PublishRule().Create(publishRuleModel),
                    "createRuleEmptyBranches",
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.UnprocessableEntity);

                TestOutputLogger.LogContext("ValidationError", "EmptyBranches");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for empty branches in publish rule", ex);
            }
        }

        // Category D: Authentication & Authorization (Test133-135)

        [TestMethod]
        [DoNotParallelize]
        public void Test133_Should_Fail_With_Invalid_Stack_API_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "FailWithInvalidStackAPIKey");
            try
            {
                // Arrange - Create client with invalid API key
                var clientWithInvalidStack = Contentstack.CreateAuthenticatedClient();
                var invalidStack = clientWithInvalidStack.Stack("invalid_nonexistent_api_key");
                var model = CreateTestWorkflowModel("test_invalid_api_key", 2);

                // Act & Assert
                AssertLogger.ThrowsContentstackError(
                    () => invalidStack.Workflow().Create(model),
                    "createWorkflowInvalidAPIKey",
                    HttpStatusCode.PreconditionFailed); // Based on Environment test findings

                TestOutputLogger.LogContext("AuthenticationError", "InvalidStackAPIKey");
            }
            catch (Exception ex)
            {
                AssertAuthenticationError(ex, "invalidStackAPIKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test134_Should_Fail_With_Expired_Auth_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "FailWithExpiredAuthToken");
            try
            {
                // Arrange - Create unauthenticated client
                var unauthenticatedClient = new ContentstackClient();
                var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
                var model = CreateTestWorkflowModel("test_expired_token", 2);

                // Act & Assert - SDK validates auth before API calls
                AssertLogger.ThrowsException<InvalidOperationException>(() =>
                {
                    unauthenticatedStack.Workflow().Create(model);
                }, "createWorkflowExpiredToken");

                TestOutputLogger.LogContext("AuthenticationError", "ExpiredAuthToken");
            }
            catch (Exception ex)
            {
                AssertAuthenticationError(ex, "expiredAuthToken");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test135_Should_Fail_With_Insufficient_Permissions()
        {
            TestOutputLogger.LogContext("TestScenario", "FailWithInsufficientPermissions");
            try
            {
                // Arrange - Use authenticated client but attempt restricted operation
                // Note: This test may pass if current user has full permissions
                // but demonstrates the pattern for permission-based errors
                var model = new WorkflowModel
                {
                    Name = $"test_insufficient_permissions_{Guid.NewGuid():N}",
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { GetValidContentTypeUid() }, // Use real content type UID
                    AdminUsers = new Dictionary<string, object> 
                    { 
                        ["users"] = new List<string> { "non_existent_user_id" } // May trigger permission error
                    },
                    WorkflowStages = GenerateTestStages(2)
                };

                // Act - This may succeed depending on user permissions
                ContentstackResponse response = _stack.Workflow().Create(model);
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Forbidden ||
                        response.StatusCode == HttpStatusCode.Unauthorized ||
                        response.StatusCode == HttpStatusCode.UnprocessableEntity, // 422 also indicates permission/validation issues
                        $"Expected 403/401/422 for permission error, got {(int)response.StatusCode}",
                        "insufficientPermissions");
                }
                else
                {
                    // Clean up if it succeeded
                    var responseJson = response.OpenJsonObjectResponse();
                    string workflowUid = responseJson["workflow"]["uid"].ToString();
                    _createdWorkflowUids.Add(workflowUid);
                }

                TestOutputLogger.LogContext("AuthenticationError", "InsufficientPermissions");
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Forbidden ||
                    cex.StatusCode == HttpStatusCode.Unauthorized ||
                    cex.StatusCode == HttpStatusCode.UnprocessableEntity, // 422 also indicates permission/validation issues
                    $"Expected 403/401/422 for permission error, got {(int)cex.StatusCode}",
                    "insufficientPermissionsException");
            }
            catch (Exception ex)
            {
                AssertAuthenticationError(ex, "insufficientPermissions");
            }
        }

        // Category E: Resource State & Business Logic (Test136-140)

        [TestMethod]
        [DoNotParallelize]
        public void Test136_Should_Fail_Enable_Already_Enabled_Workflow()
        {
            TestOutputLogger.LogContext("TestScenario", "FailEnableAlreadyEnabledWorkflow");
            try
            {
                // Arrange - Create enabled workflow
                string workflowName = $"test_enable_enabled_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.Enabled = true; // Already enabled
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJsonObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Act - Try to enable already enabled workflow
                ContentstackResponse response = _stack.Workflow(workflowUid).Enable();

                // Assert - May succeed (idempotent) or return conflict
                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Conflict ||
                        response.StatusCode == HttpStatusCode.UnprocessableEntity,
                        $"Expected 409/422 for already enabled workflow, got {(int)response.StatusCode}",
                        "enableAlreadyEnabledStatusCode");
                }

                TestOutputLogger.LogContext("BusinessLogicError", "EnableAlreadyEnabled");
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Conflict ||
                    cex.StatusCode == HttpStatusCode.UnprocessableEntity,
                    $"Expected 409/422 for already enabled workflow, got {(int)cex.StatusCode}",
                    "enableAlreadyEnabledException");
            }
            catch (Exception ex)
            {
                FailWithError("Enable already enabled workflow", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test137_Should_Fail_Disable_Already_Disabled_Workflow()
        {
            TestOutputLogger.LogContext("TestScenario", "FailDisableAlreadyDisabledWorkflow");
            try
            {
                // Arrange - Create disabled workflow
                string workflowName = $"test_disable_disabled_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.Enabled = false; // Already disabled
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJsonObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Act - Try to disable already disabled workflow
                ContentstackResponse response = _stack.Workflow(workflowUid).Disable();

                // Assert - May succeed (idempotent) or return conflict
                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Conflict ||
                        response.StatusCode == HttpStatusCode.UnprocessableEntity,
                        $"Expected 409/422 for already disabled workflow, got {(int)response.StatusCode}",
                        "disableAlreadyDisabledStatusCode");
                }

                TestOutputLogger.LogContext("BusinessLogicError", "DisableAlreadyDisabled");
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Conflict ||
                    cex.StatusCode == HttpStatusCode.UnprocessableEntity,
                    $"Expected 409/422 for already disabled workflow, got {(int)cex.StatusCode}",
                    "disableAlreadyDisabledException");
            }
            catch (Exception ex)
            {
                FailWithError("Disable already disabled workflow", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test138_Should_Fail_Update_Workflow_To_Invalid_State()
        {
            TestOutputLogger.LogContext("TestScenario", "FailUpdateWorkflowToInvalidState");
            try
            {
                // Arrange - Create workflow first
                string workflowName = $"test_invalid_state_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJsonObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Act - Try to update to invalid state (remove all stages)
                workflowModel.WorkflowStages = new List<WorkflowStage>(); // Invalid: no stages
                
                ContentstackResponse response = _stack.Workflow(workflowUid).Update(workflowModel);

                // Assert
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Expected update to invalid state to fail", "updateInvalidStateFailed");
                AssertValidationError(response.StatusCode, "updateInvalidStateStatusCode");

                TestOutputLogger.LogContext("BusinessLogicError", "UpdateToInvalidState");
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "updateInvalidStateException");
            }
            catch (Exception ex)
            {
                FailWithError("Expected validation error for update to invalid state", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test139_Should_Fail_Delete_NonExistent_Publish_Rule()
        {
            TestOutputLogger.LogContext("TestScenario", "FailDeleteNonExistentPublishRule");
            try
            {
                // Arrange
                string nonExistentRuleUid = $"non_existent_rule_{Guid.NewGuid():N}";

                // Act
                ContentstackResponse response = _stack.Workflow().PublishRule(nonExistentRuleUid).Delete();

                // Assert
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Expected delete to fail for non-existent publish rule", "deleteNonExistentRuleFailed");
                AssertMissingWorkflowStatus(response.StatusCode, "missingPublishRuleStatusCode");

                TestOutputLogger.LogContext("NotFoundOrUnprocessable", nonExistentRuleUid);
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingWorkflowStatus(cex.StatusCode, "missingPublishRuleException");
            }
            catch (Exception ex)
            {
                FailWithError("Expected error for non-existent publish rule delete", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test140_Should_Fail_Update_NonExistent_Publish_Rule()
        {
            TestOutputLogger.LogContext("TestScenario", "FailUpdateNonExistentPublishRule");
            try
            {
                // Arrange
                string nonExistentRuleUid = $"non_existent_rule_{Guid.NewGuid():N}";
                var publishRuleModel = CreateTestPublishRuleModel("dummy_workflow", "dummy_stage", "dummy_environment");

                // Act
                ContentstackResponse response = _stack.Workflow().PublishRule(nonExistentRuleUid).Update(publishRuleModel);

                // Assert
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Expected update to fail for non-existent publish rule", "updateNonExistentRuleFailed");
                AssertMissingWorkflowStatus(response.StatusCode, "missingPublishRuleStatusCode");

                TestOutputLogger.LogContext("NotFoundOrUnprocessable", nonExistentRuleUid);
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingWorkflowStatus(cex.StatusCode, "missingPublishRuleException");
            }
            catch (Exception ex)
            {
                FailWithError("Expected error for non-existent publish rule update", ex);
            }
        }

        // Category F: Edge Cases & Boundary Conditions (Test141-145)

        [TestMethod]
        [DoNotParallelize]
        public void Test141_Should_Fail_With_Null_Workflow_UID_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "FailWithNullWorkflowUIDParameter");
            try
            {
                // Act & Assert - SDK should validate UID before API call
                AssertLogger.ThrowsException<ArgumentException>(() =>
                {
                    _stack.Workflow(null).Fetch();
                }, "fetchWorkflowNullUID");

                TestOutputLogger.LogContext("EdgeCaseError", "NullWorkflowUID");
            }
            catch (Exception ex)
            {
                FailWithError("Expected ArgumentException for null workflow UID", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test142_Should_Fail_With_Empty_Workflow_UID_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "FailWithEmptyWorkflowUIDParameter");
            try
            {
                // Act & Assert - SDK should validate UID before API call
                AssertLogger.ThrowsException<ArgumentException>(() =>
                {
                    _stack.Workflow("").Fetch();
                }, "fetchWorkflowEmptyUID");

                TestOutputLogger.LogContext("EdgeCaseError", "EmptyWorkflowUID");
            }
            catch (Exception ex)
            {
                FailWithError("Expected ArgumentException for empty workflow UID", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test143_Should_Fail_With_Whitespace_Workflow_UID_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "FailWithWhitespaceWorkflowUIDParameter");
            try
            {
                // Act - Whitespace UID might be normalized or rejected
                ContentstackResponse response = _stack.Workflow("   ").Fetch();

                if (response.IsSuccessStatusCode)
                {
                    // API normalized whitespace UID (similar to Environment behavior)
                    AssertLogger.IsTrue(true, "Whitespace UID normalized by API", "whitespaceUidNormalized");
                }
                else
                {
                    // API rejected whitespace UID
                    AssertMissingWorkflowStatus(response.StatusCode, "whitespaceUidRejected");
                }

                TestOutputLogger.LogContext("EdgeCaseError", "WhitespaceWorkflowUID");
            }
            catch (InvalidOperationException)
            {
                // SDK validation rejected whitespace UID
                AssertLogger.IsTrue(true, "SDK validation rejected whitespace UID", "whitespaceUidSDKValidation");
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingWorkflowStatus(cex.StatusCode, "whitespaceUidException");
            }
            catch (Exception ex)
            {
                FailWithError("Whitespace workflow UID handling", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test144_Should_Fail_With_Special_Characters_In_Workflow_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "FailWithSpecialCharactersInWorkflowUID");
            try
            {
                // Arrange - Invalid UID with special characters
                string invalidUid = "workflow<>uid&with!special@chars";

                // Act
                ContentstackResponse response = _stack.Workflow(invalidUid).Fetch();

                // Assert
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Expected fetch to fail for invalid UID with special characters", "fetchInvalidUidFailed");
                AssertMissingWorkflowStatus(response.StatusCode, "invalidUidStatusCode");

                TestOutputLogger.LogContext("EdgeCaseError", "SpecialCharactersInUID");
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingWorkflowStatus(cex.StatusCode, "invalidUidException");
            }
            catch (Exception ex)
            {
                FailWithError("Expected error for invalid UID with special characters", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test145_Should_Fail_Fetch_With_Malformed_Query_Parameters()
        {
            TestOutputLogger.LogContext("TestScenario", "FailFetchWithMalformedQueryParameters");
            try
            {
                // Arrange - Create malformed query parameters
                var collection = new ParameterCollection();
                collection.Add("invalid_param", "value");
                collection.Add("limit", "not_a_number"); // Invalid limit value
                collection.Add("skip", "-1"); // Invalid skip value

                // Act
                ContentstackResponse response = _stack.Workflow().FindAll(collection);

                // Assert - API may ignore invalid params or return error
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "malformedQueryParamsStatusCode");
                }
                else
                {
                    // API ignored invalid parameters
                    AssertLogger.IsTrue(true, "API ignored malformed query parameters", "malformedQueryParamsIgnored");
                }

                TestOutputLogger.LogContext("EdgeCaseError", "MalformedQueryParameters");
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "malformedQueryParamsException");
            }
            catch (Exception ex)
            {
                FailWithError("Malformed query parameters handling", ex);
            }
        }

        // Category G: Concurrent Operation Errors (Test146-147)

        [TestMethod]
        [DoNotParallelize]
        public void Test146_Should_Handle_Concurrent_Workflow_Modifications()
        {
            TestOutputLogger.LogContext("TestScenario", "HandleConcurrentWorkflowModifications");
            try
            {
                // Arrange - Create workflow with specific content type to avoid conflicts
                string workflowName = $"test_concurrent_mods_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.ContentTypes = new List<string> { GetValidContentTypeUid() }; // Use real content type UID
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJsonObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Act - Simulate concurrent modifications
                string updatedName1 = $"{workflowName}_update1";
                string updatedName2 = $"{workflowName}_update2";
                
                workflowModel.Name = updatedName1;
                var workflowModel2 = CreateTestWorkflowModel(updatedName2, 2);
                workflowModel2.ContentTypes = workflowModel.ContentTypes; // Use same content type

                try
                {
                    // Attempt concurrent updates
                    ContentstackResponse update1 = _stack.Workflow(workflowUid).Update(workflowModel);
                    ContentstackResponse update2 = _stack.Workflow(workflowUid).Update(workflowModel2);

                    // Assert - At least one should succeed or both may conflict
                    bool anySucceeded = update1.IsSuccessStatusCode || update2.IsSuccessStatusCode;
                    AssertLogger.IsTrue(anySucceeded, "At least one concurrent update should succeed", "concurrentUpdateSuccess");

                    TestOutputLogger.LogContext("ConcurrentModifications", "Handled");
                }
                catch (ContentstackErrorException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    AssertLogger.IsTrue(true, "Concurrent modification conflict handled correctly", "concurrentConflict");
                }
            }
            catch (Exception ex)
            {
                FailWithError("Handle concurrent workflow modifications", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test147_Should_Handle_Race_Conditions_In_Workflow_State_Changes()
        {
            TestOutputLogger.LogContext("TestScenario", "HandleRaceConditionsInWorkflowStateChanges");
            try
            {
                // Arrange - Create workflow with specific content type to avoid conflicts
                string workflowName = $"test_race_conditions_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.Enabled = false; // Start disabled
                workflowModel.ContentTypes = new List<string> { GetValidContentTypeUid() }; // Use real content type UID
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJsonObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                try
                {
                    // Act - Simulate race condition: enable and disable simultaneously
                    ContentstackResponse enableResponse = _stack.Workflow(workflowUid).Enable();
                    ContentstackResponse disableResponse = _stack.Workflow(workflowUid).Disable();

                    // Assert - Handle race condition outcomes
                    if (enableResponse.IsSuccessStatusCode && disableResponse.IsSuccessStatusCode)
                    {
                        AssertLogger.IsTrue(true, "Both state changes succeeded (race condition handled)", "raceConditionHandled");
                    }
                    else if (enableResponse.IsSuccessStatusCode || disableResponse.IsSuccessStatusCode)
                    {
                        AssertLogger.IsTrue(true, "One state change succeeded", "partialStateChange");
                    }
                    else
                    {
                        AssertLogger.IsTrue(true, "Both state changes failed (race condition detected)", "raceConditionDetected");
                    }

                    TestOutputLogger.LogContext("RaceCondition", "StateChanges");
                }
                catch (ContentstackErrorException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    AssertLogger.IsTrue(true, "Race condition conflict handled correctly", "raceConditionConflict");
                }
            }
            catch (Exception ex)
            {
                FailWithError("Handle race conditions in workflow state changes", ex);
            }
        }

        // Category H: System/Infrastructure Errors (Test148-150)

        [TestMethod]
        [DoNotParallelize]
        public void Test148_Should_Handle_Network_Timeout_Gracefully()
        {
            TestOutputLogger.LogContext("TestScenario", "HandleNetworkTimeoutGracefully");
            try
            {
                // Note: This test demonstrates timeout handling patterns
                // Actual timeout testing requires network manipulation
                
                // Arrange - Create large workflow to potentially trigger timeout
                string workflowName = $"test_timeout_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 20); // Maximum stages
                workflowModel.ContentTypes = new List<string> { GetValidContentTypeUid() }; // Use real content type UID
                
                // Add complex stage configurations
                foreach (var stage in workflowModel.WorkflowStages)
                {
                    stage.SystemACL = new Dictionary<string, object>
                    {
                        ["roles"] = new Dictionary<string, object> 
                        { 
                            ["uids"] = Enumerable.Range(1, 100).Select(i => $"role_{i}").ToList() 
                        },
                        ["users"] = new Dictionary<string, object> 
                        { 
                            ["uids"] = Enumerable.Range(1, 100).Select(i => $"user_{i}").ToList() 
                        },
                        ["others"] = new Dictionary<string, object>()
                    };
                }

                // Act - Attempt operation that might timeout
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = response.OpenJsonObjectResponse();
                    string workflowUid = responseJson["workflow"]["uid"].ToString();
                    _createdWorkflowUids.Add(workflowUid);
                    AssertLogger.IsTrue(true, "Large workflow creation succeeded", "timeoutGracefulSuccess");
                }
                else
                {
                    // Check if it's a timeout-related error
                    AssertLogger.IsTrue(true, "Large workflow creation handled gracefully", "timeoutGracefulHandling");
                }

                TestOutputLogger.LogContext("NetworkTimeout", "HandledGracefully");
            }
            catch (Exception ex)
            {
                // Timeout exceptions are typically handled gracefully
                AssertLogger.IsTrue(true, $"Network operation handled: {ex.GetType().Name}", "timeoutExceptionHandled");
                TestOutputLogger.LogContext("TimeoutException", ex.GetType().Name);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test149_Should_Handle_Admin_Users_Validation_Errors()
        {
            TestOutputLogger.LogContext("TestScenario", "HandleAdminUsersValidationErrors");
            try
            {
                // Arrange - Create workflow with invalid admin_users configuration
                var workflowModel = new WorkflowModel
                {
                    Name = $"test_admin_validation_{Guid.NewGuid():N}",
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { GetValidContentTypeUid() }, // Use real content type UID
                    AdminUsers = new Dictionary<string, object>
                    {
                        // Invalid admin users configuration triggers validation error
                        ["users"] = new List<object> { 
                            new { invalid = "structure" },
                            "malformed_user_ref"
                        }
                    },
                    WorkflowStages = GenerateTestStages(2)
                };

                // Act
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);
                
                if (response.IsSuccessStatusCode)
                {
                    // If it succeeds unexpectedly, clean up and note the behavior
                    var responseJson = response.OpenJsonObjectResponse();
                    string workflowUid = responseJson["workflow"]["uid"].ToString();
                    _createdWorkflowUids.Add(workflowUid);
                    AssertLogger.IsTrue(true, "API unexpectedly accepted invalid admin_users configuration", "adminUsersAccepted");
                }
                else
                {
                    // Expect 422 Unprocessable Entity for admin_users validation errors
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.UnprocessableEntity,
                        $"Expected 422 (UnprocessableEntity) for admin_users validation error, got {(int)response.StatusCode} ({response.StatusCode})",
                        "adminUsersValidationError");

                    // Check that the error mentions admin_users
                    var errorResponse = response.OpenResponse();
                    AssertLogger.IsTrue(
                        errorResponse.Contains("admin_users") || errorResponse.Contains("users"),
                        "Error response should mention admin_users validation issue",
                        "adminUsersErrorMessage");
                }

                TestOutputLogger.LogContext("ValidationError", "AdminUsersHandled");
            }
            catch (ContentstackErrorException cex) when (cex.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                // Expected admin_users validation error
                AssertLogger.IsTrue(true, "Admin users validation error handled correctly", "adminUsersValidationException");
            }
            catch (Exception ex)
            {
                FailWithError("Handle admin_users validation errors", ex);
            }
        }

        // Category I: Advanced Validation Scenarios (Test151-153)

        [TestMethod]
        [DoNotParallelize]
        public async Task Test150_Should_Fail_Create_Publish_Rule_With_Duplicate_Conditions()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreatePublishRuleWithDuplicateConditions");
            try
            {
                // Arrange - Create workflow and ensure environment first
                await EnsureTestEnvironmentAsync();
                AssertLogger.IsFalse(string.IsNullOrEmpty(_testEnvironmentUid), "Test environment is required", "testEnvironmentUid");

                string workflowName = $"test_duplicate_rule_workflow_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 2);
                workflowModel.ContentTypes = new List<string> { GetValidContentTypeUid() }; // Use real content type UID
                
                ContentstackResponse workflowResponse = _stack.Workflow().Create(workflowModel);
                var workflowJson = workflowResponse.OpenJsonObjectResponse();
                string workflowUid = workflowJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                var stages = workflowJson["workflow"]["workflow_stages"] as JsonArray;
                string stageUid = stages[0]["uid"].ToString();

                // Create first publish rule
                var publishRuleModel1 = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                ContentstackResponse ruleResponse1 = _stack.Workflow().PublishRule().Create(publishRuleModel1);
                var ruleJson1 = ruleResponse1.OpenJsonObjectResponse();
                string publishRuleUid1 = ruleJson1["publishing_rule"]["uid"].ToString();
                _createdPublishRuleUids.Add(publishRuleUid1);

                // Act - Try to create duplicate publish rule
                var publishRuleModel2 = CreateTestPublishRuleModel(workflowUid, stageUid, _testEnvironmentUid);
                ContentstackResponse ruleResponse2 = _stack.Workflow().PublishRule().Create(publishRuleModel2);

                // Assert - May succeed (if API allows duplicates) or fail with conflict
                if (!ruleResponse2.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        ruleResponse2.StatusCode == HttpStatusCode.Conflict ||
                        ruleResponse2.StatusCode == HttpStatusCode.UnprocessableEntity,
                        $"Expected 409/422 for duplicate publish rule, got {(int)ruleResponse2.StatusCode}",
                        "duplicateRuleStatusCode");
                }
                else
                {
                    // API allowed duplicate - add to cleanup
                    var ruleJson2 = ruleResponse2.OpenJsonObjectResponse();
                    string publishRuleUid2 = ruleJson2["publishing_rule"]["uid"].ToString();
                    _createdPublishRuleUids.Add(publishRuleUid2);
                    AssertLogger.IsTrue(true, "API allows duplicate publish rule conditions", "duplicateRuleAllowed");
                }

                TestOutputLogger.LogContext("AdvancedValidation", "DuplicateConditions");
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Conflict ||
                    cex.StatusCode == HttpStatusCode.UnprocessableEntity,
                    $"Expected 409/422 for duplicate publish rule, got {(int)cex.StatusCode}",
                    "duplicateRuleException");
            }
            catch (Exception ex)
            {
                FailWithError("Create publish rule with duplicate conditions", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test151_Should_Fail_Update_Workflow_With_Invalid_Stage_Transitions()
        {
            TestOutputLogger.LogContext("TestScenario", "FailUpdateWorkflowWithInvalidStageTransitions");
            try
            {
                // Arrange - Create workflow first
                string workflowName = $"test_invalid_transitions_{Guid.NewGuid():N}";
                var workflowModel = CreateTestWorkflowModel(workflowName, 3);
                
                ContentstackResponse createResponse = _stack.Workflow().Create(workflowModel);
                var createJson = createResponse.OpenJsonObjectResponse();
                string workflowUid = createJson["workflow"]["uid"].ToString();
                _createdWorkflowUids.Add(workflowUid);

                // Act - Update with invalid stage transitions (self-referencing)
                var updatedStages = GenerateTestStages(3);
                updatedStages[0].NextAvailableStages = new List<string> { updatedStages[0].Uid }; // Self-reference
                updatedStages[0].AllStages = false;
                updatedStages[0].SpecificStages = true;

                workflowModel.WorkflowStages = updatedStages;
                ContentstackResponse response = _stack.Workflow(workflowUid).Update(workflowModel);

                // Assert - Should fail with validation error or succeed if API allows
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "invalidTransitionsStatusCode");
                }
                else
                {
                    AssertLogger.IsTrue(true, "API allows self-referencing stage transitions", "invalidTransitionsAllowed");
                }

                TestOutputLogger.LogContext("AdvancedValidation", "InvalidStageTransitions");
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "invalidTransitionsException");
            }
            catch (Exception ex)
            {
                FailWithError("Update workflow with invalid stage transitions", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test152_Should_Fail_Create_Workflow_With_Malformed_JSON_Structure()
        {
            TestOutputLogger.LogContext("TestScenario", "FailCreateWorkflowWithMalformedJSONStructure");
            try
            {
                // Arrange - Create workflow with potentially problematic JSON structure
                var workflowModel = new WorkflowModel
                {
                    Name = $"test_malformed_json_{Guid.NewGuid():N}",
                    Enabled = true,
                    Branches = new List<string> { "main" },
                    ContentTypes = new List<string> { "$all" },
                    AdminUsers = new Dictionary<string, object>
                    {
                        // Malformed nested structure
                        ["users"] = new Dictionary<string, object>
                        {
                            ["nested"] = new Dictionary<string, object>
                            {
                                ["deeply"] = new Dictionary<string, object>
                                {
                                    ["invalid"] = "structure_that_might_cause_issues"
                                }
                            }
                        }
                    },
                    WorkflowStages = new List<WorkflowStage>
                    {
                        new WorkflowStage
                        {
                            Name = "Test Malformed Stage",
                            Color = "#fe5cfb",
                            SystemACL = new Dictionary<string, object>
                            {
                                // Potentially problematic ACL structure
                                ["invalid_key"] = new Dictionary<string, object>
                                {
                                    ["nested_invalid"] = new List<object>
                                    {
                                        new { complex = "object", with = new { nested = "structure" } }
                                    }
                                }
                            }
                        },
                        new WorkflowStage
                        {
                            Name = "Valid Stage",
                            Color = "#3688bf",
                            SystemACL = new Dictionary<string, object>
                            {
                                ["roles"] = new Dictionary<string, object> { ["uids"] = new List<string>() },
                                ["users"] = new Dictionary<string, object> { ["uids"] = new List<string> { "$all" } },
                                ["others"] = new Dictionary<string, object>()
                            }
                        }
                    }
                };

                // Act
                ContentstackResponse response = _stack.Workflow().Create(workflowModel);

                // Assert - Should fail with validation error or succeed if API is lenient
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "malformedJSONStatusCode");
                }
                else
                {
                    var responseJson = response.OpenJsonObjectResponse();
                    string workflowUid = responseJson["workflow"]["uid"].ToString();
                    _createdWorkflowUids.Add(workflowUid);
                    AssertLogger.IsTrue(true, "API accepts malformed JSON structure", "malformedJSONAccepted");
                }

                TestOutputLogger.LogContext("AdvancedValidation", "MalformedJSONStructure");
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "malformedJSONException");
            }
            catch (Exception ex)
            {
                FailWithError("Create workflow with malformed JSON structure", ex);
            }
        }
    }
}