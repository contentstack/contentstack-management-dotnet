using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models.ContentModel
{
    [TestClass]
    public class ContentModellingTest
    {
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions();
        private IFixture _fixture;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<JsonElement>(() => JsonDocument.Parse("null").RootElement);
        }

        [TestMethod]
        public void Initialize_ContentModel()
        {
            ContentModelling contentModelling = new ContentModelling()
            {
                Title = _fixture.Create<string>(),
                Uid = _fixture.Create<string>(),
                Schema = _fixture.Create<List<Field>>(),
                FieldRules = _fixture.Create<List<FieldRules>>(),
                Options = _fixture.Create<Option>(),
            };

            string snippet = JsonSerializer.Serialize(contentModelling, serializerOptions);
            Assert.IsNotNull(snippet);
        }

        [TestMethod]
        public void Initialize_ContentModel_Custom_field()
        {
            List<Field> fields = new List<Field>()
            {
                _fixture.Create<DateField>(),
                _fixture.Create<ModularBlockField>(),
                _fixture.Create<ExtensionField>(),
                _fixture.Create<FileField>(),
                _fixture.Create<GroupField>(),
                _fixture.Create<ReferenceField>(),
                _fixture.Create<SelectField>(),
                _fixture.Create<TextboxField>(),
            };
            ContentModelling contentModelling = new ContentModelling()
            {
                Schema = fields,
            };

            string snippet = JsonSerializer.Serialize(contentModelling, serializerOptions);
            Assert.IsNotNull(snippet);
        }
    }
}
