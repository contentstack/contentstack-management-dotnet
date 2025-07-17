using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services.Stack.BulkOperation;
using Contentstack.Management.Core;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace Contentstack.Management.Core.Unit.Tests.Services
{
    [TestClass]
    public class BulkOperationServicesTest
    {
        private JsonSerializer _serializer;
        private Stack _stack;

        [TestInitialize]
        public void Setup()
        {
            _serializer = new JsonSerializer();
            var client = new ContentstackClient("test_token", "test_host");
            _stack = client.Stack("test_api_key");
        }

        [TestMethod]
        public void Test001_BulkReleaseItemsService_Initialization()
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

            
            var service = new BulkReleaseItemsService(_serializer, _stack, releaseData, "1.0");

            Assert.IsNotNull(service);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual("1.0", service.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Test002_BulkReleaseItemsService_ContentBody()
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

            var service = new BulkReleaseItemsService(_serializer, _stack, releaseData);

            
            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            Assert.IsTrue(service.ByteContent.Length > 0);
            
            var json = Encoding.UTF8.GetString(service.ByteContent);
            Assert.IsTrue(json.Contains("release"));
            Assert.IsTrue(json.Contains("action"));
            Assert.IsTrue(json.Contains("locale"));
            Assert.IsTrue(json.Contains("reference"));
            Assert.IsTrue(json.Contains("items"));
        }

        [TestMethod]
        public void Test003_BulkReleaseItemsService_Without_BulkVersion()
        {
            
            var releaseData = new BulkReleaseItemsData
            {
                Release = "release_uid",
                Action = "publish",
                Locale = new List<string> { "en-us" },
                Reference = true,
                Items = new List<BulkReleaseItem>()
            };

            
            var service = new BulkReleaseItemsService(_serializer, _stack, releaseData);

            Assert.IsNotNull(service);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.IsFalse(service.Headers.ContainsKey("bulk_version"));
        }

        [TestMethod]
        public void Test004_BulkPublishService_Initialization()
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
                Environments = new List<string> { "env1" }
            };

            
            var service = new BulkPublishService(_serializer, _stack, publishDetails, true, true, true);

            Assert.IsNotNull(service);
            Assert.AreEqual("/bulk/publish", service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.IsTrue(service.Headers.ContainsKey("skip_workflow_stage_check"));
            Assert.IsTrue(service.Headers.ContainsKey("approvals"));
            Assert.AreEqual("true", service.Headers["skip_workflow_stage_check"]);
            Assert.AreEqual("true", service.Headers["approvals"]);
        }

        [TestMethod]
        public void Test005_BulkPublishService_ContentBody()
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
                Environments = new List<string> { "env1" }
            };

            var service = new BulkPublishService(_serializer, _stack, publishDetails);

            
            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            Assert.IsTrue(service.ByteContent.Length > 0);
            
            var json = Encoding.UTF8.GetString(service.ByteContent);
            Assert.IsTrue(json.Contains("entries"));
            Assert.IsTrue(json.Contains("locales"));
            Assert.IsTrue(json.Contains("environments"));
        }

        [TestMethod]
        public void Test006_BulkPublishService_With_All_Flags()
        {
            
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>(),
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "env1" }
            };

            
            var service = new BulkPublishService(_serializer, _stack, publishDetails, true, true, true);

            Assert.IsNotNull(service);
            Assert.IsTrue(service.Headers.ContainsKey("skip_workflow_stage_check"));
            Assert.IsTrue(service.Headers.ContainsKey("approvals"));
            Assert.AreEqual("true", service.Headers["skip_workflow_stage_check"]);
            Assert.AreEqual("true", service.Headers["approvals"]);
        }

        [TestMethod]
        public void Test007_BulkPublishService_Without_Flags()
        {
            
            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>(),
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "env1" }
            };

            
            var service = new BulkPublishService(_serializer, _stack, publishDetails, false, false, false);

            Assert.IsNotNull(service);
            Assert.IsFalse(service.Headers.ContainsKey("skip_workflow_stage_check"));
            Assert.IsFalse(service.Headers.ContainsKey("approvals"));
        }

        [TestMethod]
        public void Test008_BulkUnpublishService_Initialization()
        {
            
            var unpublishDetails = new BulkPublishDetails
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
                Environments = new List<string> { "env1" }
            };

            
            var service = new BulkUnpublishService(_serializer, _stack, unpublishDetails, true, true, true);

            Assert.IsNotNull(service);
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.IsTrue(service.Headers.ContainsKey("skip_workflow_stage_check"));
            Assert.IsTrue(service.Headers.ContainsKey("approvals"));
        }

        [TestMethod]
        public void Test009_BulkUnpublishService_ContentBody()
        {
            
            var unpublishDetails = new BulkPublishDetails
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
                Environments = new List<string> { "env1" }
            };

            var service = new BulkUnpublishService(_serializer, _stack, unpublishDetails);

            
            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            Assert.IsTrue(service.ByteContent.Length > 0);
            
            var json = Encoding.UTF8.GetString(service.ByteContent);
            Assert.IsTrue(json.Contains("entries"));
            Assert.IsTrue(json.Contains("locales"));
            Assert.IsTrue(json.Contains("environments"));
        }

        [TestMethod]
        public void Test010_BulkDeleteService_Initialization()
        {
            
            var deleteDetails = new BulkDeleteDetails
            {
                Entries = new List<BulkDeleteEntry>
                {
                    new BulkDeleteEntry
                    {
                        Uid = "entry_uid",
                        ContentType = "ct0",
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkDeleteAsset>
                {
                    new BulkDeleteAsset { Uid = "asset_uid" }
                }
            };

            
            var service = new BulkDeleteService(_serializer, _stack, deleteDetails);

            Assert.IsNotNull(service);
            Assert.AreEqual("/bulk/delete", service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
        }

        [TestMethod]
        public void Test011_BulkDeleteService_ContentBody()
        {
            
            var deleteDetails = new BulkDeleteDetails
            {
                Entries = new List<BulkDeleteEntry>
                {
                    new BulkDeleteEntry
                    {
                        Uid = "entry_uid",
                        ContentType = "ct0",
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkDeleteAsset>
                {
                    new BulkDeleteAsset { Uid = "asset_uid" }
                }
            };

            var service = new BulkDeleteService(_serializer, _stack, deleteDetails);

            
            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            Assert.IsTrue(service.ByteContent.Length > 0);
            
            var json = Encoding.UTF8.GetString(service.ByteContent);
            Assert.IsTrue(json.Contains("entries"));
            Assert.IsTrue(json.Contains("assets"));
        }

        [TestMethod]
        public void Test012_BulkWorkflowUpdateService_Initialization()
        {
            
            var updateBody = new BulkWorkflowUpdateBody
            {
                Entries = new List<BulkWorkflowEntry>
                {
                    new BulkWorkflowEntry
                    {
                        Uid = "entry_uid",
                        ContentType = "ct0",
                        Locale = "en-us"
                    }
                },
                Workflow = new BulkWorkflowStage
                {
                    Uid = "workflow_stage_uid",
                    Comment = "Test comment",
                    DueDate = "2023-12-01",
                    Notify = true
                }
            };

            
            var service = new BulkWorkflowUpdateService(_serializer, _stack, updateBody);

            Assert.IsNotNull(service);
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
        }

        [TestMethod]
        public void Test013_BulkWorkflowUpdateService_ContentBody()
        {
            
            var updateBody = new BulkWorkflowUpdateBody
            {
                Entries = new List<BulkWorkflowEntry>
                {
                    new BulkWorkflowEntry
                    {
                        Uid = "entry_uid",
                        ContentType = "ct0",
                        Locale = "en-us"
                    }
                },
                Workflow = new BulkWorkflowStage
                {
                    Uid = "workflow_stage_uid",
                    Comment = "Test comment",
                    DueDate = "2023-12-01",
                    Notify = true
                }
            };

            var service = new BulkWorkflowUpdateService(_serializer, _stack, updateBody);

            
            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            Assert.IsTrue(service.ByteContent.Length > 0);
            
            var json = Encoding.UTF8.GetString(service.ByteContent);
            Assert.IsTrue(json.Contains("entries"));
            Assert.IsTrue(json.Contains("workflow"));
        }

        [TestMethod]
        public void Test014_BulkJobStatusService_Initialization()
        {
            
            string jobId = "test_job_id";
            string bulkVersion = "1.0";

            
            var service = new BulkJobStatusService(_serializer, _stack, jobId, bulkVersion);

            Assert.IsNotNull(service);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual("1.0", service.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Test015_BulkJobStatusService_Without_BulkVersion()
        {
            
            string jobId = "test_job_id";

            
            var service = new BulkJobStatusService(_serializer, _stack, jobId);

            Assert.IsNotNull(service);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.IsFalse(service.Headers.ContainsKey("bulk_version"));
        }

        [TestMethod]
        public void Test016_BulkAddItemsService_Initialization()
        {
            
            var addItemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "uid",
                        ContentType = "ct_1"
                    }
                }
            };

            
            var service = new BulkAddItemsService(_serializer, _stack, addItemsData, "1.0");

            Assert.IsNotNull(service);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual("1.0", service.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Test017_BulkUpdateItemsService_Initialization()
        {
            
            var updateItemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "uid",
                        ContentType = "ct_1"
                    }
                }
            };

            
            var service = new BulkUpdateItemsService(_serializer, _stack, updateItemsData, "1.0");

            Assert.IsNotNull(service);
            Assert.AreEqual("/bulk/release/update_items", service.ResourcePath);
            Assert.AreEqual("PUT", service.HttpMethod);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual("1.0", service.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Test018_BulkReleaseItemsService_Null_Data_Throws_Exception()
        {
              
            Assert.ThrowsException<System.ArgumentNullException>(() =>
            {
                new BulkReleaseItemsService(_serializer, _stack, null);
            });
        }

        [TestMethod]
        public void Test019_BulkPublishService_Null_Data_Throws_Exception()
        {
              
            Assert.ThrowsException<System.ArgumentNullException>(() =>
            {
                new BulkPublishService(_serializer, _stack, null);
            });
        }

        [TestMethod]
        public void Test020_BulkDeleteService_Null_Data_Throws_Exception()
        {
              
            Assert.ThrowsException<System.ArgumentNullException>(() =>
            {
                new BulkDeleteService(_serializer, _stack, null);
            });
        }
    }
} 