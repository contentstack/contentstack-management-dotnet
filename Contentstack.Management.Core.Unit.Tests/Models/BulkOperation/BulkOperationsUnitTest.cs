using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models.BulkOperation
{
    [TestClass]
    public class BulkOperationsUnitTest
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
            var releaseData = new BulkReleaseItemsData();

            // Act
            var json = JsonConvert.SerializeObject(releaseData);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsNotNull(releaseData.Locale);
            Assert.IsNotNull(releaseData.Items);
            Assert.AreEqual(0, releaseData.Locale.Count);
            Assert.AreEqual(0, releaseData.Items.Count);
        }

        [TestMethod]
        public void Test006_BulkReleaseItemsData_Multiple_Items()
        {
            // Arrange
            var releaseData = new BulkReleaseItemsData
            {
                Release = "release_uid",
                Action = "publish",
                Locale = new List<string> { "en-us", "es-es" },
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
                    },
                    new BulkReleaseItem
                    {
                        ContentTypeUid = "ct_2",
                        Uid = "blt**************47",
                        Version = 1,
                        Locale = "es-es",
                        Title = "prueba de validación"
                    }
                }
            };

            // Act
            var json = JsonConvert.SerializeObject(releaseData);
            var deserialized = JsonConvert.DeserializeObject<BulkReleaseItemsData>(json);

            // Assert
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.Items.Count);
            Assert.AreEqual(2, deserialized.Locale.Count);
            Assert.AreEqual("ct_1", deserialized.Items[0].ContentTypeUid);
            Assert.AreEqual("ct_2", deserialized.Items[1].ContentTypeUid);
            Assert.AreEqual("en-us", deserialized.Locale[0]);
            Assert.AreEqual("es-es", deserialized.Locale[1]);
        }

        [TestMethod]
        public void Test007_BulkReleaseItemsData_Different_Actions()
        {
            // Arrange
            var publishData = new BulkReleaseItemsData
            {
                Release = "release_uid",
                Action = "publish",
                Locale = new List<string> { "en-us" },
                Reference = true,
                Items = new List<BulkReleaseItem>()
            };

            var unpublishData = new BulkReleaseItemsData
            {
                Release = "release_uid",
                Action = "unpublish",
                Locale = new List<string> { "en-us" },
                Reference = false,
                Items = new List<BulkReleaseItem>()
            };

            // Act
            var publishJson = JsonConvert.SerializeObject(publishData);
            var unpublishJson = JsonConvert.SerializeObject(unpublishData);

            // Assert
            Assert.IsTrue(publishJson.Contains("publish"));
            Assert.IsTrue(publishJson.Contains("true"));
            Assert.IsTrue(unpublishJson.Contains("unpublish"));
            Assert.IsTrue(unpublishJson.Contains("false"));
        }

        [TestMethod]
        public void Test008_BulkPublishDetails_Serialization()
        {
            // Arrange
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "blt0e0945888fb09dea",
                        ContentType = "ct0",
                        Version = 5,
                        Locale = "en-us"
                    }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "env1" },
                Rules = new BulkPublishRules
                {
                    Approvals = "true"
                },
                ScheduledAt = "2023-12-01T10:00:00Z",
                PublishWithReference = true
            };

            // Act
            var json = JsonConvert.SerializeObject(publishDetails);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("entries"));
            Assert.IsTrue(json.Contains("locales"));
            Assert.IsTrue(json.Contains("environments"));
            Assert.IsTrue(json.Contains("rules"));
            Assert.IsTrue(json.Contains("scheduled_at"));
            Assert.IsTrue(json.Contains("publish_with_reference"));
            Assert.IsTrue(json.Contains("approvals"));
        }

        [TestMethod]
        public void Test009_BulkPublishDetails_Deserialization()
        {
            // Arrange
            var json = @"{
                ""entries"": [
                    {
                        ""uid"": ""blt0e0945888fb09dea"",
                        ""content_type"": ""ct0"",
                        ""version"": 5,
                        ""locale"": ""en-us""
                    }
                ],
                ""locales"": [""en-us""],
                ""environments"": [""env1""],
                ""rules"": {
                    ""approvals"": ""true""
                },
                ""scheduled_at"": ""2023-12-01T10:00:00Z"",
                ""publish_with_reference"": true
            }";

            // Act
            var publishDetails = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            // Assert
            Assert.IsNotNull(publishDetails);
            Assert.AreEqual(1, publishDetails.Entries.Count);
            Assert.AreEqual(1, publishDetails.Locales.Count);
            Assert.AreEqual(1, publishDetails.Environments.Count);
            Assert.IsNotNull(publishDetails.Rules);
            Assert.AreEqual("true", publishDetails.Rules.Approvals);
            Assert.AreEqual("2023-12-01T10:00:00Z", publishDetails.ScheduledAt);
            Assert.IsTrue(publishDetails.PublishWithReference);

            var entry = publishDetails.Entries[0];
            Assert.AreEqual("blt0e0945888fb09dea", entry.Uid);
            Assert.AreEqual("ct0", entry.ContentType);
            Assert.AreEqual(5, entry.Version);
            Assert.AreEqual("en-us", entry.Locale);
        }

        [TestMethod]
        public void Test010_BulkPublishEntry_Serialization()
        {
            // Arrange
            var entry = new BulkPublishEntry
            {
                Uid = "blt0e0945888fb09dea",
                ContentType = "ct0",
                Version = 5,
                Locale = "en-us"
            };

            // Act
            var json = JsonConvert.SerializeObject(entry);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("uid"));
            Assert.IsTrue(json.Contains("content_type"));
            Assert.IsTrue(json.Contains("version"));
            Assert.IsTrue(json.Contains("locale"));
        }

        [TestMethod]
        public void Test011_BulkPublishRules_Serialization()
        {
            // Arrange
            var rules = new BulkPublishRules
            {
                Approvals = "true"
            };

            // Act
            var json = JsonConvert.SerializeObject(rules);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("approvals"));
            Assert.IsTrue(json.Contains("true"));
        }

        [TestMethod]
        public void Test012_BulkPublishRules_Deserialization()
        {
            // Arrange
            var json = @"{
                ""approvals"": ""false""
            }";

            // Act
            var rules = JsonConvert.DeserializeObject<BulkPublishRules>(json);

            // Assert
            Assert.IsNotNull(rules);
            Assert.AreEqual("false", rules.Approvals);
        }

        [TestMethod]
        public void Test013_BulkPublishDetails_Multiple_Entries()
        {
            // Arrange
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "blt0e0945888fb09dea",
                        ContentType = "ct0",
                        Version = 5,
                        Locale = "en-us"
                    },
                    new BulkPublishEntry
                    {
                        Uid = "bltabb69092b8d45ff7",
                        ContentType = "ct0",
                        Version = 1,
                        Locale = "en-us"
                    },
                    new BulkPublishEntry
                    {
                        Uid = "blt5eb4637f09f0ac3e",
                        ContentType = "ct5",
                        Version = 2,
                        Locale = "en-us"
                    }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "env1" },
                PublishWithReference = true
            };

            // Act
            var json = JsonConvert.SerializeObject(publishDetails);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            // Assert
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(3, deserialized.Entries.Count);
            Assert.AreEqual("blt0e0945888fb09dea", deserialized.Entries[0].Uid);
            Assert.AreEqual("bltabb69092b8d45ff7", deserialized.Entries[1].Uid);
            Assert.AreEqual("blt5eb4637f09f0ac3e", deserialized.Entries[2].Uid);
            Assert.AreEqual("ct0", deserialized.Entries[0].ContentType);
            Assert.AreEqual("ct0", deserialized.Entries[1].ContentType);
            Assert.AreEqual("ct5", deserialized.Entries[2].ContentType);
        }

        [TestMethod]
        public void Test014_BulkPublishDetails_With_Assets()
        {
            // Arrange
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "blt0e0945888fb09dea",
                        ContentType = "ct0",
                        Version = 5,
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkPublishAsset>
                {
                    new BulkPublishAsset { Uid = "blt6f299e4805f0b1dd" }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "env1" }
            };

            // Act
            var json = JsonConvert.SerializeObject(publishDetails);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            // Assert
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(1, deserialized.Entries.Count);
            Assert.AreEqual(1, deserialized.Assets.Count);
            Assert.AreEqual("blt6f299e4805f0b1dd", deserialized.Assets[0].Uid);
        }

        [TestMethod]
        public void Test015_BulkPublishAsset_Serialization()
        {
            // Arrange
            var asset = new BulkPublishAsset
            {
                Uid = "blt6f299e4805f0b1dd"
            };

            // Act
            var json = JsonConvert.SerializeObject(asset);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("uid"));
            Assert.IsTrue(json.Contains("blt6f299e4805f0b1dd"));
        }

        [TestMethod]
        public void Test016_BulkPublishDetails_Empty_Collections()
        {
            // Arrange
            var publishDetails = new BulkPublishDetails();

            // Act
            var json = JsonConvert.SerializeObject(publishDetails);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            // Assert
            Assert.IsNotNull(deserialized);
            Assert.IsNotNull(deserialized.Entries);
            Assert.IsNotNull(deserialized.Assets);
            Assert.IsNotNull(deserialized.Locales);
            Assert.IsNotNull(deserialized.Environments);
            Assert.AreEqual(0, deserialized.Entries.Count);
            Assert.AreEqual(0, deserialized.Assets.Count);
            Assert.AreEqual(0, deserialized.Locales.Count);
            Assert.AreEqual(0, deserialized.Environments.Count);
        }

        [TestMethod]
        public void Test017_BulkReleaseItemsData_Null_Values()
        {
            // Arrange
            var releaseData = new BulkReleaseItemsData
            {
                Release = null,
                Action = null,
                Items = new List<BulkReleaseItem>
                {
                    new BulkReleaseItem
                    {
                        ContentTypeUid = null,
                        Uid = null,
                        Locale = null,
                        Title = null
                    }
                }
            };

            // Act
            var json = JsonConvert.SerializeObject(releaseData);

            // Assert
            Assert.IsNotNull(json);
            // Should handle null values gracefully
        }

        [TestMethod]
        public void Test018_BulkPublishDetails_Null_Values()
        {
            // Arrange
            var publishDetails = new BulkPublishDetails
            {
                Rules = null,
                ScheduledAt = null,
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = null,
                        ContentType = null,
                        Locale = null
                    }
                }
            };

            // Act
            var json = JsonConvert.SerializeObject(publishDetails);

            // Assert
            Assert.IsNotNull(json);
            // Should handle null values gracefully
        }

        [TestMethod]
        public void Test019_BulkReleaseItemsData_Complex_Scenario()
        {
            // Arrange - Complex scenario with multiple items and different locales
            var releaseData = new BulkReleaseItemsData
            {
                Release = "release_uid",
                Action = "publish",
                Locale = new List<string> { "en-us", "es-es", "fr-fr" },
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
                    },
                    new BulkReleaseItem
                    {
                        ContentTypeUid = "ct_2",
                        Uid = "blt**************47",
                        Version = 1,
                        Locale = "es-es",
                        Title = "prueba de validación"
                    },
                    new BulkReleaseItem
                    {
                        ContentTypeUid = "ct_3",
                        Uid = "blt**************48",
                        Version = 3,
                        Locale = "fr-fr",
                        Title = "test de validation"
                    }
                }
            };

            // Act
            var json = JsonConvert.SerializeObject(releaseData);
            var deserialized = JsonConvert.DeserializeObject<BulkReleaseItemsData>(json);

            // Assert
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(3, deserialized.Locale.Count);
            Assert.AreEqual(3, deserialized.Items.Count);
            Assert.AreEqual("en-us", deserialized.Locale[0]);
            Assert.AreEqual("es-es", deserialized.Locale[1]);
            Assert.AreEqual("fr-fr", deserialized.Locale[2]);
            Assert.AreEqual("ct_1", deserialized.Items[0].ContentTypeUid);
            Assert.AreEqual("ct_2", deserialized.Items[1].ContentTypeUid);
            Assert.AreEqual("ct_3", deserialized.Items[2].ContentTypeUid);
        }

        [TestMethod]
        public void Test020_BulkPublishDetails_Complex_Scenario()
        {
            // Arrange - Complex scenario with multiple entries, assets, and rules
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "blt0e0945888fb09dea",
                        ContentType = "ct0",
                        Version = 5,
                        Locale = "en-us"
                    },
                    new BulkPublishEntry
                    {
                        Uid = "bltabb69092b8d45ff7",
                        ContentType = "ct0",
                        Version = 1,
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkPublishAsset>
                {
                    new BulkPublishAsset { Uid = "blt6f299e4805f0b1dd" },
                    new BulkPublishAsset { Uid = "blt7f399f5916f1c2ee" }
                },
                Locales = new List<string> { "en-us", "es-es" },
                Environments = new List<string> { "env1", "env2" },
                Rules = new BulkPublishRules
                {
                    Approvals = "true"
                },
                ScheduledAt = "2023-12-01T10:00:00Z",
                PublishWithReference = true
            };

            // Act
            var json = JsonConvert.SerializeObject(publishDetails);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            // Assert
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.Entries.Count);
            Assert.AreEqual(2, deserialized.Assets.Count);
            Assert.AreEqual(2, deserialized.Locales.Count);
            Assert.AreEqual(2, deserialized.Environments.Count);
            Assert.IsNotNull(deserialized.Rules);
            Assert.AreEqual("true", deserialized.Rules.Approvals);
            Assert.AreEqual("2023-12-01T10:00:00Z", deserialized.ScheduledAt);
            Assert.IsTrue(deserialized.PublishWithReference);
        }
    }
} 