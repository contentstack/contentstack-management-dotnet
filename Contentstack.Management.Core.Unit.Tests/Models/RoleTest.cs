using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class RoleTest
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
        public void Initialize_Role()
        {
            Role role = new Role(_stack);

            Assert.IsNull(role.Uid);
            Assert.AreEqual("/roles", role.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => role.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => role.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => role.Update(_fixture.Create<RoleModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => role.UpdateAsync(_fixture.Create<RoleModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => role.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => role.DeleteAsync());
            Assert.AreEqual(role.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Role_With_Uid()
        {
            string code = _fixture.Create<string>();
            Role role = new Role(_stack, code);

            Assert.AreEqual(code, role.Uid);
            Assert.AreEqual($"/roles/{role.Uid}", role.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => role.Create(_fixture.Create<RoleModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => role.CreateAsync(_fixture.Create<RoleModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => role.Query());
        }

        [TestMethod]
        public void Should_Create_Role()
        {
            ContentstackResponse response = _stack.Role().Create(_fixture.Create<RoleModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Role_Async()
        {
            ContentstackResponse response = await _stack.Role().CreateAsync(_fixture.Create<RoleModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Role()
        {
            ContentstackResponse response = _stack.Role().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Role_Async()
        {
            ContentstackResponse response = await _stack.Role().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Role()
        {
            ContentstackResponse response = _stack.Role(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Role_Async()
        {
            ContentstackResponse response = await _stack.Role(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Role()
        {
            ContentstackResponse response = _stack.Role(_fixture.Create<string>()).Update(_fixture.Create<RoleModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Role_Async()
        {
            ContentstackResponse response = await _stack.Role(_fixture.Create<string>()).UpdateAsync(_fixture.Create<RoleModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Role()
        {
            ContentstackResponse response = _stack.Role(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Role_Async()
        {
            ContentstackResponse response = await _stack.Role(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
