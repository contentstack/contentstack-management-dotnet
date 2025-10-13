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
    public class NodeJsonConverterTest
    {
        private JsonSerializer _serializer;

        [TestInitialize]
        public void Setup()
        {
            _serializer = new JsonSerializer();
        }

        [TestMethod]
        public void NodeJsonConverter_ReadJson_WithTypeProperty_ShouldCreateNode()
        {
            var json = @"{
                ""type"": ""paragraph"",
                ""attrs"": { ""class"": ""test-class"" },
                ""children"": []
            }";
            var reader = new JsonTextReader(new System.IO.StringReader(json));
            var converter = new NodeJsonConverter();

            var result = converter.ReadJson(reader, typeof(Node), null, false, _serializer);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Node));
            Assert.AreEqual("paragraph", result.type);
            Assert.IsNotNull(result.attrs);
            Assert.AreEqual("test-class", result.attrs["class"]);
            Assert.IsNotNull(result.children);
        }

        [TestMethod]
        public void NodeJsonConverter_ReadJson_WithoutTypeProperty_ShouldCreateTextNode()
        {
            var json = @"{
                ""text"": ""Hello World"",
                ""bold"": true
            }";
            var reader = new JsonTextReader(new System.IO.StringReader(json));
            var converter = new NodeJsonConverter();

            var result = converter.ReadJson(reader, typeof(Node), null, false, _serializer);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TextNode));
            Assert.AreEqual("text", result.type);
        }

        [TestMethod]
        public void NodeJsonConverter_WriteJson_WithNode_ShouldWriteCorrectJson()
        {
            var node = new Node
            {
                type = "paragraph",
                attrs = new Dictionary<string, object> { { "class", "test-class" } },
                children = new List<Node>()
            };
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new NodeJsonConverter();

            converter.WriteJson(writer, node, _serializer);

            var result = stringWriter.ToString();
            Assert.IsTrue(result.Contains("\"type\":\"paragraph\""));
            Assert.IsTrue(result.Contains("\"attrs\""));
            Assert.IsTrue(result.Contains("\"children\""));
        }

        [TestMethod]
        public void NodeJsonConverter_WriteJson_WithNullAttrs_ShouldNotWriteAttrs()
        {
            var node = new Node
            {
                type = "paragraph",
                attrs = null,
                children = new List<Node>()
            };
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new NodeJsonConverter();

            converter.WriteJson(writer, node, _serializer);

            var result = stringWriter.ToString();
            Assert.IsTrue(result.Contains("\"type\":\"paragraph\""));
            Assert.IsFalse(result.Contains("\"attrs\""));
        }

        [TestMethod]
        public void NodeJsonConverter_WriteJson_WithNullChildren_ShouldNotWriteChildren()
        {
            var node = new Node
            {
                type = "paragraph",
                attrs = new Dictionary<string, object>(),
                children = null
            };
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new NodeJsonConverter();

            converter.WriteJson(writer, node, _serializer);

            var result = stringWriter.ToString();
            Assert.IsTrue(result.Contains("\"type\":\"paragraph\""));
            Assert.IsFalse(result.Contains("\"children\""));
        }

        [TestMethod]
        public void NodeJsonConverter_WriteJson_WithChildren_ShouldWriteChildrenArray()
        {
            var childNode = new Node { type = "text" };
            var node = new Node
            {
                type = "paragraph",
                attrs = new Dictionary<string, object>(),
                children = new List<Node> { childNode }
            };
            var stringWriter = new System.IO.StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            var converter = new NodeJsonConverter();

            converter.WriteJson(writer, node, _serializer);

            var result = stringWriter.ToString();
            Assert.IsTrue(result.Contains("\"children\""));
            Assert.IsTrue(result.Contains("["));
            Assert.IsTrue(result.Contains("]"));
        }
    }
}
