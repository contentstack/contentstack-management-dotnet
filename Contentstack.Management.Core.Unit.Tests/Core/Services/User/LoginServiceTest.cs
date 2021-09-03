using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Services.User;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.User
{
    [TestClass]
    public class LoginServiceTest
    {
        ICredentials credentials = new NetworkCredential("name", "password");
        JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());

        [TestMethod]
        public void Should_Not_Allow_Null_serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LoginService(null, credentials));
        }

        [TestMethod]
        public void Should_Not_Allow_Null_Credentials()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LoginService(serializer, null));
        }

        [TestMethod]
        public void Should_Allow_Credentials()
        {
            var loginService = new LoginService(serializer, credentials);
            loginService.ContentBody();

            Assert.IsNotNull(loginService);
            Assert.AreEqual("POST", loginService.HttpMethod);
            Assert.AreEqual("user-session", loginService.ResourcePath);
            Assert.AreEqual("{\"user\":{\"email\":\"name\",\"password\":\"password\"}}", Encoding.Default.GetString(loginService.Content));
        }

        [TestMethod]
        public void Should_Allow_Credentials_With_Token()
        {
            var loginService = new LoginService(serializer, credentials, "token");
            loginService.ContentBody();

            Assert.IsNotNull(loginService);
            Assert.AreEqual("{\"user\":{\"email\":\"name\",\"password\":\"password\",\"tfa_token\":\"token\"}}", Encoding.Default.GetString(loginService.Content));
        }

        [TestMethod]
        public void Should_Override_Authtoken_To_ContentstackOptions_On_Success()
        {
            var loginService = new LoginService(serializer, credentials);
            var config = new ContentstackClientOptions();
            ContentstackResponse httpResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");

            Assert.IsNull(config.Authtoken);

            loginService.OnResponse(httpResponse, config);

            Assert.AreEqual("authtoken", config.Authtoken);
        }

        [TestMethod]
        public void Should_Not_Override_Authtoken_To_ContentstackOptions_On_Failuer_response()
        {
            var loginService = new LoginService(serializer, credentials);
            var config = new ContentstackClientOptions();
            ContentstackResponse httpResponse = MockResponse.CreateContentstackResponse("422Response.txt");

            Assert.IsNull(config.Authtoken);
            loginService.OnResponse(httpResponse, config);
            Assert.IsNull(config.Authtoken);
        }
    }
}
