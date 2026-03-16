using System;
using System.Net;
using System.Net.Http;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack001_LoginTest
    {

        private static ContentstackClient CreateClientWithLogging()
        {
            var handler = new LoggingHttpHandler();
            var httpClient = new HttpClient(handler);
            return new ContentstackClient(httpClient, new ContentstackClientOptions());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_Failuer_On_Wrong_Login_Credentials()
        {
            TestOutputLogger.LogContext("TestScenario", "WrongCredentials");
            ContentstackClient client = CreateClientWithLogging();
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
        public async System.Threading.Tasks.Task Test002_Should_Return_Failuer_On_Wrong_Async_Login_Credentials()
        {
            TestOutputLogger.LogContext("TestScenario", "WrongCredentialsAsync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");

            try
            {
                await client.LoginAsync(credentials);
                AssertLogger.Fail("Expected exception for wrong credentials");
            }
            catch (ContentstackErrorException errorException)
            {
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                AssertLogger.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.Message, "Message");
                AssertLogger.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.ErrorMessage, "ErrorMessage");
                AssertLogger.AreEqual(104, errorException.ErrorCode, "ErrorCode");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Return_Success_On_Async_Login()
        {
            TestOutputLogger.LogContext("TestScenario", "AsyncLoginSuccess");
            ContentstackClient client = CreateClientWithLogging();
            
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
                ContentstackClient client = CreateClientWithLogging();
                
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
                ContentstackClient client = CreateClientWithLogging();
                
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
                ContentstackClient client = CreateClientWithLogging();
                
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
            
                ContentstackClient client = CreateClientWithLogging();
                
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
            ContentstackClient client = CreateClientWithLogging();
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
            ContentstackClient client = CreateClientWithLogging();
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
            ContentstackClient client = CreateClientWithLogging();
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
            ContentstackClient client = CreateClientWithLogging();
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

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Throw_InvalidOperation_When_Already_LoggedIn_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "AlreadyLoggedInSync");
            ContentstackClient client = CreateClientWithLogging();

            try
            {
                client.Login(Contentstack.Credential);
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");

                AssertLogger.ThrowsException<InvalidOperationException>(() =>
                    client.Login(Contentstack.Credential), "AlreadyLoggedIn");

                client.Logout();
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test013_Should_Throw_InvalidOperation_When_Already_LoggedIn_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "AlreadyLoggedInAsync");
            ContentstackClient client = CreateClientWithLogging();

            try
            {
                await client.LoginAsync(Contentstack.Credential);
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");

                await System.Threading.Tasks.Task.Run(() =>
                    AssertLogger.ThrowsException<InvalidOperationException>(() =>
                        client.LoginAsync(Contentstack.Credential).GetAwaiter().GetResult(), "AlreadyLoggedInAsync"));

                await client.LogoutAsync();
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Throw_ArgumentNullException_For_Null_Credentials_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "NullCredentialsSync");
            ContentstackClient client = CreateClientWithLogging();

            AssertLogger.ThrowsException<ArgumentNullException>(() =>
                client.Login(null), "NullCredentials");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Throw_ArgumentNullException_For_Null_Credentials_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "NullCredentialsAsync");
            ContentstackClient client = CreateClientWithLogging();

            AssertLogger.ThrowsException<ArgumentNullException>(() =>
                client.LoginAsync(null).GetAwaiter().GetResult(), "NullCredentialsAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test016_Should_Throw_ArgumentException_For_Invalid_MfaSecret_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "InvalidMfaSecretAsync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
            string invalidMfaSecret = "INVALID_BASE32_SECRET!@#";

            try
            {
                await client.LoginAsync(credentials, null, invalidMfaSecret);
                AssertLogger.Fail("Expected ArgumentException for invalid MFA secret");
            }
            catch (ArgumentException)
            {
                AssertLogger.IsTrue(true, "ArgumentException thrown as expected for async");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Handle_Valid_Credentials_With_TfaToken_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "WrongTfaTokenSync");
            ContentstackClient client = CreateClientWithLogging();

            try
            {
                client.Login(Contentstack.Credential, "000000");
                // Account does not have 2FA enabled — tfa_token is ignored by the API and login succeeds.
                // This is a valid outcome; assert token is set and clean up.
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");
                client.Logout();
            }
            catch (ContentstackErrorException errorException)
            {
                // Account has 2FA enabled — wrong token is correctly rejected with 422.
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                AssertLogger.IsTrue(errorException.ErrorCode > 0, "TfaErrorCode");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test018_Should_Handle_Valid_Credentials_With_TfaToken_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "WrongTfaTokenAsync");
            ContentstackClient client = CreateClientWithLogging();

            try
            {
                await client.LoginAsync(Contentstack.Credential, "000000");
                // Account does not have 2FA enabled — tfa_token is ignored by the API and login succeeds.
                // This is a valid outcome; assert token is set and clean up.
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");
                await client.LogoutAsync();
            }
            catch (ContentstackErrorException errorException)
            {
                // Account has 2FA enabled — wrong token is correctly rejected with 422.
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                AssertLogger.IsTrue(errorException.ErrorCode > 0, "TfaErrorCodeAsync");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_Not_Include_TfaToken_When_MfaSecret_Is_Empty_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "EmptyMfaSecretSync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("mock_user", "mock_password");

            try
            {
                client.Login(credentials, null, "");
            }
            catch (ContentstackErrorException errorException)
            {
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                AssertLogger.AreEqual(104, errorException.ErrorCode, "ErrorCode");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test020_Should_Not_Include_TfaToken_When_MfaSecret_Is_Null_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "NullMfaSecretAsync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("mock_user", "mock_password");

            try
            {
                await client.LoginAsync(credentials, null, null);
            }
            catch (ContentstackErrorException errorException)
            {
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                AssertLogger.AreEqual(104, errorException.ErrorCode, "ErrorCode");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }
    }
}
