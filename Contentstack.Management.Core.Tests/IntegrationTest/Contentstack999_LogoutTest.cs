using System;
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
            try
            {
                ContentstackClient client = Contentstack.Client;
                ContentstackResponse contentstackResponse = client.Logout();
                string loginResponse = contentstackResponse.OpenResponse();

                Assert.IsNull(client.contentstackOptions.Authtoken);
                Assert.IsNotNull(loginResponse);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
