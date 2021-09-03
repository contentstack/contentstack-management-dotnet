using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.Organization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Organization
{
    [TestClass]
    public class OrgRolesTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new OrganizationRolesService(null, _fixture.Create<string>(), null));
        }

        [TestMethod]
        public void Should_Throw_On_Null_UID()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new OrganizationRolesService(serializer, null, null));

        }

        [TestMethod]
        public void Should_Initialize_with_Organization_Uid()
        {
            var orgId = _fixture.Create<string>();
            var orgRoles = new OrganizationRolesService(serializer, orgId, null);

            Assert.IsNotNull(orgRoles);
            Assert.AreEqual(true, orgRoles.UseQueryString);
            Assert.AreEqual("GET", orgRoles.HttpMethod);
            Assert.AreEqual("organizations/{organization_uid}/roles", orgRoles.ResourcePath);
            Assert.AreEqual(orgId, orgRoles.PathResources["{organization_uid}"]);
        }
    }
}
