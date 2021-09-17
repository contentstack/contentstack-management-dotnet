using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.Stack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Stack
{
    [TestClass]
    public class FetchStackServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FetchStackService(null, null, _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Initialize_with_Serializer()
        {
            var fetchStackService = new FetchStackService(serializer, null);

            Assert.IsNotNull(fetchStackService);
            Assert.AreEqual(true, fetchStackService.UseQueryString);
            Assert.AreEqual("GET", fetchStackService.HttpMethod);
            Assert.AreEqual("stacks", fetchStackService.ResourcePath);
        }

        [TestMethod]
        public void Should_Initialize_with_Organization_Uid()
        {
            var apiKey = _fixture.Create<string>();
            var fetchStackService = new FetchStackService(serializer, null, apiKey);

            Assert.IsNotNull(fetchStackService);
            Assert.AreEqual(true, fetchStackService.UseQueryString);
            Assert.AreEqual("GET", fetchStackService.HttpMethod);
            Assert.AreEqual("stacks", fetchStackService.ResourcePath);
            fetchStackService.ContentBody();
            Assert.AreEqual(apiKey, fetchStackService.Headers["api_key"]);
        }

        [TestMethod]
        public void Should_Initialize_with_Serializer_Empty_Param_Collection()
        {
            var fetchStackService = new FetchStackService(serializer, new Management.Core.Queryable.ParameterCollection());

            Assert.IsNotNull(fetchStackService);
            Assert.AreEqual(true, fetchStackService.UseQueryString);
            Assert.AreEqual("GET", fetchStackService.HttpMethod);
            Assert.AreEqual("stacks", fetchStackService.ResourcePath);
        }
    }
}
