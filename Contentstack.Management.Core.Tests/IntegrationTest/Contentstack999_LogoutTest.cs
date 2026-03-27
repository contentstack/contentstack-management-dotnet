using System;
using System.Net.Http;
using Contentstack.Management.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack999_LogoutTest
    {
        private static ContentstackClient CreateClientWithLogging()
        {
            var handler = new LoggingHttpHandler();
            var httpClient = new HttpClient(handler);
            return new ContentstackClient(httpClient, new ContentstackClientOptions());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_Success_On_Sync_Logout()
        {
            TestOutputLogger.LogContext("TestScenario", "SyncLogout");
            try
            {
                ContentstackClient client = Contentstack.CreateAuthenticatedClient();
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "AuthtokenBeforeLogout");

                ContentstackResponse contentstackResponse = client.Logout();
                string loginResponse = contentstackResponse.OpenResponse();

                AssertLogger.IsNull(client.contentstackOptions.Authtoken, "AuthtokenAfterLogout");
                AssertLogger.IsNotNull(loginResponse, "LogoutResponse");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Return_Success_On_Async_Logout()
        {
            TestOutputLogger.LogContext("TestScenario", "AsyncLogout");
            try
            {
                ContentstackClient client = Contentstack.CreateAuthenticatedClient();
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "AuthtokenBeforeLogout");

                ContentstackResponse contentstackResponse = await client.LogoutAsync();
                string logoutResponse = contentstackResponse.OpenResponse();

                AssertLogger.IsNull(client.contentstackOptions.Authtoken, "AuthtokenAfterLogout");
                AssertLogger.IsNotNull(logoutResponse, "LogoutResponse");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Handle_Logout_When_Not_LoggedIn()
        {
            TestOutputLogger.LogContext("TestScenario", "LogoutWhenNotLoggedIn");
            ContentstackClient client = CreateClientWithLogging();

            AssertLogger.IsNull(client.contentstackOptions.Authtoken, "AuthtokenNotSet");

            try
            {
                client.Logout();
            }
            catch (Exception e)
            {
                AssertLogger.IsTrue(
                    e.Message.Contains("token") || e.Message.Contains("Authentication") || e.Message.Contains("not logged in"),
                    "LogoutNotLoggedInError");
            }
        }
    }
}
