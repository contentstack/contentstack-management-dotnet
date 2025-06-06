using System;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Token;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class DeliveryTokenTest
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
        public void Initialize_DeliveryToken()
        {
            DeliveryToken token = new DeliveryToken(_stack);
            Assert.IsNull(token.Uid);
            Assert.AreEqual("stacks/delivery_tokens", token.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => token.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => token.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => token.Update(_fixture.Create<DeliveryTokenModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => token.UpdateAsync(_fixture.Create<DeliveryTokenModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => token.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => token.DeleteAsync());
            Assert.AreEqual(token.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_DeliveryToken_With_Uid()
        {
            string uid = _fixture.Create<string>();
            DeliveryToken token = new DeliveryToken(_stack, uid);
            Assert.AreEqual(uid, token.Uid);
            Assert.AreEqual($"stacks/delivery_tokens/{uid}", token.resourcePath);
        }

        [TestMethod]
        public void Should_Create_DeliveryToken()
        {
            ContentstackResponse response = _stack.DeliveryToken().Create(_fixture.Create<DeliveryTokenModel>());
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Create_DeliveryToken_Async()
        {
            ContentstackResponse response = await _stack.DeliveryToken().CreateAsync(_fixture.Create<DeliveryTokenModel>());
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_DeliveryToken()
        {
            ContentstackResponse response = _stack.DeliveryToken().Query().Find();
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Query_DeliveryToken_Async()
        {
            ContentstackResponse response = await _stack.DeliveryToken().Query().FindAsync();
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_DeliveryToken()
        {
            ContentstackResponse response = _stack.DeliveryToken(_fixture.Create<string>()).Fetch();
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Fetch_DeliveryToken_Async()
        {
            ContentstackResponse response = await _stack.DeliveryToken(_fixture.Create<string>()).FetchAsync();
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_DeliveryToken()
        {
            ContentstackResponse response = _stack.DeliveryToken(_fixture.Create<string>()).Update(_fixture.Create<DeliveryTokenModel>());
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Update_DeliveryToken_Async()
        {
            ContentstackResponse response = await _stack.DeliveryToken(_fixture.Create<string>()).UpdateAsync(_fixture.Create<DeliveryTokenModel>());
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_DeliveryToken()
        {
            ContentstackResponse response = _stack.DeliveryToken(_fixture.Create<string>()).Delete();
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Delete_DeliveryToken_Async()
        {
            ContentstackResponse response = await _stack.DeliveryToken(_fixture.Create<string>()).DeleteAsync();
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
