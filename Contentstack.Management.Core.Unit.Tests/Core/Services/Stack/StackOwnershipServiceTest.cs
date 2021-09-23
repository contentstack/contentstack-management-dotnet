using System;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Services.Stack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Stack
{
    [TestClass]
    public class StackOwnershipServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackOwnershipService(null, _fixture.Create<string>(), _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Null_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackOwnershipService(serializer, null, _fixture.Create<string>()));

        }

        [TestMethod]
        public void Should_Throw_On_Null_Email()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackOwnershipService(serializer, _fixture.Create<string>(), null));
        }

        [TestMethod]
        public void Should_Initialize_with_Organization_Uid()
        {
            var apiKey = _fixture.Create<string>();
            var email = _fixture.Create<string>();
            var service = new StackOwnershipService(serializer, apiKey, email);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("stacks/transfer_ownership", service.ResourcePath);
            Assert.AreEqual(apiKey, service.Headers["api_key"]);
        }

        [TestMethod]
        public void Should_Return_Content_Of_Post_Method()
        {
            var apiKey = _fixture.Create<string>();
            var email = _fixture.Create<string>();
            var service = new StackOwnershipService(serializer, apiKey, email);

            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual($"{{\"transfer_to\":\"{email}\"}}", Encoding.Default.GetString(service.Content));
        }
    }
}
