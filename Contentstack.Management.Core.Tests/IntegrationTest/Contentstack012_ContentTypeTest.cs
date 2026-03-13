using System;
using System.Collections.Generic;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
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

        [TestInitialize]
        public void Initialize ()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
            _singlePage = Contentstack.serialize<ContentModelling>(Contentstack.Client.serializer, "singlepageCT.json");
            _multiPage = Contentstack.serialize<ContentModelling>(Contentstack.Client.serializer, "multiPageCT.json");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_SinglePage");
            ContentstackResponse response = _stack.ContentType().Create(_singlePage);
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            TestOutputLogger.LogContext("ContentType", _singlePage.Uid);
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modelling, "ContentType.Modelling");
            AssertLogger.AreEqual(_singlePage.Title, ContentType.Modelling.Title, "Title");
            AssertLogger.AreEqual(_singlePage.Uid, ContentType.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_singlePage.Schema.Count, ContentType.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Create_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateContentType_MultiPage");
            ContentstackResponse response = _stack.ContentType().Create(_multiPage);
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            TestOutputLogger.LogContext("ContentType", _multiPage.Uid);
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modelling, "ContentType.Modelling");
            AssertLogger.AreEqual(_multiPage.Title, ContentType.Modelling.Title, "Title");
            AssertLogger.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count, "SchemaCount");
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
            AssertLogger.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count, "SchemaCount");
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
            AssertLogger.AreEqual(_singlePage.Schema.Count, ContentType.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_Content_Type()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateContentType");
            TestOutputLogger.LogContext("ContentType", _multiPage.Uid);
            _multiPage.Schema = Contentstack.serializeArray<List<Models.Fields.Field>>(Contentstack.Client.serializer, "contentTypeSchema.json"); ;
            ContentstackResponse response = _stack.ContentType(_multiPage.Uid).Update(_multiPage);
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(ContentType, "ContentType");
            AssertLogger.IsNotNull(ContentType.Modelling, "ContentType.Modelling");
            AssertLogger.AreEqual(_multiPage.Title, ContentType.Modelling.Title, "Title");
            AssertLogger.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count, "SchemaCount");
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
                _multiPage.Schema = Contentstack.serializeArray<List<Models.Fields.Field>>(Contentstack.Client.serializer, "contentTypeSchema.json");
                
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
                    AssertLogger.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count, "SchemaCount");
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
            AssertLogger.AreEqual(2, ContentType.Modellings.Count, "ModellingsCount");
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
            AssertLogger.AreEqual(2, ContentType.Modellings.Count, "ModellingsCount");
        }
    }
}
