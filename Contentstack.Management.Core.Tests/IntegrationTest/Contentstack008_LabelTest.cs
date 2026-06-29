using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    [DoNotParallelize]
    public class Contentstack008_LabelTest
    {
        private static ContentstackClient _client;
        private static Stack _stack;

        private static readonly string _suffix = Guid.NewGuid().ToString("N").Substring(0, 8);
        private static readonly string _labelName = "Test Label " + _suffix;
        private static readonly string _asyncLabelName = "Async Label " + _suffix;

        private static string _labelUid;
        private static string _asyncLabelUid;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            SafeDelete(_labelUid);
            SafeDelete(_asyncLabelUid);
            try { _client?.Logout(); } catch { }
            _client = null;
        }

        // -----------------------------------------------------------------------
        // Private helpers
        // -----------------------------------------------------------------------

        private static LabelModel BuildLabelModel(string name)
        {
            return new LabelModel
            {
                Name = name,
                ContentTypes = new List<string>()
            };
        }

        private static string ParseLabelUid(ContentstackResponse response)
        {
            return response.OpenJObjectResponse()["label"]["uid"].ToString();
        }

        private static void SafeDelete(string uid)
        {
            if (string.IsNullOrEmpty(uid))
                return;
            try { _stack.Label(uid).Delete(); } catch { }
        }

        // -----------------------------------------------------------------------
        // Tests
        // -----------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Label_Sync()
        {
            try
            {
                ContentstackResponse contentstackResponse = _stack.Label("").Create(BuildLabelModel(_labelName));
                AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Create label must succeed", "CreateSuccess");

                var jobj = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(jobj?["label"], "label node in response");

                _labelUid = jobj["label"]["uid"].ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(_labelUid), "Label uid must be non-empty", "LabelUidNonEmpty");

                string returnedName = jobj["label"]["name"].ToString();
                AssertLogger.AreEqual(_labelName, returnedName, "Returned label name must match", "LabelName");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test001_Should_Create_Label_Sync failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_Label_Async()
        {
            try
            {
                ContentstackResponse contentstackResponse = await _stack.Label("").CreateAsync(BuildLabelModel(_asyncLabelName));
                AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Async create label must succeed", "CreateAsyncSuccess");

                var jobj = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(jobj?["label"], "label node in async response");

                _asyncLabelUid = jobj["label"]["uid"].ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(_asyncLabelUid), "Async label uid must be non-empty", "AsyncLabelUidNonEmpty");

                string returnedName = jobj["label"]["name"].ToString();
                AssertLogger.AreEqual(_asyncLabelName, returnedName, "Returned async label name must match", "AsyncLabelName");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test002_Should_Create_Label_Async failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_Label_Sync()
        {
            if (string.IsNullOrEmpty(_labelUid))
            {
                AssertLogger.Inconclusive("_labelUid is not set; Test001 may have failed.");
                return;
            }

            try
            {
                ContentstackResponse contentstackResponse = _stack.Label(_labelUid).Fetch();
                AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Fetch label must succeed", "FetchSuccess");

                var jobj = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(jobj?["label"], "label node in fetch response");

                string fetchedUid = jobj["label"]["uid"].ToString();
                AssertLogger.AreEqual(_labelUid, fetchedUid, "Fetched uid must match stored uid (round-trip)", "FetchedUidRoundTrip");

                string fetchedName = jobj["label"]["name"].ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(fetchedName), "Fetched name must be non-empty", "FetchedNameNonEmpty");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test003_Should_Fetch_Label_Sync failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Fetch_Label_Async()
        {
            if (string.IsNullOrEmpty(_asyncLabelUid))
            {
                AssertLogger.Inconclusive("_asyncLabelUid is not set; Test002 may have failed.");
                return;
            }

            try
            {
                ContentstackResponse contentstackResponse = await _stack.Label(_asyncLabelUid).FetchAsync();
                AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Async fetch label must succeed", "FetchAsyncSuccess");

                var jobj = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(jobj?["label"], "label node in async fetch response");

                string fetchedUid = jobj["label"]["uid"].ToString();
                AssertLogger.AreEqual(_asyncLabelUid, fetchedUid, "Async fetched uid must match stored uid (round-trip)", "AsyncFetchedUidRoundTrip");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test004_Should_Fetch_Label_Async failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_Label_Name_Sync()
        {
            if (string.IsNullOrEmpty(_labelUid))
            {
                AssertLogger.Inconclusive("_labelUid is not set; Test001 may have failed.");
                return;
            }

            try
            {
                string updatedName = "Updated " + _labelName;
                ContentstackResponse contentstackResponse = _stack.Label(_labelUid).Update(BuildLabelModel(updatedName));
                AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Update label must succeed", "UpdateSuccess");

                var jobj = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(jobj?["label"], "label node in update response");

                string returnedName = jobj["label"]["name"].ToString();
                AssertLogger.AreEqual(updatedName, returnedName, "Updated name must match new name", "UpdatedNameMatch");
                AssertLogger.IsTrue(returnedName != _labelName,
                    $"Updated name must differ from original name (catches update-ignored bug). Got: '{returnedName}', original: '{_labelName}'",
                    "UpdatedNameDiffersFromOriginal");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test005_Should_Update_Label_Name_Sync failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_Label_Async()
        {
            if (string.IsNullOrEmpty(_asyncLabelUid))
            {
                AssertLogger.Inconclusive("_asyncLabelUid is not set; Test002 may have failed.");
                return;
            }

            try
            {
                string updatedName = "Updated " + _asyncLabelName;
                ContentstackResponse contentstackResponse = await _stack.Label(_asyncLabelUid).UpdateAsync(BuildLabelModel(updatedName));
                AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Async update label must succeed", "UpdateAsyncSuccess");

                var jobj = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(jobj?["label"], "label node in async update response");

                string returnedName = jobj["label"]["name"].ToString();
                AssertLogger.AreEqual(updatedName, returnedName, "Async updated name must match new name", "AsyncUpdatedNameMatch");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test006_Should_Update_Label_Async failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_Labels_Contains_Created()
        {
            if (string.IsNullOrEmpty(_asyncLabelUid))
            {
                AssertLogger.Inconclusive("_asyncLabelUid is not set; Test002 may have failed.");
                return;
            }

            try
            {
                ContentstackResponse contentstackResponse = _stack.Label("").Query().Find();
                AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Query labels must succeed", "QuerySuccess");

                var jobj = contentstackResponse.OpenJObjectResponse();
                AssertLogger.IsNotNull(jobj?["labels"], "labels array in query response");

                var labels = jobj["labels"] as JArray;
                AssertLogger.IsNotNull(labels, "labels must be a JArray");

                bool found = false;
                foreach (var label in labels)
                {
                    if (label["uid"]?.ToString() == _asyncLabelUid)
                    {
                        found = true;
                        break;
                    }
                }

                AssertLogger.IsTrue(found,
                    $"Query must contain the created label with uid='{_asyncLabelUid}' (catches labels not appearing in list)",
                    "AsyncLabelFoundInQuery");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test007_Should_Query_Labels_Contains_Created failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Delete_Label_Sync()
        {
            if (string.IsNullOrEmpty(_asyncLabelUid))
            {
                AssertLogger.Inconclusive("_asyncLabelUid is not set; Test002 may have failed.");
                return;
            }

            try
            {
                // Delete the async label
                ContentstackResponse deleteResponse = _stack.Label(_asyncLabelUid).Delete();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete must succeed", "DeleteSuccess");

                // Verify it is truly gone — fetch must fail with 404/422
                bool deletedConfirmed = false;
                try
                {
                    ContentstackResponse fetchAfterDelete = _stack.Label(_asyncLabelUid).Fetch();
                    // If we reach here without exception, confirm via status code
                    deletedConfirmed = !fetchAfterDelete.IsSuccessStatusCode;
                }
                catch (ContentstackErrorException csEx)
                {
                    int status = (int)csEx.StatusCode;
                    deletedConfirmed = status == 404 || status == 422 || status == 422;
                    AssertLogger.IsTrue(deletedConfirmed,
                        $"Fetch after delete should throw 404 or 422, got {csEx.StatusCode}: {csEx.Message}",
                        "FetchAfterDeleteStatusCode");
                }

                AssertLogger.IsTrue(deletedConfirmed,
                    "Fetch after delete must fail (verifies delete is not a no-op)",
                    "DeleteConfirmed");

                // Mark uid as null so ClassCleanup does not attempt a second delete
                _asyncLabelUid = null;
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test008_Should_Delete_Label_Sync failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Delete_Label_Async()
        {
            if (string.IsNullOrEmpty(_labelUid))
            {
                AssertLogger.Inconclusive("_labelUid is not set; Test001 may have failed.");
                return;
            }

            try
            {
                ContentstackResponse deleteResponse = await _stack.Label(_labelUid).DeleteAsync();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Async delete must succeed", "DeleteAsyncSuccess");

                // Mark uid as null so ClassCleanup does not attempt a second delete
                _labelUid = null;
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test009_Should_Delete_Label_Async failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Throw_On_Fetch_NonExistent_Label()
        {
            try
            {
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Label("blt_label_does_not_exist").Fetch(),
                    "FetchNonExistentLabel",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception e)
            {
                // Some SDK versions may throw a different exception type wrapping the HTTP error
                AssertLogger.IsTrue(
                    e.Message.Contains("404") || e.Message.Contains("422") ||
                    e.Message.ToLowerInvariant().Contains("not found") ||
                    e.Message.ToLowerInvariant().Contains("does not exist"),
                    $"Expected 404/422 style error for non-existent label fetch, got: {e.Message}",
                    "NonExistentFetchError");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Throw_On_Create_Empty_Name()
        {
            try
            {
                bool exceptionCaught = false;
                try
                {
                    ContentstackResponse contentstackResponse = _stack.Label("").Create(BuildLabelModel(string.Empty));
                    // If we get here without exception, assert status is not success
                    exceptionCaught = !contentstackResponse.IsSuccessStatusCode;
                }
                catch (ContentstackErrorException csEx)
                {
                    int status = (int)csEx.StatusCode;
                    exceptionCaught = status == 400 || status == 422;
                    AssertLogger.IsTrue(exceptionCaught,
                        $"Create with empty name should throw 400 or 422, got {csEx.StatusCode}: {csEx.Message}",
                        "EmptyNameStatusCode");
                }

                AssertLogger.IsTrue(exceptionCaught,
                    "Creating a label with an empty name must fail with 400 or 422",
                    "EmptyNameFails");
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Test011_Should_Throw_On_Create_Empty_Name failed unexpectedly: {e.Message}");
            }
        }
    }
}
