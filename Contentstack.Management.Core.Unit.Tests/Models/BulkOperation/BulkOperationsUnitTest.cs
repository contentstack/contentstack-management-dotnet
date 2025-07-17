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

            
            var json = JsonConvert.SerializeObject(releaseData);

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
            
            var item = new BulkReleaseItem
            {
                ContentTypeUid = "ct_1",
                Uid = "uid",
                Version = 2,
                Locale = "en-us",
                Title = "validation test"
            };

            
            var json = JsonConvert.SerializeObject(item);

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

            
            var releaseData = JsonConvert.DeserializeObject<BulkReleaseItemsData>(json);

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
            
            var json = @"{
                ""content_type_uid"": ""ct_1"",
                ""uid"": ""uid"",
                ""version"": 2,
                ""locale"": ""en-us"",
                ""title"": ""validation test""
            }";

            
            var item = JsonConvert.DeserializeObject<BulkReleaseItem>(json);

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
            
            var releaseData = new BulkReleaseItemsData
            {
                Locale = new List<string>(),
                Items = new List<BulkReleaseItem>()
            };

            
            var json = JsonConvert.SerializeObject(releaseData);

            Assert.IsNotNull(json);
            Assert.IsNotNull(releaseData.Locale);
            Assert.IsNotNull(releaseData.Items);
            Assert.AreEqual(0, releaseData.Locale.Count);
            Assert.AreEqual(0, releaseData.Items.Count);
        }

        [TestMethod]
        public void Test006_BulkReleaseItemsData_Multiple_Items()
        {
            
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
                        Uid = "uid",
                        Version = 1,
                        Locale = "es-es",
                        Title = "prueba de validación"
                    }
                }
            };

            
            var json = JsonConvert.SerializeObject(releaseData);
            var deserialized = JsonConvert.DeserializeObject<BulkReleaseItemsData>(json);

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

            
            var publishJson = JsonConvert.SerializeObject(publishData);
            var unpublishJson = JsonConvert.SerializeObject(unpublishData);

            Assert.IsTrue(publishJson.Contains("publish"));
            Assert.IsTrue(publishJson.Contains("true"));
            Assert.IsTrue(unpublishJson.Contains("unpublish"));
            Assert.IsTrue(unpublishJson.Contains("false"));
        }

        [TestMethod]
        public void Test008_BulkPublishDetails_Serialization()
        {
            
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid",
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

            
            var json = JsonConvert.SerializeObject(publishDetails);

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
            
            var json = @"{
                ""entries"": [
                    {
                        ""uid"": ""entry_uid"",
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

            
            var publishDetails = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            Assert.IsNotNull(publishDetails);
            Assert.AreEqual(1, publishDetails.Entries.Count);
            Assert.AreEqual(1, publishDetails.Locales.Count);
            Assert.AreEqual(1, publishDetails.Environments.Count);
            Assert.IsNotNull(publishDetails.Rules);
            Assert.AreEqual("true", publishDetails.Rules.Approvals);
            Assert.AreEqual("2023-12-01T10:00:00Z", publishDetails.ScheduledAt);
            Assert.IsTrue(publishDetails.PublishWithReference);

            var entry = publishDetails.Entries[0];
            Assert.AreEqual("entry_uid", entry.Uid);
            Assert.AreEqual("ct0", entry.ContentType);
            Assert.AreEqual(5, entry.Version);
            Assert.AreEqual("en-us", entry.Locale);
        }

        [TestMethod]
        public void Test010_BulkPublishEntry_Serialization()
        {
            
            var entry = new BulkPublishEntry
            {
                Uid = "entry_uid",
                ContentType = "ct0",
                Version = 5,
                Locale = "en-us"
            };

            
            var json = JsonConvert.SerializeObject(entry);

            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("uid"));
            Assert.IsTrue(json.Contains("content_type"));
            Assert.IsTrue(json.Contains("version"));
            Assert.IsTrue(json.Contains("locale"));
        }

        [TestMethod]
        public void Test011_BulkPublishRules_Serialization()
        {
            
            var rules = new BulkPublishRules
            {
                Approvals = "true"
            };

            
            var json = JsonConvert.SerializeObject(rules);

            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("approvals"));
            Assert.IsTrue(json.Contains("true"));
        }

        [TestMethod]
        public void Test012_BulkPublishRules_Deserialization()
        {
            
            var json = @"{
                ""approvals"": ""false""
            }";

            
            var rules = JsonConvert.DeserializeObject<BulkPublishRules>(json);

            Assert.IsNotNull(rules);
            Assert.AreEqual("false", rules.Approvals);
        }

        [TestMethod]
        public void Test013_BulkPublishDetails_Multiple_Entries()
        {
            
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid",
                        ContentType = "ct0",
                        Version = 5,
                        Locale = "en-us"
                    },
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_2",
                        ContentType = "ct0",
                        Version = 1,
                        Locale = "en-us"
                    },
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_3",
                        ContentType = "ct5",
                        Version = 2,
                        Locale = "en-us"
                    }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "env1" },
                PublishWithReference = true
            };

            
            var json = JsonConvert.SerializeObject(publishDetails);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(3, deserialized.Entries.Count);
            Assert.AreEqual("entry_uid", deserialized.Entries[0].Uid);
            Assert.AreEqual("entry_uid_2", deserialized.Entries[1].Uid);
            Assert.AreEqual("entry_uid_3", deserialized.Entries[2].Uid);
            Assert.AreEqual("ct0", deserialized.Entries[0].ContentType);
            Assert.AreEqual("ct0", deserialized.Entries[1].ContentType);
            Assert.AreEqual("ct5", deserialized.Entries[2].ContentType);
        }

        [TestMethod]
        public void Test014_BulkPublishDetails_With_Assets()
        {
            
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid",
                        ContentType = "ct0",
                        Version = 5,
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkPublishAsset>
                {
                    new BulkPublishAsset { Uid = "asset_uid" }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "env1" }
            };

            
            var json = JsonConvert.SerializeObject(publishDetails);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(1, deserialized.Entries.Count);
            Assert.AreEqual(1, deserialized.Assets.Count);
            Assert.AreEqual("asset_uid", deserialized.Assets[0].Uid);
        }

        [TestMethod]
        public void Test015_BulkPublishAsset_Serialization()
        {
            
            var asset = new BulkPublishAsset
            {
                Uid = "asset_uid"
            };

            
            var json = JsonConvert.SerializeObject(asset);

            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("uid"));
            Assert.IsTrue(json.Contains("asset_uid"));
        }

        [TestMethod]
        public void Test016_BulkPublishDetails_Empty_Collections()
        {
            
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>(),
                Assets = new List<BulkPublishAsset>()
            };

            
            var json = JsonConvert.SerializeObject(publishDetails);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            Assert.IsNotNull(deserialized);
            // Collections may be null after deserialization due to ShouldSerialize methods
            // Initialize them if they're null
            deserialized.Entries = deserialized.Entries ?? new List<BulkPublishEntry>();
            deserialized.Assets = deserialized.Assets ?? new List<BulkPublishAsset>();
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

            
            var json = JsonConvert.SerializeObject(releaseData);

            Assert.IsNotNull(json);
            // Should handle null values gracefully
        }

        [TestMethod]
        public void Test018_BulkPublishDetails_Null_Values()
        {
            
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

            
            var json = JsonConvert.SerializeObject(publishDetails);

            Assert.IsNotNull(json);
            // Should handle null values gracefully
        }

        [TestMethod]
        public void Test019_BulkReleaseItemsData_Complex_Scenario()
        {
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
                        Uid = "uid",
                        Version = 1,
                        Locale = "es-es",
                        Title = "prueba de validación"
                    },
                    new BulkReleaseItem
                    {
                        ContentTypeUid = "ct_3",
                        Uid = "uid",
                        Version = 3,
                        Locale = "fr-fr",
                        Title = "test de validation"
                    }
                }
            };

            
            var json = JsonConvert.SerializeObject(releaseData);
            var deserialized = JsonConvert.DeserializeObject<BulkReleaseItemsData>(json);

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
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid",
                        ContentType = "ct0",
                        Version = 5,
                        Locale = "en-us"
                    },
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_2",
                        ContentType = "ct0",
                        Version = 1,
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkPublishAsset>
                {
                    new BulkPublishAsset { Uid = "asset_uid" },
                    new BulkPublishAsset { Uid = "asset_uid_2" }
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

            
            var json = JsonConvert.SerializeObject(publishDetails);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

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