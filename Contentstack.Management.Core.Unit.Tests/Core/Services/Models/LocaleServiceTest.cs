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
    public class LocaleServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());


        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LocaleService(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LocaleService(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Null_Resource_Path()
        {
            var apiKey = _fixture.Create<string>();
            var collection = new Management.Core.Queryable.ParameterCollection();

            collection.Add(_fixture.Create<string>(), false);
            LocaleService service = new LocaleService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey));

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual("locales", service.ResourcePath);
        }


        [TestMethod]
        public void Should_Non_Null_Resource_Path()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var collection = new Management.Core.Queryable.ParameterCollection();

            collection.Add(_fixture.Create<string>(), false);
            LocaleService service = new LocaleService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                resourcePath);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"{resourcePath}/locales", service.ResourcePath);
        }
    }
}
