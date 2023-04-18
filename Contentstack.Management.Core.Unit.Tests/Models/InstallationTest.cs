using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Installation installaton = new Installation(client, orgUid);

            Assert.IsNotNull(installaton);
            Assert.IsNotNull(installaton.client);
            Assert.IsNull(installaton.uid);
            Assert.IsNull(installaton.appUid);
            Assert.Equals(orgUid, installaton.orgUid);
        }

        [TestMethod]
        void Initialize_Installation_with_AppUid()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            Installation installaton = new Installation(client, orgUid, appUid);

            Assert.IsNotNull(installaton);
            Assert.IsNotNull(installaton.client);
            Assert.IsNull(installaton.uid);
            Assert.Equals(orgUid, installaton.orgUid);
            Assert.Equals(appUid, installaton.appUid);
        }
        [TestMethod]
        void Initialize_Installation_with_Uid()
        {
            string orgUid = _fixture.Create<string>();
            string appUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            Installation installaton = new Installation(client, orgUid, appUid, uid);

            Assert.IsNotNull(installaton);
            Assert.IsNotNull(installaton.client);
            Assert.Equals(uid, installaton.uid);
            Assert.Equals(orgUid, installaton.orgUid);
            Assert.Equals(appUid, installaton.appUid);
        }
    }

}

