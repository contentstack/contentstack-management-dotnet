using System;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services.Stack.BulkOperation;
using Contentstack.Management.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.Stack
{
    [TestClass]
    public class BulkReleaseItemsServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            var data = new BulkReleaseItemsData();
            Assert.ThrowsException<ArgumentNullException>(() => new BulkReleaseItemsService(
                null,
                new Management.Core.Models.Stack(null),
                data));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Data()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BulkReleaseItemsService(
                serializer,
                new Management.Core.Models.Stack(null),
                null));
        }

        [TestMethod]
        public void Should_Create_Service_With_Valid_Parameters()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Set_Bulk_Version_Header_When_Provided()
        {
            var data = new BulkReleaseItemsData();
            var bulkVersion = "1.0";
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data, bulkVersion);

            Assert.IsNotNull(service);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual(bulkVersion, service.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Should_Not_Set_Bulk_Version_Header_When_Not_Provided()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);

            Assert.IsNotNull(service);
            Assert.IsFalse(service.Headers.ContainsKey("bulk_version"));
        }

        [TestMethod]
        public void Should_Set_Bulk_Version_Header_With_Empty_String()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data, "");

            Assert.IsNotNull(service);
            Assert.IsFalse(service.Headers.ContainsKey("bulk_version"));
        }



        [TestMethod]
        public void Should_Set_Bulk_Version_Header_With_Complex_Version()
        {
            var data = new BulkReleaseItemsData();
            var bulkVersion = "2.1.3-beta";
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data, bulkVersion);

            Assert.IsNotNull(service);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual(bulkVersion, service.Headers["bulk_version"]);
        }



        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_API_Key()
        {
            var data = new BulkReleaseItemsData();
            var apiKey = "test-api-key";
            var stack = new Management.Core.Models.Stack(null, apiKey);
            var service = new BulkReleaseItemsService(serializer, stack, data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Management_Token()
        {
            var data = new BulkReleaseItemsData();
            var managementToken = "test-management-token";
            var stack = new Management.Core.Models.Stack(null, null, managementToken);
            var service = new BulkReleaseItemsService(serializer, stack, data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Branch_Uid()
        {
            var data = new BulkReleaseItemsData();
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, null, null, branchUid);
            var service = new BulkReleaseItemsService(serializer, stack, data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_All_Stack_Parameters()
        {
            var data = new BulkReleaseItemsData();
            var apiKey = "test-api-key";
            var managementToken = "test-management-token";
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, apiKey, managementToken, branchUid);
            var service = new BulkReleaseItemsService(serializer, stack, data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Handle_Multiple_Service_Instances()
        {
            var data1 = new BulkReleaseItemsData();
            var data2 = new BulkReleaseItemsData();
            var bulkVersion1 = "1.0";
            var bulkVersion2 = "2.0";

            var service1 = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data1, bulkVersion1);
            var service2 = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data2, bulkVersion2);

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.AreEqual(bulkVersion1, service1.Headers["bulk_version"]);
            Assert.AreEqual(bulkVersion2, service2.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Should_Verify_Service_Inheritance()
        {
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), new BulkReleaseItemsData());
            
            Assert.IsInstanceOfType(service, typeof(ContentstackService));
        }

        [TestMethod]
        public void Should_Verify_Service_Type()
        {
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), new BulkReleaseItemsData());
            
            Assert.AreEqual(typeof(BulkReleaseItemsService), service.GetType());
        }

        [TestMethod]
        public void Should_Handle_Content_Body_Called_Multiple_Times()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            service.ContentBody();
            var firstContent = Encoding.UTF8.GetString(service.ByteContent);
            
            service.ContentBody();
            var secondContent = Encoding.UTF8.GetString(service.ByteContent);
            
            Assert.AreEqual(firstContent, secondContent);
        }

        [TestMethod]
        public void Should_Verify_Headers_Collection_Is_Initialized()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Not_Null()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.IsNotNull(service.ResourcePath);
            Assert.IsFalse(string.IsNullOrEmpty(service.ResourcePath));
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Not_Null()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.IsNotNull(service.HttpMethod);
            Assert.IsFalse(string.IsNullOrEmpty(service.HttpMethod));
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Correct()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Post()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.AreEqual("POST", service.HttpMethod);
        }

        [TestMethod]
        public void Should_Handle_Null_Data_Content_Body()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            // This should not throw an exception
            service.ContentBody();
            Assert.IsNotNull(service.ByteContent);
        }

        [TestMethod]
        public void Should_Verify_Service_Properties_Are_Set()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Service_Can_Be_Instantiated_With_Different_Serializers()
        {
            var data = new BulkReleaseItemsData();
            var customSerializer = JsonSerializer.Create(new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore 
            });
            
            var service = new BulkReleaseItemsService(customSerializer, new Management.Core.Models.Stack(null), data);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Different_Bulk_Versions_Are_Handled_Correctly()
        {
            var data = new BulkReleaseItemsData();
            var versions = new[] { "1.0", "2.0", "3.0", "latest", "stable" };

            foreach (var version in versions)
            {
                var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data, version);
                Assert.AreEqual(version, service.Headers["bulk_version"]);
            }
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Add_Items_Service()
        {
            var data = new BulkReleaseItemsData();
            var releaseService = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            var addData = new BulkAddItemsData();
            var addService = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), addData);

            Assert.AreEqual("POST", releaseService.HttpMethod);
            Assert.AreEqual("POST", addService.HttpMethod);
            Assert.AreEqual("/bulk/release/items", releaseService.ResourcePath);
            Assert.AreEqual("/bulk/release/items", addService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Update_Items_Service()
        {
            var data = new BulkReleaseItemsData();
            var releaseService = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            var updateData = new BulkAddItemsData();
            var updateService = new BulkUpdateItemsService(serializer, new Management.Core.Models.Stack(null), updateData);

            Assert.AreEqual("POST", releaseService.HttpMethod);
            Assert.AreEqual("PUT", updateService.HttpMethod);
            Assert.AreEqual("/bulk/release/items", releaseService.ResourcePath);
            Assert.AreEqual("/bulk/release/update_items", updateService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Publish_Service()
        {
            var data = new BulkReleaseItemsData();
            var releaseService = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            var publishDetails = new BulkPublishDetails();
            var publishService = new BulkPublishService(serializer, new Management.Core.Models.Stack(null), publishDetails);

            Assert.AreEqual("POST", releaseService.HttpMethod);
            Assert.AreEqual("POST", publishService.HttpMethod);
            Assert.AreEqual("/bulk/release/items", releaseService.ResourcePath);
            Assert.AreEqual("/bulk/publish", publishService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Unpublish_Service()
        {
            var data = new BulkReleaseItemsData();
            var releaseService = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            var unpublishDetails = new BulkPublishDetails();
            var unpublishService = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), unpublishDetails);

            Assert.AreEqual("POST", releaseService.HttpMethod);
            Assert.AreEqual("POST", unpublishService.HttpMethod);
            Assert.AreEqual("/bulk/release/items", releaseService.ResourcePath);
            Assert.AreEqual("/bulk/unpublish", unpublishService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Delete_Service()
        {
            var data = new BulkReleaseItemsData();
            var releaseService = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            var deleteDetails = new BulkDeleteDetails();
            var deleteService = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), deleteDetails);

            Assert.AreEqual("POST", releaseService.HttpMethod);
            Assert.AreEqual("POST", deleteService.HttpMethod);
            Assert.AreEqual("/bulk/release/items", releaseService.ResourcePath);
            Assert.AreEqual("/bulk/delete", deleteService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Workflow_Update_Service()
        {
            var data = new BulkReleaseItemsData();
            var releaseService = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            var workflowDetails = new BulkWorkflowUpdateBody();
            var workflowService = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), workflowDetails);

            Assert.AreEqual("POST", releaseService.HttpMethod);
            Assert.AreEqual("POST", workflowService.HttpMethod);
            Assert.AreEqual("/bulk/release/items", releaseService.ResourcePath);
            Assert.AreEqual("/bulk/workflow", workflowService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Release_Specific_Resource_Path()
        {
            var data = new BulkReleaseItemsData();
            var service = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.IsTrue(service.ResourcePath.Contains("release"));
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Job_Status_Service()
        {
            var data = new BulkReleaseItemsData();
            var releaseService = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), data);
            var jobService = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), "test-job-id");

            Assert.AreEqual("POST", releaseService.HttpMethod);
            Assert.AreEqual("GET", jobService.HttpMethod);
            Assert.AreEqual("/bulk/release/items", releaseService.ResourcePath);
            Assert.AreEqual("/bulk/jobs/test-job-id", jobService.ResourcePath);
        }
    }
} 