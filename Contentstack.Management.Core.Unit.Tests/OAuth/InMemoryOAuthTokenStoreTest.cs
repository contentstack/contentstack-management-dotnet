using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Unit.Tests.OAuth
{
    [TestClass]
    public class InMemoryOAuthTokenStoreTest
    {
        private const string TestClientId = "test-client-id";

        [TestCleanup]
        public void Cleanup()
        {
            // Clear any test tokens after each test
            InMemoryOAuthTokenStore.ClearTokens(TestClientId);
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_SetAndGetTokens_ShouldWork()
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
            InMemoryOAuthTokenStore.SetTokens(TestClientId, tokens);
            var retrievedTokens = InMemoryOAuthTokenStore.GetTokens(TestClientId);

            // Assert
            Assert.IsNotNull(retrievedTokens);
            Assert.AreEqual("test-access-token", retrievedTokens.AccessToken);
            Assert.AreEqual("test-refresh-token", retrievedTokens.RefreshToken);
            Assert.AreEqual("test-org-uid", retrievedTokens.OrganizationUid);
            Assert.AreEqual("test-user-uid", retrievedTokens.UserUid);
            Assert.AreEqual(TestClientId, retrievedTokens.ClientId);
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_GetTokens_WithNonExistentClientId_ShouldReturnNull()
        {
            // Act
            var tokens = InMemoryOAuthTokenStore.GetTokens("non-existent-client-id");

            // Assert
            Assert.IsNull(tokens);
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_HasTokens_WithExistingTokens_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ClientId = TestClientId
            };
            InMemoryOAuthTokenStore.SetTokens(TestClientId, tokens);

            // Act & Assert
            Assert.IsTrue(InMemoryOAuthTokenStore.HasTokens(TestClientId));
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_HasTokens_WithNoTokens_ShouldReturnFalse()
        {
            // Act & Assert
            Assert.IsFalse(InMemoryOAuthTokenStore.HasTokens(TestClientId));
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_HasValidTokens_WithValidTokens_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                ClientId = TestClientId
            };
            InMemoryOAuthTokenStore.SetTokens(TestClientId, tokens);

            // Act & Assert
            Assert.IsTrue(InMemoryOAuthTokenStore.HasValidTokens(TestClientId));
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_HasValidTokens_WithExpiredTokens_ShouldReturnFalse()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
                ClientId = TestClientId
            };
            InMemoryOAuthTokenStore.SetTokens(TestClientId, tokens);

            // Act & Assert
            Assert.IsFalse(InMemoryOAuthTokenStore.HasValidTokens(TestClientId));
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_HasValidTokens_WithNoTokens_ShouldReturnFalse()
        {
            // Act & Assert
            Assert.IsFalse(InMemoryOAuthTokenStore.HasValidTokens(TestClientId));
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_ClearTokens_ShouldRemoveTokens()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ClientId = TestClientId
            };
            InMemoryOAuthTokenStore.SetTokens(TestClientId, tokens);
            Assert.IsTrue(InMemoryOAuthTokenStore.HasTokens(TestClientId));

            // Act
            InMemoryOAuthTokenStore.ClearTokens(TestClientId);

            // Assert
            Assert.IsNull(InMemoryOAuthTokenStore.GetTokens(TestClientId));
            Assert.IsFalse(InMemoryOAuthTokenStore.HasTokens(TestClientId));
            Assert.IsFalse(InMemoryOAuthTokenStore.HasValidTokens(TestClientId));
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_ClearTokens_WithNonExistentClientId_ShouldNotThrow()
        {
            // Act & Assert - Should not throw
            InMemoryOAuthTokenStore.ClearTokens("non-existent-client-id");
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_GetRefreshLock_ShouldReturnSemaphore()
        {
            // Act
            var lock1 = InMemoryOAuthTokenStore.GetRefreshLock(TestClientId);
            var lock2 = InMemoryOAuthTokenStore.GetRefreshLock(TestClientId);

            // Assert
            Assert.IsNotNull(lock1);
            Assert.IsNotNull(lock2);
            Assert.AreSame(lock1, lock2); // Should return the same instance
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_GetRefreshLock_DifferentClientIds_ShouldReturnDifferentSemaphores()
        {
            // Act
            var lock1 = InMemoryOAuthTokenStore.GetRefreshLock("client-1");
            var lock2 = InMemoryOAuthTokenStore.GetRefreshLock("client-2");

            // Assert
            Assert.IsNotNull(lock1);
            Assert.IsNotNull(lock2);
            Assert.AreNotSame(lock1, lock2); // Should return different instances
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_ThreadSafety_ShouldHandleConcurrentAccess()
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
                InMemoryOAuthTokenStore.SetTokens("client-1", tokens1);
                return InMemoryOAuthTokenStore.GetTokens("client-1");
            });

            var task2 = Task.Run(() =>
            {
                InMemoryOAuthTokenStore.SetTokens("client-2", tokens2);
                return InMemoryOAuthTokenStore.GetTokens("client-2");
            });

            Task.WaitAll(task1, task2);

            // Assert
            Assert.AreEqual("token-1", task1.Result.AccessToken);
            Assert.AreEqual("token-2", task2.Result.AccessToken);
        }

        [TestMethod]
        public void InMemoryOAuthTokenStore_UpdateTokens_ShouldReplaceExistingTokens()
        {
            // Arrange
            var originalTokens = new OAuthTokens
            {
                AccessToken = "original-token",
                ClientId = TestClientId
            };
            InMemoryOAuthTokenStore.SetTokens(TestClientId, originalTokens);

            var updatedTokens = new OAuthTokens
            {
                AccessToken = "updated-token",
                RefreshToken = "new-refresh-token",
                ClientId = TestClientId
            };

            // Act
            InMemoryOAuthTokenStore.SetTokens(TestClientId, updatedTokens);
            var retrievedTokens = InMemoryOAuthTokenStore.GetTokens(TestClientId);

            // Assert
            Assert.AreEqual("updated-token", retrievedTokens.AccessToken);
            Assert.AreEqual("new-refresh-token", retrievedTokens.RefreshToken);
        }
    }
}


