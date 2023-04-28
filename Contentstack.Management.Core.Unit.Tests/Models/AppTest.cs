using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class AppTest
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
        public void Initialize_App_without_Uid()
        {
            string orgUid = _fixture.Create<string>();
            App app = new App(client, orgUid);

            Assert.IsNotNull(app);
            Assert.IsNotNull(app.client);
            Assert.IsNull(app.uid);
            Assert.IsNull(app.resourcePathOAuth);
            Assert.AreEqual("/manifests", app.resourcePath);
            Assert.AreEqual(orgUid, app.orgUid);
            Assert.ThrowsException<InvalidOperationException>(() => app.Update(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.UpdateAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => app.Fetch(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.FetchAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => app.Delete(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.DeleteAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => app.UpdateOAuth(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.UpdateOAuthAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => app.FetchOAuth(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.FetchOAuthAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => app.Install(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.InstallAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => app.Reinstall(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.ReinstallAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => app.Authorize(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.AuthorizeAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => app.Installation());
            Assert.ThrowsException<InvalidOperationException>(() => app.Authorization());
            Assert.ThrowsException<InvalidOperationException>(() => app.Hosting());
        }

        [TestMethod]
        public void Initialize_App_with_Uid()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);

            Assert.IsNotNull(app);
            Assert.IsNotNull(app.client);
            Assert.AreEqual(uid, app.uid);
            Assert.AreEqual(orgUid, app.orgUid);
            Assert.AreEqual($"/manifests/{uid}", app.resourcePath);
            Assert.AreEqual($"/manifests/{uid}/oauth", app.resourcePathOAuth);
            Assert.ThrowsException<InvalidOperationException>(() => app.Create(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.CreateAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => app.FindAll(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => app.FindAllAsync(null));
        }

        [TestMethod]
        public void Should_Create_App()
        {
            string orgUid = _fixture.Create<string>();
            App app = new App(client, orgUid);
            ContentstackResponse response = app.Create(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            App app = new App(client, orgUid);
            ContentstackResponse response = await app.CreateAsync(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_App()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = app.Update(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = await app.UpdateAsync(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Fetch_App()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = app.Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = await app.FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Delete_App()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = app.Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = await app.DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Update_OAuth_App()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = app.UpdateOAuth(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_OAUTH_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = await app.UpdateOAuthAsync(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Fetch_OAuth_App()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = app.FetchOAuth();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_Auth_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = await app.FetchOAuthAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Install_App()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = app.Install(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Install_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = await app.InstallAsync(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Reinstall_App()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = app.Reinstall(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Reinstall_App_Async()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = await app.ReinstallAsync(_fixture.Create<JObject>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_App_Authorize()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = app.Authorize();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_App_Authorize_Async()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid, uid);
            ContentstackResponse response = await app.AuthorizeAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}

