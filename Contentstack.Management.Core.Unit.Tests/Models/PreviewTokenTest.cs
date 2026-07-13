using System;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Token;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class PreviewTokenTest
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
        public void Initialize_PreviewToken()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            PreviewToken token = new PreviewToken(_stack, deliveryTokenUid);

            Assert.IsNull(token.Uid);
            Assert.AreEqual($"stacks/delivery_tokens/{deliveryTokenUid}/preview_token", token.resourcePath);
        }

        [TestMethod]
        public void Initialize_PreviewToken_ResourcePath_Is_Singular()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            PreviewToken token = new PreviewToken(_stack, deliveryTokenUid);

            // Endpoint is "preview_token" (singular), not "preview_tokens"
            StringAssert.EndsWith(token.resourcePath, "/preview_token");
        }

        [TestMethod]
        public void Should_Create_PreviewToken()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            ContentstackResponse response = _stack.PreviewToken(deliveryTokenUid).Create(_fixture.Create<PreviewTokenModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToString(), response.OpenJsonObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Create_PreviewToken_Async()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            ContentstackResponse response = await _stack.PreviewToken(deliveryTokenUid).CreateAsync(_fixture.Create<PreviewTokenModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToString(), response.OpenJsonObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_PreviewToken()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            ContentstackResponse response = _stack.PreviewToken(deliveryTokenUid).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToString(), response.OpenJsonObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Delete_PreviewToken_Async()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            ContentstackResponse response = await _stack.PreviewToken(deliveryTokenUid).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJsonObjectResponse().ToString(), response.OpenJsonObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Throw_On_Create_With_Null_Model()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            Assert.ThrowsException<ArgumentNullException>(() => _stack.PreviewToken(deliveryTokenUid).Create(null));
        }

        [TestMethod]
        public async Task Should_Throw_On_CreateAsync_With_Null_Model()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _stack.PreviewToken(deliveryTokenUid).CreateAsync(null));
        }

        [TestMethod]
        public void Should_Use_Stack_PreviewToken_Factory_Method()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            PreviewToken token = _stack.PreviewToken(deliveryTokenUid);

            Assert.IsNotNull(token);
            Assert.IsNull(token.Uid);
            Assert.AreEqual($"stacks/delivery_tokens/{deliveryTokenUid}/preview_token", token.resourcePath);
        }

        [TestMethod]
        public void Delete_Does_Not_Require_Uid()
        {
            // Delete uses the delivery token uid in the path only — no preview token uid needed.
            string deliveryTokenUid = _fixture.Create<string>();
            PreviewToken token = new PreviewToken(_stack, deliveryTokenUid);

            // Should NOT throw ArgumentException (unlike BaseModel.Delete which needs uid)
            ContentstackResponse response = token.Delete();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task DeleteAsync_Does_Not_Require_Uid()
        {
            string deliveryTokenUid = _fixture.Create<string>();
            PreviewToken token = new PreviewToken(_stack, deliveryTokenUid);

            ContentstackResponse response = await token.DeleteAsync();
            Assert.IsNotNull(response);
        }
    }
}
