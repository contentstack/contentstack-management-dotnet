using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Models
{

    [TestClass]
    public class InstallationTest
    {
        private ContentstackClient client;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;

        [TestInitialize]
        public void initialize()
        {
            client = new ContentstackClient();
            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(_contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
        }

        [TestMethod]
        void Initialize_Installation_without_Uid()
        {
            string orgUid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid);

            Assert.IsNotNull(installation);
            Assert.IsNotNull(installation.client);
            Assert.IsNull(installation.uid);
            Assert.IsNull(installation.appUid);
            Assert.Equals(orgUid, installation.orgUid);
            Assert.ThrowsException<InvalidOperationException>(() => installation.FindAll(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => installation.FindAllAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => installation.AppInstallations(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => installation.AppInstallationsAsync(null));
        }

        [TestMethod]
        void Initialize_Installation_with_InstallationUid()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid);

            Assert.IsNotNull(installation);
            Assert.IsNotNull(installation.client);
            Assert.IsNull(installation.uid);
            Assert.Equals(orgUid, installation.orgUid);
            Assert.Equals(appUid, installation.appUid);
            Assert.ThrowsException<InvalidOperationException>(() => installation.Update(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => installation.UpdateAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => installation.Fetch(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => installation.FetchAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => installation.Delete(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => installation.DeleteAsync(null));
        }
        [TestMethod]
        void Initialize_Installation_with_Uid()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid, uid);

            Assert.IsNotNull(installation);
            Assert.IsNotNull(installation.client);
            Assert.Equals(uid, installation.uid);
            Assert.Equals(orgUid, installation.orgUid);
            Assert.Equals(appUid, installation.appUid);
        }
        [TestMethod]
        public void Should_Update_Installation()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid, uid);
            ContentstackResponse response = installation.Update(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Installation_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid, uid);
            ContentstackResponse response = await installation.UpdateAsync(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Fetch_Installation()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid, uid);
            ContentstackResponse response = installation.Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_Installation_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid, uid);
            ContentstackResponse response = await installation.FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Delete_Installation()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid, uid);
            ContentstackResponse response = installation.Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Installation_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid, uid);
            ContentstackResponse response = await installation.DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_FindAll_Installation()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid);
            ContentstackResponse response = installation.FindAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_FindAll_Installation_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid);
            ContentstackResponse response = await installation.FindAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Fetch_App_Installation()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid, uid);
            ContentstackResponse response = installation.AppInstallations();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_app_Installation_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installation = new Installation(client, orgUid, appUid, uid);
            ContentstackResponse response = await installation.AppInstallationsAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }

}

