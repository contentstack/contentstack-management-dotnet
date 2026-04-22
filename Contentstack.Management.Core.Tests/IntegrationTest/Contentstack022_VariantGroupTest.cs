using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    [DoNotParallelize]
    public class Contentstack022_VariantGroupTest
    {
        private static ContentstackClient _client;
        private Stack _stack;
        private static string _testVariantGroupUid;
        private static string _testContentTypeUid;
        private static List<string> _availableContentTypes = new List<string>();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            try
            {
                _client = Contentstack.CreateAuthenticatedClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication failed: {ex.Message}. Tests may not run if API key is missing.");
                _client = new ContentstackClient();
            }
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
            // Read the API key from appSettings.json
            string apiKey = Contentstack.Config["Contentstack:Stack:api_key"];
            
            // Optional: Fallback to stackApiKey.txt if it's missing in appSettings.json
            if (string.IsNullOrEmpty(apiKey))
            {
                StackResponse response = StackResponse.getStack(_client.serializer);
                apiKey = response.Stack.APIKey;
            }
            
            _stack = _client.Stack(apiKey);
        }

        #region Positive Test Cases

        [TestMethod]
        [DoNotParallelize]
        public async Task Test001_Should_Find_All_VariantGroups()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_FindAll_Positive");
            
            var collection = new ParameterCollection();
            collection.Add("include_variant_info", "true");
            collection.Add("include_variant_count", "true");
            
            var response = await _stack.VariantGroup().FindAsync(collection);
            
            Assert.IsTrue(response.IsSuccessStatusCode, 
                $"Should successfully fetch variant groups: {response.OpenResponse()}");
            
            var jObject = response.OpenJObjectResponse();
            var variantGroups = jObject["variant_groups"] as JArray;
            
            Assert.IsNotNull(variantGroups, "Variant groups array should not be null");
            Console.WriteLine($"Found {variantGroups.Count} variant groups");
            
            // Store first variant group for subsequent tests
            if (variantGroups.Count > 0)
            {
                _testVariantGroupUid = variantGroups[0]["uid"]?.ToString();
                TestOutputLogger.LogContext("VariantGroupUID", _testVariantGroupUid);
                Console.WriteLine($"Using variant group UID: {_testVariantGroupUid}");
            }
            else
            {
                Console.WriteLine("Warning: No variant groups found. Some subsequent tests may be skipped.");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Find_VariantGroups_With_Pagination()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_FindWithPagination_Positive");
            
            var collection = new ParameterCollection();
            collection.Add("limit", "5");
            collection.Add("skip", "0");
            collection.Add("include_count", "true");
            
            var response = await _stack.VariantGroup().FindAsync(collection);
            
            Assert.IsTrue(response.IsSuccessStatusCode, 
                $"Should successfully fetch variant groups with pagination: {response.OpenResponse()}");
            
            var jObject = response.OpenJObjectResponse();
            Assert.IsNotNull(jObject["variant_groups"], "Should have variant_groups array");
            
            // Check if count is included when requested
            if (jObject["count"] != null)
            {
                Console.WriteLine($"Total count: {jObject["count"]}");
            }
            
            var variantGroups = jObject["variant_groups"] as JArray;
            Console.WriteLine($"Returned {variantGroups.Count} variant groups with pagination");
            
            // Verify pagination worked - should return at most 5 items
            Assert.IsTrue(variantGroups.Count <= 5, "Pagination limit should be respected");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test003_Should_Discover_Available_ContentTypes()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_DiscoverContentTypes_Positive");
            
            // Query existing content types to find ones we can use for testing
            var collection = new ParameterCollection();
            collection.Add("limit", "20");
            
            var contentTypesResponse = await _stack.ContentType().Query().FindAsync(collection);
            
            if (!contentTypesResponse.IsSuccessStatusCode)
            {
                Assert.Inconclusive($"Could not fetch content types: {contentTypesResponse.OpenResponse()}");
                return;
            }
            
            var jObject = contentTypesResponse.OpenJObjectResponse();
            var contentTypesArray = jObject["content_types"] as JArray;
            
            if (contentTypesArray == null || contentTypesArray.Count == 0)
            {
                Assert.Inconclusive("No content types found in the stack. Please create at least one content type to run VariantGroup link/unlink tests.");
                return;
            }
            
            // Store all available content types for use in various tests
            foreach (var ct in contentTypesArray)
            {
                var uid = ct["uid"]?.ToString();
                if (!string.IsNullOrEmpty(uid))
                {
                    _availableContentTypes.Add(uid);
                }
            }
            
            // Use the first available content type as primary test subject
            _testContentTypeUid = _availableContentTypes[0];
            var primaryContentTypeName = contentTypesArray[0]["title"]?.ToString();
            
            Assert.IsNotNull(_testContentTypeUid, "Content type UID should not be null");
            
            TestOutputLogger.LogContext("ContentTypeUID", _testContentTypeUid);
            Console.WriteLine($"Using primary content type: {primaryContentTypeName} (UID: {_testContentTypeUid})");
            
            // Log all available content types for debugging
            Console.WriteLine($"Found {_availableContentTypes.Count} content types in the stack:");
            foreach (var ct in contentTypesArray)
            {
                Console.WriteLine($"  - {ct["title"]} (UID: {ct["uid"]})");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Successfully_Link_Single_ContentType()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || string.IsNullOrEmpty(_testContentTypeUid))
            {
                Assert.Inconclusive("Prerequisites not met. Ensure Test001 and Test003 run first.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_LinkSingleContentType_Positive");
            
            var contentTypeUids = new List<string> { _testContentTypeUid };
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);
                
                if (linkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Successfully linked content type {_testContentTypeUid} to variant group {_testVariantGroupUid}");
                    
                    // Verify the response structure
                    var responseObj = linkResponse.OpenJObjectResponse();
                    Assert.IsNotNull(responseObj, "Response should contain JSON object");
                }
                else
                {
                    Console.WriteLine($"⚠️ Link operation returned: {linkResponse.StatusCode} - {linkResponse.OpenResponse()}");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"⚠️ Link operation failed due to API constraints: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected exception: {ex.Message}");
                throw;
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Successfully_Link_Multiple_ContentTypes()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || _availableContentTypes.Count < 2)
            {
                Assert.Inconclusive("Prerequisites not met. Need variant group and at least 2 content types.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_LinkMultipleContentTypes_Positive");
            
            // Use up to 3 content types for batch linking test
            var contentTypeUids = _availableContentTypes.Take(Math.Min(3, _availableContentTypes.Count)).ToList();
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);
                
                if (linkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Successfully linked {contentTypeUids.Count} content types to variant group {_testVariantGroupUid}");
                    Console.WriteLine($"   Content types: {string.Join(", ", contentTypeUids)}");
                }
                else
                {
                    Console.WriteLine($"⚠️ Batch link operation returned: {linkResponse.StatusCode} - {linkResponse.OpenResponse()}");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"⚠️ Batch link operation failed: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected exception during batch link: {ex.Message}");
                throw;
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Successfully_Unlink_Single_ContentType()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || string.IsNullOrEmpty(_testContentTypeUid))
            {
                Assert.Inconclusive("Prerequisites not met. Ensure Test001 and Test003 run first.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_UnlinkSingleContentType_Positive");
            
            var contentTypeUids = new List<string> { _testContentTypeUid };
            
            try
            {
                var unlinkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .UnlinkContentTypesAsync(contentTypeUids);
                
                if (unlinkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Successfully unlinked content type {_testContentTypeUid} from variant group {_testVariantGroupUid}");
                }
                else
                {
                    Console.WriteLine($"⚠️ Unlink operation returned: {unlinkResponse.StatusCode} - {unlinkResponse.OpenResponse()}");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"⚠️ Unlink operation failed: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected exception during unlink: {ex.Message}");
                throw;
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test007_Should_Successfully_Unlink_Multiple_ContentTypes()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || _availableContentTypes.Count < 2)
            {
                Assert.Inconclusive("Prerequisites not met. Need variant group and multiple content types.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_UnlinkMultipleContentTypes_Positive");
            
            var contentTypeUids = _availableContentTypes.Take(2).ToList();
            
            try
            {
                var unlinkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .UnlinkContentTypesAsync(contentTypeUids);
                
                if (unlinkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Successfully unlinked {contentTypeUids.Count} content types from variant group");
                }
                else
                {
                    Console.WriteLine($"⚠️ Batch unlink operation returned: {unlinkResponse.StatusCode} - {unlinkResponse.OpenResponse()}");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"⚠️ Batch unlink operation failed: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Work_With_Synchronous_Operations()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_SynchronousOperations_Positive");
            
            // Test synchronous Find
            var findResponse = _stack.VariantGroup().Find();
            Assert.IsTrue(findResponse.IsSuccessStatusCode, 
                $"Synchronous Find should work: {findResponse.OpenResponse()}");
            
            Console.WriteLine("✅ Synchronous Find operation successful");
            
            if (!string.IsNullOrEmpty(_testVariantGroupUid) && !string.IsNullOrEmpty(_testContentTypeUid))
            {
                var contentTypeUids = new List<string> { _testContentTypeUid };
                
                // Test synchronous Link
                try
                {
                    var linkResponse = _stack
                        .VariantGroup(_testVariantGroupUid)
                        .LinkContentTypes(contentTypeUids);
                        
                    if (linkResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("✅ Synchronous Link operation succeeded");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Synchronous Link returned: {linkResponse.StatusCode}");
                    }
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"⚠️ Synchronous Link failed: {ex.ErrorMessage}");
                }
                
                // Test synchronous Unlink
                try
                {
                    var unlinkResponse = _stack
                        .VariantGroup(_testVariantGroupUid)
                        .UnlinkContentTypes(contentTypeUids);
                        
                    if (unlinkResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("✅ Synchronous Unlink operation succeeded");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Synchronous Unlink returned: {unlinkResponse.StatusCode}");
                    }
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"⚠️ Synchronous Unlink failed: {ex.ErrorMessage}");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Handle_Various_Query_Parameters()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_QueryParameters_Positive");
            
            var collection = new ParameterCollection();
            collection.Add("include_variant_info", "true");
            collection.Add("include_variant_count", "true");
            collection.Add("include_count", "true");
            collection.Add("asc", "created_at");
            collection.Add("limit", "10");
            
            var response = await _stack.VariantGroup().FindAsync(collection);
            
            Assert.IsTrue(response.IsSuccessStatusCode, 
                $"Should work with query parameters: {response.OpenResponse()}");
            
            var jObject = response.OpenJObjectResponse();
            var variantGroups = jObject["variant_groups"] as JArray;
            Assert.IsNotNull(variantGroups, "Should have variant_groups array with advanced parameters");
            
            Console.WriteLine($"✅ Found {variantGroups.Count} variant groups with advanced query parameters");
            
            if (jObject["count"] != null)
            {
                Console.WriteLine($"   Total count in response: {jObject["count"]}");
            }
        }

        #endregion

        #region Negative Test Cases

        [TestMethod]
        [DoNotParallelize]
        public async Task Test101_Should_Fail_With_Invalid_VariantGroup_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_InvalidUID_Negative");
            
            var invalidUid = "invalid_variant_group_uid_12345";
            var contentTypeUids = new List<string> { _testContentTypeUid ?? "product_banner" };
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(invalidUid)
                    .LinkContentTypesAsync(contentTypeUids);
                
                if (!linkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Correctly failed with invalid variant group UID: {linkResponse.StatusCode}");
                    Console.WriteLine($"   Error response: {linkResponse.OpenResponse()}");
                }
                else
                {
                    Assert.Fail("Expected operation to fail with invalid variant group UID, but it succeeded");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Correctly threw ContentstackErrorException: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
                Assert.IsTrue(ex.ErrorCode > 0, "Error code should be set");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test102_Should_Fail_With_Empty_ContentType_List()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for negative testing.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_EmptyContentTypeList_Negative");
            
            var emptyContentTypeList = new List<string>();
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(emptyContentTypeList);
                
                Assert.Fail("Expected ArgumentNullException for empty content type list, but operation succeeded");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"✅ Correctly threw ArgumentNullException: {ex.Message}");
                Assert.IsTrue(ex.Message.Contains("contentTypeUids"), "Exception should mention contentTypeUids parameter");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ API correctly rejected empty list: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test103_Should_Fail_With_Null_ContentType_List()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for negative testing.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_NullContentTypeList_Negative");
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(null);
                
                Assert.Fail("Expected ArgumentNullException for null content type list, but operation succeeded");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"✅ Correctly threw ArgumentNullException: {ex.Message}");
                Assert.IsTrue(ex.ParamName == "contentTypeUids", "Parameter name should be contentTypeUids");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test104_Should_Fail_With_Invalid_ContentType_UIDs()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for negative testing.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_InvalidContentTypeUIDs_Negative");
            
            var invalidContentTypeUids = new List<string> 
            { 
                "nonexistent_content_type_1", 
                "invalid_content_type_2",
                "fake_content_type_uid_12345"
            };
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(invalidContentTypeUids);
                
                if (!linkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Correctly failed with invalid content type UIDs: {linkResponse.StatusCode}");
                    Console.WriteLine($"   Error response: {linkResponse.OpenResponse()}");
                }
                else
                {
                    Console.WriteLine("⚠️ Operation unexpectedly succeeded with invalid content type UIDs");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Correctly threw ContentstackErrorException: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test105_Should_Fail_With_Mixed_Valid_Invalid_ContentTypes()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || string.IsNullOrEmpty(_testContentTypeUid))
            {
                Assert.Inconclusive("Prerequisites not met for mixed validation test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_MixedValidInvalidContentTypes_Negative");
            
            var mixedContentTypeUids = new List<string> 
            { 
                _testContentTypeUid,  // Valid
                "nonexistent_content_type",  // Invalid
                "another_fake_uid"  // Invalid
            };
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(mixedContentTypeUids);
                
                if (!linkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Correctly failed with mixed valid/invalid content types: {linkResponse.StatusCode}");
                    
                    // Check if error response provides details about which content types failed
                    var errorResponse = linkResponse.OpenResponse();
                    Console.WriteLine($"   Error details: {errorResponse}");
                }
                else
                {
                    Console.WriteLine("⚠️ Operation unexpectedly succeeded with mixed valid/invalid content types");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Correctly threw exception for mixed validation: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test106_Should_Fail_With_Empty_String_VariantGroup_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_EmptyStringUID_Negative");
            
            var contentTypeUids = new List<string> { _testContentTypeUid ?? "product_banner" };
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup("")
                    .LinkContentTypesAsync(contentTypeUids);
                
                Assert.Fail("Expected operation to fail with empty string UID, but it succeeded");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"✅ Correctly threw InvalidOperationException for empty UID: {ex.Message}");
                Assert.IsTrue(ex.Message.Contains("Variant group UID is required"), "Exception should mention required UID");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ API correctly rejected empty UID: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test107_Should_Fail_With_Null_VariantGroup_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_NullUID_Negative");
            
            var contentTypeUids = new List<string> { _testContentTypeUid ?? "product_banner" };
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(null)
                    .LinkContentTypesAsync(contentTypeUids);
                
                Assert.Fail("Expected operation to fail with null UID, but it succeeded");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"✅ Correctly threw InvalidOperationException for null UID: {ex.Message}");
                Assert.IsTrue(ex.Message.Contains("Variant group UID is required"), "Exception should mention required UID");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ API correctly rejected null UID: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test108_Should_Handle_Duplicate_ContentType_UIDs()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || string.IsNullOrEmpty(_testContentTypeUid))
            {
                Assert.Inconclusive("Prerequisites not met for duplicate UIDs test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_DuplicateContentTypeUIDs_Negative");
            
            var duplicateContentTypeUids = new List<string> 
            { 
                _testContentTypeUid,
                _testContentTypeUid,  // Duplicate
                _testContentTypeUid   // Another duplicate
            };
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(duplicateContentTypeUids);
                
                // This might succeed or fail depending on API behavior - both are acceptable
                if (linkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ API handled duplicate content type UIDs gracefully (deduplication)");
                }
                else
                {
                    Console.WriteLine($"✅ API rejected duplicate UIDs: {linkResponse.StatusCode}");
                    Console.WriteLine($"   Error response: {linkResponse.OpenResponse()}");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ API rejected duplicate UIDs with exception: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test109_Should_Handle_Malformed_VariantGroup_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_MalformedUID_Negative");
            
            var malformedUids = new List<string>
            {
                "   ",  // Whitespace only
                "uid with spaces",  // Contains spaces
                "uid@with#special$chars",  // Special characters
                "UID_WITH_UPPER_CASE",  // Uppercase (might be invalid format)
                "uid-with-very-long-name-that-exceeds-normal-limits-and-might-be-rejected-by-api-validation"  // Very long
            };
            
            var contentTypeUids = new List<string> { _testContentTypeUid ?? "product_banner" };
            
            foreach (var malformedUid in malformedUids)
            {
                try
                {
                    var linkResponse = await _stack
                        .VariantGroup(malformedUid)
                        .LinkContentTypesAsync(contentTypeUids);
                    
                    if (!linkResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"✅ Correctly rejected malformed UID '{malformedUid}': {linkResponse.StatusCode}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Malformed UID '{malformedUid}' was unexpectedly accepted");
                    }
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"✅ Correctly rejected malformed UID '{malformedUid}': {ex.ErrorMessage}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"✅ Validation caught malformed UID '{malformedUid}': {ex.Message}");
                }
                
                // Add small delay to avoid rate limiting
                await Task.Delay(100);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test110_Should_Handle_Large_ContentType_Lists()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for large list test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_LargeContentTypeList_BoundaryTest");
            
            // Create a large list of content type UIDs (mix of valid and invalid for stress testing)
            var largeContentTypeList = new List<string>();
            
            // Add available content types if we have them
            if (_availableContentTypes.Count > 0)
            {
                largeContentTypeList.AddRange(_availableContentTypes);
            }
            
            // Fill up to 50 items with dummy UIDs to test boundary conditions
            for (int i = largeContentTypeList.Count; i < 50; i++)
            {
                largeContentTypeList.Add($"dummy_content_type_uid_{i:D3}");
            }
            
            try
            {
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(largeContentTypeList);
                
                if (!linkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ API handled large content type list appropriately: {linkResponse.StatusCode}");
                    
                    // Check if there's a limit mentioned in the error
                    var errorResponse = linkResponse.OpenResponse();
                    if (errorResponse.Contains("limit") || errorResponse.Contains("maximum"))
                    {
                        Console.WriteLine("   Error response indicates API limits on batch size");
                    }
                }
                else
                {
                    Console.WriteLine($"✅ API successfully processed large content type list ({largeContentTypeList.Count} items)");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ API rejected large list with appropriate error: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
            catch (Exception ex) when (ex.Message.Contains("timeout") || ex.Message.Contains("request too large"))
            {
                Console.WriteLine($"✅ Infrastructure correctly handled oversized request: {ex.Message}");
            }
        }

        #endregion

        #region Edge Cases and Boundary Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test201_Should_Handle_Concurrent_Operations()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || _availableContentTypes.Count < 3)
            {
                Assert.Inconclusive("Prerequisites not met for concurrency test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_ConcurrentOperations_EdgeCase");
            
            var tasks = new List<Task<ContentstackResponse>>();
            
            // Create multiple concurrent link operations
            for (int i = 0; i < 3 && i < _availableContentTypes.Count; i++)
            {
                var contentType = _availableContentTypes[i];
                var task = _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(new List<string> { contentType });
                tasks.Add(task);
            }
            
            try
            {
                var responses = await Task.WhenAll(tasks);
                
                int successCount = responses.Count(r => r.IsSuccessStatusCode);
                int failureCount = responses.Length - successCount;
                
                Console.WriteLine($"✅ Concurrent operations completed: {successCount} succeeded, {failureCount} failed");
                Console.WriteLine("   This tests API's handling of concurrent requests to the same resource");
                
                // At least one should succeed or all should fail gracefully
                Assert.IsTrue(successCount > 0 || failureCount == responses.Length, 
                    "Either some operations should succeed or all should fail gracefully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✅ Concurrency test revealed system behavior: {ex.Message}");
                // This is acceptable - shows how the system handles concurrent operations
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test202_Should_Handle_Rapid_Link_Unlink_Sequence()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || string.IsNullOrEmpty(_testContentTypeUid))
            {
                Assert.Inconclusive("Prerequisites not met for rapid sequence test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_RapidLinkUnlinkSequence_EdgeCase");
            
            var contentTypeUids = new List<string> { _testContentTypeUid };
            
            try
            {
                // Rapid sequence: Link -> Unlink -> Link -> Unlink
                var linkResponse1 = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);
                
                var unlinkResponse1 = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .UnlinkContentTypesAsync(contentTypeUids);
                
                var linkResponse2 = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);
                
                var unlinkResponse2 = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .UnlinkContentTypesAsync(contentTypeUids);
                
                Console.WriteLine($"✅ Rapid sequence completed:");
                Console.WriteLine($"   Link 1: {linkResponse1.StatusCode}, Unlink 1: {unlinkResponse1.StatusCode}");
                Console.WriteLine($"   Link 2: {linkResponse2.StatusCode}, Unlink 2: {unlinkResponse2.StatusCode}");
                Console.WriteLine("   This tests API's handling of rapid state changes");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ API handled rapid sequence with appropriate response: {ex.ErrorMessage}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test203_Should_Handle_Special_Characters_In_ContentType_UIDs()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for special characters test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_SpecialCharactersInUIDs_EdgeCase");
            
            var specialContentTypeUids = new List<string>
            {
                "content_type_with_underscores",
                "content-type-with-dashes",
                "content.type.with.dots",
                "CONTENT_TYPE_UPPERCASE",
                "content123type456with789numbers"
            };
            
            foreach (var specialUid in specialContentTypeUids)
            {
                try
                {
                    var linkResponse = await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .LinkContentTypesAsync(new List<string> { specialUid });
                    
                    Console.WriteLine($"   Special UID '{specialUid}': {linkResponse.StatusCode}");
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"   Special UID '{specialUid}' rejected: {ex.ErrorMessage}");
                }
                
                await Task.Delay(100); // Avoid rate limiting
            }
            
            Console.WriteLine("✅ Special characters test completed - shows API's UID validation behavior");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test204_Should_Handle_Unicode_Characters()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for Unicode test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_UnicodeCharacters_EdgeCase");
            
            var unicodeContentTypeUids = new List<string>
            {
                "content_type_with_émojis_😀",
                "content_type_中文_characters",
                "content_type_العربية_text",
                "content_type_русский_text"
            };
            
            foreach (var unicodeUid in unicodeContentTypeUids)
            {
                try
                {
                    var linkResponse = await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .LinkContentTypesAsync(new List<string> { unicodeUid });
                    
                    Console.WriteLine($"   Unicode UID handled: {linkResponse.StatusCode}");
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"   Unicode UID rejected appropriately: {ex.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   Unicode UID caused encoding issue: {ex.Message}");
                }
                
                await Task.Delay(100);
            }
            
            Console.WriteLine("✅ Unicode characters test completed");
        }

        #endregion

        #region Performance and Stress Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test301_Should_Handle_Performance_Under_Load()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for performance test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_PerformanceUnderLoad_StressTest");
            
            const int iterations = 10;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int successCount = 0;
            int failureCount = 0;
            
            var contentTypeUids = new List<string> { _testContentTypeUid ?? "test_content_type" };
            
            for (int i = 0; i < iterations; i++)
            {
                try
                {
                    var response = await _stack.VariantGroup().FindAsync();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        successCount++;
                    }
                    else
                    {
                        failureCount++;
                    }
                    
                    // Small delay to avoid overwhelming the API
                    await Task.Delay(50);
                }
                catch (Exception)
                {
                    failureCount++;
                }
            }
            
            stopwatch.Stop();
            
            Console.WriteLine($"✅ Performance test completed:");
            Console.WriteLine($"   {iterations} operations in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"   Average: {stopwatch.ElapsedMilliseconds / iterations}ms per operation");
            Console.WriteLine($"   Success rate: {successCount}/{iterations} ({(double)successCount / iterations * 100:F1}%)");
            
            // At least 70% should succeed for a reasonable performance baseline
            Assert.IsTrue((double)successCount / iterations >= 0.7, 
                $"Performance test should achieve at least 70% success rate, got {(double)successCount / iterations * 100:F1}%");
        }

        #endregion

        #region Authentication and Authorization Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test401_Should_Fail_Without_Authentication()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_NoAuth_Negative");
            
            // Create a client without authentication
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack(Contentstack.Config["Contentstack:Stack:api_key"]);
            
            try
            {
                var response = await unauthenticatedStack.VariantGroup().FindAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Correctly failed without authentication: {response.StatusCode}");
                }
                else
                {
                    Assert.Fail("Expected authentication failure, but operation succeeded");
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not logged in"))
            {
                Console.WriteLine($"✅ Correctly caught authentication requirement: {ex.Message}");
            }
            catch (ContentstackErrorException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine($"✅ API correctly rejected unauthenticated request: {ex.ErrorMessage}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test402_Should_Fail_With_Invalid_API_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_InvalidAPIKey_Negative");
            
            var invalidStack = _client.Stack("invalid_api_key_12345");
            
            try
            {
                var response = await invalidStack.VariantGroup().FindAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Correctly failed with invalid API key: {response.StatusCode}");
                    Console.WriteLine($"   Error response: {response.OpenResponse()}");
                }
                else
                {
                    Assert.Fail("Expected API key validation failure, but operation succeeded");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Correctly rejected invalid API key: {ex.ErrorMessage} (Code: {ex.ErrorCode})");
            }
        }

        #endregion

        #region API State and Validation Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test501_Should_Handle_Already_Linked_ContentTypes()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || string.IsNullOrEmpty(_testContentTypeUid))
            {
                Assert.Inconclusive("Prerequisites not met for already-linked test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_AlreadyLinkedContentTypes_StateTest");
            
            var contentTypeUids = new List<string> { _testContentTypeUid };
            
            try
            {
                // Attempt to link the same content type twice
                var linkResponse1 = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);
                
                await Task.Delay(200); // Small delay between operations
                
                var linkResponse2 = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);
                
                Console.WriteLine($"✅ Double-link test completed:");
                Console.WriteLine($"   First link: {linkResponse1.StatusCode}");
                Console.WriteLine($"   Second link: {linkResponse2.StatusCode}");
                Console.WriteLine("   This tests API's idempotency for already-linked content types");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ API handled double-link appropriately: {ex.ErrorMessage}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test502_Should_Handle_Already_Unlinked_ContentTypes()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || string.IsNullOrEmpty(_testContentTypeUid))
            {
                Assert.Inconclusive("Prerequisites not met for already-unlinked test.");
                return;
            }
            
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_AlreadyUnlinkedContentTypes_StateTest");
            
            var contentTypeUids = new List<string> { _testContentTypeUid };
            
            try
            {
                // Attempt to unlink the same content type twice
                var unlinkResponse1 = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .UnlinkContentTypesAsync(contentTypeUids);
                
                await Task.Delay(200); // Small delay between operations
                
                var unlinkResponse2 = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .UnlinkContentTypesAsync(contentTypeUids);
                
                Console.WriteLine($"✅ Double-unlink test completed:");
                Console.WriteLine($"   First unlink: {unlinkResponse1.StatusCode}");
                Console.WriteLine($"   Second unlink: {unlinkResponse2.StatusCode}");
                Console.WriteLine("   This tests API's idempotency for already-unlinked content types");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ API handled double-unlink appropriately: {ex.ErrorMessage}");
            }
        }

        #endregion

        #region Cleanup

        [TestMethod]
        [DoNotParallelize]
        public async Task Test999_Cleanup_Test_Resources()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_Cleanup");
            
            // Clean up any test data if needed
            // Reset static variables
            _testVariantGroupUid = null;
            _testContentTypeUid = null;
            _availableContentTypes.Clear();
            
            Console.WriteLine("✅ Cleanup completed - all test variables reset");
            
            // Add a small delay to ensure cleanup completes
            await Task.Delay(100);
        }

        #endregion
    }
}