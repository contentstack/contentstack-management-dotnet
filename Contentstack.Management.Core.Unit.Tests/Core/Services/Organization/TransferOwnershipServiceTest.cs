using System;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.Organization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Organization
{
    [TestClass]
    public class TransferOwnershipServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransferOwnershipService(null, _fixture.Create<string>(), _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Null_UID()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransferOwnershipService(serializer, null, _fixture.Create<string>()));

        }

        [TestMethod]
        public void Should_Throw_On_Null_Email()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransferOwnershipService(serializer, _fixture.Create<string>(), null));
        }

        [TestMethod]
        public void Should_Initialize_with_Organization_Uid()
        {
            var orgUid = _fixture.Create<string>();
            var email = _fixture.Create<string>();
            var service = new TransferOwnershipService(serializer, orgUid, email);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/organizations/{organization_uid}/transfer-ownership", service.ResourcePath);
            Assert.AreEqual(orgUid, service.PathResources["{organization_uid}"]);
        }

        [TestMethod]
        public void Should_Return_Content_Of_Post_Method()
        {
            var orgUid = _fixture.Create<string>();
            var email = _fixture.Create<string>();
            var service = new TransferOwnershipService(serializer, orgUid, email);


            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual($"{{\"transfer_to\":\"{email}\"}}", Encoding.Default.GetString(service.ByteContent));
        }
    }
}
