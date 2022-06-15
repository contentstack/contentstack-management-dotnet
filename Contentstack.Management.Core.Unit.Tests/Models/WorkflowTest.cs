using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class WorkflowTest
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
        public void Initialize_Workflow()
        {
            Workflow workflow = new Workflow(_stack, null);

            Assert.IsNull(workflow.Uid);
            Assert.AreEqual($"/workflows", workflow.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => workflow.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => workflow.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => workflow.Update(_fixture.Create<WorkflowModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => workflow.UpdateAsync(_fixture.Create<WorkflowModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => workflow.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => workflow.DeleteAsync());
            Assert.AreEqual(workflow.PublishRule().GetType(), typeof(PublishRule));
        }

        [TestMethod]
        public void Initialize_Workflow_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Workflow workflow = new Workflow(_stack, uid);

            Assert.AreEqual(uid, workflow.Uid);
            Assert.AreEqual($"/workflows/{workflow.Uid}", workflow.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => workflow.Create(_fixture.Create<WorkflowModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => workflow.CreateAsync(_fixture.Create<WorkflowModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => workflow.FindAll());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => workflow.FindAllAsync());
            Assert.AreEqual(workflow.PublishRule().GetType(), typeof(PublishRule));
        }

        [TestMethod]
        public void Should_Throw_On_ContentType_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _stack.Workflow().GetPublishRule(null, null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _stack.Workflow().GetPublishRuleAsync(null, null));
        }

        [TestMethod]
        public void Should_Create_Workflow()
        {
            ContentstackResponse response = _stack.Workflow().Create(new WorkflowModel());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Workflow_Async()
        {
            ContentstackResponse response = await _stack.Workflow().CreateAsync(new WorkflowModel());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Find_All_Workflow()
        {
            ContentstackResponse response = _stack.Workflow().FindAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_All_Workflow_Async()
        {
            ContentstackResponse response = await _stack.Workflow().FindAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Workflow()
        {
            ContentstackResponse response = _stack.Workflow(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Workflow_Async()
        {
            ContentstackResponse response = await _stack.Workflow(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Workflow()
        {
            ContentstackResponse response = _stack.Workflow(_fixture.Create<string>()).Update(new WorkflowModel());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Workflow_Async()
        {
            ContentstackResponse response = await _stack.Workflow(_fixture.Create<string>()).UpdateAsync(new WorkflowModel());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Workflow()
        {
            ContentstackResponse response = _stack.Workflow(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Workflow_Async()
        {
            ContentstackResponse response = await _stack.Workflow(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Disable_Workflow()
        {
            ContentstackResponse response = _stack.Workflow(_fixture.Create<string>()).Disable();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Disable_Workflow_Async()
        {
            ContentstackResponse response = await _stack.Workflow(_fixture.Create<string>()).DisableAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Enable_Workflow()
        {
            ContentstackResponse response = _stack.Workflow(_fixture.Create<string>()).Enable();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Enable_Workflow_Async()
        {
            ContentstackResponse response = await _stack.Workflow(_fixture.Create<string>()).EnableAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Get_Publish_Rule_ContentType()
        {
            ContentstackResponse response = _stack.Workflow(_fixture.Create<string>()).GetPublishRule(_fixture.Create<string>(), null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Get_Publish_Rule_ContentType_Async()
        {
            ContentstackResponse response = await _stack.Workflow(_fixture.Create<string>()).GetPublishRuleAsync(_fixture.Create<string>(), null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
