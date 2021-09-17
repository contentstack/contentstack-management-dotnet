using System;
using System.Net.Mail;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack002_OrganisationTest
    {
        private double _count;
        static string RoleUID = "";
        static string EmailSync = "testcs@contentstack.com";
        static string EmailAsync = "testcs_1@contentstack.com";
        static string InviteID = "";
        static string InviteIDAsync = "";
        private readonly IFixture _fixture = new Fixture();

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_All_Organizations()
        {
            try
            {
                Organization organization = Contentstack.Client.Organization();

                ContentstackResponse contentstackResponse = organization.GetOrganizations();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                _count = (response["organizations"] as Newtonsoft.Json.Linq.JArray).Count;
                
            } catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Return_All_OrganizationsAsync()
        {
            try
            {
                Organization organization = Contentstack.Client.Organization();

                ContentstackResponse contentstackResponse = await organization.GetOrganizationsAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                _count = (response["organizations"] as Newtonsoft.Json.Linq.JArray).Count;

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Return_With_Skipping_Organizations()
        {
            try
            {
                Organization organization = Contentstack.Client.Organization();
                ParameterCollection collection = new ParameterCollection();
                collection.Add("skip", 4);
                ContentstackResponse contentstackResponse = organization.GetOrganizations(collection);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                var count = (response["organizations"] as Newtonsoft.Json.Linq.JArray).Count;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Return_Organization_With_UID()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.GetOrganizations();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["organization"]);

                OrganisationResponse model = contentstackResponse.OpenTResponse<OrganisationResponse>();
                Assert.AreEqual(org.Name, model.Organization.Name);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Return_Organization_With_UID_Include_Plan()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                ParameterCollection collection = new ParameterCollection();
                collection.Add("include_plan", true);

                ContentstackResponse contentstackResponse = organization.GetOrganizations(collection);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["organization"]);
                Assert.IsNotNull(response["organization"]["plan"]);
                
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Return_Organization_Roles()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.Roles();

                var response = contentstackResponse.OpenJObjectResponse();

                RoleUID = (string)response["roles"][0]["uid"];
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["roles"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Return_Organization_RolesAsync()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = await organization.RolesAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["roles"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Add_User_To_Organization()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                UserInvitation invitation = new UserInvitation()
                {
                    Email = EmailSync,
                    Roles = new System.Collections.Generic.List<string>() { RoleUID }
                };
                ContentstackResponse contentstackResponse = organization.AddUser(new System.Collections.Generic.List<UserInvitation>()
                {
                    invitation
                }, null);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.AreEqual(1, ((JArray)response["shares"]).Count);
                InviteID = (string)response["shares"][0]["uid"];
                Assert.AreEqual("The invitation has been sent successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test009_Should_Add_User_To_Organization()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                UserInvitation invitation = new UserInvitation()
                {
                    Email = EmailAsync,
                    Roles = new System.Collections.Generic.List<string>() { RoleUID }
                };
                ContentstackResponse contentstackResponse = await organization.AddUserAsync(new System.Collections.Generic.List<UserInvitation>()
                {
                    invitation
                }, null);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.AreEqual(1, ((JArray)response["shares"]).Count);
                InviteIDAsync = (string)response["shares"][0]["uid"];
                Assert.AreEqual("The invitation has been sent successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }
        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Resend_Invite()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.ResendInvitation(InviteID);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.AreEqual("The invitation has been resent successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test011_Should_Resend_Invite()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                ContentstackResponse contentstackResponse = await organization.ResendInvitationAsync(InviteIDAsync);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.AreEqual("The invitation has been resent successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Remove_User_From_Organization()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.RemoveUser(new System.Collections.Generic.List<string>() { EmailSync } );

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.AreEqual("The invitation has been deleted successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test013_Should_Remove_User_From_Organization()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                ContentstackResponse contentstackResponse = await organization.RemoveUserAsync(new System.Collections.Generic.List<string>() { EmailAsync });

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.AreEqual("The invitation has been deleted successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Get_All_Invites()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.GetInvitations();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["shares"]);
                Assert.AreEqual(response["shares"].GetType(), typeof(JArray));

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test015_Should_Get_All_Invites_Async()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                ContentstackResponse contentstackResponse = await organization.GetInvitationsAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["shares"]);
                Assert.AreEqual(response["shares"].GetType(), typeof(JArray));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Get_All_Stacks()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.GetStacks();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["stacks"]);
                Assert.AreEqual(response["stacks"].GetType(), typeof(JArray));

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test017_Should_Get_All_Stacks_Async()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                ContentstackResponse contentstackResponse = await organization.GetStacksAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["stacks"]);
                Assert.AreEqual(response["stacks"].GetType(), typeof(JArray));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }
    }
}
