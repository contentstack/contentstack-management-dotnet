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
    public class StackCreateUpdateServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackCreateUpdateService(
                null,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_API_Key_And_Organization_Uid_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackCreateUpdateService(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                null));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Name()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackCreateUpdateService(
                serializer,
                new Management.Core.Models.Stack(null),
                null,
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_On_Master_Locale_Null_Exception_On_Stack_Creation()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackCreateUpdateService(
                serializer,
                new Management.Core.Models.Stack(null),
                _fixture.Create<string>(),
                null,
                _fixture.Create<string>(),
                _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Create_Stack_With_Name_And_Locale()
        {
            var name = _fixture.Create<string>();
            var masterLocale = _fixture.Create<string>();
            var orgUID = _fixture.Create<string>();
            var service = new StackCreateUpdateService(serializer, new Management.Core.Models.Stack(null), name, masterLocale, organizationUid: orgUID);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/stacks", service.ResourcePath);
            Assert.AreEqual($"{{\"stack\":{{\"name\":\"{name}\",\"master_locale\":\"{masterLocale}\"}}}}", Encoding.Default.GetString(service.Content));
        }

        [TestMethod]
        public void Should_Create_Stack_With_Name_Description_And_Locale()
        {
            var name = _fixture.Create<string>();
            var masterLocale = _fixture.Create<string>();
            var desc = _fixture.Create<string>();
            var orgUID = _fixture.Create<string>();
            var service = new StackCreateUpdateService(serializer, new Management.Core.Models.Stack(null), name, masterLocale, desc, organizationUid: orgUID);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/stacks", service.ResourcePath);
            Assert.AreEqual($"{{\"stack\":{{\"name\":\"{name}\",\"description\":\"{desc}\",\"master_locale\":\"{masterLocale}\"}}}}", Encoding.Default.GetString(service.Content));
        }

        [TestMethod]
        public void Should_Update_Stack_With_Name()
        {
            var name = _fixture.Create<string>();
            var apiKey = _fixture.Create<string>();
            var service = new StackCreateUpdateService(serializer, new Management.Core.Models.Stack(null, apiKey), name);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("PUT", service.HttpMethod);
            Assert.AreEqual("/stacks", service.ResourcePath);
            Assert.AreEqual($"{{\"stack\":{{\"name\":\"{name}\"}}}}", Encoding.Default.GetString(service.Content));
        }

        [TestMethod]
        public void Should_Update_Stack_With_Name_Description()
        {
            var name = _fixture.Create<string>();
            var desc = _fixture.Create<string>();
            var apiKey = _fixture.Create<string>();
            var service = new StackCreateUpdateService(serializer, new Management.Core.Models.Stack(null, apiKey), name, description: desc);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("PUT", service.HttpMethod);
            Assert.AreEqual("/stacks", service.ResourcePath);
            Assert.AreEqual($"{{\"stack\":{{\"name\":\"{name}\",\"description\":\"{desc}\"}}}}", Encoding.Default.GetString(service.Content));
        }
    }
}
