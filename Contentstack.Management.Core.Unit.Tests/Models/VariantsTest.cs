using System;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class VariantsTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;
        private VariantsModel _variantsModel = new VariantsModel();

        [TestInitialize]
        public void Initialize()
        {
            var client = new ContentstackClient();
            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(_contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
        }

        #region Initialize Tests

        [TestMethod]
        public void Initialize_Variants_Without_Uid()
        {
            Variants variants = new Variants(_stack, null);

            Assert.IsNull(variants.Uid);
            Assert.AreEqual("/variants", variants.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => variants.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => variants.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => variants.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => variants.DeleteAsync());
        }

        [TestMethod]
        public void Initialize_Variants_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Variants variants = new Variants(_stack, uid);

            Assert.AreEqual(uid, variants.Uid);
            Assert.AreEqual($"/variants/{uid}", variants.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => variants.Create(_variantsModel));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => variants.CreateAsync(_variantsModel));
            Assert.ThrowsException<InvalidOperationException>(() => variants.FetchByUid(new string[] { "uid1", "uid2" }));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => variants.FetchByUidAsync(new string[] { "uid1", "uid2" }));
        }

        #endregion

        #region Create Tests

        [TestMethod]
        public void Should_Create_Variants()
        {
            ContentstackResponse response = _stack.Variants().Create(_variantsModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Create_Variants_Async()
        {
            ContentstackResponse response = await _stack.Variants().CreateAsync(_variantsModel);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        #endregion

        #region Fetch Tests

        [TestMethod]
        public void Should_Fetch_Variants()
        {
            ContentstackResponse response = _stack.Variants(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Fetch_Variants_Async()
        {
            ContentstackResponse response = await _stack.Variants(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Variants_With_Parameters()
        {
            ParameterCollection collection = new ParameterCollection();
            collection.Add("include_count", true);

            ContentstackResponse response = _stack.Variants(_fixture.Create<string>()).Fetch(collection);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Fetch_Variants_With_Parameters_Async()
        {
            ParameterCollection collection = new ParameterCollection();
            collection.Add("include_count", true);

            ContentstackResponse response = await _stack.Variants(_fixture.Create<string>()).FetchAsync(collection);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        #endregion

        #region FetchByUid Tests

        [TestMethod]
        public void Should_FetchByUid_Variants()
        {
            string[] uids = { _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>() };
            ContentstackResponse response = _stack.Variants().FetchByUid(uids);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_FetchByUid_Variants_Async()
        {
            string[] uids = { _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>() };
            ContentstackResponse response = await _stack.Variants().FetchByUidAsync(uids);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Throw_Exception_When_FetchByUid_Called_With_Null_Uids()
        {
            Assert.ThrowsException<ArgumentException>(() => _stack.Variants().FetchByUid(null));
        }

        [TestMethod]
        public async Task Should_Throw_Exception_When_FetchByUidAsync_Called_With_Null_Uids()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _stack.Variants().FetchByUidAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_FetchByUid_Called_With_Empty_Uids()
        {
            string[] emptyUids = new string[0];
            Assert.ThrowsException<ArgumentException>(() => _stack.Variants().FetchByUid(emptyUids));
        }

        [TestMethod]
        public async Task Should_Throw_Exception_When_FetchByUidAsync_Called_With_Empty_Uids()
        {
            string[] emptyUids = new string[0];
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _stack.Variants().FetchByUidAsync(emptyUids));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_FetchByUid_Called_On_Instance_With_Uid()
        {
            string[] uids = { _fixture.Create<string>(), _fixture.Create<string>() };
            Assert.ThrowsException<InvalidOperationException>(() => _stack.Variants(_fixture.Create<string>()).FetchByUid(uids));
        }

        [TestMethod]
        public async Task Should_Throw_Exception_When_FetchByUidAsync_Called_On_Instance_With_Uid()
        {
            string[] uids = { _fixture.Create<string>(), _fixture.Create<string>() };
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _stack.Variants(_fixture.Create<string>()).FetchByUidAsync(uids));
        }

        [TestMethod]
        public void Should_FetchByUid_Single_Uid()
        {
            string[] uids = { _fixture.Create<string>() };
            ContentstackResponse response = _stack.Variants().FetchByUid(uids);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_FetchByUid_Single_Uid_Async()
        {
            string[] uids = { _fixture.Create<string>() };
            ContentstackResponse response = await _stack.Variants().FetchByUidAsync(uids);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        public void Should_Delete_Variants()
        {
            ContentstackResponse response = _stack.Variants(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async Task Should_Delete_Variants_Async()
        {
            ContentstackResponse response = await _stack.Variants(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void Should_Throw_Exception_When_APIKey_Is_Null()
        {
            var client = new ContentstackClient();
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            var stackWithNullAPIKey = new Stack(client, null);

            Assert.ThrowsException<InvalidOperationException>(() => new Variants(stackWithNullAPIKey));
        }

        [TestMethod]
        public void Should_Throw_Exception_When_APIKey_Is_Empty()
        {
            var client = new ContentstackClient();
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            var stackWithEmptyAPIKey = new Stack(client, "");

            Assert.ThrowsException<InvalidOperationException>(() => new Variants(stackWithEmptyAPIKey));
        }

        #endregion
    }
}
