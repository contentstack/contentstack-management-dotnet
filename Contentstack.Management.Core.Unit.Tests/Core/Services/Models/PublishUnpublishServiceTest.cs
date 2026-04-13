using System;
using System.Collections.Generic;
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
    public class PublishUnpublishServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PublishUnpublishService(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<PublishUnpublishDetails>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()));
        }
        [TestMethod]
        public void Should_Throw_On_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PublishUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<PublishUnpublishDetails>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Data_Model_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new PublishUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                null,
                _fixture.Create<string>(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Resource_Path_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new PublishUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                _fixture.Create<PublishUnpublishDetails>(),
                null,
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_FieldName_Null()
        {
            var apiKey = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() => new PublishUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                _fixture.Create<PublishUnpublishDetails>(),
                _fixture.Create<string>(),
                null));
        }

        [TestMethod]
        public void Should_Create_Content_Body()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var fieldName = _fixture.Create<string>();
            var details = _fixture.Create<PublishUnpublishDetails>();
            details.Variants = null;
            details.VariantRules = null;
            PublishUnpublishService service = new PublishUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                details,
                resourcePath,
                fieldName);
            service.ContentBody();

            var locales = new List<string>();
            foreach (string locale in details.Locales)
                locales.Add($"\"{locale}\"");
            var environments = new List<string>();
            foreach (string environment in details.Environments)
                environments.Add($"\"{environment}\"");

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual($"{{\"{fieldName}\":{{\"locales\":[{string.Join(",", locales)}],\"environments\":[{string.Join(",", environments)}]}},\"version\":{details.Version},\"scheduled_at\":\"{details.ScheduledAt}\"}}", Encoding.Default.GetString(service.ByteContent));
        }

        [TestMethod]
        public void Should_Create_Content_Body_with_Locale()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var fieldName = _fixture.Create<string>();
            var details = _fixture.Create<PublishUnpublishDetails>();
            details.Variants = null;
            details.VariantRules = null;
            var locale = _fixture.Create<string>();
            PublishUnpublishService service = new PublishUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                details,
                resourcePath,
                fieldName,
                locale
                );
            service.ContentBody();

            var locales = new List<string>();
            foreach (string local in details.Locales)
                locales.Add($"\"{local}\"");
            var environments = new List<string>();
            foreach (string environment in details.Environments)
                environments.Add($"\"{environment}\"");

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual($"{{\"{fieldName}\":{{\"locales\":[{string.Join(",", locales)}],\"environments\":[{string.Join(",", environments)}]}},\"version\":{details.Version},\"locale\":\"{locale}\",\"scheduled_at\":\"{details.ScheduledAt}\"}}", Encoding.Default.GetString(service.ByteContent));
        }

        public void Should_Create_Blank_Content_Body()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var fieldName = _fixture.Create<string>();

            PublishUnpublishService service = new PublishUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                new PublishUnpublishDetails(),
                resourcePath,
                fieldName);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual($"{{\"{fieldName}\": {{}}}}", Encoding.Default.GetString(service.ByteContent));
        }
        public void Should_Create_Blank_Locale_and_Environment_Content_Body()
        {
            var apiKey = _fixture.Create<string>();
            var resourcePath = _fixture.Create<string>();
            var fieldName = _fixture.Create<string>();

            PublishUnpublishService service = new PublishUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                new PublishUnpublishDetails()
                {
                    Locales = new List<string>(),
                    Environments = new List<string>()

                },
                resourcePath,
                fieldName);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual($"{{\"{fieldName}\": {{}}}}", Encoding.Default.GetString(service.ByteContent));
        }
        [TestMethod]
        public void Should_Create_Content_Body_With_Variants()
        {
            var apiKey = "api_key";
            var resourcePath = "/publish";
            var fieldName = "entry";
            var details = new PublishUnpublishDetails()
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "development" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = "cs123", Version = 1 }
                },
                VariantRules = new PublishVariantRules
                {
                    PublishLatestBase = true,
                    PublishLatestBaseConditionally = false
                }
            };
            PublishUnpublishService service = new PublishUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null, apiKey),
                details,
                resourcePath,
                fieldName);
            service.ContentBody();

            string expectedJson = "{\"entry\":{\"locales\":[\"en-us\"],\"environments\":[\"development\"],\"variants\":[{\"uid\":\"cs123\",\"version\":1}],\"variant_rules\":{\"publish_latest_base\":true,\"publish_latest_base_conditionally\":false}}}";
            
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual(expectedJson, Encoding.Default.GetString(service.ByteContent));
        }
    }
}
