using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
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
            TestOutputLogger.LogContext("TestScenario", "CreateGlobalField");
            ContentstackResponse response = _stack.GlobalField().Create(_modelling);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Fetch_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchGlobalField");
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            ContentstackResponse response = _stack.GlobalField(_modelling.Uid).Fetch();
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Fetch_Async_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAsyncGlobalField");
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            ContentstackResponse response = await _stack.GlobalField(_modelling.Uid).FetchAsync();
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateGlobalField");
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            _modelling.Title = "Updated title";
            ContentstackResponse response = _stack.GlobalField(_modelling.Uid).Update(_modelling);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Update_Async_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateAsyncGlobalField");
            TestOutputLogger.LogContext("GlobalField", _modelling.Uid);
            _modelling.Title = "First Async";
            ContentstackResponse response = await _stack.GlobalField(_modelling.Uid).UpdateAsync(_modelling);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modelling, "globalField.Modelling");
            AssertLogger.AreEqual(_modelling.Title, globalField.Modelling.Title, "Title");
            AssertLogger.AreEqual(_modelling.Uid, globalField.Modelling.Uid, "Uid");
            AssertLogger.AreEqual(_modelling.Schema.Count, globalField.Modelling.Schema.Count, "SchemaCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Query_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryGlobalField");
            ContentstackResponse response = _stack.GlobalField().Query().Find();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modellings, "globalField.Modellings");
            AssertLogger.AreEqual(1, globalField.Modellings.Count, "ModellingsCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006a_Should_Query_Global_Field_With_ApiVersion()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryGlobalFieldWithApiVersion");
            ContentstackResponse response = _stack.GlobalField(apiVersion: "3.2").Query().Find();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modellings, "globalField.Modellings");
            AssertLogger.AreEqual(1, globalField.Modellings.Count, "ModellingsCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Query_Async_Global_Field()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryAsyncGlobalField");
            ContentstackResponse response = await _stack.GlobalField().Query().FindAsync();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modellings, "globalField.Modellings");
            AssertLogger.AreEqual(1, globalField.Modellings.Count, "ModellingsCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007a_Should_Query_Async_Global_Field_With_ApiVersion()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryAsyncGlobalFieldWithApiVersion");
            ContentstackResponse response = await _stack.GlobalField(apiVersion: "3.2").Query().FindAsync();
            GlobalFieldsModel globalField = response.OpenTResponse<GlobalFieldsModel>();
            AssertLogger.IsNotNull(response, "response");
            AssertLogger.IsNotNull(globalField, "globalField");
            AssertLogger.IsNotNull(globalField.Modellings, "globalField.Modellings");
            AssertLogger.AreEqual(1, globalField.Modellings.Count, "ModellingsCount");
        }
    }
}
