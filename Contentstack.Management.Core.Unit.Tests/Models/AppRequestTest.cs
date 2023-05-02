using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class AppRequestTest
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
        public void Initialize_App_Request_without_Org_Uid()
        {

            AppRequest appRequest = new AppRequest(client, null);

            Assert.IsNotNull(appRequest);
            Assert.IsNotNull(appRequest.client);
            Assert.AreEqual("/requests", appRequest.resourcePath);
            Assert.IsNull(appRequest.orgUid);
            Assert.ThrowsException<InvalidOperationException>(() => appRequest.Create(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => appRequest.CreateAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => appRequest.FindAll(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => appRequest.FindAllAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => appRequest.Delete(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => appRequest.DeleteAsync(null));
        }

        [TestMethod]
        public void Initialize_App_Request_with_org_uid()
        {
            string orgUid = _fixture.Create<string>();
            AppRequest appRequest = new AppRequest(client, orgUid);

            Assert.IsNotNull(appRequest);
            Assert.IsNotNull(appRequest.client);
            Assert.AreEqual("/requests", appRequest.resourcePath);
            Assert.AreEqual(orgUid, appRequest.orgUid);
        }
        [TestMethod]
        public void Should_Create_App()
        {
            string orgUid = _fixture.Create<string>();
            AppRequest appRequest = new AppRequest(client, orgUid);
            ContentstackResponse response = appRequest.Create(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            AppRequest appRequest = new AppRequest(client, orgUid);
            ContentstackResponse response = await appRequest.CreateAsync(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Delete_App()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            AppRequest appRequest = new AppRequest(client, orgUid);
            ContentstackResponse response = appRequest.Delete(uid);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            AppRequest appRequest = new AppRequest(client, orgUid);
            ContentstackResponse response = await appRequest.DeleteAsync(uid);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_FindAll_App()
        {
            string orgUid = _fixture.Create<string>();
            AppRequest appRequest = new AppRequest(client, orgUid);
            ContentstackResponse response = appRequest.FindAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_FindAll_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            AppRequest appRequest = new AppRequest(client, orgUid);
            ContentstackResponse response = await appRequest.FindAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}

