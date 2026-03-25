using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
