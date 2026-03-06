using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack003_StackTest
    {
        private readonly string _locale = "en-us";
        private string _stackName = "DotNet Management Stack";
        private string _updatestackName = "DotNet Management SDK Stack";
        private string _description = "Integration testing Stack for DotNet Management SDK";

        private OrganizationModel _org = Contentstack.Organization;

        private static string _host => Contentstack.Client.contentstackOptions.Host;

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_All_Stacks()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack();
                TestReportHelper.LogRequest("stack.GetAll()", "GET",
                    $"https://{_host}/v3/stacks");

                ContentstackResponse contentstackResponse = stack.GetAll();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Return_All_StacksAsync()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack();
                TestReportHelper.LogRequest("stack.GetAllAsync()", "GET",
                    $"https://{_host}/v3/stacks");

                ContentstackResponse contentstackResponse = await stack.GetAllAsync();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Create_Stack()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack();
                TestReportHelper.LogRequest("stack.Create()", "POST",
                    $"https://{_host}/v3/stacks",
                    body: $"{{\"stack\":{{\"name\":\"{_stackName}\",\"master_locale\":\"{_locale}\",\"org_uid\":\"{_org.Uid}\"}}}}");

                ContentstackResponse contentstackResponse = stack.Create(_stackName, _locale, _org.Uid);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
                Contentstack.Stack = model.Stack;

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(model.Stack.Description == null, "Description is null", type: "IsNull");
                TestReportHelper.LogAssertion(model.Stack.Name == _stackName,
                    "Stack name matches", expected: _stackName, actual: model.Stack.Name, type: "AreEqual");
                TestReportHelper.LogAssertion(model.Stack.MasterLocale == _locale,
                    "Master locale matches", expected: _locale, actual: model.Stack.MasterLocale, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNull(model.Stack.Description);
                Assert.AreEqual(_stackName, model.Stack.Name);
                Assert.AreEqual(_locale, model.Stack.MasterLocale);
                Assert.AreEqual(_org.Uid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Stack()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                TestReportHelper.LogRequest("stack.Update()", "PUT",
                    $"https://{_host}/v3/stacks",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" },
                    body: $"{{\"stack\":{{\"name\":\"{_updatestackName}\"}}}}");

                ContentstackResponse contentstackResponse = stack.Update(_updatestackName);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                File.WriteAllText("./stackApiKey.txt", body);

                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
                Contentstack.Stack = model.Stack;

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(model.Stack.Name == _updatestackName,
                    "Stack name updated", expected: _updatestackName, actual: model.Stack.Name, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNull(model.Stack.Description);
                Assert.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey);
                Assert.AreEqual(_updatestackName, model.Stack.Name);
                Assert.AreEqual(_locale, model.Stack.MasterLocale);
                Assert.AreEqual(_org.Uid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Update_Stack_Async()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                TestReportHelper.LogRequest("stack.UpdateAsync()", "PUT",
                    $"https://{_host}/v3/stacks",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" });

                ContentstackResponse contentstackResponse = await stack.UpdateAsync(_updatestackName, _description);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
                Contentstack.Stack = model.Stack;

                TestReportHelper.LogAssertion(model.Stack.Name == _updatestackName,
                    "Stack name matches", expected: _updatestackName, actual: model.Stack.Name, type: "AreEqual");
                TestReportHelper.LogAssertion(model.Stack.Description == _description,
                    "Description matches", expected: _description, actual: model.Stack.Description, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey);
                Assert.AreEqual(_updatestackName, model.Stack.Name);
                Assert.AreEqual(_locale, model.Stack.MasterLocale);
                Assert.AreEqual(_description, model.Stack.Description);
                Assert.AreEqual(_org.Uid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Fetch_Stack()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                TestReportHelper.LogRequest("stack.Fetch()", "GET",
                    $"https://{_host}/v3/stacks",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" });

                ContentstackResponse contentstackResponse = stack.Fetch();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();

                TestReportHelper.LogAssertion(model.Stack.APIKey == Contentstack.Stack.APIKey,
                    "API key matches", expected: Contentstack.Stack.APIKey, actual: model.Stack.APIKey, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey);
                Assert.AreEqual(Contentstack.Stack.Name, model.Stack.Name);
                Assert.AreEqual(Contentstack.Stack.MasterLocale, model.Stack.MasterLocale);
                Assert.AreEqual(Contentstack.Stack.Description, model.Stack.Description);
                Assert.AreEqual(Contentstack.Stack.OrgUid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Fetch_StackAsync()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                TestReportHelper.LogRequest("stack.FetchAsync()", "GET",
                    $"https://{_host}/v3/stacks",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" });

                ContentstackResponse contentstackResponse = await stack.FetchAsync();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();

                TestReportHelper.LogAssertion(model.Stack.APIKey == Contentstack.Stack.APIKey,
                    "API key matches", expected: Contentstack.Stack.APIKey, actual: model.Stack.APIKey, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.AreEqual(Contentstack.Stack.APIKey, model.Stack.APIKey);
                Assert.AreEqual(Contentstack.Stack.Name, model.Stack.Name);
                Assert.AreEqual(Contentstack.Stack.MasterLocale, model.Stack.MasterLocale);
                Assert.AreEqual(Contentstack.Stack.Description, model.Stack.Description);
                Assert.AreEqual(Contentstack.Stack.OrgUid, model.Stack.OrgUid);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Add_Stack_Settings()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
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
                TestReportHelper.LogRequest("stack.AddSettings()", "POST",
                    $"https://{_host}/v3/stacks/settings",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" });

                ContentstackResponse contentstackResponse = stack.AddSettings(settings);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                TestReportHelper.LogAssertion(model.Notice == "Stack settings updated successfully.",
                    "Notice matches", expected: "Stack settings updated successfully.", actual: model.Notice, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.AreEqual("Stack settings updated successfully.", model.Notice);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Stack_Settings()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                TestReportHelper.LogRequest("stack.Settings()", "GET",
                    $"https://{_host}/v3/stacks/settings",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" });

                ContentstackResponse contentstackResponse = stack.Settings();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNull(model.Notice);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Reset_Stack_Settings()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                TestReportHelper.LogRequest("stack.ResetSettings()", "DELETE",
                    $"https://{_host}/v3/stacks/settings",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" });

                ContentstackResponse contentstackResponse = stack.ResetSettings();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                TestReportHelper.LogAssertion(model.Notice == "Stack settings updated successfully.",
                    "Notice matches", expected: "Stack settings updated successfully.", actual: model.Notice, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.AreEqual("Stack settings updated successfully.", model.Notice);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test011_Add_Stack_Settings_Async()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
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
                TestReportHelper.LogRequest("stack.AddSettingsAsync()", "POST",
                    $"https://{_host}/v3/stacks/settings",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" });

                ContentstackResponse contentstackResponse = await stack.AddSettingsAsync(settings);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                TestReportHelper.LogAssertion(model.Notice == "Stack settings updated successfully.",
                    "Notice matches", expected: "Stack settings updated successfully.", actual: model.Notice, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.AreEqual("Stack settings updated successfully.", model.Notice);
                Assert.AreEqual(true, model.StackSettings.Rte["cs_only_breakline"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test012_Reset_Stack_Settings_Async()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                TestReportHelper.LogRequest("stack.ResetSettingsAsync()", "DELETE",
                    $"https://{_host}/v3/stacks/settings",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" });

                ContentstackResponse contentstackResponse = await stack.ResetSettingsAsync();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                TestReportHelper.LogAssertion(model.Notice == "Stack settings updated successfully.",
                    "Notice matches", expected: "Stack settings updated successfully.", actual: model.Notice, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.AreEqual("Stack settings updated successfully.", model.Notice);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test013_Stack_Settings_Async()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Stack stack = Contentstack.Client.Stack(Contentstack.Stack.APIKey);
                TestReportHelper.LogRequest("stack.SettingsAsync()", "GET",
                    $"https://{_host}/v3/stacks/settings",
                    headers: new Dictionary<string, string> { ["api_key"] = Contentstack.Stack.APIKey ?? "***" });

                ContentstackResponse contentstackResponse = await stack.SettingsAsync();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                StackSettingsModel model = contentstackResponse.OpenTResponse<StackSettingsModel>();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.AreEqual(true, model.StackSettings.StackVariables["enforce_unique_urls"]);
                Assert.AreEqual("figure", model.StackSettings.StackVariables["sys_rte_allowed_tags"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }
    }
}
