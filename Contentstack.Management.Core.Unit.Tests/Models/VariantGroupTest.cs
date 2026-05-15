using System;
using System.Collections.Generic;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class VariantGroupTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;

        [TestInitialize]
        public void initialize()
        {
            var client = new ContentstackClient();
            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(_contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
        }

        [TestMethod]
        public void Initialize_VariantGroup()
        {
            VariantGroup variantGroup = new VariantGroup(_stack);

            Assert.IsNull(variantGroup.Uid);
            Assert.AreEqual("/variant_groups", variantGroup.resourcePath);

            // Find should work without UID - test by calling and verifying no exception
            try
            {
                variantGroup.Find();
                // If we get here, no exception was thrown
            }
            catch (Exception ex)
            {
                Assert.Fail($"Find() should not throw exception but threw: {ex.Message}");
            }

            try
            {
                var task = variantGroup.FindAsync();
                task.Wait();
                // If we get here, no exception was thrown
            }
            catch (Exception ex)
            {
                Assert.Fail($"FindAsync() should not throw exception but threw: {ex.Message}");
            }

            // Link/Unlink should throw when no UID
            Assert.ThrowsException<InvalidOperationException>(() =>
                variantGroup.LinkContentTypes(new List<string> { "ct_uid" })
            );
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                variantGroup.LinkContentTypesAsync(new List<string> { "ct_uid" })
            );
            Assert.ThrowsException<InvalidOperationException>(() =>
                variantGroup.UnlinkContentTypes(new List<string> { "ct_uid" })
            );
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                variantGroup.UnlinkContentTypesAsync(new List<string> { "ct_uid" })
            );
        }

        [TestMethod]
        public void Initialize_VariantGroup_With_Uid()
        {
            string uid = _fixture.Create<string>();
            VariantGroup variantGroup = new VariantGroup(_stack, uid);

            Assert.AreEqual(uid, variantGroup.Uid);
            Assert.AreEqual($"/variant_groups/{uid}", variantGroup.resourcePath);

            // Find should throw when UID is provided
            Assert.ThrowsException<InvalidOperationException>(() => variantGroup.Find());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => variantGroup.FindAsync());

            // Link/Unlink should work with UID - test by calling and verifying no exception
            try
            {
                variantGroup.LinkContentTypes(new List<string> { "ct_uid" });
                // If we get here, no exception was thrown
            }
            catch (Exception ex)
            {
                Assert.Fail(
                    $"LinkContentTypes() should not throw exception but threw: {ex.Message}"
                );
            }

            try
            {
                var task = variantGroup.LinkContentTypesAsync(new List<string> { "ct_uid" });
                task.Wait();
                // If we get here, no exception was thrown
            }
            catch (Exception ex)
            {
                Assert.Fail(
                    $"LinkContentTypesAsync() should not throw exception but threw: {ex.Message}"
                );
            }

            try
            {
                variantGroup.UnlinkContentTypes(new List<string> { "ct_uid" });
                // If we get here, no exception was thrown
            }
            catch (Exception ex)
            {
                Assert.Fail(
                    $"UnlinkContentTypes() should not throw exception but threw: {ex.Message}"
                );
            }

            try
            {
                var task = variantGroup.UnlinkContentTypesAsync(new List<string> { "ct_uid" });
                task.Wait();
                // If we get here, no exception was thrown
            }
            catch (Exception ex)
            {
                Assert.Fail(
                    $"UnlinkContentTypesAsync() should not throw exception but threw: {ex.Message}"
                );
            }
        }

        [TestMethod]
        public void Should_Find_VariantGroups()
        {
            ContentstackResponse response = _stack.VariantGroup().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_VariantGroups_Async()
        {
            ContentstackResponse response = await _stack.VariantGroup().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public void Should_Link_Content_Types()
        {
            string variantGroupUid = _fixture.Create<string>();
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };

            ContentstackResponse response = _stack
                .VariantGroup(variantGroupUid)
                .LinkContentTypes(contentTypeUids);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Link_Content_Types_Async()
        {
            string variantGroupUid = _fixture.Create<string>();
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };

            ContentstackResponse response = await _stack
                .VariantGroup(variantGroupUid)
                .LinkContentTypesAsync(contentTypeUids);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public void Should_Unlink_Content_Types()
        {
            string variantGroupUid = _fixture.Create<string>();
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };

            ContentstackResponse response = _stack
                .VariantGroup(variantGroupUid)
                .UnlinkContentTypes(contentTypeUids);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Unlink_Content_Types_Async()
        {
            string variantGroupUid = _fixture.Create<string>();
            List<string> contentTypeUids = new List<string> { "ct_uid_1", "ct_uid_2" };

            ContentstackResponse response = await _stack
                .VariantGroup(variantGroupUid)
                .UnlinkContentTypesAsync(contentTypeUids);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Stack_Is_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VariantGroup(null));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ContentTypeUids_Is_Null_For_Link()
        {
            string variantGroupUid = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                _stack.VariantGroup(variantGroupUid).LinkContentTypes(null)
            );
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _stack.VariantGroup(variantGroupUid).LinkContentTypesAsync(null)
            );
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ContentTypeUids_Is_Null_For_Unlink()
        {
            string variantGroupUid = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                _stack.VariantGroup(variantGroupUid).UnlinkContentTypes(null)
            );
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _stack.VariantGroup(variantGroupUid).UnlinkContentTypesAsync(null)
            );
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ContentTypeUids_Is_Empty_For_Link()
        {
            string variantGroupUid = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                _stack.VariantGroup(variantGroupUid).LinkContentTypes(new List<string>())
            );
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _stack.VariantGroup(variantGroupUid).LinkContentTypesAsync(new List<string>())
            );
        }

        [TestMethod]
        public void Should_Throw_Exception_When_ContentTypeUids_Is_Empty_For_Unlink()
        {
            string variantGroupUid = _fixture.Create<string>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                _stack.VariantGroup(variantGroupUid).UnlinkContentTypes(new List<string>())
            );
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _stack.VariantGroup(variantGroupUid).UnlinkContentTypesAsync(new List<string>())
            );
        }

        [TestMethod]
        public void Should_Work_With_Query_Parameters()
        {
            var parameters = new ParameterCollection();
            parameters.Add("limit", "10");
            parameters.Add("skip", "0");

            ContentstackResponse response = _stack.VariantGroup().Find(parameters);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Work_With_Query_Parameters_Async()
        {
            var parameters = new ParameterCollection();
            parameters.Add("limit", "10");
            parameters.Add("skip", "0");

            ContentstackResponse response = await _stack.VariantGroup().FindAsync(parameters);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public void Should_Work_With_Link_Query_Parameters()
        {
            string variantGroupUid = _fixture.Create<string>();
            List<string> contentTypeUids = new List<string> { "ct_uid_1" };
            var parameters = new ParameterCollection();
            parameters.Add("include_count", "true");

            ContentstackResponse response = _stack
                .VariantGroup(variantGroupUid)
                .LinkContentTypes(contentTypeUids, parameters);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Work_With_Unlink_Query_Parameters_Async()
        {
            string variantGroupUid = _fixture.Create<string>();
            List<string> contentTypeUids = new List<string> { "ct_uid_1" };
            var parameters = new ParameterCollection();
            parameters.Add("include_count", "true");

            ContentstackResponse response = await _stack
                .VariantGroup(variantGroupUid)
                .UnlinkContentTypesAsync(contentTypeUids, parameters);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public void Should_Use_Correct_Resource_Path_For_Link_Content_Types()
        {
            string variantGroupUid = _fixture.Create<string>();
            List<string> contentTypeUids = new List<string> { "ct_uid_1" };

            // Create a mock handler that captures the request
            var mockHandler = new MockHttpHandler(_contentstackResponse);
            _stack.client.ContentstackPipeline.ReplaceHandler(mockHandler);

            _stack.VariantGroup(variantGroupUid).LinkContentTypes(contentTypeUids);

            // Verify the request was made to the correct endpoint
            Assert.IsTrue(
                mockHandler.LastRequestUri.AbsolutePath.EndsWith(
                    $"/variant_groups/{variantGroupUid}/variants"
                )
            );
        }

        [TestMethod]
        public void Should_Use_Correct_Resource_Path_For_Unlink_Content_Types()
        {
            string variantGroupUid = _fixture.Create<string>();
            List<string> contentTypeUids = new List<string> { "ct_uid_1" };

            // Create a mock handler that captures the request
            var mockHandler = new MockHttpHandler(_contentstackResponse);
            _stack.client.ContentstackPipeline.ReplaceHandler(mockHandler);

            _stack.VariantGroup(variantGroupUid).UnlinkContentTypes(contentTypeUids);

            // Verify the request was made to the correct endpoint
            Assert.IsTrue(
                mockHandler.LastRequestUri.AbsolutePath.EndsWith(
                    $"/variant_groups/{variantGroupUid}/variants"
                )
            );
        }

        [TestMethod]
        public void Should_Use_Correct_Resource_Path_For_Find_Variant_Groups()
        {
            // Create a mock handler that captures the request
            var mockHandler = new MockHttpHandler(_contentstackResponse);
            _stack.client.ContentstackPipeline.ReplaceHandler(mockHandler);

            _stack.VariantGroup().Find();

            // Verify the request was made to the correct endpoint
            Assert.IsTrue(mockHandler.LastRequestUri.AbsolutePath.EndsWith("/variant_groups"));
        }
    }
}
