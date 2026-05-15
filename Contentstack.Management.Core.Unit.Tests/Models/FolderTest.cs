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
    public class FolderTest
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
        public void Initialize_Folder()
        {
            Folder folder = new Folder(_stack, null);

            Assert.IsNull(folder.Uid);
            Assert.AreEqual($"/assets/folders", folder.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => folder.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => folder.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => folder.Update(_fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => folder.UpdateAsync(_fixture.Create<string>()));
            Assert.ThrowsException<InvalidOperationException>(() => folder.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => folder.DeleteAsync());
        }

        [TestMethod]
        public void Initialize_Folder_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Folder folder = new Folder(_stack, uid);

            Assert.AreEqual(uid, folder.Uid);
            Assert.AreEqual($"/assets/folders/{folder.Uid}", folder.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => folder.Create(_fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => folder.CreateAsync(_fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Create_Folder()
        {
            ContentstackResponse response = _stack.Asset().Folder().Create(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Folder_Async()
        {
            ContentstackResponse response = await _stack.Asset().Folder().CreateAsync(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Folder()
        {
            ContentstackResponse response = _stack.Asset().Folder(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Folder_Async()
        {
            ContentstackResponse response = await _stack.Asset().Folder(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Folder()
        {

            ContentstackResponse response = _stack.Asset().Folder(_fixture.Create<string>()).Update(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Folder_Async()
        {
            ContentstackResponse response = await _stack.Asset().Folder(_fixture.Create<string>()).UpdateAsync(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Folder()
        {
            ContentstackResponse response = _stack.Asset().Folder(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Folder_Async()
        {
            ContentstackResponse response = await _stack.Asset().Folder(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
