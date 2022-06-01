using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class ReleaseTest
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
        public void Initialize_Release()
        {
            Release release = new Release(_stack);

            Assert.IsNull(release.Uid);
            Asset.Equals("/releases", release.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => release.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => release.Update(_fixture.Create<ReleaseModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.UpdateAsync(_fixture.Create<ReleaseModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => release.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.DeleteAsync());
            Assert.ThrowsException<InvalidOperationException>(() => release.Deploy(_fixture.Create<DeployModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.DeployAsync(_fixture.Create<DeployModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => release.Clone(_fixture.Create<string>(), _fixture.Create<string>())); 
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.CloneAsync(_fixture.Create<string>(), _fixture.Create<string>()));
            Assert.AreEqual(release.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Release_With_Uid()
        {
            string code = _fixture.Create<string>();
            Release release = new Release(_stack, code);

            Assert.AreEqual(code, release.Uid);
            Asset.Equals($"/releases/{release.Uid}", release.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => release.Create(_fixture.Create<ReleaseModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.CreateAsync(_fixture.Create<ReleaseModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => release.Query());
            Assert.AreEqual(release.Item().GetType(), typeof(ReleaseItem));
        }

        [TestMethod]
        public void Initialize_Release_Clone_NAME_Null()
        {
            string code = _fixture.Create<string>();
            Release release = new Release(_stack, code);
            Assert.ThrowsException<ArgumentNullException>(() => release.Clone(null, _fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => release.CloneAsync(_fixture.Create<string>(), _fixture.Create<string>()));
        }


        [TestMethod]
        public void Should_Create_Release()
        {
            ContentstackResponse response = _stack.Release().Create(_fixture.Create<ReleaseModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Release_Async()
        {
            ContentstackResponse response = await _stack.Release().CreateAsync(_fixture.Create<ReleaseModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Release()
        {
            ContentstackResponse response = _stack.Release().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Release_Async()
        {
            ContentstackResponse response = await _stack.Release().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Release()
        {
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Release()
        {
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Update(_fixture.Create<ReleaseModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).UpdateAsync(_fixture.Create<ReleaseModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Release()
        {
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Initialize_Release_Clone()
        {
            string code = _fixture.Create<string>();
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Clone(code, null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Initialize_Release_Clone_Async()
        {
            string code = _fixture.Create<string>();
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).CloneAsync(code, null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
