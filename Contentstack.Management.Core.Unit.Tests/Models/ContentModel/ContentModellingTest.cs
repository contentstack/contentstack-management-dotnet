using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models.ContentModel
{
    [TestClass]
    public class ContentModellingTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
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

            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);

                serializer.Serialize(writer, contentModelling);
                string snippet = stringWriter.ToString();
                Assert.IsNotNull(snippet);
            }
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

            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);

                serializer.Serialize(writer, contentModelling);
                string snippet = stringWriter.ToString();
                Assert.IsNotNull(snippet);
            }
        }
    }
}
