using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class OrganizationTest
    {
        private ContentstackClient client;
        private readonly IFixture _fixture = new Fixture();

        [TestInitialize]
        public void initialize()
        {
            client = new ContentstackClient();
        }

        [TestMethod]
        public void Initialize_Organization()
        {
            Organization organization = new Organization(client);

            Assert.IsNull(organization.uid);
            Assert.IsNotNull(organization);
        }

        [TestMethod]
        public void Initialize_Organization_Uid()
        {
            Organization organization = new Organization(client, "org_uid");

            Assert.AreEqual("org_uid", organization.uid);
            Assert.IsNotNull(organization);
        }

        [TestMethod]
        public void Should_Throw_On_Login_If_Not_Logged_In()
        {
            Organization organization = new Organization(client);

            Assert.ThrowsException<InvalidOperationException>(() => organization.GetOrganizations());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.GetOrganizationsAsync());
            Assert.ThrowsException<InvalidOperationException>(() => organization.Roles());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.RolesAsync());
        }

        [TestMethod]
        public void Should_Throw_On_Organization_Uid_Is_Null()
        {
            Organization organization = new Organization(client);

            Assert.ThrowsException<InvalidOperationException>(() => organization.Roles());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.RolesAsync());
        }

        [TestMethod]
        public void Should_Return_All_Organizations()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client);

            ContentstackResponse response = organization.GetOrganizations();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_On_GetOrganisationsAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client);

            ContentstackResponse response = await organization.GetOrganizationsAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Response_On_GetOrganisationAsync_Success_On_ContinueWith()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client);

            var response = organization.GetOrganizationsAsync();

            response.ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    var result = t.Result as ContentstackResponse;
                    Assert.AreEqual(contentstackResponse.OpenResponse(), result.OpenResponse());
                    Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), result.OpenJObjectResponse().ToString());
                }
            });
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Organization()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = organization.GetOrganizations();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_On_GetOrganisationAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = await organization.GetOrganizationsAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }
    }
}
