using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Token;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    [DoNotParallelize]
    public class Contentstack024_ManagementTokenTest
    {
        private static ContentstackClient _client;
        private static Stack _stack;

        private static string _suffix = Guid.NewGuid().ToString("N").Substring(0, 8);
        private static string _tokenUid;
        private static string _asyncTokenUid;
        private static string _originalTokenName;

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
            SafeDelete(_tokenUid);
            SafeDelete(_asyncTokenUid);
            try { _client?.Logout(); } catch { }
            _client = null;
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private static ManagementTokenModel BuildTokenModel(string name)
        {
            return new ManagementTokenModel
            {
                Name = name,
                Description = "Integration test token",
                Scope = new List<TokenScope>
                {
                    new TokenScope
                    {
                        Module = "content_type",
                        ACL = new Dictionary<string, string>
                        {
                            { "read", "true" },
                            { "write", "true" }
                        },
                        Branches = new List<string> { "main" }
                    }
                },
                ExpiresOn = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd")
            };
        }

        private static string ParseTokenUid(ContentstackResponse response)
        {
            return response.OpenJObjectResponse()["token"]["uid"].ToString();
        }

        private static void SafeDelete(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return;
            try { _stack.ManagementTokens(uid).Delete(); } catch { }
        }

        // ── Happy-path tests ─────────────────────────────────────────────────────

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_ManagementToken_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test001_Should_Create_ManagementToken_Sync");
            try
            {
                _originalTokenName = $"MgmtToken_Sync_{_suffix}";
                var model = BuildTokenModel(_originalTokenName);

                ContentstackResponse response = _stack.ManagementTokens().Create(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create management token (sync) should succeed", "CreateSyncSuccess");

                var tokenNode = response.OpenJObjectResponse()["token"];
                AssertLogger.IsNotNull(tokenNode, "Response should contain 'token' node", "TokenNodePresent");

                _tokenUid = tokenNode["uid"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(_tokenUid), "Token uid must be non-empty after create", "TokenUidNonEmpty");

                string returnedName = tokenNode["name"]?.ToString();
                AssertLogger.AreEqual(_originalTokenName, returnedName, "Returned token name must match the name we sent", "TokenNameMatch");

                var scope = tokenNode["scope"] as JArray;
                AssertLogger.IsNotNull(scope, "Response token should contain 'scope' array", "ScopePresent");
                AssertLogger.IsTrue(scope.Count == 1, "Scope array should have exactly 1 entry (catches scope being stripped)", "ScopeCount");

                string tokenSecret = tokenNode["token"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(tokenSecret), "Response token.token (the actual secret) must be non-empty", "TokenSecretNonEmpty");

                TestOutputLogger.LogContext("TokenUid", _tokenUid ?? "");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test001 failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_ManagementToken_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test002_Should_Create_ManagementToken_Async");
            try
            {
                string asyncName = $"MgmtToken_Async_{_suffix}";
                var model = BuildTokenModel(asyncName);

                ContentstackResponse response = await _stack.ManagementTokens().CreateAsync(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create management token (async) should succeed", "CreateAsyncSuccess");

                var tokenNode = response.OpenJObjectResponse()["token"];
                AssertLogger.IsNotNull(tokenNode, "Response should contain 'token' node", "AsyncTokenNodePresent");

                _asyncTokenUid = tokenNode["uid"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(_asyncTokenUid), "Async token uid must be non-empty", "AsyncTokenUidNonEmpty");

                AssertLogger.IsTrue(
                    _asyncTokenUid != _tokenUid,
                    "Async-created token uid must differ from the sync-created one (two distinct tokens)",
                    "TwoDistinctUids");

                TestOutputLogger.LogContext("AsyncTokenUid", _asyncTokenUid ?? "");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test002 failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_ManagementToken_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test003_Should_Fetch_ManagementToken_Sync");
            try
            {
                if (string.IsNullOrEmpty(_tokenUid))
                    Test001_Should_Create_ManagementToken_Sync();

                ContentstackResponse response = _stack.ManagementTokens(_tokenUid).Fetch();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Fetch management token (sync) should succeed", "FetchSyncSuccess");

                var tokenNode = response.OpenJObjectResponse()["token"];
                AssertLogger.IsNotNull(tokenNode, "Fetch response should contain 'token' node", "FetchTokenNodePresent");

                string fetchedUid = tokenNode["uid"]?.ToString();
                AssertLogger.AreEqual(_tokenUid, fetchedUid, "Fetched uid must match the uid we created (round-trip)", "FetchUidRoundTrip");

                string fetchedName = tokenNode["name"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(fetchedName), "Fetched token name must be non-empty", "FetchNameNonEmpty");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test003 failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Fetch_ManagementToken_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test004_Should_Fetch_ManagementToken_Async");
            try
            {
                if (string.IsNullOrEmpty(_asyncTokenUid))
                    await Test002_Should_Create_ManagementToken_Async();

                ContentstackResponse response = await _stack.ManagementTokens(_asyncTokenUid).FetchAsync();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Fetch management token (async) should succeed", "FetchAsyncSuccess");

                var tokenNode = response.OpenJObjectResponse()["token"];
                AssertLogger.IsNotNull(tokenNode, "Async fetch response should contain 'token' node", "AsyncFetchTokenNodePresent");

                string fetchedUid = tokenNode["uid"]?.ToString();
                AssertLogger.AreEqual(_asyncTokenUid, fetchedUid, "Async fetched uid must match _asyncTokenUid", "AsyncFetchUidMatch");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test004 failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_ManagementToken_Name_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test005_Should_Update_ManagementToken_Name_Sync");
            try
            {
                if (string.IsNullOrEmpty(_tokenUid))
                    Test001_Should_Create_ManagementToken_Sync();

                string updatedName = "Updated " + _originalTokenName;
                var updateModel = BuildTokenModel(updatedName);

                ContentstackResponse response = _stack.ManagementTokens(_tokenUid).Update(updateModel);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Update management token name (sync) should succeed", "UpdateNameSyncSuccess");

                var tokenNode = response.OpenJObjectResponse()["token"];
                AssertLogger.IsNotNull(tokenNode, "Update response should contain 'token' node", "UpdateTokenNodePresent");

                string returnedName = tokenNode["name"]?.ToString();
                AssertLogger.AreEqual(updatedName, returnedName, "Update response must reflect the new name (catches stale data)", "UpdatedNameReturned");
                AssertLogger.IsTrue(returnedName != _originalTokenName, "Updated name must differ from original name", "NameActuallyChanged");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test005 failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_ManagementToken_Description_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test006_Should_Update_ManagementToken_Description_Async");
            try
            {
                if (string.IsNullOrEmpty(_asyncTokenUid))
                    await Test002_Should_Create_ManagementToken_Async();

                string asyncName = $"MgmtToken_Async_{_suffix}";
                var updateModel = BuildTokenModel(asyncName);
                updateModel.Description = "Updated description";

                ContentstackResponse response = await _stack.ManagementTokens(_asyncTokenUid).UpdateAsync(updateModel);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Update management token description (async) should succeed", "UpdateDescAsyncSuccess");

                var tokenNode = response.OpenJObjectResponse()["token"];
                AssertLogger.IsNotNull(tokenNode, "Async update response should contain 'token' node", "AsyncUpdateTokenNodePresent");

                string returnedDescription = tokenNode["description"]?.ToString();
                AssertLogger.AreEqual("Updated description", returnedDescription, "Async update must return the new description", "AsyncUpdatedDescription");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test006 failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_ManagementTokens_Contains_Both()
        {
            TestOutputLogger.LogContext("TestScenario", "Test007_Should_Query_ManagementTokens_Contains_Both");
            try
            {
                if (string.IsNullOrEmpty(_tokenUid))
                    Test001_Should_Create_ManagementToken_Sync();

                ContentstackResponse response = _stack.ManagementTokens().Query().Find();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Query management tokens should succeed", "QuerySuccess");

                var tokensNode = response.OpenJObjectResponse()["tokens"] as JArray;
                AssertLogger.IsNotNull(tokensNode, "Query response should contain 'tokens' array", "TokensArrayPresent");
                AssertLogger.IsTrue(tokensNode.Count >= 2, "Query should return at least 2 tokens (the two we created)", "TokenCountAtLeastTwo");

                bool foundToken = false;
                foreach (var t in tokensNode)
                {
                    if (t["uid"]?.ToString() == _tokenUid)
                    {
                        foundToken = true;
                        break;
                    }
                }
                AssertLogger.IsTrue(foundToken, $"Query result must contain _tokenUid '{_tokenUid}' (catches token missing from list)", "TokenFoundInList");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test007 failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Delete_ManagementToken_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test008_Should_Delete_ManagementToken_Sync");
            try
            {
                if (string.IsNullOrEmpty(_asyncTokenUid))
                    Test002_Should_Create_ManagementToken_Async().GetAwaiter().GetResult();

                ContentstackResponse deleteResponse = _stack.ManagementTokens(_asyncTokenUid).Delete();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete management token (sync) should succeed", "DeleteSyncSuccess");

                // Verify delete is not a no-op: a subsequent Fetch must throw a 404/422.
                string deletedUid = _asyncTokenUid;
                _asyncTokenUid = null;

                AssertLogger.ThrowsContentstackError(
                    () => _stack.ManagementTokens(deletedUid).Fetch(),
                    "FetchAfterDelete",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test008 failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Delete_ManagementToken_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test009_Should_Delete_ManagementToken_Async");
            try
            {
                if (string.IsNullOrEmpty(_tokenUid))
                    Test001_Should_Create_ManagementToken_Sync();

                ContentstackResponse deleteResponse = await _stack.ManagementTokens(_tokenUid).DeleteAsync();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete management token (async) should succeed", "DeleteAsyncSuccess");

                _tokenUid = null;
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test009 failed: {ex.Message}");
            }
        }

        // ── Negative-path tests ──────────────────────────────────────────────────

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Throw_On_Fetch_NonExistent_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test010_Should_Throw_On_Fetch_NonExistent_Token");
            AssertLogger.ThrowsContentstackError(
                () => _stack.ManagementTokens("blt_nonexistent_token_xyz").Fetch(),
                "FetchNonExistentToken",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Throw_On_Create_Without_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "Test011_Should_Throw_On_Create_Without_Name");
            var model = new ManagementTokenModel
            {
                Name = null,
                Description = "Integration test token",
                Scope = new List<TokenScope>
                {
                    new TokenScope
                    {
                        Module = "content_type",
                        ACL = new Dictionary<string, string>
                        {
                            { "read", "true" },
                            { "write", "true" }
                        },
                        Branches = new List<string> { "main" }
                    }
                },
                ExpiresOn = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd")
            };

            AssertLogger.ThrowsContentstackError(
                () => _stack.ManagementTokens().Create(model),
                "CreateWithoutName",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Throw_On_Create_Without_Scope()
        {
            TestOutputLogger.LogContext("TestScenario", "Test012_Should_Throw_On_Create_Without_Scope");
            var model = new ManagementTokenModel
            {
                Name = $"MgmtToken_NoScope_{_suffix}",
                Description = "Integration test token",
                Scope = new List<TokenScope>(),
                ExpiresOn = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd")
            };

            AssertLogger.ThrowsContentstackError(
                () => _stack.ManagementTokens().Create(model),
                "CreateWithEmptyScope",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }
    }
}
