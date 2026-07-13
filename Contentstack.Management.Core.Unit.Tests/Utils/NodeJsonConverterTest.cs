using System.Collections.Generic;
using System.Text.Json;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Utils
{
    [TestClass]
    public class NodeJsonConverterTest
    {
        private JsonSerializerOptions _options;

        [TestInitialize]
        public void Setup()
        {
            _options = new JsonSerializerOptions();
            _options.Converters.Add(new NodeJsonConverter());
            _options.Converters.Add(new TextNodeJsonConverter());
        }

        [TestMethod]
        public void NodeJsonConverter_Read_WithTypeProperty_ShouldCreateNode()
        {
            var json = @"{""type"":""paragraph"",""attrs"":{""class"":""test-class""},""children"":[]}";

            var result = JsonSerializer.Deserialize<Node>(json, _options);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Node));
            Assert.AreEqual("paragraph", result.type);
            Assert.IsNotNull(result.attrs);
            Assert.AreEqual("test-class", result.attrs["class"].ToString());
            Assert.IsNotNull(result.children);
        }

        [TestMethod]
        public void NodeJsonConverter_Read_WithoutTypeProperty_ShouldCreateTextNode()
        {
            var json = @"{""text"":""Hello World"",""bold"":true}";

            var result = JsonSerializer.Deserialize<Node>(json, _options);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TextNode));
            Assert.AreEqual("text", result.type);
        }

        [TestMethod]
        public void NodeJsonConverter_Write_WithNode_ShouldWriteCorrectJson()
        {
            var node = new Node
            {
                type = "paragraph",
                attrs = new Dictionary<string, object> { { "class", "test-class" } },
                children = new List<Node>()
            };

            var result = JsonSerializer.Serialize(node, _options);

            Assert.IsTrue(result.Contains("\"type\":\"paragraph\""));
            Assert.IsTrue(result.Contains("\"attrs\""));
            Assert.IsTrue(result.Contains("\"children\""));
        }

        [TestMethod]
        public void NodeJsonConverter_Write_WithNullAttrs_ShouldNotWriteAttrs()
        {
            var node = new Node
            {
                type = "paragraph",
                attrs = null,
                children = new List<Node>()
            };

            var result = JsonSerializer.Serialize(node, _options);

            Assert.IsTrue(result.Contains("\"type\":\"paragraph\""));
            Assert.IsFalse(result.Contains("\"attrs\""));
        }

        [TestMethod]
        public void NodeJsonConverter_Write_WithNullChildren_ShouldNotWriteChildren()
        {
            var node = new Node
            {
                type = "paragraph",
                attrs = new Dictionary<string, object>(),
                children = null
            };

            var result = JsonSerializer.Serialize(node, _options);

            Assert.IsTrue(result.Contains("\"type\":\"paragraph\""));
            Assert.IsFalse(result.Contains("\"children\""));
        }

        [TestMethod]
        public void NodeJsonConverter_Write_WithChildren_ShouldWriteChildrenArray()
        {
            var childNode = new Node { type = "text" };
            var node = new Node
            {
                type = "paragraph",
                attrs = new Dictionary<string, object>(),
                children = new List<Node> { childNode }
            };

            var result = JsonSerializer.Serialize(node, _options);

            Assert.IsTrue(result.Contains("\"children\""));
            Assert.IsTrue(result.Contains("["));
            Assert.IsTrue(result.Contains("]"));
        }
    }
}
