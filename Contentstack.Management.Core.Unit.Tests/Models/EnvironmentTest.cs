using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Environment = Contentstack.Management.Core.Models.Environment;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class EnvironmentTest
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
        public void Initialize_Environment()
        {
            Environment environment = new Environment(_stack);

            Assert.IsNull(environment.Uid);
            Asset.Equals($"/environments", environment.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => environment.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => environment.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => environment.Update(_fixture.Create<EnvironmentModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => environment.UpdateAsync(_fixture.Create<EnvironmentModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => environment.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => environment.DeleteAsync());
            Assert.AreEqual(environment.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Environment_With_Uid()
        {
            string code = _fixture.Create<string>();
            Environment environment = new Environment(_stack, code);

            Assert.AreEqual(code, environment.Uid);
            Asset.Equals($"/environments/{environment.Uid}", environment.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => environment.Create(_fixture.Create<EnvironmentModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => environment.CreateAsync(_fixture.Create<EnvironmentModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => environment.Query());
        }

        [TestMethod]
        public void Should_Create_Environment()
        {
            ContentstackResponse response = _stack.Environment().Create(_fixture.Create<EnvironmentModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Environment_Async()
        {
            ContentstackResponse response = await _stack.Environment().CreateAsync(_fixture.Create<EnvironmentModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Environment()
        {
            ContentstackResponse response = _stack.Environment().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Environment_Async()
        {
            ContentstackResponse response = await _stack.Environment().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Environment()
        {
            ContentstackResponse response = _stack.Environment(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Environment_Async()
        {
            ContentstackResponse response = await _stack.Environment(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Environment()
        {
            ContentstackResponse response = _stack.Environment(_fixture.Create<string>()).Update(_fixture.Create<EnvironmentModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Environment_Async()
        {
            ContentstackResponse response = await _stack.Environment(_fixture.Create<string>()).UpdateAsync(_fixture.Create<EnvironmentModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Environment()
        {
            ContentstackResponse response = _stack.Environment(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Environment_Async()
        {
            ContentstackResponse response = await _stack.Environment(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
