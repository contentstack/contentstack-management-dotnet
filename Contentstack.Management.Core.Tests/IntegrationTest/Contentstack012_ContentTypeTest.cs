using System;
using System.Collections.Generic;
using System.Diagnostics;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack005_ContentTypeTest
    {
        private Stack _stack;
        private ContentModelling _singlePage;
        private ContentModelling _multiPage;

        private static string _host => Contentstack.Client.contentstackOptions.Host;

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
            _singlePage = Contentstack.serialize<ContentModelling>(Contentstack.Client.serializer, "singlepageCT.json");
            _multiPage  = Contentstack.serialize<ContentModelling>(Contentstack.Client.serializer, "multiPageCT.json");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Content_Type()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.ContentType().Create(_singlePage)", "POST",
                    $"https://{_host}/v3/stacks/content_types");

                ContentstackResponse response = _stack.ContentType().Create(_singlePage);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();

                TestReportHelper.LogAssertion(ContentType?.Modelling?.Title == _singlePage.Title,
                    "Title matches", expected: _singlePage.Title, actual: ContentType?.Modelling?.Title, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(ContentType);
                Assert.IsNotNull(ContentType.Modelling);
                Assert.AreEqual(_singlePage.Title, ContentType.Modelling.Title);
                Assert.AreEqual(_singlePage.Uid, ContentType.Modelling.Uid);
                Assert.AreEqual(_singlePage.Schema.Count, ContentType.Modelling.Schema.Count);
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
        public void Test002_Should_Create_Content_Type()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.ContentType().Create(_multiPage)", "POST",
                    $"https://{_host}/v3/stacks/content_types");

                ContentstackResponse response = _stack.ContentType().Create(_multiPage);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();

                TestReportHelper.LogAssertion(ContentType?.Modelling?.Title == _multiPage.Title,
                    "Title matches", expected: _multiPage.Title, actual: ContentType?.Modelling?.Title, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(ContentType);
                Assert.IsNotNull(ContentType.Modelling);
                Assert.AreEqual(_multiPage.Title, ContentType.Modelling.Title);
                Assert.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid);
                Assert.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count);
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
        public void Test003_Should_Fetch_Content_Type()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.ContentType(uid).Fetch()", "GET",
                    $"https://{_host}/v3/stacks/content_types/{_multiPage.Uid}");

                ContentstackResponse response = _stack.ContentType(_multiPage.Uid).Fetch();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();

                TestReportHelper.LogAssertion(ContentType?.Modelling?.Uid == _multiPage.Uid,
                    "UID matches", expected: _multiPage.Uid, actual: ContentType?.Modelling?.Uid, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(ContentType);
                Assert.IsNotNull(ContentType.Modelling);
                Assert.AreEqual(_multiPage.Title, ContentType.Modelling.Title);
                Assert.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid);
                Assert.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count);
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
        public async System.Threading.Tasks.Task Test004_Should_Fetch_Async_Content_Type()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.ContentType(uid).FetchAsync()", "GET",
                    $"https://{_host}/v3/stacks/content_types/{_singlePage.Uid}");

                ContentstackResponse response = await _stack.ContentType(_singlePage.Uid).FetchAsync();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();

                TestReportHelper.LogAssertion(ContentType?.Modelling?.Uid == _singlePage.Uid,
                    "UID matches", expected: _singlePage.Uid, actual: ContentType?.Modelling?.Uid, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(ContentType);
                Assert.IsNotNull(ContentType.Modelling);
                Assert.AreEqual(_singlePage.Title, ContentType.Modelling.Title);
                Assert.AreEqual(_singlePage.Uid, ContentType.Modelling.Uid);
                Assert.AreEqual(_singlePage.Schema.Count, ContentType.Modelling.Schema.Count);
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
        public void Test005_Should_Update_Content_Type()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                _multiPage.Schema = Contentstack.serializeArray<List<Models.Fields.Field>>(Contentstack.Client.serializer, "contentTypeSchema.json");
                TestReportHelper.LogRequest("_stack.ContentType(uid).Update()", "PUT",
                    $"https://{_host}/v3/stacks/content_types/{_multiPage.Uid}");

                ContentstackResponse response = _stack.ContentType(_multiPage.Uid).Update(_multiPage);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(ContentType);
                Assert.IsNotNull(ContentType.Modelling);
                Assert.AreEqual(_multiPage.Title, ContentType.Modelling.Title);
                Assert.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid);
                Assert.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count);
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
        public async System.Threading.Tasks.Task Test006_Should_Update_Async_Content_Type()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                _multiPage.Schema = Contentstack.serializeArray<List<Models.Fields.Field>>(Contentstack.Client.serializer, "contentTypeSchema.json");
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

                TestReportHelper.LogRequest("_stack.ContentType(uid).UpdateAsync()", "PUT",
                    $"https://{_host}/v3/stacks/content_types/{_multiPage.Uid}");

                ContentstackResponse response = await _stack.ContentType(_multiPage.Uid).UpdateAsync(_multiPage);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                if (response.IsSuccessStatusCode)
                {
                    ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
                    TestReportHelper.LogAssertion(ContentType?.Modelling?.Uid == _multiPage.Uid,
                        "UID matches", expected: _multiPage.Uid, actual: ContentType?.Modelling?.Uid, type: "AreEqual");
                    Assert.IsNotNull(response);
                    Assert.IsNotNull(ContentType);
                    Assert.IsNotNull(ContentType.Modelling);
                    Assert.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid);
                    Assert.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count);
                    Console.WriteLine($"Successfully updated content type with {ContentType.Modelling.Schema.Count} fields");
                }
                else
                {
                    Assert.Fail($"Update failed with status {response.StatusCode}: {response.OpenResponse()}");
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception during async update: {ex.Message}", type: "Fail");
                Assert.Fail($"Exception during async update: {ex.Message}");
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_Content_Type()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.ContentType().Query().Find()", "GET",
                    $"https://{_host}/v3/stacks/content_types");

                ContentstackResponse response = _stack.ContentType().Query().Find();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                ContentTypesModel ContentType = response.OpenTResponse<ContentTypesModel>();

                TestReportHelper.LogAssertion(ContentType?.Modellings?.Count == 2,
                    "Content types count is 2", expected: "2", actual: ContentType?.Modellings?.Count.ToString(), type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(ContentType);
                Assert.IsNotNull(ContentType.Modellings);
                Assert.AreEqual(2, ContentType.Modellings.Count);
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
        public async System.Threading.Tasks.Task Test008_Should_Query_Async_Content_Type()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.ContentType().Query().FindAsync()", "GET",
                    $"https://{_host}/v3/stacks/content_types");

                ContentstackResponse response = await _stack.ContentType().Query().FindAsync();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                ContentTypesModel ContentType = response.OpenTResponse<ContentTypesModel>();

                TestReportHelper.LogAssertion(ContentType?.Modellings?.Count == 2,
                    "Content types count is 2", expected: "2", actual: ContentType?.Modellings?.Count.ToString(), type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(ContentType);
                Assert.IsNotNull(ContentType.Modellings);
                Assert.AreEqual(2, ContentType.Modellings.Count);
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
