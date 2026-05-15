using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services.Organization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Organization
{
    [TestClass]
    public class UserInvitationServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserInvitationService(null, _fixture.Create<string>(), _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Null_UID()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserInvitationService(serializer, null, _fixture.Create<string>()));

        }

        [TestMethod]
        public void Should_Throw_On_Null_Method()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserInvitationService(serializer, _fixture.Create<string>(), null));

        }

        [TestMethod]
        public void Should_Initialize_with_Organization_Uid()
        {
            var orgUid = _fixture.Create<string>();
            var service = new UserInvitationService(serializer, orgUid);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual("/organizations/{organization_uid}/share", service.ResourcePath);
            Assert.AreEqual(orgUid, service.PathResources["{organization_uid}"]);
        }

        [TestMethod]
        public void Should_Initialize_with_Organization_Uid_And_Method()
        {
            var orgUid = _fixture.Create<string>();
            var service = new UserInvitationService(serializer, orgUid, "POST");

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/organizations/{organization_uid}/share", service.ResourcePath);
            Assert.AreEqual(orgUid, service.PathResources["{organization_uid}"]);
        }

        [TestMethod]
        public void Should_Pass_Null_Content_On_Get_Method()
        {
            var orgUid = _fixture.Create<string>();
            var service = new UserInvitationService(serializer, orgUid);

            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("", Encoding.Default.GetString(service.ByteContent));
        }

        [TestMethod]
        public void Should_Initialize_with_Serializer_Param_Collection()
        {
            var orgUid = _fixture.Create<string>();
            var collection = new Management.Core.Queryable.ParameterCollection();
            collection.Add(_fixture.Create<string>(), _fixture.Create<string>());
            var service = new UserInvitationService(serializer, orgUid, "GET", collection);

            Assert.IsNotNull(service);
            Assert.AreEqual(true, service.UseQueryString);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual("/organizations/{organization_uid}/share", service.ResourcePath);
            Assert.AreEqual(orgUid, service.PathResources["{organization_uid}"]);
        }

        [TestMethod]
        public void Should_Return_Content_Of_Post_Method()
        {
            var orgUid = _fixture.Create<string>();
            var service = new UserInvitationService(serializer, orgUid, "POST");

            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("{\"share\":{}}", Encoding.Default.GetString(service.ByteContent));
        }

        [TestMethod]
        public void Should_Return_Content_Organization_Invite_Of_Post_Method()
        {
            var orgUid = _fixture.Create<string>();
            var service = new UserInvitationService(serializer, orgUid, "POST");

            var userInvitation = new UserInvitation()
            {
                Email = _fixture.Create<string>(),
                Roles = _fixture.Create<List<string>>()
            };

            service.AddOrganizationInvite(new List<UserInvitation>() { userInvitation });
            service.ContentBody();
            var roles = new List<string>();
            foreach (string role in userInvitation.Roles)
                roles.Add($"\"{role}\"");

            Assert.IsNotNull(service);
            Assert.AreEqual($"{{\"share\":{{\"users\":{{\"{userInvitation.Email}\":[{string.Join(",", roles)}]}}}}}}", Encoding.Default.GetString(service.ByteContent));
        }

        [TestMethod]
        public void Should_Return_Content_Stack_Invite_Of_Post_Method()
        {
            var orgUid = _fixture.Create<string>();
            var service = new UserInvitationService(serializer, orgUid, "POST");

            var userInvitation = new UserInvitation()
            {
                Email = _fixture.Create<string>(),
                Roles = _fixture.Create<List<string>>()
            };
            var stackID = _fixture.Create<string>();

            var stackInvite = new Dictionary<string, List<UserInvitation>>();
            stackInvite.Add(stackID, new List<UserInvitation>() { userInvitation });

            service.AddStackInvite(stackInvite);
            service.ContentBody();
            var roles = new List<string>();
            foreach (string role in userInvitation.Roles)
                roles.Add($"\"{role}\"");

            Assert.IsNotNull(service);
            Assert.AreEqual($"{{\"share\":{{\"stacks\":{{\"{userInvitation.Email}\":{{\"{stackID}\":[{string.Join(",", roles)}]}}}}}}}}", Encoding.Default.GetString(service.ByteContent));
        }

        [TestMethod]
        public void Should_Return_Content_Organization_and_Stack_Invite_Of_Post_Method()
        {
            var orgUid = _fixture.Create<string>();
            var service = new UserInvitationService(serializer, orgUid, "POST");

            var userInvitation = new UserInvitation()
            {
                Email = _fixture.Create<string>(),
                Roles = _fixture.Create<List<string>>()
            };

            service.AddOrganizationInvite(new List<UserInvitation>() { userInvitation });
            service.ContentBody();
 
            var stackID = _fixture.Create<string>();

            var stackInvite = new Dictionary<string, List<UserInvitation>>();
            stackInvite.Add(stackID, new List<UserInvitation>() { userInvitation });

            service.AddStackInvite(stackInvite);
            service.ContentBody();

            var roles = new List<string>();
            foreach (string role in userInvitation.Roles)
                roles.Add($"\"{role}\"");

            Assert.IsNotNull(service);
            Assert.AreEqual($"{{\"share\":{{\"users\":{{\"{userInvitation.Email}\":[{string.Join(",", roles)}]}},\"stacks\":{{\"{userInvitation.Email}\":{{\"{stackID}\":[{string.Join(",", roles)}]}}}}}}}}", Encoding.Default.GetString(service.ByteContent));
        }

        [TestMethod]
        public void Should_Return_Content_Of_Delete_Method()
        {
            var orgUid = _fixture.Create<string>();
            var service = new UserInvitationService(serializer, orgUid, "DELETE");

            Assert.IsNotNull(service);
            service.ContentBody();

            Assert.AreEqual("", Encoding.Default.GetString(service.ByteContent));
        }

        [TestMethod]
        public void Should_Return_Content_User_Email_Delete_Method()
        {
            var orgUid = _fixture.Create<string>();
            var service = new UserInvitationService(serializer, orgUid, "DELETE");

            var emails = _fixture.Create<List<string>>();
            var email = new List<string>();
            foreach (string role in emails)
                email.Add($"\"{role}\"");

            service.RemoveUsers(emails);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual($"{{\"emails\":[{string.Join(",", email)}]}}", Encoding.Default.GetString(service.ByteContent));
        }
    }
}
