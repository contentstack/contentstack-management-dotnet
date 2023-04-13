using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class AppTest
	{
        private ContentstackClient client;
        private readonly IFixture _fixture = new Fixture();

        [TestInitialize]
        public void initialize()
        {
            client = new ContentstackClient();
        }

        [TestMethod]
        void Initialize_App_without_Uid()
        {
            string orgUid = _fixture.Create<string>();
            App app = new App(client, orgUid);

            Assert.IsNotNull(app);
            Assert.IsNotNull(app.client);
            Assert.IsNull(app.uid);
            Assert.AreEqual(orgUid, app.orgUid);
        }

        [TestMethod]
        void Initialize_App_with_Uid()
        {
            string orgUid = _fixture.Create<string>();
            string uid = _fixture.Create<string>();
            App app = new App(client, orgUid);


            Assert.IsNotNull(app);
            Assert.IsNotNull(app.client);
            Assert.AreEqual(uid, app.uid);
            Assert.AreEqual(orgUid, app.orgUid);
        }
    }
}

