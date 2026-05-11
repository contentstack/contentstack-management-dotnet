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
        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new NodeJsonConverter());
            options.Converters.Add(new TextNodeJsonConverter());
            return options;
        }

        [TestMethod]
        public void NodeJsonConverter_ReadJson_WithTypeProperty_ShouldCreateNode()
        {
            var json = @"{
                ""type"": ""paragraph"",
                ""attrs"": { ""class"": ""test-class"" },
                ""children"": []
            }";
            var result = JsonSerializer.Deserialize<Node>(json, CreateOptions());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Node));
            Assert.AreEqual("paragraph", result.type);
            Assert.IsNotNull(result.attrs);
            var classVal = result.attrs["class"];
            var classStr = classVal is JsonElement je ? je.GetString() : classVal?.ToString();
            Assert.AreEqual("test-class", classStr);
            Assert.IsNotNull(result.children);
        }

        [TestMethod]
        public void NodeJsonConverter_ReadJson_WithoutTypeProperty_ShouldCreateTextNode()
        {
            var json = @"{
                ""text"": ""Hello World"",
                ""bold"": true
            }";
            var result = JsonSerializer.Deserialize<Node>(json, CreateOptions());

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
            var json = JsonSerializer.Serialize(node, CreateOptions());

            Assert.IsTrue(json.Contains("\"type\":\"paragraph\""));
            Assert.IsTrue(json.Contains("\"attrs\""));
            Assert.IsTrue(json.Contains("\"children\""));
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
            var json = JsonSerializer.Serialize(node, CreateOptions());

            Assert.IsTrue(json.Contains("\"type\":\"paragraph\""));
            Assert.IsFalse(json.Contains("\"attrs\""));
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
            var json = JsonSerializer.Serialize(node, CreateOptions());

            Assert.IsTrue(json.Contains("\"type\":\"paragraph\""));
            Assert.IsFalse(json.Contains("\"children\""));
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
            var json = JsonSerializer.Serialize(node, CreateOptions());

            Assert.IsTrue(json.Contains("\"children\""));
            Assert.IsTrue(json.Contains("["));
            Assert.IsTrue(json.Contains("]"));
        }
    }
}
