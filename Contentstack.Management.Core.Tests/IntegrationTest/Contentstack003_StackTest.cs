using System;
using System.Collections.Generic;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack003_StackTest
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly string _locale = "en-us";
        private string _stackName = "DotNet Management Stack";
        private string _updatestackName = "DotNet Management SDK Stack";
        private string _description = "Integration testing Stack for DotNet Management SDK";
        static string RoleUID = "";
        static string EmailSync = "testcs@contentstack.com";
        static string EmailAsync = "testcs_1@contentstack.com";
        static double Count = -1;
        
        private OrganizationModel _org = Contentstack.Organization;

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_All_Stacks()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack();

                ContentstackResponse contentstackResponse = stack.GetAll();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Count = (response["stacks"] as Newtonsoft.Json.Linq.JArray).Count;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Return_All_StacksAsync()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack();

                ContentstackResponse contentstackResponse = await stack.GetAllAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Count = (response["stacks"] as Newtonsoft.Json.Linq.JArray).Count;

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Create_Stack()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack();
                ContentstackResponse contentstackResponse = stack.Create(_stackName, _locale, _org.Uid);

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
                Contentstack.Stack = model.Stack;

                Assert.IsNotNull(response);
                Assert.IsNull(model.Stack.Description);
                Assert.AreEqual(_stackName, model.Stack.Name);
                Assert.AreEqual(_locale, model.Stack.MasterLocale);
                Assert.AreEqual(_org.Uid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Stack()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                ContentstackResponse contentstackResponse = stack.Update(_updatestackName);

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
                Contentstack.Stack = model.Stack;

                Assert.IsNotNull(response);
                Assert.IsNull(model.Stack.Description);
                Assert.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey);
                Assert.AreEqual(_updatestackName, model.Stack.Name);
                Assert.AreEqual(_locale, model.Stack.MasterLocale);
                Assert.AreEqual(_org.Uid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Update_Stack_Async()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                ContentstackResponse contentstackResponse = await stack.UpdateAsync(_updatestackName, _description);

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
                Contentstack.Stack = model.Stack;

                Assert.IsNotNull(response);
                Assert.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey);
                Assert.AreEqual(_updatestackName, model.Stack.Name);
                Assert.AreEqual(_locale, model.Stack.MasterLocale);
                Assert.AreEqual(_description, model.Stack.Description);
                Assert.AreEqual(_org.Uid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Fetch_Stack()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                ContentstackResponse contentstackResponse = stack.Fetch();

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();

                Assert.IsNotNull(response);
                Assert.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey);
                Assert.AreEqual(Contentstack.Stack.Name, model.Stack.Name);
                Assert.AreEqual(Contentstack.Stack.MasterLocale, model.Stack.MasterLocale);
                Assert.AreEqual(Contentstack.Stack.Description, model.Stack.Description);
                Assert.AreEqual(Contentstack.Stack.OrgUid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Fetch_StackAsync()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                ContentstackResponse contentstackResponse = await stack.FetchAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();

                Assert.IsNotNull(response);
                Assert.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey);
                Assert.AreEqual(Contentstack.Stack.Name, model.Stack.Name);
                Assert.AreEqual(Contentstack.Stack.MasterLocale, model.Stack.MasterLocale);
                Assert.AreEqual(Contentstack.Stack.Description, model.Stack.Description);
                Assert.AreEqual(Contentstack.Stack.OrgUid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Add_Stack_Settings()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
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

                Assert.IsNotNull(response);
                Assert.AreEqual("Stack settings updated successfully.", model.Notice);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Stack_Settings()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);

                ContentstackResponse contentstackResponse = stack.Settings();

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                Assert.IsNotNull(response);
                Assert.IsNull(model.Notice);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Reset_Stack_Settings()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);

                ContentstackResponse contentstackResponse = stack.ResetSettings();

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                Assert.IsNotNull(response);
                Assert.AreEqual("Stack settings updated successfully.", model.Notice);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test011_Add_Stack_Settings_Async()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
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

                Assert.IsNotNull(response);
                Assert.AreEqual("Stack settings updated successfully.", model.Notice);
                Assert.AreEqual(true, model.StackSettings.Rte["cs_only_breakline"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test012_Reset_Stack_Settings_Async()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);

                ContentstackResponse contentstackResponse = await stack.ResetSettingsAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                Assert.IsNotNull(response);
                Assert.AreEqual("Stack settings updated successfully.", model.Notice);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test013_Stack_Settings_Async()
        {
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);

                ContentstackResponse contentstackResponse = await stack.SettingsAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                Assert.IsNotNull(response);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
