using System;
using System.Net;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
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
        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_Failuer_On_Wrong_Login_Credentials()
        {
            TestOutputLogger.LogContext("TestScenario", "WrongCredentials");
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");
            
            try
            {
                ContentstackResponse contentstackResponse = client.Login(credentials);
            } catch (Exception e)
            {
                ContentstackErrorException errorException = e as ContentstackErrorException;
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                AssertLogger.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.Message, "Message");
                AssertLogger.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.ErrorMessage, "ErrorMessage");
                AssertLogger.AreEqual(104, errorException.ErrorCode, "ErrorCode");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Return_Failuer_On_Wrong_Async_Login_Credentials()
        {
            TestOutputLogger.LogContext("TestScenario", "WrongCredentialsAsync");
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");
            var response = client.LoginAsync(credentials);

            response.ContinueWith((t) =>
            {
                if (t.IsCompleted && t.Status == System.Threading.Tasks.TaskStatus.Faulted)
                {
                    ContentstackErrorException errorException = t.Exception.InnerException as ContentstackErrorException;
                    AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                    AssertLogger.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.Message, "Message");
                    AssertLogger.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.ErrorMessage, "ErrorMessage");
                    AssertLogger.AreEqual(104, errorException.ErrorCode, "ErrorCode");
                }
            });
            Thread.Sleep(3000);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Return_Success_On_Async_Login()
        {
            TestOutputLogger.LogContext("TestScenario", "AsyncLoginSuccess");
            ContentstackClient client = new ContentstackClient();
            
            try
            {
                ContentstackResponse contentstackResponse =  await client.LoginAsync(Contentstack.Credential);
                string loginResponse = contentstackResponse.OpenResponse();

                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");
                AssertLogger.IsNotNull(loginResponse, "loginResponse");
                
                await client.LogoutAsync();
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Return_Success_On_Login()
        {
            TestOutputLogger.LogContext("TestScenario", "SyncLoginSuccess");
            try
            {
                ContentstackClient client = new ContentstackClient();
                
                ContentstackResponse contentstackResponse = client.Login(Contentstack.Credential);
                string loginResponse = contentstackResponse.OpenResponse();

                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");
                AssertLogger.IsNotNull(loginResponse, "loginResponse");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Return_Loggedin_User()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUser");
            try
            {
                ContentstackClient client = new ContentstackClient();
                
                client.Login(Contentstack.Credential);
                
                ContentstackResponse response = client.GetUser();

                var user = response.OpenJObjectResponse();

                AssertLogger.IsNotNull(user, "user");

            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test006_Should_Return_Loggedin_User_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserAsync");
            try
            {
                ContentstackClient client = new ContentstackClient();
                
                await client.LoginAsync(Contentstack.Credential);
                
                ContentstackResponse response = await client.GetUserAsync();

                var user = response.OpenJObjectResponse();

                AssertLogger.IsNotNull(user, "user");
                AssertLogger.IsNotNull(user["user"]["organizations"], "organizations");
                AssertLogger.IsInstanceOfType(user["user"]["organizations"], typeof(JArray), "organizations");
                AssertLogger.IsNull(user["user"]["organizations"][0]["org_roles"], "org_roles");

            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Return_Loggedin_User_With_Organizations_detail()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserWithOrgRoles");
            try
            {
                ParameterCollection collection = new ParameterCollection();
                collection.Add("include_orgs_roles", true);
            
                ContentstackClient client = new ContentstackClient();
                
                client.Login(Contentstack.Credential);
                
                ContentstackResponse response = client.GetUser(collection);

                var user = response.OpenJObjectResponse();

                AssertLogger.IsNotNull(user, "user");
                AssertLogger.IsNotNull(user["user"]["organizations"], "organizations");
                AssertLogger.IsInstanceOfType(user["user"]["organizations"], typeof(JArray), "organizations");
                AssertLogger.IsNotNull(user["user"]["organizations"][0]["org_roles"], "org_roles");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Fail_Login_With_Invalid_MfaSecret()
        {
            TestOutputLogger.LogContext("TestScenario", "InvalidMfaSecret");
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
            string invalidMfaSecret = "INVALID_BASE32_SECRET!@#";
            
            try
            {
                ContentstackResponse contentstackResponse = client.Login(credentials, null, invalidMfaSecret);
                AssertLogger.Fail("Expected exception for invalid MFA secret");
            }
            catch (ArgumentException)
            {
                AssertLogger.IsTrue(true, "ArgumentException thrown as expected");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Generate_TOTP_Token_With_Valid_MfaSecret()
        {
            TestOutputLogger.LogContext("TestScenario", "ValidMfaSecret");
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
            string validMfaSecret = "JBSWY3DPEHPK3PXP";
            
            try
            {
                ContentstackResponse contentstackResponse = client.Login(credentials, null, validMfaSecret);
            }
            catch (ContentstackErrorException errorException)
            {
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                AssertLogger.IsTrue(errorException.Message.Contains("email or password") || 
                             errorException.Message.Contains("credentials") ||
                             errorException.Message.Contains("authentication"), "MFA error message check");
            }
            catch (ArgumentException)
            {
                AssertLogger.Fail("Should not throw ArgumentException for valid MFA secret");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test010_Should_Generate_TOTP_Token_With_Valid_MfaSecret_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "ValidMfaSecretAsync");
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
            string validMfaSecret = "JBSWY3DPEHPK3PXP";
            
            try
            {
                ContentstackResponse contentstackResponse = await client.LoginAsync(credentials, null, validMfaSecret);
            }
            catch (ContentstackErrorException errorException)
            {
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                AssertLogger.IsTrue(errorException.Message.Contains("email or password") || 
                             errorException.Message.Contains("credentials") ||
                             errorException.Message.Contains("authentication"), "MFA error message check");
            }
            catch (ArgumentException)
            {
                AssertLogger.Fail("Should not throw ArgumentException for valid MFA secret");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Prefer_Explicit_Token_Over_MfaSecret()
        {
            TestOutputLogger.LogContext("TestScenario", "ExplicitTokenOverMfa");
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
            string validMfaSecret = "JBSWY3DPEHPK3PXP";
            string explicitToken = "123456";
            
            try
            {
                ContentstackResponse contentstackResponse = client.Login(credentials, explicitToken, validMfaSecret);
            }
            catch (ContentstackErrorException errorException)
            {
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
            }
            catch (ArgumentException)
            {
                AssertLogger.Fail("Should not throw ArgumentException when explicit token is provided");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }
    }
}
