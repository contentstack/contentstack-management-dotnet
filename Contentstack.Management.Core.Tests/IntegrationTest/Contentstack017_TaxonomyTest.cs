using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack017_TaxonomyTest
    {
        private static string _taxonomyUid;
        private Stack _stack;
        private TaxonomyModel _createModel;

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
            if (_taxonomyUid == null)
                _taxonomyUid = "taxonomy_integration_test_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            _createModel = new TaxonomyModel
            {
                Uid = _taxonomyUid,
                Name = "Taxonomy Integration Test",
                Description = "Description for taxonomy integration test"
            };
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Taxonomy()
        {
            ContentstackResponse response = _stack.Taxonomy().Create(_createModel);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Create failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual(_createModel.Uid, wrapper.Taxonomy.Uid);
            Assert.AreEqual(_createModel.Name, wrapper.Taxonomy.Name);
            Assert.AreEqual(_createModel.Description, wrapper.Taxonomy.Description);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Fetch_Taxonomy()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Fetch();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Fetch failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual(_taxonomyUid, wrapper.Taxonomy.Uid);
            Assert.IsNotNull(wrapper.Taxonomy.Name);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Query_Taxonomies()
        {
            ContentstackResponse response = _stack.Taxonomy().Query().Find();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Query failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomiesResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomies);
            Assert.IsTrue(wrapper.Taxonomies.Count >= 0);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Taxonomy()
        {
            var updateModel = new TaxonomyModel
            {
                Name = "Taxonomy Integration Test Updated",
                Description = "Updated description"
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Update(updateModel);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Update failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual("Taxonomy Integration Test Updated", wrapper.Taxonomy.Name);
            Assert.AreEqual("Updated description", wrapper.Taxonomy.Description);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Fetch_Taxonomy_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).FetchAsync();
            Assert.IsTrue(response.IsSuccessStatusCode, $"FetchAsync failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual(_taxonomyUid, wrapper.Taxonomy.Uid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Delete_Taxonomy()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Delete();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Delete failed: {response.OpenResponse()}");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Throw_When_Fetch_NonExistent_Taxonomy()
        {
            Assert.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy("non_existent_taxonomy_uid_12345").Fetch());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Throw_When_Delete_NonExistent_Taxonomy()
        {
            Assert.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy("non_existent_taxonomy_uid_12345").Delete());
        }
    }
}
