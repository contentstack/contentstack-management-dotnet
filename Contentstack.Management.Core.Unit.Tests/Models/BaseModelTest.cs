using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class BaseModelTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;
        [TestInitialize]
        public void initialize()
        {
            ContentstackClient client = new ContentstackClient();

            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(_contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            _stack = new Stack(client, _fixture.Create<string>());
        }

        [TestMethod]
        public void Should_Throw_on_API_Key_Empty()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new BaseModel<ContentModeling>(new Stack(null), _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_on_FieldName_Empty()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BaseModel<ContentModeling>(_stack, null));
        }

        [TestMethod]
        public void Initialize_ContentType()
        {
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>());

            Assert.IsNull(baseModel.Uid);
            Assert.ThrowsException<InvalidOperationException>(() => baseModel.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => baseModel.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => baseModel.Update(new ContentModeling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => baseModel.UpdateAsync(new ContentModeling()));
            Assert.ThrowsException<InvalidOperationException>(() => baseModel.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => baseModel.DeleteAsync());

        }
        [TestMethod]
        public void Initialize_BaseModel_With_Uid()
        {
            string uid = _fixture.Create<string>();
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>(), uid);

            Assert.AreEqual(uid, baseModel.Uid);
            Assert.ThrowsException<InvalidOperationException>(() => baseModel.Create(new ContentModeling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => baseModel.CreateAsync(new ContentModeling()));
        }

        [TestMethod]
        public void Should_Return_Mock_Response_On_Create()
        {
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = baseModel.Create(new ContentModeling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Mock_Response_On_CreateAsync()
        {
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = await baseModel.CreateAsync(new ContentModeling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Return_Mock_Response_On_Update()
        {
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = baseModel.Update(new ContentModeling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Mock_Response_On_UpdateAsync()
        {
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = await baseModel.UpdateAsync(new ContentModeling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Return_Mock_Response_On_Fetch()
        {
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = baseModel.Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Mock_Response_On_FetchAsync()
        {
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = await baseModel.FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Return_Mock_Response_On_Delete()
        {
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = baseModel.Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Mock_Response_On_DeleteAsync()
        {
            BaseModel<ContentModeling> baseModel = new BaseModel<ContentModeling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = await baseModel.DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
