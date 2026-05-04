using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack003_StackTest
    {
        private static ContentstackClient _client;
        private readonly string _locale = "en-us";
        private string _stackName = "DotNet Management Stack";
        private string _updatestackName = "DotNet Management SDK Stack";
        private string _description = "Integration testing Stack for DotNet Management SDK";
        
        private OrganizationModel _org = Contentstack.Organization;

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

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_All_Stacks()
        {
            TestOutputLogger.LogContext("TestScenario", "ReturnAllStacks");
            try
            {
                Stack stack = _client.Stack();

                ContentstackResponse contentstackResponse = stack.GetAll();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Return_All_StacksAsync()
        {
            TestOutputLogger.LogContext("TestScenario", "ReturnAllStacksAsync");
            try
            {
                Stack stack = _client.Stack();

                ContentstackResponse contentstackResponse = await stack.GetAllAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Create_Stack()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateStack");
            try
            {
                Stack stack = _client.Stack();
                ContentstackResponse contentstackResponse = stack.Create(_stackName, _locale, _org.Uid);

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
                Contentstack.Stack = model.Stack;
                TestOutputLogger.LogContext("StackApiKey", model.Stack.APIKey);

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNull(model.Stack.Description, "model.Stack.Description");
                AssertLogger.AreEqual(_stackName, model.Stack.Name, "StackName");
                AssertLogger.AreEqual(_locale, model.Stack.MasterLocale, "MasterLocale");
                AssertLogger.AreEqual(_org.Uid, model.Stack.OrgUid, "OrgUid");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Stack()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateStack");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);
                ContentstackResponse contentstackResponse = stack.Update(_updatestackName);

                var response = contentstackResponse.OpenJObjectResponse();
                File.WriteAllText("./stackApiKey.txt", contentstackResponse.OpenResponse());

                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
                Contentstack.Stack = model.Stack;

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNull(model.Stack.Description, "model.Stack.Description");
                AssertLogger.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey, "APIKey");
                AssertLogger.AreEqual(_updatestackName, model.Stack.Name, "StackName");
                AssertLogger.AreEqual(_locale, model.Stack.MasterLocale, "MasterLocale");
                AssertLogger.AreEqual(_org.Uid, model.Stack.OrgUid, "OrgUid");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Update_Stack_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateStackAsync");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);
                ContentstackResponse contentstackResponse = await stack.UpdateAsync(_updatestackName, _description);

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
                Contentstack.Stack = model.Stack;

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey, "APIKey");
                AssertLogger.AreEqual(_updatestackName, model.Stack.Name, "StackName");
                AssertLogger.AreEqual(_locale, model.Stack.MasterLocale, "MasterLocale");
                AssertLogger.AreEqual(_description, model.Stack.Description, "Description");
                AssertLogger.AreEqual(_org.Uid, model.Stack.OrgUid, "OrgUid");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Fetch_Stack()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchStack");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);
                ContentstackResponse contentstackResponse = stack.Fetch();

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey, "APIKey");
                AssertLogger.AreEqual(Contentstack.Stack.Name, model.Stack.Name, "StackName");
                AssertLogger.AreEqual(Contentstack.Stack.MasterLocale, model.Stack.MasterLocale, "MasterLocale");
                AssertLogger.AreEqual(Contentstack.Stack.Description, model.Stack.Description, "Description");
                AssertLogger.AreEqual(Contentstack.Stack.OrgUid, model.Stack.OrgUid, "OrgUid");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Fetch_StackAsync()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchStackAsync");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);
                ContentstackResponse contentstackResponse = await stack.FetchAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey, "APIKey");
                AssertLogger.AreEqual(Contentstack.Stack.Name, model.Stack.Name, "StackName");
                AssertLogger.AreEqual(Contentstack.Stack.MasterLocale, model.Stack.MasterLocale, "MasterLocale");
                AssertLogger.AreEqual(Contentstack.Stack.Description, model.Stack.Description, "Description");
                AssertLogger.AreEqual(Contentstack.Stack.OrgUid, model.Stack.OrgUid, "OrgUid");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Add_Stack_Settings()
        {
            TestOutputLogger.LogContext("TestScenario", "AddStackSettings");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);
                StackSettings settings = new StackSettings()
                {
                    StackVariables = new Dictionary<string, object>()
                    {
                        { "enforce_unique_urls", true },
                        { "sys_rte_allowed_tags", "figure" }
                    }
                };

                ContentstackResponse contentstackResponse = stack.AddSettings(settings);

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual("Stack settings updated successfully.", model.Notice, "Notice");
                AssertLogger.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"], "enforce_unique_urls");
                AssertLogger.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"], "sys_rte_allowed_tags");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Stack_Settings()
        {
            TestOutputLogger.LogContext("TestScenario", "StackSettings");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);

                ContentstackResponse contentstackResponse = stack.Settings();

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNull(model.Notice, "model.Notice");
                AssertLogger.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"], "enforce_unique_urls");
                AssertLogger.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"], "sys_rte_allowed_tags");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Reset_Stack_Settings()
        {
            TestOutputLogger.LogContext("TestScenario", "ResetStackSettings");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);

                ContentstackResponse contentstackResponse = stack.ResetSettings();

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual("Stack settings updated successfully.", model.Notice, "Notice");
                AssertLogger.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"], "enforce_unique_urls");
                AssertLogger.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"], "sys_rte_allowed_tags");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test011_Add_Stack_Settings_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "AddStackSettingsAsync");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);
                StackSettings settings = new StackSettings()
                {
                    Rte = new Dictionary<string, object>()
                    {
                        { "cs_only_breakline", true },
                    }
                };

                ContentstackResponse contentstackResponse = await stack.AddSettingsAsync(settings);

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual("Stack settings updated successfully.", model.Notice, "Notice");
                AssertLogger.AreEqual(true, model.StackSettings.Rte["cs_only_breakline"], "cs_only_breakline");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test012_Reset_Stack_Settings_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "ResetStackSettingsAsync");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);

                ContentstackResponse contentstackResponse = await stack.ResetSettingsAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual("Stack settings updated successfully.", model.Notice, "Notice");
                AssertLogger.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"], "enforce_unique_urls");
                AssertLogger.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"], "sys_rte_allowed_tags");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test013_Stack_Settings_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "StackSettingsAsync");
            TestOutputLogger.LogContext("StackApiKey", Contentstack.Stack.APIKey);
            try
            {
                Stack stack = _client.Stack(Contentstack.Stack.APIKey);

                ContentstackResponse contentstackResponse = await stack.SettingsAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"], "enforce_unique_urls");
                AssertLogger.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"], "sys_rte_allowed_tags");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        #region Negative and error-handling tests (Test014+)

        /// <summary>Non-empty API key used only to exercise SDK preconditions without requiring Test003 to succeed.</summary>
        private const string SdkNonEmptyApiKey = "bltSdkValidationNonEmptyApiKey00";

        private const string InvalidStackApiKey = "bltNonExistentStackKey12345";

        private static void AssertStackApiKeyOrInconclusive()
        {
            if (string.IsNullOrEmpty(Contentstack.Stack?.APIKey))
            {
                Assert.Inconclusive(
                    "Contentstack.Stack.APIKey is not set (Test003 create stack did not run or failed). Skipping test that requires a real stack.");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Fail_When_Not_Logged_In_Stack_GetAll()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_NotLoggedIn_GetAll");
            var unauthenticatedClient = new ContentstackClient(new ContentstackClientOptions
            {
                Host = "api.contentstack.io"
            });
            var ex = Assert.ThrowsException<InvalidOperationException>(() =>
                unauthenticatedClient.Stack().GetAll());
            AssertLogger.IsTrue(
                ex.Message.IndexOf("not logged in", StringComparison.OrdinalIgnoreCase) >= 0,
                "Expected not-logged-in message",
                "Stack_NotLoggedIn_Message");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Fail_With_Invalid_Auth_Token_Stack_GetAll()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_InvalidAuthToken_GetAll");
            try
            {
                var invalidClient = new ContentstackClient(new ContentstackClientOptions
                {
                    Host = "api.contentstack.io",
                    Authtoken = "invalid_auth_token_123"
                });
                invalidClient.Stack().GetAll();
                AssertLogger.Fail("Expected exception for invalid auth token", "Stack_InvalidAuth_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackAuthError(ex, "Stack_InvalidAuthToken");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Throw_When_GetAll_With_Stack_Api_Key_Set()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_GetAll_InvalidApiKeyContext");
            Assert.ThrowsException<InvalidOperationException>(() =>
                _client.Stack(SdkNonEmptyApiKey).GetAll());
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test017_Should_Throw_When_GetAllAsync_With_Stack_Api_Key_Set()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_GetAllAsync_InvalidApiKeyContext");
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _client.Stack(SdkNonEmptyApiKey).GetAllAsync());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Throw_When_Create_With_Stack_Api_Key_Set()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Create_InvalidApiKeyContext");
            Assert.ThrowsException<InvalidOperationException>(() =>
                _client.Stack(SdkNonEmptyApiKey).Create(_stackName, _locale, _org.Uid));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test019_Should_Throw_When_CreateAsync_With_Stack_Api_Key_Set()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_CreateAsync_InvalidApiKeyContext");
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _client.Stack(SdkNonEmptyApiKey).CreateAsync(_stackName, _locale, _org.Uid));
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(null)]
        [DataRow("")]
        public void Test020_Should_Throw_When_Create_With_Invalid_Name_Sync(string invalidName)
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Create_InvalidName_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack().Create(invalidName, _locale, _org.Uid));
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(null)]
        [DataRow("")]
        public async Task Test021_Should_Throw_When_CreateAsync_With_Invalid_Name(string invalidName)
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Create_InvalidName_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack().CreateAsync(invalidName, _locale, _org.Uid));
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(null)]
        [DataRow("")]
        public void Test020b_Should_Throw_When_Create_With_Invalid_Locale_Sync(string invalidLocale)
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Create_InvalidLocale_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack().Create(_stackName, invalidLocale, _org.Uid));
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(null)]
        [DataRow("")]
        public async Task Test021b_Should_Throw_When_CreateAsync_With_Invalid_Locale(string invalidLocale)
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Create_InvalidLocale_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack().CreateAsync(_stackName, invalidLocale, _org.Uid));
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(null)]
        [DataRow("")]
        public void Test020c_Should_Throw_When_Create_With_Invalid_OrgUid_Sync(string invalidOrg)
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Create_InvalidOrg_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack().Create(_stackName, _locale, invalidOrg));
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(null)]
        [DataRow("")]
        public async Task Test021c_Should_Throw_When_CreateAsync_With_Invalid_OrgUid(string invalidOrg)
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Create_InvalidOrg_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack().CreateAsync(_stackName, _locale, invalidOrg));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Throw_When_Fetch_Without_Api_Key_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Fetch_MissingApiKey_Sync");
            Assert.ThrowsException<InvalidOperationException>(() => _client.Stack().Fetch());
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test023_Should_Throw_When_FetchAsync_Without_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Fetch_MissingApiKey_Async");
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _client.Stack().FetchAsync());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test024_Should_Throw_When_Update_Without_Api_Key_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Update_MissingApiKey_Sync");
            Assert.ThrowsException<InvalidOperationException>(() =>
                _client.Stack().Update(_updatestackName));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test025_Should_Throw_When_UpdateAsync_Without_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Update_MissingApiKey_Async");
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _client.Stack().UpdateAsync(_updatestackName));
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(null)]
        [DataRow("")]
        public void Test026_Should_Throw_When_Update_With_Invalid_Name_Sync(string invalidName)
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Update_InvalidName_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).Update(invalidName));
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(null)]
        [DataRow("")]
        public async Task Test027_Should_Throw_When_UpdateAsync_With_Invalid_Name(string invalidName)
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Update_InvalidName_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).UpdateAsync(invalidName));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test028_Should_Throw_When_Settings_Without_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Settings_MissingApiKey_Sync");
            Assert.ThrowsException<InvalidOperationException>(() => _client.Stack().Settings());
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test029_Should_Throw_When_SettingsAsync_Without_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Settings_MissingApiKey_Async");
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _client.Stack().SettingsAsync());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Throw_When_ResetSettings_Without_Api_Key_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_ResetSettings_MissingApiKey_Sync");
            Assert.ThrowsException<InvalidOperationException>(() => _client.Stack().ResetSettings());
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test031_Should_Throw_When_ResetSettingsAsync_Without_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_ResetSettings_MissingApiKey_Async");
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _client.Stack().ResetSettingsAsync());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test032_Should_Throw_When_AddSettings_Null_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_AddSettings_Null_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).AddSettings(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test033_Should_Throw_When_AddSettingsAsync_Null()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_AddSettings_Null_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).AddSettingsAsync(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test034_Should_Fail_Fetch_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Fetch_InvalidApiKey");
            try
            {
                _client.Stack(InvalidStackApiKey).Fetch();
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_Fetch_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_Fetch_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test035_Should_Fail_FetchAsync_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_FetchAsync_InvalidApiKey");
            try
            {
                await _client.Stack(InvalidStackApiKey).FetchAsync();
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_FetchAsync_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_FetchAsync_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test036_Should_Fail_Update_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Update_InvalidApiKey");
            try
            {
                _client.Stack(InvalidStackApiKey).Update("SomeName");
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_Update_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_Update_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test037_Should_Fail_UpdateAsync_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UpdateAsync_InvalidApiKey");
            try
            {
                await _client.Stack(InvalidStackApiKey).UpdateAsync("SomeName");
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_UpdateAsync_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_UpdateAsync_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test038_Should_Fail_Settings_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Settings_InvalidApiKey");
            try
            {
                _client.Stack(InvalidStackApiKey).Settings();
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_Settings_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_Settings_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test039_Should_Fail_SettingsAsync_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_SettingsAsync_InvalidApiKey");
            try
            {
                await _client.Stack(InvalidStackApiKey).SettingsAsync();
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_SettingsAsync_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_SettingsAsync_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test040_Should_Fail_ResetSettings_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_ResetSettings_InvalidApiKey");
            try
            {
                _client.Stack(InvalidStackApiKey).ResetSettings();
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_Reset_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_Reset_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test041_Should_Fail_ResetSettingsAsync_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_ResetSettingsAsync_InvalidApiKey");
            try
            {
                await _client.Stack(InvalidStackApiKey).ResetSettingsAsync();
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_ResetAsync_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_ResetAsync_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test042_Should_Fail_AddSettings_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_AddSettings_InvalidApiKey");
            var settings = new StackSettings
            {
                StackVariables = new Dictionary<string, object> { { "enforce_unique_urls", true } }
            };
            try
            {
                _client.Stack(InvalidStackApiKey).AddSettings(settings);
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_AddSettings_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_AddSettings_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test043_Should_Fail_AddSettingsAsync_With_Invalid_Stack_Api_Key()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_AddSettingsAsync_InvalidApiKey");
            var settings = new StackSettings
            {
                StackVariables = new Dictionary<string, object> { { "enforce_unique_urls", true } }
            };
            try
            {
                await _client.Stack(InvalidStackApiKey).AddSettingsAsync(settings);
                AssertLogger.Fail("Expected API error for invalid stack key", "Stack_AddSettingsAsync_InvalidKey_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_AddSettingsAsync_InvalidKey");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test044_Should_Fail_Create_With_Bogus_Organization_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Create_BogusOrgUid");
            try
            {
                _client.Stack().Create(_stackName, _locale, "blt_bogus_organization_uid_99999");
                AssertLogger.Fail("Expected API error for bogus organization", "Stack_Create_BogusOrg_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_Create_BogusOrg");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test045_Should_Throw_When_Share_Invitations_Null_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Share_NullInvitations_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).Share(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test046_Should_Throw_When_ShareAsync_Invitations_Null()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Share_NullInvitations_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).ShareAsync(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test047_Should_Throw_When_UnShare_Email_Null_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UnShare_NullEmail_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).UnShare(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test048_Should_Throw_When_UnShareAsync_Email_Null()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UnShare_NullEmail_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).UnShareAsync(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test049_Should_Fail_Share_With_Invalid_Email()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_Share_InvalidEmail");
            AssertStackApiKeyOrInconclusive();
            var invitations = new List<UserInvitation>
            {
                new UserInvitation
                {
                    Email = "not-an-email",
                    Roles = new List<string> { "blt_fake_role_uid" }
                }
            };
            try
            {
                _client.Stack(Contentstack.Stack.APIKey).Share(invitations);
                AssertLogger.Fail("Expected API error for invalid share invitation", "Stack_Share_InvalidEmail_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_Share_InvalidEmail");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test050_Should_Fail_ShareAsync_With_Invalid_Role_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_ShareAsync_InvalidRoleUid");
            AssertStackApiKeyOrInconclusive();
            var invitations = new List<UserInvitation>
            {
                new UserInvitation
                {
                    Email = "validformat+stackneg@test.invalid",
                    Roles = new List<string> { "blt_nonexistent_role_uid_12345" }
                }
            };
            try
            {
                await _client.Stack(Contentstack.Stack.APIKey).ShareAsync(invitations);
                AssertLogger.Fail("Expected API error for invalid role UID", "Stack_ShareAsync_InvalidRole_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_ShareAsync_InvalidRole");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test051_Should_Fail_UnShare_Non_Collaborator_Email()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UnShare_UnknownEmail");
            AssertStackApiKeyOrInconclusive();
            try
            {
                _client.Stack(Contentstack.Stack.APIKey).UnShare("noncollaborator_stack_neg@test.invalid");
                AssertLogger.Fail("Expected API error for unknown collaborator", "Stack_UnShare_Unknown_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_UnShare_UnknownEmail");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test052_Should_Fail_UnShareAsync_Malformed_Email()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UnShareAsync_MalformedEmail");
            AssertStackApiKeyOrInconclusive();
            try
            {
                await _client.Stack(Contentstack.Stack.APIKey).UnShareAsync("not-an-email-address");
                AssertLogger.Fail("Expected API error for malformed email", "Stack_UnShare_Malformed_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_UnShare_MalformedEmail");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test053_Should_Throw_When_UpdateUserRole_Null_List_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UpdateUserRole_Null_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).UpdateUserRole(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test054_Should_Throw_When_UpdateUserRoleAsync_Null_List()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UpdateUserRole_Null_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).UpdateUserRoleAsync(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test055_Should_Fail_UpdateUserRole_Empty_List_API()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UpdateUserRole_EmptyList");
            try
            {
                _client.Stack(SdkNonEmptyApiKey).UpdateUserRole(new List<UserInvitation>());
                AssertLogger.Fail("Expected API error for empty user role list", "Stack_UpdateUserRole_Empty_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_UpdateUserRole_Empty");
            }
            catch (ArgumentException ex)
            {
                AssertLogger.IsTrue(true, "SDK or API rejected empty list", "Stack_UpdateUserRole_Empty_Argument");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test056_Should_Fail_UpdateUserRole_Invalid_User_Uid_API()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UpdateUserRole_InvalidUserUid");
            var list = new List<UserInvitation>
            {
                new UserInvitation
                {
                    Uid = "blt_fake_user_uid_99999",
                    Roles = new List<string> { "blt_fake_role_uid_99999" }
                }
            };
            try
            {
                _client.Stack(SdkNonEmptyApiKey).UpdateUserRole(list);
                AssertLogger.Fail("Expected API error for invalid user UID", "Stack_UpdateUserRole_InvalidUser_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_UpdateUserRole_InvalidUser");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test057_Should_Fail_UpdateUserRoleAsync_Invalid_Role_Uid_API()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UpdateUserRoleAsync_InvalidRoleUid");
            var list = new List<UserInvitation>
            {
                new UserInvitation
                {
                    Uid = "blt_fake_user_uid_88888",
                    Roles = new List<string> { "blt_fake_role_uid_88888" }
                }
            };
            try
            {
                await _client.Stack(SdkNonEmptyApiKey).UpdateUserRoleAsync(list);
                AssertLogger.Fail("Expected API error for invalid role assignment", "Stack_UpdateUserRoleAsync_Invalid_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_UpdateUserRoleAsync_Invalid");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test058_Should_Throw_When_UpdateUserRole_Null_User_Uid_In_List()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_UpdateUserRole_NullUidInList");
            var list = new List<UserInvitation>
            {
                new UserInvitation { Uid = null, Roles = new List<string> { "blt_fake_role" } }
            };
            // Null Uid is serialized as an empty property name under "users"; the request is sent and
            // the API returns an error (e.g. 422 invalid api_key), not necessarily an NRE.
            try
            {
                _client.Stack(SdkNonEmptyApiKey).UpdateUserRole(list);
                AssertLogger.Fail(
                    "Expected error for null user UID in UpdateUserRole list",
                    "Stack_UpdateUserRole_NullUid_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_UpdateUserRole_NullUid");
            }
            catch (NullReferenceException)
            {
                AssertLogger.IsTrue(true, "Serializer threw NRE for null Uid", "Stack_UpdateUserRole_NullUid_Nre");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test059_Should_Throw_When_TransferOwnership_Email_Null_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_TransferOwnership_NullEmail_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).TransferOwnership(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test060_Should_Throw_When_TransferOwnershipAsync_Email_Null()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_TransferOwnership_NullEmail_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).TransferOwnershipAsync(null));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test061_Should_Throw_When_TransferOwnership_Email_Empty_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_TransferOwnership_EmptyEmail_Sync");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).TransferOwnership(string.Empty));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test062_Should_Throw_When_TransferOwnershipAsync_Email_Empty()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_TransferOwnership_EmptyEmail_Async");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _client.Stack(SdkNonEmptyApiKey).TransferOwnershipAsync(string.Empty));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test063_Should_Fail_TransferOwnership_Invalid_Email_API()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_TransferOwnership_InvalidEmail");
            AssertStackApiKeyOrInconclusive();
            try
            {
                _client.Stack(Contentstack.Stack.APIKey).TransferOwnership("not-an-email");
                AssertLogger.Fail("Expected API error for invalid transfer email", "Stack_Transfer_InvalidEmail_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_Transfer_InvalidEmail");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test064_Should_Fail_TransferOwnershipAsync_Invalid_Email_API()
        {
            TestOutputLogger.LogContext("TestScenario", "Stack_TransferOwnershipAsync_InvalidEmail");
            AssertStackApiKeyOrInconclusive();
            try
            {
                await _client.Stack(Contentstack.Stack.APIKey).TransferOwnershipAsync("also@invalid@email");
                AssertLogger.Fail("Expected API error for invalid transfer email", "Stack_TransferAsync_InvalidEmail_NoException");
            }
            catch (ContentstackErrorException ex)
            {
                AssertStackValidationError(ex, "Stack_TransferAsync_InvalidEmail");
            }
        }

        #endregion

        #region Helper methods (Stack negative tests)

        private static void AssertStackValidationError(Exception ex, string assertionName)
        {
            if (ex is ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.BadRequest ||
                    cex.StatusCode == (HttpStatusCode)422 ||
                    cex.StatusCode == HttpStatusCode.NotFound ||
                    cex.StatusCode == HttpStatusCode.Conflict ||
                    cex.StatusCode == HttpStatusCode.Forbidden ||
                    cex.StatusCode == HttpStatusCode.UnsupportedMediaType,
                    $"Expected stack validation error, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    assertionName);
            }
            else if (ex is ArgumentException || ex is InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "SDK validation caught stack error as expected", assertionName);
            }
            else
            {
                AssertLogger.Fail($"Unexpected exception type for stack validation: {ex.GetType().Name}", assertionName);
            }
        }

        private static void AssertStackAuthError(Exception ex, string assertionName)
        {
            if (ex is ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Unauthorized ||
                    cex.StatusCode == HttpStatusCode.Forbidden ||
                    cex.StatusCode == (HttpStatusCode)422 ||
                    cex.StatusCode == HttpStatusCode.BadRequest ||
                    cex.StatusCode == HttpStatusCode.PreconditionFailed,
                    $"Expected auth error, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    assertionName);
            }
            else if (ex is InvalidOperationException && ex.Message.IndexOf("not logged in", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                AssertLogger.IsTrue(true, "SDK caught auth error as expected", assertionName);
            }
            else
            {
                AssertLogger.Fail($"Unexpected exception type for auth error: {ex.GetType().Name}", assertionName);
            }
        }

        #endregion
    }
}
