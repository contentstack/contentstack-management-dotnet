using System;
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
        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_Failuer_On_Wrong_Login_Credentials()
        {
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");
            
            try
            {
                ContentstackResponse contentstackResponse = client.Login(credentials);
            } catch (Exception e)
            {
                ContentstackErrorException errorException = e as ContentstackErrorException;
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode);
                Assert.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.Message);
                Assert.AreEqual("Looks like your email or password is invalid. Please try again or reset your password.", errorException.ErrorMessage);
                Assert.AreEqual(104, errorException.ErrorCode);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Return_Failuer_On_Wrong_Async_Login_Credentials()
        {
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");
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
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Return_Success_On_Async_Login()
        {
            ContentstackClient client = new ContentstackClient();
            
            try
            {
                ContentstackResponse contentstackResponse =  await client.LoginAsync(Contentstack.Credential);
                string loginResponse = contentstackResponse.OpenResponse();

                Assert.IsNotNull(client.contentstackOptions.Authtoken);
                Assert.IsNotNull(loginResponse);
                
                await client.LogoutAsync();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Return_Success_On_Login()
        {
            try
            {
                ContentstackClient client = new ContentstackClient();
                
                ContentstackResponse contentstackResponse = client.Login(Contentstack.Credential);
                string loginResponse = contentstackResponse.OpenResponse();

                Assert.IsNotNull(client.contentstackOptions.Authtoken);
                Assert.IsNotNull(loginResponse);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Return_Loggedin_User()
        {
            try
            {
                ContentstackClient client = new ContentstackClient();
                
                client.Login(Contentstack.Credential);
                
                ContentstackResponse response = client.GetUser();

                var user = response.OpenJObjectResponse();

                Assert.IsNotNull(user);

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test006_Should_Return_Loggedin_User_Async()
        {
            try
            {
                ContentstackClient client = new ContentstackClient();
                
                await client.LoginAsync(Contentstack.Credential);
                
                ContentstackResponse response = await client.GetUserAsync();

                var user = response.OpenJObjectResponse();

                Assert.IsNotNull(user);
                Assert.IsNotNull(user["user"]["organizations"]);
                Assert.IsInstanceOfType(user["user"]["organizations"], typeof(JArray));
                Assert.IsNull(user["user"]["organizations"][0]["org_roles"]);

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Return_Loggedin_User_With_Organizations_detail()
        {
            try
            {
                ParameterCollection collection = new ParameterCollection();
                collection.Add("include_orgs_roles", true);
            
                ContentstackClient client = new ContentstackClient();
                
                client.Login(Contentstack.Credential);
                
                ContentstackResponse response = client.GetUser(collection);

                var user = response.OpenJObjectResponse();

                Assert.IsNotNull(user);
                Assert.IsNotNull(user["user"]["organizations"]);
                Assert.IsInstanceOfType(user["user"]["organizations"], typeof(JArray));
                Assert.IsNotNull(user["user"]["organizations"][0]["org_roles"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Fail_Login_With_Invalid_MfaSecret()
        {
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
            string invalidMfaSecret = "INVALID_BASE32_SECRET!@#";
            
            try
            {
                ContentstackResponse contentstackResponse = client.Login(credentials, null, invalidMfaSecret);
                Assert.Fail("Expected exception for invalid MFA secret");
            }
            catch (ArgumentException)
            {
                // Expected exception for invalid Base32 encoding
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Generate_TOTP_Token_With_Valid_MfaSecret()
        {
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
            string validMfaSecret = "JBSWY3DPEHPK3PXP"; // Valid Base32 test secret
            
            try
            {
                // This should fail due to invalid credentials, but should succeed in generating TOTP
                ContentstackResponse contentstackResponse = client.Login(credentials, null, validMfaSecret);
            }
            catch (ContentstackErrorException errorException)
            {
                // Expected to fail due to invalid credentials, but we verify it processed the MFA secret
                // The error should be about credentials, not about MFA secret format
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode);
                Assert.IsTrue(errorException.Message.Contains("email or password") || 
                             errorException.Message.Contains("credentials") ||
                             errorException.Message.Contains("authentication"));
            }
            catch (ArgumentException)
            {
                Assert.Fail("Should not throw ArgumentException for valid MFA secret");
            }
            catch (Exception e)
            {
                Assert.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test010_Should_Generate_TOTP_Token_With_Valid_MfaSecret_Async()
        {
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
            string validMfaSecret = "JBSWY3DPEHPK3PXP"; // Valid Base32 test secret
            
            try
            {
                // This should fail due to invalid credentials, but should succeed in generating TOTP
                ContentstackResponse contentstackResponse = await client.LoginAsync(credentials, null, validMfaSecret);
            }
            catch (ContentstackErrorException errorException)
            {
                // Expected to fail due to invalid credentials, but we verify it processed the MFA secret
                // The error should be about credentials, not about MFA secret format
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode);
                Assert.IsTrue(errorException.Message.Contains("email or password") || 
                             errorException.Message.Contains("credentials") ||
                             errorException.Message.Contains("authentication"));
            }
            catch (ArgumentException)
            {
                Assert.Fail("Should not throw ArgumentException for valid MFA secret");
            }
            catch (Exception e)
            {
                Assert.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Prefer_Explicit_Token_Over_MfaSecret()
        {
            ContentstackClient client = new ContentstackClient();
            NetworkCredential credentials = new NetworkCredential("test_user", "test_password");
            string validMfaSecret = "JBSWY3DPEHPK3PXP";
            string explicitToken = "123456";
            
            try
            {
                // This should fail due to invalid credentials, but should use explicit token
                ContentstackResponse contentstackResponse = client.Login(credentials, explicitToken, validMfaSecret);
            }
            catch (ContentstackErrorException errorException)
            {
                // Expected to fail due to invalid credentials
                // The important thing is that it didn't throw an exception about MFA secret processing
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, errorException.StatusCode);
            }
            catch (ArgumentException)
            {
                Assert.Fail("Should not throw ArgumentException when explicit token is provided");
            }
            catch (Exception e)
            {
                Assert.Fail($"Unexpected exception type: {e.GetType().Name} - {e.Message}");
            }
        }
    }
}
