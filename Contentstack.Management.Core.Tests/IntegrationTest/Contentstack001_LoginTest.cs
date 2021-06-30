using System;
using System.Net;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

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
            User user = new ContentstackClient().User();
            NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");
            
            try
            {
                ContentstackResponse contentstackResponse = user.Login(credentials);
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
            User user = new ContentstackClient().User();
            NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");
            var response = user.LoginAsync(credentials);

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
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Return_Success_On_Async_Login()
        {
            ContentstackClient client = new ContentstackClient();
            var response = client.User().LoginAsync(Contentstack.Credential);

            response.ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    ContentstackResponse contentstackResponse = t.Result as ContentstackResponse;
                    string loginResponse = contentstackResponse.OpenResponse();

                    Assert.IsNotNull(client.contentstackOptions.Authtoken);
                    Assert.IsNotNull(loginResponse);
                }
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Return_Success_On_Login()
        {
            string email = _configuration.GetSection("Contentstack:Credentials:Email").Value;
            string password = _configuration.GetSection("Contentstack:Credentials:Password").Value;

            try
            {
                ContentstackClient client = Contentstack.Client;
                ContentstackResponse contentstackResponse = client.User().Login(Contentstack.Credential);
                string loginResponse = contentstackResponse.OpenResponse();

                Assert.IsNotNull(client.contentstackOptions.Authtoken);
                Assert.IsNotNull(loginResponse);
            }
            catch (ContentstackErrorException e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
