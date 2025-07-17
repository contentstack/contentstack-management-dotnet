using System;
using AutoFixture;
using Contentstack.Management.Core.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models.Fields
{
    [TestClass]
    public class GlobalFieldReferenceTest
    {
        private readonly IFixture _fixture = new Fixture();

        [TestMethod]
        public void Should_Create_GlobalFieldReference_With_All_Properties()
        {
            
            var displayName = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            var referenceTo = _fixture.Create<string>();
            var fieldMetadata = new FieldMetadata
            {
                Description = "Test description"
            };

            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = displayName,
                Uid = uid,
                DataType = "global_field",
                ReferenceTo = referenceTo,
                FieldMetadata = fieldMetadata,
                Multiple = true,
                Mandatory = false,
                Unique = false,
                NonLocalizable = true
            };

            Assert.AreEqual(displayName, globalFieldRef.DisplayName);
            Assert.AreEqual(uid, globalFieldRef.Uid);
            Assert.AreEqual("global_field", globalFieldRef.DataType);
            Assert.AreEqual(referenceTo, globalFieldRef.ReferenceTo);
            Assert.AreEqual(fieldMetadata, globalFieldRef.FieldMetadata);
            Assert.IsTrue(globalFieldRef.Multiple);
            Assert.IsFalse(globalFieldRef.Mandatory);
            Assert.IsFalse(globalFieldRef.Unique);
            Assert.IsTrue(globalFieldRef.NonLocalizable);
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldReference_Correctly()
        {
            
            var globalFieldRef = new GlobalFieldReference
            {
                DisplayName = "Test Global Field Reference",
                Uid = "test_global_field_ref",
                DataType = "global_field",
                ReferenceTo = "referenced_global_field",
                FieldMetadata = new FieldMetadata
                {
                    Description = "Test description"
                },
                Multiple = false,
                Mandatory = true,
                Unique = false,
                NonLocalizable = false
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRef);

            Assert.IsTrue(json.Contains("\"display_name\":\"Test Global Field Reference\""));
            Assert.IsTrue(json.Contains("\"uid\":\"test_global_field_ref\""));
            Assert.IsTrue(json.Contains("\"data_type\":\"global_field\""));
            Assert.IsTrue(json.Contains("\"reference_to\":\"referenced_global_field\""));
            Assert.IsTrue(json.Contains("\"mandatory\":true"));
            Assert.IsTrue(json.Contains("\"multiple\":false"));
            Assert.IsTrue(json.Contains("\"unique\":false"));
            Assert.IsTrue(json.Contains("\"non_localizable\":false"));
        }

        [TestMethod]
        public void Should_Deserialize_GlobalFieldReference_Correctly()
        {
            
            var json = @"{
                ""display_name"": ""Test Global Field Reference"",
                ""uid"": ""test_global_field_ref"",
                ""data_type"": ""global_field"",
                ""reference_to"": ""referenced_global_field"",
                ""field_metadata"": {
                    ""description"": ""Test description""
                },
                ""mandatory"": true,
                ""multiple"": false,
                ""unique"": false,
                ""non_localizable"": false
            }";

            
            var globalFieldRef = JsonConvert.DeserializeObject<GlobalFieldReference>(json);

            Assert.AreEqual("Test Global Field Reference", globalFieldRef.DisplayName);
            Assert.AreEqual("test_global_field_ref", globalFieldRef.Uid);
            Assert.AreEqual("global_field", globalFieldRef.DataType);
            Assert.AreEqual("referenced_global_field", globalFieldRef.ReferenceTo);
            Assert.IsNotNull(globalFieldRef.FieldMetadata);
            Assert.AreEqual("Test description", globalFieldRef.FieldMetadata.Description);
            Assert.IsTrue(globalFieldRef.Mandatory);
            Assert.IsFalse(globalFieldRef.Multiple);
            Assert.IsFalse(globalFieldRef.Unique);
            Assert.IsFalse(globalFieldRef.NonLocalizable);
        }

        [TestMethod]
        public void Should_Inherit_From_Field_Base_Class()
        {
            var globalFieldRef = new GlobalFieldReference();

            Assert.IsInstanceOfType(globalFieldRef, typeof(Field));
        }
    }
} 