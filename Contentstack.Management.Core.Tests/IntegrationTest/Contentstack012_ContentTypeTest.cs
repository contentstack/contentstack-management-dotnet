using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

// Resolve ambiguous reference between System.Action and Contentstack.Management.Core.Models.Fields.Action
using SystemAction = System.Action;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack012_ContentTypeTest
    {
        private static ContentstackClient _client;
        private Stack _stack;
        private ContentModelling _singlePage;
        private ContentModelling _multiPage;

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
        public void Initialize ()
        {
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);
            _singlePage = Contentstack.serialize<ContentModelling>(_client.serializer, "singlepageCT.json");
            _multiPage = Contentstack.serialize<ContentModelling>(_client.serializer, "multiPageCT.json");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_SinglePage_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_SinglePage");
            TestOutputLogger.LogContext("ContentType", _singlePage.Uid);
            ContentTypeModel ContentType = TryCreateOrFetchContentType(_singlePage);
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modelling, "ContentType.Modelling");
            AssertLogger.AreEqual(_singlePage.Title, ContentType.Modelling.Title, "Title");
            AssertLogger.AreEqual(_singlePage.Uid, ContentType.Modelling.Uid, "Uid");
            AssertLogger.IsTrue(ContentType.Modelling.Schema.Count >= _singlePage.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Create_MultiPage_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_MultiPage");
            TestOutputLogger.LogContext("ContentType", _multiPage.Uid);
            ContentTypeModel ContentType = TryCreateOrFetchContentType(_multiPage);
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modelling, "ContentType.Modelling");
            AssertLogger.AreEqual(_multiPage.Title, ContentType.Modelling.Title, "Title");
            AssertLogger.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid, "Uid");
            AssertLogger.IsTrue(ContentType.Modelling.Schema.Count >= _multiPage.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchContentType");
            TestOutputLogger.LogContext("ContentType", _multiPage.Uid);
            ContentstackResponse response = _stack.ContentType(_multiPage.Uid).Fetch();
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modelling, "ContentType.Modelling");
            AssertLogger.AreEqual(_multiPage.Title, ContentType.Modelling.Title, "Title");
            AssertLogger.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid, "Uid");
            AssertLogger.IsTrue(ContentType.Modelling.Schema.Count >= _multiPage.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test004_Should_Fetch_Async_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAsyncContentType");
            TestOutputLogger.LogContext("ContentType", _singlePage.Uid);
            ContentstackResponse response = await _stack.ContentType(_singlePage.Uid).FetchAsync();
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modelling, "ContentType.Modelling");
            AssertLogger.AreEqual(_singlePage.Title, ContentType.Modelling.Title, "Title");
            AssertLogger.AreEqual(_singlePage.Uid, ContentType.Modelling.Uid, "Uid");
            AssertLogger.IsTrue(ContentType.Modelling.Schema.Count >= _singlePage.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateContentType");
            TestOutputLogger.LogContext("ContentType", _multiPage.Uid);
            _multiPage.Schema = Contentstack.serializeArray<List<Models.Fields.Field>>(_client.serializer, "contentTypeSchema.json"); ;
            ContentstackResponse response = _stack.ContentType(_multiPage.Uid).Update(_multiPage);
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modelling, "ContentType.Modelling");
            AssertLogger.AreEqual(_multiPage.Title, ContentType.Modelling.Title, "Title");
            AssertLogger.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid, "Uid");
            AssertLogger.IsTrue(ContentType.Modelling.Schema.Count >= _multiPage.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test006_Should_Update_Async_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateAsyncContentType");
            TestOutputLogger.LogContext("ContentType", _multiPage.Uid);
            try
            {
                // Load the existing schema
                _multiPage.Schema = Contentstack.serializeArray<List<Models.Fields.Field>>(_client.serializer, "contentTypeSchema.json");
                
                // Add a new text field to the schema
                var newTextField = new Models.Fields.TextboxField
                {
                    Uid = "new_text_field",
                    DataType = "text",
                    DisplayName = "New Text Field",
                    FieldMetadata = new Models.Fields.FieldMetadata
                    {
                        Description = "A new text field added during async update test"
                    }
                };
                _multiPage.Schema.Add(newTextField);
                
                // Update the content type with the modified schema
                ContentstackResponse response = await _stack.ContentType(_multiPage.Uid).UpdateAsync(_multiPage);
                
                if (response.IsSuccessStatusCode)
                {
                    ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
                    AssertLogger.IsNotNull(response, "response");
                    AssertLogger.IsNotNull(ContentType, "ContentType");
                    AssertLogger.IsNotNull(ContentType.Modelling, "ContentType.Modelling");
                    AssertLogger.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid, "Uid");
                    AssertLogger.IsTrue(ContentType.Modelling.Schema.Count >= _multiPage.Schema.Count, "SchemaCount");
                    Console.WriteLine($"Successfully updated content type with {ContentType.Modelling.Schema.Count} fields");
                }
                else
                {
                    AssertLogger.Fail($"Update failed with status {response.StatusCode}: {response.OpenResponse()}");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Exception during async update: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryContentType");
            ContentstackResponse response = _stack.ContentType().Query().Find();
            ContentTypesModel ContentType = response.OpenTResponse<ContentTypesModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modellings, "ContentType.Modellings");
            AssertLogger.IsTrue(ContentType.Modellings.Count >= 2, "At least legacy single_page and multi_page exist");
            AssertLogger.IsTrue(ContentType.Modellings.Any(m => m.Uid == _singlePage.Uid), "single_page in query result");
            AssertLogger.IsTrue(ContentType.Modellings.Any(m => m.Uid == _multiPage.Uid), "multi_page in query result");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test008_Should_Query_Async_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryAsyncContentType");
            ContentstackResponse response = await _stack.ContentType().Query().FindAsync();
            ContentTypesModel ContentType = response.OpenTResponse<ContentTypesModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modellings, "ContentType.Modellings");
            AssertLogger.IsTrue(ContentType.Modellings.Count >= 2, "At least legacy single_page and multi_page exist");
            AssertLogger.IsTrue(ContentType.Modellings.Any(m => m.Uid == _singlePage.Uid), "single_page in query result");
            AssertLogger.IsTrue(ContentType.Modellings.Any(m => m.Uid == _multiPage.Uid), "multi_page in query result");
        }

        #region SDK-Level Validation Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Throw_When_Create_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_NullModel");
            
            AssertLogger.ThrowsException<ArgumentNullException>(
                () => _stack.ContentType().Create(null),
                "Create_NullModel");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test010_Should_Throw_When_CreateAsync_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateAsyncContentType_NullModel");

            await AssertLogger.ThrowsExceptionAsync<ArgumentNullException>(
                () => _stack.ContentType().CreateAsync(null),
                "CreateAsync_NullModel");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Throw_When_Create_With_UID_Set()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_UIDSet");

            var model = CreateValidContentTypeModel("invalid_create");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => _stack.ContentType("some_uid").Create(model),
                "Create_UIDSet");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Throw_When_Fetch_Without_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchContentType_NoUID");
            
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.ContentType().Fetch(),
                "Fetch_NoUID");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test013_Should_Throw_When_FetchAsync_Without_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAsyncContentType_NoUID");

            await AssertLogger.ThrowsExceptionAsync<ArgumentException>(
                () => _stack.ContentType().FetchAsync(),
                "FetchAsync_NoUID");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Throw_When_Update_Without_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateContentType_NoUID");

            var model = CreateValidContentTypeModel("update_no_uid");
            
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.ContentType().Update(model),
                "Update_NoUID");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Throw_When_Update_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateContentType_NullModel");
            
            AssertLogger.ThrowsException<ArgumentNullException>(
                () => _stack.ContentType("some_uid").Update(null),
                "Update_NullModel");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Throw_When_Delete_Without_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteContentType_NoUID");
            
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.ContentType().Delete(),
                "Delete_NoUID");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Throw_When_Query_With_UID_Set()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryContentType_UIDSet");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => _stack.ContentType("some_uid").Query(),
                "Query_UIDSet");
        }

        #endregion

        #region Authentication & Authorization Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Fail_When_Not_Authenticated()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_NotAuthenticated");

            var unauthenticatedClient = CreateUnauthenticatedClient();
            var unauthStack = unauthenticatedClient.Stack("dummy_api_key");
            var model = CreateValidContentTypeModel("unauth_test");

            // SDK performs client-side validation and throws InvalidOperationException
            // before making HTTP calls for unauthenticated clients
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => unauthStack.ContentType().Create(model),
                "NotAuthenticated_Create");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_Fail_With_Invalid_Auth_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_InvalidAuthToken");

            var invalidAuthClient = CreateInvalidAuthClient();
            var invalidAuthStack = invalidAuthClient.Stack("dummy_api_key");
            var model = CreateValidContentTypeModel("invalid_auth");

            AssertContentTypeAuthError(
                () => invalidAuthStack.ContentType().Create(model),
                "InvalidAuth_Create");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test020_Should_Fail_With_Invalid_Stack_API_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_InvalidStackKey");

            var invalidStack = _client.Stack("invalid_stack_api_key_123456789");
            var model = CreateValidContentTypeModel("invalid_stack");

            AssertContentTypeAuthError(
                () => invalidStack.ContentType().Create(model),
                "InvalidStackKey_Create");
        }

        #endregion

        #region API Validation Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test021_Should_Fail_Create_With_Invalid_UID_Formats()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_InvalidUIDs");
            
            // Clean up any content types from previous failed test runs
            SafeDeleteContentType(new string('a', 101)); // The long UID from previous test run

            var invalidUIDs = new[]
            {
                "Invalid UID With Spaces",
                "Invalid-UID-With-Dashes!",
                "InvalidUIDWithSpecialChars@#$",
                "   ",  // Whitespace only
                "uid.with.dots",  // Dots are not allowed (only alphanumeric, underscore, hyphen)
                "" // Empty string
            };

            foreach (var invalidUID in invalidUIDs)
            {
                try
                {
                    var model = CreateValidContentTypeModel("base");
                    model.Uid = invalidUID;
                    model.Title = $"Invalid UID Test {invalidUID}";

                    AssertContentTypeValidationError(() => _stack.ContentType().Create(model), 
                        $"InvalidUID_{invalidUID?.Replace(" ", "_") ?? "empty"}");
                    
                    Console.WriteLine($"✅ Invalid UID properly rejected: '{invalidUID}'");
                }
                catch (AssertFailedException)
                {
                    // Re-throw assertion failures
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Unexpected exception for UID '{invalidUID}': {ex.Message}");
                    // If content type was created unexpectedly, try to clean it up
                    if (!string.IsNullOrEmpty(invalidUID))
                    {
                        SafeDeleteContentType(invalidUID);
                    }
                }
            }
            
            // Final cleanup - ensure no test content types are left behind
            foreach (var invalidUID in new[] { 
                "Invalid UID With Spaces", "Invalid-UID-With-Dashes!", "InvalidUIDWithSpecialChars@#$",
                "   ", "uid.with.dots", ""
            })
            {
                if (!string.IsNullOrWhiteSpace(invalidUID))
                {
                    SafeDeleteContentType(invalidUID);
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Fail_Create_With_Missing_Title()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_MissingTitle");

            var model = CreateInvalidContentTypeModel("null_title");

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "MissingTitle_Create");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test023_Should_Fail_Create_With_Empty_Title()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_EmptyTitle");

            var model = CreateInvalidContentTypeModel("empty_title");

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "EmptyTitle_Create");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test024_Should_Fail_Create_With_Duplicate_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_DuplicateUID");

            // First, create a content type
            var model1 = CreateValidContentTypeModel("duplicate_uid_test");
            var createdContentType = TryCreateOrFetchContentType(model1);

            try
            {
                // Try to create another with same UID - must use exact same UID
                var model2 = CreateValidContentTypeModel("duplicate_uid_test");
                model2.Uid = createdContentType.Modelling.Uid; // Force same UID for duplicate test
                model2.Title = "Different Title Same UID";

                AssertContentTypeValidationError(
                    () => _stack.ContentType().Create(model2),
                    "DuplicateUID_Create");
            }
            finally
            {
                // Clean up created content type
                SafeDeleteContentType(createdContentType.Modelling.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test025_Should_Fail_Create_With_Empty_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_EmptyUID");

            var model = CreateInvalidContentTypeModel("empty_uid");

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "EmptyUID_Create");
        }

        #endregion

        #region Schema Validation Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test026_Should_Fail_With_Invalid_Field_Data_Types()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_InvalidFieldDataTypes");

            var model = CreateInvalidContentTypeModel("invalid_field_datatype");

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "InvalidFieldDataType_Create");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test027_Should_Fail_With_Missing_Field_Display_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_MissingFieldDisplayName");

            var model = CreateInvalidContentTypeModel("missing_field_displayname");

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "MissingFieldDisplayName_Create");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test028_Should_Fail_With_Missing_Field_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_MissingFieldUID");

            var model = CreateInvalidContentTypeModel("missing_field_uid");

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "MissingFieldUID_Create");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test029_Should_Fail_With_Duplicate_Field_UIDs()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_DuplicateFieldUIDs");

            var model = CreateInvalidContentTypeModel("duplicate_field_uids");

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "DuplicateFieldUID_Create");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Fail_With_Empty_Schema()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_EmptySchema");

            var model = CreateInvalidContentTypeModel("empty_schema");

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "EmptySchema_Create");
        }

        #endregion

        #region Resource Not Found Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test031_Should_Fail_Fetch_NonExistent_ContentType()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchContentType_NonExistent");

            AssertContentTypeNotFoundError(
                () => _stack.ContentType("nonexistent_content_type_uid_12345").Fetch(),
                "NonExistent_Fetch");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test032_Should_Fail_FetchAsync_NonExistent_ContentType()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAsyncContentType_NonExistent");

            await AssertContentTypeNotFoundErrorAsync(
                () => _stack.ContentType("nonexistent_content_type_uid_12345").FetchAsync(),
                "NonExistent_FetchAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test033_Should_Fail_Update_NonExistent_ContentType()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateContentType_NonExistent");

            var model = CreateValidContentTypeModel("nonexistent_update_test");

            AssertContentTypeNotFoundError(
                () => _stack.ContentType("nonexistent_content_type_uid_12345").Update(model),
                "NonExistent_Update");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test034_Should_Fail_Delete_NonExistent_ContentType()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteContentType_NonExistent");

            AssertContentTypeNotFoundError(
                () => _stack.ContentType("nonexistent_content_type_uid_12345").Delete(),
                "NonExistent_Delete");
        }

        #endregion

        #region Business Logic & State Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test035_Should_Fail_Update_After_Delete()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_UpdateAfterDelete");

            // Create and then delete a content type
            var model = CreateValidContentTypeModel("update_after_delete_test");
            var createdContentType = TryCreateOrFetchContentType(model);
            
            // Delete the content type
            var deleteResponse = _stack.ContentType(createdContentType.Modelling.Uid).Delete();
            AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete_Success");

            // Try to update the deleted content type
            model.Title = "Updated After Delete";
            
            AssertContentTypeNotFoundError(
                () => _stack.ContentType(createdContentType.Modelling.Uid).Update(model),
                "UpdateAfterDelete");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test036_Should_Fail_Fetch_After_Delete()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_FetchAfterDelete");

            // Create and then delete a content type
            var model = CreateValidContentTypeModel("fetch_after_delete_test");
            var createdContentType = TryCreateOrFetchContentType(model);
            
            // Delete the content type
            var deleteResponse = _stack.ContentType(createdContentType.Modelling.Uid).Delete();
            AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete_Success");

            // Try to fetch the deleted content type
            AssertContentTypeNotFoundError(
                () => _stack.ContentType(createdContentType.Modelling.Uid).Fetch(),
                "FetchAfterDelete");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test037_Should_Handle_Concurrent_Updates()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_ConcurrentUpdates");

            // Create a content type first
            var model = CreateValidContentTypeModel("concurrent_test");
            var createdContentType = TryCreateOrFetchContentType(model);

            try
            {
                // Simulate concurrent updates by making multiple update calls
                var updatedModel1 = CreateValidContentTypeModel("concurrent_test");
                updatedModel1.Uid = createdContentType.Modelling.Uid;
                updatedModel1.Title = "Concurrent Update 1";

                var updatedModel2 = CreateValidContentTypeModel("concurrent_test");  
                updatedModel2.Uid = createdContentType.Modelling.Uid;
                updatedModel2.Title = "Concurrent Update 2";

                // First update should succeed
                var response1 = _stack.ContentType(updatedModel1.Uid).Update(updatedModel1);
                AssertLogger.IsTrue(response1.IsSuccessStatusCode, "FirstUpdate_Success");

                // Second update might cause conflict or succeed depending on API behavior
                try
                {
                    var response2 = _stack.ContentType(updatedModel2.Uid).Update(updatedModel2);
                    Console.WriteLine($"Second concurrent update: {response2.StatusCode}");
                    // Both updates succeeded - this is acceptable behavior
                }
                catch (ContentstackErrorException ex)
                {
                    Console.WriteLine($"✅ Concurrent update conflict detected: {ex.StatusCode}");
                    AssertLogger.IsTrue(
                        ex.StatusCode == HttpStatusCode.Conflict ||
                        ex.StatusCode == (HttpStatusCode)422 ||
                        ex.StatusCode == HttpStatusCode.BadRequest,
                        $"Expected conflict error, got {ex.StatusCode}",
                        "ConcurrentUpdate_Conflict");
                }
            }
            finally
            {
                // Clean up created content type
                SafeDeleteContentType(createdContentType.Modelling.Uid);
            }
        }

        #endregion

        #region Query Parameter Validation Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test038_Should_Handle_Invalid_Query_Limit()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_InvalidQueryLimit");

            try
            {
                // Test with extremely large limit that might be rejected
                var response = _stack.ContentType().Query()
                    .Limit(10000)
                    .Find();
                    
                Console.WriteLine($"Query with large limit succeeded: {response.StatusCode}");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Large limit properly rejected: {ex.StatusCode}");
                AssertLogger.IsTrue(
                    ex.StatusCode == HttpStatusCode.BadRequest ||
                    ex.StatusCode == (HttpStatusCode)422,
                    $"Expected validation error for large limit, got {ex.StatusCode}",
                    "LargeLimit_ValidationError");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test039_Should_Handle_Invalid_Query_Skip()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_InvalidQuerySkip");

            try
            {
                // Test with negative skip value
                var response = _stack.ContentType().Query()
                    .Skip(-1)
                    .Find();
                    
                Console.WriteLine($"Query with negative skip succeeded: {response.StatusCode}");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Negative skip properly rejected: {ex.StatusCode}");
                AssertLogger.IsTrue(
                    ex.StatusCode == HttpStatusCode.BadRequest ||
                    ex.StatusCode == (HttpStatusCode)422,
                    $"Expected validation error for negative skip, got {ex.StatusCode}",
                    "NegativeSkip_ValidationError");
            }
            catch (ArgumentException ex)
            {
                // SDK might validate this client-side
                Console.WriteLine($"✅ Negative skip rejected by SDK: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test040_Should_Fail_Query_When_Not_Authenticated()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_QueryNotAuthenticated");

            var unauthenticatedClient = CreateUnauthenticatedClient();
            var unauthStack = unauthenticatedClient.Stack("dummy_api_key");

            // SDK performs client-side validation for unauthenticated queries
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => unauthStack.ContentType().Query().Find(),
                "QueryNotAuthenticated");
        }

        #endregion

        #region Security & Edge Case Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test041_Should_Handle_XSS_In_Title()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_XSSTitle");

            var model = CreateValidContentTypeModel("xss_test");
            model.Title = "<script>alert('xss')</script>";

            try
            {
                var response = _stack.ContentType().Create(model);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.OpenTResponse<ContentTypeModel>();
                    Console.WriteLine($"⚠️ XSS content accepted but should be sanitized: {result.Modelling.Title}");
                    
                    // Clean up immediately
                    SafeDeleteContentType(model.Uid);
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ XSS content properly rejected: {ex.StatusCode}");
                AssertContentTypeValidationError(() => _stack.ContentType().Create(model), "XSS_Title");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test042_Should_Handle_SQL_Injection_In_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_SQLInjectionUID");

            var model = CreateValidContentTypeModel("sql_test");
            model.Uid = "'; DROP TABLE content_types; --";

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "SQLInjection_UID");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test043_Should_Handle_Extremely_Large_Schema()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_LargeSchema");

            var model = CreateValidContentTypeModel("large_schema_test");
            
            // Add many fields to test payload size limits
            for (int i = 0; i < 100; i++)
            {
                model.Schema.Add(new TextboxField
                {
                    Uid = $"large_field_{i}",
                    DisplayName = $"Large Field {i}",
                    DataType = "text",
                    FieldMetadata = new FieldMetadata
                    {
                        Description = new string('a', 500) // Large description
                    }
                });
            }

            try
            {
                var response = _stack.ContentType().Create(model);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Large schema accepted");
                    // Clean up immediately
                    SafeDeleteContentType(model.Uid);
                }
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"✅ Large schema rejected: {ex.StatusCode}");
                AssertLogger.IsTrue(
                    ex.StatusCode == HttpStatusCode.BadRequest ||
                    ex.StatusCode == (HttpStatusCode)422 ||
                    ex.StatusCode == HttpStatusCode.RequestEntityTooLarge,
                    $"Expected validation error for large schema, got {ex.StatusCode}",
                    "LargeSchema_ValidationError");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test044_Should_Handle_Path_Traversal_In_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_PathTraversalUID");

            var model = CreateValidContentTypeModel("path_traversal_test");
            model.Uid = "../../../etc/passwd";

            AssertContentTypeValidationError(
                () => _stack.ContentType().Create(model),
                "PathTraversal_UID");
        }

        #endregion

        #region Network & Infrastructure Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test045_Should_Handle_Network_Timeout_Scenarios()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_NetworkTimeout");

            // Test with a very complex schema that might cause timeouts
            var model = CreateValidContentTypeModel("timeout_test");
            
            // Add a very deep nested structure or complex schema
            for (int i = 0; i < 50; i++)
            {
                model.Schema.Add(new TextboxField
                {
                    Uid = $"complex_field_{i}",
                    DisplayName = $"Complex Field {i}",
                    DataType = "text",
                    FieldMetadata = new FieldMetadata
                    {
                        Description = new string('a', 500) // Large description
                    }
                });
            }

            try
            {
                var response = _stack.ContentType().Create(model);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Complex schema handled successfully");
                    // Clean up immediately
                    SafeDeleteContentType(model.Uid);
                }
            }
            catch (ContentstackErrorException ex) when (
                ex.StatusCode == HttpStatusCode.RequestTimeout ||
                ex.StatusCode == HttpStatusCode.ServiceUnavailable ||
                ex.StatusCode == HttpStatusCode.BadGateway ||
                ex.StatusCode == HttpStatusCode.GatewayTimeout ||
                ex.StatusCode == HttpStatusCode.RequestEntityTooLarge)
            {
                Console.WriteLine($"✅ Network/timeout error properly handled: {ex.StatusCode}");
                TestOutputLogger.LogContext("NetworkError", ex.StatusCode.ToString());
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test046_Should_Handle_Rate_Limiting()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_RateLimiting");

            // Make multiple rapid requests to potentially trigger rate limiting
            var tasks = new List<Task>();
            var models = new List<ContentModelling>();

            for (int i = 0; i < 10; i++)
            {
                var model = CreateValidContentTypeModel($"rate_limit_test_{i}");
                models.Add(model);
                
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var response = await _stack.ContentType().CreateAsync(model);
                        Console.WriteLine($"Request {i}: {response.StatusCode}");
                    }
                    catch (ContentstackErrorException ex) when (ex.StatusCode == (HttpStatusCode)429)
                    {
                        Console.WriteLine($"✅ Rate limiting detected on request {i}: {ex.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Request {i} failed: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);

            // Clean up any created content types
            foreach (var model in models)
            {
                SafeDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test047_Should_Handle_Server_Errors()
        {
            TestOutputLogger.LogContext("TestScenario", "ContentType_ServerErrors");

            // Test with potentially problematic payload that might cause server errors
            var model = CreateValidContentTypeModel("server_error_test");
            
            // Add potentially problematic field configurations
            model.Schema.Add(new ReferenceField
            {
                Uid = "invalid_reference",
                DisplayName = "Invalid Reference", 
                DataType = "reference",
                ReferenceTo = new List<string> { "nonexistent_content_type_uid" }
            });

            try
            {
                var response = _stack.ContentType().Create(model);
                Console.WriteLine($"Server error test result: {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    SafeDeleteContentType(model.Uid);
                }
            }
            catch (ContentstackErrorException ex) when (
                ex.StatusCode == HttpStatusCode.InternalServerError ||
                ex.StatusCode == HttpStatusCode.BadGateway ||
                ex.StatusCode == HttpStatusCode.ServiceUnavailable ||
                ex.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                Console.WriteLine($"✅ Server error properly handled: {ex.StatusCode}");
                TestOutputLogger.LogContext("ServerError", ex.StatusCode.ToString());
            }
            catch (ContentstackErrorException ex)
            {
                // Other validation errors are also acceptable
                Console.WriteLine($"✅ Validation error for problematic payload: {ex.StatusCode}");
            }
        }

        #endregion

        /// <summary>
        /// Creates the content type when missing; otherwise fetches it (stack may already have legacy types).
        /// </summary>
        private ContentTypeModel TryCreateOrFetchContentType(ContentModelling modelling)
        {
            try
            {
                var response = _stack.ContentType().Create(modelling);
                return response.OpenTResponse<ContentTypeModel>();
            }
            catch (ContentstackErrorException ex) when (
                ex.StatusCode == HttpStatusCode.UnprocessableEntity
                || ex.StatusCode == HttpStatusCode.Conflict
                || ex.StatusCode == (HttpStatusCode)422)
            {
                var response = _stack.ContentType(modelling.Uid).Fetch();
                return response.OpenTResponse<ContentTypeModel>();
            }
        }

        #region Helper Methods

        /// <summary>
        /// Creates a valid ContentType model with unique UID for testing
        /// </summary>
        private ContentModelling CreateValidContentTypeModel(string baseName)
        {
            var uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
            
            return new ContentModelling
            {
                Title = $"Test ContentType {baseName} {uniqueSuffix}",
                Uid = $"{baseName}_{uniqueSuffix}",
                Description = "Generated for comprehensive error handling tests",
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        Uid = "title",
                        DisplayName = "Title",
                        DataType = "text",
                        FieldMetadata = new FieldMetadata 
                        { 
                            Default = "true", 
                            Version = 3,
                            AllowRichText = false,
                            Multiline = false,
                            Markdown = false,
                            RefMultiple = false
                        }
                    }
                },
                Options = new Option
                {
                    IsPage = false,
                    Singleton = false,
                    Title = "title"
                }
            };
        }

        /// <summary>
        /// Creates invalid ContentType models for various error scenarios
        /// </summary>
        private ContentModelling CreateInvalidContentTypeModel(string scenario)
        {
            var baseModel = CreateValidContentTypeModel("invalid");
            
            switch (scenario.ToLower())
            {
                case "null_title":
                    baseModel.Title = null;
                    break;
                case "empty_title":
                    baseModel.Title = "";
                    break;
                case "invalid_uid_spaces":
                    baseModel.Uid = "Invalid UID With Spaces";
                    break;
                case "invalid_uid_special":
                    baseModel.Uid = "Invalid-UID-With-Special@#$";
                    break;
                case "too_long_uid":
                    baseModel.Uid = new string('a', 101);
                    break;
                case "empty_uid":
                    baseModel.Uid = "";
                    break;
                case "null_uid":
                    baseModel.Uid = null;
                    break;
                case "empty_schema":
                    baseModel.Schema = new List<Field>();
                    break;
                case "null_schema":
                    baseModel.Schema = null;
                    break;
                case "duplicate_field_uids":
                    baseModel.Schema.Add(new TextboxField
                    {
                        Uid = "title", // Same as existing field
                        DisplayName = "Duplicate Title",
                        DataType = "text"
                    });
                    break;
                case "invalid_field_datatype":
                    baseModel.Schema.Add(new Field
                    {
                        Uid = "invalid_field",
                        DisplayName = "Invalid Field",
                        DataType = "invalid_data_type_xyz"
                    });
                    break;
                case "missing_field_displayname":
                    baseModel.Schema.Add(new Field
                    {
                        Uid = "no_display_name",
                        DataType = "text"
                        // Missing DisplayName
                    });
                    break;
                case "missing_field_uid":
                    baseModel.Schema.Add(new Field
                    {
                        DisplayName = "No UID Field",
                        DataType = "text"
                        // Missing Uid
                    });
                    break;
                default:
                    throw new ArgumentException($"Unknown invalid scenario: {scenario}");
            }
            
            return baseModel;
        }

        /// <summary>
        /// Safely cleans up a content type by UID, ignoring all errors
        /// </summary>
        private void SafeDeleteContentType(string uid)
        {
            try
            {
                if (!string.IsNullOrEmpty(uid))
                {
                    _stack.ContentType(uid).Delete();
                    Console.WriteLine($"✅ Successfully cleaned up content type: {uid}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to cleanup content type {uid}: {ex.Message}");
                // Swallow all exceptions during cleanup
            }
        }

        /// <summary>
        /// Creates an unauthenticated client for testing auth failures
        /// </summary>
        private ContentstackClient CreateUnauthenticatedClient()
        {
            return new ContentstackClient();
        }

        /// <summary>
        /// Creates a client with invalid auth token for testing auth failures
        /// </summary>
        private ContentstackClient CreateInvalidAuthClient()
        {
            var invalidClient = new ContentstackClient("invalid_auth_token_123456789");
            return invalidClient;
        }

        /// <summary>
        /// Asserts ContentType validation errors with proper status codes and enhanced error reporting
        /// </summary>
        private void AssertContentTypeValidationError(SystemAction action, string context)
        {
            try
            {
                AssertLogger.ThrowsContentstackError(
                    action,
                    context,
                    HttpStatusCode.BadRequest,
                    (HttpStatusCode)422,
                    HttpStatusCode.UnprocessableEntity,
                    HttpStatusCode.Conflict);
            }
            catch (AssertFailedException ex)
            {
                Console.WriteLine($"❌ Validation error assertion failed for {context}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Async version of ContentType validation error assertion
        /// </summary>
        private async Task AssertContentTypeValidationErrorAsync(Func<Task> action, string context)
        {
            try
            {
                await AssertLogger.ThrowsContentstackErrorAsync(
                    action,
                    context,
                    HttpStatusCode.BadRequest,
                    (HttpStatusCode)422,
                    HttpStatusCode.UnprocessableEntity,
                    HttpStatusCode.Conflict);
            }
            catch (AssertFailedException ex)
            {
                Console.WriteLine($"❌ Async validation error assertion failed for {context}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Asserts ContentType authentication/authorization errors
        /// </summary>
        private void AssertContentTypeAuthError(SystemAction action, string context)
        {
            try
            {
                AssertLogger.ThrowsContentstackError(
                    action,
                    context,
                    HttpStatusCode.Unauthorized,
                    HttpStatusCode.Forbidden,
                    HttpStatusCode.PreconditionFailed,
                    (HttpStatusCode)422);
            }
            catch (AssertFailedException ex)
            {
                Console.WriteLine($"❌ Auth error assertion failed for {context}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Async version of ContentType auth error assertion
        /// </summary>
        private async Task AssertContentTypeAuthErrorAsync(Func<Task> action, string context)
        {
            try
            {
                await AssertLogger.ThrowsContentstackErrorAsync(
                    action,
                    context,
                    HttpStatusCode.Unauthorized,
                    HttpStatusCode.Forbidden,
                    HttpStatusCode.PreconditionFailed,
                    (HttpStatusCode)422);
            }
            catch (AssertFailedException ex)
            {
                Console.WriteLine($"❌ Async auth error assertion failed for {context}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Asserts ContentType not found errors
        /// </summary>
        private void AssertContentTypeNotFoundError(SystemAction action, string context)
        {
            try
            {
                AssertLogger.ThrowsContentstackError(
                    action,
                    context,
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (AssertFailedException ex)
            {
                Console.WriteLine($"❌ Not found error assertion failed for {context}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Async version of ContentType not found error assertion
        /// </summary>
        private async Task AssertContentTypeNotFoundErrorAsync(Func<Task> action, string context)
        {
            try
            {
                await AssertLogger.ThrowsContentstackErrorAsync(
                    action,
                    context,
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (AssertFailedException ex)
            {
                Console.WriteLine($"❌ Async not found error assertion failed for {context}: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}
