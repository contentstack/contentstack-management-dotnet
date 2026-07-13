using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.Organization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Organization
{
    [TestClass]
    public class ResendInvitationServiceTest
    {

        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions();
        private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResendInvitationService(null, _fixture.Create<string>(), _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Null_UID()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResendInvitationService(serializerOptions, null, _fixture.Create<string>()));

        }

        [TestMethod]
        public void Should_Throw_On_Null_ShareUid()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResendInvitationService(serializerOptions, _fixture.Create<string>(), null));

        }

        [TestMethod]
        public void Should_Initialize_with_Organization_Uid()
        {
            var orgUid = _fixture.Create<string>();
            var shareUid = _fixture.Create<string>();
            var service = new ResendInvitationService(serializerOptions, orgUid, shareUid);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual("/organizations/{organization_uid}/share/{share_uid}/resend_invitation", service.ResourcePath);
            Assert.AreEqual(orgUid, service.PathResources["{organization_uid}"]);
            Assert.AreEqual(shareUid, service.PathResources["{share_uid}"]);
        }

    }
}
