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
    public class BulkAddItemsServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            var data = new BulkAddItemsData();
            Assert.ThrowsException<ArgumentNullException>(() => new BulkAddItemsService(
                null,
                new Management.Core.Models.Stack(null),
                data));
        }

        [TestMethod]
        public void Should_Throw_On_Null_Data()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BulkAddItemsService(
                serializer,
                new Management.Core.Models.Stack(null),
                null));
        }

        [TestMethod]
        public void Should_Create_Service_With_Valid_Parameters()
        {
            var data = new BulkAddItemsData();
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Set_Bulk_Version_Header_When_Provided()
        {
            var data = new BulkAddItemsData();
            var bulkVersion = "1.0";
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data, bulkVersion);

            Assert.IsNotNull(service);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual(bulkVersion, service.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Should_Not_Set_Bulk_Version_Header_When_Not_Provided()
        {
            var data = new BulkAddItemsData();
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data);

            Assert.IsNotNull(service);
            Assert.IsFalse(service.Headers.ContainsKey("bulk_version"));
        }

        [TestMethod]
        public void Should_Set_Bulk_Version_Header_With_Empty_String()
        {
            var data = new BulkAddItemsData();
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data, "");

            Assert.IsNotNull(service);
            Assert.IsFalse(service.Headers.ContainsKey("bulk_version"));
        }



        [TestMethod]
        public void Should_Set_Bulk_Version_Header_With_Complex_Version()
        {
            var data = new BulkAddItemsData();
            var bulkVersion = "2.1.3-beta";
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data, bulkVersion);

            Assert.IsNotNull(service);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual(bulkVersion, service.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Should_Create_Content_Body_From_Data()
        {
            var data = new BulkAddItemsData();
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data);
            service.ContentBody();

            Assert.IsNotNull(service.ByteContent);
            var content = Encoding.UTF8.GetString(service.ByteContent);
            Assert.IsTrue(content.Contains("{}")); // Empty JSON object for empty data
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_API_Key()
        {
            var data = new BulkAddItemsData();
            var apiKey = "test-api-key";
            var stack = new Management.Core.Models.Stack(null, apiKey);
            var service = new BulkAddItemsService(serializer, stack, data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Management_Token()
        {
            var data = new BulkAddItemsData();
            var managementToken = "test-management-token";
            var stack = new Management.Core.Models.Stack(null, null, managementToken);
            var service = new BulkAddItemsService(serializer, stack, data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Branch_Uid()
        {
            var data = new BulkAddItemsData();
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, null, null, branchUid);
            var service = new BulkAddItemsService(serializer, stack, data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_All_Stack_Parameters()
        {
            var data = new BulkAddItemsData();
            var apiKey = "test-api-key";
            var managementToken = "test-management-token";
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, apiKey, managementToken, branchUid);
            var service = new BulkAddItemsService(serializer, stack, data);

            Assert.IsNotNull(service);
            Assert.AreEqual("POST", service.HttpMethod);
            Assert.AreEqual("/bulk/release/items", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Handle_Multiple_Service_Instances()
        {
            var data1 = new BulkAddItemsData();
            var data2 = new BulkAddItemsData();
            var bulkVersion1 = "1.0";
            var bulkVersion2 = "2.0";

            var service1 = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data1, bulkVersion1);
            var service2 = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data2, bulkVersion2);

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.AreEqual(bulkVersion1, service1.Headers["bulk_version"]);
            Assert.AreEqual(bulkVersion2, service2.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Should_Verify_Service_Inheritance()
        {
            var data = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), new BulkAddItemsData());
            
            Assert.IsInstanceOfType(data, typeof(ContentstackService));
        }

        [TestMethod]
        public void Should_Verify_Service_Type()
        {
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), new BulkAddItemsData());
            
            Assert.AreEqual(typeof(BulkAddItemsService), service.GetType());
        }

        [TestMethod]
        public void Should_Handle_Content_Body_Called_Multiple_Times()
        {
            var data = new BulkAddItemsData();
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            service.ContentBody();
            var firstContent = Encoding.UTF8.GetString(service.ByteContent);
            
            service.ContentBody();
            var secondContent = Encoding.UTF8.GetString(service.ByteContent);
            
            Assert.AreEqual(firstContent, secondContent);
        }

        [TestMethod]
        public void Should_Verify_Headers_Collection_Is_Initialized()
        {
            var data = new BulkAddItemsData();
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Not_Null()
        {
            var data = new BulkAddItemsData();
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.IsNotNull(service.ResourcePath);
            Assert.IsFalse(string.IsNullOrEmpty(service.ResourcePath));
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Not_Null()
        {
            var data = new BulkAddItemsData();
            var service = new BulkAddItemsService(serializer, new Management.Core.Models.Stack(null), data);
            
            Assert.IsNotNull(service.HttpMethod);
            Assert.IsFalse(string.IsNullOrEmpty(service.HttpMethod));
        }
    }
} 