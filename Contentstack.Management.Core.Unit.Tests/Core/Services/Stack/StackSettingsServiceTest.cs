using System;
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
    public class StackSettingsServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackSettingsService(null, _fixture.Create<string>()));

        }

        [TestMethod]
        public void Should_Throw_On_Null_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StackSettingsService(serializer, null));
        }

        [TestMethod]
        public void Should_Initialize_With_Get_Method()
        {
            var apiKey = _fixture.Create<string>();
            var service = new StackSettingsService(serializer, apiKey);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual("stacks/settings", service.ResourcePath);
            Assert.AreEqual(apiKey, service.Headers["api_key"]);
            Assert.IsNull(service.Content);
        }

        [TestMethod]
        public void Should_Return_Stack_Settings_Content()
        {
            var apiKey = _fixture.Create<string>();
            var settings = _fixture.Create<StackSettings>();
            var service = new StackSettingsService(serializer, apiKey, "POST", settings);
            service.ContentBody();

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("stacks/settings", service.ResourcePath);
            Assert.AreEqual(apiKey, service.Headers["api_key"]);
            var json = JsonConvert.SerializeObject(settings);

            Assert.AreEqual($"{{\"stack_settings\":{json.ToString()}}}", Encoding.Default.GetString(service.Content));
        }
    }
}
