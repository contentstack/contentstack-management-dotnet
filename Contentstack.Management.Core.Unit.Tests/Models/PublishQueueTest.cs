using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class PublishQueueTest
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
        public void Initialize_PublishQueue()
        {
            PublishQueue publishQueue = new PublishQueue(_stack, null);

            Assert.IsNull(publishQueue.Uid);
            Assert.AreEqual($"/publish-queue", publishQueue.resourcePath);

            Assert.ThrowsException<InvalidOperationException>(() => publishQueue.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publishQueue.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => publishQueue.Cancel());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publishQueue.CancelAsync());
        }

        [TestMethod]
        public void Initialize_PublishQueue_With_Uid()
        {
            string uid = _fixture.Create<string>();
            PublishQueue publishQueue = new PublishQueue(_stack, uid);

            Assert.AreEqual(uid, publishQueue.Uid);
            Assert.AreEqual($"/publish-queue/{publishQueue.Uid}", publishQueue.resourcePath);

            Assert.ThrowsException<InvalidOperationException>(() => publishQueue.FindAll());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publishQueue.FindAllAsync());
        }

        [TestMethod]
        public void Should_Fetch_PublishQueue()
        {
            ContentstackResponse response = _stack.PublishQueue(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_PublishQueue_Async()
        {
            ContentstackResponse response = await _stack.PublishQueue(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Find_PublishQueue()
        {
            ContentstackResponse response = _stack.PublishQueue().FindAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_PublishQueue_Async()
        {
            ContentstackResponse response = await _stack.PublishQueue().FindAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Cancel_PublishQueue()
        {
            ContentstackResponse response = _stack.PublishQueue(_fixture.Create<string>()).Cancel();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Cancel_PublishQueue_Async()
        {
            ContentstackResponse response = await _stack.PublishQueue(_fixture.Create<string>()).CancelAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
