using System;
using Contentstack.Management.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack999_LogoutTest
    {
        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_Success_On_Logout()
        {
            TestOutputLogger.LogContext("TestScenario", "Logout");
            try
            {
                ContentstackClient client = Contentstack.Client;
                ContentstackResponse contentstackResponse = client.Logout();
                string loginResponse = contentstackResponse.OpenResponse();

                AssertLogger.IsNull(client.contentstackOptions.Authtoken, "Authtoken");
                AssertLogger.IsNotNull(loginResponse, "loginResponse");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }
    }
}
