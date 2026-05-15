using System;
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
    public class BulkJobStatusServiceTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            var jobId = "test-job-id";
            Assert.ThrowsException<ArgumentNullException>(() => new BulkJobStatusService(
                null,
                new Management.Core.Models.Stack(null),
                jobId));
        }

        [TestMethod]
        public void Should_Throw_On_Null_JobId()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BulkJobStatusService(
                serializer,
                new Management.Core.Models.Stack(null),
                null));
        }



        [TestMethod]
        public void Should_Create_Service_With_Valid_Parameters()
        {
            var jobId = "test-job-id";
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Set_Bulk_Version_Header_When_Provided()
        {
            var jobId = "test-job-id";
            var bulkVersion = "1.0";
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId, bulkVersion);

            Assert.IsNotNull(service);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual(bulkVersion, service.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Should_Not_Set_Bulk_Version_Header_When_Not_Provided()
        {
            var jobId = "test-job-id";
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId);

            Assert.IsNotNull(service);
            Assert.IsFalse(service.Headers.ContainsKey("bulk_version"));
        }

        [TestMethod]
        public void Should_Not_Set_Bulk_Version_Header_With_Empty_String()
        {
            var jobId = "test-job-id";
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId, "");

            Assert.IsNotNull(service);
            Assert.IsFalse(service.Headers.ContainsKey("bulk_version"));
        }



        [TestMethod]
        public void Should_Set_Bulk_Version_Header_With_Complex_Version()
        {
            var jobId = "test-job-id";
            var bulkVersion = "2.1.3-beta";
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId, bulkVersion);

            Assert.IsNotNull(service);
            Assert.IsTrue(service.Headers.ContainsKey("bulk_version"));
            Assert.AreEqual(bulkVersion, service.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_API_Key()
        {
            var jobId = "test-job-id";
            var apiKey = "test-api-key";
            var stack = new Management.Core.Models.Stack(null, apiKey);
            var service = new BulkJobStatusService(serializer, stack, jobId);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Management_Token()
        {
            var jobId = "test-job-id";
            var managementToken = "test-management-token";
            var stack = new Management.Core.Models.Stack(null, null, managementToken);
            var service = new BulkJobStatusService(serializer, stack, jobId);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_Stack_Having_Branch_Uid()
        {
            var jobId = "test-job-id";
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, null, null, branchUid);
            var service = new BulkJobStatusService(serializer, stack, jobId);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_Service_With_All_Stack_Parameters()
        {
            var jobId = "test-job-id";
            var apiKey = "test-api-key";
            var managementToken = "test-management-token";
            var branchUid = "test-branch-uid";
            var stack = new Management.Core.Models.Stack(null, apiKey, managementToken, branchUid);
            var service = new BulkJobStatusService(serializer, stack, jobId);

            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Handle_Multiple_Service_Instances()
        {
            var jobId1 = "job-1";
            var jobId2 = "job-2";
            var bulkVersion1 = "1.0";
            var bulkVersion2 = "2.0";

            var service1 = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId1, bulkVersion1);
            var service2 = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId2, bulkVersion2);

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.AreEqual($"/bulk/jobs/{jobId1}", service1.ResourcePath);
            Assert.AreEqual($"/bulk/jobs/{jobId2}", service2.ResourcePath);
            Assert.AreEqual(bulkVersion1, service1.Headers["bulk_version"]);
            Assert.AreEqual(bulkVersion2, service2.Headers["bulk_version"]);
        }

        [TestMethod]
        public void Should_Verify_Service_Inheritance()
        {
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), "test-job-id");
            
            Assert.IsInstanceOfType(service, typeof(ContentstackService));
        }

        [TestMethod]
        public void Should_Verify_Service_Type()
        {
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), "test-job-id");
            
            Assert.AreEqual(typeof(BulkJobStatusService), service.GetType());
        }

        [TestMethod]
        public void Should_Verify_Headers_Collection_Is_Initialized()
        {
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), "test-job-id");
            
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Not_Null()
        {
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), "test-job-id");
            
            Assert.IsNotNull(service.ResourcePath);
            Assert.IsFalse(string.IsNullOrEmpty(service.ResourcePath));
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Not_Null()
        {
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), "test-job-id");
            
            Assert.IsNotNull(service.HttpMethod);
            Assert.IsFalse(string.IsNullOrEmpty(service.HttpMethod));
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Is_Correct()
        {
            var jobId = "test-job-id";
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId);
            
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Http_Method_Is_Get()
        {
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), "test-job-id");
            
            Assert.AreEqual("GET", service.HttpMethod);
        }

        [TestMethod]
        public void Should_Verify_Service_Properties_Are_Set()
        {
            var jobId = "test-job-id";
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
            Assert.IsNotNull(service.Headers);
        }

        [TestMethod]
        public void Should_Verify_Service_Can_Be_Instantiated_With_Different_Serializers()
        {
            var jobId = "test-job-id";
            var customSerializer = JsonSerializer.Create(new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore 
            });
            
            var service = new BulkJobStatusService(customSerializer, new Management.Core.Models.Stack(null), jobId);
            
            Assert.IsNotNull(service);
            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Different_Job_Ids_Are_Handled_Correctly()
        {
            var jobIds = new[] { "job-1", "job-2", "job-3", "bulk-job-123", "test-job-id" };

            foreach (var jobId in jobIds)
            {
                var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId);
                Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
            }
        }

        [TestMethod]
        public void Should_Verify_Different_Bulk_Versions_Are_Handled_Correctly()
        {
            var jobId = "test-job-id";
            var versions = new[] { "1.0", "2.0", "3.0", "latest", "stable" };

            foreach (var version in versions)
            {
                var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId, version);
                Assert.AreEqual(version, service.Headers["bulk_version"]);
            }
        }

        [TestMethod]
        public void Should_Verify_Service_Distinction_From_Other_Services()
        {
            var jobId = "test-job-id";
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId);

            Assert.AreEqual("GET", service.HttpMethod);
            Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
            Assert.IsTrue(service.ResourcePath.Contains("/bulk/jobs/"));
        }

        [TestMethod]
        public void Should_Handle_Special_Characters_In_JobId()
        {
            var jobIds = new[] { "job-123", "job_456", "job.789", "job@test", "job#test" };

            foreach (var jobId in jobIds)
            {
                var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), jobId);
                Assert.AreEqual($"/bulk/jobs/{jobId}", service.ResourcePath);
            }
        }

        [TestMethod]
        public void Should_Handle_Long_JobId()
        {
            var longJobId = new string('a', 1000);
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), longJobId);
            
            Assert.AreEqual($"/bulk/jobs/{longJobId}", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Verify_Service_Has_No_Content_Body_Method()
        {
            var service = new BulkJobStatusService(serializer, new Management.Core.Models.Stack(null), "test-job-id");
            
            // GET requests typically don't have content body
            Assert.IsNull(service.ByteContent);
        }
    }
} 