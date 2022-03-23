using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Models.ContentModel;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class LocaleTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;

        [TestInitialize]
        public void initialize()
        {
            var client = new ContentstackClient();
            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(_contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
        }

        [TestMethod]
        public void Initialize_Locale()
        {
            Locale locale = new Locale(_stack);

            Assert.IsNull(locale.Uid);
            Assert.ThrowsException<InvalidOperationException>(() => locale.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => locale.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => locale.Update(_fixture.Create<LocaleModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => locale.UpdateAsync(_fixture.Create<LocaleModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => locale.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => locale.DeleteAsync());
            Assert.AreEqual(locale.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Locale_With_Uid()
        {
            string code = _fixture.Create<string>();
            Locale locale = new Locale(_stack, code);

            Assert.AreEqual(code, locale.Uid);
            Assert.ThrowsException<InvalidOperationException>(() => locale.Create(_fixture.Create<LocaleModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => locale.CreateAsync(_fixture.Create<LocaleModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => locale.Query());
        }

        [TestMethod]
        public void Should_Create_Locale()
        {
            ContentstackResponse response = _stack.Locale().Create(_fixture.Create<LocaleModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Locale_Async()
        {
            ContentstackResponse response = await _stack.Locale().CreateAsync(_fixture.Create<LocaleModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Locale()
        {
            ContentstackResponse response = _stack.Locale().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Locale_Async()
        {
            ContentstackResponse response = await _stack.Locale().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Locale()
        {
            ContentstackResponse response = _stack.Locale(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Locale_Async()
        {
            ContentstackResponse response = await _stack.Locale(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Locale()
        {
            ContentstackResponse response = _stack.Locale(_fixture.Create<string>()).Update(_fixture.Create<LocaleModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Locale_Async()
        {
            ContentstackResponse response = await _stack.Locale(_fixture.Create<string>()).UpdateAsync(_fixture.Create<LocaleModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Locale()
        {
            ContentstackResponse response = _stack.Locale(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Locale_Async()
        {
            ContentstackResponse response = await _stack.Locale(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
