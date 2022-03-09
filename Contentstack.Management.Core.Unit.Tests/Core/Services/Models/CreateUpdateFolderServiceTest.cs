using System;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Models
{
    [TestClass]
    public class CreateUpdateFolderServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());


        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateFolderService(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()));
        }
        [TestMethod]
        public void Should_Throw_On_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateFolderService(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Name_Null()
        {
            var apiKey = _fixture.Create<string>();
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateFolderService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                null,
                _fixture.Create<string>(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Create_Content_Body()
        {
            var apiKey = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            var parentUid = _fixture.Create<string>();
            var collection = new Management.Core.Queryable.ParameterCollection();
            collection.Add(_fixture.Create<string>(), false);
            CreateUpdateFolderService service = new CreateUpdateFolderService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                name,
                null,
                parentUid);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/assets/folders", service.ResourcePath);
            Assert.AreEqual($"{{\"asset\":{{\"name\":\"{name}\",\"parent_uid\":\"{parentUid}\"}}}}", Encoding.Default.GetString(service.ByteContent));

        }
        [TestMethod]
        public void Should_Update_Content_Body()
        {
            var apiKey = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            var parentUid = _fixture.Create<string>();
            var collection = new Management.Core.Queryable.ParameterCollection();
            collection.Add(_fixture.Create<string>(), false);
            CreateUpdateFolderService service = new CreateUpdateFolderService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                name,
                uid,
                parentUid);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("PUT", service.HttpMethod);
            Assert.AreEqual($"/assets/folders/{uid}", service.ResourcePath);
            Assert.AreEqual($"{{\"asset\":{{\"name\":\"{name}\",\"parent_uid\":\"{parentUid}\"}}}}", Encoding.Default.GetString(service.ByteContent));

        }
    }
}
