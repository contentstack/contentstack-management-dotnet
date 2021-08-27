using System;
using System.Net;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class UserTest
    {
        private ContentstackClient client;
        private readonly NetworkCredential credentials = new NetworkCredential("mock_user", "mock_pasword");
        private readonly IFixture _fixture = new Fixture();

        [TestInitialize]
        public void initialize()
        {
            client = new ContentstackClient();
        }

        [TestMethod]
        public void Initialize_User()
        {
            User user = new User(client);

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void Should_Throw_On_Login_If_Already_Logged_In ()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.ThrowsException<InvalidOperationException>(() => client.Login(credentials));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.LoginAsync(credentials));
        }

        [TestMethod]
        public void Should_Return_Response_On_Login_Success()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));

            ContentstackResponse response = client.Login(credentials);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Authtoken_Null_On_Login_Failuer()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("400Response.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));

            ContentstackResponse response = client.Login(credentials);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_On_LoginAsync_SuccessAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));

            ContentstackResponse response = await client.LoginAsync(credentials);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Response_On_LoginAsync_Success_On_ContinueWith()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));

            var response = client.LoginAsync(credentials);

            response.ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    var result = t.Result as ContentstackResponse;
                    Assert.AreEqual(contentstackResponse.OpenResponse(), result.OpenResponse());
                    Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), result.OpenJObjectResponse().ToString());
                }
            });
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Throw_On_Null_Authtoken_LogOut()
        {
            Assert.ThrowsException<ArgumentNullException>(() => client.Logout());
            Assert.ThrowsException<ArgumentNullException>(() => client.LogoutAsync());
        }

        [TestMethod]
        public void Should_Return_Response_On_Logout()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("LogoutResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            var response = client.Logout();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Authtoken_Null_On_Logout_Failuer()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("400Response.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            ContentstackResponse response = client.Logout();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNotNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Response_On_LogoutAsync()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("LogoutResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            var response = await client.LogoutAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNull(client.contentstackOptions.Authtoken);
        }

        
        [TestMethod]
        public void Should_Return_Response_On_LogoutAsync_ContinueWith()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("LogoutResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            _ = client.LogoutAsync().ContinueWith((response) =>
              {
                  if (response.IsCompleted)
                  {
                      var result = response.Result as ContentstackResponse;
                      Assert.AreEqual(contentstackResponse.OpenResponse(), result.OpenResponse());
                      Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), result.OpenJObjectResponse().ToString());
                  }
              });
            Assert.IsNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Throw_On_ForgotPassword_Already_Logged_In()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("ForgotPasswordResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            User user = new User(client);
            
            Assert.ThrowsException<InvalidOperationException>(() => user.ForgotPassword(_fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => user.ForgotPasswordAsync(_fixture.Create<string>()));

        }

        [TestMethod]
        public void Should_Return_Response_On_ForgotPassword_Success()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("ForgotPasswordResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));

            User user = new User(client);
            var response = user.ForgotPassword(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Response_On_ForgotPassword_Async_Success()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("ForgotPasswordResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));

            User user = new User(client);
            var response = user.ForgotPasswordAsync(_fixture.Create<string>()).ContinueWith((response) =>
            {
                if (response.IsCompleted)
                {
                    var result = response.Result as ContentstackResponse;
                    Assert.AreEqual(contentstackResponse.OpenResponse(), result.OpenResponse());
                    Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), result.OpenJObjectResponse().ToString());
                }
            });
            Assert.IsNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Throw_On_ResetPassword_Already_Logged_In()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("ForgotPasswordResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            User user = new User(client);

            Assert.ThrowsException<InvalidOperationException>(() => user.ResetPassword(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => user.ResetPasswordAsync(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()));

        }

        [TestMethod]
        public void Should_Return_Response_On_ResetPassword_Success()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("ForgotPasswordResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));

            User user = new User(client);
            var response = user.ResetPassword(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
            Assert.IsNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Return_Response_On_ResetPassword_Async_Success()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("ForgotPasswordResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));

            User user = new User(client);
            var response = user.ResetPasswordAsync(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()).ContinueWith((response) =>
            {
                if (response.IsCompleted)
                {
                    var result = response.Result as ContentstackResponse;
                    Assert.AreEqual(contentstackResponse.OpenResponse(), result.OpenResponse());
                    Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), result.OpenJObjectResponse().ToString());
                }
            });
            Assert.IsNull(client.contentstackOptions.Authtoken);
        }

        [TestMethod]
        public void Should_Throw_If_Not_Loggedin()
        {
            Assert.ThrowsException<InvalidOperationException>(() => client.GetUser());
            Assert.ThrowsException<InvalidOperationException>(() => client.GetUserAsync());
        }

        [TestMethod]
        public void Should_Return_Response_For_LoggedIn_User()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<String>();

            var response = client.GetUser();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Return_Response_For_LoggedIn_User_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<String>();

            var response = client.GetUserAsync().ContinueWith((response) =>
            {
                if (response.IsCompleted)
                {
                    var result = response.Result as ContentstackResponse;
                    Assert.AreEqual(contentstackResponse.OpenResponse(), result.OpenResponse());
                    Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), result.OpenJObjectResponse().ToString());
                }
            });
        }
    }
}
