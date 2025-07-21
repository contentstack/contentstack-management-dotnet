using System;
using System.Collections.Generic;
using AutoFixture;
using Contentstack.Management.Core.Abstractions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Services.Models
{
    [TestClass]
    public class GlobalFieldServiceTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;
        private MockHttpHandler _mockHandler;

        [TestInitialize]
        public void Initialize()
        {
            var client = new ContentstackClient();
            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            _mockHandler = new MockHttpHandler(_contentstackResponse);
            client.ContentstackPipeline.ReplaceHandler(_mockHandler);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
        }

        [TestMethod]
        public void Should_Create_GlobalFieldService_Without_ApiVersion()
        {
            // Arrange
            var model = new ContentModelling { Title = "Test" };
            var uid = _fixture.Create<string>();

            // Act
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", model, uid, null);

            // Assert
            Assert.IsNotNull(service);
            Assert.AreEqual("/global_fields", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_GlobalFieldService_With_ApiVersion()
        {
            // Arrange
            var model = new ContentModelling { Title = "Test" };
            var uid = _fixture.Create<string>();
            var apiVersion = "3.2";

            // Act
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", model, uid, apiVersion);

            // Assert
            Assert.IsNotNull(service);
            Assert.AreEqual("/global_fields", service.ResourcePath);
        }

        [TestMethod]
        public void Should_Add_ApiVersion_Header_When_Provided()
        {
            // Arrange
            var model = new ContentModelling { Title = "Test" };
            var uid = _fixture.Create<string>();
            var apiVersion = "3.2";

            // Act
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", model, uid, apiVersion);

            // Assert
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));
            Assert.AreEqual(apiVersion, service.Headers["api_version"]);
        }

        [TestMethod]
        public void Should_Not_Add_ApiVersion_Header_When_Not_Provided()
        {
            // Arrange
            var model = new ContentModelling { Title = "Test" };
            var uid = _fixture.Create<string>();

            // Act
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", model, uid, null);

            // Assert
            Assert.IsFalse(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Remove_ApiVersion_Header_After_Successful_Response()
        {
            // Arrange
            var model = new ContentModelling { Title = "Test" };
            var uid = _fixture.Create<string>();
            var apiVersion = "3.2";
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", model, uid, apiVersion);
            
            // Verify header is initially present
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));

            // Act - simulate successful response
            var mockResponse = new MockHttpResponse(200, "Success");
            service.OnResponse(mockResponse, _stack.client.contentstackOptions);

            // Assert
            Assert.IsFalse(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Not_Remove_ApiVersion_Header_After_Failed_Response()
        {
            // Arrange
            var model = new ContentModelling { Title = "Test" };
            var uid = _fixture.Create<string>();
            var apiVersion = "3.2";
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", model, uid, apiVersion);
            
            // Verify header is initially present
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));

            // Act - simulate failed response
            var mockResponse = new MockHttpResponse(400, "Bad Request");
            service.OnResponse(mockResponse, _stack.client.contentstackOptions);

            // Assert - header should still be present after failed response
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Not_Remove_ApiVersion_Header_When_No_ApiVersion_Was_Set()
        {
            // Arrange
            var model = new ContentModelling { Title = "Test" };
            var uid = _fixture.Create<string>();
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", model, uid, null);
            
            // Manually add api_version header (simulating it being added elsewhere)
            service.Headers["api_version"] = "3.2";

            // Act - simulate successful response
            var mockResponse = new MockHttpResponse(200, "Success");
            service.OnResponse(mockResponse, _stack.client.contentstackOptions);

            // Assert - header should still be present since no apiVersion was passed to constructor
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Handle_Null_Response_Gracefully()
        {
            // Arrange
            var model = new ContentModelling { Title = "Test" };
            var uid = _fixture.Create<string>();
            var apiVersion = "3.2";
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", model, uid, apiVersion);

            // Act & Assert - should not throw exception
            service.OnResponse(null, _stack.client.contentstackOptions);
            
            // Header should still be present since response was null
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Create_Content_Body_Correctly()
        {
            // Arrange
            var model = new ContentModelling { Title = "Test Global Field" };
            var uid = _fixture.Create<string>();
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", model, uid, null);

            // Act
            service.ContentBody();

            // Assert
            Assert.IsNotNull(service.ByteContent);
            var content = System.Text.Encoding.UTF8.GetString(service.ByteContent);
            Assert.IsTrue(content.Contains("Test Global Field"));
        }

        [TestMethod]
        public void Should_Handle_Null_Model_In_ContentBody()
        {
            // Arrange
            var uid = _fixture.Create<string>();
            var service = new GlobalFieldService(JsonSerializer.CreateDefault(), _stack, "/global_fields", null, uid, null);

            // Act
            service.ContentBody();

            // Assert
            Assert.IsNull(service.ByteContent);
        }
    }
} 