using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class ExtensionModelTest
    {
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Initialize_ExtensionModel_For_URL()
        {
            ExtensionModel extensionModel = new ExtensionModel()
            {
                Title = _fixture.Create<string>(),
                Tags = _fixture.Create<List<string>>(),
                Src = _fixture.Create<string>(),
                DataType = _fixture.Create<string>(),
                Type = _fixture.Create<string>(),
                Config = _fixture.Create<string>(),
                Multiple = _fixture.Create<bool>(),
                Scope = _fixture.Create<ExtensionScope>()
            };

            var options = new ContentstackClient().SerializerOptions;
            string snippet = JsonSerializer.Serialize(extensionModel, options);
            Assert.IsNotNull(snippet);
        }

        [TestMethod]
        public void Initialize_ExtensionModel_For_Source_Code()
        {
            ExtensionModel extensionModel = new ExtensionModel()
            {
                Title = _fixture.Create<string>(),
                Tags = _fixture.Create<List<string>>(),
                Srcdoc = _fixture.Create<string>(),
                DataType = _fixture.Create<string>(),
                Type = _fixture.Create<string>(),
                Config = _fixture.Create<string>(),
                Multiple = _fixture.Create<bool>(),
                Scope = _fixture.Create<ExtensionScope>()
            };

            var options = new ContentstackClient().SerializerOptions;
            string snippet = JsonSerializer.Serialize(extensionModel, options);
            Assert.IsNotNull(snippet);
        }
    }
}
