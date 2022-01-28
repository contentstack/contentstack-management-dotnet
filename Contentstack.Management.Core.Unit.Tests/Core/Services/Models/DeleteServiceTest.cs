using System;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.Models;
using Contentstack.Management.Core.Unit.Tests.Models.ContentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Models
{
    [TestClass]
    public class DeleteServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());


        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DeleteService<EntryModel>(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<EntryModel>()));
        }

        [TestMethod]
        public void Should_Throw_On_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DeleteService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<EntryModel>()));
        }

        [TestMethod]
        public void Should_Throw_On_Resource_Path_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DeleteService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null, _fixture.Create<string>()),
                null,
                _fixture.Create<string>(),
                _fixture.Create<EntryModel>()));
        }

        [TestMethod]
        public void Should_Throw_On_Field_Name()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DeleteService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null, _fixture.Create<string>()),
                _fixture.Create<string>(),
                null,
                _fixture.Create<EntryModel>()));
        }

        [TestMethod]
        public void Should_Throw_On_Resource_Model()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DeleteService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null, _fixture.Create<string>()),
                 _fixture.Create<string>(),
                _fixture.Create<string>(),
                null));
        }

        [TestMethod]
        public void Should_Create_Content_Body()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var fieldName = _fixture.Create<string>();
            var collection = new Management.Core.Queryable.ParameterCollection();

            collection.Add(_fixture.Create<string>(), false);
            DeleteService<EntryModel> service = new DeleteService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                resourcePath,
                fieldName,
                _fixture.Create<EntryModel>());
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("DELETE", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual($"{{\"{fieldName}\": {{\"title\":\"{service.model.Title}\"}}}}", Encoding.Default.GetString(service.Content));
        }
    }
}
