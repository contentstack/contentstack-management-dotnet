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
        public void Test003_Should_Return_Success_On_Async_Login()
        {
            ContentstackClient client = new ContentstackClient();
            var response = client.LoginAsync(Contentstack.Credential);

            response.ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    try
                    {
                        ContentstackResponse contentstackResponse = t.Result;
                        string loginResponse = contentstackResponse.OpenResponse();

                        Assert.IsNotNull(client.contentstackOptions.Authtoken);
                        Assert.IsNotNull(loginResponse);
                    }catch (Exception e)
                    {
                        Assert.Fail(e.Message);
                    }

                }
            });
            Thread.Sleep(3000);
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
                ContentstackClient client = Contentstack.Client;
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
                ContentstackClient client = Contentstack.Client;
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
            
                ContentstackClient client = Contentstack.Client;
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
    }
}
