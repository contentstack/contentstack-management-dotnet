using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Services.Models
{
    [TestClass]
    public class VariantContentTypeLinkServiceTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private JsonSerializer _serializer;

        [TestInitialize]
        public void initialize()
        {
            var client = new ContentstackClient();
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
            _serializer = client.serializer;
        }

        [TestMethod]
        public void Initialize_VariantContentTypeLinkService_For_Link()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };
            bool isLink = true;

            var service = new VariantContentTypeLinkService(
                _serializer,
                _stack,
                resourcePath,
                contentTypeUids,
                isLink
            );

            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.IsFalse(service.UseQueryString);
        }

        [TestMethod]
        public void Initialize_VariantContentTypeLinkService_For_Unlink()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };
            bool isLink = false;

            var service = new VariantContentTypeLinkService(
                _serializer,
                _stack,
                resourcePath,
                contentTypeUids,
                isLink
            );

            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.IsFalse(service.UseQueryString);
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Stack_Is_Null()
        {
            Assert.ThrowsException<NullReferenceException>(() => new VariantContentTypeLinkService(
                _serializer,
                null,
                "/variant_groups/test_uid/content_types",
                new List<string> { "ct_uid" },
                true
            ));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ResourcePath_Is_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VariantContentTypeLinkService(
                _serializer,
                _stack,
                null,
                new List<string> { "ct_uid" },
                true
            ));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ContentTypeUids_Is_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VariantContentTypeLinkService(
                _serializer,
                _stack,
                "/variant_groups/test_uid/content_types",
                null,
                true
            ));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ContentTypeUids_Is_Empty()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VariantContentTypeLinkService(
                _serializer,
                _stack,
                "/variant_groups/test_uid/content_types",
                new List<string>(),
                true
            ));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Stack_APIKey_Is_Null()
        {
            var client = new ContentstackClient();
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            var stackWithoutApiKey = new Stack(client);

            Assert.ThrowsException<ArgumentNullException>(() => new VariantContentTypeLinkService(
                _serializer,
                stackWithoutApiKey,
                "/variant_groups/test_uid/content_types",
                new List<string> { "ct_uid" },
                true
            ));
        }

        [TestMethod]
        public void Should_Serialize_Link_Request_Body_Correctly()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };
            bool isLink = true;

            var service = new VariantContentTypeLinkService(
                _serializer,
                _stack,
                resourcePath,
                contentTypeUids,
                isLink
            );

            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            string requestBody = Encoding.UTF8.GetString(service.ByteContent);
            
            // Parse the JSON to verify structure
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(requestBody);
            Assert.IsNotNull(jsonObject.content_types);
            
            var contentTypes = jsonObject.content_types;
            Assert.AreEqual(2, contentTypes.Count);
            
            Assert.AreEqual("ct_uid_1", (string)contentTypes[0].uid);
            Assert.AreEqual("linked", (string)contentTypes[0].status);
            
            Assert.AreEqual("ct_uid_2", (string)contentTypes[1].uid);
            Assert.AreEqual("linked", (string)contentTypes[1].status);
        }

        [TestMethod]
        public void Should_Serialize_Unlink_Request_Body_Correctly()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };
            bool isLink = false;

            var service = new VariantContentTypeLinkService(
                _serializer,
                _stack,
                resourcePath,
                contentTypeUids,
                isLink
            );

            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            string requestBody = Encoding.UTF8.GetString(service.ByteContent);
            
            // Parse the JSON to verify structure
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(requestBody);
            Assert.IsNotNull(jsonObject.content_types);
            
            var contentTypes = jsonObject.content_types;
            Assert.AreEqual(2, contentTypes.Count);
            
            Assert.AreEqual("ct_uid_1", (string)contentTypes[0].uid);
            Assert.AreEqual("unlinked", (string)contentTypes[0].status);
            
            Assert.AreEqual("ct_uid_2", (string)contentTypes[1].uid);
            Assert.AreEqual("unlinked", (string)contentTypes[1].status);
        }

        [TestMethod]
        public void Should_Serialize_Single_Content_Type_Correctly()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "single_ct_uid" };
            bool isLink = true;

            var service = new VariantContentTypeLinkService(
                _serializer,
                _stack,
                resourcePath,
                contentTypeUids,
                isLink
            );

            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            string requestBody = Encoding.UTF8.GetString(service.ByteContent);
            
            // Parse the JSON to verify structure
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(requestBody);
            Assert.IsNotNull(jsonObject.content_types);
            
            var contentTypes = jsonObject.content_types;
            Assert.AreEqual(1, contentTypes.Count);
            
            Assert.AreEqual("single_ct_uid", (string)contentTypes[0].uid);
            Assert.AreEqual("linked", (string)contentTypes[0].status);
        }

        [TestMethod]
        public void Should_Work_With_Query_Parameters()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid" };
            bool isLink = true;
            var parameters = new ParameterCollection();
            parameters.Add("include_count", "true");

            var service = new VariantContentTypeLinkService(
                _serializer,
                _stack,
                resourcePath,
                contentTypeUids,
                isLink,
                parameters
            );

            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
            // UseQueryString should be true when parameters are provided
            Assert.IsTrue(service.UseQueryString);
        }

        [TestMethod]
        public void Should_Handle_Empty_Query_Parameters()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid" };
            bool isLink = true;

            var service = new VariantContentTypeLinkService(
                _serializer,
                _stack,
                resourcePath,
                contentTypeUids,
                isLink,
                null
            );

            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.IsFalse(service.UseQueryString);
        }
    }
}
