using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Models
{
    [TestClass]
    public class FetchReferencesServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());


        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FetchReferencesService(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FetchReferencesService(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Resource_Path_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FetchReferencesService(
                serializer,
                new Management.Core.Models.Stack(null, _fixture.Create<string>()),
                null));
        }

        [TestMethod]
        public void Should_Provide_Valid_Param_On_Initialize()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var fieldName = _fixture.Create<string>();
            var collection = new Management.Core.Queryable.ParameterCollection();

            collection.Add(_fixture.Create<string>(), false);
            FetchReferencesService service = new FetchReferencesService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                resourcePath);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"{resourcePath}/references", service.ResourcePath);
        }
    }
}
