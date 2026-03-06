using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack004_GlobalFieldTest
    {
        private Stack _stack;
        private ContentModelling _modelling;

        private static string _host => Contentstack.Client.contentstackOptions.Host;

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
            _modelling = Contentstack.serialize<ContentModelling>(Contentstack.Client.serializer, "globalfield.json");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField().Create()", "POST",
                    $"https://{_host}/v3/stacks/global_fields");

                ContentstackResponse response = _stack.GlobalField().Create(_modelling);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(globalField?.Modelling?.Title == _modelling.Title,
                    "Title matches", expected: _modelling.Title, actual: globalField?.Modelling?.Title, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
                Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
                Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Fetch_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField(uid).Fetch()", "GET",
                    $"https://{_host}/v3/stacks/global_fields/{_modelling.Uid}");

                ContentstackResponse response = _stack.GlobalField(_modelling.Uid).Fetch();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(globalField?.Modelling?.Uid == _modelling.Uid,
                    "UID matches", expected: _modelling.Uid, actual: globalField?.Modelling?.Uid, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
                Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
                Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Fetch_Async_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField(uid).FetchAsync()", "GET",
                    $"https://{_host}/v3/stacks/global_fields/{_modelling.Uid}");

                ContentstackResponse response = await _stack.GlobalField(_modelling.Uid).FetchAsync();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
                Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
                Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                _modelling.Title = "Updated title";
                TestReportHelper.LogRequest("_stack.GlobalField(uid).Update()", "PUT",
                    $"https://{_host}/v3/stacks/global_fields/{_modelling.Uid}");

                ContentstackResponse response = _stack.GlobalField(_modelling.Uid).Update(_modelling);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(globalField?.Modelling?.Title == _modelling.Title,
                    "Updated title matches", expected: _modelling.Title, actual: globalField?.Modelling?.Title, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
                Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
                Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Update_Async_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                _modelling.Title = "First Async";
                TestReportHelper.LogRequest("_stack.GlobalField(uid).UpdateAsync()", "PUT",
                    $"https://{_host}/v3/stacks/global_fields/{_modelling.Uid}");

                ContentstackResponse response = await _stack.GlobalField(_modelling.Uid).UpdateAsync(_modelling);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(globalField?.Modelling?.Title == _modelling.Title,
                    "Title matches", expected: _modelling.Title, actual: globalField?.Modelling?.Title, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
                Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
                Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Query_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField().Query().Find()", "GET",
                    $"https://{_host}/v3/stacks/global_fields");

                ContentstackResponse response = _stack.GlobalField().Query().Find();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();

                TestReportHelper.LogAssertion(globalField?.Modellings?.Count == 1,
                    "Modellings count is 1", expected: "1", actual: globalField?.Modellings?.Count.ToString(), type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modellings);
                Assert.AreEqual(1, globalField.Modellings.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006a_Should_Query_Global_Field_With_ApiVersion()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField(apiVersion: 3.2).Query().Find()", "GET",
                    $"https://{_host}/v3/stacks/global_fields",
                    headers: new System.Collections.Generic.Dictionary<string, string> { ["api_version"] = "3.2" });

                ContentstackResponse response = _stack.GlobalField(apiVersion: "3.2").Query().Find();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modellings);
                Assert.AreEqual(1, globalField.Modellings.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Query_Async_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField().Query().FindAsync()", "GET",
                    $"https://{_host}/v3/stacks/global_fields");

                ContentstackResponse response = await _stack.GlobalField().Query().FindAsync();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();

                TestReportHelper.LogAssertion(globalField?.Modellings?.Count == 1,
                    "Modellings count is 1", expected: "1", actual: globalField?.Modellings?.Count.ToString(), type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modellings);
                Assert.AreEqual(1, globalField.Modellings.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007a_Should_Query_Async_Global_Field_With_ApiVersion()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField(apiVersion: 3.2).Query().FindAsync()", "GET",
                    $"https://{_host}/v3/stacks/global_fields",
                    headers: new System.Collections.Generic.Dictionary<string, string> { ["api_version"] = "3.2" });

                ContentstackResponse response = await _stack.GlobalField(apiVersion: "3.2").Query().FindAsync();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modellings);
                Assert.AreEqual(1, globalField.Modellings.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }
    }
}
