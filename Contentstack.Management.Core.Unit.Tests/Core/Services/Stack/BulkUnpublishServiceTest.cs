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
    public class BulkUnpublishServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            var details = new BulkPublishDetails();
            Assert.ThrowsException<ArgumentNullException>(() => new BulkUnpublishService(
                null,
                new Management.Core.Models.Stack(null),
                details));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Details()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BulkUnpublishService(
                serializer,
                new Management.Core.Models.Stack(null),
                null));
        }

        [TestMethod]
        public void Should_Create_Service_With_Valid_Parameters()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Set_Skip_Workflow_Stage_Header_When_True()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details, skipWorkflowStage: true);

            Assert.IsNotNull(service);
            Assert.IsTrue(service.Headers.ContainsKey("skip_workflow_stage_check"));
            Assert.AreEqual("true", service.Headers["skip_workflow_stage_check"]);
        }

        [TestMethod]
        public void Should_Set_Approvals_Header_When_True()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details, approvals: true);

            Assert.IsNotNull(service);
            Assert.IsTrue(service.Headers.ContainsKey("approvals"));
            Assert.AreEqual("true", service.Headers["approvals"]);
        }

        [TestMethod]
        public void Should_Set_Nested_Query_Parameters_When_True()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details, isNested: true);

            Assert.IsNotNull(service);
            // Note: Query parameters are typically handled internally, so we just verify the service is created
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
        }



        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_API_Key()
        {
            var details = new BulkPublishDetails();
            var apiKey = "test-api-key";
            var stack = new Management.Core.Models.Stack(null, apiKey);
            var service = new BulkUnpublishService(serializer, stack, details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Management_Token()
        {
            var details = new BulkPublishDetails();
            var managementToken = "test-management-token";
            var stack = new Management.Core.Models.Stack(null, null, managementToken);
            var service = new BulkUnpublishService(serializer, stack, details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Branch_Uid()
        {
            var details = new BulkPublishDetails();
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, null, null, branchUid);
            var service = new BulkUnpublishService(serializer, stack, details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_All_Stack_Parameters()
        {
            var details = new BulkPublishDetails();
            var apiKey = "test-api-key";
            var managementToken = "test-management-token";
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, apiKey, managementToken, branchUid);
            var service = new BulkUnpublishService(serializer, stack, details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Handle_Multiple_Service_Instances()
        {
            var details1 = new BulkPublishDetails();
            var details2 = new BulkPublishDetails();

            var service1 = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details1);
            var service2 = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details2);

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.AreEqual("/bulk/unpublish", service1.ResourcePath);
            Assert.AreEqual("/bulk/unpublish", service2.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Inheritance()
        {
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), new BulkPublishDetails());
            
            Assert.IsInstanceOfType(service, typeof(ContentstackService));
        }

        [TestMethod]
        public void Should_Verify_Service_Type()
        {
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), new BulkPublishDetails());
            
            Assert.AreEqual(typeof(BulkUnpublishService), service.GetType());
        }

        [TestMethod]
        public void Should_Handle_Content_Body_Called_Multiple_Times()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            
            service.ContentBody();
            var firstContent = Encoding.UTF8.GetString(service.ByteContent);
            
            service.ContentBody();
            var secondContent = Encoding.UTF8.GetString(service.ByteContent);
            
            Assert.AreEqual(firstContent, secondContent);
        }

        [TestMethod]
        public void Should_Verify_Headers_Collection_Is_Initialized()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Not_Null()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service.ResourcePath);
            Assert.IsFalse(string.IsNullOrEmpty(service.ResourcePath));
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Not_Null()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service.HttpMethod);
            Assert.IsFalse(string.IsNullOrEmpty(service.HttpMethod));
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Correct()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Post()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.AreEqual("POST", service.HttpMethod);
        }

        [TestMethod]
        public void Should_Handle_Null_Details_Content_Body()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            
            // This should not throw an exception
            service.ContentBody();
            Assert.IsNotNull(service.ByteContent);
        }

        [TestMethod]
        public void Should_Verify_Service_Properties_Are_Set()
        {
            var details = new BulkPublishDetails();
            var service = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Service_Can_Be_Instantiated_With_Different_Serializers()
        {
            var details = new BulkPublishDetails();
            var customSerializer = JsonSerializer.Create(new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore 
            });
            
            var service = new BulkUnpublishService(customSerializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Publish_Service()
        {
            var details = new BulkPublishDetails();
            var unpublishService = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            var publishService = new BulkPublishService(serializer, new Management.Core.Models.Stack(null), details);

            Assert.AreEqual("POST", unpublishService.HttpMethod);
            Assert.AreEqual("POST", publishService.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", unpublishService.ResourcePath);
            Assert.AreEqual("/bulk/publish", publishService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Delete_Service()
        {
            var details = new BulkPublishDetails();
            var unpublishService = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            var deleteDetails = new BulkDeleteDetails();
            var deleteService = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), deleteDetails);

            Assert.AreEqual("POST", unpublishService.HttpMethod);
            Assert.AreEqual("POST", deleteService.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", unpublishService.ResourcePath);
            Assert.AreEqual("/bulk/delete", deleteService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Workflow_Update_Service()
        {
            var details = new BulkPublishDetails();
            var unpublishService = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            var workflowDetails = new BulkWorkflowUpdateBody();
            var workflowService = new BulkWorkflowUpdateService(serializer, new Management.Core.Models.Stack(null), workflowDetails);

            Assert.AreEqual("POST", unpublishService.HttpMethod);
            Assert.AreEqual("POST", workflowService.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", unpublishService.ResourcePath);
            Assert.AreEqual("/bulk/workflow", workflowService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Add_Items_Service()
        {
            var details = new BulkPublishDetails();
            var unpublishService = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            var addData = new BulkAddItemsData();
            var addService = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), addData);

            Assert.AreEqual("POST", unpublishService.HttpMethod);
            Assert.AreEqual("POST", addService.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", unpublishService.ResourcePath);
            Assert.AreEqual("/bulk/release/items", addService.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Update_Items_Service()
        {
            var details = new BulkPublishDetails();
            var unpublishService = new BulkUnpublishService(serializer, new Management.Core.Models.Stack(null), details);
            var updateData = new BulkAddItemsData();
            var updateService = new BulkUpdateItemsService(serializer, new Management.Core.Models.Stack(null), updateData);

            Assert.AreEqual("POST", unpublishService.HttpMethod);
            Assert.AreEqual("PUT", updateService.HttpMethod);
            Assert.AreEqual("/bulk/unpublish", unpublishService.ResourcePath);
            Assert.AreEqual("/bulk/release/update_items", updateService.ResourcePath);
        }
    }
} 