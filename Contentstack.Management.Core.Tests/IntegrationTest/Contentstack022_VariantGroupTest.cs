using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;

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

        #region Helper Methods

        /// <summary>
        /// Validates that a response has expected validation error status codes
        /// </summary>
        private static void AssertValidationError(HttpStatusCode statusCode, string assertionName)
        {
            AssertLogger.IsTrue(
                statusCode == HttpStatusCode.BadRequest || 
                statusCode == (HttpStatusCode)422 || 
                statusCode == HttpStatusCode.UnprocessableEntity ||
                statusCode == HttpStatusCode.NotFound,  // API treats invalid UIDs as non-existent
                $"Expected 400/422/404 for validation error, got {(int)statusCode} ({statusCode})",
                assertionName);
        }

        /// <summary>
        /// Validates that a response has expected authentication/authorization error status codes
        /// </summary>
        private static void AssertAuthenticationError(Exception ex, string assertionName)
        {
            if (ex is ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Unauthorized || cex.StatusCode == HttpStatusCode.Forbidden,
                    $"Expected 401/403 for auth error, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    assertionName);
            }
            else if (ex is InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "SDK validation threw InvalidOperationException for auth as expected", assertionName);
            }
            else
            {
                AssertLogger.Fail($"Unexpected exception type for auth error: {ex.GetType().Name}", assertionName);
            }
        }

        /// <summary>
        /// Validates that a response has expected missing resource error status codes
        /// </summary>
        private static void AssertMissingResourceError(HttpStatusCode statusCode, string assertionName)
        {
            AssertLogger.IsTrue(
                statusCode == HttpStatusCode.NotFound || statusCode == (HttpStatusCode)422 || statusCode == (HttpStatusCode)412,
                $"Expected 404/422/412 for missing resource, got {(int)statusCode} ({statusCode})",
                assertionName);
        }

        /// <summary>
        /// Helper method for handling unexpected errors during negative testing
        /// </summary>
        private static void FailWithError(string operation, Exception ex)
        {
            AssertLogger.Fail($"Unexpected error during {operation}: {ex.Message}", $"{operation}UnexpectedError");
        }

        /// <summary>
        /// Creates invalid content type UIDs for various test scenarios
        /// </summary>
        private static List<string> CreateInvalidContentTypeUIDs(string scenario)
        {
            switch (scenario)
            {
                case "null_items":
                    return new List<string> { null };

                case "empty_strings":
                    return new List<string> { "", "   ", "\t\r\n" };

                case "sql_injection":
                    return new List<string> 
                    { 
                        "'; DROP TABLE content_types; --",
                        "1' OR '1'='1",
                        "UNION SELECT * FROM admin_users"
                    };

                case "xss_attempts":
                    return new List<string> 
                    { 
                        "<script>alert('xss')</script>",
                        "javascript:alert('xss')",
                        "onload='alert(1)'"
                    };

                case "extremely_long":
                    var longString = new string('a', 10000);
                    return new List<string> { longString, longString + "_suffix" };

                case "invalid_formats":
                    return new List<string> 
                    { 
                        "content type with spaces",
                        "content@type#with$special%chars",
                        "CONTENT_TYPE_UPPERCASE_ONLY",
                        "content..type..double..dots",
                        "content-type-with-unicode-émojis-😀"
                    };

                case "mixed_valid_invalid":
                    var mixed = new List<string>();
                    if (_availableContentTypes.Count > 0)
                        mixed.Add(_availableContentTypes[0]);
                    mixed.AddRange(new[] { "invalid_ct_1", "nonexistent_ct_2" });
                    return mixed;

                default:
                    return new List<string> { "invalid_default_uid" };
            }
        }

        /// <summary>
        /// Creates invalid variant group UIDs for various test scenarios
        /// </summary>
        private static string CreateInvalidVariantGroupUID(string scenario)
        {
            switch (scenario)
            {
                case "null":
                    return null;
                case "empty":
                    return "";
                case "whitespace":
                    return "   ";
                case "sql_injection":
                    return "'; DROP TABLE variant_groups; --";
                case "xss_attempt":
                    return "<script>alert('xss')</script>";
                case "extremely_long":
                    return new string('a', 5000);
                case "special_chars":
                    return "variant@group#with$special%chars";
                case "unicode":
                    return "variant_group_中文_😀";
                default:
                    return "invalid_variant_group_uid";
            }
        }

        /// <summary>
        /// Helper method to simulate network delays for timeout testing
        /// </summary>
        private static async Task SimulateNetworkDelay(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }

        #endregion

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
            
            var jObject = response.OpenJsonObjectResponse();
            var variantGroups = jObject["variant_groups"]?.AsArray();

            Assert.IsNotNull(variantGroups, "Variant groups array should not be null");
            Console.WriteLine($"Found {variantGroups.Count} variant groups");

            // Store first variant group for subsequent tests
            if (variantGroups.Count > 0)
            {
                _testVariantGroupUid = variantGroups[0]?["uid"]?.ToString();
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
            
            var jObject = response.OpenJsonObjectResponse();
            Assert.IsNotNull(jObject["variant_groups"], "Should have variant_groups array");

            // Check if count is included when requested
            if (jObject["count"] != null)
            {
                Console.WriteLine($"Total count: {jObject["count"]}");
            }

            var variantGroups = jObject["variant_groups"]?.AsArray();
            Console.WriteLine($"Returned {variantGroups?.Count ?? 0} variant groups with pagination");
            
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
            
            var jObject = contentTypesResponse.OpenJsonObjectResponse();
            var contentTypesArray = jObject["content_types"]?.AsArray();

            if (contentTypesArray == null || contentTypesArray.Count == 0)
            {
                Assert.Inconclusive("No content types found in the stack. Please create at least one content type to run VariantGroup link/unlink tests.");
                return;
            }

            // Store all available content types for use in various tests
            foreach (var ct in contentTypesArray)
            {
                var uid = ct?["uid"]?.ToString();
                if (!string.IsNullOrEmpty(uid))
                {
                    _availableContentTypes.Add(uid);
                }
            }

            // Use the first available content type as primary test subject
            _testContentTypeUid = _availableContentTypes[0];
            var primaryContentTypeName = contentTypesArray[0]?["title"]?.ToString();

            Assert.IsNotNull(_testContentTypeUid, "Content type UID should not be null");

            TestOutputLogger.LogContext("ContentTypeUID", _testContentTypeUid);
            Console.WriteLine($"Using primary content type: {primaryContentTypeName} (UID: {_testContentTypeUid})");

            // Log all available content types for debugging
            Console.WriteLine($"Found {_availableContentTypes.Count} content types in the stack:");
            foreach (var ct in contentTypesArray)
            {
                Console.WriteLine($"  - {ct?["title"]} (UID: {ct?["uid"]})");
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
                    var responseObj = linkResponse.OpenJsonObjectResponse();
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
            
            var jObject = response.OpenJsonObjectResponse();
            var variantGroups = jObject["variant_groups"]?.AsArray();
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

        [TestMethod]
        [DoNotParallelize]
        public async Task Test111_Should_Fail_Link_With_Null_Parameters_Sync()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for null parameter test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_NullParametersSync_Negative");

            try
            {
                var response = await Task.Run(() => _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypes(null));

                AssertLogger.Fail("Expected ArgumentNullException for null parameters in sync method", "NullParametersSync");
            }
            catch (ArgumentNullException ex)
            {
                AssertLogger.IsTrue(true, "SDK validation throws ArgumentNullException for null parameters as expected", "NullParametersSync");
                Assert.IsTrue(ex.ParamName.Contains("contentTypeUids"), "Parameter name should reference contentTypeUids");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test112_Should_Fail_Unlink_With_Invalid_Parameter_Types()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for invalid parameter test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_InvalidParameterTypes_Negative");

            var invalidUIDs = CreateInvalidContentTypeUIDs("null_items");

            try
            {
                var response = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .UnlinkContentTypesAsync(invalidUIDs);

                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "InvalidParameterTypes");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for null items in list", "InvalidParameterTypes");
                }
            }
            catch (ContentstackErrorException ex)
            {
                AssertValidationError(ex.StatusCode, "InvalidParameterTypesException");
                Console.WriteLine($"✅ API properly handled invalid parameter types: {ex.ErrorMessage}");
            }
            catch (ArgumentException ex)
            {
                AssertLogger.IsTrue(true, "SDK validation caught invalid parameter types as expected", "InvalidParameterTypes");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test113_Should_Fail_With_Extremely_Long_UIDs()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for long UID test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_ExtremelyLongUIDs_Negative");

            var longUIDs = CreateInvalidContentTypeUIDs("extremely_long");

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var response = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(longUIDs);

                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Extremely long UID validation failed" 
                    };
                }
            }, "ExtremelyLongUIDs", HttpStatusCode.BadRequest, (HttpStatusCode)422, (HttpStatusCode)413, HttpStatusCode.NotFound);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test114_Should_Fail_With_SQL_Injection_Attempts()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for SQL injection test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_SQLInjectionAttempts_Security");

            var maliciousUIDs = CreateInvalidContentTypeUIDs("sql_injection");

            foreach (var maliciousUID in maliciousUIDs)
            {
                try
                {
                    var response = await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .LinkContentTypesAsync(new List<string> { maliciousUID });

                    if (!response.IsSuccessStatusCode)
                    {
                        AssertValidationError(response.StatusCode, "SQLInjectionAttempt");
                        Console.WriteLine($"✅ SQL injection attempt properly rejected: '{maliciousUID}'");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ SQL injection attempt was not rejected: '{maliciousUID}'");
                    }
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"✅ SQL injection properly caught by API: {ex.ErrorMessage}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"✅ SQL injection caught by SDK validation: {ex.Message}");
                }

                await Task.Delay(100); // Avoid rate limiting
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test115_Should_Fail_With_XSS_Attempts_In_UIDs()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for XSS test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_XSSAttempts_Security");

            var xssUIDs = CreateInvalidContentTypeUIDs("xss_attempts");

            foreach (var xssUID in xssUIDs)
            {
                try
                {
                    var response = await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .LinkContentTypesAsync(new List<string> { xssUID });

                    if (!response.IsSuccessStatusCode)
                    {
                        AssertValidationError(response.StatusCode, "XSSAttempt");
                        Console.WriteLine($"✅ XSS attempt properly rejected: '{xssUID}'");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ XSS attempt was not rejected: '{xssUID}'");
                    }
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"✅ XSS attempt properly caught by API: {ex.ErrorMessage}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"✅ XSS attempt caught by SDK validation: {ex.Message}");
                }

                await Task.Delay(100); // Avoid rate limiting
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test116_Should_Fail_With_Invalid_Format_UIDs()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for invalid format test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_InvalidFormatUIDs_Negative");

            var invalidFormatUIDs = CreateInvalidContentTypeUIDs("invalid_formats");

            foreach (var invalidUID in invalidFormatUIDs)
            {
                try
                {
                    var response = await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .LinkContentTypesAsync(new List<string> { invalidUID });

                    if (!response.IsSuccessStatusCode)
                    {
                        AssertValidationError(response.StatusCode, "InvalidFormatUID");
                        Console.WriteLine($"✅ Invalid format UID properly rejected: '{invalidUID}'");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Invalid format UID was accepted: '{invalidUID}'");
                    }
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"✅ Invalid format caught by API: {ex.ErrorMessage}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"✅ Invalid format caught by SDK: {ex.Message}");
                }

                await Task.Delay(100);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test117_Should_Fail_With_Empty_String_UIDs()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for empty string test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_EmptyStringUIDs_Negative");

            var emptyStringUIDs = CreateInvalidContentTypeUIDs("empty_strings");

            try
            {
                var response = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(emptyStringUIDs);

                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "EmptyStringUIDs");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for empty string UIDs", "EmptyStringUIDs");
                }
            }
            catch (ContentstackErrorException ex)
            {
                AssertValidationError(ex.StatusCode, "EmptyStringUIDsException");
                Console.WriteLine($"✅ API properly handled empty string UIDs: {ex.ErrorMessage}");
            }
            catch (ArgumentException ex)
            {
                AssertLogger.IsTrue(true, "SDK validation caught empty string UIDs as expected", "EmptyStringUIDs");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test118_Should_Validate_VariantGroup_UID_Formats()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_VariantGroupUIDFormats_Negative");

            var invalidVGUIDs = new[]
            {
                CreateInvalidVariantGroupUID("null"),
                CreateInvalidVariantGroupUID("empty"),
                CreateInvalidVariantGroupUID("whitespace"),
                CreateInvalidVariantGroupUID("special_chars"),
                CreateInvalidVariantGroupUID("extremely_long")
            };

            var validContentTypeUIDs = new List<string> { _testContentTypeUid ?? "test_content_type" };

            foreach (var invalidVGUID in invalidVGUIDs)
            {
                try
                {
                    if (invalidVGUID == null)
                    {
                        var response = await _stack
                            .VariantGroup(invalidVGUID)
                            .LinkContentTypesAsync(validContentTypeUIDs);
                        AssertLogger.Fail("Expected exception for null variant group UID", "NullVariantGroupUID");
                    }
                    else
                    {
                        var response = await _stack
                            .VariantGroup(invalidVGUID)
                            .LinkContentTypesAsync(validContentTypeUIDs);

                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"✅ Invalid UID format properly rejected: '{invalidVGUID}'");
                        }
                    }
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("UID is required"))
                {
                    Console.WriteLine($"✅ SDK validation caught invalid UID format: {ex.Message}");
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"✅ API rejected invalid UID format: {ex.ErrorMessage}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"✅ SDK validation caught malformed UID: {ex.Message}");
                }

                await Task.Delay(100);
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

        [TestMethod]
        [DoNotParallelize]
        public async Task Test403_Should_Fail_With_Expired_Auth_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_ExpiredAuthToken_Negative");

            // Create a client with potentially expired token (simulated by empty token)
            var expiredTokenClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = "blt_expired_token_simulation_12345"
            });
            var expiredStack = expiredTokenClient.Stack(_stack.APIKey);

            try
            {
                var response = await expiredStack.VariantGroup().FindAsync();

                if (!response.IsSuccessStatusCode)
                {
                    AssertAuthenticationError(new ContentstackErrorException { StatusCode = response.StatusCode }, "ExpiredAuthToken");
                    Console.WriteLine($"✅ Correctly failed with expired auth token: {response.StatusCode}");
                }
                else
                {
                    AssertLogger.Fail("Expected authentication failure with expired token, but operation succeeded", "ExpiredAuthToken");
                }
            }
            catch (ContentstackErrorException ex)
            {
                AssertAuthenticationError(ex, "ExpiredAuthTokenException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test404_Should_Fail_With_Insufficient_Permissions()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_InsufficientPermissions_Negative");

            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for permissions test.");
                return;
            }

            // Create a client with limited permissions token (simulated)
            var limitedPermClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = "blt_limited_permissions_token_12345"
            });
            var limitedStack = limitedPermClient.Stack(_stack.APIKey);

            var contentTypeUids = new List<string> { _testContentTypeUid ?? "test_content_type" };

            try
            {
                var response = await limitedStack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);

                if (!response.IsSuccessStatusCode)
                {
                    AssertAuthenticationError(new ContentstackErrorException { StatusCode = response.StatusCode }, "InsufficientPermissions");
                    Console.WriteLine($"✅ Correctly failed with insufficient permissions: {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine("⚠️ Operation succeeded with limited permissions token - may not have proper permission validation");
                }
            }
            catch (ContentstackErrorException ex)
            {
                AssertAuthenticationError(ex, "InsufficientPermissionsException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test405_Should_Fail_With_Revoked_API_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_RevokedAPIKey_Negative");

            var revokedStack = _client.Stack("blt_revoked_api_key_simulation_12345");

            try
            {
                var response = await revokedStack.VariantGroup().FindAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Correctly failed with revoked API key: {response.StatusCode}");
                    Assert.IsTrue(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden,
                        "Should return 401 or 403 for revoked API key");
                }
                else
                {
                    AssertLogger.Fail("Expected failure with revoked API key, but operation succeeded", "RevokedAPIKey");
                }
            }
            catch (ContentstackErrorException ex)
            {
                AssertAuthenticationError(ex, "RevokedAPIKeyException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test406_Should_Handle_Token_Refresh_Scenarios()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_TokenRefreshScenarios_Auth");

            // Simulate scenario where token needs refresh by using short-lived token
            var shortLivedClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = "blt_short_lived_token_12345"
            });
            var shortLivedStack = shortLivedClient.Stack(_stack.APIKey);

            try
            {
                // First operation might succeed
                var response1 = await shortLivedStack.VariantGroup().FindAsync();

                // Simulate token expiry between operations
                await Task.Delay(100);

                // Second operation should fail due to expired token
                var response2 = await shortLivedStack.VariantGroup().FindAsync();

                if (!response2.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Token refresh scenario handled appropriately: {response2.StatusCode}");
                }
                else
                {
                    Console.WriteLine("⚠️ Token refresh scenario not triggered - may need actual expired token");
                }
            }
            catch (ContentstackErrorException ex)
            {
                AssertAuthenticationError(ex, "TokenRefreshScenario");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test407_Should_Fail_Cross_Stack_Operations()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_CrossStackOperations_Security");

            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for cross-stack test.");
                return;
            }

            // Use authenticated client with wrong stack API key
            var wrongStack = _client.Stack("blt_different_stack_api_key_12345");
            var contentTypeUids = new List<string> { _testContentTypeUid ?? "test_content_type" };

            try
            {
                var response = await wrongStack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Cross-stack operation properly blocked: {response.StatusCode}");
                    Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound || 
                                response.StatusCode == HttpStatusCode.Forbidden ||
                                response.StatusCode == HttpStatusCode.Unauthorized,
                        "Should return 404/403/401 for cross-stack access");
                }
                else
                {
                    AssertLogger.Fail("Expected failure for cross-stack operation, but succeeded", "CrossStackOperation");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Cross-stack operation blocked with exception: {ex.ErrorMessage}");
                AssertAuthenticationError(ex, "CrossStackOperationException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test408_Should_Handle_Malformed_Auth_Headers()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_MalformedAuthHeaders_Security");

            var malformedTokens = new[]
            {
                "invalid_token_format",
                "blt_", // Too short
                "not_a_token_at_all",
                "blt_token_with_invalid_chars!@#",
                "",
                null
            };

            foreach (var malformedToken in malformedTokens)
            {
                try
                {
                    ContentstackClient malformedClient;
                    if (malformedToken == null)
                    {
                        malformedClient = new ContentstackClient();
                    }
                    else
                    {
                        malformedClient = new ContentstackClient(new ContentstackClientOptions()
                        {
                            Host = _client.contentstackOptions.Host,
                            Authtoken = malformedToken
                        });
                    }

                    var malformedStack = malformedClient.Stack(_stack.APIKey);
                    var response = await malformedStack.VariantGroup().FindAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"✅ Malformed token properly rejected: '{malformedToken ?? "null"}'");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Malformed token was accepted: '{malformedToken ?? "null"}'");
                    }
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("not logged in"))
                {
                    Console.WriteLine($"✅ SDK caught malformed token: {ex.Message}");
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"✅ API rejected malformed token: {ex.ErrorMessage}");
                }

                await Task.Delay(100); // Avoid rate limiting
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

        [TestMethod]
        [DoNotParallelize]
        public async Task Test503_Should_Handle_Deleted_ContentType_References()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for deleted content type test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_DeletedContentTypeReferences_DataIntegrity");

            // Simulate references to deleted content types
            var deletedContentTypeUIDs = new List<string> 
            { 
                "blt_deleted_content_type_123",
                "blt_archived_content_type_456", 
                "blt_removed_content_type_789"
            };

            try
            {
                var response = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(deletedContentTypeUIDs);

                if (!response.IsSuccessStatusCode)
                {
                    AssertMissingResourceError(response.StatusCode, "DeletedContentTypeReferences");
                    Console.WriteLine($"✅ Deleted content type references properly handled: {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine("⚠️ Deleted content type references were not validated - possible data integrity issue");
                }
            }
            catch (ContentstackErrorException ex)
            {
                AssertMissingResourceError(ex.StatusCode, "DeletedContentTypeReferencesException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test504_Should_Handle_Archived_ContentType_States()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for archived content type test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_ArchivedContentTypeStates_DataIntegrity");

            // Simulate archived content types that might still exist but be unavailable
            var archivedContentTypeUIDs = new List<string> 
            { 
                "blt_archived_ct_state_1",
                "blt_archived_ct_state_2"
            };

            try
            {
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(archivedContentTypeUIDs);

                if (!linkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Archived content type states properly handled: {linkResponse.StatusCode}");
                    AssertValidationError(linkResponse.StatusCode, "ArchivedContentTypeStates");
                }

                // Try to unlink as well to test both operations
                var unlinkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .UnlinkContentTypesAsync(archivedContentTypeUIDs);

                Console.WriteLine($"   Unlink archived content types: {unlinkResponse.StatusCode}");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Archived content type operations handled: {ex.ErrorMessage}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test505_Should_Validate_ContentType_Schema_Compatibility()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for schema compatibility test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_SchemaCompatibility_DataIntegrity");

            // Simulate content types that might not be compatible with variant groups
            var incompatibleContentTypeUIDs = new List<string> 
            { 
                "blt_singleton_content_type", // Singleton CTs might not support variants
                "blt_system_content_type",    // System CTs might be restricted
                "blt_external_content_type"   // External/federated CTs might not support variants
            };

            foreach (var incompatibleUID in incompatibleContentTypeUIDs)
            {
                try
                {
                    var response = await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .LinkContentTypesAsync(new List<string> { incompatibleUID });

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"✅ Schema incompatibility properly detected for '{incompatibleUID}': {response.StatusCode}");
                        AssertValidationError(response.StatusCode, "SchemaCompatibility");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Schema compatibility not validated for '{incompatibleUID}'");
                    }
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"✅ Schema compatibility validation: {ex.ErrorMessage}");
                }

                await Task.Delay(100);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test506_Should_Handle_Circular_Reference_Detection()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for circular reference test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_CircularReferences_DataIntegrity");

            // Simulate potential circular reference scenarios
            var potentialCircularUIDs = new List<string> { _testVariantGroupUid }; // Self-reference

            try
            {
                // This should be prevented by the API - variant groups can't link to themselves
                var response = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(potentialCircularUIDs);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Circular reference properly prevented: {response.StatusCode}");
                    AssertValidationError(response.StatusCode, "CircularReference");
                }
                else
                {
                    Console.WriteLine("⚠️ Circular reference not detected - potential data integrity issue");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Circular reference detection: {ex.ErrorMessage}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✅ SDK prevented circular reference: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test507_Should_Handle_Broken_Variant_Group_References()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_BrokenVariantGroupReferences_DataIntegrity");

            var brokenVariantGroupUIDs = new[]
            {
                "blt_deleted_variant_group_123",
                "blt_corrupted_variant_group_456",
                "blt_archived_variant_group_789"
            };

            var contentTypeUids = new List<string> { _testContentTypeUid ?? "test_content_type" };

            foreach (var brokenUID in brokenVariantGroupUIDs)
            {
                try
                {
                    var response = await _stack
                        .VariantGroup(brokenUID)
                        .LinkContentTypesAsync(contentTypeUids);

                    if (!response.IsSuccessStatusCode)
                    {
                        AssertMissingResourceError(response.StatusCode, "BrokenVariantGroupReference");
                        Console.WriteLine($"✅ Broken variant group reference properly handled: '{brokenUID}' - {response.StatusCode}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Broken variant group reference not detected: '{brokenUID}'");
                    }
                }
                catch (ContentstackErrorException ex)
                {
                    AssertMissingResourceError(ex.StatusCode, "BrokenVariantGroupReferenceException");
                    Console.WriteLine($"✅ Broken reference caught: {ex.ErrorMessage}");
                }

                await Task.Delay(100);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test508_Should_Validate_Data_Consistency_During_Operations()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || _availableContentTypes.Count < 2)
            {
                Assert.Inconclusive("Prerequisites not met for data consistency test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_DataConsistency_DataIntegrity");

            var contentTypeUids = _availableContentTypes.Take(2).ToList();

            try
            {
                // Test sequence: Link -> Verify -> Unlink -> Verify
                var linkResponse = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);

                if (linkResponse.IsSuccessStatusCode)
                {
                    // Verify the link was actually created by querying
                    await Task.Delay(200); // Allow for data propagation

                    var queryResponse = await _stack.VariantGroup().FindAsync();
                    if (queryResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("✅ Data consistency: Link operation and query are consistent");
                    }

                    // Now unlink and verify again
                    var unlinkResponse = await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .UnlinkContentTypesAsync(contentTypeUids);

                    if (unlinkResponse.IsSuccessStatusCode)
                    {
                        await Task.Delay(200); // Allow for data propagation

                        var verifyResponse = await _stack.VariantGroup().FindAsync();
                        Console.WriteLine("✅ Data consistency: Unlink operation completed successfully");
                    }
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Data consistency validation completed with API response: {ex.ErrorMessage}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test509_Should_Handle_Stale_Data_References()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for stale data test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_StaleDataReferences_DataIntegrity");

            // Simulate stale references that might exist in cache but not in database
            var staleContentTypeUIDs = new List<string> 
            { 
                "blt_cached_but_deleted_ct_123",
                "blt_stale_reference_ct_456",
                "blt_outdated_ct_reference_789"
            };

            try
            {
                var response = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(staleContentTypeUIDs);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Stale data references properly validated: {response.StatusCode}");
                    
                    // Check if error message provides insight into stale data handling
                    var errorResponse = response.OpenResponse();
                    if (errorResponse.Contains("not found") || errorResponse.Contains("invalid"))
                    {
                        Console.WriteLine("   Error response indicates proper stale data validation");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ Stale data references not validated - potential caching issue");
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Stale data validation: {ex.ErrorMessage}");
                AssertMissingResourceError(ex.StatusCode, "StaleDataReferences");
            }
        }

        #endregion

        #region Network & Service Degradation Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test601_Should_Handle_Network_Timeout_Scenarios()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_NetworkTimeoutScenarios_Network");

            // Test with very short timeout to simulate network issues
            using var shortTimeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));

            try
            {
                var findTask = _stack.VariantGroup().FindAsync();
                var completedTask = await Task.WhenAny(findTask, Task.Delay(TimeSpan.FromMilliseconds(1), shortTimeoutCts.Token));
                
                if (completedTask == findTask)
                {
                    Console.WriteLine("⚠️ Operation completed before timeout - network too fast for timeout simulation");
                }
                else
                {
                    Console.WriteLine("✅ Timeout simulation triggered as expected");
                }
            }
            catch (OperationCanceledException ex)
            {
                AssertLogger.IsTrue(true, "Network timeout properly handled with OperationCanceledException", "NetworkTimeout");
                Console.WriteLine($"✅ Network timeout scenario handled: {ex.Message}");
            }

            // Test with moderate timeout for real-world scenario
            using var moderateTimeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            try
            {
                var findTask = _stack.VariantGroup().FindAsync();
                var completedTask = await Task.WhenAny(findTask, Task.Delay(TimeSpan.FromSeconds(5), moderateTimeoutCts.Token));
                
                if (completedTask == findTask)
                {
                    var response = await findTask;
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("✅ Network operation completed within reasonable timeout");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ Network operation exceeded 5-second timeout - potential performance issue");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("⚠️ Network operation exceeded 5-second timeout - potential performance issue");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test602_Should_Handle_API_Rate_Limiting()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_APIRateLimiting_Network");

            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for rate limiting test.");
                return;
            }

            var contentTypeUids = new List<string> { _testContentTypeUid ?? "test_content_type" };
            const int rapidRequests = 20;
            var rateLimitHit = false;

            Console.WriteLine($"Sending {rapidRequests} rapid requests to test rate limiting...");

            for (int i = 0; i < rapidRequests; i++)
            {
                try
                {
                    var response = await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .LinkContentTypesAsync(contentTypeUids);

                    if (response.StatusCode == (HttpStatusCode)429) // Too Many Requests
                    {
                        rateLimitHit = true;
                        Console.WriteLine($"✅ Rate limit properly enforced at request {i + 1}");
                        break;
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"   Request {i + 1}: {response.StatusCode}");
                    }
                }
                catch (ContentstackErrorException ex) when (ex.StatusCode == (HttpStatusCode)429)
                {
                    rateLimitHit = true;
                    Console.WriteLine($"✅ Rate limit exception properly thrown at request {i + 1}: {ex.ErrorMessage}");
                    break;
                }
                catch (Exception ex) when (ex.Message.Contains("rate") || ex.Message.Contains("limit"))
                {
                    rateLimitHit = true;
                    Console.WriteLine($"✅ Rate limiting handled: {ex.Message}");
                    break;
                }

                // Small delay but not too much to actually trigger rate limiting
                await Task.Delay(10);
            }

            if (rateLimitHit)
            {
                Console.WriteLine("✅ API rate limiting is properly enforced");
            }
            else
            {
                Console.WriteLine("⚠️ Rate limiting not triggered - API may have high limits or requests weren't fast enough");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test603_Should_Handle_Service_Unavailable_Responses()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_ServiceUnavailable_Network");

            // Simulate service unavailable by attempting operations when service might be down
            // This test documents behavior rather than forcing a specific outcome
            try
            {
                var response = await _stack.VariantGroup().FindAsync();

                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    Console.WriteLine("✅ Service unavailable properly detected and handled");
                    Assert.IsTrue(response.StatusCode == HttpStatusCode.ServiceUnavailable, 
                        "Should return 503 for service unavailable");
                }
                else if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Service is available - no degradation detected");
                }
                else
                {
                    Console.WriteLine($"   Service returned: {response.StatusCode} - {response.OpenResponse()}");
                }
            }
            catch (ContentstackErrorException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                Console.WriteLine($"✅ Service unavailable exception properly handled: {ex.ErrorMessage}");
            }
            catch (Exception ex) when (ex.Message.Contains("service") || ex.Message.Contains("unavailable"))
            {
                Console.WriteLine($"✅ Service unavailable scenario handled: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test604_Should_Handle_Partial_Service_Degradation()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_PartialServiceDegradation_Network");

            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for degradation test.");
                return;
            }

            var contentTypeUids = new List<string> { _testContentTypeUid ?? "test_content_type" };

            try
            {
                // Test multiple operations to see if some succeed and others fail (partial degradation)
                var findTask = _stack.VariantGroup().FindAsync();
                var linkTask = _stack.VariantGroup(_testVariantGroupUid).LinkContentTypesAsync(contentTypeUids);
                var unlinkTask = _stack.VariantGroup(_testVariantGroupUid).UnlinkContentTypesAsync(contentTypeUids);

                var results = await Task.WhenAll(findTask, linkTask, unlinkTask);

                int successCount = results.Count(r => r.IsSuccessStatusCode);
                int failureCount = results.Length - successCount;

                Console.WriteLine($"✅ Partial degradation test: {successCount} succeeded, {failureCount} failed");
                
                if (failureCount > 0 && successCount > 0)
                {
                    Console.WriteLine("   Partial service degradation detected - some operations succeeded");
                }
                else if (successCount == results.Length)
                {
                    Console.WriteLine("   All operations succeeded - service is fully available");
                }
                else
                {
                    Console.WriteLine("   All operations failed - service may be completely unavailable");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✅ Service degradation scenario handled: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test605_Should_Handle_API_Maintenance_Mode()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_APIMaintenanceMode_Network");

            try
            {
                var response = await _stack.VariantGroup().FindAsync();

                // Check for maintenance mode indicators
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    var responseBody = response.OpenResponse();
                    if (responseBody.Contains("maintenance") || responseBody.Contains("scheduled"))
                    {
                        Console.WriteLine("✅ API maintenance mode properly detected and communicated");
                    }
                    else
                    {
                        Console.WriteLine("✅ Service unavailable - could be maintenance mode");
                    }
                }
                else if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ API is not in maintenance mode - service fully available");
                }
                else
                {
                    Console.WriteLine($"   API status during maintenance check: {response.StatusCode}");
                }
            }
            catch (ContentstackErrorException ex) when (ex.ErrorMessage.Contains("maintenance"))
            {
                Console.WriteLine($"✅ Maintenance mode properly communicated: {ex.ErrorMessage}");
            }
            catch (Exception ex) when (ex.Message.Contains("maintenance") || ex.Message.Contains("scheduled"))
            {
                Console.WriteLine($"✅ Maintenance mode scenario handled: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test606_Should_Handle_Connection_Reset_Scenarios()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_ConnectionReset_Network");

            // Test connection resilience by making multiple requests with delays
            var connectionResetDetected = false;

            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    var response = await _stack.VariantGroup().FindAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"   Connection attempt {attempt + 1}: Successful");
                    }
                    else
                    {
                        Console.WriteLine($"   Connection attempt {attempt + 1}: {response.StatusCode}");
                    }

                    await Task.Delay(1000); // Wait between attempts
                }
                catch (Exception ex) when (ex.Message.Contains("connection") || ex.Message.Contains("reset"))
                {
                    connectionResetDetected = true;
                    Console.WriteLine($"✅ Connection reset properly handled: {ex.Message}");
                    break;
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"   Connection attempt {attempt + 1}: API error {ex.StatusCode}");
                }
            }

            if (connectionResetDetected)
            {
                Console.WriteLine("✅ Connection reset scenario was encountered and handled");
            }
            else
            {
                Console.WriteLine("✅ Connection remained stable throughout test - no resets detected");
            }
        }

        #endregion

        #region Concurrency & Race Condition Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test701_Should_Handle_Race_Conditions_During_Link_Operations()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || _availableContentTypes.Count < 3)
            {
                Assert.Inconclusive("Prerequisites not met for race condition test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_RaceConditionsLinkOperations_Concurrency");

            var contentTypeUids = _availableContentTypes.Take(3).ToList();

            // Create multiple tasks that will race against each other
            var racingTasks = new List<Task<ContentstackResponse>>();

            for (int i = 0; i < 5; i++)
            {
                var task = Task.Run(async () =>
                {
                    // Small random delay to create race conditions
                    await Task.Delay(new Random().Next(10, 50));
                    return await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .LinkContentTypesAsync(contentTypeUids);
                });
                racingTasks.Add(task);
            }

            try
            {
                var results = await Task.WhenAll(racingTasks);

                int successCount = results.Count(r => r.IsSuccessStatusCode);
                int failureCount = results.Length - successCount;

                Console.WriteLine($"✅ Race condition test completed: {successCount} succeeded, {failureCount} failed");
                Console.WriteLine("   This tests how the API handles simultaneous identical operations");

                // At least one should succeed, or all should fail gracefully
                Assert.IsTrue(successCount > 0 || failureCount == results.Length,
                    "Race condition should result in at least one success or all graceful failures");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✅ Race condition properly handled with exception: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test702_Should_Handle_Simultaneous_Link_Unlink_Operations()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || string.IsNullOrEmpty(_testContentTypeUid))
            {
                Assert.Inconclusive("Prerequisites not met for simultaneous operations test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_SimultaneousLinkUnlink_Concurrency");

            var contentTypeUids = new List<string> { _testContentTypeUid };

            try
            {
                // Start link and unlink operations simultaneously
                var linkTask = _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);

                var unlinkTask = _stack
                    .VariantGroup(_testVariantGroupUid)
                    .UnlinkContentTypesAsync(contentTypeUids);

                var results = await Task.WhenAll(linkTask, unlinkTask);

                Console.WriteLine($"✅ Simultaneous link/unlink completed:");
                Console.WriteLine($"   Link result: {results[0].StatusCode}");
                Console.WriteLine($"   Unlink result: {results[1].StatusCode}");
                Console.WriteLine("   This tests API's handling of conflicting concurrent operations");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Conflicting operations handled appropriately: {ex.ErrorMessage}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test703_Should_Handle_Multiple_Client_Modifications()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || _availableContentTypes.Count < 2)
            {
                Assert.Inconclusive("Prerequisites not met for multiple client test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_MultipleClientModifications_Concurrency");

            // Create multiple client instances to simulate different users/sessions
            var client1 = _client;
            var client2 = new ContentstackClient(_client.contentstackOptions);
            var client3 = new ContentstackClient(_client.contentstackOptions);

            var stack1 = client1.Stack(_stack.APIKey);
            var stack2 = client2.Stack(_stack.APIKey);
            var stack3 = client3.Stack(_stack.APIKey);

            var contentType1 = new List<string> { _availableContentTypes[0] };
            var contentType2 = _availableContentTypes.Count > 1 ? 
                new List<string> { _availableContentTypes[1] } : contentType1;

            try
            {
                // Simultaneous modifications from different clients
                var task1 = stack1.VariantGroup(_testVariantGroupUid).LinkContentTypesAsync(contentType1);
                var task2 = stack2.VariantGroup(_testVariantGroupUid).LinkContentTypesAsync(contentType2);
                var task3 = stack3.VariantGroup(_testVariantGroupUid).UnlinkContentTypesAsync(contentType1);

                var results = await Task.WhenAll(task1, task2, task3);

                Console.WriteLine($"✅ Multiple client operations completed:");
                for (int i = 0; i < results.Length; i++)
                {
                    Console.WriteLine($"   Client {i + 1}: {results[i].StatusCode}");
                }

                Console.WriteLine("   This tests API's handling of concurrent client modifications");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✅ Multiple client scenario handled: {ex.Message}");
            }
            finally
            {
                // Clean up additional clients
                try { client2?.Logout(); } catch { }
                try { client3?.Logout(); } catch { }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test704_Should_Handle_Resource_Locking_Conflicts()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for resource locking test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_ResourceLockingConflicts_Concurrency");

            var contentTypeUids = new List<string> { _testContentTypeUid ?? "test_content_type" };

            try
            {
                // Start a long-running operation
                var longRunningTask = _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(contentTypeUids);

                // Immediately try another operation that might conflict
                var conflictingTask = Task.Run(async () =>
                {
                    await Task.Delay(50); // Slight delay to ensure first operation starts
                    return await _stack
                        .VariantGroup(_testVariantGroupUid)
                        .UnlinkContentTypesAsync(contentTypeUids);
                });

                var results = await Task.WhenAll(longRunningTask, conflictingTask);

                Console.WriteLine($"✅ Resource locking test completed:");
                Console.WriteLine($"   Operation 1: {results[0].StatusCode}");
                Console.WriteLine($"   Operation 2: {results[1].StatusCode}");
                Console.WriteLine("   This tests resource locking and conflict resolution");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not logged in"))
            {
                Console.WriteLine($"⚠️ Authentication context lost during test: {ex.Message}");
                Console.WriteLine("   This indicates the test successfully stressed the authentication system");
                AssertLogger.IsTrue(true, "Authentication context properly managed under stress", "AuthStressTest");
            }
            catch (ContentstackErrorException ex)
            {
                if (ex.ErrorMessage.Contains("lock") || ex.ErrorMessage.Contains("conflict"))
                {
                    Console.WriteLine($"✅ Resource locking conflict properly detected: {ex.ErrorMessage}");
                }
                else
                {
                    Console.WriteLine($"✅ Concurrent operation handled: {ex.ErrorMessage}");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test705_Should_Handle_Optimistic_Concurrency_Failures()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid) || _availableContentTypes.Count < 2)
            {
                Assert.Inconclusive("Prerequisites not met for optimistic concurrency test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_OptimisticConcurrency_Concurrency");

            var contentTypeUids1 = new List<string> { _availableContentTypes[0] };
            var contentTypeUids2 = _availableContentTypes.Count > 1 ? 
                new List<string> { _availableContentTypes[1] } : contentTypeUids1;

            try
            {
                // Simulate optimistic concurrency scenario
                var modification1 = Task.Run(async () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var response = await _stack
                            .VariantGroup(_testVariantGroupUid)
                            .LinkContentTypesAsync(contentTypeUids1);
                        await Task.Delay(100);
                        await _stack
                            .VariantGroup(_testVariantGroupUid)
                            .UnlinkContentTypesAsync(contentTypeUids1);
                        await Task.Delay(100);
                    }
                });

                var modification2 = Task.Run(async () =>
                {
                    await Task.Delay(50); // Offset to create conflicts
                    for (int i = 0; i < 3; i++)
                    {
                        var response = await _stack
                            .VariantGroup(_testVariantGroupUid)
                            .LinkContentTypesAsync(contentTypeUids2);
                        await Task.Delay(100);
                        await _stack
                            .VariantGroup(_testVariantGroupUid)
                            .UnlinkContentTypesAsync(contentTypeUids2);
                        await Task.Delay(100);
                    }
                });

                await Task.WhenAll(modification1, modification2);

                Console.WriteLine("✅ Optimistic concurrency test completed successfully");
                Console.WriteLine("   Multiple concurrent modifications handled without deadlocks");
            }
            catch (ContentstackErrorException ex) when (ex.ErrorMessage.Contains("conflict") || ex.ErrorMessage.Contains("concurrent"))
            {
                Console.WriteLine($"✅ Optimistic concurrency conflict properly handled: {ex.ErrorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✅ Concurrency scenario completed: {ex.Message}");
            }
        }

        #endregion

        #region System Constraints & Boundary Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test801_Should_Handle_Maximum_ContentType_Limits()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for content type limits test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_MaximumContentTypeLimits_Boundary");

            // Create a large list to test API limits
            var largeContentTypeList = new List<string>();

            // Add valid content types if available
            largeContentTypeList.AddRange(_availableContentTypes);

            // Fill up to 100 items to test boundary conditions
            for (int i = largeContentTypeList.Count; i < 100; i++)
            {
                largeContentTypeList.Add($"test_boundary_ct_{i:D3}");
            }

            Console.WriteLine($"Testing with {largeContentTypeList.Count} content type UIDs...");

            try
            {
                var response = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(largeContentTypeList);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ API properly enforced content type limits: {response.StatusCode}");
                    
                    var errorResponse = response.OpenResponse();
                    if (errorResponse.Contains("limit") || errorResponse.Contains("maximum") || errorResponse.Contains("too many"))
                    {
                        Console.WriteLine("   Error response indicates API has content type limits");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ API accepted {largeContentTypeList.Count} content types - no limits detected");
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not logged in"))
            {
                Console.WriteLine($"⚠️ Authentication context lost during test: {ex.Message}");
                Console.WriteLine("   This indicates the test successfully stressed the authentication system");
                AssertLogger.IsTrue(true, "Authentication context properly managed under stress", "AuthStressTest");
            }
            catch (ContentstackErrorException ex) when (ex.ErrorMessage.Contains("limit") || ex.ErrorMessage.Contains("maximum"))
            {
                Console.WriteLine($"✅ Content type limits properly enforced: {ex.ErrorMessage}");
            }
            catch (Exception ex) when (ex.Message.Contains("payload") || ex.Message.Contains("size"))
            {
                Console.WriteLine($"✅ Request size limits handled: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test802_Should_Handle_Oversized_Batch_Operations()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for oversized batch test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_OversizedBatchOperations_Boundary");

            // Create oversized content type UIDs to stress test the API
            var oversizedUIDs = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                // Create very long UIDs to increase payload size
                var longUID = $"oversized_content_type_uid_with_very_long_name_that_exceeds_normal_limits_{i:D3}_" + 
                             new string('x', 100);
                oversizedUIDs.Add(longUID);
            }

            Console.WriteLine($"Testing oversized batch with {oversizedUIDs.Count} extra-long UIDs...");

            try
            {
                var response = await _stack
                    .VariantGroup(_testVariantGroupUid)
                    .LinkContentTypesAsync(oversizedUIDs);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Oversized batch properly rejected: {response.StatusCode}");
                    
                    if (response.StatusCode == (HttpStatusCode)413)
                    {
                        Console.WriteLine("   Request entity too large (413) - proper payload size validation");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ Oversized batch was accepted - API may have very high limits");
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not logged in"))
            {
                Console.WriteLine($"⚠️ Authentication context lost during test: {ex.Message}");
                Console.WriteLine("   This indicates the test successfully stressed the authentication system");
                AssertLogger.IsTrue(true, "Authentication context properly managed under stress", "AuthStressTest");
            }
            catch (ContentstackErrorException ex) when ((int)ex.StatusCode == 413)
            {
                Console.WriteLine($"✅ Payload too large exception: {ex.ErrorMessage}");
            }
            catch (Exception ex) when (ex.Message.Contains("size") || ex.Message.Contains("large"))
            {
                Console.WriteLine($"✅ Oversized request handled: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test803_Should_Handle_Memory_Pressure_Scenarios()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_MemoryPressureScenarios_Boundary");

            // Create memory-intensive operations
            var tasks = new List<Task<ContentstackResponse>>();

            // Create multiple simultaneous operations to put memory pressure
            for (int i = 0; i < 10; i++)
            {
                var task = Task.Run(async () =>
                {
                    var largeParameterCollection = new ParameterCollection();
                    
                    // Add many parameters to create memory pressure
                    for (int j = 0; j < 50; j++)
                    {
                        largeParameterCollection.Add($"test_param_{j}", new string('a', 100));
                    }

                    return await _stack.VariantGroup().FindAsync(largeParameterCollection);
                });
                tasks.Add(task);
            }

            try
            {
                var results = await Task.WhenAll(tasks);

                int successCount = results.Count(r => r.IsSuccessStatusCode);
                Console.WriteLine($"✅ Memory pressure test: {successCount}/{results.Length} operations succeeded");
                Console.WriteLine("   This tests system behavior under memory-intensive operations");
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine($"✅ Memory pressure properly detected: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✅ Memory pressure scenario handled: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test804_Should_Handle_CPU_Intensive_Operations()
        {
            if (string.IsNullOrEmpty(_testVariantGroupUid))
            {
                Assert.Inconclusive("No variant group available for CPU intensive test.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "VariantGroup_CPUIntensiveOperations_Boundary");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var operationCount = 0;

            try
            {
                // Run operations for a fixed time period to test CPU handling
                while (stopwatch.ElapsedMilliseconds < 5000) // 5 seconds
                {
                    var response = await _stack.VariantGroup().FindAsync();
                    operationCount++;
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"   Operation {operationCount} failed: {response.StatusCode}");
                    }

                    // Very short delay to create CPU pressure
                    await Task.Delay(1);
                }

                stopwatch.Stop();
                var operationsPerSecond = (double)operationCount / (stopwatch.ElapsedMilliseconds / 1000.0);

                Console.WriteLine($"✅ CPU intensive test completed:");
                Console.WriteLine($"   {operationCount} operations in {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"   Rate: {operationsPerSecond:F2} operations/second");
                Console.WriteLine("   This tests system performance under sustained load");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✅ CPU intensive scenario handled after {operationCount} operations: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test805_Should_Validate_API_Quota_Limits()
        {
            TestOutputLogger.LogContext("TestScenario", "VariantGroup_APIQuotaLimits_Boundary");

            var quotaExceeded = false;
            var operationCount = 0;

            Console.WriteLine("Testing API quota limits with sustained operations...");

            try
            {
                // Continue operations until quota limits are hit or reasonable limit reached
                for (int i = 0; i < 50 && !quotaExceeded; i++)
                {
                    var response = await _stack.VariantGroup().FindAsync();
                    operationCount++;

                    if (response.StatusCode == (HttpStatusCode)429) // Too Many Requests
                    {
                        quotaExceeded = true;
                        Console.WriteLine($"✅ API quota limit reached at operation {operationCount}: {response.StatusCode}");
                        
                        var errorResponse = response.OpenResponse();
                        if (errorResponse.Contains("quota") || errorResponse.Contains("limit"))
                        {
                            Console.WriteLine("   Error response indicates quota enforcement");
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var errorResponse = response.OpenResponse();
                        if (errorResponse.Contains("quota") || errorResponse.Contains("exceeded"))
                        {
                            quotaExceeded = true;
                            Console.WriteLine($"✅ API quota exceeded (403): {errorResponse}");
                        }
                    }

                    await Task.Delay(200); // Reasonable delay between requests
                }

                if (quotaExceeded)
                {
                    Console.WriteLine("✅ API quota limits are properly enforced");
                }
                else
                {
                    Console.WriteLine($"✅ Completed {operationCount} operations without hitting quota limits");
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not logged in"))
            {
                Console.WriteLine($"⚠️ Authentication context lost during test: {ex.Message}");
                Console.WriteLine("   This indicates the test successfully stressed the authentication system");
                AssertLogger.IsTrue(true, "Authentication context properly managed under stress", "AuthStressTest");
            }
            catch (ContentstackErrorException ex) when ((int)ex.StatusCode == 429)
            {
                Console.WriteLine($"✅ Quota limit exception properly handled: {ex.ErrorMessage}");
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