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
    public class BulkDeleteServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            var details = new BulkDeleteDetails();
            Assert.ThrowsException<ArgumentNullException>(() => new BulkDeleteService(
                null,
                new Management.Core.Models.Stack(null),
                details));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Details()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BulkDeleteService(
                serializer,
                new Management.Core.Models.Stack(null),
                null));
        }

        [TestMethod]
        public void Should_Create_Service_With_Valid_Parameters()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/delete", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Content_Body_From_Details()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);
            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            var content = Encoding.UTF8.GetString(service.ByteContent);
            Assert.IsTrue(content.Contains("{}")); // Empty JSON object for empty details
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_API_Key()
        {
            var details = new BulkDeleteDetails();
            var apiKey = "test-api-key";
            var stack = new Management.Core.Models.Stack(null, apiKey);
            var service = new BulkDeleteService(serializer, stack, details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/delete", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Management_Token()
        {
            var details = new BulkDeleteDetails();
            var managementToken = "test-management-token";
            var stack = new Management.Core.Models.Stack(null, null, managementToken);
            var service = new BulkDeleteService(serializer, stack, details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/delete", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Branch_Uid()
        {
            var details = new BulkDeleteDetails();
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, null, null, branchUid);
            var service = new BulkDeleteService(serializer, stack, details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/delete", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_All_Stack_Parameters()
        {
            var details = new BulkDeleteDetails();
            var apiKey = "test-api-key";
            var managementToken = "test-management-token";
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, apiKey, managementToken, branchUid);
            var service = new BulkDeleteService(serializer, stack, details);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/delete", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Handle_Multiple_Service_Instances()
        {
            var details1 = new BulkDeleteDetails();
            var details2 = new BulkDeleteDetails();

            var service1 = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details1);
            var service2 = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details2);

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.AreEqual("/bulk/delete", service1.ResourcePath);
            Assert.AreEqual("/bulk/delete", service2.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Inheritance()
        {
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), new BulkDeleteDetails());
            
            Assert.IsInstanceOfType(service, typeof(ContentstackService));
        }

        [TestMethod]
        public void Should_Verify_Service_Type()
        {
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), new BulkDeleteDetails());
            
            Assert.AreEqual(typeof(BulkDeleteService), service.GetType());
        }

        [TestMethod]
        public void Should_Handle_Content_Body_Called_Multiple_Times()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);
            
            service.ContentBody();
            var firstContent = Encoding.UTF8.GetString(service.ByteContent);
            
            service.ContentBody();
            var secondContent = Encoding.UTF8.GetString(service.ByteContent);
            
            Assert.AreEqual(firstContent, secondContent);
        }

        [TestMethod]
        public void Should_Verify_Headers_Collection_Is_Initialized()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Not_Null()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service.ResourcePath);
            Assert.IsFalse(string.IsNullOrEmpty(service.ResourcePath));
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Not_Null()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service.HttpMethod);
            Assert.IsFalse(string.IsNullOrEmpty(service.HttpMethod));
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Correct()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.AreEqual("/bulk/delete", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Post()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.AreEqual("POST", service.HttpMethod);
        }

        [TestMethod]
        public void Should_Handle_Null_Details_Content_Body()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);
            
            // This should not throw an exception
            service.ContentBody();
            Assert.IsNotNull(service.ByteContent);
        }

        [TestMethod]
        public void Should_Verify_Service_Properties_Are_Set()
        {
            var details = new BulkDeleteDetails();
            var service = new BulkDeleteService(serializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/delete", service.ResourcePath);
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Service_Can_Be_Instantiated_With_Different_Serializers()
        {
            var details = new BulkDeleteDetails();
            var customSerializer = JsonSerializer.Create(new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore 
            });
            
            var service = new BulkDeleteService(customSerializer, new Management.Core.Models.Stack(null), details);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/delete", service.ResourcePath);
        }
    }
} 