using System;
using System.Diagnostics;
using System.Net;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack001_LoginTest
    {
        private readonly IConfigurationRoot _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        private static string _host => Contentstack.Client.contentstackOptions.Host;

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_Failuer_On_Wrong_Login_Credentials()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();
                NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");

                TestReportHelper.LogRequest("client.Login()", "POST",
                    $"https://{_host}/v3/user-session");
                try
                {
                    ContentstackResponse contentstackResponse = client.Login(credentials);
                }
                catch (Exception e)
                {
                    sw.Stop();
                    ContentstackErrorException errorException = e as ContentstackErrorException;
                    TestReportHelper.LogAssertion(errorException?.StatusCode == HttpStatusCode.UnprocessableEntity,
                        "Status code is UnprocessableEntity",
                        expected: "UnprocessableEntity", actual: errorException?.StatusCode.ToString(), type: "AreEqual");
                    TestReportHelper.LogAssertion(errorException?.ErrorCode == 104,
                        "Error code is 104",
                        expected: "104", actual: errorException?.ErrorCode.ToString(), type: "AreEqual");
                    Assert.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode);
                    Assert.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.Message);
                    Assert.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.ErrorMessage);
                    Assert.AreEqual(104, errorException.ErrorCode);
                }
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Unexpected exception: {e.GetType().Name}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Return_Failuer_On_Wrong_Async_Login_Credentials()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();
                NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");

                TestReportHelper.LogRequest("client.LoginAsync()", "POST",
                    $"https://{_host}/v3/user-session");

                var response = client.LoginAsync(credentials);
                response.ContinueWith((t) =>
                {
                    if (t.IsCompleted && t.Status == System.Threading.Tasks.TaskStatus.Faulted)
                    {
                        ContentstackErrorException errorException = t.Exception.InnerException as ContentstackErrorException;
                        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode);
                        Assert.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.Message);
                        Assert.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.ErrorMessage);
                        Assert.AreEqual(104, errorException.ErrorCode);
                    }
                });
                Thread.Sleep(3000);
                sw.Stop();
                TestReportHelper.LogAssertion(true, "Async login with wrong credentials handled via ContinueWith", type: "IsTrue");
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Unexpected exception: {e.GetType().Name}", type: "Fail");
                Assert.Fail(e.Message);
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Return_Success_On_Async_Login()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();

                TestReportHelper.LogRequest("client.LoginAsync()", "POST",
                    $"https://{_host}/v3/user-session");

                ContentstackResponse contentstackResponse = await client.LoginAsync(Contentstack.Credential);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                string loginResponse = body;

                TestReportHelper.LogAssertion(client.contentstackOptions.Authtoken != null,
                    "Authtoken is not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(loginResponse != null, "Login response is not null", type: "IsNotNull");
                Assert.IsNotNull(client.contentstackOptions.Authtoken);
                Assert.IsNotNull(loginResponse);

                await client.LogoutAsync();
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
        public void Test004_Should_Return_Success_On_Login()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();

                TestReportHelper.LogRequest("client.Login()", "POST",
                    $"https://{_host}/v3/user-session");

                ContentstackResponse contentstackResponse = client.Login(Contentstack.Credential);
                sw.Stop();
                var body = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                string loginResponse = body;

                TestReportHelper.LogAssertion(client.contentstackOptions.Authtoken != null, "Authtoken not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(loginResponse != null, "Login response not null", type: "IsNotNull");
                Assert.IsNotNull(client.contentstackOptions.Authtoken);
                Assert.IsNotNull(loginResponse);
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
        public void Test005_Should_Return_Loggedin_User()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();
                client.Login(Contentstack.Credential);

                TestReportHelper.LogRequest("client.GetUser()", "GET",
                    $"https://{_host}/v3/user");

                ContentstackResponse response = client.GetUser();
                sw.Stop();
                var body = response.OpenResponse();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var user = response.OpenJObjectResponse();

                TestReportHelper.LogAssertion(user != null, "User response not null", type: "IsNotNull");
                Assert.IsNotNull(user);
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
        public async System.Threading.Tasks.Task Test006_Should_Return_Loggedin_User_Async()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();
                await client.LoginAsync(Contentstack.Credential);

                TestReportHelper.LogRequest("client.GetUserAsync()", "GET",
                    $"https://{_host}/v3/user");

                ContentstackResponse response = await client.GetUserAsync();
                sw.Stop();
                var body = response.OpenResponse();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var user = response.OpenJObjectResponse();

                TestReportHelper.LogAssertion(user != null, "User not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(user["user"]["organizations"] != null, "Organizations not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(user["user"]["organizations"] is JArray, "Organizations is JArray", type: "IsInstanceOfType");
                TestReportHelper.LogAssertion(user["user"]["organizations"][0]["org_roles"] == null, "org_roles is null", type: "IsNull");
                Assert.IsNotNull(user);
                Assert.IsNotNull(user["user"]["organizations"]);
                Assert.IsInstanceOfType(user["user"]["organizations"], typeof(JArray));
                Assert.IsNull(user["user"]["organizations"][0]["org_roles"]);
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
        public void Test007_Should_Return_Loggedin_User_With_Organizations_detail()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ParameterCollection collection = new ParameterCollection();
                collection.Add("include_orgs_roles", true);

                ContentstackClient client = new ContentstackClient();
                client.Login(Contentstack.Credential);

                TestReportHelper.LogRequest("client.GetUser(collection)", "GET",
                    $"https://{_host}/v3/user",
                    queryParams: new System.Collections.Generic.Dictionary<string, string> { ["include_orgs_roles"] = "true" });

                ContentstackResponse response = client.GetUser(collection);
                sw.Stop();
                var body = response.OpenResponse();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, body);

                var user = response.OpenJObjectResponse();

                TestReportHelper.LogAssertion(user != null, "User not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(user["user"]["organizations"] != null, "Organizations not null", type: "IsNotNull");
                TestReportHelper.LogAssertion(user["user"]["organizations"][0]["org_roles"] != null, "org_roles not null", type: "IsNotNull");
                Assert.IsNotNull(user);
                Assert.IsNotNull(user["user"]["organizations"]);
                Assert.IsInstanceOfType(user["user"]["organizations"], typeof(JArray));
                Assert.IsNotNull(user["user"]["organizations"][0]["org_roles"]);
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
        public void Test008_Should_Fail_Login_With_Invalid_MfaSecret()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();
                NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
                string invalidMfaSecret = "INVALID_BASE32_SECRET!@#";

                TestReportHelper.LogRequest("client.Login() with invalid MFA secret", "POST",
                    $"https://{_host}/v3/user-session");

                try
                {
                    ContentstackResponse contentstackResponse = client.Login(credentials, null, invalidMfaSecret);
                    sw.Stop();
                    Assert.Fail("Expected exception for invalid MFA secret");
                }
                catch (ArgumentException)
                {
                    sw.Stop();
                    TestReportHelper.LogAssertion(true, "ArgumentException thrown for invalid Base32 MFA secret", type: "IsTrue");
                    Assert.IsTrue(true);
                }
                catch (Exception e)
                {
                    sw.Stop();
                    Assert.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
                }
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
        public void Test009_Should_Generate_TOTP_Token_With_Valid_MfaSecret()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();
                NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
                string validMfaSecret = "JBSWY3DPEHPK3PXP";

                TestReportHelper.LogRequest("client.Login() with valid MFA secret", "POST",
                    $"https://{_host}/v3/user-session");

                try
                {
                    ContentstackResponse contentstackResponse = client.Login(credentials, null, validMfaSecret);
                    sw.Stop();
                }
                catch (ContentstackErrorException errorException)
                {
                    sw.Stop();
                    TestReportHelper.LogAssertion(
                        errorException.StatusCode == HttpStatusCode.UnprocessableEntity,
                        "Status code is UnprocessableEntity (credentials rejected, not MFA format)",
                        expected: "UnprocessableEntity", actual: errorException.StatusCode.ToString(), type: "AreEqual");
                    Assert.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode);
                    Assert.IsTrue(errorException.Message.Contains("email or password") ||
                                 errorException.Message.Contains("credentials") ||
                                 errorException.Message.Contains("authentication"));
                }
                catch (ArgumentException)
                {
                    sw.Stop();
                    Assert.Fail("Should not throw ArgumentException for valid MFA secret");
                }
                catch (Exception e)
                {
                    sw.Stop();
                    Assert.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
                }
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
        public async System.Threading.Tasks.Task Test010_Should_Generate_TOTP_Token_With_Valid_MfaSecret_Async()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();
                NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
                string validMfaSecret = "JBSWY3DPEHPK3PXP";

                TestReportHelper.LogRequest("client.LoginAsync() with valid MFA secret", "POST",
                    $"https://{_host}/v3/user-session");

                try
                {
                    ContentstackResponse contentstackResponse = await client.LoginAsync(credentials, null, validMfaSecret);
                    sw.Stop();
                }
                catch (ContentstackErrorException errorException)
                {
                    sw.Stop();
                    TestReportHelper.LogAssertion(
                        errorException.StatusCode == HttpStatusCode.UnprocessableEntity,
                        "Status code is UnprocessableEntity",
                        expected: "UnprocessableEntity", actual: errorException.StatusCode.ToString(), type: "AreEqual");
                    Assert.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode);
                    Assert.IsTrue(errorException.Message.Contains("email or password") ||
                                 errorException.Message.Contains("credentials") ||
                                 errorException.Message.Contains("authentication"));
                }
                catch (ArgumentException)
                {
                    sw.Stop();
                    Assert.Fail("Should not throw ArgumentException for valid MFA secret");
                }
                catch (Exception e)
                {
                    sw.Stop();
                    Assert.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
                }
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
        public void Test011_Should_Prefer_Explicit_Token_Over_MfaSecret()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = new ContentstackClient();
                NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
                string validMfaSecret = "JBSWY3DPEHPK3PXP";
                string explicitToken = "123456";

                TestReportHelper.LogRequest("client.Login() explicit token over MFA", "POST",
                    $"https://{_host}/v3/user-session");

                try
                {
                    ContentstackResponse contentstackResponse = client.Login(credentials, explicitToken, validMfaSecret);
                    sw.Stop();
                }
                catch (ContentstackErrorException errorException)
                {
                    sw.Stop();
                    TestReportHelper.LogAssertion(
                        errorException.StatusCode == HttpStatusCode.UnprocessableEntity,
                        "Status code is UnprocessableEntity (credentials rejected)",
                        expected: "UnprocessableEntity", actual: errorException.StatusCode.ToString(), type: "AreEqual");
                    Assert.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode);
                }
                catch (ArgumentException)
                {
                    sw.Stop();
                    Assert.Fail("Should not throw ArgumentException when explicit token is provided");
                }
                catch (Exception e)
                {
                    sw.Stop();
                    Assert.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
                }
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
