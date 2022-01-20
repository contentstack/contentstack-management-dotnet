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
            Assert.ThrowsException<InvalidOperationException>(() => new BaseModel<ContentModelling>(new Stack(null), _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_on_FieldName_Empty()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BaseModel<ContentModelling>(_stack, null));
        }

        [TestMethod]
        public void Initialize_ContentType()
        {
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>());

            Assert.IsNull(baseModel.Uid);
            Assert.ThrowsException<InvalidOperationException>(() => baseModel.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => baseModel.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => baseModel.Update(new ContentModelling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => baseModel.UpdateAsync(new ContentModelling()));
            Assert.ThrowsException<InvalidOperationException>(() => baseModel.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => baseModel.DeleteAsync());

        }
        [TestMethod]
        public void Initialize_BaseModel_With_Uid()
        {
            string uid = _fixture.Create<string>();
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>(), uid);

            Assert.AreEqual(uid, baseModel.Uid);
            Assert.ThrowsException<InvalidOperationException>(() => baseModel.Create(new ContentModelling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => baseModel.CreateAsync(new ContentModelling()));
        }

        [TestMethod]
        public void Should_Return_Mock_Response_On_Create()
        {
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = baseModel.Create(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Mock_Response_On_CreateAsync()
        {
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = await baseModel.CreateAsync(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Return_Mock_Response_On_Update()
        {
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = baseModel.Update(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Mock_Response_On_UpdateAsync()
        {
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = await baseModel.UpdateAsync(new ContentModelling());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Return_Mock_Response_On_Fetch()
        {
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = baseModel.Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Mock_Response_On_FetchAsync()
        {
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = await baseModel.FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Return_Mock_Response_On_Delete()
        {
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = baseModel.Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Mock_Response_On_DeleteAsync()
        {
            BaseModel<ContentModelling> baseModel = new BaseModel<ContentModelling>(_stack, _fixture.Create<string>(), _fixture.Create<string>());
            baseModel.resourcePath = "/Path";

            ContentstackResponse response = await baseModel.DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
