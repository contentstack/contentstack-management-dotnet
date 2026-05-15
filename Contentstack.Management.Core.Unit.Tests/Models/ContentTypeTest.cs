using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class ContentTypeTest
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
        public void Initialize_ContentType()
        {
            ContentType contentType = new ContentType(_stack, null);

            Assert.IsNull(contentType.Uid);
            Assert.AreEqual($"/content_types", contentType.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => contentType.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Update(new ContentModelling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => contentType.UpdateAsync(new ContentModelling()));
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => contentType.DeleteAsync());
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Entry());
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Entry(_fixture.Create<string>()));
            Assert.AreEqual(contentType.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_ContentType_With_Uid()
        {
            string uid = _fixture.Create<string>();
            ContentType contentType = new ContentType(_stack, uid);

            Assert.AreEqual(uid, contentType.Uid);
            Assert.AreEqual($"/content_types/{contentType.Uid}", contentType.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Create(new ContentModelling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => contentType.CreateAsync(new ContentModelling()));
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Query());
            Assert.AreEqual(contentType.Entry().GetType(), typeof(Entry));
        }

        [TestMethod]
        public void Should_Create_Content_Type()
        {
            ContentstackResponse response = _stack.ContentType().Create(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.ContentType().CreateAsync(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Content_Type()
        {
            ContentstackResponse response = _stack.ContentType().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.ContentType().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Content_Type()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Content_Type()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Update(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).UpdateAsync(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Content_Type()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Content_Type_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
