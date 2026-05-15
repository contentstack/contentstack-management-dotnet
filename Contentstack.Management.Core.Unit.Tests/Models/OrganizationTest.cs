using System;
using System.Collections.Generic;
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

            Assert.IsNull(organization.Uid);
            Assert.IsNotNull(organization);
        }

        [TestMethod]
        public void Initialize_Organization_Uid()
        {
            Organization organization = new Organization(client, "org_uid");

            Assert.AreEqual("org_uid", organization.Uid);
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
            Assert.ThrowsException<InvalidOperationException>(() => organization.AddUser(null, null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.AddUserAsync(null, null));
            Assert.ThrowsException<InvalidOperationException>(() => organization.RemoveUser(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.RemoveUserAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => organization.ResendInvitation(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.ResendInvitationAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => organization.GetInvitations());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.GetInvitationsAsync());
            Assert.ThrowsException<InvalidOperationException>(() => organization.TransferOwnership(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.TransferOwnershipAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => organization.GetStacks());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.GetStacksAsync());
        }

        [TestMethod]
        public void Should_Throw_On_Organization_Uid_Is_Null()
        {
            Organization organization = new Organization(client);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.ThrowsException<InvalidOperationException>(() => organization.Roles());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.RolesAsync());
            Assert.ThrowsException<InvalidOperationException>(() => organization.AddUser(null, null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.AddUserAsync(null, null));
            Assert.ThrowsException<InvalidOperationException>(() => organization.RemoveUser(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.RemoveUserAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => organization.ResendInvitation(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.ResendInvitationAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => organization.GetInvitations());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.GetInvitationsAsync());
            Assert.ThrowsException<InvalidOperationException>(() => organization.TransferOwnership(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.TransferOwnershipAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => organization.GetStacks(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => organization.GetStacksAsync(null));
        }

        [TestMethod]
        public void Should_Throw_On_Remove_User_Email_Null()
        {
            Organization organization = new Organization(client, "org_uid");
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Assert.ThrowsException<ArgumentNullException>(() => organization.RemoveUser(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => organization.RemoveUserAsync(null));
        }

        [TestMethod]
        public void Should_Throw_On_Share_UID_Null()
        {
            Organization organization = new Organization(client, "org_uid");
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Assert.ThrowsException<ArgumentNullException>(() => organization.ResendInvitation(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => organization.ResendInvitationAsync(null));
        }

        [TestMethod]
        public void Should_Throw_On_Email_Id_Null()
        {
            Organization organization = new Organization(client, "org_uid");
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Assert.ThrowsException<ArgumentNullException>(() => organization.TransferOwnership(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => organization.TransferOwnershipAsync(null));
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

        [TestMethod]
        public void Should_Return_Organizarion_Roles()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = organization.Roles();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_On_Organisation_RolesAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = await organization.RolesAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Invite_User_to_Organization()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");
            var userInvitation = new UserInvitation()
            {
                Email = _fixture.Create<string>(),
                Roles = _fixture.Create<List<string>>()
            };
            var stackInvite = new Dictionary<string, List<UserInvitation>>()
            {
                { "Stack",  new List<UserInvitation>() { userInvitation } }
            };
            ContentstackResponse response = organization.AddUser(new List<UserInvitation>() { userInvitation }, stackInvite);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_On_Invite_User_to_OrganisationAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");
            var userInvitation = new UserInvitation()
            {
                Email = _fixture.Create<string>(),
                Roles = _fixture.Create<List<string>>()
            };
            var stackInvite = new Dictionary<string, List<UserInvitation>>()
            {
                { "Stack",  new List<UserInvitation>() { userInvitation } }
            };

            ContentstackResponse response = await organization.AddUserAsync(new List<UserInvitation>() { userInvitation }, stackInvite);


            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Remove_User_From_Organization()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = organization.RemoveUser(_fixture.Create<List<string>>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_On_Remove_User_From_OrganisationAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = await organization.RemoveUserAsync(_fixture.Create<List<string>>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }


        [TestMethod]
        public void Should_Return_Get_Organization_Invites()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = organization.GetInvitations();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_Get_Organization_InvitesAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = await organization.GetInvitationsAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Resend_Organization_Invites()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = organization.ResendInvitation(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_Resend_Organization_InvitesAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = await organization.ResendInvitationAsync(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_TransferOwnership()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = organization.TransferOwnership(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_TransferOwnershipAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = await organization.TransferOwnershipAsync(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }


        [TestMethod]
        public void Should_Return_Get_Organization_Stacks()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = organization.GetStacks();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_Get_Organization_StacksAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("OrganizationResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Organization organization = new Organization(client, "org_uid");

            ContentstackResponse response = await organization.GetStacksAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }
    }
}
