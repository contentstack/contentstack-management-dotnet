using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Utils
{
    [TestClass]
    public class TextNodeJsonConverterTest
    {
        private JsonSerializer _serializer;

        [TestInitialize]
        public void Setup()
        {
            _serializer = new JsonSerializer();
        }

        [TestMethod]
        public void TextNodeJsonConverter_ReadJson_ShouldCreateTextNode()
        {
            var json = @"{
                ""text"": ""Hello World"",
                ""bold"": true,
                ""italic"": false,
                ""underline"": true,
                ""strikethrough"": false,
                ""inlineCode"": true,
                ""subscript"": false,
                ""superscript"": true,
                ""break"": false
            }";
            var reader = new JsonTextReader(new System.IO.StringReader(json));
            var converter = new TextNodeJsonConverter();

            var result = converter.ReadJson(reader, typeof(TextNode), null, false, _serializer);

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
        public void TextNodeJsonConverter_WriteJson_WithAllProperties_ShouldWriteCorrectJson()
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
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new TextNodeJsonConverter();

            converter.WriteJson(writer, textNode, _serializer);

            var result = stringWriter.ToString();
            Assert.IsTrue(result.Contains("\"text\":\"Hello World\""));
            Assert.IsTrue(result.Contains("\"bold\":true"));
            // italic is false, so it won't be written by the converter
            Assert.IsTrue(result.Contains("\"underline\":true"));
            // strikethrough is false, so it won't be written by the converter
            Assert.IsTrue(result.Contains("\"inlineCode\":true"));
            // subscript is false, so it won't be written by the converter
            Assert.IsTrue(result.Contains("\"superscript\":true"));
            // break is false, so it won't be written by the converter
            Assert.IsTrue(result.Contains("\"attrs\""));
            Assert.IsTrue(result.Contains("\"children\""));
        }

        [TestMethod]
        public void TextNodeJsonConverter_WriteJson_WithOnlyText_ShouldWriteOnlyText()
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
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new TextNodeJsonConverter();

            converter.WriteJson(writer, textNode, _serializer);

            var result = stringWriter.ToString();
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
        public void TextNodeJsonConverter_WriteJson_WithNullText_ShouldNotWriteText()
        {
            var textNode = new TextNode
            {
                text = null,
                bold = true,
                italic = false,
                underline = false,
                strikethrough = false,
                inlineCode = false,
                subscript = false,
                superscript = false,
                @break = false
            };
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new TextNodeJsonConverter();

            converter.WriteJson(writer, textNode, _serializer);

            var result = stringWriter.ToString();
            Assert.IsFalse(result.Contains("\"text\""));
            Assert.IsTrue(result.Contains("\"bold\":true"));
        }

        [TestMethod]
        public void TextNodeJsonConverter_WriteJson_WithEmptyText_ShouldNotWriteText()
        {
            var textNode = new TextNode
            {
                text = "",
                bold = true,
                italic = false,
                underline = false,
                strikethrough = false,
                inlineCode = false,
                subscript = false,
                superscript = false,
                @break = false
            };
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new TextNodeJsonConverter();

            converter.WriteJson(writer, textNode, _serializer);

            var result = stringWriter.ToString();
            Assert.IsFalse(result.Contains("\"text\""));
            Assert.IsTrue(result.Contains("\"bold\":true"));
        }

        [TestMethod]
        public void TextNodeJsonConverter_WriteJson_WithNullAttrs_ShouldNotWriteAttrs()
        {
            var textNode = new TextNode
            {
                text = "Test",
                attrs = null,
                children = new List<Node>()
            };
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new TextNodeJsonConverter();

            converter.WriteJson(writer, textNode, _serializer);

            var result = stringWriter.ToString();
            Assert.IsFalse(result.Contains("\"attrs\""));
        }

        [TestMethod]
        public void TextNodeJsonConverter_WriteJson_WithNullChildren_ShouldNotWriteChildren()
        {
            var textNode = new TextNode
            {
                text = "Test",
                attrs = new Dictionary<string, object>(),
                children = null
            };
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new TextNodeJsonConverter();

            converter.WriteJson(writer, textNode, _serializer);

            var result = stringWriter.ToString();
            Assert.IsFalse(result.Contains("\"children\""));
        }
    }
}
