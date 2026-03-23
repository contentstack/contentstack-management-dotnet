using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Token;
using Contentstack.Management.Core.Tests.Helpers;
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
            try
            {
                // First, ensure the client is logged in
                try
                {
                    ContentstackResponse loginResponse = Contentstack.Client.Login(Contentstack.Credential);
                    if (!loginResponse.IsSuccessStatusCode)
                    {
                        AssertLogger.Fail($"Login failed: {loginResponse.OpenResponse()}");
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
                AssertLogger.Fail($"Initialize failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test001_Should_Create_Delivery_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test001_Should_Create_Delivery_Token");
            try
            {
                ContentstackResponse response = _stack.DeliveryToken().Create(_testTokenModel);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create delivery token failed", "CreateDeliveryTokenSuccess");

                var responseObject = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");
                AssertLogger.AreEqual(_testTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match", "TokenName");
                AssertLogger.AreEqual(_testTokenModel.Description, tokenData["description"]?.ToString(), "Token description should match", "TokenDescription");

                _deliveryTokenUid = tokenData["uid"]?.ToString();
                AssertLogger.IsNotNull(_deliveryTokenUid, "Delivery token UID should not be null");

                TestOutputLogger.LogContext("DeliveryTokenUid", _deliveryTokenUid ?? "");
                Console.WriteLine($"Created delivery token with UID: {_deliveryTokenUid}");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Create delivery token test failed", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_Delivery_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test002_Should_Create_Delivery_Token_Async");
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

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Async create delivery token failed", "AsyncCreateSuccess");

                var responseObject = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");
                AssertLogger.AreEqual(asyncTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match", "AsyncTokenName");

                string asyncTokenUid = tokenData["uid"]?.ToString();
                TestOutputLogger.LogContext("AsyncCreatedTokenUid", asyncTokenUid ?? "");

                if (!string.IsNullOrEmpty(asyncTokenUid))
                {
                    await _stack.DeliveryToken(asyncTokenUid).DeleteAsync();
                }

            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Async create delivery token test failed", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test003_Should_Fetch_Delivery_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test003_Should_Fetch_Delivery_Token");
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                TestOutputLogger.LogContext("DeliveryTokenUid", _deliveryTokenUid ?? "");
                ContentstackResponse response = _stack.DeliveryToken(_deliveryTokenUid).Fetch();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Fetch delivery token failed", "FetchSuccess");

                var responseObject = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                AssertLogger.AreEqual(_deliveryTokenUid, tokenData["uid"]?.ToString(), "Token UID should match", "TokenUid");
                AssertLogger.AreEqual(_testTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match", "TokenName");
                AssertLogger.IsNotNull(tokenData["token"], "Token should have access token");

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Fetch delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Fetch_Delivery_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test004_Should_Fetch_Delivery_Token_Async");
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                TestOutputLogger.LogContext("DeliveryTokenUid", _deliveryTokenUid ?? "");
                ContentstackResponse response = await _stack.DeliveryToken(_deliveryTokenUid).FetchAsync();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Async fetch delivery token failed", "AsyncFetchSuccess");

                var responseObject = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                AssertLogger.AreEqual(_deliveryTokenUid, tokenData["uid"]?.ToString(), "Token UID should match", "TokenUid");
                AssertLogger.IsNotNull(tokenData["token"], "Token should have access token");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Async fetch delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Update_Delivery_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test005_Should_Update_Delivery_Token");
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                TestOutputLogger.LogContext("DeliveryTokenUid", _deliveryTokenUid ?? "");
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

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Update delivery token failed", "UpdateSuccess");

                var responseObject = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                AssertLogger.AreEqual(_deliveryTokenUid, tokenData["uid"]?.ToString(), "Token UID should match", "TokenUid");
                AssertLogger.AreEqual(updateModel.Name, tokenData["name"]?.ToString(), "Updated token name should match", "UpdatedTokenName");
                AssertLogger.AreEqual(updateModel.Description, tokenData["description"]?.ToString(), "Updated token description should match", "UpdatedTokenDescription");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Update delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_Delivery_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test006_Should_Update_Delivery_Token_Async");
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                TestOutputLogger.LogContext("DeliveryTokenUid", _deliveryTokenUid ?? "");
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

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Async update delivery token failed", "AsyncUpdateSuccess");

                var responseObject = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JObject;
                AssertLogger.AreEqual(_deliveryTokenUid, tokenData["uid"]?.ToString(), "Token UID should match", "TokenUid");
                AssertLogger.AreEqual(updateModel.Name, tokenData["name"]?.ToString(), "Updated token name should match", "UpdatedTokenName");

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Async update delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test007_Should_Query_All_Delivery_Tokens()
        {
            TestOutputLogger.LogContext("TestScenario", "Test007_Should_Query_All_Delivery_Tokens");
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                TestOutputLogger.LogContext("DeliveryTokenUid", _deliveryTokenUid ?? "");
                ContentstackResponse response = _stack.DeliveryToken().Query().Find();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Query delivery tokens failed", "QuerySuccess");

                var responseObject = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JArray;
                AssertLogger.IsTrue(tokens.Count > 0, "Should have at least one delivery token", "TokensCountGreaterThanZero");

                bool foundTestToken = false;
                foreach (var token in tokens)
                {
                    if (token["uid"]?.ToString() == _deliveryTokenUid)
                    {
                        foundTestToken = true;
                        break;
                    }
                }

                AssertLogger.IsTrue(foundTestToken, "Test token should be found in query results", "TestTokenFoundInQuery");

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Query delivery tokens test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Should_Query_Delivery_Tokens_With_Parameters()
        {
            TestOutputLogger.LogContext("TestScenario", "Test008_Should_Query_Delivery_Tokens_With_Parameters");
            try
            {
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                TestOutputLogger.LogContext("DeliveryTokenUid", _deliveryTokenUid ?? "");
                var parameters = new ParameterCollection();
                parameters.Add("limit", "5");
                parameters.Add("skip", "0");

                ContentstackResponse response = _stack.DeliveryToken().Query().Limit(5).Skip(0).Find();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Query delivery tokens with parameters failed", "QueryWithParamsSuccess");

                var responseObject = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JArray;
                AssertLogger.IsTrue(tokens.Count <= 5, "Should respect limit parameter", "RespectLimitParam");

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Query delivery tokens with parameters test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Create_Token_With_Multiple_Environments()
        {
            TestOutputLogger.LogContext("TestScenario", "Test009_Should_Create_Token_With_Multiple_Environments");
            try
            {
                string secondEnvironmentUid = "test_delivery_environment_2";
                await CreateTestEnvironment(secondEnvironmentUid);
                TestOutputLogger.LogContext("SecondEnvironmentUid", secondEnvironmentUid);

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

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create multi-environment delivery token failed", "MultiEnvCreateSuccess");

                var responseObject = response.OpenJObjectResponse();
                var tokenData = responseObject["token"] as JObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");

                string multiEnvTokenUid = tokenData["uid"]?.ToString();
                TestOutputLogger.LogContext("MultiEnvTokenUid", multiEnvTokenUid ?? "");

                var scope = tokenData["scope"] as JArray;
                AssertLogger.IsNotNull(scope, "Token should have scope");
                AssertLogger.IsTrue(scope.Count > 0, "Token should have at least one scope", "ScopeCount");

                if (!string.IsNullOrEmpty(multiEnvTokenUid))
                {
                    await _stack.DeliveryToken(multiEnvTokenUid).DeleteAsync();
                }

                await CleanupTestEnvironment(secondEnvironmentUid);

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Multi-environment delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test011_Should_Create_Token_With_Complex_Scope()
        {
            TestOutputLogger.LogContext("TestScenario", "Test011_Should_Create_Token_With_Complex_Scope");
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

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create complex scope delivery token failed", "ComplexScopeCreateSuccess");

                var responseObject = response.OpenJObjectResponse();
                var tokenData = responseObject["token"] as JObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");

                string complexScopeTokenUid = tokenData["uid"]?.ToString();
                TestOutputLogger.LogContext("ComplexScopeTokenUid", complexScopeTokenUid ?? "");

                // Verify multiple scopes
                var scope = tokenData["scope"] as JArray;
                AssertLogger.IsNotNull(scope, "Token should have scope");
                AssertLogger.IsTrue(scope.Count >= 2, "Token should have multiple scopes", "ScopeCountMultiple");

                // Clean up the complex scope token
                if (!string.IsNullOrEmpty(complexScopeTokenUid))
                {
                    await _stack.DeliveryToken(complexScopeTokenUid).DeleteAsync();
                }

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Complex scope delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test012_Should_Create_Token_With_UI_Structure()
        {
            TestOutputLogger.LogContext("TestScenario", "Test012_Should_Create_Token_With_UI_Structure");
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

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create UI structure delivery token failed", "UIStructureCreateSuccess");

                var responseObject = response.OpenJObjectResponse();
                var tokenData = responseObject["token"] as JObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");
                AssertLogger.AreEqual(uiStructureTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match", "UITokenName");

                // Verify the scope structure matches UI format
                var scope = tokenData["scope"] as JArray;
                AssertLogger.IsNotNull(scope, "Token should have scope");
                AssertLogger.IsTrue(scope.Count == 2, "Token should have 2 scope modules (environment and branch)", "UIScopeCount");

                string uiTokenUid = tokenData["uid"]?.ToString();
                TestOutputLogger.LogContext("UITokenUid", uiTokenUid ?? "");

                // Clean up the UI structure token
                if (!string.IsNullOrEmpty(uiTokenUid))
                {
                    await _stack.DeliveryToken(uiTokenUid).DeleteAsync();
                }

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"UI structure delivery token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test015_Should_Query_Delivery_Tokens_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test015_Should_Query_Delivery_Tokens_Async");
            try
            {
                // Ensure we have at least one token
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                TestOutputLogger.LogContext("DeliveryTokenUid", _deliveryTokenUid ?? "");
                ContentstackResponse response = await _stack.DeliveryToken().Query().FindAsync();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Async query delivery tokens failed: {response.OpenResponse()}", "AsyncQuerySuccess");

                var responseObject = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JArray;
                AssertLogger.IsTrue(tokens.Count > 0, "Should have at least one delivery token", "AsyncTokensCount");

                bool foundTestToken = false;
                foreach (var token in tokens)
                {
                    if (token["uid"]?.ToString() == _deliveryTokenUid)
                    {
                        foundTestToken = true;
                        break;
                    }
                }

                AssertLogger.IsTrue(foundTestToken, "Test token should be found in async query results", "TestTokenFoundInAsyncQuery");

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Async query delivery tokens test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test016_Should_Create_Token_With_Empty_Description()
        {
            TestOutputLogger.LogContext("TestScenario", "Test016_Should_Create_Token_With_Empty_Description");
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

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Create token with empty description failed: {response.OpenResponse()}", "EmptyDescCreateSuccess");

                var responseObject = response.OpenJObjectResponse();
                var tokenData = responseObject["token"] as JObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");
                AssertLogger.AreEqual(emptyDescTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match", "EmptyDescTokenName");

                string emptyDescTokenUid = tokenData["uid"]?.ToString();
                TestOutputLogger.LogContext("EmptyDescTokenUid", emptyDescTokenUid ?? "");

                // Clean up the empty description token
                if (!string.IsNullOrEmpty(emptyDescTokenUid))
                {
                    await _stack.DeliveryToken(emptyDescTokenUid).DeleteAsync();
                }

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Empty description token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test017_Should_Validate_Environment_Scope_Requirement()
        {
            TestOutputLogger.LogContext("TestScenario", "Test017_Should_Validate_Environment_Scope_Requirement");
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
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Environment-only token should be rejected by API", "EnvOnlyRejected");
                AssertLogger.IsTrue(response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                             response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity,
                    $"Expected 400 or 422 for environment-only token, got {response.StatusCode}", "Expected400Or422");

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Environment scope validation test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test018_Should_Validate_Branch_Scope_Requirement()
        {
            TestOutputLogger.LogContext("TestScenario", "Test018_Should_Validate_Branch_Scope_Requirement");
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
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Branch-only token should be rejected by API", "BranchOnlyRejected");
                AssertLogger.IsTrue(response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                             response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity,
                    $"Expected 400 or 422 for branch-only token, got {response.StatusCode}", "Expected400Or422");

            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Branch scope validation test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test019_Should_Delete_Delivery_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test019_Should_Delete_Delivery_Token");
            try
            {
                // Ensure we have a token to delete
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                string tokenUidToDelete = _deliveryTokenUid;
                AssertLogger.IsNotNull(tokenUidToDelete, "Should have a valid token UID to delete");

                TestOutputLogger.LogContext("TokenUidToDelete", tokenUidToDelete ?? "");
                // Test synchronous delete
                ContentstackResponse response = _stack.DeliveryToken(tokenUidToDelete).Delete();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Delete delivery token failed: {response.OpenResponse()}", "DeleteSuccess");

                // Verify token is deleted by trying to fetch it
                try
                {
                    ContentstackResponse fetchResponse = _stack.DeliveryToken(tokenUidToDelete).Fetch();
                    AssertLogger.IsFalse(fetchResponse.IsSuccessStatusCode, "Deleted token should not be fetchable", "DeletedTokenNotFetchable");

                    // Verify the response indicates the token was not found
                    AssertLogger.IsTrue(fetchResponse.StatusCode == System.Net.HttpStatusCode.NotFound ||
                                 fetchResponse.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity ||
                                 fetchResponse.StatusCode == System.Net.HttpStatusCode.BadRequest,
                        $"Expected 404, 422, or 400 for deleted token fetch, got {fetchResponse.StatusCode}", "Expected404Or422Or400");
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
                AssertLogger.Fail("Delete delivery token test failed", ex.Message);
            }
        }

        [TestCleanup]
        public async Task Cleanup()
        {
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
                AssertLogger.Fail("Cleanup failed", ex.Message);
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
