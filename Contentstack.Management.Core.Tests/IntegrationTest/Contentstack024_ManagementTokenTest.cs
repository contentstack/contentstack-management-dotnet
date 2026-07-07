using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Token;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack024_ManagementTokenTest
    {
        private static ContentstackClient _client;
        private Stack _stack;
        private string _managementTokenUid;
        private ManagementTokenModel _testTokenModel;

        private const string NonExistentTokenUid = "blt00000000000000000000";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            try { _client?.Logout(); } catch { }
            _client = null;
        }

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);

            _testTokenModel = BuildValidTokenModel("Test Management Token");
        }

        private static ManagementTokenModel BuildValidTokenModel(string name)
        {
            return new ManagementTokenModel
            {
                Name = name,
                Description = "Integration test management token",
                Scope = new List<TokenScope>
                {
                    new TokenScope
                    {
                        Module = "content_type",
                        ACL = new Dictionary<string, string> { { "read", "true" }, { "write", "true" } }
                    },
                    new TokenScope
                    {
                        Module = "entry",
                        ACL = new Dictionary<string, string> { { "read", "true" }, { "write", "true" } }
                    }
                }
            };
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Management_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test001_Should_Create_Management_Token");
            try
            {
                ContentstackResponse response = _stack.ManagementTokens().Create(_testTokenModel);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Create management token failed: {response.OpenResponse()}", "CreateManagementTokenSuccess");

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JsonObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");
                AssertLogger.AreEqual(_testTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match", "TokenName");

                // Proves the JSON-casing fix: a "Scope"-cased payload would previously have been
                // silently dropped by the API, leaving this field empty/missing.
                var scope = tokenData["scope"] as JsonArray;
                AssertLogger.IsNotNull(scope, "Token should have scope");
                AssertLogger.IsTrue(scope.Count == _testTokenModel.Scope.Count, "Scope module count should round-trip", "ScopeRoundTrip");

                _managementTokenUid = tokenData["uid"]?.ToString();
                AssertLogger.IsNotNull(_managementTokenUid, "Management token UID should not be null");

                TestOutputLogger.LogContext("ManagementTokenUid", _managementTokenUid ?? "");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Create management token test failed", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_Management_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test002_Should_Create_Management_Token_Async");
            try
            {
                var model = BuildValidTokenModel("Async Test Management Token");
                ContentstackResponse response = await _stack.ManagementTokens().CreateAsync(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Async create management token failed: {response.OpenResponse()}", "AsyncCreateSuccess");

                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");

                string asyncTokenUid = tokenData["uid"]?.ToString();
                TestOutputLogger.LogContext("AsyncCreatedTokenUid", asyncTokenUid ?? "");

                if (!string.IsNullOrEmpty(asyncTokenUid))
                {
                    await _stack.ManagementTokens(asyncTokenUid).DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Async create management token test failed", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_Management_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test003_Should_Fetch_Management_Token");
            try
            {
                if (string.IsNullOrEmpty(_managementTokenUid))
                {
                    Test001_Should_Create_Management_Token();
                }

                ContentstackResponse response = _stack.ManagementTokens(_managementTokenUid).Fetch();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Fetch management token failed: {response.OpenResponse()}", "FetchSuccess");

                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                AssertLogger.AreEqual(_managementTokenUid, tokenData["uid"]?.ToString(), "Token UID should match", "TokenUid");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Fetch management token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Fetch_Management_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test004_Should_Fetch_Management_Token_Async");
            try
            {
                if (string.IsNullOrEmpty(_managementTokenUid))
                {
                    Test001_Should_Create_Management_Token();
                }

                ContentstackResponse response = await _stack.ManagementTokens(_managementTokenUid).FetchAsync();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Async fetch management token failed: {response.OpenResponse()}", "AsyncFetchSuccess");

                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                AssertLogger.AreEqual(_managementTokenUid, tokenData["uid"]?.ToString(), "Token UID should match", "TokenUid");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Async fetch management token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_Management_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test005_Should_Update_Management_Token");
            try
            {
                if (string.IsNullOrEmpty(_managementTokenUid))
                {
                    Test001_Should_Create_Management_Token();
                }

                var updateModel = BuildValidTokenModel("Updated Test Management Token");
                ContentstackResponse response = _stack.ManagementTokens(_managementTokenUid).Update(updateModel);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Update management token failed: {response.OpenResponse()}", "UpdateSuccess");

                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                AssertLogger.AreEqual(_managementTokenUid, tokenData["uid"]?.ToString(), "Token UID should match", "TokenUid");
                AssertLogger.AreEqual(updateModel.Name, tokenData["name"]?.ToString(), "Updated token name should match", "UpdatedTokenName");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Update management token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_Management_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test006_Should_Update_Management_Token_Async");
            try
            {
                if (string.IsNullOrEmpty(_managementTokenUid))
                {
                    Test001_Should_Create_Management_Token();
                }

                var updateModel = BuildValidTokenModel("Async Updated Test Management Token");
                ContentstackResponse response = await _stack.ManagementTokens(_managementTokenUid).UpdateAsync(updateModel);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Async update management token failed: {response.OpenResponse()}", "AsyncUpdateSuccess");

                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                AssertLogger.AreEqual(updateModel.Name, tokenData["name"]?.ToString(), "Updated token name should match", "UpdatedTokenName");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Async update management token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_All_Management_Tokens()
        {
            TestOutputLogger.LogContext("TestScenario", "Test007_Should_Query_All_Management_Tokens");
            try
            {
                if (string.IsNullOrEmpty(_managementTokenUid))
                {
                    Test001_Should_Create_Management_Token();
                }

                ContentstackResponse response = _stack.ManagementTokens().Query().Find();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Query management tokens failed: {response.OpenResponse()}", "QuerySuccess");

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JsonArray;
                AssertLogger.IsTrue(tokens.Count > 0, "Should have at least one management token", "TokensCountGreaterThanZero");

                bool foundTestToken = false;
                foreach (var token in tokens)
                {
                    if (token["uid"]?.ToString() == _managementTokenUid)
                    {
                        foundTestToken = true;
                        break;
                    }
                }

                AssertLogger.IsTrue(foundTestToken, "Test token should be found in query results", "TestTokenFoundInQuery");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Query management tokens test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Should_Query_All_Management_Tokens_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test008_Should_Query_All_Management_Tokens_Async");
            try
            {
                if (string.IsNullOrEmpty(_managementTokenUid))
                {
                    Test001_Should_Create_Management_Token();
                }

                ContentstackResponse response = await _stack.ManagementTokens().Query().FindAsync();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Async query management tokens failed: {response.OpenResponse()}", "AsyncQuerySuccess");

                var responseObject = response.OpenJsonObjectResponse();
                var tokens = responseObject["tokens"] as JsonArray;
                AssertLogger.IsTrue(tokens.Count > 0, "Should have at least one management token", "AsyncTokensCount");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Async query management tokens test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Delete_Management_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test009_Should_Delete_Management_Token");
            try
            {
                ContentstackResponse createResponse = _stack.ManagementTokens().Create(BuildValidTokenModel("Delete Test Management Token"));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Setup create for delete test failed");
                var tokenData = createResponse.OpenJsonObjectResponse()["token"] as JsonObject;
                string tokenUidToDelete = tokenData["uid"]?.ToString();
                AssertLogger.IsNotNull(tokenUidToDelete, "Should have a valid token UID to delete");

                ContentstackResponse response = _stack.ManagementTokens(tokenUidToDelete).Delete();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Delete management token failed: {response.OpenResponse()}", "DeleteSuccess");

                AssertLogger.ThrowsContentstackError(
                    () => _stack.ManagementTokens(tokenUidToDelete).Fetch(),
                    "FetchDeletedManagementToken",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Delete management token test failed", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test010_Should_Delete_Management_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test010_Should_Delete_Management_Token_Async");
            try
            {
                ContentstackResponse createResponse = await _stack.ManagementTokens().CreateAsync(BuildValidTokenModel("Async Delete Test Management Token"));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Setup create for async delete test failed");
                var tokenData = createResponse.OpenJsonObjectResponse()["token"] as JsonObject;
                string tokenUidToDelete = tokenData["uid"]?.ToString();
                AssertLogger.IsNotNull(tokenUidToDelete, "Should have a valid token UID to delete");

                ContentstackResponse response = await _stack.ManagementTokens(tokenUidToDelete).DeleteAsync();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Async delete management token failed: {response.OpenResponse()}", "AsyncDeleteSuccess");

                await AssertLogger.ThrowsContentstackErrorAsync(
                    async () => await _stack.ManagementTokens(tokenUidToDelete).FetchAsync(),
                    "AsyncFetchDeletedManagementToken",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Async delete management token test failed", ex.Message);
            }
        }

        #region Negative-path tests

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Fail_Create_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "Test011_Should_Fail_Create_With_Null_Model");
            AssertLogger.ThrowsException<ArgumentNullException>(
                () => _stack.ManagementTokens().Create(null),
                "CreateWithNullModel");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test012_Should_Fail_Create_With_Null_Model_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test012_Should_Fail_Create_With_Null_Model_Async");
            await AssertLogger.ThrowsExceptionAsync<ArgumentNullException>(
                () => _stack.ManagementTokens().CreateAsync(null),
                "CreateWithNullModelAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Fail_Fetch_With_Null_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Test013_Should_Fail_Fetch_With_Null_Uid");
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.ManagementTokens(null).Fetch(),
                "FetchWithNullUid");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Fail_Fetch_With_Empty_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Test014_Should_Fail_Fetch_With_Empty_Uid");
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.ManagementTokens("").Fetch(),
                "FetchWithEmptyUid");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Fail_Fetch_NonExistent_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test015_Should_Fail_Fetch_NonExistent_Token");
            AssertLogger.ThrowsContentstackError(
                () => _stack.ManagementTokens(NonExistentTokenUid).Fetch(),
                "FetchNonExistentToken",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test016_Should_Fail_Fetch_NonExistent_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test016_Should_Fail_Fetch_NonExistent_Token_Async");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.ManagementTokens(NonExistentTokenUid).FetchAsync(),
                "FetchNonExistentTokenAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Fail_Update_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "Test017_Should_Fail_Update_With_Null_Model");
            AssertLogger.ThrowsException<ArgumentNullException>(
                () => _stack.ManagementTokens(NonExistentTokenUid).Update(null),
                "UpdateWithNullModel");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Fail_Update_NonExistent_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test018_Should_Fail_Update_NonExistent_Token");
            var model = BuildValidTokenModel("Update Non Existent");
            AssertLogger.ThrowsContentstackError(
                () => _stack.ManagementTokens(NonExistentTokenUid).Update(model),
                "UpdateNonExistentToken",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_Fail_Delete_NonExistent_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test019_Should_Fail_Delete_NonExistent_Token");
            AssertLogger.ThrowsContentstackError(
                () => _stack.ManagementTokens(NonExistentTokenUid).Delete(),
                "DeleteNonExistentToken",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test020_Should_Fail_When_Stack_Api_Key_Missing()
        {
            TestOutputLogger.LogContext("TestScenario", "Test020_Should_Fail_When_Stack_Api_Key_Missing");
            var stackWithNoApiKey = _client.Stack();
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => stackWithNoApiKey.ManagementTokens(),
                "ManagementTokensWithMissingApiKey");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test021_Should_Fail_Delete_With_Empty_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Test021_Should_Fail_Delete_With_Empty_Uid");
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.ManagementTokens("").Delete(),
                "DeleteWithEmptyUid");
        }

        #endregion
    }
}
