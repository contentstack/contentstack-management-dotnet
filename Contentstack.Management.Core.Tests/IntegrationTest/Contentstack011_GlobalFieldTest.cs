using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack011_GlobalFieldTest
    {
        private static ContentstackClient _client;
        private Stack _stack;
        private ContentModelling _modelling;

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
            _modelling = Contentstack.serialize<ContentModelling>(_client.serializer, "globalfield.json");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField");
            ContentstackResponse response = _stack.GlobalField().Create(_modelling);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Fetch_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchGlobalField");
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            ContentstackResponse response = _stack.GlobalField(_modelling.Uid).Fetch();
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Fetch_Async_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAsyncGlobalField");
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            ContentstackResponse response = await _stack.GlobalField(_modelling.Uid).FetchAsync();
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateGlobalField");
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            _modelling.Title = "Updated title";
            ContentstackResponse response = _stack.GlobalField(_modelling.Uid).Update(_modelling);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Update_Async_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateAsyncGlobalField");
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            _modelling.Title = "First Async";
            ContentstackResponse response = await _stack.GlobalField(_modelling.Uid).UpdateAsync(_modelling);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Query_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryGlobalField");
            ContentstackResponse response = _stack.GlobalField().Query().Find();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modellings, "globalField.Modellings");
            AssertLogger.AreEqual(1, globalField.Modellings.Count, "ModellingsCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006a_Should_Query_Global_Field_With_ApiVersion()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryGlobalFieldWithApiVersion");
            ContentstackResponse response = _stack.GlobalField(apiVersion: "3.2").Query().Find();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modellings, "globalField.Modellings");
            AssertLogger.AreEqual(1, globalField.Modellings.Count, "ModellingsCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Query_Async_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryAsyncGlobalField");
            ContentstackResponse response = await _stack.GlobalField().Query().FindAsync();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modellings, "globalField.Modellings");
            AssertLogger.AreEqual(1, globalField.Modellings.Count, "ModellingsCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007a_Should_Query_Async_Global_Field_With_ApiVersion()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryAsyncGlobalFieldWithApiVersion");
            ContentstackResponse response = await _stack.GlobalField(apiVersion: "3.2").Query().FindAsync();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modellings, "globalField.Modellings");
            AssertLogger.AreEqual(1, globalField.Modellings.Count, "ModellingsCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Delete_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteGlobalField");
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            ContentstackResponse response = _stack.GlobalField(_modelling.Uid).Delete();
            AssertLogger.IsNotNull(response, "response");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test009_Should_Delete_Async_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteAsyncGlobalField");
            
            // Create a new global field for async delete test
            ContentModelling deleteModel = Contentstack.serialize<ContentModelling>(_client.serializer, "globalfield.json");
            deleteModel.Uid = "test_delete_async";
            deleteModel.Title = "Test Delete Async";
            ContentstackResponse createResponse = _stack.GlobalField().Create(deleteModel);
            GlobalFieldModel createdGlobalField = createResponse.OpenTResponse<GlobalFieldModel>();
            AssertLogger.IsNotNull(createdGlobalField, "createdGlobalField");

            TestOutputLogger.LogContext("GlobalField", deleteModel.Uid);
            ContentstackResponse response = await _stack.GlobalField(deleteModel.Uid).DeleteAsync();
            AssertLogger.IsNotNull(response, "response");
        }

        #region Constants
        private const string InvalidGlobalFieldUid = "non_existent_global_field_uid_12345";
        private const string InvalidApiKey = "bltInvalidApiKey12345";
        private static readonly string VeryLongTitle = new string('a', 300); // 300 characters
        private const string SqlInjectionTitle = "'; DROP TABLE global_fields; --";
        private const string XssTitle = "<script>alert('xss')</script>";
        #endregion

        #region Helper Methods
        private static void AssertGlobalFieldValidationError(Exception ex, string assertContext)
        {
            if (ex is ContentstackErrorException csException)
            {
                AssertLogger.IsTrue(
                    csException.StatusCode == HttpStatusCode.BadRequest ||
                    csException.StatusCode == HttpStatusCode.UnprocessableEntity ||
                    csException.StatusCode == HttpStatusCode.Conflict,
                    $"{assertContext}: Expected validation error (400/422/409), but got {csException.StatusCode}",
                    "statusCode");
            }
            else if (ex is ArgumentNullException || ex is ArgumentException)
            {
                // SDK-level validation - acceptable
            }
            else
            {
                AssertLogger.Fail($"{assertContext}: Expected validation error but got {ex.GetType().Name}: {ex.Message}");
            }
        }

        private static void AssertGlobalFieldAuthError(Exception ex, string assertContext)
        {
            if (ex is ContentstackErrorException csException)
            {
                AssertLogger.IsTrue(
                    csException.StatusCode == HttpStatusCode.Unauthorized ||
                    csException.StatusCode == HttpStatusCode.Forbidden,
                    $"{assertContext}: Expected auth error (401/403), but got {csException.StatusCode}",
                    "statusCode");
            }
            else if (ex is InvalidOperationException)
            {
                // SDK-level "not logged in" - acceptable
            }
            else
            {
                AssertLogger.Fail($"{assertContext}: Expected auth error but got {ex.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Contentstack Management API returns 422 (UnprocessableEntity) for invalid Global Field UIDs,
        /// not 404 (NotFound) like some other resources. Both are acceptable "not found" responses.
        /// </summary>
        private static void AssertGlobalFieldNotFoundError(Exception ex, string assertContext)
        {
            if (ex is ContentstackErrorException csException)
            {
                AssertLogger.IsTrue(
                    csException.StatusCode == HttpStatusCode.NotFound ||
                    csException.StatusCode == HttpStatusCode.UnprocessableEntity,
                    $"{assertContext}: Expected 404 or 422 error, but got {csException.StatusCode}",
                    "statusCode");
                    
                // Log additional context for debugging - ErrorCode 118 indicates "Global Field was not found"
                if (csException.ErrorCode == 118)
                {
                    TestOutputLogger.LogContext("ErrorCodeContext", "Expected ErrorCode 118 for Global Field not found");
                }
            }
            else
            {
                AssertLogger.Fail($"{assertContext}: Expected ContentstackErrorException with 404 or 422 but got {ex.GetType().Name}: {ex.Message}");
            }
        }

        private ContentModelling CreateInvalidGlobalFieldModel(string scenario)
        {
            var model = Contentstack.serialize<ContentModelling>(_client.serializer, "globalfield.json");
            
            switch (scenario)
            {
                case "null_title":
                    model.Title = null;
                    break;
                case "empty_title":
                    model.Title = "";
                    break;
                case "long_title":
                    model.Title = VeryLongTitle;
                    break;
                // sql_injection case removed - API correctly accepts special characters in titles
                // xss_attempt case removed - API correctly accepts special characters in titles
                case "invalid_schema":
                    model.Schema = null;
                    break;
                case "duplicate_uids":
                    if (model.Schema?.Count > 0)
                    {
                        model.Schema[0].Uid = "duplicate_uid";
                        if (model.Schema.Count > 1)
                        {
                            model.Schema[1].Uid = "duplicate_uid";
                        }
                    }
                    break;
            }
            
            return model;
        }
        #endregion

        #region Negative Path Tests - Authentication & Authorization Errors
        
        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Fail_When_Not_Logged_In_Create()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField_NotLoggedIn");
            var unauthenticatedClient = new ContentstackClient(new ContentstackClientOptions
            {
                Host = "api.contentstack.io"
            });
            var stack = unauthenticatedClient.Stack("dummy_api_key");

            try
            {
                stack.GlobalField().Create(_modelling);
                AssertLogger.Fail("Expected InvalidOperationException for not logged in");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldAuthError(ex, "CreateNotLoggedIn");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Fail_When_Not_Logged_In_Fetch()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchGlobalField_NotLoggedIn");
            var unauthenticatedClient = new ContentstackClient(new ContentstackClientOptions
            {
                Host = "api.contentstack.io"
            });
            var stack = unauthenticatedClient.Stack("dummy_api_key");

            try
            {
                stack.GlobalField("dummy_uid").Fetch();
                AssertLogger.Fail("Expected InvalidOperationException for not logged in");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldAuthError(ex, "FetchNotLoggedIn");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Fail_When_Not_Logged_In_Update()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateGlobalField_NotLoggedIn");
            var unauthenticatedClient = new ContentstackClient(new ContentstackClientOptions
            {
                Host = "api.contentstack.io"
            });
            var stack = unauthenticatedClient.Stack("dummy_api_key");

            try
            {
                stack.GlobalField("dummy_uid").Update(_modelling);
                AssertLogger.Fail("Expected InvalidOperationException for not logged in");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldAuthError(ex, "UpdateNotLoggedIn");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Fail_When_Not_Logged_In_Delete()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteGlobalField_NotLoggedIn");
            var unauthenticatedClient = new ContentstackClient(new ContentstackClientOptions
            {
                Host = "api.contentstack.io"
            });
            var stack = unauthenticatedClient.Stack("dummy_api_key");

            try
            {
                stack.GlobalField("dummy_uid").Delete();
                AssertLogger.Fail("Expected InvalidOperationException for not logged in");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldAuthError(ex, "DeleteNotLoggedIn");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Fail_When_Not_Logged_In_Query()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryGlobalField_NotLoggedIn");
            var unauthenticatedClient = new ContentstackClient(new ContentstackClientOptions
            {
                Host = "api.contentstack.io"
            });
            var stack = unauthenticatedClient.Stack("dummy_api_key");

            try
            {
                stack.GlobalField().Query().Find();
                AssertLogger.Fail("Expected InvalidOperationException for not logged in");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldAuthError(ex, "QueryNotLoggedIn");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Fail_With_Invalid_Auth_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "GlobalField_InvalidAuthToken");
            var invalidClient = new ContentstackClient(new ContentstackClientOptions
            {
                Host = "api.contentstack.io",
                Authtoken = "invalid_auth_token_12345"
            });
            var invalidStack = invalidClient.Stack(_stack.APIKey);

            try
            {
                invalidStack.GlobalField().Query().Find();
                AssertLogger.Fail("Expected ContentstackErrorException for invalid auth token");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldAuthError(ex, "InvalidAuthToken");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Fail_With_Empty_API_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "GlobalField_EmptyApiKey");
            
            try
            {
                _client.Stack("").GlobalField().Query().Find();
                AssertLogger.Fail("Expected InvalidOperationException for empty API key");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldAuthError(ex, "EmptyApiKey");
            }
        }
        #endregion

        #region Negative Path Tests - Input Validation Errors

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Fail_Create_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField_NullModel");
            
            try
            {
                _stack.GlobalField().Create(null);
                AssertLogger.Fail("Expected ArgumentNullException for null model");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "CreateNullModel");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Fail_Create_With_Empty_Title()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField_EmptyTitle");
            var invalidModel = CreateInvalidGlobalFieldModel("empty_title");
            
            try
            {
                _stack.GlobalField().Create(invalidModel);
                AssertLogger.Fail("Expected validation error for empty title");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "CreateEmptyTitle");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_Fail_Create_With_Invalid_Schema()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField_InvalidSchema");
            var invalidModel = CreateInvalidGlobalFieldModel("invalid_schema");
            
            try
            {
                _stack.GlobalField().Create(invalidModel);
                AssertLogger.Fail("Expected validation error for invalid schema");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "CreateInvalidSchema");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test020_Should_Fail_Create_With_Duplicate_Field_UIDs()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField_DuplicateFieldUIDs");
            var invalidModel = CreateInvalidGlobalFieldModel("duplicate_uids");
            
            try
            {
                _stack.GlobalField().Create(invalidModel);
                AssertLogger.Fail("Expected validation error for duplicate field UIDs");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "CreateDuplicateUIDs");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test021_Should_Fail_Update_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateGlobalField_NullModel");
            
            try
            {
                _stack.GlobalField(_modelling.Uid).Update(null);
                AssertLogger.Fail("Expected ArgumentNullException for null model");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "UpdateNullModel");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Fail_Update_With_Invalid_Data()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateGlobalField_InvalidData");
            var invalidModel = CreateInvalidGlobalFieldModel("null_title");
            
            try
            {
                _stack.GlobalField(_modelling.Uid).Update(invalidModel);
                AssertLogger.Fail("Expected validation error for invalid data");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "UpdateInvalidData");
            }
        }
        #endregion

        #region Negative Path Tests - Non-Existent Resource Errors

        [TestMethod]
        [DoNotParallelize]
        public void Test023_Should_Fail_Fetch_Non_Existent_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchGlobalField_NonExistent");
            
            try
            {
                _stack.GlobalField(InvalidGlobalFieldUid).Fetch();
                AssertLogger.Fail("Expected ContentstackErrorException for non-existent global field");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldNotFoundError(ex, "FetchNonExistent");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test024_Should_Fail_Fetch_Async_Non_Existent_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAsyncGlobalField_NonExistent");
            
            try
            {
                await _stack.GlobalField(InvalidGlobalFieldUid).FetchAsync();
                AssertLogger.Fail("Expected ContentstackErrorException for non-existent global field");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldNotFoundError(ex, "FetchAsyncNonExistent");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test025_Should_Fail_Update_Non_Existent_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateGlobalField_NonExistent");
            
            try
            {
                _stack.GlobalField(InvalidGlobalFieldUid).Update(_modelling);
                AssertLogger.Fail("Expected ContentstackErrorException for non-existent global field");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldNotFoundError(ex, "UpdateNonExistent");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test026_Should_Fail_Update_Async_Non_Existent_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateAsyncGlobalField_NonExistent");
            
            try
            {
                await _stack.GlobalField(InvalidGlobalFieldUid).UpdateAsync(_modelling);
                AssertLogger.Fail("Expected ContentstackErrorException for non-existent global field");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldNotFoundError(ex, "UpdateAsyncNonExistent");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test027_Should_Fail_Delete_Non_Existent_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteGlobalField_NonExistent");
            
            try
            {
                _stack.GlobalField(InvalidGlobalFieldUid).Delete();
                AssertLogger.Fail("Expected ContentstackErrorException for non-existent global field");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldNotFoundError(ex, "DeleteNonExistent");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test028_Should_Fail_Delete_Async_Non_Existent_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteAsyncGlobalField_NonExistent");
            
            try
            {
                await _stack.GlobalField(InvalidGlobalFieldUid).DeleteAsync();
                AssertLogger.Fail("Expected ContentstackErrorException for non-existent global field");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldNotFoundError(ex, "DeleteAsyncNonExistent");
            }
        }
        #endregion

        #region Negative Path Tests - Invalid UID Errors

        [TestMethod]
        [DoNotParallelize]
        public void Test029_Should_Fail_Fetch_With_Empty_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchGlobalField_EmptyUID");
            
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.GlobalField("").Fetch(),
                "FetchEmptyUID");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Fail_Update_With_Empty_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateGlobalField_EmptyUID");
            
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.GlobalField("").Update(_modelling),
                "UpdateEmptyUID");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test031_Should_Fail_Delete_With_Empty_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteGlobalField_EmptyUID");
            
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.GlobalField("").Delete(),
                "DeleteEmptyUID");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test032_Should_Fail_Create_With_UID_Set()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField_UIDSet");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => _stack.GlobalField("some_uid").Create(_modelling),
                "CreateWithUID");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test033_Should_Fail_Query_With_UID_Set()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryGlobalField_UIDSet");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => _stack.GlobalField("some_uid").Query().Find(),
                "QueryWithUID");
        }
        #endregion

        #region Negative Path Tests - Boundary & Edge Cases

        [TestMethod]
        [DoNotParallelize]
        public void Test034_Should_Fail_Create_With_Extremely_Long_Title()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField_ExtremelyLongTitle");
            var invalidModel = CreateInvalidGlobalFieldModel("long_title");
            
            try
            {
                _stack.GlobalField().Create(invalidModel);
                AssertLogger.Fail("Expected validation error for extremely long title");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "CreateLongTitle");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test035_Should_Fail_Create_With_Special_Characters_In_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField_SpecialCharactersUID");
            var invalidModel = Contentstack.serialize<ContentModelling>(_client.serializer, "globalfield.json");
            invalidModel.Uid = "invalid@uid#with$special%characters";
            
            try
            {
                _stack.GlobalField().Create(invalidModel);
                AssertLogger.Fail("Expected validation error for special characters in UID");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "CreateSpecialCharUID");
            }
        }

        // Test036 removed: SQL injection strings are valid title content.
        // The API correctly accepts special characters in titles and handles
        // SQL injection protection at the database layer, not input validation.

        // Test037 removed: XSS strings are valid title content.
        // The API correctly accepts special characters in titles and handles
        // XSS protection at the output/rendering layer, not input validation.
        #endregion

        #region Negative Path Tests - Network & API Version Errors

        [TestMethod]
        [DoNotParallelize]
        public void Test038_Should_Handle_Network_Timeout_Gracefully()
        {
            TestOutputLogger.LogContext("TestScenario", "GlobalField_NetworkTimeout");
            
            // This test would require network manipulation which is complex
            // For now, we'll test with invalid host to simulate network issues
            var networkClient = new ContentstackClient(new ContentstackClientOptions
            {
                Host = "invalid.host.that.does.not.exist.contentstack.io",
                Authtoken = _client.contentstackOptions.Authtoken
            });
            var networkStack = networkClient.Stack(_stack.APIKey);

            try
            {
                networkStack.GlobalField().Query().Find();
                AssertLogger.Fail("Expected network-related exception");
            }
            catch (Exception ex)
            {
                // Accept various network-related exceptions
                AssertLogger.IsTrue(
                    ex is System.Net.Http.HttpRequestException ||
                    ex is System.Net.Sockets.SocketException ||
                    ex is ContentstackErrorException,
                    "Expected network-related exception",
                    "NetworkException");
            }
        }

        // Test039 removed: Invalid API versions are gracefully ignored by the API.
        // The API correctly falls back to the default version (3.2) when given 
        // invalid version headers, which is proper backward compatibility behavior.
        #endregion

        #region Async Error Tests - Comprehensive Coverage

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test040_Should_Fail_Create_Async_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateAsyncGlobalField_NullModel");
            
            try
            {
                await _stack.GlobalField().CreateAsync(null);
                AssertLogger.Fail("Expected ArgumentNullException for null model");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "CreateAsyncNullModel");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test041_Should_Fail_Update_Async_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateAsyncGlobalField_NullModel");
            
            try
            {
                await _stack.GlobalField(_modelling.Uid).UpdateAsync(null);
                AssertLogger.Fail("Expected ArgumentNullException for null model");
            }
            catch (Exception ex)
            {
                AssertGlobalFieldValidationError(ex, "UpdateAsyncNullModel");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test042_Should_Fail_Fetch_Async_With_Empty_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAsyncGlobalField_EmptyUID");
            
            try
            {
                await _stack.GlobalField("").FetchAsync();
                AssertLogger.Fail("Expected ArgumentException for empty UID");
            }
            catch (ArgumentException)
            {
                // Expected exception - test passes
            }
        }

        [TestMethod]
        [DoNotParallelize]  
        public async System.Threading.Tasks.Task Test043_Should_Fail_Update_Async_With_Empty_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateAsyncGlobalField_EmptyUID");
            
            try
            {
                await _stack.GlobalField("").UpdateAsync(_modelling);
                AssertLogger.Fail("Expected ArgumentException for empty UID");
            }
            catch (ArgumentException)
            {
                // Expected exception - test passes
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test044_Should_Fail_Delete_Async_With_Empty_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteAsyncGlobalField_EmptyUID");
            
            try
            {
                await _stack.GlobalField("").DeleteAsync();
                AssertLogger.Fail("Expected ArgumentException for empty UID");
            }
            catch (ArgumentException)
            {
                // Expected exception - test passes
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test045_Should_Fail_Query_Async_With_UID_Set()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryAsyncGlobalField_UIDSet");
            
            try
            {
                await _stack.GlobalField("some_uid").Query().FindAsync();
                AssertLogger.Fail("Expected InvalidOperationException when UID is set for query");
            }
            catch (InvalidOperationException)
            {
                // Expected exception - test passes
            }
        }
        #endregion
    }
}
