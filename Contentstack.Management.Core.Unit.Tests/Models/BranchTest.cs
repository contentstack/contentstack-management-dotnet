using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class BranchTest
    {
        private Stack _stack;
        private MockHttpHandler _mockHandler;
        private readonly IFixture _fixture = new Fixture();

        [TestInitialize]
        public void initialize()
        {
            var client = new ContentstackClient();
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            _mockHandler = new MockHttpHandler(contentstackResponse);
            client.ContentstackPipeline.ReplaceHandler(_mockHandler);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
        }

        [TestMethod]
        public void Initialize_Branch()
        {
            Branch branch = new Branch(_stack);

            Assert.IsNull(branch.Uid);
            Assert.AreEqual("/stacks/branches", branch.resourcePath);
            Assert.ThrowsException<ArgumentException>(() => branch.Fetch());
            Assert.ThrowsExceptionAsync<ArgumentException>(() => branch.FetchAsync());
            Assert.ThrowsException<ArgumentException>(() => branch.Delete());
            Assert.ThrowsExceptionAsync<ArgumentException>(() => branch.DeleteAsync());
            Assert.AreEqual(branch.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Branch_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Branch branch = new Branch(_stack, uid);

            Assert.AreEqual(uid, branch.Uid);
            Assert.AreEqual($"/stacks/branches/{branch.Uid}", branch.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => branch.Create(_fixture.Create<BranchModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => branch.CreateAsync(_fixture.Create<BranchModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => branch.Query());
        }

        [TestMethod]
        public void Should_Query_Branches_With_Params()
        {
            _stack.Branch().Query().Limit(2).Skip(2).IncludeCount().Find();

            Assert.IsNotNull(_mockHandler.LastRequestUri);
            StringAssert.Contains(_mockHandler.LastRequestUri.AbsoluteUri, "/v3/stacks/branches");
            StringAssert.Contains(_mockHandler.LastRequestUri.Query, "include_count=true");
            StringAssert.Contains(_mockHandler.LastRequestUri.Query, "limit=2");
            StringAssert.Contains(_mockHandler.LastRequestUri.Query, "skip=2");
        }
    }
}

