using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture;
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
            ContentstackResponse response = _stack.ContentType().Create(_singlePage);
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(ContentType);
            Assert.IsNotNull(ContentType.Modelling);
            Assert.AreEqual(_singlePage.Title, ContentType.Modelling.Title);
            Assert.AreEqual(_singlePage.Uid, ContentType.Modelling.Uid);
            Assert.AreEqual(_singlePage.Schema.Count, ContentType.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Create_Content_Type()
        {
            ContentstackResponse response = _stack.ContentType().Create(_multiPage);
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(ContentType);
            Assert.IsNotNull(ContentType.Modelling);
            Assert.AreEqual(_multiPage.Title, ContentType.Modelling.Title);
            Assert.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid);
            Assert.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_Content_Type()
        {
            ContentstackResponse response = _stack.ContentType(_multiPage.Uid).Fetch();
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(ContentType);
            Assert.IsNotNull(ContentType.Modelling);
            Assert.AreEqual(_multiPage.Title, ContentType.Modelling.Title);
            Assert.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid);
            Assert.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test004_Should_Fetch_Async_Content_Type()
        {
            ContentstackResponse response = await _stack.ContentType(_singlePage.Uid).FetchAsync();
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(ContentType);
            Assert.IsNotNull(ContentType.Modelling);
            Assert.AreEqual(_singlePage.Title, ContentType.Modelling.Title);
            Assert.AreEqual(_singlePage.Uid, ContentType.Modelling.Uid);
            Assert.AreEqual(_singlePage.Schema.Count, ContentType.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_Content_Type()
        {
            _multiPage.Schema = Contentstack.serializeArray<List<Models.Fields.Field>>(Contentstack.Client.serializer, "contentTypeSchema.json"); ;
            ContentstackResponse response = _stack.ContentType(_multiPage.Uid).Update(_multiPage);
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(ContentType);
            Assert.IsNotNull(ContentType.Modelling);
            Assert.AreEqual(_multiPage.Title, ContentType.Modelling.Title);
            Assert.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid);
            Assert.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test006_Should_Update_Async_Content_Type()
        {
            _multiPage.Title = "First Async";
            ContentstackResponse response = await _stack.ContentType(_multiPage.Uid).UpdateAsync(_multiPage);
            ContentTypeModel ContentType = response.OpenTResponse<ContentTypeModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(ContentType);
            Assert.IsNotNull(ContentType.Modelling);
            Assert.AreEqual(_multiPage.Title, ContentType.Modelling.Title);
            Assert.AreEqual(_multiPage.Uid, ContentType.Modelling.Uid);
            Assert.AreEqual(_multiPage.Schema.Count, ContentType.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_Content_Type()
        {
            ContentstackResponse response = _stack.ContentType().Query().Find();
            ContentTypesModel ContentType = response.OpenTResponse<ContentTypesModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(ContentType);
            Assert.IsNotNull(ContentType.Modellings);
            Assert.AreEqual(2, ContentType.Modellings.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test008_Should_Update_Async_Content_Type()
        {
            ContentstackResponse response = await _stack.ContentType().Query().FindAsync();
            ContentTypesModel ContentType = response.OpenTResponse<ContentTypesModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(ContentType);
            Assert.IsNotNull(ContentType.Modellings);
            Assert.AreEqual(2, ContentType.Modellings.Count);
        }
    }
}
