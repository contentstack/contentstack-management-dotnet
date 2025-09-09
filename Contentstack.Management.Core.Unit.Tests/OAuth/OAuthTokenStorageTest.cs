using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Unit.Tests.OAuth
{
    [TestClass]
    public class OAuthTokenStorageTest
    {
        private const string TestClientId = "test-client-id";
        private ContentstackClient _client;

        [TestInitialize]
        public void Setup()
        {
            _client = new ContentstackClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clear any test tokens after each test
            _client.ClearOAuthTokens(TestClientId);
        }

        [TestMethod]
        public void OAuthTokenStorage_SetAndGetTokens_ShouldWork()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                RefreshToken = "test-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                OrganizationUid = "test-org-uid",
                UserUid = "test-user-uid",
                ClientId = TestClientId
            };

            // Act
            _client.StoreOAuthTokens(TestClientId, tokens);
            var retrievedTokens = _client.GetOAuthTokens(TestClientId);

            // Assert
            Assert.IsNotNull(retrievedTokens);
            Assert.AreEqual("test-access-token", retrievedTokens.AccessToken);
            Assert.AreEqual("test-refresh-token", retrievedTokens.RefreshToken);
            Assert.AreEqual("test-org-uid", retrievedTokens.OrganizationUid);
            Assert.AreEqual("test-user-uid", retrievedTokens.UserUid);
            Assert.AreEqual(TestClientId, retrievedTokens.ClientId);
        }

        [TestMethod]
        public void OAuthTokenStorage_GetTokens_WithNonExistentClientId_ShouldReturnNull()
        {
            // Act
            var tokens = _client.GetOAuthTokens("non-existent-client-id");

            // Assert
            Assert.IsNull(tokens);
        }

        [TestMethod]
        public void OAuthTokenStorage_HasTokens_WithExistingTokens_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ClientId = TestClientId
            };
            _client.StoreOAuthTokens(TestClientId, tokens);

            // Act & Assert
            Assert.IsTrue(_client.HasOAuthTokens(TestClientId));
        }

        [TestMethod]
        public void OAuthTokenStorage_HasTokens_WithNoTokens_ShouldReturnFalse()
        {
            // Act & Assert
            Assert.IsFalse(_client.HasOAuthTokens(TestClientId));
        }

        [TestMethod]
        public void OAuthTokenStorage_HasValidTokens_WithValidTokens_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                ClientId = TestClientId
            };
            _client.StoreOAuthTokens(TestClientId, tokens);

            // Act & Assert
            Assert.IsTrue(_client.HasValidOAuthTokens(TestClientId));
        }

        [TestMethod]
        public void OAuthTokenStorage_HasValidTokens_WithExpiredTokens_ShouldReturnFalse()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
                ClientId = TestClientId
            };
            _client.StoreOAuthTokens(TestClientId, tokens);

            // Act & Assert
            Assert.IsFalse(_client.HasValidOAuthTokens(TestClientId));
        }

        [TestMethod]
        public void OAuthTokenStorage_HasValidTokens_WithNoTokens_ShouldReturnFalse()
        {
            // Act & Assert
            Assert.IsFalse(_client.HasValidOAuthTokens(TestClientId));
        }

        [TestMethod]
        public void OAuthTokenStorage_ClearTokens_ShouldRemoveTokens()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ClientId = TestClientId
            };
            _client.StoreOAuthTokens(TestClientId, tokens);
            Assert.IsTrue(_client.HasOAuthTokens(TestClientId));

            // Act
            _client.ClearOAuthTokens(TestClientId);

            // Assert
            Assert.IsNull(_client.GetOAuthTokens(TestClientId));
            Assert.IsFalse(_client.HasOAuthTokens(TestClientId));
            Assert.IsFalse(_client.HasOAuthTokens(TestClientId));
        }

        [TestMethod]
        public void OAuthTokenStorage_ClearTokens_WithNonExistentClientId_ShouldNotThrow()
        {
            // Act & Assert - Should not throw
            _client.ClearOAuthTokens("non-existent-client-id");
        }


        [TestMethod]
        public void OAuthTokenStorage_ThreadSafety_ShouldHandleConcurrentAccess()
        {
            // Arrange
            var tokens1 = new OAuthTokens
            {
                AccessToken = "token-1",
                ClientId = "client-1"
            };
            var tokens2 = new OAuthTokens
            {
                AccessToken = "token-2",
                ClientId = "client-2"
            };

            // Act - Simulate concurrent access
            var task1 = Task.Run(() =>
            {
                _client.StoreOAuthTokens("client-1", tokens1);
                return _client.GetOAuthTokens("client-1");
            });

            var task2 = Task.Run(() =>
            {
                _client.StoreOAuthTokens("client-2", tokens2);
                return _client.GetOAuthTokens("client-2");
            });

            Task.WaitAll(task1, task2);

            // Assert
            Assert.AreEqual("token-1", task1.Result.AccessToken);
            Assert.AreEqual("token-2", task2.Result.AccessToken);
        }

        [TestMethod]
        public void OAuthTokenStorage_UpdateTokens_ShouldReplaceExistingTokens()
        {
            // Arrange
            var originalTokens = new OAuthTokens
            {
                AccessToken = "original-token",
                ClientId = TestClientId
            };
            _client.StoreOAuthTokens(TestClientId, originalTokens);

            var updatedTokens = new OAuthTokens
            {
                AccessToken = "updated-token",
                RefreshToken = "new-refresh-token",
                ClientId = TestClientId
            };

            // Act
            _client.StoreOAuthTokens(TestClientId, updatedTokens);
            var retrievedTokens = _client.GetOAuthTokens(TestClientId);

            // Assert
            Assert.AreEqual("updated-token", retrievedTokens.AccessToken);
            Assert.AreEqual("new-refresh-token", retrievedTokens.RefreshToken);
        }
    }
}


