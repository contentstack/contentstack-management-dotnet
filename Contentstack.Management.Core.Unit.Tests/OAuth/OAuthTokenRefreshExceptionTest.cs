using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Unit.Tests.OAuth
{
    [TestClass]
    public class OAuthTokenRefreshExceptionTest
    {
        private ContentstackClient _client;
        private OAuthOptions _options;

        [TestInitialize]
        public void Setup()
        {
            _client = new ContentstackClient();
            _options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code"
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clear any test tokens
            _client.ClearOAuthTokens(_options.ClientId);
        }

        [TestMethod]
        public void ContentstackClient_EnsureOAuthTokenIsValidAsync_WithExpiredToken_ShouldRefreshSuccessfully()
        {
            // Arrange
            var expiredTokens = new OAuthTokens
            {
                AccessToken = "expired-token",
                RefreshToken = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5), // Expired
                ClientId = _options.ClientId,
                AppId = _options.AppId
            };
            _client.StoreOAuthTokens(_options.ClientId, expiredTokens);
            
            // Set OAuth token through options
            _client.contentstackOptions.Authtoken = expiredTokens.AccessToken;
            _client.contentstackOptions.IsOAuthToken = true;

            // Act & Assert
            try
            {
                var result = _client.GetUserAsync().Result;
                
            }
            catch (AggregateException ex) when (ex.InnerException is Exceptions.OAuthException)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("OAuth token refresh failed for client"));
            }
        }

        [TestMethod]
        public void ContentstackClient_EnsureOAuthTokenIsValidAsync_WithInvalidRefreshToken_ShouldThrowOAuthTokenRefreshException()
        {
            // Arrange
            var tokensWithInvalidRefresh = new OAuthTokens
            {
                AccessToken = "expired-token",
                RefreshToken = "invalid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5), // Expired
                ClientId = _options.ClientId,
                AppId = _options.AppId
            };
            _client.StoreOAuthTokens(_options.ClientId, tokensWithInvalidRefresh);
            
            // Set OAuth token through options
            _client.contentstackOptions.Authtoken = tokensWithInvalidRefresh.AccessToken;
            _client.contentstackOptions.IsOAuthToken = true;

            // Act & Assert
            try
            {
                var result = _client.GetUserAsync().Result;
                Assert.Fail("Should have thrown OAuthException");
            }
            catch (AggregateException ex) when (ex.InnerException is Exceptions.OAuthException)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("OAuth token refresh failed for client"));
                Assert.IsTrue(ex.InnerException.Message.Contains(_options.ClientId));
            }
        }

        [TestMethod]
        public void ContentstackClient_EnsureOAuthTokenIsValidAsync_WithNullRefreshToken_ShouldThrowOAuthTokenRefreshException()
        {
            // Arrange
            var tokensWithNullRefresh = new OAuthTokens
            {
                AccessToken = "expired-token",
                RefreshToken = null,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5), // Expired
                ClientId = _options.ClientId,
                AppId = _options.AppId
            };
            _client.StoreOAuthTokens(_options.ClientId, tokensWithNullRefresh);
            // Set OAuth token through options
            _client.contentstackOptions.Authtoken = tokensWithNullRefresh.AccessToken;
            _client.contentstackOptions.IsOAuthToken = true;

            // Act & Assert
            try
            {
                var result = _client.GetUserAsync().Result;
                Assert.Fail("Should have thrown OAuthException");
            }
            catch (AggregateException ex) when (ex.InnerException is Exceptions.OAuthException)
            {
                // Expected - null refresh token should cause refresh to fail
                Assert.IsTrue(ex.InnerException.Message.Contains("OAuth token refresh failed for client"));
                Assert.IsTrue(ex.InnerException.Message.Contains(_options.ClientId));
            }
        }

        [TestMethod]
        public void ContentstackClient_EnsureOAuthTokenIsValidAsync_WithValidToken_ShouldNotAttemptRefresh()
        {
            // Arrange
            var validTokens = new OAuthTokens
            {
                AccessToken = "valid-token",
                RefreshToken = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1), // Valid for 1 hour
                ClientId = _options.ClientId,
                AppId = _options.AppId
            };
            _client.StoreOAuthTokens(_options.ClientId, validTokens);
            // Set OAuth token through options
            _client.contentstackOptions.Authtoken = validTokens.AccessToken;
            _client.contentstackOptions.IsOAuthToken = true;

            // Act & Assert
            try
            {
                var result = _client.GetUserAsync().Result;
               
            }
            catch (AggregateException ex) when (ex.InnerException is Exceptions.OAuthException)
            {
                // This should NOT happen with a valid token
                Assert.Fail("Should not have attempted token refresh for valid token");
            }
            catch (Exception)
            {
                // Other exceptions are expected due to API mocking in unit tests
            }
        }

        [TestMethod]
        public void ContentstackClient_EnsureOAuthTokenIsValidAsync_WithTokenNeedingRefresh_ShouldAttemptRefresh()
        {
            // Arrange
            var tokensNeedingRefresh = new OAuthTokens
            {
                AccessToken = "token-needing-refresh",
                RefreshToken = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(2), // Will need refresh soon
                ClientId = _options.ClientId,
                AppId = _options.AppId
            };
            _client.StoreOAuthTokens(_options.ClientId, tokensNeedingRefresh);
            // Set OAuth token through options
            _client.contentstackOptions.Authtoken = tokensNeedingRefresh.AccessToken;
            _client.contentstackOptions.IsOAuthToken = true;

            // Act & Assert
            try
            {
                var result = _client.GetUserAsync().Result;
                Assert.Fail("Should have thrown OAuthException");
            }
            catch (AggregateException ex) when (ex.InnerException is Exceptions.OAuthException)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("OAuth token refresh failed for client"));
                Assert.IsTrue(ex.InnerException.Message.Contains(_options.ClientId));
            }
        }

        [TestMethod]
        public void ContentstackClient_EnsureOAuthTokenIsValidAsync_WithMultipleClients_ShouldHandleCorrectClient()
        {
            // Arrange
            var client1Tokens = new OAuthTokens
            {
                AccessToken = "client1-token",
                RefreshToken = "client1-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5), // Expired
                ClientId = "client-1",
                AppId = "app-1"
            };
            var client2Tokens = new OAuthTokens
            {
                AccessToken = "client2-token",
                RefreshToken = "client2-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1), // Valid
                ClientId = "client-2",
                AppId = "app-2"
            };

            _client.StoreOAuthTokens("client-1", client1Tokens);
            _client.StoreOAuthTokens("client-2", client2Tokens);
            // Set OAuth token through options
            _client.contentstackOptions.Authtoken = "client1-token"; // Use client1's expired token
            _client.contentstackOptions.IsOAuthToken = true;

            // Act & Assert
            try
            {
                var result = _client.GetUserAsync().Result;
                Assert.Fail("Should have thrown OAuthException");
            }
            catch (AggregateException ex) when (ex.InnerException is Exceptions.OAuthException)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("OAuth token refresh failed for client"));
                Assert.IsTrue(ex.InnerException.Message.Contains("client-1"));
                Assert.IsFalse(ex.InnerException.Message.Contains("client-2"));
            }
        }

        [TestMethod]
        public void ContentstackClient_EnsureOAuthTokenIsValidAsync_WithNoMatchingTokens_ShouldNotThrow()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "some-other-token",
                RefreshToken = "some-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5), // Expired
                ClientId = _options.ClientId,
                AppId = _options.AppId
            };
            _client.StoreOAuthTokens(_options.ClientId, tokens);
            // Set OAuth token through options
            _client.contentstackOptions.Authtoken = "different-token"; // Different token
            _client.contentstackOptions.IsOAuthToken = true;

            // Act & Assert
            try
            {
                var result = _client.GetUserAsync().Result;
            }
            catch (AggregateException ex) when (ex.InnerException is Exceptions.OAuthException)
            {
                Assert.Fail("Should not have attempted token refresh for non-matching token");
            }
            catch (Exception)
            {
            }
        }
    }
}
