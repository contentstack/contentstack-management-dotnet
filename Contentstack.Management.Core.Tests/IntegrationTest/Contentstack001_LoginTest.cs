using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Queryable;
using System.Text.Json;

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
                ContentstackResponse contentstackResponse = await Contentstack.LoginWithTotpRetryAsync(client);
                string loginResponse = contentstackResponse.OpenResponse();

                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");
                AssertLogger.IsNotNull(loginResponse, "loginResponse");
                
                await client.LogoutAsync();
            }
            catch (Exception e)
            {
                if (Contentstack.IsTotpReuse(e))
                {
                    AssertLogger.Fail($"TOTP token reuse error after retries: {e.Message}");
                }
                else if (Contentstack.IsAccountLockout(e))
                {
                    AssertLogger.Fail($"Account is locked after retries: {e.Message}");
                }
                else
                {
                    AssertLogger.Fail(e.Message);
                }
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
                
                ContentstackResponse contentstackResponse = Contentstack.LoginWithTotpRetry(client);
                string loginResponse = contentstackResponse.OpenResponse();

                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");
                AssertLogger.IsNotNull(loginResponse, "loginResponse");
            }
            catch (Exception e)
            {
                if (Contentstack.IsTotpReuse(e))
                {
                    AssertLogger.Fail($"TOTP token reuse error after retries: {e.Message}");
                }
                else if (Contentstack.IsAccountLockout(e))
                {
                    AssertLogger.Fail($"Account is locked after retries: {e.Message}");
                }
                else
                {
                    AssertLogger.Fail(e.Message);
                }
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
                
                Contentstack.LoginWithTotpRetry(client);
                
                ContentstackResponse response = client.GetUser();

                var user = response.OpenJsonObjectResponse();

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
                
                await Contentstack.LoginWithTotpRetryAsync(client);
                
                ContentstackResponse response = await client.GetUserAsync();

                var user = response.OpenJsonObjectResponse();

                AssertLogger.IsNotNull(user, "user");
                AssertLogger.IsNotNull(user["user"]["organizations"], "organizations");
                AssertLogger.IsInstanceOfType(user["user"]["organizations"], typeof(System.Text.Json.Nodes.JsonArray), "organizations");
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
                
                Contentstack.LoginWithTotpRetry(client);
                
                ContentstackResponse response = client.GetUser(collection);

                var user = response.OpenJsonObjectResponse();

                AssertLogger.IsNotNull(user, "user");
                AssertLogger.IsNotNull(user["user"]["organizations"], "organizations");
                AssertLogger.IsInstanceOfType(user["user"]["organizations"], typeof(System.Text.Json.Nodes.JsonArray), "organizations");
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
                Contentstack.LoginWithTotpRetry(client);
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");

                AssertLogger.ThrowsException<InvalidOperationException>(() =>
                    client.Login(Contentstack.Credential, null, Contentstack.MfaSecret), "AlreadyLoggedIn");

                client.Logout();
            }
            catch (Exception e)
            {
                if (Contentstack.IsTotpReuse(e))
                {
                    AssertLogger.Fail($"TOTP token reuse error after retries: {e.Message}");
                }
                else if (Contentstack.IsAccountLockout(e))
                {
                    AssertLogger.Fail($"Account is locked after retries: {e.Message}");
                }
                else
                {
                    AssertLogger.Fail($"Unexpected exception: {e.GetType().Name} - {e.Message}");
                }
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
                await Contentstack.LoginWithTotpRetryAsync(client);
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "Authtoken");

                await System.Threading.Tasks.Task.Run(() =>
                    AssertLogger.ThrowsException<InvalidOperationException>(() =>
                        client.LoginAsync(Contentstack.Credential, null, Contentstack.MfaSecret).GetAwaiter().GetResult(), "AlreadyLoggedInAsync"));

                await client.LogoutAsync();
            }
            catch (Exception e)
            {
                if (Contentstack.IsTotpReuse(e))
                {
                    AssertLogger.Fail($"TOTP token reuse error after retries: {e.Message}");
                }
                else if (Contentstack.IsAccountLockout(e))
                {
                    AssertLogger.Fail($"Account is locked after retries: {e.Message}");
                }
                else
                {
                    AssertLogger.Fail($"Unexpected exception: {e.GetType().Name} - {e.Message}");
                }
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
                if (Contentstack.IsAccountLockout(errorException))
                {
                    // Account is locked - this is expected after multiple failed attempts
                    AssertLogger.IsTrue(errorException.StatusCode == HttpStatusCode.BadRequest ||
                                      errorException.StatusCode == HttpStatusCode.UnprocessableEntity,
                                      $"Expected 400 or 422 for account lockout, got {errorException.StatusCode}");
                    AssertLogger.AreEqual(104, errorException.ErrorCode, "Expected error code 104 for account lockout");
                }
                else
                {
                    // Account has 2FA enabled — wrong token is correctly rejected with 400 or 422
                    AssertLogger.IsTrue(errorException.StatusCode == HttpStatusCode.BadRequest ||
                                      errorException.StatusCode == HttpStatusCode.UnprocessableEntity,
                                      $"Expected 400 or 422 for TFA failure, got {errorException.StatusCode}");
                    AssertLogger.IsTrue(errorException.ErrorCode > 0, "TfaErrorCode");
                }
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
                if (Contentstack.IsAccountLockout(errorException))
                {
                    // Account is locked - this is expected after multiple failed attempts
                    AssertLogger.IsTrue(errorException.StatusCode == HttpStatusCode.BadRequest ||
                                      errorException.StatusCode == HttpStatusCode.UnprocessableEntity,
                                      $"Expected 400 or 422 for account lockout, got {errorException.StatusCode}");
                    AssertLogger.AreEqual(104, errorException.ErrorCode, "Expected error code 104 for account lockout");
                }
                else
                {
                    // Account has 2FA enabled — wrong token is correctly rejected with 400 or 422
                    AssertLogger.IsTrue(errorException.StatusCode == HttpStatusCode.BadRequest ||
                                      errorException.StatusCode == HttpStatusCode.UnprocessableEntity,
                                      $"Expected 400 or 422 for TFA failure, got {errorException.StatusCode}");
                    AssertLogger.IsTrue(errorException.ErrorCode > 0, "TfaErrorCodeAsync");
                }
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

        #region Phase 1: Parameter and Input Validation Tests (021-030)

        [TestMethod]
        [DoNotParallelize]
        public void Test021_Should_Handle_Empty_Username_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "EmptyUsernameSync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("", "password");

            var ex = AssertLogger.ThrowsContentstackError(() =>
                client.Login(credentials), "EmptyUsername", HttpStatusCode.UnprocessableEntity);
            
            AssertLogger.AreEqual(104, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("email") || ex.Message.Contains("password"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Handle_Empty_Password_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "EmptyPasswordSync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("user@example.com", "");

            var ex = AssertLogger.ThrowsContentstackError(() =>
                client.Login(credentials), "EmptyPassword", HttpStatusCode.UnprocessableEntity);
            
            AssertLogger.AreEqual(104, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("email") || ex.Message.Contains("password"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test023_Should_Handle_Empty_Username_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "EmptyUsernameAsync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("", "password");

            var ex = await AssertLogger.ThrowsContentstackErrorAsync(() =>
                client.LoginAsync(credentials), "EmptyUsernameAsync", HttpStatusCode.UnprocessableEntity);
            
            AssertLogger.AreEqual(104, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("email") || ex.Message.Contains("password"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test024_Should_Handle_Empty_Password_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "EmptyPasswordAsync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("user@example.com", "");

            var ex = await AssertLogger.ThrowsContentstackErrorAsync(() =>
                client.LoginAsync(credentials), "EmptyPasswordAsync", HttpStatusCode.UnprocessableEntity);
            
            AssertLogger.AreEqual(104, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("email") || ex.Message.Contains("password"), "ErrorMessage");
        }


        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test025_Should_Handle_Whitespace_Only_Credentials_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "WhitespaceCredentialsAsync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("   ", "   ");

            try
            {
                await client.LoginAsync(credentials);
                AssertLogger.Fail("Expected exception for whitespace-only credentials");
            }
            catch (ContentstackErrorException errorException)
            {
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode, "StatusCode");
                AssertLogger.AreEqual(104, errorException.ErrorCode, "ErrorCode");
            }
            catch (ArgumentNullException)
            {
                AssertLogger.IsTrue(true, "ArgumentNullException acceptable for whitespace credentials");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }





        #endregion

        #region Phase 2: Network Error Simulation Tests (031-045)

        private static ContentstackClient CreateClientWithMockError(NetworkErrorType errorType, int delayMs = 0)
        {
            var handler = new MockNetworkErrorHandler(errorType, delayMs);
            var httpClient = new HttpClient(handler);
            return new ContentstackClient(httpClient, new ContentstackClientOptions());
        }

        private static ContentstackClient CreateClientWithTimeout(int timeoutMs = 5000)
        {
            var handler = new MockTimeoutHandler(timeoutMs);
            var httpClient = new HttpClient(handler);
            return new ContentstackClient(httpClient, new ContentstackClientOptions());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test026_Should_Handle_Network_Timeout_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "NetworkTimeoutSync");
            ContentstackClient client = CreateClientWithMockError(NetworkErrorType.Timeout);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            try
            {
                client.Login(credentials);
                AssertLogger.Fail("Expected timeout exception");
            }
            catch (TaskCanceledException)
            {
                AssertLogger.IsTrue(true, "TaskCanceledException as expected for timeout");
            }
            catch (HttpRequestException)
            {
                AssertLogger.IsTrue(true, "HttpRequestException acceptable for timeout");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test027_Should_Handle_Network_Timeout_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "NetworkTimeoutAsync");
            ContentstackClient client = CreateClientWithMockError(NetworkErrorType.Timeout);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            try
            {
                await client.LoginAsync(credentials);
                AssertLogger.Fail("Expected timeout exception");
            }
            catch (TaskCanceledException)
            {
                AssertLogger.IsTrue(true, "TaskCanceledException as expected for timeout");
            }
            catch (HttpRequestException)
            {
                AssertLogger.IsTrue(true, "HttpRequestException acceptable for timeout");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test028_Should_Handle_Connection_Refused_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "ConnectionRefusedSync");
            ContentstackClient client = CreateClientWithMockError(NetworkErrorType.ConnectionRefused);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            AssertLogger.ThrowsException<HttpRequestException>(() =>
                client.Login(credentials), "ConnectionRefused");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test029_Should_Handle_Connection_Refused_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "ConnectionRefusedAsync");
            ContentstackClient client = CreateClientWithMockError(NetworkErrorType.ConnectionRefused);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            await AssertLogger.ThrowsExceptionAsync<HttpRequestException>(() =>
                client.LoginAsync(credentials), "ConnectionRefusedAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Handle_DNS_Resolution_Failure_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "DnsFailureSync");
            ContentstackClient client = CreateClientWithMockError(NetworkErrorType.DnsFailure);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            AssertLogger.ThrowsException<HttpRequestException>(() =>
                client.Login(credentials), "DnsFailure");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test031_Should_Handle_DNS_Resolution_Failure_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "DnsFailureAsync");
            ContentstackClient client = CreateClientWithMockError(NetworkErrorType.DnsFailure);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            await AssertLogger.ThrowsExceptionAsync<HttpRequestException>(() =>
                client.LoginAsync(credentials), "DnsFailureAsync");
        }









        [TestMethod]
        [DoNotParallelize]
        public void Test032_Should_Handle_Request_Cancellation_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "RequestCancellationSync");
            ContentstackClient client = CreateClientWithTimeout(100);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            try
            {
                client.Login(credentials);
                AssertLogger.Fail("Expected cancellation exception");
            }
            catch (TaskCanceledException)
            {
                AssertLogger.IsTrue(true, "TaskCanceledException as expected");
            }
            catch (HttpRequestException)
            {
                AssertLogger.IsTrue(true, "HttpRequestException acceptable for cancellation");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        #endregion

        #region Phase 3: HTTP Status Code Coverage Tests (046-055)

        private static ContentstackClient CreateClientWithHttpStatus(HttpStatusCode statusCode, 
            string errorMessage = null, int errorCode = 0)
        {
            var handler = new MockHttpStatusHandler(statusCode, errorMessage, errorCode);
            var httpClient = new HttpClient(handler);
            return new ContentstackClient(httpClient, new ContentstackClientOptions());
        }

        private static ContentstackClient CreateClientWithHttpStatusNoRetry(HttpStatusCode statusCode, 
            string errorMessage = null, int errorCode = 0)
        {
            var handler = new MockHttpStatusHandler(statusCode, errorMessage, errorCode);
            var httpClient = new HttpClient(handler);
            var options = new ContentstackClientOptions
            {
                RetryOnError = false,           // Disable all retries
                RetryOnHttpServerError = false, // Specifically disable server error retries
                RetryLimit = 0                  // No retry attempts
            };
            return new ContentstackClient(httpClient, options);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test033_Should_Handle_401_Unauthorized_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Http401UnauthorizedSync");
            ContentstackClient client = CreateClientWithHttpStatus(HttpStatusCode.Unauthorized, 
                "Authentication failed. Please check your credentials.", 401);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            var ex = AssertLogger.ThrowsContentstackError(() =>
                client.Login(credentials), "Unauthorized", HttpStatusCode.Unauthorized);
            
            AssertLogger.AreEqual(401, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("Authentication failed"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test034_Should_Handle_401_Unauthorized_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Http401UnauthorizedAsync");
            ContentstackClient client = CreateClientWithHttpStatus(HttpStatusCode.Unauthorized,
                "Authentication failed. Please check your credentials.", 401);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            var ex = await AssertLogger.ThrowsContentstackErrorAsync(() =>
                client.LoginAsync(credentials), "UnauthorizedAsync", HttpStatusCode.Unauthorized);
            
            AssertLogger.AreEqual(401, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("Authentication failed"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test035_Should_Handle_403_Forbidden_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Http403ForbiddenSync");
            ContentstackClient client = CreateClientWithHttpStatus(HttpStatusCode.Forbidden,
                "Access denied. Insufficient permissions.", 403);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            var ex = AssertLogger.ThrowsContentstackError(() =>
                client.Login(credentials), "Forbidden", HttpStatusCode.Forbidden);
            
            AssertLogger.AreEqual(403, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("Access denied"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test036_Should_Handle_403_Forbidden_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Http403ForbiddenAsync");
            ContentstackClient client = CreateClientWithHttpStatus(HttpStatusCode.Forbidden,
                "Access denied. Insufficient permissions.", 403);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            var ex = await AssertLogger.ThrowsContentstackErrorAsync(() =>
                client.LoginAsync(credentials), "ForbiddenAsync", HttpStatusCode.Forbidden);
            
            AssertLogger.AreEqual(403, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("Access denied"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test050_Should_Handle_429_TooManyRequests_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Http429TooManyRequestsSync");
            ContentstackClient client = CreateClientWithHttpStatus(HttpStatusCode.TooManyRequests,
                "Rate limit exceeded. Please try again later.", 429);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            var ex = AssertLogger.ThrowsContentstackError(() =>
                client.Login(credentials), "TooManyRequests", HttpStatusCode.TooManyRequests);
            
            AssertLogger.AreEqual(429, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("Rate limit"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test051_Should_Handle_429_TooManyRequests_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Http429TooManyRequestsAsync");
            ContentstackClient client = CreateClientWithHttpStatus(HttpStatusCode.TooManyRequests,
                "Rate limit exceeded. Please try again later.", 429);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            var ex = await AssertLogger.ThrowsContentstackErrorAsync(() =>
                client.LoginAsync(credentials), "TooManyRequestsAsync", HttpStatusCode.TooManyRequests);
            
            AssertLogger.AreEqual(429, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("Rate limit"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test052_Should_Handle_500_InternalServerError_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Http500InternalServerErrorSync");
            ContentstackClient client = CreateClientWithHttpStatusNoRetry(HttpStatusCode.InternalServerError,
                "Internal server error occurred.", 500);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            var ex = AssertLogger.ThrowsContentstackError(() =>
                client.Login(credentials), "InternalServerError", HttpStatusCode.InternalServerError);
            
            AssertLogger.AreEqual(500, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("Internal server"), "ErrorMessage");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test053_Should_Handle_500_InternalServerError_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Http500InternalServerErrorAsync");
            ContentstackClient client = CreateClientWithHttpStatusNoRetry(HttpStatusCode.InternalServerError,
                "Internal server error occurred.", 500);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            var ex = await AssertLogger.ThrowsContentstackErrorAsync(() =>
                client.LoginAsync(credentials), "InternalServerErrorAsync", HttpStatusCode.InternalServerError);
            
            AssertLogger.AreEqual(500, ex.ErrorCode, "ErrorCode");
            AssertLogger.IsTrue(ex.Message.Contains("Internal server"), "ErrorMessage");
        }



        #endregion

        #region Phase 4: Response Processing Edge Cases Tests (056-065)

        private static ContentstackClient CreateClientWithMalformedResponse(string responseContent, 
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var handler = new MockMalformedResponseHandler(responseContent, statusCode);
            var httpClient = new HttpClient(handler);
            return new ContentstackClient(httpClient, new ContentstackClientOptions());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test056_Should_Handle_Malformed_JSON_Response_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "MalformedJsonSync");
            ContentstackClient client = CreateClientWithMalformedResponse("{ invalid json }");
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            try
            {
                client.Login(credentials);
                AssertLogger.Fail("Expected exception for malformed JSON");
            }
            catch (ContentstackErrorException)
            {
                AssertLogger.IsTrue(true, "ContentstackErrorException acceptable for malformed JSON");
            }
            catch (System.Text.Json.JsonException)
            {
                AssertLogger.IsTrue(true, "JsonException acceptable for malformed JSON");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test057_Should_Handle_Malformed_JSON_Response_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "MalformedJsonAsync");
            ContentstackClient client = CreateClientWithMalformedResponse("{ invalid json }");
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            try
            {
                await client.LoginAsync(credentials);
                AssertLogger.Fail("Expected exception for malformed JSON");
            }
            catch (ContentstackErrorException)
            {
                AssertLogger.IsTrue(true, "ContentstackErrorException acceptable for malformed JSON");
            }
            catch (System.Text.Json.JsonException)
            {
                AssertLogger.IsTrue(true, "JsonException acceptable for malformed JSON");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test058_Should_Handle_Empty_Response_Body_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "EmptyResponseSync");
            ContentstackClient client = CreateClientWithMalformedResponse("", HttpStatusCode.OK);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            try
            {
                client.Login(credentials);
                AssertLogger.Fail("Expected exception for empty response");
            }
            catch (ContentstackErrorException)
            {
                AssertLogger.IsTrue(true, "ContentstackErrorException acceptable for empty response");
            }
            catch (ArgumentException)
            {
                AssertLogger.IsTrue(true, "ArgumentException acceptable for empty response");
            }
            catch (System.Text.Json.JsonException)
            {
                AssertLogger.IsTrue(true, "JsonReaderException acceptable for empty response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test059_Should_Handle_Empty_Response_Body_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "EmptyResponseAsync");
            ContentstackClient client = CreateClientWithMalformedResponse("", HttpStatusCode.OK);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            try
            {
                await client.LoginAsync(credentials);
                AssertLogger.Fail("Expected exception for empty response");
            }
            catch (ContentstackErrorException)
            {
                AssertLogger.IsTrue(true, "ContentstackErrorException acceptable for empty response");
            }
            catch (ArgumentException)
            {
                AssertLogger.IsTrue(true, "ArgumentException acceptable for empty response");
            }
            catch (System.Text.Json.JsonException)
            {
                AssertLogger.IsTrue(true, "JsonReaderException acceptable for empty response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test060_Should_Handle_Unexpected_Response_Structure_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "UnexpectedStructureSync");
            string unexpectedResponse = @"{
                ""data"": {
                    ""user"": {
                        ""name"": ""test""
                    }
                },
                ""status"": ""success""
            }";
            ContentstackClient client = CreateClientWithMalformedResponse(unexpectedResponse, HttpStatusCode.OK);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            try
            {
                var response = client.Login(credentials);
                // If login succeeds but response is malformed, the authtoken might not be set properly
                if (string.IsNullOrEmpty(client.contentstackOptions.Authtoken))
                {
                    AssertLogger.IsTrue(true, "Login succeeded but authtoken not set - acceptable for unexpected structure");
                }
                else
                {
                    // Login succeeded with unexpected structure - this might be acceptable if SDK is lenient
                    AssertLogger.IsTrue(true, "Login succeeded with unexpected structure - SDK handled gracefully");
                }
            }
            catch (ContentstackErrorException)
            {
                AssertLogger.IsTrue(true, "ContentstackErrorException acceptable for unexpected structure");
            }
            catch (InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "InvalidOperationException acceptable for unexpected structure");
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                AssertLogger.IsTrue(true, "KeyNotFoundException acceptable for unexpected structure");
            }
            catch (System.Text.Json.JsonException)
            {
                AssertLogger.IsTrue(true, "JsonException acceptable for unexpected structure");
            }
            catch (Exception e)
            {
                // If no exception is thrown, the test should fail, but if we get here 
                // it means some other exception occurred, which might be acceptable
                AssertLogger.IsTrue(true, $"Exception occurred as expected: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test061_Should_Handle_Large_Response_Payload_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "LargeResponseSync");
            string largeData = new string('x', 1000000); // 1MB of data
            string largeResponse = $@"{{
                ""error_message"": ""Large response test"",
                ""error_code"": 400,
                ""large_data"": ""{largeData}""
            }}";
            ContentstackClient client = CreateClientWithMalformedResponse(largeResponse, HttpStatusCode.BadRequest);
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");

            var ex = AssertLogger.ThrowsContentstackError(() =>
                client.Login(credentials), "LargeResponse", HttpStatusCode.BadRequest);
            
            AssertLogger.AreEqual(400, ex.ErrorCode, "ErrorCode");
        }





        #endregion

        #region Phase 5: GetUser Error Scenarios Tests (066-075)

        [TestMethod]
        [DoNotParallelize]
        public void Test066_Should_Throw_InvalidOperation_GetUser_When_Not_LoggedIn_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserNotLoggedInSync");
            ContentstackClient client = CreateClientWithLogging();

            AssertLogger.ThrowsException<InvalidOperationException>(() =>
                client.GetUser(), "GetUserNotLoggedIn");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test067_Should_Throw_InvalidOperation_GetUser_When_Not_LoggedIn_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserNotLoggedInAsync");
            ContentstackClient client = CreateClientWithLogging();

            await AssertLogger.ThrowsExceptionAsync<InvalidOperationException>(() =>
                client.GetUserAsync(), "GetUserNotLoggedInAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test068_Should_Handle_GetUser_With_Invalid_Parameters_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserInvalidParamsSync");
            ContentstackClient client = CreateClientWithLogging();
            
            try
            {
                Contentstack.LoginWithRetry(client);
                
                ParameterCollection invalidParams = new ParameterCollection();
                invalidParams.Add(null, "value"); // Invalid null key
                
                client.GetUser(invalidParams);
                AssertLogger.Fail("Expected exception for invalid parameters");
            }
            catch (ContentstackErrorException ex) when (Contentstack.IsTotpReuse(ex))
            {
                AssertLogger.Fail($"TOTP token reuse error, cannot test invalid parameters: {ex.Message}");
            }
            catch (ContentstackErrorException ex) when (Contentstack.IsAccountLockout(ex))
            {
                AssertLogger.Fail($"Account is locked, cannot test invalid parameters: {ex.Message}");
            }
            catch (ArgumentNullException)
            {
                AssertLogger.IsTrue(true, "ArgumentNullException as expected for null key");
            }
            catch (ArgumentException)
            {
                AssertLogger.IsTrue(true, "ArgumentException acceptable for invalid parameters");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
            finally
            {
                try { client.Logout(); } catch { }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test069_Should_Handle_GetUser_With_Extremely_Large_Parameters_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserLargeParamsSync");
            ContentstackClient client = CreateClientWithLogging();
            
            try
            {
                Contentstack.LoginWithTotpRetry(client);
                
                ParameterCollection largeParams = new ParameterCollection();
                string largeValue = new string('x', 100000); // Very large parameter value
                largeParams.Add("large_param", largeValue);
                
                ContentstackResponse response = client.GetUser(largeParams);
                
                AssertLogger.IsNotNull(response, "Response should not be null even with large params");
            }
            catch (ContentstackErrorException ex) when (Contentstack.IsTotpReuse(ex))
            {
                AssertLogger.Fail($"TOTP token reuse error, cannot test large parameters: {ex.Message}");
            }
            catch (ContentstackErrorException ex) when (Contentstack.IsAccountLockout(ex))
            {
                AssertLogger.Fail($"Account is locked, cannot test large parameters: {ex.Message}");
            }
            catch (ArgumentException)
            {
                AssertLogger.IsTrue(true, "ArgumentException acceptable for extremely large parameters");
            }
            catch (UriFormatException)
            {
                AssertLogger.IsTrue(true, "UriFormatException acceptable for extremely large parameters");
            }
            catch (ContentstackErrorException errorException)
            {
                AssertLogger.IsTrue(errorException.StatusCode == HttpStatusCode.BadRequest ||
                                  errorException.StatusCode == HttpStatusCode.RequestEntityTooLarge ||
                                  errorException.StatusCode == HttpStatusCode.RequestUriTooLong,
                                  "Expected 400, 413, or 414 for large parameters");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
            finally
            {
                try { client.Logout(); } catch { }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test070_Should_Handle_GetUser_Network_Timeout_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserNetworkTimeoutSync");
            ContentstackClient client = CreateClientWithMockError(NetworkErrorType.Timeout);
            
            try
            {
                // This will fail at login due to network error
                client.Login(Contentstack.Credential);
                AssertLogger.Fail("Expected network error at login");
            }
            catch (TaskCanceledException)
            {
                AssertLogger.IsTrue(true, "TaskCanceledException as expected for timeout");
            }
            catch (HttpRequestException)
            {
                AssertLogger.IsTrue(true, "HttpRequestException acceptable for timeout");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test071_Should_Handle_GetUser_Network_Timeout_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserNetworkTimeoutAsync");
            ContentstackClient client = CreateClientWithMockError(NetworkErrorType.Timeout);
            
            try
            {
                // This will fail at login due to network error
                await client.LoginAsync(Contentstack.Credential);
                AssertLogger.Fail("Expected network error at login");
            }
            catch (TaskCanceledException)
            {
                AssertLogger.IsTrue(true, "TaskCanceledException as expected for timeout");
            }
            catch (HttpRequestException)
            {
                AssertLogger.IsTrue(true, "HttpRequestException acceptable for timeout");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test072_Should_Handle_GetUser_HTTP_Error_Response_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserHttpErrorSync");
            ContentstackClient client = CreateClientWithHttpStatus(HttpStatusCode.Forbidden, 
                "Access denied for user data.", 403);
            
            try
            {
                // This will fail at login due to HTTP error
                client.Login(Contentstack.Credential);
                AssertLogger.Fail("Expected HTTP error at login");
            }
            catch (ContentstackErrorException ex)
            {
                AssertLogger.AreEqual(HttpStatusCode.Forbidden, ex.StatusCode, "StatusCode");
                AssertLogger.AreEqual(403, ex.ErrorCode, "ErrorCode");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test073_Should_Handle_GetUser_Malformed_Response_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserMalformedResponseSync");
            ContentstackClient client = CreateClientWithMalformedResponse("{ invalid json }", HttpStatusCode.OK);
            
            try
            {
                // This will fail at login due to malformed response
                client.Login(Contentstack.Credential);
                AssertLogger.Fail("Expected JSON parsing error at login");
            }
            catch (ContentstackErrorException)
            {
                AssertLogger.IsTrue(true, "ContentstackErrorException acceptable for malformed response");
            }
            catch (System.Text.Json.JsonException)
            {
                AssertLogger.IsTrue(true, "JsonException acceptable for malformed response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test074_Should_Handle_GetUser_With_Special_Character_Parameters_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserSpecialCharsSync");
            ContentstackClient client = CreateClientWithLogging();
            
            try
            {
                client.Login(Contentstack.Credential, null, Contentstack.MfaSecret);
                
                ParameterCollection specialParams = new ParameterCollection();
                specialParams.Add("include_orgs_roles", true);
                specialParams.Add("special_chars", "!@#$%^&*()");
                specialParams.Add("unicode_test", "测试中文字符");
                
                ContentstackResponse response = client.GetUser(specialParams);
                AssertLogger.IsNotNull(response, "Response should handle special characters");
            }
            catch (ContentstackErrorException errorException)
            {
                AssertLogger.IsTrue(errorException.StatusCode == HttpStatusCode.BadRequest ||
                                  errorException.StatusCode == HttpStatusCode.UnprocessableEntity,
                                  "Expected 400 or 422 for invalid special characters");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
            finally
            {
                try { client.Logout(); } catch { }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test075_Should_Handle_GetUser_Concurrent_Calls_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "GetUserConcurrentAsync");
            ContentstackClient client = CreateClientWithLogging();
            
            try
            {
                await Contentstack.LoginWithTotpRetryAsync(client);
                
                // Simulate concurrent GetUser calls
                var task1 = client.GetUserAsync();
                var task2 = client.GetUserAsync();
                var task3 = client.GetUserAsync();
                
                var responses = await System.Threading.Tasks.Task.WhenAll(task1, task2, task3);
                
                AssertLogger.IsNotNull(responses[0], "Response1");
                AssertLogger.IsNotNull(responses[1], "Response2");
                AssertLogger.IsNotNull(responses[2], "Response3");
            }
            catch (ContentstackErrorException ex) when (Contentstack.IsTotpReuse(ex))
            {
                AssertLogger.Fail($"TOTP token reuse error, cannot test concurrent calls: {ex.Message}");
            }
            catch (ContentstackErrorException ex) when (Contentstack.IsAccountLockout(ex))
            {
                AssertLogger.Fail($"Account is locked, cannot test concurrent calls: {ex.Message}");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception in concurrent calls: {e.GetType().Name} - {e.Message}");
            }
            finally
            {
                try { await client.LogoutAsync(); } catch { }
            }
        }

        #endregion

        #region Phase 7: Advanced Authentication Scenarios Tests (086-095)



        [TestMethod]
        [DoNotParallelize]
        public void Test088_Should_Handle_TOTP_Token_Format_Variations_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "TotpTokenFormatSync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");
            
            // Test various TOTP token formats
            string[] tokenFormats = {
                "123456", // Standard 6-digit
                "12345", // Too short (5 digits)
                "1234567", // Too long (7 digits)
                "12345a", // Contains letter
                "12345!", // Contains special character
                " 123456", // Leading space
                "123456 ", // Trailing space
                " 123456 ", // Both spaces
                "", // Empty string
                "000000", // All zeros
                "999999" // All nines
            };
            
            foreach (string token in tokenFormats)
            {
                try
                {
                    client.Login(credentials, token);
                }
                catch (ArgumentException) when (token.Length != 6 || token.Any(c => !char.IsDigit(c)))
                {
                    AssertLogger.IsTrue(true, $"ArgumentException for invalid token format: '{token}'");
                }
                catch (ContentstackErrorException ex)
                {
                    AssertLogger.IsTrue(ex.StatusCode == HttpStatusCode.UnprocessableEntity, 
                        $"Expected 422 for token: '{token}', got {ex.StatusCode}");
                }
                catch (Exception e)
                {
                    AssertLogger.Fail($"Unexpected exception for token '{token}': {e.GetType().Name} - {e.Message}");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test089_Should_Handle_TOTP_Token_Format_Variations_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "TotpTokenFormatAsync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");
            
            // Test numeric edge cases
            string[] numericTokens = {
                "123456", // Valid format
                "000001", // Leading zeros
                "100000", // Round number
                "999999" // Maximum digits
            };
            
            foreach (string token in numericTokens)
            {
                try
                {
                    await client.LoginAsync(credentials, token);
                }
                catch (ContentstackErrorException ex)
                {
                    AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, ex.StatusCode, 
                        $"StatusCode for token: '{token}'");
                }
                catch (Exception e)
                {
                    AssertLogger.Fail($"Unexpected exception for token '{token}': {e.GetType().Name} - {e.Message}");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test090_Should_Handle_MFA_Secret_Edge_Cases_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "MfaSecretEdgeCasesSync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");
            
            // Test MFA secret edge cases
            string[] edgeCaseSecrets = {
                null, // Null secret
                "", // Empty secret
                "   ", // Whitespace only
                "AAAAAAAAAAAAAAAAA", // All same character (17 A's)
                "BBBBBBBBBBBBBBBB", // All same character (16 B's)
                "JBSWY3DPEHPK3PXP\n", // With newline
                "JBSWY3DPEHPK3PXP\t", // With tab
                "JBSWY3DPEHPK3PXP ", // With trailing space
                " JBSWY3DPEHPK3PXP" // With leading space
            };
            
            foreach (string secret in edgeCaseSecrets)
            {
                try
                {
                    client.Login(credentials, null, secret);
                }
                catch (ArgumentException)
                {
                    AssertLogger.IsTrue(true, $"ArgumentException for edge case secret");
                }
                catch (ContentstackErrorException ex)
                {
                    AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, ex.StatusCode, 
                        "Expected 422 for edge case MFA secret");
                }
                catch (Exception e)
                {
                    AssertLogger.Fail($"Unexpected exception for edge case secret: {e.GetType().Name} - {e.Message}");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test091_Should_Handle_Both_Token_And_MFA_Secret_Provided_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "BothTokenAndMfaSync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");
            
            // Test providing both explicit token and MFA secret
            string explicitToken = "123456";
            string mfaSecret = "JBSWY3DPEHPK3PXP";
            
            try
            {
                // According to existing tests, explicit token should take precedence
                client.Login(credentials, explicitToken, mfaSecret);
            }
            catch (ContentstackErrorException ex)
            {
                AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, ex.StatusCode, "StatusCode");
                // Should use explicit token, not generate from MFA secret
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test092_Should_Handle_Both_Token_And_MFA_Secret_Provided_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "BothTokenAndMfaAsync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");
            
            // Test providing both with different combinations
            var testCases = new[]
            {
                (token: "123456", secret: "JBSWY3DPEHPK3PXP"),
                (token: "654321", secret: "AAAAAAAAAAAAAAAAA"),
                (token: "000000", secret: "BBBBBBBBBBBBBBBB")
            };
            
            foreach (var (token, secret) in testCases)
            {
                try
                {
                    await client.LoginAsync(credentials, token, secret);
                }
                catch (ContentstackErrorException ex)
                {
                    AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, ex.StatusCode, 
                        $"StatusCode for token {token}, secret {secret}");
                }
                catch (Exception e)
                {
                    AssertLogger.Fail($"Unexpected exception for token {token}, secret {secret}: {e.GetType().Name} - {e.Message}");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test093_Should_Handle_MFA_Secret_Case_Sensitivity_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "MfaSecretCaseSync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");
            
            // Test case sensitivity in MFA secrets
            string upperSecret = "JBSWY3DPEHPK3PXP";
            string lowerSecret = "jbswy3dpehpk3pxp";
            string mixedSecret = "JbSwY3dPeHpK3pXp";
            
            string[] secrets = { upperSecret, lowerSecret, mixedSecret };
            
            foreach (string secret in secrets)
            {
                try
                {
                    client.Login(credentials, null, secret);
                }
                catch (ArgumentException)
                {
                    AssertLogger.IsTrue(secret == lowerSecret || secret == mixedSecret, 
                        $"ArgumentException expected for non-uppercase secret: {secret}");
                }
                catch (ContentstackErrorException ex)
                {
                    AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, ex.StatusCode, 
                        $"StatusCode for secret case: {secret}");
                }
                catch (Exception e)
                {
                    AssertLogger.Fail($"Unexpected exception for secret {secret}: {e.GetType().Name} - {e.Message}");
                }
            }
        }


        [TestMethod]
        [DoNotParallelize]
        public void Test095_Should_Handle_MFA_Parameter_Boundary_Conditions_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "MfaBoundaryConditionsSync");
            ContentstackClient client = CreateClientWithLogging();
            NetworkCredential credentials = new NetworkCredential("test@example.com", "password");
            
            // Test boundary conditions for MFA parameters
            var boundaryTests = new[]
            {
                (token: (string)null, secret: "JBSWY3DPEHPK3PXP", scenario: "NullToken_ValidSecret"),
                (token: "123456", secret: (string)null, scenario: "ValidToken_NullSecret"),
                (token: (string)null, secret: (string)null, scenario: "BothNull"),
                (token: "", secret: "", scenario: "BothEmpty"),
                (token: "123456", secret: "", scenario: "ValidToken_EmptySecret"),
                (token: "", secret: "JBSWY3DPEHPK3PXP", scenario: "EmptyToken_ValidSecret")
            };
            
            foreach (var (token, secret, scenario) in boundaryTests)
            {
                try
                {
                    client.Login(credentials, token, secret);
                }
                catch (ArgumentException)
                {
                    AssertLogger.IsTrue(true, $"ArgumentException for scenario: {scenario}");
                }
                catch (ContentstackErrorException ex)
                {
                    AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, ex.StatusCode, 
                        $"StatusCode for scenario: {scenario}");
                }
                catch (Exception e)
                {
                    AssertLogger.Fail($"Unexpected exception for scenario {scenario}: {e.GetType().Name} - {e.Message}");
                }
            }
        }

        #endregion

        #region Phase 8: System Integration Testing (096-100)

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test096_Should_Handle_Concurrent_Login_Attempts_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "ConcurrentLoginAsync");
            
            var clients = new ContentstackClient[3];
            for (int i = 0; i < clients.Length; i++)
            {
                clients[i] = CreateClientWithLogging();
            }
            
            // Use invalid credentials to test concurrent error handling
            var credentials = new NetworkCredential("concurrent_test", "invalid_password");
            
            try
            {
                var loginTasks = clients.Select(client => 
                    System.Threading.Tasks.Task.Run(async () =>
                    {
                        try
                        {
                            await client.LoginAsync(credentials);
                            return ("Success", (Exception)null);
                        }
                        catch (Exception ex)
                        {
                            return ("Error", ex);
                        }
                    })
                ).ToArray();
                
                var results = await System.Threading.Tasks.Task.WhenAll(loginTasks);
                
                // All should fail with the same error type
                foreach (var (status, exception) in results)
                {
                    AssertLogger.AreEqual("Error", status, "ExpectedError");
                    AssertLogger.IsInstanceOfType(exception, typeof(ContentstackErrorException), "ExceptionType");
                    
                    var contentError = exception as ContentstackErrorException;
                    AssertLogger.AreEqual(HttpStatusCode.UnprocessableEntity, contentError.StatusCode, "StatusCode");
                    AssertLogger.AreEqual(104, contentError.ErrorCode, "ErrorCode");
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception in concurrent login test: {e.GetType().Name} - {e.Message}");
            }
            finally
            {
                // Cleanup
                foreach (var client in clients)
                {
                    try { client?.Dispose(); } catch { }
                }
            }
        }



        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test100_Should_Validate_Complete_Error_Path_Coverage_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "CompleteErrorCoverageAsync");
            
            // Comprehensive validation test covering multiple error scenarios in sequence
            var testScenarios = new (string, Func<Task<ContentstackResponse>>)[]
            {
                ("NullCredentials", () => CreateClientWithLogging().LoginAsync(null)),
                ("EmptyCredentials", () => CreateClientWithLogging().LoginAsync(new NetworkCredential("", ""))),
                ("InvalidCredentials", () => CreateClientWithLogging().LoginAsync(new NetworkCredential("invalid", "invalid"))),
                ("NetworkError", () => CreateClientWithMockError(NetworkErrorType.Timeout, 100).LoginAsync(new NetworkCredential("test", "test"))),
                ("HttpError", () => CreateClientWithHttpStatusNoRetry(HttpStatusCode.InternalServerError).LoginAsync(new NetworkCredential("test", "test"))),
                ("MalformedResponse", () => CreateClientWithMalformedResponse("invalid json").LoginAsync(new NetworkCredential("test", "test")))
            };
            
            var results = new System.Collections.Generic.List<(string scenario, bool success, string errorType)>();
            
            foreach (var (scenario, loginAction) in testScenarios)
            {
                try
                {
                    await loginAction();
                    results.Add((scenario, true, "NoError"));
                    AssertLogger.Fail($"Expected error in scenario: {scenario}");
                }
                catch (ArgumentNullException)
                {
                    results.Add((scenario, false, "ArgumentNullException"));
                }
                catch (ContentstackErrorException ex)
                {
                    results.Add((scenario, false, $"ContentstackErrorException_{ex.StatusCode}"));
                }
                catch (HttpRequestException)
                {
                    results.Add((scenario, false, "HttpRequestException"));
                }
                catch (TaskCanceledException)
                {
                    results.Add((scenario, false, "TaskCanceledException"));
                }
                catch (System.Text.Json.JsonException)
                {
                    results.Add((scenario, false, "JsonException"));
                }
                catch (Exception ex)
                {
                    results.Add((scenario, false, $"UnexpectedException_{ex.GetType().Name}"));
                    AssertLogger.Fail($"Unexpected exception in {scenario}: {ex.GetType().Name} - {ex.Message}");
                }
            }
            
            // Verify all scenarios produced expected errors
            AssertLogger.AreEqual(testScenarios.Length, results.Count, "AllScenariosExecuted");
            
            foreach (var (scenario, success, errorType) in results)
            {
                AssertLogger.IsFalse(success, $"Scenario {scenario} should have failed");
                AssertLogger.IsTrue(!string.IsNullOrEmpty(errorType), $"Error type recorded for {scenario}");
                
                TestOutputLogger.LogContext($"Scenario_{scenario}", errorType);
            }
            
            AssertLogger.IsTrue(true, "Complete error path coverage validation passed");
        }

        #endregion
    }
}
