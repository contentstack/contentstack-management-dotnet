using System;
using System.Collections.Generic;
using AutoFixture;
using Contentstack.Management.Core.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models.Fields
{
    [TestClass]
    public class GlobalFieldReferenceValidationTest
    {
        private readonly IFixture _fixture = new Fixture();

        [TestMethod]
        public void Should_Validate_GlobalFieldReference_With_Valid_Data()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Valid Product Reference",
                Uid = "valid_product_ref",
                DataType = "global_field",
                ReferenceTo = "valid_global_field_uid",
                Mandatory = true,
                Multiple = false,
                Unique = false,
                NonLocalizable = false,
                FieldMetadata = new FieldMetadata
                {
                    Description = "Valid reference to global field"
                }
            };

             
            Assert.IsNotNull(globalFieldRef);
            Assert.AreEqual("global_field", globalFieldRef.DataType);
            Assert.IsFalse(string.IsNullOrEmpty(globalFieldRef.ReferenceTo));
            Assert.IsNotNull(globalFieldRef.FieldMetadata);
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldReference_With_Empty_DisplayName()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "",
                Uid = "test_field",
                DataType = "global_field",
                ReferenceTo = "referenced_field",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"display_name\":\"\""));
            Assert.AreEqual("", globalFieldRef.DisplayName);
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldReference_With_Null_DisplayName()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = null,
                Uid = "test_field",
                DataType = "global_field",
                ReferenceTo = "referenced_field",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            // Null values are ignored due to ItemNullValueHandling = NullValueHandling.Ignore
            Assert.IsFalse(json.Contains("\"display_name\""));
            Assert.IsNull(globalFieldRef.DisplayName);
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldReference_With_Empty_Uid()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Test Field",
                Uid = "",
                DataType = "global_field",
                ReferenceTo = "referenced_field",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"uid\":\"\""));
            Assert.AreEqual("", globalFieldRef.Uid);
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldReference_With_Null_Uid()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Test Field",
                Uid = null,
                DataType = "global_field",
                ReferenceTo = "referenced_field",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            // Null values are ignored due to ItemNullValueHandling = NullValueHandling.Ignore
            Assert.IsFalse(json.Contains("\"uid\""));
            Assert.IsNull(globalFieldRef.Uid);
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldReference_With_Empty_ReferenceTo()
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

            Assert.IsTrue(json.Contains("\"reference_to\":\"\""));
            Assert.AreEqual("", globalFieldRef.ReferenceTo);
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldReference_With_Null_ReferenceTo()
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
            Assert.IsNull(globalFieldRef.ReferenceTo);
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldReference_With_Null_FieldMetadata()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Test Field",
                Uid = "test_field",
                DataType = "global_field",
                ReferenceTo = "referenced_field",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false,
                FieldMetadata = null
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            // Null values are ignored due to ItemNullValueHandling = NullValueHandling.Ignore
            Assert.IsFalse(json.Contains("\"field_metadata\""));
            Assert.IsNull(globalFieldRef.FieldMetadata);
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldReference_With_Empty_FieldMetadata()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Test Field",
                Uid = "test_field",
                DataType = "global_field",
                ReferenceTo = "referenced_field",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false,
                FieldMetadata = new FieldMetadata()
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"field_metadata\""));
            Assert.IsNotNull(globalFieldRef.FieldMetadata);
        }

        [TestMethod]
        public void Should_Validate_GlobalFieldReference_With_Special_Characters_In_Uid()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Special Characters Test",
                Uid = "test-field_with_underscores.and.dots",
                DataType = "global_field",
                ReferenceTo = "referenced-field_with_underscores.and.dots",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"uid\":\"test-field_with_underscores.and.dots\""));
            Assert.IsTrue(json.Contains("\"reference_to\":\"referenced-field_with_underscores.and.dots\""));
            Assert.AreEqual("test-field_with_underscores.and.dots", globalFieldRef.Uid);
            Assert.AreEqual("referenced-field_with_underscores.and.dots", globalFieldRef.ReferenceTo);
        }

        [TestMethod]
        public void Should_Validate_GlobalFieldReference_With_Unicode_Characters()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Unicode Test 中文",
                Uid = "test_field_中文",
                DataType = "global_field",
                ReferenceTo = "referenced_field_中文",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"display_name\":\"Unicode Test 中文\""));
            Assert.IsTrue(json.Contains("\"uid\":\"test_field_中文\""));
            Assert.IsTrue(json.Contains("\"reference_to\":\"referenced_field_中文\""));
            Assert.AreEqual("Unicode Test 中文", globalFieldRef.DisplayName);
            Assert.AreEqual("test_field_中文", globalFieldRef.Uid);
            Assert.AreEqual("referenced_field_中文", globalFieldRef.ReferenceTo);
        }

        [TestMethod]
        public void Should_Validate_GlobalFieldReference_With_Long_Strings()
        {
            
            var longString = new string('a', 1000);
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = longString,
                Uid = longString,
                DataType = "global_field",
                ReferenceTo = longString,
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false,
                FieldMetadata = new FieldMetadata
                {
                    Description = longString
                }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains($"\"display_name\":\"{longString}\""));
            Assert.IsTrue(json.Contains($"\"uid\":\"{longString}\""));
            Assert.IsTrue(json.Contains($"\"reference_to\":\"{longString}\""));
            Assert.AreEqual(longString, globalFieldRef.DisplayName);
            Assert.AreEqual(longString, globalFieldRef.Uid);
            Assert.AreEqual(longString, globalFieldRef.ReferenceTo);
        }

        [TestMethod]
        public void Should_Validate_GlobalFieldReference_With_All_Boolean_Combinations()
        {
            // Test all combinations of boolean properties
            var combinations = new[]
            {
                new { Mandatory = false, Multiple = false, Unique = false, NonLocalizable = false },
                new { Mandatory = true, Multiple = false, Unique = false, NonLocalizable = false },
                new { Mandatory = false, Multiple = true, Unique = false, NonLocalizable = false },
                new { Mandatory = false, Multiple = false, Unique = true, NonLocalizable = false },
                new { Mandatory = false, Multiple = false, Unique = false, NonLocalizable = true },
                new { Mandatory = true, Multiple = true, Unique = false, NonLocalizable = false },
                new { Mandatory = true, Multiple = false, Unique = true, NonLocalizable = false },
                new { Mandatory = true, Multiple = false, Unique = false, NonLocalizable = true },
                new { Mandatory = false, Multiple = true, Unique = true, NonLocalizable = false },
                new { Mandatory = false, Multiple = true, Unique = false, NonLocalizable = true },
                new { Mandatory = false, Multiple = false, Unique = true, NonLocalizable = true },
                new { Mandatory = true, Multiple = true, Unique = true, NonLocalizable = false },
                new { Mandatory = true, Multiple = true, Unique = false, NonLocalizable = true },
                new { Mandatory = true, Multiple = false, Unique = true, NonLocalizable = true },
                new { Mandatory = false, Multiple = true, Unique = true, NonLocalizable = true },
                new { Mandatory = true, Multiple = true, Unique = true, NonLocalizable = true }
            };

            foreach (var combo in combinations)
            {
                
                var globalFieldRef = new GlobalFieldReference
                {
                    DisplayName = "Boolean Test",
                    Uid = "boolean_test",
                    DataType = "global_field",
                    ReferenceTo = "referenced_field",
                    Mandatory = combo.Mandatory,
                    Multiple = combo.Multiple,
                    Unique = combo.Unique,
                    NonLocalizable = combo.NonLocalizable
                };

                
                var json = JsonConvert.SerializeObject(globalFieldRef);

                Assert.AreEqual(combo.Mandatory, globalFieldRef.Mandatory);
                Assert.AreEqual(combo.Multiple, globalFieldRef.Multiple);
                Assert.AreEqual(combo.Unique, globalFieldRef.Unique);
                Assert.AreEqual(combo.NonLocalizable, globalFieldRef.NonLocalizable);

                Assert.IsTrue(json.Contains($"\"mandatory\":{combo.Mandatory.ToString().ToLower()}"));
                Assert.IsTrue(json.Contains($"\"multiple\":{combo.Multiple.ToString().ToLower()}"));
                Assert.IsTrue(json.Contains($"\"unique\":{combo.Unique.ToString().ToLower()}"));
                Assert.IsTrue(json.Contains($"\"non_localizable\":{combo.NonLocalizable.ToString().ToLower()}"));
            }
        }

        [TestMethod]
        public void Should_Validate_GlobalFieldReference_With_Complex_FieldMetadata()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Complex Metadata Test",
                Uid = "complex_metadata_test",
                DataType = "global_field",
                ReferenceTo = "referenced_field",
                Mandatory = true,
                Multiple = true,
                Unique = true,
                NonLocalizable = false,
                FieldMetadata = new FieldMetadata
                {
                    Description = "Complex metadata description",
                    DefaultValue = "default_value",
                    Version = 3,
                    AllowRichText = true,
                    Multiline = false,
                    RichTextType = "advanced",
                    Options = new List<string> { "option1", "option2", "option3" }
                }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsNotNull(globalFieldRef.FieldMetadata);
            Assert.AreEqual("Complex metadata description", globalFieldRef.FieldMetadata.Description);
            Assert.AreEqual("default_value", globalFieldRef.FieldMetadata.DefaultValue);
            Assert.AreEqual(3, globalFieldRef.FieldMetadata.Version);
            Assert.IsTrue(globalFieldRef.FieldMetadata.AllowRichText);
            Assert.IsFalse(globalFieldRef.FieldMetadata.Multiline);
            Assert.AreEqual("advanced", globalFieldRef.FieldMetadata.RichTextType);
            Assert.IsNotNull(globalFieldRef.FieldMetadata.Options);
            Assert.AreEqual(3, globalFieldRef.FieldMetadata.Options.Count);

            Assert.IsTrue(json.Contains("\"field_metadata\""));
            Assert.IsTrue(json.Contains("\"description\":\"Complex metadata description\""));
            Assert.IsTrue(json.Contains("\"default_value\":\"default_value\""));
            Assert.IsTrue(json.Contains("\"version\":3"));
            Assert.IsTrue(json.Contains("\"allow_rich_text\":true"));
            Assert.IsTrue(json.Contains("\"multiline\":false"));
            Assert.IsTrue(json.Contains("\"rich_text_type\":\"advanced\""));
        }

        [TestMethod]
        public void Should_Validate_GlobalFieldReference_Data_Type_Is_Always_GlobalField()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Data Type Test",
                Uid = "data_type_test",
                DataType = "global_field",
                ReferenceTo = "referenced_field",
                Mandatory = false,
                Multiple = false,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.AreEqual("global_field", globalFieldRef.DataType);
            Assert.IsTrue(json.Contains("\"data_type\":\"global_field\""));
        }
    }
} 