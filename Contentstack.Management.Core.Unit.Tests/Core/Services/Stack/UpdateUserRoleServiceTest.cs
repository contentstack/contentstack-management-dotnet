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
            Assert.ThrowsException<ArgumentNullException>(() => new UpdateUserRoleService(null, new Management.Core.Models.Stack(null, _fixture.Create<string>()), _fixture.Create<List<UserInvitation>>()));
        }

        [TestMethod]
        public void Should_Throw_On_Null_UserInvite()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UpdateUserRoleService(serializer, new Management.Core.Models.Stack(null, _fixture.Create<string>()), null));
        }

        [TestMethod]
        public void Should_Throw_On_Null_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UpdateUserRoleService(serializer, new Management.Core.Models.Stack(null), _fixture.Create<List<UserInvitation>>()));
        }

        [TestMethod]
        public void Should_Initialize_with_Data()
        {
            var service = new UpdateUserRoleService(serializer, new Management.Core.Models.Stack(null, _fixture.Create<string>()), _fixture.Create<List<UserInvitation>>());

            Assert.IsNotNull(service);
            Assert.AreEqual(false, service.UseQueryString);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("stacks/users/roles", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Return_Content_Of_Post_Method()
        {
            var users = new List<UserInvitation>();
            var service = new UpdateUserRoleService(serializer, new Management.Core.Models.Stack(null, _fixture.Create<string>()), users);

            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("{\"users\":{}}", Encoding.Default.GetString(service.ByteContent));
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
            var service = new UpdateUserRoleService(serializer, new Management.Core.Models.Stack(null, _fixture.Create<string>()), users);

            service.ContentBody();
            var roles = new List<string>();
            foreach (string role in user.Roles)
                roles.Add($"\"{role}\"");

            Assert.IsNotNull(service);
            Assert.AreEqual($"{{\"users\":{{\"{user.Uid}\":[{string.Join(",", roles)}]}}}}", Encoding.Default.GetString(service.ByteContent));
        }
    }
}
