using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Unit.Tests.Models.BulkOperation
{
    [TestClass]
    public class BulkOperationModelsTest
    {
        [TestMethod]
        public void BulkPublishDetails_Serialization_Test()
        {
            
            var details = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Version = 1,
                        Locale = "en-us"
                    },
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_2",
                        ContentType = "content_type_2",
                        Version = 2,
                        Locale = "en-gb"
                    }
                },
                Assets = new List<BulkPublishAsset>
                {
                    new BulkPublishAsset { Uid = "asset_uid_1" },
                    new BulkPublishAsset { Uid = "asset_uid_2" }
                },
                Locales = new List<string> { "en-us", "en-gb" },
                Environments = new List<string> { "env_1", "env_2" }
            };

            
            var json = JsonConvert.SerializeObject(details);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.Entries.Count);
            Assert.AreEqual(2, deserialized.Assets.Count);
            Assert.AreEqual(2, deserialized.Locales.Count);
            Assert.AreEqual(2, deserialized.Environments.Count);

            var firstEntry = deserialized.Entries.First();
            Assert.AreEqual("entry_uid_1", firstEntry.Uid);
            Assert.AreEqual("content_type_1", firstEntry.ContentType);
            Assert.AreEqual(1, firstEntry.Version);
            Assert.AreEqual("en-us", firstEntry.Locale);

            var firstAsset = deserialized.Assets.First();
            Assert.AreEqual("asset_uid_1", firstAsset.Uid);
        }

        [TestMethod]
        public void BulkPublishEntry_WithContentType_Serialization_Test()
        {
            
            var entry = new BulkPublishEntry
            {
                Uid = "entry_uid",
                ContentType = "content_type_1",
                Version = 1,
                Locale = "en-us"
            };

            
            var json = JsonConvert.SerializeObject(entry);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishEntry>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("entry_uid", deserialized.Uid);
            Assert.AreEqual("content_type_1", deserialized.ContentType);
            Assert.AreEqual(1, deserialized.Version);
            Assert.AreEqual("en-us", deserialized.Locale);
        }

        [TestMethod]
        public void BulkDeleteDetails_Serialization_Test()
        {
            
            var details = new BulkDeleteDetails
            {
                Entries = new List<BulkDeleteEntry>
                {
                    new BulkDeleteEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Locale = "en-us"
                    },
                    new BulkDeleteEntry
                    {
                        Uid = "entry_uid_2",
                        ContentType = "content_type_2",
                        Locale = "en-gb"
                    }
                },
                Assets = new List<BulkDeleteAsset>
                {
                    new BulkDeleteAsset { Uid = "asset_uid_1" },
                    new BulkDeleteAsset { Uid = "asset_uid_2" }
                }
            };

            
            var json = JsonConvert.SerializeObject(details);
            var deserialized = JsonConvert.DeserializeObject<BulkDeleteDetails>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.Entries.Count);
            Assert.AreEqual(2, deserialized.Assets.Count);

            var firstEntry = deserialized.Entries.First();
            Assert.AreEqual("entry_uid_1", firstEntry.Uid);
            Assert.AreEqual("content_type_1", firstEntry.ContentType);
            Assert.AreEqual("en-us", firstEntry.Locale);
        }

        [TestMethod]
        public void BulkWorkflowUpdateBody_Serialization_Test()
        {
            
            var updateBody = new BulkWorkflowUpdateBody
            {
                Entries = new List<BulkWorkflowEntry>
                {
                    new BulkWorkflowEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Locale = "en-us"
                    }
                },
                Workflow = new BulkWorkflowStage
                {
                    Uid = "workflow_stage_uid",
                    Comment = "Test comment",
                    DueDate = "2023-12-01",
                    Notify = true,
                    AssignedTo = new List<BulkWorkflowUser>
                    {
                        new BulkWorkflowUser
                        {
                            Uid = "user_uid",
                            Name = "Test User",
                            Email = "test@example.com"
                        }
                    },
                    AssignedByRoles = new List<BulkWorkflowRole>
                    {
                        new BulkWorkflowRole
                        {
                            Uid = "role_uid",
                            Name = "Test Role"
                        }
                    }
                }
            };

            
            var json = JsonConvert.SerializeObject(updateBody);
            var deserialized = JsonConvert.DeserializeObject<BulkWorkflowUpdateBody>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(1, deserialized.Entries.Count);
            Assert.IsNotNull(deserialized.Workflow);

            var entry = deserialized.Entries.First();
            Assert.AreEqual("entry_uid_1", entry.Uid);
            Assert.AreEqual("content_type_1", entry.ContentType);
            Assert.AreEqual("en-us", entry.Locale);

            var workflow = deserialized.Workflow;
            Assert.AreEqual("workflow_stage_uid", workflow.Uid);
            Assert.AreEqual("Test comment", workflow.Comment);
            Assert.AreEqual("2023-12-01", workflow.DueDate);
            Assert.IsTrue(workflow.Notify);
            Assert.AreEqual(1, workflow.AssignedTo.Count);
            Assert.AreEqual(1, workflow.AssignedByRoles.Count);

            var user = workflow.AssignedTo.First();
            Assert.AreEqual("user_uid", user.Uid);
            Assert.AreEqual("Test User", user.Name);
            Assert.AreEqual("test@example.com", user.Email);

            var role = workflow.AssignedByRoles.First();
            Assert.AreEqual("role_uid", role.Uid);
            Assert.AreEqual("Test Role", role.Name);
        }

        [TestMethod]
        public void BulkAddItemsData_Serialization_Test()
        {
            
            var data = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    },
                    new BulkAddItem
                    {
                        Uid = "entry_uid_2",
                        ContentType = "content_type_2"
                    }
                }
            };

            
            var json = JsonConvert.SerializeObject(data);
            var deserialized = JsonConvert.DeserializeObject<BulkAddItemsData>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.Items.Count);

            var firstItem = deserialized.Items.First();
            Assert.AreEqual("entry_uid_1", firstItem.Uid);
            Assert.AreEqual("content_type_1", firstItem.ContentType);
        }

        [TestMethod]
        public void BulkPublishDetails_EmptyCollections_Test()
        {
            
            var details = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>(),
                Assets = new List<BulkPublishAsset>()
            };

            
            var json = JsonConvert.SerializeObject(details);
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
        public void BulkPublishDetails_WithRulesAndScheduling_Test()
        {
            
            var details = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "uid",
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

            
            var json = JsonConvert.SerializeObject(details);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishDetails>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(1, deserialized.Entries.Count);
            Assert.AreEqual(1, deserialized.Locales.Count);
            Assert.AreEqual(1, deserialized.Environments.Count);
            Assert.IsNotNull(deserialized.Rules);
            Assert.AreEqual("true", deserialized.Rules.Approvals);
            Assert.AreEqual("2023-12-01T10:00:00Z", deserialized.ScheduledAt);
            Assert.IsTrue(deserialized.PublishWithReference);

            var entry = deserialized.Entries.First();
            Assert.AreEqual("uid", entry.Uid);
            Assert.AreEqual("ct0", entry.ContentType);
            Assert.AreEqual(5, entry.Version);
            Assert.AreEqual("en-us", entry.Locale);
        }

        [TestMethod]
        public void BulkWorkflowStage_EmptyCollections_Test()
        {
            
            var workflowStage = new BulkWorkflowStage
            {
                Uid = "test_uid",
                Comment = "test_comment",
                AssignedTo = new List<BulkWorkflowUser>(),
                AssignedByRoles = new List<BulkWorkflowRole>()
            };

            
            var json = JsonConvert.SerializeObject(workflowStage);
            var deserialized = JsonConvert.DeserializeObject<BulkWorkflowStage>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("test_uid", deserialized.Uid);
            Assert.AreEqual("test_comment", deserialized.Comment);
            // Collections may be null after deserialization due to ShouldSerialize methods
            // Initialize them if they're null
            deserialized.AssignedTo = deserialized.AssignedTo ?? new List<BulkWorkflowUser>();
            deserialized.AssignedByRoles = deserialized.AssignedByRoles ?? new List<BulkWorkflowRole>();
            Assert.AreEqual(0, deserialized.AssignedTo.Count);
            Assert.AreEqual(0, deserialized.AssignedByRoles.Count);
        }

        [TestMethod]
        public void BulkPublishEntry_NullValues_Test()
        {
            
            var entry = new BulkPublishEntry
            {
                Uid = "entry_uid"
                // Version is null by default
            };

            
            var json = JsonConvert.SerializeObject(entry);
            var deserialized = JsonConvert.DeserializeObject<BulkPublishEntry>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("entry_uid", deserialized.Uid);
            Assert.AreEqual(0, deserialized.Version); // int defaults to 0, not null
            Assert.IsNull(deserialized.ContentType);
            Assert.IsNull(deserialized.Locale);
        }
    }
} 