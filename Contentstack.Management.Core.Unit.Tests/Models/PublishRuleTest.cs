using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class PublishRuleTest
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
        public void Initialize_PublishRule()
        {
            PublishRule publishRule = new PublishRule(_stack, null);

            Assert.IsNull(publishRule.Uid);
            Assert.AreEqual($"/workflows/publishing_rules", publishRule.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => publishRule.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publishRule.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => publishRule.Update(_fixture.Create<PublishRuleModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publishRule.UpdateAsync(_fixture.Create<PublishRuleModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => publishRule.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publishRule.DeleteAsync());
        }

        [TestMethod]
        public void Initialize_PublishRule_With_Uid()
        {
            string uid = _fixture.Create<string>();
            PublishRule publishRule = new PublishRule(_stack, uid);

            Assert.AreEqual(uid, publishRule.Uid);
            Assert.AreEqual($"/workflows/publishing_rules/{publishRule.Uid}", publishRule.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => publishRule.Create(_fixture.Create<PublishRuleModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publishRule.CreateAsync(_fixture.Create<PublishRuleModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => publishRule.FindAll());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publishRule.FindAllAsync());
        }

        [TestMethod]
        public void Should_Create_PublishRule()
        {
            ContentstackResponse response = _stack.Workflow().PublishRule().Create(new PublishRuleModel());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_PublishRule_Async()
        {
            ContentstackResponse response = await _stack.Workflow().PublishRule().CreateAsync(new PublishRuleModel());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Find_All_PublishRule()
        {
            ContentstackResponse response = _stack.Workflow().PublishRule().FindAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_All_PublishRule_Async()
        {
            ContentstackResponse response = await _stack.Workflow().PublishRule().FindAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_PublishRule()
        {
            ContentstackResponse response = _stack.Workflow().PublishRule(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_PublishRule_Async()
        {
            ContentstackResponse response = await _stack.Workflow().PublishRule(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_PublishRule()
        {
            ContentstackResponse response = _stack.Workflow().PublishRule(_fixture.Create<string>()).Update(new PublishRuleModel());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_PublishRule_Async()
        {
            ContentstackResponse response = await _stack.Workflow().PublishRule(_fixture.Create<string>()).UpdateAsync(new PublishRuleModel());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_PublishRule()
        {
            ContentstackResponse response = _stack.Workflow().PublishRule(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_PublishRule_Async()
        {
            ContentstackResponse response = await _stack.Workflow().PublishRule(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
