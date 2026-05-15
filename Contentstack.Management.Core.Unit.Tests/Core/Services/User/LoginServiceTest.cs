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
            Assert.AreEqual("{\"user\":{\"email\":\"name\",\"password\":\"password\"}}", Encoding.Default.GetString(loginService.ByteContent));
        }

        [TestMethod]
        public void Should_Allow_Credentials_With_Token()
        {
            var loginService = new LoginService(serializer, credentials, "token");
            loginService.ContentBody();

            Assert.IsNotNull(loginService);
            Assert.AreEqual("{\"user\":{\"email\":\"name\",\"password\":\"password\",\"tfa_token\":\"token\"}}", Encoding.Default.GetString(loginService.ByteContent));
        }

        [TestMethod]
        public void Should_Allow_Credentials_With_MfaSecret()
        {

            string testMfaSecret = "JBSWY3DPEHPK3PXP"; // Base32 encoded "Hello!"
            var loginService = new LoginService(serializer, credentials, null, testMfaSecret);
            loginService.ContentBody();

            Assert.IsNotNull(loginService);
            var contentString = Encoding.Default.GetString(loginService.ByteContent);
            
            Assert.IsTrue(contentString.Contains("\"email\":\"name\""));
            Assert.IsTrue(contentString.Contains("\"password\":\"password\""));
            Assert.IsTrue(contentString.Contains("\"tfa_token\":"));
            
            // Verify the tfa_token is not null or empty in the JSON
            Assert.IsFalse(contentString.Contains("\"tfa_token\":null"));
            Assert.IsFalse(contentString.Contains("\"tfa_token\":\"\""));
        }

        [TestMethod]
        public void Should_Generate_TOTP_Token_When_MfaSecret_Provided()
        {
            string testMfaSecret = "JBSWY3DPEHPK3PXP"; // Base32 encoded "Hello!"
            var loginService1 = new LoginService(serializer, credentials, null, testMfaSecret);
            var loginService2 = new LoginService(serializer, credentials, null, testMfaSecret);
            
            loginService1.ContentBody();
            loginService2.ContentBody();

            var content1 = Encoding.Default.GetString(loginService1.ByteContent);
            var content2 = Encoding.Default.GetString(loginService2.ByteContent);

            // Both should contain tfa_token
            Assert.IsTrue(content1.Contains("\"tfa_token\":"));
            Assert.IsTrue(content2.Contains("\"tfa_token\":"));
            
            // Extract the tokens for comparison (tokens should be 6 digits)
            var token1Match = System.Text.RegularExpressions.Regex.Match(content1, "\"tfa_token\":\"(\\d{6})\"");
            var token2Match = System.Text.RegularExpressions.Regex.Match(content2, "\"tfa_token\":\"(\\d{6})\"");
            
            Assert.IsTrue(token1Match.Success);
            Assert.IsTrue(token2Match.Success);
            
            // Tokens should be valid 6-digit numbers
            Assert.AreEqual(6, token1Match.Groups[1].Value.Length);
            Assert.AreEqual(6, token2Match.Groups[1].Value.Length);
        }

        [TestMethod]
        public void Should_Prefer_Explicit_Token_Over_MfaSecret()
        {
            string testMfaSecret = "JBSWY3DPEHPK3PXP";
            // file deepcode ignore NoHardcodedCredentials/test: random test token
            string explicitToken = "123456";
            
            var loginService = new LoginService(serializer, credentials, explicitToken, testMfaSecret);
            loginService.ContentBody();

            var contentString = Encoding.Default.GetString(loginService.ByteContent);
            
            // Should use the explicit token, not generate one from MFA secret
            Assert.IsTrue(contentString.Contains("\"tfa_token\":\"123456\""));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_Exception_For_Invalid_Base32_MfaSecret()
        {
            // Invalid Base32 secret (contains invalid characters)
            string invalidMfaSecret = "INVALID_BASE32_123!@#";
            
            var loginService = new LoginService(serializer, credentials, null, invalidMfaSecret);
        }

        [TestMethod]
        public void Should_Not_Generate_Token_When_MfaSecret_Is_Empty()
        {
            var loginService = new LoginService(serializer, credentials, null, "");
            loginService.ContentBody();

            var contentString = Encoding.Default.GetString(loginService.ByteContent);
            
            // Should not contain tfa_token when MFA secret is empty
            Assert.IsFalse(contentString.Contains("\"tfa_token\":"));
            Assert.AreEqual("{\"user\":{\"email\":\"name\",\"password\":\"password\"}}", contentString);
        }

        [TestMethod]
        public void Should_Not_Generate_Token_When_MfaSecret_Is_Null()
        {
            var loginService = new LoginService(serializer, credentials, null, null);
            loginService.ContentBody();

            var contentString = Encoding.Default.GetString(loginService.ByteContent);
            
            // Should not contain tfa_token when MFA secret is null
            Assert.IsFalse(contentString.Contains("\"tfa_token\":"));
            Assert.AreEqual("{\"user\":{\"email\":\"name\",\"password\":\"password\"}}", contentString);
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
