using System.Collections.Generic;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class GlobalFieldRefsTest
    {
        private readonly IFixture _fixture = new Fixture();

        [TestMethod]
        public void Should_Create_GlobalFieldRefs_With_All_Properties()
        {
            
            var uid = _fixture.Create<string>();
            var occurrenceCount = _fixture.Create<int>();
            var isChild = _fixture.Create<bool>();
            var paths = new List<string> { "schema.1", "schema.3.schema.4" };

            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = uid,
                OccurrenceCount = occurrenceCount,
                IsChild = isChild,
                Paths = paths
            };

            Assert.AreEqual(uid, globalFieldRefs.Uid);
            Assert.AreEqual(occurrenceCount, globalFieldRefs.OccurrenceCount);
            Assert.AreEqual(isChild, globalFieldRefs.IsChild);
            Assert.AreEqual(paths, globalFieldRefs.Paths);
            Assert.AreEqual(2, globalFieldRefs.Paths.Count);
        }

        [TestMethod]
        public void Should_Serialize_GlobalFieldRefs_Correctly()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "referenced_global_field",
                OccurrenceCount = 3,
                IsChild = true,
                Paths = new List<string> { "schema.1", "schema.3.schema.4", "schema.4.blocks.0.schema.2" }
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"uid\":\"referenced_global_field\""));
            Assert.IsTrue(json.Contains("\"occurrence_count\":3"));
            Assert.IsTrue(json.Contains("\"isChild\":true"));
            Assert.IsTrue(json.Contains("\"paths\""));
            Assert.IsTrue(json.Contains("schema.1"));
            Assert.IsTrue(json.Contains("schema.3.schema.4"));
            Assert.IsTrue(json.Contains("schema.4.blocks.0.schema.2"));
        }

        [TestMethod]
        public void Should_Deserialize_GlobalFieldRefs_Correctly()
        {
            
            var json = @"{
                ""uid"": ""referenced_global_field"",
                ""occurrence_count"": 2,
                ""isChild"": false,
                ""paths"": [""schema.1"", ""schema.2""]
            }";

            
            var globalFieldRefs = JsonConvert.DeserializeObject<GlobalFieldRefs>(json);

            Assert.AreEqual("referenced_global_field", globalFieldRefs.Uid);
            Assert.AreEqual(2, globalFieldRefs.OccurrenceCount);
            Assert.IsFalse(globalFieldRefs.IsChild);
            Assert.IsNotNull(globalFieldRefs.Paths);
            Assert.AreEqual(2, globalFieldRefs.Paths.Count);
            Assert.AreEqual("schema.1", globalFieldRefs.Paths[0]);
            Assert.AreEqual("schema.2", globalFieldRefs.Paths[1]);
        }

        [TestMethod]
        public void Should_Handle_Null_Paths()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "test_uid",
                OccurrenceCount = 1,
                IsChild = false,
                Paths = null
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"uid\":\"test_uid\""));
            Assert.IsTrue(json.Contains("\"occurrence_count\":1"));
            Assert.IsTrue(json.Contains("\"isChild\":false"));
        }

        [TestMethod]
        public void Should_Handle_Empty_Paths()
        {
            
            var globalFieldRefs = new GlobalFieldRefs
            {
                Uid = "test_uid",
                OccurrenceCount = 0,
                IsChild = true,
                Paths = new List<string>()
            };

            
            var json = JsonConvert.SerializeObject(globalFieldRefs);

            Assert.IsTrue(json.Contains("\"uid\":\"test_uid\""));
            Assert.IsTrue(json.Contains("\"occurrence_count\":0"));
            Assert.IsTrue(json.Contains("\"isChild\":true"));
            Assert.IsTrue(json.Contains("\"paths\":[]"));
        }
    }
} 