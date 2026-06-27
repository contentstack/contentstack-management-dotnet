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
            Assert.AreEqual($"/assets", Asset.resourcePath);
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
            Assert.ThrowsException<InvalidOperationException>(() => Asset.References());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Asset.ReferencesAsync());
            Assert.AreEqual(Asset.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Asset_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Asset Asset = new Asset(_stack, uid);

            Assert.AreEqual(uid, Asset.Uid);
            Assert.AreEqual($"/assets/{Asset.Uid}", Asset.resourcePath);
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
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset().CreateAsync(_assetModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Query_Asset()
        {
            ParameterCollection collection = new ParameterCollection();

            collection.Add("limit", 10);
            ContentstackResponse response = _stack.Asset().Query().Find(collection);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Fetch_Asset()
        {
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Update_Asset()
        {

            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Update(_assetModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).UpdateAsync(_assetModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Delete_Asset()
        {
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Publish_Asset()
        {
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Publish(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Publish_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).PublishAsync(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }
        [TestMethod]
        public void Should_Unpublish_Asset()
        {
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Unpublish(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Unpublish_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).UnpublishAsync(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Get_References_Asset()
        {
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).References();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Get_References_Asset_Async()
        {
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).ReferencesAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        // Image format upload tests

        [TestMethod]
        public void Should_Create_JPG_Image_Asset()
        {
            var jpgModel = new AssetModel("london.jpg", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, "image/jpeg",
                title: "London", description: "JPG image", parentUID: null, tags: "image,jpeg");

            ContentstackResponse response = _stack.Asset().Create(jpgModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Create_JPEG_Extension_Image_Asset()
        {
            var jpegModel = new AssetModel("tokyo.jpeg", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, "image/jpeg",
                title: "Tokyo", description: "JPEG image", parentUID: null, tags: "image,jpeg");

            ContentstackResponse response = _stack.Asset().Create(jpegModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Create_AVIF_Image_Asset()
        {
            var avifModel = new AssetModel("dubai.avif", new byte[] { 0x00, 0x00, 0x00, 0x1C, 0x66, 0x74, 0x79, 0x70 }, "image/avif",
                title: "Dubai", description: "AVIF image", parentUID: null, tags: "image,avif");

            ContentstackResponse response = _stack.Asset().Create(avifModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_JPG_Image_Asset_Async()
        {
            var jpgModel = new AssetModel("london.jpg", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, "image/jpeg",
                title: "London Async", description: "JPG image async", parentUID: null, tags: "image,jpeg,async");

            ContentstackResponse response = await _stack.Asset().CreateAsync(jpgModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_AVIF_Image_Asset_Async()
        {
            var avifModel = new AssetModel("dubai.avif", new byte[] { 0x00, 0x00, 0x00, 0x1C, 0x66, 0x74, 0x79, 0x70 }, "image/avif",
                title: "Dubai Async", description: "AVIF image async", parentUID: null, tags: "image,avif,async");

            ContentstackResponse response = await _stack.Asset().CreateAsync(avifModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Update_Image_Asset_With_JPG()
        {
            var jpgModel = new AssetModel("london_updated.jpg", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, "image/jpeg",
                title: "London Updated", description: "Updated JPG", parentUID: null, tags: "image,jpeg,updated");

            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Update(jpgModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Image_Asset_With_JPG_Async()
        {
            var jpgModel = new AssetModel("tokyo_updated.jpeg", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, "image/jpeg",
                title: "Tokyo Updated Async", description: "Updated JPEG async", parentUID: null, tags: "image,jpeg,async,updated");

            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).UpdateAsync(jpgModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        // ── Asset Scanning Tests ──────────────────────────────────────────────────────
        // These tests verify that the SDK accepts include_asset_scan_status param and api_version header
        // without throwing. MockHttpHandler returns a fixed response for all requests so we only
        // assert that the SDK wires up the call and returns the expected response object.

        [TestMethod]
        public void Should_Fetch_Asset_With_ScanStatus_Param()
        {
            // Example: asset.add_param("include_asset_scan_status", true) → asset.fetch()
            var collection = new ParameterCollection();
            collection.Add("include_asset_scan_status", true);

            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Fetch(collection);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_Asset_With_ScanStatus_Param_Async()
        {
            // Example: asset.add_param("include_asset_scan_status", true) → await asset.fetch_async()
            var collection = new ParameterCollection();
            collection.Add("include_asset_scan_status", true);

            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>()).FetchAsync(collection);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Create_Asset_With_ScanStatus_Param()
        {
            // Example: asset.add_param("include_asset_scan_status", true) → asset.upload(file)
            var collection = new ParameterCollection();
            collection.Add("include_asset_scan_status", true);

            ContentstackResponse response = _stack.Asset().Create(_assetModel, collection);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Asset_With_ScanStatus_Param_Async()
        {
            // Example: asset.add_param("include_asset_scan_status", true) → await asset.upload_async(file)
            var collection = new ParameterCollection();
            collection.Add("include_asset_scan_status", true);

            ContentstackResponse response = await _stack.Asset().CreateAsync(_assetModel, collection);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Update_Asset_With_ScanStatus_Param()
        {
            // Example: asset.add_param("include_asset_scan_status", true) → asset.update(model)
            var collection = new ParameterCollection();
            collection.Add("include_asset_scan_status", true);

            ContentstackResponse response = _stack.Asset(_fixture.Create<string>()).Update(_assetModel, collection);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Publish_Asset_With_ApiVersion_Header()
        {
            // Example: asset.add_header("api_version", "3.2") → asset.publish(details)
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>())
                .Publish(_fixture.Create<PublishUnpublishDetails>(), "3.2");

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Publish_Asset_With_ApiVersion_Header_Async()
        {
            // Example: asset.add_header("api_version", "3.2") → await asset.publish_async(details)
            ContentstackResponse response = await _stack.Asset(_fixture.Create<string>())
                .PublishAsync(_fixture.Create<PublishUnpublishDetails>(), "3.2");

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }

        [TestMethod]
        public void Should_Publish_Asset_Without_ApiVersion_Header()
        {
            // Example: asset.publish(details)  — no api_version header, default SDK behavior
            ContentstackResponse response = _stack.Asset(_fixture.Create<string>())
                .Publish(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToJsonString(), response.OpenJsonObjectResponse().ToJsonString());
        }
    }
}
