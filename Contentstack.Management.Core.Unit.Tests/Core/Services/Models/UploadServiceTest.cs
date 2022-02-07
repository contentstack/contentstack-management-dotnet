using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Models
{
    [TestClass]
    public class UploadServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());
        private AssetModel _assetModel;

        [TestInitialize]
        public void initialize()
        {
            _assetModel = new AssetModel(_fixture.Create<string>(), "../../../../README.md", "application/text");
        }

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UploadService(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _assetModel));
        }
        [TestMethod]
        public void Should_Throw_On_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UploadService(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _assetModel));
        }

        [TestMethod]
        public void Should_Throw_On_Resource_Path_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new UploadService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                null,
                _assetModel));
        }

        [TestMethod]
        public void Should_Throw_On_Data_Model_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new UploadService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                _fixture.Create<string>(),
                null));
        }

        [TestMethod]
        public void Should_Create_Content_Body()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();

            UploadService service = new UploadService(serializer, new Management.Core.Models.Stack(null, apiKey), resourcePath, _assetModel);

            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            //Assert.AreEqual($"{{\"{fieldName}\": {{}}}}", Encoding.Default.GetString(service.ByteContent));
        }
    }
}
