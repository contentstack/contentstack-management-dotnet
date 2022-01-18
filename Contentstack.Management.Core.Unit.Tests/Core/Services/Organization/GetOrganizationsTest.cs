using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.Organization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Organization
{
    [TestClass]
    public class GetOrganizationsTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new GetOrganizations(null, null, _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Initialize_with_Serializer()
        {
            var getOrganisationService = new GetOrganizations(serializer, null);

            Assert.IsNotNull(getOrganisationService);
            Assert.AreEqual(true, getOrganisationService.UseQueryString);
            Assert.AreEqual("GET", getOrganisationService.HttpMethod);
            Assert.AreEqual("organizations", getOrganisationService.ResourcePath);
        }

        [TestMethod]
        public void Should_Initialize_with_Organization_Uid()
        {
            var orgid = _fixture.Create<string>();
            var getOrganisationService = new GetOrganizations(serializer, null, orgid);

            Assert.IsNotNull(getOrganisationService);
            Assert.AreEqual(true, getOrganisationService.UseQueryString);
            Assert.AreEqual("GET", getOrganisationService.HttpMethod);
            Assert.AreEqual("organizations/{organization_uid}", getOrganisationService.ResourcePath);
            Assert.AreEqual(orgid, getOrganisationService.PathResources["{organization_uid}"]);
        }

        [TestMethod]
        public void Should_Initialize_with_Serializer_Empty_Param_Collection()
        {
            var getOrganisationService = new GetOrganizations(serializer, new Management.Core.Queryable.ParameterCollection());

            Assert.IsNotNull(getOrganisationService);
            Assert.AreEqual(true, getOrganisationService.UseQueryString);
            Assert.AreEqual("GET", getOrganisationService.HttpMethod);
            Assert.AreEqual("organizations", getOrganisationService.ResourcePath);
        }

        [TestMethod]
        public void Should_Initialize_with_Serializer_Param_Collection()
        {
            var collection = new Management.Core.Queryable.ParameterCollection();
            collection.Add(_fixture.Create<string>(), false);
            var getOrganisationService = new GetOrganizations(serializer, collection);

            Assert.IsNotNull(getOrganisationService);
            Assert.AreEqual(true, getOrganisationService.UseQueryString);
            Assert.AreEqual("GET", getOrganisationService.HttpMethod);
            Assert.AreEqual("organizations", getOrganisationService.ResourcePath);
        }
    }
}
