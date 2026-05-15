using System;
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
    public class GlobalFieldFetchDeleteServiceTest
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
        public void Should_Create_GlobalFieldFetchDeleteService_Without_ApiVersion()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";

            // Act
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, null);

            // Assert
            Assert.IsNotNull(service);
            Assert.AreEqual(resourcePath, service.ResourcePath);
        }

        [TestMethod]
        public void Should_Create_GlobalFieldFetchDeleteService_With_ApiVersion()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "3.2";

            // Act
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);

            // Assert
            Assert.IsNotNull(service);
            Assert.AreEqual(resourcePath, service.ResourcePath);
        }

        [TestMethod]
        public void Should_Add_ApiVersion_Header_When_Provided()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "3.2";

            // Act
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);

            // Assert
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));
            Assert.AreEqual(apiVersion, service.Headers["api_version"]);
        }

        [TestMethod]
        public void Should_Not_Add_ApiVersion_Header_When_Not_Provided()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";

            // Act
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, null);

            // Assert
            Assert.IsFalse(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Remove_ApiVersion_Header_After_Successful_Response()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "3.2";
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);
            
            // Verify header is initially present
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));

            // Act - simulate successful response
            var mockResponse = new MockHttpResponse(200, "Success");
            service.OnResponse(mockResponse, _stack.client.contentstackOptions);

            // Assert
            Assert.IsFalse(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Remove_ApiVersion_Header_After_Successful_Delete_Response()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "3.2";
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);
            
            // Verify header is initially present
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));

            // Act - simulate successful delete response (204 No Content)
            var mockResponse = new MockHttpResponse(204, "No Content");
            service.OnResponse(mockResponse, _stack.client.contentstackOptions);

            // Assert
            Assert.IsFalse(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Not_Remove_ApiVersion_Header_After_Failed_Response()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "3.2";
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);
            
            // Verify header is initially present
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));

            // Act - simulate failed response
            var mockResponse = new MockHttpResponse(404, "Not Found");
            service.OnResponse(mockResponse, _stack.client.contentstackOptions);

            // Assert - header should still be present after failed response
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Not_Remove_ApiVersion_Header_When_No_ApiVersion_Was_Set()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, null);
            
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
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "3.2";
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);

            // Act & Assert - should not throw exception
            service.OnResponse(null, _stack.client.contentstackOptions);
            
            // Header should still be present since response was null
            Assert.IsTrue(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Handle_Empty_ApiVersion_String()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "";

            // Act
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);

            // Assert
            Assert.IsFalse(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Handle_Whitespace_ApiVersion_String()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "   ";

            // Act
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);

            // Assert
            Assert.IsFalse(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Handle_Different_Success_Status_Codes()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "3.2";
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);
            
            // Test with 201 Created
            var mockResponse201 = new MockHttpResponse(201, "Created");
            service.OnResponse(mockResponse201, _stack.client.contentstackOptions);
            Assert.IsFalse(service.Headers.ContainsKey("api_version"));

            // Reset for next test
            service.Headers["api_version"] = apiVersion;

            // Test with 202 Accepted
            var mockResponse202 = new MockHttpResponse(202, "Accepted");
            service.OnResponse(mockResponse202, _stack.client.contentstackOptions);
            Assert.IsFalse(service.Headers.ContainsKey("api_version"));
        }

        [TestMethod]
        public void Should_Not_Remove_ApiVersion_For_Client_Errors()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "3.2";
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);

            // Test various client error codes
            var errorCodes = new[] { 400, 401, 403, 404, 422 };

            foreach (var errorCode in errorCodes)
            {
                // Reset header
                service.Headers["api_version"] = apiVersion;
                
                // Act
                var mockResponse = new MockHttpResponse(errorCode, $"Error {errorCode}");
                service.OnResponse(mockResponse, _stack.client.contentstackOptions);

                // Assert
                Assert.IsTrue(service.Headers.ContainsKey("api_version"), $"Header should remain for status code {errorCode}");
            }
        }

        [TestMethod]
        public void Should_Not_Remove_ApiVersion_For_Server_Errors()
        {
            // Arrange
            var resourcePath = "/global_fields/test_uid";
            var apiVersion = "3.2";
            var service = new GlobalFieldFetchDeleteService(JsonSerializer.CreateDefault(), _stack, resourcePath, apiVersion);

            // Test various server error codes
            var errorCodes = new[] { 500, 502, 503, 504 };

            foreach (var errorCode in errorCodes)
            {
                // Reset header
                service.Headers["api_version"] = apiVersion;
                
                // Act
                var mockResponse = new MockHttpResponse(errorCode, $"Error {errorCode}");
                service.OnResponse(mockResponse, _stack.client.contentstackOptions);

                // Assert
                Assert.IsTrue(service.Headers.ContainsKey("api_version"), $"Header should remain for status code {errorCode}");
            }
        }
    }
} 