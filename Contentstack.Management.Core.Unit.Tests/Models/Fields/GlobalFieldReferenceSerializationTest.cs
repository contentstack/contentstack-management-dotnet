using System;
using AutoFixture;
using Contentstack.Management.Core.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models.Fields
{
    [TestClass]
    public class GlobalFieldReferenceSerializationTest
    {
        private readonly IFixture _fixture = new Fixture();

        [TestMethod]
        public void Should_Serialize_GlobalFieldReference_To_JSON()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Product Information",
                Uid = "product_info",
                DataType = "global_field",
                ReferenceTo = "referenced_global_field_uid",
                Mandatory = true,
                Multiple = false,
                Unique = false,
                NonLocalizable = false,
                FieldMetadata = new FieldMetadata
                {
                    Description = "Reference to product information global field"
                }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"display_name\":\"Product Information\""));
            Assert.IsTrue(json.Contains("\"uid\":\"product_info\""));
            Assert.IsTrue(json.Contains("\"data_type\":\"global_field\""));
            Assert.IsTrue(json.Contains("\"reference_to\":\"referenced_global_field_uid\""));
            Assert.IsTrue(json.Contains("\"mandatory\":true"));
            Assert.IsTrue(json.Contains("\"multiple\":false"));
            Assert.IsTrue(json.Contains("\"unique\":false"));
            Assert.IsTrue(json.Contains("\"non_localizable\":false"));
        }

        [TestMethod]
        public void Should_Deserialize_JSON_To_GlobalFieldReference()
        {
            
            var json = @"{
                ""display_name"": ""Product Information"",
                ""uid"": ""product_info"",
                ""data_type"": ""global_field"",
                ""reference_to"": ""referenced_global_field_uid"",
                ""mandatory"": true,
                ""multiple"": false,
                ""unique"": false,
                ""non_localizable"": false,
                ""field_metadata"": {
                    ""description"": ""Reference to product information global field""
                }
            }";

            
            var globalFieldRef = JsonConvert.DeserializeObject<GlobalFieldReference>(json);

            Assert.IsNotNull(globalFieldRef);
            Assert.AreEqual("Product Information", globalFieldRef.DisplayName);
            Assert.AreEqual("product_info", globalFieldRef.Uid);
            Assert.AreEqual("global_field", globalFieldRef.DataType);
            Assert.AreEqual("referenced_global_field_uid", globalFieldRef.ReferenceTo);
            Assert.IsTrue(globalFieldRef.Mandatory);
            Assert.IsFalse(globalFieldRef.Multiple);
            Assert.IsFalse(globalFieldRef.Unique);
            Assert.IsFalse(globalFieldRef.NonLocalizable);
            Assert.IsNotNull(globalFieldRef.FieldMetadata);
            Assert.AreEqual("Reference to product information global field", globalFieldRef.FieldMetadata.Description);
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldReference_With_Multiple_True()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Multiple Products",
                Uid = "multiple_products",
                DataType = "global_field",
                ReferenceTo = "product_info",
                Mandatory = false,
                Multiple = true,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"multiple\":true"));
            Assert.IsTrue(json.Contains("\"mandatory\":false"));
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldReference_With_Unique_True()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Unique Product",
                Uid = "unique_product",
                DataType = "global_field",
                ReferenceTo = "product_info",
                Mandatory = true,
                Multiple = false,
                Unique = true,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"unique\":true"));
            Assert.IsTrue(json.Contains("\"mandatory\":true"));
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldReference_With_NonLocalizable_True()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Non Localizable Product",
                Uid = "non_localizable_product",
                DataType = "global_field",
                ReferenceTo = "product_info",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = true
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"non_localizable\":true"));
        }

        [TestMethod]
        public void Should_Handle_Null_ReferenceTo()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Test Field",
                Uid = "test_field",
                DataType = "global_field",
                ReferenceTo = null,
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            // Null values are ignored due to ItemNullValueHandling = NullValueHandling.Ignore
            Assert.IsFalse(json.Contains("\"reference_to\""));
        }

        [TestMethod]
        public void Should_Handle_Empty_ReferenceTo()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Test Field",
                Uid = "test_field",
                DataType = "global_field",
                ReferenceTo = "",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            // Empty strings are still serialized
            Assert.IsTrue(json.Contains("\"reference_to\":\"\""));
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldReference_With_Complex_FieldMetadata()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Complex Product Reference",
                Uid = "complex_product_ref",
                DataType = "global_field",
                ReferenceTo = "complex_product",
                Mandatory = true,
                Multiple = true,
                Unique = true,
                NonLocalizable = false,
                FieldMetadata = new FieldMetadata
                {
                    Description = "Complex product reference with rich metadata",
                    DefaultValue = "default_product",
                    Version = 3,
                    AllowRichText = true,
                    Multiline = false,
                    RichTextType = "advanced"
                }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"field_metadata\""));
            Assert.IsTrue(json.Contains("\"description\":\"Complex product reference with rich metadata\""));
            Assert.IsTrue(json.Contains("\"default_value\":\"default_product\""));
            Assert.IsTrue(json.Contains("\"version\":3"));
            Assert.IsTrue(json.Contains("\"allow_rich_text\":true"));
            Assert.IsTrue(json.Contains("\"multiline\":false"));
            Assert.IsTrue(json.Contains("\"rich_text_type\":\"advanced\""));
        }

        [TestMethod]
        public void Should_Deserialize_Complex_JSON_To_GlobalFieldReference()
        {
            
            var json = @"{
                ""display_name"": ""Complex Product Reference"",
                ""uid"": ""complex_product_ref"",
                ""data_type"": ""global_field"",
                ""reference_to"": ""complex_product"",
                ""mandatory"": true,
                ""multiple"": true,
                ""unique"": true,
                ""non_localizable"": false,
                ""field_metadata"": {
                    ""description"": ""Complex product reference with rich metadata"",
                    ""default_value"": ""default_product"",
                    ""version"": 3,
                    ""allow_rich_text"": true,
                    ""multiline"": false,
                    ""rich_text_type"": ""advanced""
                }
            }";

            
            var globalFieldRef = JsonConvert.DeserializeObject<GlobalFieldReference>(json);

            Assert.IsNotNull(globalFieldRef);
            Assert.AreEqual("Complex Product Reference", globalFieldRef.DisplayName);
            Assert.AreEqual("complex_product_ref", globalFieldRef.Uid);
            Assert.AreEqual("global_field", globalFieldRef.DataType);
            Assert.AreEqual("complex_product", globalFieldRef.ReferenceTo);
            Assert.IsTrue(globalFieldRef.Mandatory);
            Assert.IsTrue(globalFieldRef.Multiple);
            Assert.IsTrue(globalFieldRef.Unique);
            Assert.IsFalse(globalFieldRef.NonLocalizable);
            
            Assert.IsNotNull(globalFieldRef.FieldMetadata);
            Assert.AreEqual("Complex product reference with rich metadata", globalFieldRef.FieldMetadata.Description);
            Assert.AreEqual("default_product", globalFieldRef.FieldMetadata.DefaultValue);
            Assert.AreEqual(3, globalFieldRef.FieldMetadata.Version);
            Assert.IsTrue(globalFieldRef.FieldMetadata.AllowRichText);
            Assert.IsFalse(globalFieldRef.FieldMetadata.Multiline);
            Assert.AreEqual("advanced", globalFieldRef.FieldMetadata.RichTextType);
        }
    }
} 