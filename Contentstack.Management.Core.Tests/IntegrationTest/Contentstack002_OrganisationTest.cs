using System;
using System.Diagnostics;
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

        private static string _host => Contentstack.Client.contentstackOptions.Host;

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_All_Organizations()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Organization organization = Contentstack.Client.Organization();
                TestReportHelper.LogRequest("organization.GetOrganizations()", "GET",
                    $"https://{_host}/v3/organizations");

                ContentstackResponse contentstackResponse = organization.GetOrganizations();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                _count = (response["organizations"] as JArray).Count;

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Return_All_OrganizationsAsync()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Organization organization = Contentstack.Client.Organization();
                TestReportHelper.LogRequest("organization.GetOrganizationsAsync()", "GET",
                    $"https://{_host}/v3/organizations");

                ContentstackResponse contentstackResponse = await organization.GetOrganizationsAsync();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                _count = (response["organizations"] as JArray).Count;

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Return_With_Skipping_Organizations()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                Organization organization = Contentstack.Client.Organization();
                ParameterCollection collection = new ParameterCollection();
                collection.Add("skip", 4);
                TestReportHelper.LogRequest("organization.GetOrganizations(skip=4)", "GET",
                    $"https://{_host}/v3/organizations",
                    queryParams: new System.Collections.Generic.Dictionary<string, string> { ["skip"] = "4" });

                ContentstackResponse contentstackResponse = organization.GetOrganizations(collection);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                var count = (response["organizations"] as JArray).Count;

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Return_Organization_With_UID()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.GetOrganizations() by UID", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}");

                ContentstackResponse contentstackResponse = organization.GetOrganizations();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                OrganisationResponse model = contentstackResponse.OpenTResponse<OrganisationResponse>();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["organization"] != null, "organization key present", type: "IsNotNull");
                TestReportHelper.LogAssertion(model.Organization.Name == org.Name,
                    "Organization name matches", expected: org.Name, actual: model.Organization.Name, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["organization"]);
                Assert.AreEqual(org.Name, model.Organization.Name);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Return_Organization_With_UID_Include_Plan()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                ParameterCollection collection = new ParameterCollection();
                collection.Add("include_plan", true);
                TestReportHelper.LogRequest("organization.GetOrganizations(include_plan)", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}",
                    queryParams: new System.Collections.Generic.Dictionary<string, string> { ["include_plan"] = "true" });

                ContentstackResponse contentstackResponse = organization.GetOrganizations(collection);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["organization"] != null, "organization key present", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["organization"]["plan"] != null, "plan key present", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["organization"]);
                Assert.IsNotNull(response["organization"]["plan"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Return_Organization_Roles()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.Roles()", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}/roles");

                ContentstackResponse contentstackResponse = organization.Roles();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                RoleUID = (string)response["roles"][0]["uid"];

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["roles"] != null, "roles key present", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["roles"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Return_Organization_RolesAsync()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.RolesAsync()", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}/roles");

                ContentstackResponse contentstackResponse = await organization.RolesAsync();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["roles"] != null, "roles key present", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["roles"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Add_User_To_Organization()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                UserInvitation invitation = new UserInvitation()
                {
                    Email = EmailSync,
                    Roles = new System.Collections.Generic.List<string>() { RoleUID }
                };
                TestReportHelper.LogRequest("organization.AddUser()", "POST",
                    $"https://{_host}/v3/organizations/{org.Uid}/share");

                ContentstackResponse contentstackResponse = organization.AddUser(new System.Collections.Generic.List<UserInvitation>()
                {
                    invitation
                }, null);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.AreEqual(1, ((JArray)response["shares"]).Count);
                InviteID = (string)response["shares"][0]["uid"];

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(((JArray)response["shares"]).Count == 1,
                    "Shares count is 1", expected: "1", actual: ((JArray)response["shares"]).Count.ToString(), type: "AreEqual");
                Assert.AreEqual("The invitation has been sent successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test009_Should_Add_User_To_Organization()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                UserInvitation invitation = new UserInvitation()
                {
                    Email = EmailAsync,
                    Roles = new System.Collections.Generic.List<string>() { RoleUID }
                };
                TestReportHelper.LogRequest("organization.AddUserAsync()", "POST",
                    $"https://{_host}/v3/organizations/{org.Uid}/share");

                ContentstackResponse contentstackResponse = await organization.AddUserAsync(new System.Collections.Generic.List<UserInvitation>()
                {
                    invitation
                }, null);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.AreEqual(1, ((JArray)response["shares"]).Count);
                InviteIDAsync = (string)response["shares"][0]["uid"];

                TestReportHelper.LogAssertion(((JArray)response["shares"]).Count == 1,
                    "Shares count is 1", expected: "1", actual: ((JArray)response["shares"]).Count.ToString(), type: "AreEqual");
                Assert.AreEqual("The invitation has been sent successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Resend_Invite()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.ResendInvitation()", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}/share/{InviteID}/resend_invitation");

                ContentstackResponse contentstackResponse = organization.ResendInvitation(InviteID);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(
                    response["notice"]?.ToString() == "The invitation has been resent successfully.",
                    "Notice message matches", expected: "The invitation has been resent successfully.", actual: response["notice"]?.ToString(), type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.AreEqual("The invitation has been resent successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test011_Should_Resend_Invite()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.ResendInvitationAsync()", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}/share/{InviteIDAsync}/resend_invitation");

                ContentstackResponse contentstackResponse = await organization.ResendInvitationAsync(InviteIDAsync);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.AreEqual("The invitation has been resent successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Remove_User_From_Organization()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.RemoveUser()", "DELETE",
                    $"https://{_host}/v3/organizations/{org.Uid}/share");

                ContentstackResponse contentstackResponse = organization.RemoveUser(new System.Collections.Generic.List<string>() { EmailSync });
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(
                    response["notice"]?.ToString() == "The invitation has been deleted successfully.",
                    "Notice message matches", expected: "The invitation has been deleted successfully.", actual: response["notice"]?.ToString(), type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.AreEqual("The invitation has been deleted successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test013_Should_Remove_User_From_Organization()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.RemoveUserAsync()", "DELETE",
                    $"https://{_host}/v3/organizations/{org.Uid}/share");

                ContentstackResponse contentstackResponse = await organization.RemoveUserAsync(new System.Collections.Generic.List<string>() { EmailAsync });
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.AreEqual("The invitation has been deleted successfully.", response["notice"]);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Get_All_Invites()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.GetInvitations()", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}/share");

                ContentstackResponse contentstackResponse = organization.GetInvitations();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["shares"] != null, "shares key present", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["shares"].GetType() == typeof(JArray), "shares is JArray", type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["shares"]);
                Assert.AreEqual(response["shares"].GetType(), typeof(JArray));
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test015_Should_Get_All_Invites_Async()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.GetInvitationsAsync()", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}/share");

                ContentstackResponse contentstackResponse = await organization.GetInvitationsAsync();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["shares"] != null, "shares key present", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["shares"]);
                Assert.AreEqual(response["shares"].GetType(), typeof(JArray));
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Get_All_Stacks()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.GetStacks()", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}/stacks");

                ContentstackResponse contentstackResponse = organization.GetStacks();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["stacks"] != null, "stacks key present", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["stacks"]);
                Assert.AreEqual(response["stacks"].GetType(), typeof(JArray));
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test017_Should_Get_All_Stacks_Async()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                TestReportHelper.LogRequest("organization.GetStacksAsync()", "GET",
                    $"https://{_host}/v3/organizations/{org.Uid}/stacks");

                ContentstackResponse contentstackResponse = await organization.GetStacksAsync();
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var response = contentstackResponse.OpenJObjectResponse();

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(response["stacks"] != null, "stacks key present", type: "IsNotNull");
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["stacks"]);
                Assert.AreEqual(response["stacks"].GetType(), typeof(JArray));
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }
    }
}
