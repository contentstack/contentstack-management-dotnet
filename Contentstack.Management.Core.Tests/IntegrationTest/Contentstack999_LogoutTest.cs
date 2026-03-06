using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack999_LogoutTest
    {
        private static string _host => Contentstack.Client.contentstackOptions.Host;

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_Success_On_Logout()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                ContentstackClient client = Contentstack.Client;
                TestReportHelper.LogRequest("client.Logout()", "DELETE",
                    $"https://{_host}/v3/user-session");

                ContentstackResponse contentstackResponse = client.Logout();
                sw.Stop();
                string loginResponse = contentstackResponse.OpenResponse();
                TestReportHelper.LogResponse((int)contentstackResponse.StatusCode,
                    contentstackResponse.StatusCode.ToString(), sw.ElapsedMilliseconds, loginResponse);

                TestReportHelper.LogAssertion(client.contentstackOptions.Authtoken == null,
                    "Authtoken is null after logout", type: "IsNull");
                TestReportHelper.LogAssertion(loginResponse != null,
                    "Response body is not null", type: "IsNotNull");
                Assert.IsNull(client.contentstackOptions.Authtoken);
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
    }
}
