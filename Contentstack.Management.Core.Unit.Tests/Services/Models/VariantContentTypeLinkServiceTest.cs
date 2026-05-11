using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Services.Models
{
    [TestClass]
    public class VariantContentTypeLinkServiceTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private JsonSerializerOptions _serializerOptions;

        [TestInitialize]
        public void initialize()
        {
            var client = new ContentstackClient();
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
            _serializerOptions = client.SerializerOptions;
        }

        [TestMethod]
        public void Initialize_VariantContentTypeLinkService_For_Link()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };
            string variantGroupUid = "test_variant_uid";
            bool isLink = true;

            var service = new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                resourcePath,
                contentTypeUids,
                variantGroupUid,
                isLink
            );

            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual("PUT", service.HttpMethod);
            Assert.IsFalse(service.UseQueryString);
        }

        [TestMethod]
        public void Initialize_VariantContentTypeLinkService_For_Unlink()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };
            string variantGroupUid = "test_variant_uid";
            bool isLink = false;

            var service = new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                resourcePath,
                contentTypeUids,
                variantGroupUid,
                isLink
            );

            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual("PUT", service.HttpMethod);
            Assert.IsFalse(service.UseQueryString);
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Stack_Is_Null()
        {
            Assert.ThrowsException<NullReferenceException>(() => new VariantContentTypeLinkService(
                _serializerOptions,
                null,
                "/variant_groups/test_uid/content_types",
                new List<string> { "ct_uid" },
                "test_variant_uid",
                true
            ));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ResourcePath_Is_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                null,
                new List<string> { "ct_uid" },
                "test_variant_uid",
                true
            ));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ContentTypeUids_Is_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                "/variant_groups/test_uid/content_types",
                null,
                "test_variant_uid",
                true
            ));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ContentTypeUids_Is_Empty()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                "/variant_groups/test_uid/content_types",
                new List<string>(),
                "test_variant_uid",
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
                _serializerOptions,
                stackWithoutApiKey,
                "/variant_groups/test_uid/content_types",
                new List<string> { "ct_uid" },
                "test_variant_uid",
                true
            ));
        }

        [TestMethod]
        public void Should_Serialize_Link_Request_Body_Correctly()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };
            string variantGroupUid = "test_variant_uid";
            bool isLink = true;

            var service = new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                resourcePath,
                contentTypeUids,
                variantGroupUid,
                isLink
            );

            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            string requestBody = Encoding.UTF8.GetString(service.ByteContent);

            var root = JsonNode.Parse(requestBody)!.AsObject();
            var contentTypes = root["content_types"] as JsonArray;
            Assert.IsNotNull(contentTypes);
            Assert.AreEqual(2, contentTypes.Count);

            Assert.AreEqual("ct_uid_1", contentTypes[0]!["uid"]!.GetValue<string>());
            Assert.AreEqual("linked", contentTypes[0]!["status"]!.GetValue<string>());

            Assert.AreEqual("ct_uid_2", contentTypes[1]!["uid"]!.GetValue<string>());
            Assert.AreEqual("linked", contentTypes[1]!["status"]!.GetValue<string>());
        }

        [TestMethod]
        public void Should_Serialize_Unlink_Request_Body_Correctly()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };
            string variantGroupUid = "test_variant_uid";
            bool isLink = false;

            var service = new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                resourcePath,
                contentTypeUids,
                variantGroupUid,
                isLink
            );

            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            string requestBody = Encoding.UTF8.GetString(service.ByteContent);

            var root = JsonNode.Parse(requestBody)!.AsObject();
            Assert.AreEqual("test_variant_uid", root["uid"]!.GetValue<string>());
            Assert.IsNotNull(root["branches"]);
            var contentTypes = root["content_types"] as JsonArray;
            Assert.IsNotNull(contentTypes);
            Assert.AreEqual(2, contentTypes.Count);

            Assert.AreEqual("ct_uid_1", contentTypes[0]!["uid"]!.GetValue<string>());
            Assert.AreEqual("unlinked", contentTypes[0]!["status"]!.GetValue<string>());

            Assert.AreEqual("ct_uid_2", contentTypes[1]!["uid"]!.GetValue<string>());
            Assert.AreEqual("unlinked", contentTypes[1]!["status"]!.GetValue<string>());
        }

        [TestMethod]
        public void Should_Serialize_Single_Content_Type_Correctly()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "single_ct_uid" };
            string variantGroupUid = "test_variant_uid";
            bool isLink = true;

            var service = new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                resourcePath,
                contentTypeUids,
                variantGroupUid,
                isLink
            );

            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            string requestBody = Encoding.UTF8.GetString(service.ByteContent);

            var root = JsonNode.Parse(requestBody)!.AsObject();
            Assert.AreEqual("test_variant_uid", root["uid"]!.GetValue<string>());
            Assert.IsNotNull(root["branches"]);

            var contentTypes = root["content_types"] as JsonArray;
            Assert.IsNotNull(contentTypes);
            Assert.AreEqual(1, contentTypes.Count);

            Assert.AreEqual("single_ct_uid", contentTypes[0]!["uid"]!.GetValue<string>());
            Assert.AreEqual("linked", contentTypes[0]!["status"]!.GetValue<string>());
        }

        [TestMethod]
        public void Should_Work_With_Query_Parameters()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid" };
            string variantGroupUid = "test_variant_uid";
            bool isLink = true;
            var parameters = new ParameterCollection();
            parameters.Add("include_count", "true");

            var service = new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                resourcePath,
                contentTypeUids,
                variantGroupUid,
                isLink,
                parameters
            );

            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual("PUT", service.HttpMethod);
            Assert.IsTrue(service.UseQueryString);
        }

        [TestMethod]
        public void Should_Handle_Empty_Query_Parameters()
        {
            string resourcePath = "/variant_groups/test_uid/content_types";
            List<string> contentTypeUids = new List<string> { "ct_uid" };
            string variantGroupUid = "test_variant_uid";
            bool isLink = true;

            var service = new VariantContentTypeLinkService(
                _serializerOptions,
                _stack,
                resourcePath,
                contentTypeUids,
                variantGroupUid,
                isLink,
                null
            );

            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual("PUT", service.HttpMethod);
            Assert.IsFalse(service.UseQueryString);
        }
    }
}
