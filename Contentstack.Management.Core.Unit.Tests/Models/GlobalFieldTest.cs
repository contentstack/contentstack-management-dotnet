using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{

    [TestClass]
    public class GlobalFieldTest
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
        public void Initialize_GlobalField()
        {
            GlobalField globalField = new GlobalField(_stack);

            Assert.IsNull(globalField.Uid);
            Assert.AreEqual($"/global_fields", globalField.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => globalField.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Update(new ContentModelling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => globalField.UpdateAsync(new ContentModelling()));
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => globalField.DeleteAsync());
            Assert.AreEqual(globalField.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_GlobalField_With_ApiVersion()
        {
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, apiVersion: apiVersion);

            Assert.IsNull(globalField.Uid);
            Assert.AreEqual($"/global_fields", globalField.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => globalField.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Update(new ContentModelling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => globalField.UpdateAsync(new ContentModelling()));
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => globalField.DeleteAsync());
            Assert.AreEqual(globalField.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_GlobalField_With_Uid()
        {
            string uid = _fixture.Create<string>();
            GlobalField globalField = new GlobalField(_stack, uid);

            Assert.AreEqual(uid, globalField.Uid);
            Assert.AreEqual($"/global_fields/{globalField.Uid}", globalField.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Create(new ContentModelling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => globalField.CreateAsync(new ContentModelling()));
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Query());
        }

        [TestMethod]
        public void Initialize_GlobalField_With_Uid_And_ApiVersion()
        {
            string uid = _fixture.Create<string>();
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, uid, apiVersion);

            Assert.AreEqual(uid, globalField.Uid);
            Assert.AreEqual($"/global_fields/{globalField.Uid}", globalField.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Create(new ContentModelling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => globalField.CreateAsync(new ContentModelling()));
            Assert.ThrowsException<InvalidOperationException>(() => globalField.Query());
        }
        [TestMethod]
        public void Should_Create_Content_Type()
        {
            ContentstackResponse response = _stack.GlobalField().Create(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.GlobalField().CreateAsync(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Content_Type()
        {
            ContentstackResponse response = _stack.GlobalField().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Content_Type_With_ApiVersion()
        {
            ContentstackResponse response = _stack.GlobalField(apiVersion: "3.2").Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.GlobalField().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Content_Type_Async_With_ApiVersion()
        {
            ContentstackResponse response = await _stack.GlobalField(apiVersion: "3.2").Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Content_Type()
        {
            ContentstackResponse response = _stack.GlobalField(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.GlobalField(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Content_Type()
        {
            ContentstackResponse response = _stack.GlobalField(_fixture.Create<string>()).Update(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.GlobalField(_fixture.Create<string>()).UpdateAsync(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Content_Type()
        {
            ContentstackResponse response = _stack.GlobalField(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.GlobalField(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Use_Specialized_Service_For_Create_With_ApiVersion()
        {
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, apiVersion: apiVersion);

            // This should use the specialized GlobalFieldService
            ContentstackResponse response = globalField.Create(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Use_Specialized_Service_For_Create_Async_With_ApiVersion()
        {
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, apiVersion: apiVersion);

            // This should use the specialized GlobalFieldService
            ContentstackResponse response = await globalField.CreateAsync(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Use_Specialized_Service_For_Update_With_ApiVersion()
        {
            string uid = _fixture.Create<string>();
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, uid, apiVersion);

            // This should use the specialized GlobalFieldService
            ContentstackResponse response = globalField.Update(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Use_Specialized_Service_For_Update_Async_With_ApiVersion()
        {
            string uid = _fixture.Create<string>();
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, uid, apiVersion);

            // This should use the specialized GlobalFieldService
            ContentstackResponse response = await globalField.UpdateAsync(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Use_Specialized_Service_For_Fetch_With_ApiVersion()
        {
            string uid = _fixture.Create<string>();
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, uid, apiVersion);

            // This should use the specialized GlobalFieldFetchDeleteService
            ContentstackResponse response = globalField.Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Use_Specialized_Service_For_Fetch_Async_With_ApiVersion()
        {
            string uid = _fixture.Create<string>();
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, uid, apiVersion);

            // This should use the specialized GlobalFieldFetchDeleteService
            ContentstackResponse response = await globalField.FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Use_Specialized_Service_For_Delete_With_ApiVersion()
        {
            string uid = _fixture.Create<string>();
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, uid, apiVersion);

            // This should use the specialized GlobalFieldFetchDeleteService
            ContentstackResponse response = globalField.Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Use_Specialized_Service_For_Delete_Async_With_ApiVersion()
        {
            string uid = _fixture.Create<string>();
            string apiVersion = "3.2";
            GlobalField globalField = new GlobalField(_stack, uid, apiVersion);

            // This should use the specialized GlobalFieldFetchDeleteService
            ContentstackResponse response = await globalField.DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Handle_Nested_GlobalField_Operations_Without_ApiVersion()
        {
            // Test that operations work normally without apiVersion
            GlobalField globalField = new GlobalField(_stack);

            ContentstackResponse response = globalField.Create(new ContentModelling());
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Handle_Nested_GlobalField_Operations_With_Empty_ApiVersion()
        {
            // Test that operations work normally with empty apiVersion
            GlobalField globalField = new GlobalField(_stack, apiVersion: "");

            ContentstackResponse response = globalField.Create(new ContentModelling());
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
