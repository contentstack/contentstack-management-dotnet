using System;
using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Services.Models;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Models
{
    [TestClass]
    public class FetchDeleteServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        private static ContentstackClientOptions CreateConfig(IFixture fixture)
        {
            var config = new ContentstackClientOptions();
            config.Authtoken = fixture.Create<string>();
            return config;
        }

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FetchDeleteService(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FetchDeleteService(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Resource_Path_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FetchDeleteService(
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
            FetchDeleteService service = new FetchDeleteService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                resourcePath,
                collection: collection);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
        }

        [TestMethod]
        public void Delete_Release_Should_Not_Include_Content_Type_Header()
        {
            var stack = new Management.Core.Models.Stack(null, _fixture.Create<string>());
            var service = new FetchDeleteService(serializer, stack, "/releases/release_uid_123", "DELETE");

            service.CreateHttpRequest(new HttpClient(), CreateConfig(_fixture));

            Assert.IsFalse(service.Headers.ContainsKey(HeadersKey.ContentTypeHeader), "DELETE /releases/{uid} must not include Content-Type header.");
        }

        [TestMethod]
        public void Delete_Release_With_Path_Releases_Only_Should_Not_Include_Content_Type_Header()
        {
            var stack = new Management.Core.Models.Stack(null, _fixture.Create<string>());
            var service = new FetchDeleteService(serializer, stack, "/releases", "DELETE");

            service.CreateHttpRequest(new HttpClient(), CreateConfig(_fixture));

            Assert.IsFalse(service.Headers.ContainsKey(HeadersKey.ContentTypeHeader));
        }

        [TestMethod]
        public void Delete_Release_Items_Path_Should_Include_Content_Type_Header()
        {
            var stack = new Management.Core.Models.Stack(null, _fixture.Create<string>());
            var service = new FetchDeleteService(serializer, stack, "/releases/release_uid/item", "DELETE");

            service.CreateHttpRequest(new HttpClient(), CreateConfig(_fixture));

            Assert.IsTrue(service.Headers.ContainsKey(HeadersKey.ContentTypeHeader), "DELETE /releases/{uid}/item (FetchDeleteService) should still set Content-Type.");
            Assert.AreEqual("application/json", service.GetHeaderValue(HeadersKey.ContentTypeHeader));
        }

        [TestMethod]
        public void Fetch_Release_Should_Include_Content_Type_Header()
        {
            var stack = new Management.Core.Models.Stack(null, _fixture.Create<string>());
            var service = new FetchDeleteService(serializer, stack, "/releases/release_uid_123", "GET");

            service.CreateHttpRequest(new HttpClient(), CreateConfig(_fixture));

            Assert.IsTrue(service.Headers.ContainsKey(HeadersKey.ContentTypeHeader));
            Assert.AreEqual("application/json", service.GetHeaderValue(HeadersKey.ContentTypeHeader));
        }

        [TestMethod]
        public void Delete_Non_Release_Resource_Should_Include_Content_Type_Header()
        {
            var stack = new Management.Core.Models.Stack(null, _fixture.Create<string>());
            var service = new FetchDeleteService(serializer, stack, "/contenttypes/ct_uid", "DELETE");

            service.CreateHttpRequest(new HttpClient(), CreateConfig(_fixture));

            Assert.IsTrue(service.Headers.ContainsKey(HeadersKey.ContentTypeHeader));
            Assert.AreEqual("application/json", service.GetHeaderValue(HeadersKey.ContentTypeHeader));
        }
    }
}
