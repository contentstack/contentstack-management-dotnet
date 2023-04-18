using System;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using contentstack.management.core.Services.App;
using Contentstack.Management.Core.Queryable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.App
{
    [TestClass]
    public class CreateUpdateAppsServiceTest
	{
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateAppsService(
                null,
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<JObject>(),
                _fixture.Create<string>(),
                _fixture.Create<ParameterCollection>()));
        }
        [TestMethod]
        public void Should_Throw_On_Null_Org_uid()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateAppsService(
                serializer,
                null,
                _fixture.Create<string>(),
                _fixture.Create<JObject>(),
                _fixture.Create<string>(),
                _fixture.Create<ParameterCollection>()));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Resource_Path()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateAppsService(
                serializer,
                _fixture.Create<string>(),
                null,
                _fixture.Create<JObject>(),
                _fixture.Create<string>(),
                _fixture.Create<ParameterCollection>()));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Data_Model ()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateAppsService(
                serializer,
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                null,
                _fixture.Create<string>(),
                _fixture.Create<ParameterCollection>()));
        }

        [TestMethod]
        public void Should_Have_Byte_Content()
        {
            string orgUid = _fixture.Create<string>();
            string resourcePath = _fixture.Create<string>();
            JObject keyValues = new JObject();
            keyValues["data"] = "data";

            var service = new CreateUpdateAppsService(
                serializer,
                orgUid,
                resourcePath,
                keyValues);
            service.ContentBody();
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual(false, service.UseQueryString);
            Assert.AreEqual(orgUid, service.Headers["organization_uid"]);
            Assert.AreEqual(resourcePath, service.ResourcePath);
            Assert.AreEqual($"{{\"data\":\"data\"}}", Encoding.Default.GetString(service.ByteContent));
        }
    }
}

