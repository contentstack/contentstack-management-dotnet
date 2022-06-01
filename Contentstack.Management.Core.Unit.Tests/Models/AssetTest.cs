using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class AssetTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;
        private AssetModel _assetModel = new AssetModel("name", "../../../../README.md", "application/text");

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
        public void Initialize_Asset()
        {
            Asset Asset = new Asset(_stack, null);

            Assert.IsNull(Asset.Uid);
            Asset.Equals($"/assets", Asset.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => Asset.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Asset.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => Asset.Update(_assetModel));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Asset.UpdateAsync(_assetModel));
            Assert.ThrowsException<InvalidOperationException>(() => Asset.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Asset.DeleteAsync());
            Assert.ThrowsException<InvalidOperationException>(() => Asset.Publish(_fixture.Create<PublishUnpublishDetails>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Asset.PublishAsync(_fixture.Create<PublishUnpublishDetails>()));
            Assert.ThrowsException<InvalidOperationException>(() => Asset.Unpublish(_fixture.Create<PublishUnpublishDetails>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Asset.UnpublishAsync(_fixture.Create<PublishUnpublishDetails>()));
            Assert.AreEqual(Asset.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Asset_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Asset Asset = new Asset(_stack, uid);

            Assert.AreEqual(uid, Asset.Uid);
            Asset.Equals($"/assets/{Asset.Uid}", Asset.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => Asset.Create(_assetModel));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Asset.CreateAsync(_assetModel));
            Assert.ThrowsException<InvalidOperationException>(() => Asset.Query());
            Assert.ThrowsException<InvalidOperationException>(() => Asset.Folder());
        }

        [TestMethod]
        public void Should_Create_Asset()
        {
            ContentstackResponse response = _stack.Asset().Create(_assetModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset().CreateAsync(_assetModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Asset()
        {
            ContentstackResponse response = _stack.Asset().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Asset()
        {
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Asset()
        {

            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Update(_assetModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).UpdateAsync(_assetModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Asset()
        {
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Publish_Asset()
        {
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Publish(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Publish_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).PublishAsync(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Unpublish_Asset()
        {
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Unpublish(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Unpublish_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).UnpublishAsync(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
