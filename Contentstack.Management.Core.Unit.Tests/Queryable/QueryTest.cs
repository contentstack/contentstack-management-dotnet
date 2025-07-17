using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Contentstack.Management.Core.Unit.Tests.Queryable
{
    [TestClass]
    public class QueryTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();

        [TestInitialize]
        public void initialize()
        {
            _stack = new Stack(new ContentstackClient(authtoken: _fixture.Create<string>()), _fixture.Create<string>());
        }

        [TestMethod]
        public void Should_Throw_on_Stack_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Query(null, _fixture.Create<string>()));
        }

        [TestMethod]
        public void Should_Throw_on_Resource_Path_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Query(new Stack(null),null));
        }

        [TestMethod]
        public void Initialize_Query()
        {
            Query query = new Query(new Stack(new ContentstackClient(authtoken: _fixture.Create<string>())), _fixture.Create<string>());

            Assert.ThrowsException<InvalidOperationException>(() => query.Find());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => query.FindAsync());
        }

        [TestMethod]
        public void Initialize_Query_With_ApiVersion()
        {
            string apiVersion = "3.2";
            Query query = new Query(new Stack(new ContentstackClient(authtoken: _fixture.Create<string>())), _fixture.Create<string>(), apiVersion);

            Assert.ThrowsException<InvalidOperationException>(() => query.Find());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => query.FindAsync());
        }

        [TestMethod]
        public void Query_Pagination_Parameters()
        {
            Query query = new Query(_stack, _fixture.Create<string>());
            query.Limit(10);
            query.Skip(10);
            query.IncludeCount();
        }

        [TestMethod]
        public void Query_Pagination_Parameters_With_ApiVersion()
        {
            string apiVersion = "3.2";
            Query query = new Query(_stack, _fixture.Create<string>(), apiVersion);
            query.Limit(10);
            query.Skip(10);
            query.IncludeCount();
        }
    }
}
