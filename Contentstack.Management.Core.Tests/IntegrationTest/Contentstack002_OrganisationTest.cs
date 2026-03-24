using System;
using System.Net.Mail;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack002_OrganisationTest
    {
        private static ContentstackClient _client;
        private double _count;
        static string RoleUID = "";
        static string EmailSync = "testcs@contentstack.com";
        static string EmailAsync = "testcs_1@contentstack.com";
        static string InviteID = "";
        static string InviteIDAsync = "";
        private readonly IFixture _fixture = new Fixture();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            try { _client?.Logout(); } catch { }
            _client = null;
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_All_Organizations()
        {
            TestOutputLogger.LogContext("TestScenario", "GetAllOrganizations");
            try
            {
                Organization organization = _client.Organization();

                ContentstackResponse contentstackResponse = organization.GetOrganizations();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                _count = (response["organizations"] as Newtonsoft.Json.Linq.JArray).Count;
                
            } catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
            
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Return_All_OrganizationsAsync()
        {
            TestOutputLogger.LogContext("TestScenario", "GetAllOrganizationsAsync");
            try
            {
                Organization organization = _client.Organization();

                ContentstackResponse contentstackResponse = await organization.GetOrganizationsAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                _count = (response["organizations"] as Newtonsoft.Json.Linq.JArray).Count;

            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Return_With_Skipping_Organizations()
        {
            TestOutputLogger.LogContext("TestScenario", "SkipOrganizations");
            try
            {
                Organization organization = _client.Organization();
                ParameterCollection collection = new ParameterCollection();
                collection.Add("skip", 4);
                ContentstackResponse contentstackResponse = organization.GetOrganizations(collection);

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                var count = (response["organizations"] as Newtonsoft.Json.Linq.JArray).Count;
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Return_Organization_With_UID()
        {
            TestOutputLogger.LogContext("TestScenario", "GetOrganizationByUID");
            try
            {
                var org = Contentstack.Organization;
                TestOutputLogger.LogContext("OrganizationUid", org.Uid);
                Organization organization = _client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.GetOrganizations();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNotNull(response["organization"], "organization");

                OrganisationResponse model = contentstackResponse.OpenTResponse<OrganisationResponse>();
                AssertLogger.AreEqual(org.Name, model.Organization.Name, "OrganizationName");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Return_Organization_With_UID_Include_Plan()
        {
            TestOutputLogger.LogContext("TestScenario", "GetOrganizationWithPlan");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);
                ParameterCollection collection = new ParameterCollection();
                collection.Add("include_plan", true);

                ContentstackResponse contentstackResponse = organization.GetOrganizations(collection);

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNotNull(response["organization"], "organization");
                AssertLogger.IsNotNull(response["organization"]["plan"], "plan");
                
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Return_Organization_Roles()
        {
            TestOutputLogger.LogContext("TestScenario", "GetOrganizationRoles");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.Roles();

                var response = contentstackResponse.OpenJObjectResponse();

                RoleUID = (string)response["roles"][0]["uid"];
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNotNull(response["roles"], "roles");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Return_Organization_RolesAsync()
        {
            TestOutputLogger.LogContext("TestScenario", "GetOrganizationRolesAsync");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = await organization.RolesAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNotNull(response["roles"], "roles");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Add_User_To_Organization()
        {
            TestOutputLogger.LogContext("TestScenario", "AddUserToOrg");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);
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
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual(1, ((JArray)response["shares"]).Count, "sharesCount");
                InviteID = (string)response["shares"][0]["uid"];
                AssertLogger.AreEqual("The invitation has been sent successfully.", (string)response["notice"], "notice");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test009_Should_Add_User_To_Organization()
        {
            TestOutputLogger.LogContext("TestScenario", "AddUserToOrgAsync");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);
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
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual(1, ((JArray)response["shares"]).Count, "sharesCount");
                InviteIDAsync = (string)response["shares"][0]["uid"];
                AssertLogger.AreEqual("The invitation has been sent successfully.", (string)response["notice"], "notice");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }
        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Resend_Invite()
        {
            TestOutputLogger.LogContext("TestScenario", "ResendInvite");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.ResendInvitation(InviteID);

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual("The invitation has been resent successfully.", (string)response["notice"], "notice");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test011_Should_Resend_Invite()
        {
            TestOutputLogger.LogContext("TestScenario", "ResendInviteAsync");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);
                ContentstackResponse contentstackResponse = await organization.ResendInvitationAsync(InviteIDAsync);

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual("The invitation has been resent successfully.", (string)response["notice"], "notice");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Remove_User_From_Organization()
        {
            TestOutputLogger.LogContext("TestScenario", "RemoveUser");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.RemoveUser(new System.Collections.Generic.List<string>() { EmailSync } );

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual("The invitation has been deleted successfully.", (string)response["notice"], "notice");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test013_Should_Remove_User_From_Organization()
        {
            TestOutputLogger.LogContext("TestScenario", "RemoveUserAsync");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);
                ContentstackResponse contentstackResponse = await organization.RemoveUserAsync(new System.Collections.Generic.List<string>() { EmailAsync });

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.AreEqual("The invitation has been deleted successfully.", (string)response["notice"], "notice");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Get_All_Invites()
        {
            TestOutputLogger.LogContext("TestScenario", "GetAllInvites");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.GetInvitations();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNotNull(response["shares"], "shares");
                AssertLogger.AreEqual(response["shares"].GetType(), typeof(JArray), "sharesType");

            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test015_Should_Get_All_Invites_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "GetAllInvitesAsync");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);
                ContentstackResponse contentstackResponse = await organization.GetInvitationsAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNotNull(response["shares"], "shares");
                AssertLogger.AreEqual(response["shares"].GetType(), typeof(JArray), "sharesType");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Get_All_Stacks()
        {
            TestOutputLogger.LogContext("TestScenario", "GetAllStacks");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.GetStacks();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNotNull(response["stacks"], "stacks");
                AssertLogger.AreEqual(response["stacks"].GetType(), typeof(JArray), "stacksType");

            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test017_Should_Get_All_Stacks_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "GetAllStacksAsync");
            try
            {
                var org = Contentstack.Organization;
                Organization organization = _client.Organization(org.Uid);
                ContentstackResponse contentstackResponse = await organization.GetStacksAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(response, "response");
                AssertLogger.IsNotNull(response["stacks"], "stacks");
                AssertLogger.AreEqual(response["stacks"].GetType(), typeof(JArray), "stacksType");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }

        }
    }
}
