using System;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.User
{
    [TestClass]
    public class ResetPasswordServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResetPasswordService(null, null, null, null));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Email()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResetPasswordService(serializer, null, null, null));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Password()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResetPasswordService(serializer, _fixture.Create<string>(), null, null));
        }

        [TestMethod]
        public void Should_Throw_On_Null_ConfirmPassword()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResetPasswordService(serializer, _fixture.Create<string>(), _fixture.Create<string>(), null));
        }

        [TestMethod]
        public void Should_Initialize_With_Proper_ResourcePath_And_Method()
        {
            string password = _fixture.Create<string>();
            ResetPasswordService resetPasswordService = new ResetPasswordService(serializer, _fixture.Create<string>(), password, password);
            Assert.IsNotNull(resetPasswordService);

            Assert.AreEqual("POST", resetPasswordService.HttpMethod);
            Assert.AreEqual("user/reset_password", resetPasswordService.ResourcePath);
        }

        [TestMethod]
        public void Should_Return_Null_Content_On_ContentBody_Call()
        {
            string resetToken = _fixture.Create<string>();
            string password = _fixture.Create<string>();

            ResetPasswordService resetPasswordService = new ResetPasswordService(serializer, resetToken, password, password);
            resetPasswordService.ContentBody();

            Assert.IsNotNull(resetPasswordService);
            Assert.AreEqual($"{{\"user\":{{\"reset_password_token\":\"{resetToken}\",\"password\":\"{password}\",\"password_confirmation\":\"{password}\"}}}}", Encoding.Default.GetString(resetPasswordService.Content));
        }
    }
}
