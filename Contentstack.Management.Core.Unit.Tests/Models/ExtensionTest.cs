using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.CustomExtension;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class ExtensionTest
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
        public void Initialize_Extension()
        {
            Extension extension = new Extension(_stack, null);

            Assert.IsNull(extension.Uid);
            Assert.AreEqual($"/extensions", extension.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => extension.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => extension.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => extension.Update(_fixture.Create<ExtensionModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => extension.UpdateAsync(_fixture.Create<ExtensionModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => extension.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => extension.DeleteAsync());
            Assert.AreEqual(extension.Query().GetType(), typeof(Query));

        }

        [TestMethod]
        public void Initialize_Extension_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Extension extension = new Extension(_stack, uid);
            CustomFieldModel model = new CustomFieldModel("../../../../README.md", "application/text", _fixture.Create<string>(), _fixture.Create<string>());

            Assert.AreEqual(uid, extension.Uid);
            Assert.AreEqual($"/extensions/{extension.Uid}", extension.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => extension.Query());
            Assert.ThrowsException<InvalidOperationException>(() => extension.Create(_fixture.Create<ExtensionModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => extension.CreateAsync(_fixture.Create<ExtensionModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => extension.Upload(model));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => extension.UploadAsync(model));
        }

        [TestMethod]
        public void Should_Upload_Custom_field()
        {
            CustomFieldModel model = new CustomFieldModel("../../../../README.md", "application/text", _fixture.Create<string>(), _fixture.Create<string>());
            ContentstackResponse response = _stack.Extension().Upload(model);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Upload_Async_Custom_field()
        {
            CustomFieldModel model = new CustomFieldModel("../../../../README.md", "application/text", _fixture.Create<string>(), _fixture.Create<string>());
            ContentstackResponse response = await _stack.Extension().UploadAsync(model);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Extension()
        {
            ContentstackResponse response = _stack.Extension().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Extension_Async()
        {
            ContentstackResponse response = await _stack.Extension().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Create_Extension()
        {
            ContentstackResponse response = _stack.Extension().Create(_fixture.Create<ExtensionModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Extension_Async()
        {
            ContentstackResponse response = await _stack.Extension().CreateAsync(_fixture.Create<ExtensionModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Fetch_Extension()
        {
            ContentstackResponse response = _stack.Extension(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Extension_Async()
        {
            ContentstackResponse response = await _stack.Extension(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Extension()
        {
            ContentstackResponse response = _stack.Extension(_fixture.Create<string>()).Update(_fixture.Create<ExtensionModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Extension_Async()
        {
            ContentstackResponse response = await _stack.Extension(_fixture.Create<string>()).UpdateAsync(_fixture.Create<ExtensionModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Extension()
        {
            ContentstackResponse response = _stack.Extension(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Extension_Async()
        {
            ContentstackResponse response = await _stack.Extension(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
