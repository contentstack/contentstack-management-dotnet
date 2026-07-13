using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class AssetModelTest
    {
        private readonly IFixture _fixture = new Fixture()
           .Customize(new AutoMoqCustomization());

        private static readonly byte[] _jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10 };
        private static readonly byte[] _avifBytes = new byte[] { 0x00, 0x00, 0x00, 0x1C, 0x66, 0x74, 0x79, 0x70 };

        [TestMethod]
        public void Should_Throw_On_Filename_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new AssetModel(null, "../../../../README.md", ""));
        }

        [TestMethod]
        public void Should_Throw_On_Filepath_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new AssetModel("title", byteArray: null, ""));
        }

        [TestMethod]
        public void Should_Throw_On_Bytes_Null_For_Image()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new AssetModel("london.jpg", byteArray: null, "image/jpeg"));
        }

        [TestMethod]
        public void Should_Return_AssetModel()
        {
            var fileName = _fixture.Create<string>();

            AssetModel assetModel = new AssetModel(fileName, "../../../../README.md", "application/text");

            Assert.AreEqual(fileName, assetModel.FileName);
            Assert.IsNotNull(assetModel.byteArray);
            Assert.AreEqual("application/text", assetModel.ContentType);
        }

        [TestMethod]
        public async Task Should_Return_AssetModel_Multipart()
        {
            var title = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            var parentUid = _fixture.Create<string>();
            var tags = _fixture.Create<string>();
            AssetModel assetModel = new AssetModel("title", "../../../../README.md", "application/text", title, description, parentUid, tags);

            var content = assetModel.GetHttpContent();
            var stringContent = await content.ReadAsStringAsync();

            Assert.IsTrue(stringContent.Contains(title));
            Assert.IsTrue(stringContent.Contains(description));
            Assert.IsTrue(stringContent.Contains(parentUid));
            Assert.IsTrue(stringContent.Contains(tags));
        }

        // Image format tests

        [TestMethod]
        public void Should_Create_AssetModel_With_JPEG_ContentType()
        {
            AssetModel assetModel = new AssetModel("london.jpg", _jpegBytes, "image/jpeg");

            Assert.AreEqual("london.jpg", assetModel.FileName);
            Assert.IsNotNull(assetModel.byteArray);
            Assert.AreEqual("image/jpeg", assetModel.ContentType);
        }

        [TestMethod]
        public void Should_Create_AssetModel_With_JPEG_Extension_ContentType()
        {
            AssetModel assetModel = new AssetModel("tokyo.jpeg", _jpegBytes, "image/jpeg");

            Assert.AreEqual("tokyo.jpeg", assetModel.FileName);
            Assert.IsNotNull(assetModel.byteArray);
            Assert.AreEqual("image/jpeg", assetModel.ContentType);
        }

        [TestMethod]
        public void Should_Create_AssetModel_With_AVIF_ContentType()
        {
            AssetModel assetModel = new AssetModel("dubai.avif", _avifBytes, "image/avif");

            Assert.AreEqual("dubai.avif", assetModel.FileName);
            Assert.IsNotNull(assetModel.byteArray);
            Assert.AreEqual("image/avif", assetModel.ContentType);
        }

        [TestMethod]
        public void Should_Preserve_JPG_Filename_Extension()
        {
            AssetModel assetModel = new AssetModel("london.jpg", _jpegBytes, "image/jpeg");

            Assert.IsTrue(assetModel.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void Should_Preserve_JPEG_Filename_Extension()
        {
            AssetModel assetModel = new AssetModel("tokyo.jpeg", _jpegBytes, "image/jpeg");

            Assert.IsTrue(assetModel.FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void Should_Preserve_AVIF_Filename_Extension()
        {
            AssetModel assetModel = new AssetModel("dubai.avif", _avifBytes, "image/avif");

            Assert.IsTrue(assetModel.FileName.EndsWith(".avif", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public async Task Should_Return_AssetModel_Multipart_Contains_JPEG_Filename()
        {
            var title = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            var tags = _fixture.Create<string>();
            AssetModel assetModel = new AssetModel("london.jpg", _jpegBytes, "image/jpeg", title, description, null, tags);

            var content = assetModel.GetHttpContent();
            var stringContent = await content.ReadAsStringAsync();

            Assert.IsTrue(stringContent.Contains("london.jpg"));
            Assert.IsTrue(stringContent.Contains(title));
            Assert.IsTrue(stringContent.Contains(description));
            Assert.IsTrue(stringContent.Contains(tags));
        }

        [TestMethod]
        public async Task Should_Return_AssetModel_Multipart_Contains_AVIF_Filename()
        {
            var title = _fixture.Create<string>();
            var tags = _fixture.Create<string>();
            AssetModel assetModel = new AssetModel("dubai.avif", _avifBytes, "image/avif", title, null, null, tags);

            var content = assetModel.GetHttpContent();
            var stringContent = await content.ReadAsStringAsync();

            Assert.IsTrue(stringContent.Contains("dubai.avif"));
            Assert.IsTrue(stringContent.Contains(title));
            Assert.IsTrue(stringContent.Contains(tags));
        }

        [TestMethod]
        public async Task Should_Return_AssetModel_Multipart_With_ParentUID_For_Image()
        {
            var parentUid = _fixture.Create<string>();
            AssetModel assetModel = new AssetModel("london.jpg", _jpegBytes, "image/jpeg", parentUID: parentUid);

            var content = assetModel.GetHttpContent();
            var stringContent = await content.ReadAsStringAsync();

            Assert.IsTrue(stringContent.Contains(parentUid));
        }

    }
}
