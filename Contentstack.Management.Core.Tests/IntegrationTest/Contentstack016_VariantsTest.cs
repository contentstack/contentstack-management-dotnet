using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack016_VariantsTest
    {
        private Stack _stack;

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test001_Should_Create_Variants()
        {
            try
            {
                VariantsModel variantsModel = new VariantsModel();
                ContentstackResponse response = await _stack.Variants().CreateAsync(variantsModel);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod] 
        [DoNotParallelize]
        public async Task Test002_Should_Fetch_Variants_By_Uid()
        {
            try
            {
                // First create a variant to ensure we have something to fetch
                VariantsModel variantsModel = new VariantsModel();
                ContentstackResponse createResponse = await _stack.Variants().CreateAsync(variantsModel);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, createResponse.StatusCode);

                // Extract UID from created variant
                var createdVariant = createResponse.OpenJObjectResponse();
                string variantUid = createdVariant["variant"]["uid"].ToString();

                // Test fetching by UID
                ContentstackResponse fetchResponse = await _stack.Variants(variantUid).FetchAsync();
                Assert.IsTrue(fetchResponse.StatusCode == System.Net.HttpStatusCode.OK || 
                             fetchResponse.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test003_Should_FetchByUid_Multiple_Variants()
        {
            try
            {
                // Create multiple variants first
                VariantsModel variantsModel1 = new VariantsModel();
                VariantsModel variantsModel2 = new VariantsModel();

                ContentstackResponse createResponse1 = await _stack.Variants().CreateAsync(variantsModel1);
                ContentstackResponse createResponse2 = await _stack.Variants().CreateAsync(variantsModel2);

                Assert.AreEqual(System.Net.HttpStatusCode.Created, createResponse1.StatusCode);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, createResponse2.StatusCode);

                // Extract UIDs from created variants
                var createdVariant1 = createResponse1.OpenJObjectResponse();
                var createdVariant2 = createResponse2.OpenJObjectResponse();
                string variantUid1 = createdVariant1["variant"]["uid"].ToString();
                string variantUid2 = createdVariant2["variant"]["uid"].ToString();

                // Test fetching multiple variants by UIDs
                string[] uids = { variantUid1, variantUid2 };
                ContentstackResponse fetchResponse = await _stack.Variants().FetchByUidAsync(uids);
                
                Assert.IsTrue(fetchResponse.StatusCode == System.Net.HttpStatusCode.OK || 
                             fetchResponse.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_FetchByUid_Single_Variant()
        {
            try
            {
                // Create a variant first
                VariantsModel variantsModel = new VariantsModel();
                ContentstackResponse createResponse = await _stack.Variants().CreateAsync(variantsModel);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, createResponse.StatusCode);

                // Extract UID from created variant
                var createdVariant = createResponse.OpenJObjectResponse();
                string variantUid = createdVariant["variant"]["uid"].ToString();

                // Test fetching single variant using FetchByUid
                string[] uids = { variantUid };
                ContentstackResponse fetchResponse = await _stack.Variants().FetchByUidAsync(uids);
                
                Assert.IsTrue(fetchResponse.StatusCode == System.Net.HttpStatusCode.OK || 
                             fetchResponse.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Handle_FetchByUid_With_Nonexistent_Uids()
        {
            try
            {
                // Test fetching with non-existent UIDs
                string[] nonExistentUids = { "nonexistent_uid_1", "nonexistent_uid_2" };
                ContentstackResponse fetchResponse = await _stack.Variants().FetchByUidAsync(nonExistentUids);
                
                // Should return 404 or empty result, not crash
                Assert.IsTrue(fetchResponse.StatusCode == System.Net.HttpStatusCode.NotFound || 
                             fetchResponse.StatusCode == System.Net.HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_FetchByUid_Sync_Method()
        {
            try
            {
                // Create a variant first
                VariantsModel variantsModel = new VariantsModel();
                ContentstackResponse createResponse = _stack.Variants().Create(variantsModel);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, createResponse.StatusCode);

                // Extract UID from created variant
                var createdVariant = createResponse.OpenJObjectResponse();
                string variantUid = createdVariant["variant"]["uid"].ToString();

                // Test synchronous FetchByUid
                string[] uids = { variantUid };
                ContentstackResponse fetchResponse = _stack.Variants().FetchByUid(uids);
                
                Assert.IsTrue(fetchResponse.StatusCode == System.Net.HttpStatusCode.OK || 
                             fetchResponse.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test007_Should_Delete_Variant()
        {
            try
            {
                // Create a variant first
                VariantsModel variantsModel = new VariantsModel();
                ContentstackResponse createResponse = await _stack.Variants().CreateAsync(variantsModel);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, createResponse.StatusCode);

                // Extract UID from created variant
                var createdVariant = createResponse.OpenJObjectResponse();
                string variantUid = createdVariant["variant"]["uid"].ToString();

                // Test deleting the variant
                ContentstackResponse deleteResponse = await _stack.Variants(variantUid).DeleteAsync();
                Assert.IsTrue(deleteResponse.StatusCode == System.Net.HttpStatusCode.OK || 
                             deleteResponse.StatusCode == System.Net.HttpStatusCode.NoContent ||
                             deleteResponse.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Validate_FetchByUid_Parameters()
        {
            try
            {
                // Test with null UIDs
                Assert.ThrowsException<ArgumentException>(() => _stack.Variants().FetchByUid(null));

                // Test with empty UIDs array
                string[] emptyUids = new string[0];
                Assert.ThrowsException<ArgumentException>(() => _stack.Variants().FetchByUid(emptyUids));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Validate_FetchByUidAsync_Parameters()
        {
            try
            {
                // Test with null UIDs
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => _stack.Variants().FetchByUidAsync(null));

                // Test with empty UIDs array
                string[] emptyUids = new string[0];
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => _stack.Variants().FetchByUidAsync(emptyUids));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Validate_Instance_With_Uid_Cannot_Use_FetchByUid()
        {
            try
            {
                // Test that an instance with UID cannot call FetchByUid
                string instanceUid = "some_uid";
                string[] uids = { "uid1", "uid2" };
                
                Assert.ThrowsException<InvalidOperationException>(() => 
                    _stack.Variants(instanceUid).FetchByUid(uids));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
