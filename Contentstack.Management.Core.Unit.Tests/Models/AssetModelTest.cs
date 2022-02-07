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

    }
}
