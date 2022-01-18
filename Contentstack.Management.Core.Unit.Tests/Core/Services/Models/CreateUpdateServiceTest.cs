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
    public class CreateUpdateServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());

        
        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateService<ContentModeling>(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                new ContentModeling(),
                _fixture.Create<string>()));
        }
        [TestMethod]
        public void Should_Throw_On_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateService<ContentModeling>(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                new ContentModeling(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Resource_Path_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateService<ContentModeling>(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                null,
                new ContentModeling(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Data_Model_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateService<ContentModeling>(
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

            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateService<ContentModeling>(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                _fixture.Create<string>(),
                new ContentModeling(),
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
            CreateUpdateService<ContentModeling> service = new CreateUpdateService<ContentModeling>(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                resourcePath,
                new ContentModeling(),
                fieldName,
                collection: collection);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual($"{{\"{fieldName}\": {{\"title\":null,\"uid\":null,\"field_rules\":null,\"schema\":null,\"options\":null}}}}", Encoding.Default.GetString(service.Content));
        }
    }
}
