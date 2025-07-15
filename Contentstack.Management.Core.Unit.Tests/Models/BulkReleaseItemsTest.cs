using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class BulkReleaseItemsTest
    {
        [TestMethod]
        public void Test001_BulkReleaseItemsData_Serialization()
        {
            // Arrange
            var releaseData = new BulkReleaseItemsData
            {
                Release = "release_uid",
                Action = "publish",
                Locale = new List<string> { "en-us" },
                Reference = true,
                Items = new List<BulkReleaseItem>
                {
                    new BulkReleaseItem
                    {
                        ContentTypeUid = "ct_1",
                        Uid = "uid",
                        Version = 2,
                        Locale = "en-us",
                        Title = "validation test"
                    }
                }
            };

            // Act
            var json = JsonConvert.SerializeObject(releaseData);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("release"));
            Assert.IsTrue(json.Contains("action"));
            Assert.IsTrue(json.Contains("locale"));
            Assert.IsTrue(json.Contains("reference"));
            Assert.IsTrue(json.Contains("items"));
            Assert.IsTrue(json.Contains("content_type_uid"));
            Assert.IsTrue(json.Contains("uid"));
            Assert.IsTrue(json.Contains("version"));
            Assert.IsTrue(json.Contains("title"));
        }

        [TestMethod]
        public void Test002_BulkReleaseItem_Serialization()
        {
            // Arrange
            var item = new BulkReleaseItem
            {
                ContentTypeUid = "ct_1",
                Uid = "uid",
                Version = 2,
                Locale = "en-us",
                Title = "validation test"
            };

            // Act
            var json = JsonConvert.SerializeObject(item);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("content_type_uid"));
            Assert.IsTrue(json.Contains("uid"));
            Assert.IsTrue(json.Contains("version"));
            Assert.IsTrue(json.Contains("locale"));
            Assert.IsTrue(json.Contains("title"));
        }

        [TestMethod]
        public void Test003_BulkReleaseItemsData_Deserialization()
        {
            // Arrange
            var json = @"{
                ""release"": ""release_uid"",
                ""action"": ""publish"",
                ""locale"": [""en-us""],
                ""reference"": true,
                ""items"": [
                    {
                        ""content_type_uid"": ""ct_1"",
                        ""uid"": ""uid"",
                        ""version"": 2,
                        ""locale"": ""en-us"",
                        ""title"": ""validation test""
                    }
                ]
            }";

            // Act
            var releaseData = JsonConvert.DeserializeObject<BulkReleaseItemsData>(json);

            // Assert
            Assert.IsNotNull(releaseData);
            Assert.AreEqual("release_uid", releaseData.Release);
            Assert.AreEqual("publish", releaseData.Action);
            Assert.AreEqual(1, releaseData.Locale.Count);
            Assert.AreEqual("en-us", releaseData.Locale[0]);
            Assert.IsTrue(releaseData.Reference);
            Assert.AreEqual(1, releaseData.Items.Count);
            Assert.AreEqual("ct_1", releaseData.Items[0].ContentTypeUid);
            Assert.AreEqual("uid", releaseData.Items[0].Uid);
            Assert.AreEqual(2, releaseData.Items[0].Version);
            Assert.AreEqual("en-us", releaseData.Items[0].Locale);
            Assert.AreEqual("validation test", releaseData.Items[0].Title);
        }

        [TestMethod]
        public void Test004_BulkReleaseItem_Deserialization()
        {
            // Arrange
            var json = @"{
                ""content_type_uid"": ""ct_1"",
                ""uid"": ""uid"",
                ""version"": 2,
                ""locale"": ""en-us"",
                ""title"": ""validation test""
            }";

            // Act
            var item = JsonConvert.DeserializeObject<BulkReleaseItem>(json);

            // Assert
            Assert.IsNotNull(item);
            Assert.AreEqual("ct_1", item.ContentTypeUid);
            Assert.AreEqual("uid", item.Uid);
            Assert.AreEqual(2, item.Version);
            Assert.AreEqual("en-us", item.Locale);
            Assert.AreEqual("validation test", item.Title);
        }

        [TestMethod]
        public void Test005_BulkReleaseItemsData_Empty_Collections()
        {
            // Arrange
            var releaseData = new BulkReleaseItemsData
            {
                Locale = new List<string>(),
                Items = new List<BulkReleaseItem>()
            };

            // Act
            var json = JsonConvert.SerializeObject(releaseData);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsNotNull(releaseData.Locale);
            Assert.IsNotNull(releaseData.Items);
            Assert.AreEqual(0, releaseData.Locale.Count);
            Assert.AreEqual(0, releaseData.Items.Count);
        }
    }
} 