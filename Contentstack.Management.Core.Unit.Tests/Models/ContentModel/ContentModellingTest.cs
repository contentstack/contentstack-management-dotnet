using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models.ContentModel
{
    [TestClass]
    public class ContentModellingTest
    {
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

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

            var options = new ContentstackClient().SerializerOptions;
            string snippet = JsonSerializer.Serialize(contentModelling, options);
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

            var options = new ContentstackClient().SerializerOptions;
            string snippet = JsonSerializer.Serialize(contentModelling, options);
            Assert.IsNotNull(snippet);
        }
    }
}
