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
    public class LocalizationServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());


        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LocalizationService<EntryModel>(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _fixture.Create <EntryModel>(),
                _fixture.Create<string>()));
        }
        [TestMethod]
        public void Should_Throw_On_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LocalizationService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _fixture.Create <EntryModel>(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Resource_Path_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new LocalizationService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                null,
                _fixture.Create <EntryModel>(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Data_Model_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new LocalizationService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                _fixture.Create<string>(),
                null,
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_FieldName_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new LocalizationService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                _fixture.Create<string>(),
                _fixture.Create <EntryModel>(),
                null));
        }

        [TestMethod]
        public void Should_Create_Content_Body()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var fieldName = _fixture.Create<string>();
            var model = _fixture.Create<EntryModel>();
            var collection = new Management.Core.Queryable.ParameterCollection();
            collection.Add(_fixture.Create<string>(), false);
            LocalizationService<EntryModel> service = new LocalizationService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                resourcePath,
                model,
                fieldName,
                collection);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("PUT", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual($"{{\"{fieldName}\": {{\"title\":\"{model.Title}\"}}}}", Encoding.Default.GetString(service.Content));
        }
        [TestMethod]
        public void Should_Unlocalize_Should_Have_Blank_Content()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var fieldName = _fixture.Create<string>();
            var collection = new Management.Core.Queryable.ParameterCollection();
            collection.Add(_fixture.Create<string>(), false);
            LocalizationService<EntryModel> service = new LocalizationService<EntryModel>(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                resourcePath,
                _fixture.Create<EntryModel>(),
                fieldName,
                collection,
                true);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.IsNull(service.Content);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual($"{resourcePath}/unlocalize", service.ResourcePath);
        }
    }
}
