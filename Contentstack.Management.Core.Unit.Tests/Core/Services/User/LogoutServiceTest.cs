using System;
using Contentstack.Management.Core.Services.User;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.User
{
    [TestClass]
    public class LogoutServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly string _authtoken = "authtoken";

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogoutService(null, null));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Authtoken()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogoutService(serializer, null));
            Assert.ThrowsException<ArgumentNullException>(() => new LogoutService(serializer, ""));
            Assert.ThrowsException<ArgumentNullException>(() => new LogoutService(serializer, string.Empty));
        }

        [TestMethod]
        public void Should_Allow_Authtoken()
        {
            LogoutService logoutService = new LogoutService(serializer, _authtoken);
            Assert.IsNotNull(logoutService);
            
            Assert.AreEqual("DELETE", logoutService.HttpMethod);
            Assert.AreEqual("user-session", logoutService.ResourcePath);
        }

        [TestMethod]
        public void Should_Return_Null_Content_On_ContentBody_Call()
        {
            LogoutService logoutService = new LogoutService(serializer, _authtoken);
            logoutService.ContentBody();
            Assert.AreEqual(_authtoken, logoutService.Headers["authtoken"]);
            Assert.IsNotNull(logoutService);
            Assert.IsNull(logoutService.Content);   
        }

        [TestMethod]
        public void Should_Remove_Authtoken_From_Config_On_Success_Response()
        {
            LogoutService logoutService = new LogoutService(serializer, _authtoken);
            var config = new ContentstackClientOptions();
            config.Authtoken = _authtoken;
            ContentstackResponse httpResponse = MockResponse.CreateContentstackResponse("LogoutResponse.txt");

            logoutService.OnResponse(httpResponse, config);
            Assert.IsNull(config.Authtoken);
        }

        [TestMethod]
        public void Should_Not_Authtoken_From_Config_On_Success_Response_Different_Authtoken()
        {
            LogoutService logoutService = new LogoutService(serializer, _authtoken);
            var config = new ContentstackClientOptions();
            config.Authtoken = "_authtoken";
            ContentstackResponse httpResponse = MockResponse.CreateContentstackResponse("LogoutResponse.txt");

            logoutService.OnResponse(httpResponse, config);
            Assert.IsNotNull(config.Authtoken);
            Assert.AreEqual("_authtoken", config.Authtoken);
        }
    }
}
