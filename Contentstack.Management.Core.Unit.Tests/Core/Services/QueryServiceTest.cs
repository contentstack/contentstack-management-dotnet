using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Unit.Tests.Core.Services
{
    [TestClass]
    public class QueryServiceTest
    {
        private Management.Core.Models.Stack _stack;

        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestInitialize]
        public void initialize()
        {
            _stack = new Management.Core.Models.Stack(new ContentstackClient());
        }

        [TestMethod]
        public void Should_Not_Allow_Null_ResourcePath()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new QueryService(_stack, new ParameterCollection(), null));
        }

        [TestMethod]
        public void Should_Not_Allow_Null_API_Key()
        {
            var collection = new Management.Core.Queryable.ParameterCollection();

            collection.Add(_fixture.Create<string>(), false);
            Assert.ThrowsException<ArgumentNullException>(() => new QueryService(_stack, collection, _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_QueryParam_Collection_null()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();

            QueryService service = new QueryService(
                new Management.Core.Models.Stack(new ContentstackClient(), apiKey),
                null,
                resourcePath
                );

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Content_Body()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var collection = new ParameterCollection();

            collection.Add(_fixture.Create<string>(), false);
            QueryService service = new QueryService(
                new Management.Core.Models.Stack(new ContentstackClient(), apiKey),
                collection,
                resourcePath
                );

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
        }
    }
}
