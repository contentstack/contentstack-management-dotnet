using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.Stack;
using Contentstack.Management.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Contentstack.Management.Core.Queryable;

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
            var stack = new Management.Core.Models.Stack(contentstackClient: null, apiKey: _fixture.Create<String>());
            Assert.ThrowsException<ArgumentNullException>(() => new FetchStackService(null, stack));
        }

        [TestMethod]
        public void Should_Initialize_with_Serializer()
        {
            var stack = new Management.Core.Models.Stack(contentstackClient: null, apiKey: _fixture.Create<String>());
            var fetchStackService = new FetchStackService(serializer, stack);

            Assert.IsNotNull(fetchStackService);
            Assert.AreEqual(true, fetchStackService.UseQueryString);
            Assert.AreEqual("GET", fetchStackService.HttpMethod);
            Assert.AreEqual("stacks", fetchStackService.ResourcePath);
        }

        [TestMethod]
        public void Should_Initialize_with_Organization_Uid()
        {
            var apiKey = _fixture.Create<string>();
            var stack = new Management.Core.Models.Stack(contentstackClient: null, apiKey);
            var fetchStackService = new FetchStackService(serializer, stack);

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
            var stack = new Management.Core.Models.Stack(contentstackClient: null);
            var param = new ParameterCollection();
            param.Add("limit", 10);

            var fetchStackService = new FetchStackService(serializer, stack, param);

            Assert.IsNotNull(fetchStackService);
            Assert.AreEqual(true, fetchStackService.UseQueryString);
            Assert.AreEqual("GET", fetchStackService.HttpMethod);
            Assert.AreEqual("stacks", fetchStackService.ResourcePath);
        }
    }
}
