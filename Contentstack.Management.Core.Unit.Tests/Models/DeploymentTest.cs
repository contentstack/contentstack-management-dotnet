using System;
using System.Security.Cryptography;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class DeploymentTest
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
        public void Initialize_Deployment_without_Uid()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Deployment deployment = new Deployment(client, orgUid, appUid);

            Assert.IsNotNull(deployment);
            Assert.IsNotNull(deployment.client);
            Assert.IsNull(deployment.uid);
            Assert.AreEqual($"/manifests/{appUid}/hosting/deployments", deployment.resourcePath);
            Assert.AreEqual(orgUid, deployment.orgUid);
            Assert.AreEqual(appUid, deployment.appUid);
            Assert.ThrowsException<InvalidOperationException>(() => deployment.Fetch(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => deployment.FetchAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => deployment.Logs(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => deployment.LogsAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => deployment.SignedDownloadUrl(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => deployment.SignedDownloadUrlAsync(null));
        }
        [TestMethod]
        public void Initialize_Deployment_with_Uid()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Deployment deployment = new Deployment(client, orgUid, appUid, uid);

            Assert.IsNotNull(deployment);
            Assert.IsNotNull(deployment.client);
            Assert.AreEqual($"/manifests/{appUid}/hosting/deployments/{uid}", deployment.resourcePath);
            Assert.AreEqual(orgUid, deployment.orgUid);
            Assert.AreEqual(appUid, deployment.appUid);
            Assert.AreEqual(uid, deployment.uid);
            Assert.ThrowsException<InvalidOperationException>(() => deployment.Create(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => deployment.CreateAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => deployment.FindAll(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => deployment.FindAllAsync(null));
        }
        [TestMethod]
        public void Should_Create_Deployment()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            
            Deployment deployment = new Deployment(client, orgUid, appUid);
            ContentstackResponse response = deployment.Create(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Deployment_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            
            Deployment deployment = new Deployment(client, orgUid, appUid);
            ContentstackResponse response = await deployment.CreateAsync(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_FindAll_Deployment()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            
            Deployment deployment = new Deployment(client, orgUid, appUid);
            ContentstackResponse response = deployment.FindAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_FindAll_Deployment_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            
            Deployment deployment = new Deployment(client, orgUid, appUid);
            ContentstackResponse response = await deployment.FindAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Fetch_Deployment()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Deployment deployment = new Deployment(client, orgUid, appUid, uid);
            ContentstackResponse response = deployment.Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_Deployment_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Deployment deployment = new Deployment(client, orgUid, appUid, uid);
            ContentstackResponse response = await deployment.FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Logs_Deployment()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Deployment deployment = new Deployment(client, orgUid, appUid, uid);
            ContentstackResponse response = deployment.Logs();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Logs_Deployment_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Deployment deployment = new Deployment(client, orgUid, appUid, uid);
            ContentstackResponse response = await deployment.LogsAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_SignedDownloadUrl_Deployment()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Deployment deployment = new Deployment(client, orgUid, appUid, uid);
            ContentstackResponse response = deployment.SignedDownloadUrl();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_SignedDownloadUrl_Deployment_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Deployment deployment = new Deployment(client, orgUid, appUid, uid);
            ContentstackResponse response = await deployment.SignedDownloadUrlAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}

