using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class TaxonomyTest
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
        public void Initialize_Taxonomy()
        {
            Taxonomy taxonomy = _stack.Taxonomy();

            Assert.IsNull(taxonomy.Uid);
            Assert.AreEqual("/taxonomies", taxonomy.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => taxonomy.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => taxonomy.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => taxonomy.Update(_fixture.Create<TaxonomyModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => taxonomy.UpdateAsync(_fixture.Create<TaxonomyModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => taxonomy.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => taxonomy.DeleteAsync());
            Assert.ThrowsException<InvalidOperationException>(() => taxonomy.Terms());
            Assert.AreEqual(typeof(Query), taxonomy.Query().GetType());
        }

        [TestMethod]
        public void Initialize_Taxonomy_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Taxonomy taxonomy = _stack.Taxonomy(uid);

            Assert.AreEqual(uid, taxonomy.Uid);
            Assert.AreEqual($"/taxonomies/{uid}", taxonomy.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => taxonomy.Create(_fixture.Create<TaxonomyModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => taxonomy.CreateAsync(_fixture.Create<TaxonomyModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => taxonomy.Query());
        }

        [TestMethod]
        public void Should_Create_Taxonomy()
        {
            ContentstackResponse response = _stack.Taxonomy().Create(_fixture.Create<TaxonomyModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Taxonomy_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy().CreateAsync(_fixture.Create<TaxonomyModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Taxonomy()
        {
            ContentstackResponse response = _stack.Taxonomy().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Taxonomy_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Taxonomy()
        {
            ContentstackResponse response = _stack.Taxonomy(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Fetch_Taxonomy_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Get_Terms_From_Taxonomy()
        {
            string taxonomyUid = _fixture.Create<string>();
            Term terms = _stack.Taxonomy(taxonomyUid).Terms();

            Assert.IsNotNull(terms);
            Assert.IsNull(terms.Uid);
            Assert.AreEqual($"/taxonomies/{taxonomyUid}/terms", terms.resourcePath);
        }

        [TestMethod]
        public void Should_Get_Single_Term_From_Taxonomy()
        {
            string taxonomyUid = _fixture.Create<string>();
            string termUid = _fixture.Create<string>();
            Term term = _stack.Taxonomy(taxonomyUid).Terms(termUid);

            Assert.IsNotNull(term);
            Assert.AreEqual(termUid, term.Uid);
            Assert.AreEqual($"/taxonomies/{taxonomyUid}/terms/{termUid}", term.resourcePath);
        }
    }
}
