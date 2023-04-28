using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class HostingTest
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
        public void Initialize_Hosting_with_App_Uid()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);

            Assert.IsNotNull(hosting);
            Assert.IsNotNull(hosting.client);
            Assert.AreEqual($"/manifests/{appUid}/hosting", hosting.resourcePath);
            Assert.AreEqual(orgUid, hosting.orgUid);
            Assert.AreEqual(appUid, hosting.appUid);
            Assert.IsNotNull(hosting.Deployment());
            Assert.IsNotNull(hosting.Deployment(appUid));
        }
        [TestMethod]
        public void Should_Hosting_IsEnable()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = hosting.IsEnable();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Hosting_IsEnable_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = await hosting.IsEnableAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Hosting_Enable()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = hosting.Enable();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Hosting_Enable_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = await hosting.EnableAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Hosting_Disable()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = hosting.Disable();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Hosting_Disable_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = await hosting.DisableAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Hosting_CreateUploadUrl()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = hosting.CreateUploadUrl();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Hosting_CreateUploadUrl_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = await hosting.CreateUploadUrlAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Hosting_LatestLiveDeployment()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = hosting.LatestLiveDeployment();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Hosting_LatestLiveDeployment_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Hosting hosting = new Hosting(client, orgUid, appUid);
            ContentstackResponse response = await hosting.LatestLiveDeploymentAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}

