using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class WebhookTest
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
        public void Initialize_Webhook()
        {
            Webhook webhook = new Webhook(_stack);

            Assert.IsNull(webhook.Uid);
            Assert.AreEqual($"/webhooks", webhook.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => webhook.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => webhook.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => webhook.Update(_fixture.Create<WebhookModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => webhook.UpdateAsync(_fixture.Create<WebhookModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => webhook.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => webhook.DeleteAsync());
            Assert.ThrowsException<InvalidOperationException>(() => webhook.Executions());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => webhook.ExecutionsAsync());
            Assert.ThrowsException<InvalidOperationException>(() => webhook.Retry(_fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => webhook.RetryAsync(_fixture.Create<string>()));
            Assert.ThrowsException<InvalidOperationException>(() => webhook.Logs(_fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => webhook.LogsAsync(_fixture.Create<string>()));
            Assert.AreEqual(webhook.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Webhook_With_Uid()
        {
            string code = _fixture.Create<string>();
            Webhook webhook = new Webhook(_stack, code);

            Assert.AreEqual(code, webhook.Uid);
            Assert.AreEqual($"/webhooks/{webhook.Uid}", webhook.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => webhook.Create(_fixture.Create<WebhookModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => webhook.CreateAsync(_fixture.Create<WebhookModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => webhook.Query());
        }

        [TestMethod]
        public void Should_Create_Webhook()
        {
            ContentstackResponse response = _stack.Webhook().Create(_fixture.Create<WebhookModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Webhook_Async()
        {
            ContentstackResponse response = await _stack.Webhook().CreateAsync(_fixture.Create<WebhookModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Webhook()
        {
            ContentstackResponse response = _stack.Webhook().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Webhook_Async()
        {
            ContentstackResponse response = await _stack.Webhook().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Webhook()
        {
            ContentstackResponse response = _stack.Webhook(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Webhook_Async()
        {
            ContentstackResponse response = await _stack.Webhook(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Webhook()
        {
            ContentstackResponse response = _stack.Webhook(_fixture.Create<string>()).Update(_fixture.Create<WebhookModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Webhook_Async()
        {
            ContentstackResponse response = await _stack.Webhook(_fixture.Create<string>()).UpdateAsync(_fixture.Create<WebhookModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Webhook()
        {
            ContentstackResponse response = _stack.Webhook(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Webhook_Async()
        {
            ContentstackResponse response = await _stack.Webhook(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Executions_Webhook()
        {
            ContentstackResponse response = _stack.Webhook(_fixture.Create<string>()).Executions();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Executions_Webhook_Async()
        {
            ContentstackResponse response = await _stack.Webhook(_fixture.Create<string>()).ExecutionsAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Retry_Webhook()
        {
            ContentstackResponse response = _stack.Webhook(_fixture.Create<string>()).Retry(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Retry_Webhook_Async()
        {
            ContentstackResponse response = await _stack.Webhook(_fixture.Create<string>()).RetryAsync(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Logs_Webhook()
        {
            ContentstackResponse response = _stack.Webhook(_fixture.Create<string>()).Logs(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Logs_Webhook_Async()
        {
            ContentstackResponse response = await _stack.Webhook(_fixture.Create<string>()).LogsAsync(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
