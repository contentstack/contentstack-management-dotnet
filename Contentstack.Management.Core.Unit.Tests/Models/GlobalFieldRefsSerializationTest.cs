using System;
using System.Collections.Generic;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class GlobalFieldRefsSerializationTest
    {
        private readonly IFixture _fixture = new Fixture();

        [TestMethod]
        public void Should_Serialize_GlobalFieldRefs_To_JSON()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "referenced_global_field_uid",
                OccurrenceCount = 2,
                IsChild = true,
                Paths = new List<string> { "schema.1", "schema.3.schema.1" }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"uid\":\"referenced_global_field_uid\""));
            Assert.IsTrue(json.Contains("\"occurrence_count\":2"));
            Assert.IsTrue(json.Contains("\"isChild\":true"));
            Assert.IsTrue(json.Contains("\"paths\""));
            Assert.IsTrue(json.Contains("\"schema.1\""));
            Assert.IsTrue(json.Contains("\"schema.3.schema.1\""));
        }

        [TestMethod]
        public void Should_Deserialize_JSON_To_GlobalFieldRefs()
        {
            
            var json = @"{
                ""uid"": ""referenced_global_field_uid"",
                ""occurrence_count"": 2,
                ""isChild"": true,
                ""paths"": [""schema.1"", ""schema.3.schema.1""]
            }";

            
            var globalFieldRefs = JsonConvert.DeserializeObject<GlobalFieldRefs>(json);

            Assert.IsNotNull(globalFieldRefs);
            Assert.AreEqual("referenced_global_field_uid", globalFieldRefs.Uid);
            Assert.AreEqual(2, globalFieldRefs.OccurrenceCount);
            Assert.IsTrue(globalFieldRefs.IsChild);
            Assert.IsNotNull(globalFieldRefs.Paths);
            Assert.AreEqual(2, globalFieldRefs.Paths.Count);
            Assert.AreEqual("schema.1", globalFieldRefs.Paths[0]);
            Assert.AreEqual("schema.3.schema.1", globalFieldRefs.Paths[1]);
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldRefs_With_Zero_OccurrenceCount()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "test_field",
                OccurrenceCount = 0,
                IsChild = false,
                Paths = new List<string>()
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"occurrence_count\":0"));
            Assert.IsTrue(json.Contains("\"isChild\":false"));
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldRefs_With_Large_OccurrenceCount()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "test_field",
                OccurrenceCount = 100,
                IsChild = true,
                Paths = new List<string> { "schema.0", "schema.1", "schema.2" }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"occurrence_count\":100"));
            Assert.IsTrue(json.Contains("\"isChild\":true"));
            Assert.IsTrue(json.Contains("\"schema.0\""));
            Assert.IsTrue(json.Contains("\"schema.1\""));
            Assert.IsTrue(json.Contains("\"schema.2\""));
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldRefs_With_Complex_Paths()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "complex_field",
                OccurrenceCount = 3,
                IsChild = true,
                Paths = new List<string> 
                { 
                    "schema.1", 
                    "schema.3.schema.4", 
                    "schema.4.blocks.0.schema.2",
                    "schema.5.schema.1.schema.0"
                }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"occurrence_count\":3"));
            Assert.IsTrue(json.Contains("\"schema.1\""));
            Assert.IsTrue(json.Contains("\"schema.3.schema.4\""));
            Assert.IsTrue(json.Contains("\"schema.4.blocks.0.schema.2\""));
            Assert.IsTrue(json.Contains("\"schema.5.schema.1.schema.0\""));
        }

        [TestMethod]
        public void Should_Deserialize_GlobalFieldRefs_With_Complex_Paths()
        {
            
            var json = @"{
                ""uid"": ""complex_field"",
                ""occurrence_count"": 3,
                ""isChild"": true,
                ""paths"": [
                    ""schema.1"",
                    ""schema.3.schema.4"",
                    ""schema.4.blocks.0.schema.2"",
                    ""schema.5.schema.1.schema.0""
                ]
            }";

            
            var globalFieldRefs = JsonConvert.DeserializeObject<GlobalFieldRefs>(json);

            Assert.IsNotNull(globalFieldRefs);
            Assert.AreEqual("complex_field", globalFieldRefs.Uid);
            Assert.AreEqual(3, globalFieldRefs.OccurrenceCount);
            Assert.IsTrue(globalFieldRefs.IsChild);
            Assert.IsNotNull(globalFieldRefs.Paths);
            Assert.AreEqual(4, globalFieldRefs.Paths.Count);
            Assert.AreEqual("schema.1", globalFieldRefs.Paths[0]);
            Assert.AreEqual("schema.3.schema.4", globalFieldRefs.Paths[1]);
            Assert.AreEqual("schema.4.blocks.0.schema.2", globalFieldRefs.Paths[2]);
            Assert.AreEqual("schema.5.schema.1.schema.0", globalFieldRefs.Paths[3]);
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldRefs_With_Null_Paths()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "test_field",
                OccurrenceCount = 1,
                IsChild = false,
                Paths = null
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"paths\":null"));
        }

        [TestMethod]
        public void Should_Handle_GlobalFieldRefs_With_Empty_Paths()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "test_field",
                OccurrenceCount = 1,
                IsChild = false,
                Paths = new List<string>()
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"paths\":[]"));
        }

        [TestMethod]
        public void Should_Deserialize_GlobalFieldRefs_With_Null_Paths()
        {
            
            var json = @"{
                ""uid"": ""test_field"",
                ""occurrence_count"": 1,
                ""isChild"": false,
                ""paths"": null
            }";

            
            var globalFieldRefs = JsonConvert.DeserializeObject<GlobalFieldRefs>(json);

            Assert.IsNotNull(globalFieldRefs);
            Assert.AreEqual("test_field", globalFieldRefs.Uid);
            Assert.AreEqual(1, globalFieldRefs.OccurrenceCount);
            Assert.IsFalse(globalFieldRefs.IsChild);
            Assert.IsNull(globalFieldRefs.Paths);
        }

        [TestMethod]
        public void Should_Deserialize_GlobalFieldRefs_With_Empty_Paths()
        {
            
            var json = @"{
                ""uid"": ""test_field"",
                ""occurrence_count"": 1,
                ""isChild"": false,
                ""paths"": []
            }";

            
            var globalFieldRefs = JsonConvert.DeserializeObject<GlobalFieldRefs>(json);

            Assert.IsNotNull(globalFieldRefs);
            Assert.AreEqual("test_field", globalFieldRefs.Uid);
            Assert.AreEqual(1, globalFieldRefs.OccurrenceCount);
            Assert.IsFalse(globalFieldRefs.IsChild);
            Assert.IsNotNull(globalFieldRefs.Paths);
            Assert.AreEqual(0, globalFieldRefs.Paths.Count);
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldRefs_With_Special_Characters_In_Uid()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "test-field_with_underscores.and.dots",
                OccurrenceCount = 1,
                IsChild = true,
                Paths = new List<string> { "schema.1" }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"uid\":\"test-field_with_underscores.and.dots\""));
        }

        [TestMethod]
        public void Should_Deserialize_GlobalFieldRefs_With_Special_Characters_In_Uid()
        {
            
            var json = @"{
                ""uid"": ""test-field_with_underscores.and.dots"",
                ""occurrence_count"": 1,
                ""isChild"": true,
                ""paths"": [""schema.1""]
            }";

            
            var globalFieldRefs = JsonConvert.DeserializeObject<GlobalFieldRefs>(json);

            Assert.IsNotNull(globalFieldRefs);
            Assert.AreEqual("test-field_with_underscores.and.dots", globalFieldRefs.Uid);
            Assert.AreEqual(1, globalFieldRefs.OccurrenceCount);
            Assert.IsTrue(globalFieldRefs.IsChild);
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldRefs_With_Unicode_Characters()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "test_field_中文",
                OccurrenceCount = 1,
                IsChild = false,
                Paths = new List<string> { "schema.1" }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"uid\":\"test_field_中文\""));
        }

        [TestMethod]
        public void Should_Deserialize_GlobalFieldRefs_With_Unicode_Characters()
        {
            
            var json = @"{
                ""uid"": ""test_field_中文"",
                ""occurrence_count"": 1,
                ""isChild"": false,
                ""paths"": [""schema.1""]
            }";

            
            var globalFieldRefs = JsonConvert.DeserializeObject<GlobalFieldRefs>(json);

            Assert.IsNotNull(globalFieldRefs);
            Assert.AreEqual("test_field_中文", globalFieldRefs.Uid);
            Assert.AreEqual(1, globalFieldRefs.OccurrenceCount);
            Assert.IsFalse(globalFieldRefs.IsChild);
        }
    }
} 