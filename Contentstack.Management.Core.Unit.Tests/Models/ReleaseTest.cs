using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class ReleaseTest
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
        public void Initialize_Release()
        {
            Release release = new Release(_stack);

            Assert.IsNull(release.Uid);
            Assert.AreEqual("/releases", release.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => release.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => release.Update(_fixture.Create<ReleaseModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.UpdateAsync(_fixture.Create<ReleaseModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => release.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.DeleteAsync());
            Assert.ThrowsException<InvalidOperationException>(() => release.Deploy(_fixture.Create<DeployModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.DeployAsync(_fixture.Create<DeployModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => release.Clone(_fixture.Create<string>(), _fixture.Create<string>())); 
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.CloneAsync(_fixture.Create<string>(), _fixture.Create<string>()));
            Assert.AreEqual(release.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Release_With_Uid()
        {
            string code = _fixture.Create<string>();
            Release release = new Release(_stack, code);

            Assert.AreEqual(code, release.Uid);
            Assert.AreEqual($"/releases/{release.Uid}", release.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => release.Create(_fixture.Create<ReleaseModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.CreateAsync(_fixture.Create<ReleaseModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => release.Query());
            Assert.AreEqual(release.Item().GetType(), typeof(ReleaseItem));
        }

        [TestMethod]
        public void Initialize_Release_Clone_NAME_Null()
        {
            string code = _fixture.Create<string>();
            Release release = new Release(_stack, code);
            Assert.ThrowsException<ArgumentNullException>(() => release.Clone(null, _fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => release.CloneAsync(_fixture.Create<string>(), _fixture.Create<string>()));
        }


        [TestMethod]
        public void Should_Create_Release()
        {
            ContentstackResponse response = _stack.Release().Create(_fixture.Create<ReleaseModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Release_Async()
        {
            ContentstackResponse response = await _stack.Release().CreateAsync(_fixture.Create<ReleaseModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Release()
        {
            ContentstackResponse response = _stack.Release().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Release_Async()
        {
            ContentstackResponse response = await _stack.Release().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Release()
        {
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Release()
        {
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Update(_fixture.Create<ReleaseModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).UpdateAsync(_fixture.Create<ReleaseModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Release()
        {
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Initialize_Release_Clone()
        {
            string code = _fixture.Create<string>();
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Clone(code, null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Initialize_Release_Clone_Async()
        {
            string code = _fixture.Create<string>();
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).CloneAsync(code, null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Deploy_Release()
        {
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Deploy(_fixture.Create<DeployModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Deploy_Release_Async()
        {
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).DeployAsync(_fixture.Create<DeployModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Create_Release_With_ParameterCollection()
        {
            var parameters = new ParameterCollection();
            parameters.Add("include_count", "true");
            ContentstackResponse response = _stack.Release().Create(_fixture.Create<ReleaseModel>(), parameters);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Release_With_ParameterCollection_Async()
        {
            var parameters = new ParameterCollection();
            parameters.Add("include_count", "true");
            ContentstackResponse response = await _stack.Release().CreateAsync(_fixture.Create<ReleaseModel>(), parameters);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Release_With_ParameterCollection()
        {
            var parameters = new ParameterCollection();
            parameters.Add("include_count", "true");
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Update(_fixture.Create<ReleaseModel>(), parameters);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Release_With_ParameterCollection_Async()
        {
            var parameters = new ParameterCollection();
            parameters.Add("include_count", "true");
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).UpdateAsync(_fixture.Create<ReleaseModel>(), parameters);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Throw_Exception_Deploy_Without_Uid()
        {
            Release release = new Release(_stack);
            Assert.ThrowsException<InvalidOperationException>(() => release.Deploy(_fixture.Create<DeployModel>()));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Throw_Exception_Deploy_Without_Uid_Async()
        {
            Release release = new Release(_stack);
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => release.DeployAsync(_fixture.Create<DeployModel>()));
        }

        [TestMethod]
        public void Should_Throw_Exception_Clone_Empty_Name()
        {
            Release release = new Release(_stack, _fixture.Create<string>());
            Assert.ThrowsException<ArgumentNullException>(() => release.Clone(string.Empty, _fixture.Create<string>()));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Throw_Exception_Clone_Empty_Name_Async()
        {
            Release release = new Release(_stack, _fixture.Create<string>());
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => release.CloneAsync(string.Empty, _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Accept_Clone_Whitespace_Name()
        {
            Release release = new Release(_stack, _fixture.Create<string>());
            string whitespaceDescription = _fixture.Create<string>();
            
            // Whitespace names are accepted by the Clone method (only null/empty are rejected)
            ContentstackResponse response = release.Clone("   ", whitespaceDescription);
            
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Accept_Clone_Whitespace_Name_Async()
        {
            Release release = new Release(_stack, _fixture.Create<string>());
            string whitespaceDescription = _fixture.Create<string>();
            
            // Whitespace names are accepted by the Clone method (only null/empty are rejected)
            ContentstackResponse response = await release.CloneAsync("   ", whitespaceDescription);
            
            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Clone_Release_With_Description()
        {
            string name = _fixture.Create<string>();
            string description = _fixture.Create<string>();
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Clone(name, description);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Clone_Release_With_Description_Async()
        {
            string name = _fixture.Create<string>();
            string description = _fixture.Create<string>();
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).CloneAsync(name, description);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Clone_Release_Without_Description()
        {
            string name = _fixture.Create<string>();
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Clone(name, null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Clone_Release_Without_Description_Async()
        {
            string name = _fixture.Create<string>();
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).CloneAsync(name, null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Create_Release_With_Null_ParameterCollection()
        {
            ContentstackResponse response = _stack.Release().Create(_fixture.Create<ReleaseModel>(), null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Release_With_Null_ParameterCollection_Async()
        {
            ContentstackResponse response = await _stack.Release().CreateAsync(_fixture.Create<ReleaseModel>(), null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Release_With_Null_ParameterCollection()
        {
            ContentstackResponse response = _stack.Release(_fixture.Create<string>()).Update(_fixture.Create<ReleaseModel>(), null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Release_With_Null_ParameterCollection_Async()
        {
            ContentstackResponse response = await _stack.Release(_fixture.Create<string>()).UpdateAsync(_fixture.Create<ReleaseModel>(), null);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_Without_Uid()
        {
            Release release = new Release(_stack);
            Assert.AreEqual("/releases", release.resourcePath);
        }

        [TestMethod]
        public void Should_Verify_Resource_Path_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Release release = new Release(_stack, uid);
            Assert.AreEqual($"/releases/{uid}", release.resourcePath);
        }

        [TestMethod]
        public void Should_Return_ReleaseItem_Instance()
        {
            string uid = _fixture.Create<string>();
            Release release = new Release(_stack, uid);
            ReleaseItem releaseItem = release.Item();
            
            Assert.IsNotNull(releaseItem);
            Assert.AreEqual(typeof(ReleaseItem), releaseItem.GetType());
        }

        [TestMethod]
        public void Should_Return_Query_Instance()
        {
            Release release = new Release(_stack);
            Query query = release.Query();
            
            Assert.IsNotNull(query);
            Assert.AreEqual(typeof(Query), query.GetType());
        }
    }
}
