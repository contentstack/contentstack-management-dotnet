using System;

using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class AuthorizationTest
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
        public void Initialize_Authorization_without_Authorization_Uid()
        {
            string orgUid = _fixture.Create<string>();
            Authorization authorization = new Authorization(client, orgUid, null);

            Assert.IsNotNull(authorization);
            Assert.IsNotNull(authorization.client);
            Assert.IsNull(authorization.appUid);
            Assert.IsNull(authorization.resourcePath);
            Assert.AreEqual(orgUid, authorization.orgUid);
            Assert.ThrowsException<InvalidOperationException>(() => authorization.FindAll(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => authorization.FindAllAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => authorization.Revoke(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => authorization.RevokeAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => authorization.RevokeAll(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => authorization.RevokeAllAsync(null));
        }
        [TestMethod]
        public void Initialize_Authorization_with_Authorization_Uid()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Authorization authorization = new Authorization(client, orgUid, appUid);

            Assert.IsNotNull(authorization);
            Assert.IsNotNull(authorization.client);
            Assert.AreEqual(orgUid, authorization.orgUid);
            Assert.AreEqual(appUid, authorization.appUid);
            Assert.AreEqual($"/manifests/{appUid}/authorizations", authorization.resourcePath);
        }
        [TestMethod]
        public void Should_Get_All_Authorization()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Authorization authorization = new Authorization(client, orgUid, appUid);
            ContentstackResponse response = authorization.FindAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Get_All_Authorization_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Authorization authorization = new Authorization(client, orgUid, appUid);
            ContentstackResponse response = await authorization.FindAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Revoke_All_Authorization()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Authorization authorization = new Authorization(client, orgUid, appUid);
            ContentstackResponse response = authorization.RevokeAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Revoke_All_Authorization_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Authorization authorization = new Authorization(client, orgUid, appUid);
            ContentstackResponse response = await authorization.RevokeAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Revoke_Authorization()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Authorization authorization = new Authorization(client, orgUid, appUid);
            ContentstackResponse response = authorization.Revoke(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Revoke_Authorization_Async()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Authorization authorization = new Authorization(client, orgUid, appUid);
            ContentstackResponse response = await authorization.RevokeAsync(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Revoke_without_Authorization_uid()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Authorization authorization = new Authorization(client, orgUid, appUid);

            Assert.ThrowsException<InvalidOperationException>(() => authorization.Revoke(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => authorization.RevokeAsync(null));
        }   
    }
}

