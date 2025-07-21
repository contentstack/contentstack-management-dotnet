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
    public class BulkWorkflowUpdateServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            Assert.ThrowsException<ArgumentNullException>(() => new BulkWorkflowUpdateService(
                null,
                new Management.Core.Models.Stack(null),
                updateBody));
        }

        [TestMethod]
        public void Should_Throw_On_Null_UpdateBody()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BulkWorkflowUpdateService(
                serializer,
                new Management.Core.Models.Stack(null),
                null));
        }

        [TestMethod]
        public void Should_Create_Service_With_Valid_Parameters()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
        }



        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_API_Key()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var apiKey = "test-api-key";
            var stack = new Management.Core.Models.Stack(null, apiKey);
            var service = new BulkWorkflowUpdateService(serializer, stack, updateBody);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Management_Token()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var managementToken = "test-management-token";
            var stack = new Management.Core.Models.Stack(null, null, managementToken);
            var service = new BulkWorkflowUpdateService(serializer, stack, updateBody);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Branch_Uid()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, null, null, branchUid);
            var service = new BulkWorkflowUpdateService(serializer, stack, updateBody);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_All_Stack_Parameters()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var apiKey = "test-api-key";
            var managementToken = "test-management-token";
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, apiKey, managementToken, branchUid);
            var service = new BulkWorkflowUpdateService(serializer, stack, updateBody);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Handle_Multiple_Service_Instances()
        {
            var updateBody1 = new BulkWorkflowUpdateBody();
            var updateBody2 = new BulkWorkflowUpdateBody();

            var service1 = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody1);
            var service2 = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody2);

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.AreEqual("/bulk/workflow", service1.ResourcePath);
            Assert.AreEqual("/bulk/workflow", service2.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Inheritance()
        {
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), new BulkWorkflowUpdateBody());
            
            Assert.IsInstanceOfType(service, typeof(ContentstackService));
        }

        [TestMethod]
        public void Should_Verify_Service_Type()
        {
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), new BulkWorkflowUpdateBody());
            
            Assert.AreEqual(typeof(BulkWorkflowUpdateService), service.GetType());
        }

        [TestMethod]
        public void Should_Handle_Content_Body_Called_Multiple_Times()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            
            service.ContentBody();
            var firstContent = Encoding.UTF8.GetString(service.ByteContent);
            
            service.ContentBody();
            var secondContent = Encoding.UTF8.GetString(service.ByteContent);
            
            Assert.AreEqual(firstContent, secondContent);
        }

        [TestMethod]
        public void Should_Verify_Headers_Collection_Is_Initialized()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Not_Null()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            
            Assert.IsNotNull(service.ResourcePath);
            Assert.IsFalse(string.IsNullOrEmpty(service.ResourcePath));
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Not_Null()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            
            Assert.IsNotNull(service.HttpMethod);
            Assert.IsFalse(string.IsNullOrEmpty(service.HttpMethod));
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Correct()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Post()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            
            Assert.AreEqual("POST", service.HttpMethod);
        }

        [TestMethod]
        public void Should_Handle_Null_UpdateBody_Content_Body()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            
            // This should not throw an exception
            service.ContentBody();
            Assert.IsNotNull(service.ByteContent);
        }

        [TestMethod]
        public void Should_Verify_Service_Properties_Are_Set()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Service_Can_Be_Instantiated_With_Different_Serializers()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var customSerializer = JsonSerializer.Create(new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore 
            });
            
            var service = new BulkWorkflowUpdateService(customSerializer, new Management.Core.Models.Stack(null), updateBody);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Publish_Service()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var workflowService = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            var publishDetails = new BulkPublishDetails();
            var publishService = new BulkPublishService(serializer, new Management.Core.Models.Stack(null), publishDetails);

            Assert.AreEqual("POST", workflowService.HttpMethod);
            Assert.AreEqual("POST", publishService.HttpMethod);
            Assert.AreEqual("/bulk/workflow", workflowService.ResourcePath);
            Assert.AreEqual("/bulk/publish", publishService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Unpublish_Service()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var workflowService = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            var unpublishDetails = new BulkPublishDetails();
            var unpublishService = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), unpublishDetails);

            Assert.AreEqual("POST", workflowService.HttpMethod);
            Assert.AreEqual("POST", unpublishService.HttpMethod);
            Assert.AreEqual("/bulk/workflow", workflowService.ResourcePath);
            Assert.AreEqual("/bulk/unpublish", unpublishService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Delete_Service()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var workflowService = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            var deleteDetails = new BulkDeleteDetails();
            var deleteService = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), deleteDetails);

            Assert.AreEqual("POST", workflowService.HttpMethod);
            Assert.AreEqual("POST", deleteService.HttpMethod);
            Assert.AreEqual("/bulk/workflow", workflowService.ResourcePath);
            Assert.AreEqual("/bulk/delete", deleteService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Add_Items_Service()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var workflowService = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            var addData = new BulkAddItemsData();
            var addService = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), addData);

            Assert.AreEqual("POST", workflowService.HttpMethod);
            Assert.AreEqual("POST", addService.HttpMethod);
            Assert.AreEqual("/bulk/workflow", workflowService.ResourcePath);
            Assert.AreEqual("/bulk/release/items", addService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Update_Items_Service()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var workflowService = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            var updateData = new BulkAddItemsData();
            var updateService = new BulkUpdateItemsService(serializer, new Management.Core.Models.Stack(null), updateData);

            Assert.AreEqual("POST", workflowService.HttpMethod);
            Assert.AreEqual("PUT", updateService.HttpMethod);
            Assert.AreEqual("/bulk/workflow", workflowService.ResourcePath);
            Assert.AreEqual("/bulk/release/update_items", updateService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Release_Items_Service()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var workflowService = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            var releaseData = new BulkReleaseItemsData();
            var releaseService = new BulkReleaseItemsService(serializer, new Management.Core.Models.Stack(null), releaseData);

            Assert.AreEqual("POST", workflowService.HttpMethod);
            Assert.AreEqual("POST", releaseService.HttpMethod);
            Assert.AreEqual("/bulk/workflow", workflowService.ResourcePath);
            Assert.AreEqual("/bulk/release/items", releaseService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Workflow_Specific_Resource_Path()
        {
            var updateBody = new BulkWorkflowUpdateBody();
            var service = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), updateBody);
            
            Assert.IsTrue(service.ResourcePath.Contains("workflow"));
            Assert.AreEqual("/bulk/workflow", service.ResourcePath);
        }
    }
} 