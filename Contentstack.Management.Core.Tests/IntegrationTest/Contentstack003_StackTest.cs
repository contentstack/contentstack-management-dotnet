using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
