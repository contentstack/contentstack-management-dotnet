using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class LabelTest
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
        public void Initialize_Label()
        {
            Label label = new Label(_stack);

            Assert.IsNull(label.Uid);
            Asset.Equals($"/labels", label.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => label.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => label.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => label.Update(_fixture.Create<LabelModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => label.UpdateAsync(_fixture.Create<LabelModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => label.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => label.DeleteAsync());
            Assert.AreEqual(label.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Label_With_Uid()
        {
            string code = _fixture.Create<string>();
            Label label = new Label(_stack, code);

            Assert.AreEqual(code, label.Uid);
            Asset.Equals($"/labels/{label.Uid}", label.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => label.Create(_fixture.Create<LabelModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => label.CreateAsync(_fixture.Create<LabelModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => label.Query());
        }

        [TestMethod]
        public void Should_Create_Label()
        {
            ContentstackResponse response = _stack.Label().Create(_fixture.Create<LabelModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Label_Async()
        {
            ContentstackResponse response = await _stack.Label().CreateAsync(_fixture.Create<LabelModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Label()
        {
            ContentstackResponse response = _stack.Label().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Label_Async()
        {
            ContentstackResponse response = await _stack.Label().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Label()
        {
            ContentstackResponse response = _stack.Label(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Label_Async()
        {
            ContentstackResponse response = await _stack.Label(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Label()
        {
            ContentstackResponse response = _stack.Label(_fixture.Create<string>()).Update(_fixture.Create<LabelModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Label_Async()
        {
            ContentstackResponse response = await _stack.Label(_fixture.Create<string>()).UpdateAsync(_fixture.Create<LabelModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Label()
        {
            ContentstackResponse response = _stack.Label(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Label_Async()
        {
            ContentstackResponse response = await _stack.Label(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
