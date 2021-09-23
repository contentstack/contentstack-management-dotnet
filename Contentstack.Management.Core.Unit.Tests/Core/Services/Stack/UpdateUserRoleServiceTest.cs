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
    public class UpdateUserRoleServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UpdateUserRoleService(null, _fixture.Create<List<UserInvitation>>(), _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Null_UserInvite()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UpdateUserRoleService(serializer, null, _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Null_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UpdateUserRoleService(serializer, _fixture.Create<List<UserInvitation>>(), null));
        }

        [TestMethod]
        public void Should_Initialize_with_Data()
        {
            var service = new UpdateUserRoleService(serializer, _fixture.Create<List<UserInvitation>>(), _fixture.Create<string>());

            Assert.IsNotNull(service);
            Assert.AreEqual(false, service.UseQueryString);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("stacks/users/roles", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Return_Content_Of_Post_Method()
        {
            var users = new List<UserInvitation>();
            var service = new UpdateUserRoleService(serializer, users, _fixture.Create<string>());

            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("{\"users\":{}}", Encoding.Default.GetString(service.Content));
        }

        [TestMethod]
        public void Should_Return_Content_With_User()
        {
            var user = new UserInvitation()
            {
                Uid = _fixture.Create<string>(),
                Roles = _fixture.Create<List<string>>()
            };
            var users = new List<UserInvitation>();
            users.Add(user);
            var service = new UpdateUserRoleService(serializer, users, _fixture.Create<string>());

            service.ContentBody();
            var roles = new List<string>();
            foreach (string role in user.Roles)
                roles.Add($"\"{role}\"");

            Assert.IsNotNull(service);
            Assert.AreEqual($"{{\"users\":{{\"{user.Uid}\":[{string.Join(",", roles)}]}}}}", Encoding.Default.GetString(service.Content));
        }
    }
}
