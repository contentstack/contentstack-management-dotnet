using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class TermTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;

        [TestInitialize]
        public void Initialize()
        {
            var client = new ContentstackClient();
            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(_contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
        }

        [TestMethod]
        public void Initialize_Term_Collection()
        {
            string taxonomyUid = _fixture.Create<string>();
            Term term = _stack.Taxonomy(taxonomyUid).Terms();

            Assert.IsNull(term.Uid);
            Assert.AreEqual($"/taxonomies/{taxonomyUid}/terms", term.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => term.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => term.FetchAsync());
            Assert.AreEqual(typeof(Query), term.Query().GetType());
        }

        [TestMethod]
        public void Initialize_Term_With_Uid()
        {
            string taxonomyUid = _fixture.Create<string>();
            string termUid = _fixture.Create<string>();
            Term term = _stack.Taxonomy(taxonomyUid).Terms(termUid);

            Assert.AreEqual(termUid, term.Uid);
            Assert.AreEqual($"/taxonomies/{taxonomyUid}/terms/{termUid}", term.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => term.Create(_fixture.Create<TermModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => term.CreateAsync(_fixture.Create<TermModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => term.Query());
            Assert.ThrowsException<InvalidOperationException>(() => term.Search("x"));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => term.SearchAsync("x"));
        }

        [TestMethod]
        public void Should_Create_Term()
        {
            string taxonomyUid = _fixture.Create<string>();
            ContentstackResponse response = _stack.Taxonomy(taxonomyUid).Terms().Create(_fixture.Create<TermModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Term_Async()
        {
            string taxonomyUid = _fixture.Create<string>();
            ContentstackResponse response = await _stack.Taxonomy(taxonomyUid).Terms().CreateAsync(_fixture.Create<TermModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Terms()
        {
            string taxonomyUid = _fixture.Create<string>();
            ContentstackResponse response = _stack.Taxonomy(taxonomyUid).Terms().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Terms_Async()
        {
            string taxonomyUid = _fixture.Create<string>();
            ContentstackResponse response = await _stack.Taxonomy(taxonomyUid).Terms().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Term()
        {
            string taxonomyUid = _fixture.Create<string>();
            string termUid = _fixture.Create<string>();
            ContentstackResponse response = _stack.Taxonomy(taxonomyUid).Terms(termUid).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_Term_Async()
        {
            string taxonomyUid = _fixture.Create<string>();
            string termUid = _fixture.Create<string>();
            ContentstackResponse response = await _stack.Taxonomy(taxonomyUid).Terms(termUid).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Search_Terms()
        {
            string taxonomyUid = _fixture.Create<string>();
            ContentstackResponse response = _stack.Taxonomy(taxonomyUid).Terms().Search("test");

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Search_Terms_Async()
        {
            string taxonomyUid = _fixture.Create<string>();
            ContentstackResponse response = await _stack.Taxonomy(taxonomyUid).Terms().SearchAsync("test");

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Ancestors_Throws_When_Term_Uid_Is_Empty()
        {
            string taxonomyUid = _fixture.Create<string>();
            Assert.ThrowsException<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().Ancestors());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().AncestorsAsync());
        }

        [TestMethod]
        public void Descendants_Throws_When_Term_Uid_Is_Empty()
        {
            string taxonomyUid = _fixture.Create<string>();
            Assert.ThrowsException<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().Descendants());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().DescendantsAsync());
        }

        [TestMethod]
        public void Move_Throws_When_Term_Uid_Is_Empty()
        {
            string taxonomyUid = _fixture.Create<string>();
            Assert.ThrowsException<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().Move(_fixture.Create<TermMoveModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().MoveAsync(_fixture.Create<TermMoveModel>()));
        }

        [TestMethod]
        public void Locales_Throws_When_Term_Uid_Is_Empty()
        {
            string taxonomyUid = _fixture.Create<string>();
            Assert.ThrowsException<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().Locales());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().LocalesAsync());
        }

        [TestMethod]
        public void Localize_Throws_When_Term_Uid_Is_Empty()
        {
            string taxonomyUid = _fixture.Create<string>();
            Assert.ThrowsException<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().Localize(_fixture.Create<TermModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _stack.Taxonomy(taxonomyUid).Terms().LocalizeAsync(_fixture.Create<TermModel>()));
        }
    }
}
