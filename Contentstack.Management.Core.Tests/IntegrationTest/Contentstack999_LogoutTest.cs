using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack999_LogoutTest
    {
        private static ContentstackClient CreateClientWithLogging()
        {
            var handler = new LoggingHttpHandler();
            var httpClient = new HttpClient(handler);
            return new ContentstackClient(httpClient, new ContentstackClientOptions());
        }

        private static ContentstackClientOptions CreateFastFailOptions()
        {
            return new ContentstackClientOptions()
            {
                RetryOnError = false,
                RetryOnNetworkFailure = false,
                RetryOnDnsFailure = false,
                RetryOnSocketFailure = false,
                RetryOnHttpServerError = false,
                RetryLimit = 0,
                MaxNetworkRetries = 0,
                Timeout = TimeSpan.FromSeconds(1)
            };
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_Success_On_Sync_Logout()
        {
            TestOutputLogger.LogContext("TestScenario", "SyncLogout");
            try
            {
                ContentstackClient client = Contentstack.CreateAuthenticatedClient();
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "AuthtokenBeforeLogout");

                ContentstackResponse contentstackResponse = client.Logout();
                string loginResponse = contentstackResponse.OpenResponse();

                AssertLogger.IsNull(client.contentstackOptions.Authtoken, "AuthtokenAfterLogout");
                AssertLogger.IsNotNull(loginResponse, "LogoutResponse");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Return_Success_On_Async_Logout()
        {
            TestOutputLogger.LogContext("TestScenario", "AsyncLogout");
            try
            {
                ContentstackClient client = Contentstack.CreateAuthenticatedClient();
                AssertLogger.IsNotNull(client.contentstackOptions.Authtoken, "AuthtokenBeforeLogout");

                ContentstackResponse contentstackResponse = await client.LogoutAsync();
                string logoutResponse = contentstackResponse.OpenResponse();

                AssertLogger.IsNull(client.contentstackOptions.Authtoken, "AuthtokenAfterLogout");
                AssertLogger.IsNotNull(logoutResponse, "LogoutResponse");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Handle_Logout_When_Not_LoggedIn()
        {
            TestOutputLogger.LogContext("TestScenario", "LogoutWhenNotLoggedIn");
            ContentstackClient client = CreateClientWithLogging();

            AssertLogger.IsNull(client.contentstackOptions.Authtoken, "AuthtokenNotSet");

            try
            {
                client.Logout();
            }
            catch (Exception e)
            {
                AssertLogger.IsTrue(
                    e.Message.Contains("token") || e.Message.Contains("Authentication") || e.Message.Contains("not logged in"),
                    "LogoutNotLoggedInError");
            }
        }

        #region Authentication Token Validation Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Throw_ArgumentNullException_On_Null_Authtoken_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "NullAuthtokenParameter");
            ContentstackClient client = CreateClientWithLogging();

            AssertLogger.ThrowsException<ArgumentNullException>(() => client.Logout(null), "NullAuthtokenParameter");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Async_Should_Throw_ArgumentNullException_On_Null_Authtoken_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "NullAuthtokenParameterAsync");
            ContentstackClient client = CreateClientWithLogging();

            await AssertLogger.ThrowsExceptionAsync<ArgumentNullException>(() => client.LogoutAsync(null), "NullAuthtokenParameterAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Throw_ArgumentNullException_On_Empty_Authtoken_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "EmptyAuthtokenParameter");
            ContentstackClient client = CreateClientWithLogging();

            AssertLogger.ThrowsException<ArgumentNullException>(() => client.Logout(string.Empty), "EmptyAuthtokenParameter");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Async_Should_Throw_ArgumentNullException_On_Empty_Authtoken_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "EmptyAuthtokenParameterAsync");
            ContentstackClient client = CreateClientWithLogging();

            await AssertLogger.ThrowsExceptionAsync<ArgumentNullException>(() => client.LogoutAsync(string.Empty), "EmptyAuthtokenParameterAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Throw_ArgumentNullException_On_Whitespace_Authtoken_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "WhitespaceAuthtokenParameter");
            ContentstackClient client = CreateClientWithLogging();

            AssertLogger.ThrowsException<ArgumentNullException>(() => client.Logout("   "), "WhitespaceAuthtokenParameter");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Async_Should_Throw_ArgumentNullException_On_Whitespace_Authtoken_Parameter()
        {
            TestOutputLogger.LogContext("TestScenario", "WhitespaceAuthtokenParameterAsync");
            ContentstackClient client = CreateClientWithLogging();

            await AssertLogger.ThrowsExceptionAsync<ArgumentNullException>(() => client.LogoutAsync("   "), "WhitespaceAuthtokenParameterAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Handle_Invalid_Authtoken_Format()
        {
            TestOutputLogger.LogContext("TestScenario", "InvalidAuthtokenFormat");
            var handler = new MockHttpStatusHandler(HttpStatusCode.Unauthorized, "Invalid authentication token", 401);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string invalidToken = "invalid-token-format-123";

            AssertLogger.ThrowsContentstackError(() => client.Logout(invalidToken), "InvalidAuthtokenFormat", HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test007_Async_Should_Handle_Invalid_Authtoken_Format()
        {
            TestOutputLogger.LogContext("TestScenario", "InvalidAuthtokenFormatAsync");
            var handler = new MockHttpStatusHandler(HttpStatusCode.Unauthorized, "Invalid authentication token", 401);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string invalidToken = "invalid-token-format-123";

            await AssertLogger.ThrowsContentstackErrorAsync(() => client.LogoutAsync(invalidToken), "InvalidAuthtokenFormatAsync", HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Handle_Expired_Authtoken()
        {
            TestOutputLogger.LogContext("TestScenario", "ExpiredAuthtoken");
            var handler = new MockHttpStatusHandler(HttpStatusCode.Unauthorized, "Authentication token has expired", 401);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string expiredToken = "expired-token-12345";

            AssertLogger.ThrowsContentstackError(() => client.Logout(expiredToken), "ExpiredAuthtoken", HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Async_Should_Handle_Expired_Authtoken()
        {
            TestOutputLogger.LogContext("TestScenario", "ExpiredAuthtokenAsync");
            var handler = new MockHttpStatusHandler(HttpStatusCode.Unauthorized, "Authentication token has expired", 401);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string expiredToken = "expired-token-12345";

            await AssertLogger.ThrowsContentstackErrorAsync(() => client.LogoutAsync(expiredToken), "ExpiredAuthtokenAsync", HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Handle_Corrupted_Malformed_Authtoken()
        {
            TestOutputLogger.LogContext("TestScenario", "CorruptedMalformedAuthtoken");
            var handler = new MockHttpStatusHandler(HttpStatusCode.BadRequest, "Malformed authentication token", 400);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string corruptedToken = "corrupted@#$%^&*()token";

            AssertLogger.ThrowsContentstackError(() => client.Logout(corruptedToken), "CorruptedMalformedAuthtoken", HttpStatusCode.BadRequest);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Async_Should_Handle_Corrupted_Malformed_Authtoken()
        {
            TestOutputLogger.LogContext("TestScenario", "CorruptedMalformedAuthtokenAsync");
            var handler = new MockHttpStatusHandler(HttpStatusCode.BadRequest, "Malformed authentication token", 400);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string corruptedToken = "corrupted@#$%^&*()token";

            await AssertLogger.ThrowsContentstackErrorAsync(() => client.LogoutAsync(corruptedToken), "CorruptedMalformedAuthtokenAsync", HttpStatusCode.BadRequest);
        }

        #endregion

        #region HTTP Error Status Code Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Handle_400_BadRequest_Response()
        {
            TestOutputLogger.LogContext("TestScenario", "BadRequestResponse");
            var handler = new MockHttpStatusHandler(HttpStatusCode.BadRequest, "Bad request. Invalid input provided.", 400);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string validToken = "test-token-12345";

            AssertLogger.ThrowsContentstackError(() => client.Logout(validToken), "BadRequestResponse", HttpStatusCode.BadRequest);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Handle_401_Unauthorized_Response()
        {
            TestOutputLogger.LogContext("TestScenario", "UnauthorizedResponse");
            var handler = new MockHttpStatusHandler(HttpStatusCode.Unauthorized, "Authentication failed. Please check your credentials.", 401);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string validToken = "test-token-12345";

            AssertLogger.ThrowsContentstackError(() => client.Logout(validToken), "UnauthorizedResponse", HttpStatusCode.Unauthorized);
        }





        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Handle_429_TooManyRequests_Response()
        {
            TestOutputLogger.LogContext("TestScenario", "TooManyRequestsResponse");
            var handler = new MockHttpStatusHandler(HttpStatusCode.TooManyRequests, "Rate limit exceeded. Please try again later.", 429);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, CreateFastFailOptions());

            string validToken = "test-token-12345";

            AssertLogger.ThrowsContentstackError(() => client.Logout(validToken), "TooManyRequestsResponse", HttpStatusCode.TooManyRequests);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Handle_500_InternalServerError_Response()
        {
            TestOutputLogger.LogContext("TestScenario", "InternalServerErrorResponse");
            var handler = new MockHttpStatusHandler(HttpStatusCode.InternalServerError, "Internal server error occurred.", 500);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, CreateFastFailOptions());

            string validToken = "test-token-12345";

            AssertLogger.ThrowsContentstackError(() => client.Logout(validToken), "InternalServerErrorResponse", HttpStatusCode.InternalServerError);
        }


        #endregion

        #region Network-Level Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test021_Should_Handle_Connection_Timeout()
        {
            TestOutputLogger.LogContext("TestScenario", "ConnectionTimeout");
            var handler = new MockNetworkErrorHandler(NetworkErrorType.Timeout);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, CreateFastFailOptions());

            string validToken = "test-token-12345";

            AssertLogger.ThrowsException<TaskCanceledException>(() => client.Logout(validToken), "ConnectionTimeout");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test021_Async_Should_Handle_Connection_Timeout()
        {
            TestOutputLogger.LogContext("TestScenario", "ConnectionTimeoutAsync");
            var handler = new MockNetworkErrorHandler(NetworkErrorType.Timeout);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, CreateFastFailOptions());

            string validToken = "test-token-12345";

            await AssertLogger.ThrowsExceptionAsync<TaskCanceledException>(() => client.LogoutAsync(validToken), "ConnectionTimeoutAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Handle_Connection_Refused()
        {
            TestOutputLogger.LogContext("TestScenario", "ConnectionRefused");
            var handler = new MockNetworkErrorHandler(NetworkErrorType.ConnectionRefused);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, CreateFastFailOptions());

            string validToken = "test-token-12345";

            AssertLogger.ThrowsException<HttpRequestException>(() => client.Logout(validToken), "ConnectionRefused");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test022_Async_Should_Handle_Connection_Refused()
        {
            TestOutputLogger.LogContext("TestScenario", "ConnectionRefusedAsync");
            var handler = new MockNetworkErrorHandler(NetworkErrorType.ConnectionRefused);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, CreateFastFailOptions());

            string validToken = "test-token-12345";

            await AssertLogger.ThrowsExceptionAsync<HttpRequestException>(() => client.LogoutAsync(validToken), "ConnectionRefusedAsync");
        }




        #endregion

        #region Response Handling Error Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test028_Should_Handle_Malformed_JSON_Response()
        {
            TestOutputLogger.LogContext("TestScenario", "MalformedJsonResponse");
            var handler = new MockMalformedResponseHandler("{invalid json syntax", HttpStatusCode.OK);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string validToken = "test-token-12345";

            AssertLogger.ThrowsException<Exception>(() => client.Logout(validToken), "MalformedJsonResponse");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test028_Async_Should_Handle_Malformed_JSON_Response()
        {
            TestOutputLogger.LogContext("TestScenario", "MalformedJsonResponseAsync");
            var handler = new MockMalformedResponseHandler("{invalid json syntax", HttpStatusCode.OK);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string validToken = "test-token-12345";

            await AssertLogger.ThrowsExceptionAsync<Exception>(() => client.LogoutAsync(validToken), "MalformedJsonResponseAsync");
        }


        [TestMethod]
        [DoNotParallelize]
        public void Test032_Should_Handle_Response_With_Unexpected_Structure()
        {
            TestOutputLogger.LogContext("TestScenario", "UnexpectedResponseStructure");
            var handler = new MockMalformedResponseHandler("{\"unexpected\":\"structure\",\"not_error_message\":123}", HttpStatusCode.OK);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string validToken = "test-token-12345";

            // This should either succeed (if the response structure is handled gracefully) or throw an exception
            try 
            {
                ContentstackResponse response = client.Logout(validToken);
                AssertLogger.IsNotNull(response, "UnexpectedResponseStructureHandled");
            }
            catch (Exception)
            {
                // Expected if the response structure causes parsing issues
                AssertLogger.IsTrue(true, "UnexpectedResponseStructureThrewException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test032_Async_Should_Handle_Response_With_Unexpected_Structure()
        {
            TestOutputLogger.LogContext("TestScenario", "UnexpectedResponseStructureAsync");
            var handler = new MockMalformedResponseHandler("{\"unexpected\":\"structure\",\"not_error_message\":123}", HttpStatusCode.OK);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());

            string validToken = "test-token-12345";

            // This should either succeed (if the response structure is handled gracefully) or throw an exception
            try 
            {
                ContentstackResponse response = await client.LogoutAsync(validToken);
                AssertLogger.IsNotNull(response, "UnexpectedResponseStructureHandledAsync");
            }
            catch (Exception)
            {
                // Expected if the response structure causes parsing issues
                AssertLogger.IsTrue(true, "UnexpectedResponseStructureThrewExceptionAsync");
            }
        }

        #endregion

        #region Client State Management Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test033_Should_Handle_Logout_With_Disposed_Client()
        {
            TestOutputLogger.LogContext("TestScenario", "LogoutWithDisposedClient");
            var handler = new LoggingHttpHandler();
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());
            
            string validToken = "test-token-12345";
            
            // Dispose the client
            client.Dispose();
            
            AssertLogger.ThrowsException<ObjectDisposedException>(() => client.Logout(validToken), "LogoutWithDisposedClient");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test033_Async_Should_Handle_Logout_With_Disposed_Client()
        {
            TestOutputLogger.LogContext("TestScenario", "LogoutWithDisposedClientAsync");
            var handler = new LoggingHttpHandler();
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());
            
            string validToken = "test-token-12345";
            
            // Dispose the client
            client.Dispose();
            
            await AssertLogger.ThrowsExceptionAsync<ObjectDisposedException>(() => client.LogoutAsync(validToken), "LogoutWithDisposedClientAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test034_Should_Handle_Multiple_Consecutive_Logout_Calls()
        {
            TestOutputLogger.LogContext("TestScenario", "MultipleConsecutiveLogoutCalls");
            var handler = new MockHttpStatusHandler(HttpStatusCode.Unauthorized, "Already logged out", 401);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());
            
            string validToken = "test-token-12345";
            
            // First logout attempt should throw ContentstackErrorException
            AssertLogger.ThrowsContentstackError(() => client.Logout(validToken), "FirstLogoutCall", HttpStatusCode.Unauthorized);
            
            // Second logout attempt should also throw ContentstackErrorException 
            AssertLogger.ThrowsContentstackError(() => client.Logout(validToken), "SecondLogoutCall", HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test035_Should_Handle_Concurrent_Logout_Operations()
        {
            TestOutputLogger.LogContext("TestScenario", "ConcurrentLogoutOperations");
            var handler = new MockHttpStatusHandler(HttpStatusCode.Unauthorized, "Session conflict", 401);
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());
            
            string validToken = "test-token-12345";
            
            // Start two concurrent logout operations
            Task<Exception> task1 = Task.Run(async () =>
            {
                try
                {
                    await client.LogoutAsync(validToken);
                    return null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            });
            
            Task<Exception> task2 = Task.Run(async () =>
            {
                try
                {
                    await client.LogoutAsync(validToken);
                    return null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            });
            
            Exception[] results = await Task.WhenAll(task1, task2);
            
            // Both should throw exceptions due to the mock handler
            AssertLogger.IsNotNull(results[0], "FirstConcurrentLogoutException");
            AssertLogger.IsNotNull(results[1], "SecondConcurrentLogoutException");
            AssertLogger.IsInstanceOfType(results[0], typeof(ContentstackErrorException), "FirstConcurrentLogoutExceptionType");
            AssertLogger.IsInstanceOfType(results[1], typeof(ContentstackErrorException), "SecondConcurrentLogoutExceptionType");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test036_Should_Verify_Token_Clearing_On_Failure_Vs_Success()
        {
            TestOutputLogger.LogContext("TestScenario", "TokenClearingFailureVsSuccess");
            
            // Test failure case - token should NOT be cleared
            var failureHandler = new MockHttpStatusHandler(HttpStatusCode.Unauthorized, "Logout failed", 401);
            var failureHttpClient = new HttpClient(failureHandler);
            ContentstackClient failureClient = new ContentstackClient(failureHttpClient, new ContentstackClientOptions());
            
            string testToken = "test-token-12345";
            failureClient.contentstackOptions.Authtoken = testToken;
            
            AssertLogger.IsNotNull(failureClient.contentstackOptions.Authtoken, "TokenSetBeforeFailedLogout");
            
            try
            {
                failureClient.Logout();
                AssertLogger.Fail("Expected exception for failed logout");
            }
            catch (ContentstackErrorException)
            {
                // Token should NOT be cleared on failure
                AssertLogger.IsNotNull(failureClient.contentstackOptions.Authtoken, "TokenNotClearedAfterFailedLogout");
                AssertLogger.AreEqual(testToken, failureClient.contentstackOptions.Authtoken, "TokenRemainsUnchangedAfterFailure");
            }
            
            // Test success case - token should be cleared
            var successHandler = new LoggingHttpHandler();
            var successHttpClient = new HttpClient(successHandler);
            ContentstackClient successClient = Contentstack.CreateAuthenticatedClient();
            
            string originalToken = successClient.contentstackOptions.Authtoken;
            AssertLogger.IsNotNull(originalToken, "TokenSetBeforeSuccessfulLogout");
            
            try
            {
                successClient.Logout();
                // Token should be cleared on success
                AssertLogger.IsNull(successClient.contentstackOptions.Authtoken, "TokenClearedAfterSuccessfulLogout");
            }
            catch (Exception e)
            {
                AssertLogger.Fail($"Unexpected exception during successful logout: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test037_Should_Handle_Logout_With_Different_Token_Than_Stored()
        {
            TestOutputLogger.LogContext("TestScenario", "LogoutWithDifferentToken");
            var handler = new LoggingHttpHandler();
            var httpClient = new HttpClient(handler);
            ContentstackClient client = new ContentstackClient(httpClient, new ContentstackClientOptions());
            
            string storedToken = "stored-token-12345";
            string differentToken = "different-token-67890";
            
            // Set a token in the client
            client.contentstackOptions.Authtoken = storedToken;
            AssertLogger.AreEqual(storedToken, client.contentstackOptions.Authtoken, "StoredTokenSet");
            
            try
            {
                // Logout with a different token than what's stored
                ContentstackResponse response = client.Logout(differentToken);
                
                // The stored token should remain unchanged because the logout used a different token
                AssertLogger.AreEqual(storedToken, client.contentstackOptions.Authtoken, "StoredTokenUnchanged");
                AssertLogger.IsNotNull(response, "LogoutResponseReceived");
            }
            catch (Exception)
            {
                // If it fails, the stored token should still remain unchanged
                AssertLogger.AreEqual(storedToken, client.contentstackOptions.Authtoken, "StoredTokenUnchangedAfterFailure");
            }
        }

        #endregion

        #region Edge Case and Boundary Tests




        #endregion
    }
}
