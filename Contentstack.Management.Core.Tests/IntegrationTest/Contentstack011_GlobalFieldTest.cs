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
    public class Contentstack004_GlobalFieldTest
    {
        private Stack _stack;
        private ContentModelling _modelling;
        [TestInitialize]
        public void Initialize ()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
            _modelling = Contentstack.serialize<ContentModelling>(Contentstack.Client.serializer, "globalfield.json");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Global_Field()
        {
            ContentstackResponse response = _stack.GlobalField().Create(_modelling);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
            Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
            Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Fetch_Global_Field()
        {
            ContentstackResponse response = _stack.GlobalField(_modelling.Uid).Fetch();
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
            Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
            Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Fetch_Async_Global_Field()
        {
            ContentstackResponse response = await _stack.GlobalField(_modelling.Uid).FetchAsync();
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
            Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
            Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Global_Field()
        {
            _modelling.Title = "Updated title";
            ContentstackResponse response = _stack.GlobalField(_modelling.Uid).Update(_modelling);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
            Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
            Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Update_Async_Global_Field()
        {
            _modelling.Title = "First Async";
            ContentstackResponse response = await _stack.GlobalField(_modelling.Uid).UpdateAsync(_modelling);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual(_modelling.Title, globalField.Modelling.Title);
            Assert.AreEqual(_modelling.Uid, globalField.Modelling.Uid);
            Assert.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Query_Global_Field()
        {
            ContentstackResponse response = _stack.GlobalField().Query().Find();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modellings);
            Assert.AreEqual(1, globalField.Modellings.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Update_Async_Global_Field()
        {
            ContentstackResponse response = await _stack.GlobalField().Query().FindAsync();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modellings);
            Assert.AreEqual(1, globalField.Modellings.Count);
        }
    }
}
