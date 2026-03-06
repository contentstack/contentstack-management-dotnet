using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Token;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Queryable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack016_DeliveryTokenTest
    {
        private Stack _stack;
        private string _deliveryTokenUid;
        private string _testEnvironmentUid = "test_delivery_environment";
        private DeliveryTokenModel _testTokenModel;

        [TestInitialize]
        public async Task Initialize()
        {
            TestReportHelper.Begin();
            try
            {
                // First, ensure the client is logged in
                try
                {
                    ContentstackResponse loginResponse = Contentstack.Client.Login(Contentstack.Credential);
                    if (!loginResponse.IsSuccessStatusCode)
                    {
                        Assert.Fail($"Login failed: {loginResponse.OpenResponse()}");
                    }
                }
                catch (Exception loginEx)
                {
                    // If already logged in, that's fine - continue with the test
                    if (loginEx.Message.Contains("already logged in"))
                    {
                        Console.WriteLine("Client already logged in, continuing with test");
                    }
                    else
                    {
                        throw; // Re-throw if it's a different error
                    }
                }

                StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
                _stack = Contentstack.Client.Stack(response.Stack.APIKey);

                await CreateTestEnvironment();

                _testTokenModel = new DeliveryTokenModel
                {
                    Name = "Test Delivery Token",
                    Description = "Integration test delivery token",
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "environment",
                            Environments = new List<string> { _testEnvironmentUid },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        },
                        new DeliveryTokenScope
                        {
                            Module = "branch",
                            Branches = new List<string> { "main" },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Assert.Fail($"Initialize failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test001_Should_Create_Delivery_Token()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.DeliveryToken().Create()", "POST",
                    $"https://{Contentstack.Client.contentstackOptions.Host}/v3/stacks/delivery_tokens");
                ContentstackResponse response = _stack.DeliveryToken().Create(_testTokenModel);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode, response.StatusCode.ToString(),
                    sw.ElapsedMilliseconds, response.OpenResponse());

                TestReportHelper.LogAssertion(response.IsSuccessStatusCode, "Response is successful", type: "IsTrue");
                Assert.IsTrue(response.IsSuccessStatusCode, $"Create delivery token failed");

                var responseObject = response.OpenJObjectResponse();
                Assert.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                Assert.IsNotNull(tokenData["uid"], "Token should have UID");
                Assert.AreEqual(_testTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match");
                Assert.AreEqual(_testTokenModel.Description, tokenData["description"]?.ToString(), "Token description should match");

                _deliveryTokenUid = tokenData["uid"]?.ToString();
                Assert.IsNotNull(_deliveryTokenUid, "Delivery token UID should not be null");

                Console.WriteLine($"Created delivery token with UID: {_deliveryTokenUid}");
            }
            catch (Exception ex)
            {
                Assert.Fail("Create delivery token test failed", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_Delivery_Token_Async()
        {
            try
            {
                var asyncTokenModel = new DeliveryTokenModel
                {
                    Name = "Async Test Delivery Token",
                    Description = "Async integration test delivery token",
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "environment",
                            Environments = new List<string> { _testEnvironmentUid },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        },
                        new DeliveryTokenScope
                        {
                            Module = "branch",
                            Branches = new List<string> { "main" },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        }
                    }
                };

                ContentstackResponse response = await _stack.DeliveryToken().CreateAsync(asyncTokenModel);

                Assert.IsTrue(response.IsSuccessStatusCode, $"Async create delivery token failed");

                var responseObject = response.OpenJObjectResponse();
                Assert.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                Assert.IsNotNull(tokenData["uid"], "Token should have UID");
                Assert.AreEqual(asyncTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match");

                string asyncTokenUid = tokenData["uid"]?.ToString();

                if (!string.IsNullOrEmpty(asyncTokenUid))
                {
                    await _stack.DeliveryToken(asyncTokenUid).DeleteAsync();
                }

            }
            catch (Exception ex)
            {
                Assert.Fail("Async create delivery token test failed", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test003_Should_Fetch_Delivery_Token()
        {
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                ContentstackResponse response = _stack.DeliveryToken(_deliveryTokenUid).Fetch();

                Assert.IsTrue(response.IsSuccessStatusCode, $"Fetch delivery token failed");

                var responseObject = response.OpenJObjectResponse();
                Assert.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                Assert.AreEqual(_deliveryTokenUid, tokenData["uid"]?.ToString(), "Token UID should match");
                Assert.AreEqual(_testTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match");
                Assert.IsNotNull(tokenData["token"], "Token should have access token");

            }
            catch (Exception ex)
            {
                Assert.Fail($"Fetch delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Fetch_Delivery_Token_Async()
        {
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                ContentstackResponse response = await _stack.DeliveryToken(_deliveryTokenUid).FetchAsync();

                Assert.IsTrue(response.IsSuccessStatusCode, $"Async fetch delivery token failed");

                var responseObject = response.OpenJObjectResponse();
                Assert.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                Assert.AreEqual(_deliveryTokenUid, tokenData["uid"]?.ToString(), "Token UID should match");
                Assert.IsNotNull(tokenData["token"], "Token should have access token");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Async fetch delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Update_Delivery_Token()
        {
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                var updateModel = new DeliveryTokenModel
                {
                    Name = "Updated Test Delivery Token",
                    Description = "Updated integration test delivery token",
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "environment",
                            Environments = new List<string> { _testEnvironmentUid },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        },
                        new DeliveryTokenScope
                        {
                            Module = "branch",
                            Branches = new List<string> { "main" },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        }
                    }
                };

                ContentstackResponse response = _stack.DeliveryToken(_deliveryTokenUid).Update(updateModel);

                Assert.IsTrue(response.IsSuccessStatusCode, $"Update delivery token failed");

                var responseObject = response.OpenJObjectResponse();
                Assert.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                Assert.AreEqual(_deliveryTokenUid, tokenData["uid"]?.ToString(), "Token UID should match");
                Assert.AreEqual(updateModel.Name, tokenData["name"]?.ToString(), "Updated token name should match");
                Assert.AreEqual(updateModel.Description, tokenData["description"]?.ToString(), "Updated token description should match");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Update delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_Delivery_Token_Async()
        {
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                var updateModel = new DeliveryTokenModel
                {
                    Name = "Async Updated Test Delivery Token",
                    Description = "Async updated integration test delivery token",
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "environment",
                            Environments = new List<string> { _testEnvironmentUid },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        },
                        new DeliveryTokenScope
                        {
                            Module = "branch",
                            Branches = new List<string> { "main" },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        }
                    }
                };

                ContentstackResponse response = await _stack.DeliveryToken(_deliveryTokenUid).UpdateAsync(updateModel);

                Assert.IsTrue(response.IsSuccessStatusCode, $"Async update delivery token failed");

                var responseObject = response.OpenJObjectResponse();
                Assert.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                Assert.AreEqual(_deliveryTokenUid, tokenData["uid"]?.ToString(), "Token UID should match");
                Assert.AreEqual(updateModel.Name, tokenData["name"]?.ToString(), "Updated token name should match");

            }
            catch (Exception ex)
            {
                Assert.Fail($"Async update delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test007_Should_Query_All_Delivery_Tokens()
        {
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                ContentstackResponse response = _stack.DeliveryToken().Query().Find();

                Assert.IsTrue(response.IsSuccessStatusCode, $"Query delivery tokens failed");

                var responseObject = response.OpenJObjectResponse();
                Assert.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JArray;
                Assert.IsTrue(tokens.Count > 0, "Should have at least one delivery token");

                bool foundTestToken = false;
                foreach (var token in tokens)
                {
                    if (token["uid"]?.ToString() == _deliveryTokenUid)
                    {
                        foundTestToken = true;
                        break;
                    }
                }

                Assert.IsTrue(foundTestToken, "Test token should be found in query results");

            }
            catch (Exception ex)
            {
                Assert.Fail($"Query delivery tokens test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Should_Query_Delivery_Tokens_With_Parameters()
        {
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                var parameters = new ParameterCollection();
                parameters.Add("limit", "5");
                parameters.Add("skip", "0");

                ContentstackResponse response = _stack.DeliveryToken().Query().Limit(5).Skip(0).Find();

                Assert.IsTrue(response.IsSuccessStatusCode, $"Query delivery tokens with parameters failed");

                var responseObject = response.OpenJObjectResponse();
                Assert.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JArray;
                Assert.IsTrue(tokens.Count <= 5, "Should respect limit parameter");

            }
            catch (Exception ex)
            {
                Assert.Fail($"Query delivery tokens with parameters test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Create_Token_With_Multiple_Environments()
        {
            try
            {
                string secondEnvironmentUid = "test_delivery_environment_2";
                await CreateTestEnvironment(secondEnvironmentUid);

                var multiEnvTokenModel = new DeliveryTokenModel
                {
                    Name = "Multi Environment Delivery Token",
                    Description = "Token with multiple environment access",
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "environment",
                            Environments = new List<string> { _testEnvironmentUid, secondEnvironmentUid },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        },
                        new DeliveryTokenScope
                        {
                            Module = "branch",
                            Branches = new List<string> { "main" },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        }
                    }
                };

                ContentstackResponse response = _stack.DeliveryToken().Create(multiEnvTokenModel);

                Assert.IsTrue(response.IsSuccessStatusCode, $"Create multi-environment delivery token failed");

                var responseObject = response.OpenJObjectResponse();
                var tokenData = responseObject["token"] as JObject;
                Assert.IsNotNull(tokenData["uid"], "Token should have UID");

                string multiEnvTokenUid = tokenData["uid"]?.ToString();

                var scope = tokenData["scope"] as JArray;
                Assert.IsNotNull(scope, "Token should have scope");
                Assert.IsTrue(scope.Count > 0, "Token should have at least one scope");

                if (!string.IsNullOrEmpty(multiEnvTokenUid))
                {
                    await _stack.DeliveryToken(multiEnvTokenUid).DeleteAsync();
                }

                await CleanupTestEnvironment(secondEnvironmentUid);

            }
            catch (Exception ex)
            {
                Assert.Fail($"Multi-environment delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test011_Should_Create_Token_With_Complex_Scope()
        {
            try
            {
                var complexScopeTokenModel = new DeliveryTokenModel
                {
                    Name = "Complex Scope Delivery Token",
                    Description = "Token with complex scope configuration",
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "environment",
                            Environments = new List<string> { _testEnvironmentUid },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        },
                        new DeliveryTokenScope
                        {
                            Module = "branch",
                            Branches = new List<string> { "main" },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        }
                    }
                };

                ContentstackResponse response = _stack.DeliveryToken().Create(complexScopeTokenModel);

                Assert.IsTrue(response.IsSuccessStatusCode, $"Create complex scope delivery token failed");

                var responseObject = response.OpenJObjectResponse();
                var tokenData = responseObject["token"] as JObject;
                Assert.IsNotNull(tokenData["uid"], "Token should have UID");

                string complexScopeTokenUid = tokenData["uid"]?.ToString();

                // Verify multiple scopes
                var scope = tokenData["scope"] as JArray;
                Assert.IsNotNull(scope, "Token should have scope");
                Assert.IsTrue(scope.Count >= 2, "Token should have multiple scopes");

                // Clean up the complex scope token
                if (!string.IsNullOrEmpty(complexScopeTokenUid))
                {
                    await _stack.DeliveryToken(complexScopeTokenUid).DeleteAsync();
                }

            }
            catch (Exception ex)
            {
                Assert.Fail($"Complex scope delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test012_Should_Create_Token_With_UI_Structure()
        {
            try
            {
                // Test with the exact structure from UI as provided by user
                var uiStructureTokenModel = new DeliveryTokenModel
                {
                    Name = "UI Structure Test Token",
                    Description = "", // Empty description as in UI example
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "environment",
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            },
                            Environments = new List<string> { _testEnvironmentUid }
                        },
                        new DeliveryTokenScope
                        {
                            Module = "branch",
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            },
                            Branches = new List<string> { "main" }
                        }
                    }
                };

                ContentstackResponse response = _stack.DeliveryToken().Create(uiStructureTokenModel);

                Assert.IsTrue(response.IsSuccessStatusCode, $"Create UI structure delivery token failed");

                var responseObject = response.OpenJObjectResponse();
                var tokenData = responseObject["token"] as JObject;
                Assert.IsNotNull(tokenData["uid"], "Token should have UID");
                Assert.AreEqual(uiStructureTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match");

                // Verify the scope structure matches UI format
                var scope = tokenData["scope"] as JArray;
                Assert.IsNotNull(scope, "Token should have scope");
                Assert.IsTrue(scope.Count == 2, "Token should have 2 scope modules (environment and branch)");

                string uiTokenUid = tokenData["uid"]?.ToString();

                // Clean up the UI structure token
                if (!string.IsNullOrEmpty(uiTokenUid))
                {
                    await _stack.DeliveryToken(uiTokenUid).DeleteAsync();
                }

            }
            catch (Exception ex)
            {
                Assert.Fail($"UI structure delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test015_Should_Query_Delivery_Tokens_Async()
        {
            try
            {
                // Ensure we have at least one token
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                ContentstackResponse response = await _stack.DeliveryToken().Query().FindAsync();

                Assert.IsTrue(response.IsSuccessStatusCode, $"Async query delivery tokens failed: {response.OpenResponse()}");

                var responseObject = response.OpenJObjectResponse();
                Assert.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JArray;
                Assert.IsTrue(tokens.Count > 0, "Should have at least one delivery token");

                bool foundTestToken = false;
                foreach (var token in tokens)
                {
                    if (token["uid"]?.ToString() == _deliveryTokenUid)
                    {
                        foundTestToken = true;
                        break;
                    }
                }

                Assert.IsTrue(foundTestToken, "Test token should be found in async query results");

            }
            catch (Exception ex)
            {
                Assert.Fail($"Async query delivery tokens test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test016_Should_Create_Token_With_Empty_Description()
        {
            try
            {
                var emptyDescTokenModel = new DeliveryTokenModel
                {
                    Name = "Empty Description Token",
                    Description = "", // Empty description
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "environment",
                            Environments = new List<string> { _testEnvironmentUid },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        },
                        new DeliveryTokenScope
                        {
                            Module = "branch",
                            Branches = new List<string> { "main" },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        }
                    }
                };

                ContentstackResponse response = _stack.DeliveryToken().Create(emptyDescTokenModel);

                Assert.IsTrue(response.IsSuccessStatusCode, $"Create token with empty description failed: {response.OpenResponse()}");

                var responseObject = response.OpenJObjectResponse();
                var tokenData = responseObject["token"] as JObject;
                Assert.IsNotNull(tokenData["uid"], "Token should have UID");
                Assert.AreEqual(emptyDescTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match");

                string emptyDescTokenUid = tokenData["uid"]?.ToString();

                // Clean up the empty description token
                if (!string.IsNullOrEmpty(emptyDescTokenUid))
                {
                    await _stack.DeliveryToken(emptyDescTokenUid).DeleteAsync();
                }

            }
            catch (Exception ex)
            {
                Assert.Fail($"Empty description token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test017_Should_Validate_Environment_Scope_Requirement()
        {
            try
            {
                // Test that environment-only scope is rejected by API
                var envOnlyTokenModel = new DeliveryTokenModel
                {
                    Name = "Environment Only Token",
                    Description = "Token with only environment scope - should fail",
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "environment",
                            Environments = new List<string> { _testEnvironmentUid },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        }
                    }
                };

                ContentstackResponse response;
                try
                {
                    response = _stack.DeliveryToken().Create(envOnlyTokenModel);
                }
                catch (Exception ex)
                {
                    // If an exception is thrown, that's also acceptable - the API rejected the request
                    Console.WriteLine($"Environment-only token creation threw exception (expected): {ex.Message}");
                    return; // Test passes if exception is thrown
                }

                // If no exception was thrown, check the response status
                Assert.IsFalse(response.IsSuccessStatusCode, "Environment-only token should be rejected by API");
                Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                             response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity,
                    $"Expected 400 or 422 for environment-only token, got {response.StatusCode}");

            }
            catch (Exception ex)
            {
                Assert.Fail($"Environment scope validation test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test018_Should_Validate_Branch_Scope_Requirement()
        {
            try
            {
                // Test that branch-only scope is rejected by API
                var branchOnlyTokenModel = new DeliveryTokenModel
                {
                    Name = "Branch Only Token",
                    Description = "Token with only branch scope - should fail",
                    Scope = new List<DeliveryTokenScope>
                    {
                        new DeliveryTokenScope
                        {
                            Module = "branch",
                            Branches = new List<string> { "main" },
                            ACL = new Dictionary<string, string>
                            {
                                { "read", "true" }
                            }
                        }
                    }
                };

                ContentstackResponse response;
                try
                {
                    response = _stack.DeliveryToken().Create(branchOnlyTokenModel);
                }
                catch (Exception ex)
                {
                    // If an exception is thrown, that's also acceptable - the API rejected the request
                    Console.WriteLine($"Branch-only token creation threw exception (expected): {ex.Message}");
                    return; // Test passes if exception is thrown
                }

                // If no exception was thrown, check the response status
                Assert.IsFalse(response.IsSuccessStatusCode, "Branch-only token should be rejected by API");
                Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                             response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity,
                    $"Expected 400 or 422 for branch-only token, got {response.StatusCode}");

            }
            catch (Exception ex)
            {
                Assert.Fail($"Branch scope validation test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test019_Should_Delete_Delivery_Token()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                // Ensure we have a token to delete
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                string tokenUidToDelete = _deliveryTokenUid;
                Assert.IsNotNull(tokenUidToDelete, "Should have a valid token UID to delete");

                TestReportHelper.LogRequest("_stack.DeliveryToken(uid).Delete()", "DELETE",
                    $"https://{Contentstack.Client.contentstackOptions.Host}/v3/stacks/delivery_tokens/{tokenUidToDelete}");
                ContentstackResponse response = _stack.DeliveryToken(tokenUidToDelete).Delete();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode, response.StatusCode.ToString(),
                    sw.ElapsedMilliseconds, response.OpenResponse());

                TestReportHelper.LogAssertion(response.IsSuccessStatusCode, "Delete response is successful", type: "IsTrue");
                Assert.IsTrue(response.IsSuccessStatusCode, $"Delete delivery token failed: {response.OpenResponse()}");

                // Verify token is deleted by trying to fetch it
                try
                {
                    ContentstackResponse fetchResponse = _stack.DeliveryToken(tokenUidToDelete).Fetch();
                    Assert.IsFalse(fetchResponse.IsSuccessStatusCode, "Deleted token should not be fetchable");

                    // Verify the response indicates the token was not found
                    Assert.IsTrue(fetchResponse.StatusCode == System.Net.HttpStatusCode.NotFound ||
                                 fetchResponse.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity ||
                                 fetchResponse.StatusCode == System.Net.HttpStatusCode.BadRequest,
                        $"Expected 404, 422, or 400 for deleted token fetch, got {fetchResponse.StatusCode}");
                }
                catch (Exception ex)
                {
                    // Expected behavior - deleted token should throw exception when fetched
                    Console.WriteLine($"Expected exception when fetching deleted token: {ex.Message}");
                }


                _deliveryTokenUid = null;
            }
            catch (Exception ex)
            {
                Assert.Fail("Delete delivery token test failed", ex.Message);
            }
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            TestReportHelper.Flush();
            try
            {
                // Clean up delivery token if it still exists
                if (!string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    try
                    {
                        await _stack.DeliveryToken(_deliveryTokenUid).DeleteAsync();
                        Console.WriteLine($"Cleaned up delivery token: {_deliveryTokenUid}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to cleanup delivery token {_deliveryTokenUid}: {ex.Message}");
                    }
                }

                await CleanupTestEnvironment();
            }
            catch (Exception ex)
            {
                Assert.Fail("Cleanup failed", ex.Message);
            }
        }

        #region Helper Methods

        private async Task CreateTestEnvironment(string environmentUid = null)
        {
            try
            {
                string envUid = environmentUid ?? _testEnvironmentUid;

                var environmentModel = new EnvironmentModel
                {
                    Name = envUid,
                    Urls = new List<LocalesUrl>
                    {
                        new LocalesUrl
                        {
                            Locale = "en-us",
                            Url = "https://example.com"
                        }
                    },
                    DeployContent = true
                };

                ContentstackResponse response = _stack.Environment().Create(environmentModel);

                if (!response.IsSuccessStatusCode)
                {
                    // Environment might already exist, try to fetch it
                    response = _stack.Environment().Query().Find();
                    if (response.IsSuccessStatusCode)
                    {
                        var environments = response.OpenJObjectResponse()["environments"] as JArray;
                        if (environments?.Count > 0)
                        {
                            Console.WriteLine($"Test environment {envUid} already exists");
                            return;
                        }
                    }

                    Console.WriteLine($"Failed to create test environment {envUid}: {response.OpenResponse()}");
                    // Don't fail the test if environment creation fails - just log the error
                }
                else
                {
                    Console.WriteLine($"Created test environment: {envUid}");
                }
            }
            catch (Exception ex)
            {
                // Don't fail the test for environment creation issues - just log the error
                Console.WriteLine($"Error creating test environment: {ex.Message}");
            }
        }

        private async Task CleanupTestEnvironment(string environmentUid = null)
        {
            try
            {
                string envUid = environmentUid ?? _testEnvironmentUid;

                // First, find the environment UID
                ContentstackResponse queryResponse = _stack.Environment().Query().Find();

                if (queryResponse.IsSuccessStatusCode)
                {
                    var environments = queryResponse.OpenJObjectResponse()["environments"] as JArray;
                    if (environments?.Count > 0)
                    {
                        // Find the environment with matching name
                        foreach (var env in environments)
                        {
                            if (env["name"]?.ToString() == envUid)
                            {
                                string actualEnvUid = env["uid"]?.ToString();
                                if (!string.IsNullOrEmpty(actualEnvUid))
                                {
                                    try
                                    {
                                        ContentstackResponse deleteResponse = await _stack.Environment(actualEnvUid).DeleteAsync();
                                        if (deleteResponse.IsSuccessStatusCode)
                                        {
                                            Console.WriteLine($"Cleaned up test environment: {envUid}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Failed to cleanup test environment {envUid}: {deleteResponse.OpenResponse()}");
                                        }
                                    }
                                    catch (Exception deleteEx)
                                    {
                                        Console.WriteLine($"Exception during environment deletion {envUid}: {deleteEx.Message}");
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Don't fail the test for cleanup issues - just log the error
                Console.WriteLine($"Environment cleanup failed: {ex.Message}");
            }
        }

        #endregion
    }
}