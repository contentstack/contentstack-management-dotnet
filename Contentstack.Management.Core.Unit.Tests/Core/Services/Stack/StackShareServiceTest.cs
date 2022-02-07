using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services.Stack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Stack
{
    [TestClass]
    public class StackShareServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackShareService(null, new Management.Core.Models.Stack(null, _fixture.Create<string>())));
        }

        [TestMethod]
        public void Should_Throw_On_Null_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackShareService(serializer, new Management.Core.Models.Stack(null)));
        }

        [TestMethod]
        public void Should_Initialize_Stack_Share_Service()
        {
            var apiKey = _fixture.Create<string>();

            var service = new StackShareService(serializer, new Management.Core.Models.Stack(null, apiKey));
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual(null, service.ResourcePath);
            Assert.AreEqual(apiKey, service.Headers["api_key"]);
        }

        [TestMethod]
        public void Should_Return_Stack_Share_Roles()
        {
            var apiKey = _fixture.Create<string>();
            var userInvitation = new UserInvitation()
            {
                Email = _fixture.Create<string>(),
                Roles = _fixture.Create<List<string>>()
            };
            var roles = new List<string>();
            foreach (string role in userInvitation.Roles)
                roles.Add($"\"{role}\"");

            var service = new StackShareService(serializer, new Management.Core.Models.Stack(null, apiKey));
            service.AddUsers(new List<UserInvitation>() { userInvitation });
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("stacks/share", service.ResourcePath);
            Assert.AreEqual(apiKey, service.Headers["api_key"]);
            Assert.AreEqual($"{{\"emails\":[\"{userInvitation.Email}\"],\"roles\":{{\"{userInvitation.Email}\":[{string.Join(",", roles)}]}}}}", Encoding.Default.GetString(service.ByteContent));
        }

        [TestMethod]
        public void Should_Return_Stack_Unshare()
        {
            var apiKey = _fixture.Create<string>();
            var email = _fixture.Create<string>();

            var service = new StackShareService(serializer, new Management.Core.Models.Stack(null, apiKey));
            service.RemoveUsers(email);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("stacks/unshare", service.ResourcePath);
            Assert.AreEqual($"{{\"email\":\"{email}\"}}", Encoding.Default.GetString(service.ByteContent));
        }
    }
}
