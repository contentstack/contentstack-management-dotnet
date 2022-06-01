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
    public class ReleaseItemTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;
        private string _releaseUID;
        [TestInitialize]
        public void initialize()
        {
            var client = new ContentstackClient();
            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(_contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
            _releaseUID = _fixture.Create<string>();
        }

        [TestMethod]
        public void Initialize_ReleaseItem()
        {
            ReleaseItem releaseItem = new ReleaseItem(_stack, _releaseUID);

            
            Asset.Equals($"/releases/{_releaseUID}/items", releaseItem.resourcePath);
        }

        [TestMethod]
        public void Initialize_ReleaseItem_Without_Uid()
        {
            
            ReleaseItem releaseItem = new ReleaseItem(_stack, null);

            Asset.Equals($"/releases/{_releaseUID}/items", releaseItem.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => releaseItem.Delete(_fixture.Create<List<ReleaseItemModel>>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => releaseItem.DeleteAsync(_fixture.Create<List<ReleaseItemModel>>()));
            Assert.ThrowsException<InvalidOperationException>(() => releaseItem.Create(_fixture.Create<ReleaseItemModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => releaseItem.CreateAsync(_fixture.Create<ReleaseItemModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => releaseItem.CreateMultiple(_fixture.Create<List<ReleaseItemModel>>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => releaseItem.CreateMultipleAsync(_fixture.Create< List<ReleaseItemModel>>()));
            Assert.ThrowsException<InvalidOperationException>(() => releaseItem.GetAll());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => releaseItem.GetAllAsync());
        }

        [TestMethod]
        public void Should_Create_ReleaseItem()
        {
            ContentstackResponse response = _stack.Release(_releaseUID).Item().Create(_fixture.Create<ReleaseItemModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_releaseUID).Item().CreateAsync(_fixture.Create<ReleaseItemModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Create_Multiple_Release_Item()
        {
            ContentstackResponse response = _stack.Release(_releaseUID).Item().CreateMultiple(_fixture.Create<List<ReleaseItemModel>>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Multiple_Release_Item_Async()
        {
            ContentstackResponse response = await _stack.Release(_releaseUID).Item().CreateMultipleAsync(_fixture.Create<List<ReleaseItemModel>>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_ReleaseItem()
        {
            ContentstackResponse response = _stack.Release(_releaseUID).Item().GetAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_releaseUID).Item().GetAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_ReleaseItem()
        {
            ContentstackResponse response = _stack.Release(_releaseUID).Item().Delete(_fixture.Create<List<ReleaseItemModel>>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_releaseUID).Item().DeleteAsync(_fixture.Create<List<ReleaseItemModel>>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
