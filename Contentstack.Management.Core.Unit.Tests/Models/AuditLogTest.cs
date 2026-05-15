using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    public class AuditLogTest
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
        public void Initialize_AuditLog()
        {
            AuditLog auditLog = new AuditLog(_stack, null);

            Assert.IsNull(auditLog.Uid);
            Assert.AreEqual($"/audit-logs", auditLog.resourcePath);

            Assert.ThrowsException<InvalidOperationException>(() => auditLog.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => auditLog.FetchAsync());
        }

        [TestMethod]
        public void Initialize_AuditLog_With_Uid()
        {
            string uid = _fixture.Create<string>();
            AuditLog auditLog = new AuditLog(_stack, uid);

            Assert.AreEqual(uid, auditLog.Uid);
            Assert.AreEqual($"/audit-logs/{auditLog.Uid}", auditLog.resourcePath);

            Assert.ThrowsException<InvalidOperationException>(() => auditLog.FindAll());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => auditLog.FindAllAsync());
        }

        [TestMethod]
        public void Should_Fetch_AuditLog()
        {
            ContentstackResponse response = _stack.AuditLog(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_AuditLog_Async()
        {
            ContentstackResponse response = await _stack.AuditLog(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Find_AuditLog()
        {
            ContentstackResponse response = _stack.AuditLog().FindAll();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_AuditLog_Async()
        {
            ContentstackResponse response = await _stack.AuditLog().FindAllAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

    }
}
