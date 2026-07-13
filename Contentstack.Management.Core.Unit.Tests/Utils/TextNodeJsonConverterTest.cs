using System.Collections.Generic;
using System.Text.Json;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Utils
{
    [TestClass]
    public class TextNodeJsonConverterTest
    {
        private JsonSerializerOptions _options;

        [TestInitialize]
        public void Setup()
        {
            _options = new JsonSerializerOptions();
            _options.Converters.Add(new TextNodeJsonConverter());
            _options.Converters.Add(new NodeJsonConverter());
        }

        [TestMethod]
        public void TextNodeJsonConverter_Read_ShouldCreateTextNode()
        {
            var json = @"{""text"":""Hello World"",""bold"":true,""italic"":false,""underline"":true,""strikethrough"":false,""inlineCode"":true,""subscript"":false,""superscript"":true,""break"":false}";

            var result = JsonSerializer.Deserialize<TextNode>(json, _options);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TextNode));
            Assert.AreEqual("Hello World", result.text);
            Assert.IsTrue(result.bold);
            Assert.IsFalse(result.italic);
            Assert.IsTrue(result.underline);
            Assert.IsFalse(result.strikethrough);
            Assert.IsTrue(result.inlineCode);
            Assert.IsFalse(result.subscript);
            Assert.IsTrue(result.superscript);
            Assert.IsFalse(result.@break);
        }

        [TestMethod]
        public void TextNodeJsonConverter_Write_WithAllProperties_ShouldWriteCorrectJson()
        {
            var textNode = new TextNode
            {
                text = "Hello World",
                bold = true,
                italic = false,
                underline = true,
                strikethrough = false,
                inlineCode = true,
                subscript = false,
                superscript = true,
                @break = false,
                attrs = new Dictionary<string, object> { { "class", "test-class" } },
                children = new List<Node>()
            };

            var result = JsonSerializer.Serialize(textNode, _options);

            Assert.IsTrue(result.Contains("\"text\":\"Hello World\""));
            Assert.IsTrue(result.Contains("\"bold\":true"));
            Assert.IsTrue(result.Contains("\"underline\":true"));
            Assert.IsTrue(result.Contains("\"inlineCode\":true"));
            Assert.IsTrue(result.Contains("\"superscript\":true"));
            Assert.IsTrue(result.Contains("\"attrs\""));
            Assert.IsTrue(result.Contains("\"children\""));
        }

        [TestMethod]
        public void TextNodeJsonConverter_Write_WithOnlyText_ShouldWriteOnlyText()
        {
            var textNode = new TextNode
            {
                text = "Simple text",
                bold = false,
                italic = false,
                underline = false,
                strikethrough = false,
                inlineCode = false,
                subscript = false,
                superscript = false,
                @break = false
            };

            var result = JsonSerializer.Serialize(textNode, _options);

            Assert.IsTrue(result.Contains("\"text\":\"Simple text\""));
            Assert.IsFalse(result.Contains("\"bold\""));
            Assert.IsFalse(result.Contains("\"italic\""));
            Assert.IsFalse(result.Contains("\"underline\""));
            Assert.IsFalse(result.Contains("\"strikethrough\""));
            Assert.IsFalse(result.Contains("\"inlineCode\""));
            Assert.IsFalse(result.Contains("\"subscript\""));
            Assert.IsFalse(result.Contains("\"superscript\""));
            Assert.IsFalse(result.Contains("\"break\""));
        }

        [TestMethod]
        public void TextNodeJsonConverter_Write_WithNullText_ShouldNotWriteText()
        {
            var textNode = new TextNode
            {
                text = null,
                bold = true
            };

            var result = JsonSerializer.Serialize(textNode, _options);

            Assert.IsFalse(result.Contains("\"text\""));
            Assert.IsTrue(result.Contains("\"bold\":true"));
        }

        [TestMethod]
        public void TextNodeJsonConverter_Write_WithEmptyText_ShouldNotWriteText()
        {
            var textNode = new TextNode
            {
                text = "",
                bold = true
            };

            var result = JsonSerializer.Serialize(textNode, _options);

            Assert.IsFalse(result.Contains("\"text\""));
            Assert.IsTrue(result.Contains("\"bold\":true"));
        }

        [TestMethod]
        public void TextNodeJsonConverter_Write_WithNullAttrs_ShouldNotWriteAttrs()
        {
            var textNode = new TextNode
            {
                text = "Test",
                attrs = null,
                children = new List<Node>()
            };

            var result = JsonSerializer.Serialize(textNode, _options);

            Assert.IsFalse(result.Contains("\"attrs\""));
        }

        [TestMethod]
        public void TextNodeJsonConverter_Write_WithNullChildren_ShouldNotWriteChildren()
        {
            var textNode = new TextNode
            {
                text = "Test",
                attrs = new Dictionary<string, object>(),
                children = null
            };

            var result = JsonSerializer.Serialize(textNode, _options);

            Assert.IsFalse(result.Contains("\"children\""));
        }
    }
}
