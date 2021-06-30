using System;
using System.Text;
using AutoFixture;
using Contentstack.Management.Core.Services.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.User
{
    [TestClass]
    public class ForgotPasswordServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture();

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ForgotPasswordService(null, null));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Email()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ForgotPasswordService(serializer, null));
        }

        [TestMethod]
        public void Should_Initialize_With_Proper_ResourcePath_And_Method()
        {
            ForgotPasswordService forgotPasswordService = new ForgotPasswordService(serializer, _fixture.Create<string>());
            Assert.IsNotNull(forgotPasswordService);

            Assert.AreEqual("POST", forgotPasswordService.HttpMethod);
            Assert.AreEqual("user/forgot_password", forgotPasswordService.ResourcePath);
        }

        [TestMethod]
        public void Should_Return_Null_Content_On_ContentBody_Call()
        {
            string _email = _fixture.Create<string>();
            ForgotPasswordService forgotPasswordService = new ForgotPasswordService(serializer, _email);
            forgotPasswordService.ContentBody();

            Assert.IsNotNull(forgotPasswordService);
            Assert.AreEqual($"{{\"user\":{{\"email\":\"{_email}\"}}}}", Encoding.Default.GetString(forgotPasswordService.Content));
        }
    }
}
