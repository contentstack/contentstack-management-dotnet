using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Unit.Tests.OAuth
{
    [TestClass]
    public class OAuthTokenTest
    {
        [TestMethod]
        public void OAuthTokens_DefaultValues_ShouldBeCorrect()
        {
            // Act
            var tokens = new OAuthTokens();

            // Assert
            Assert.IsNull(tokens.AccessToken);
            Assert.IsNull(tokens.RefreshToken);
            Assert.IsNull(tokens.OrganizationUid);
            Assert.IsNull(tokens.UserUid);
            Assert.IsNull(tokens.ClientId);
            Assert.IsNull(tokens.AppId);
            Assert.AreEqual(default(DateTime), tokens.ExpiresAt);
        }

        [TestMethod]
        public void OAuthTokens_IsValid_WithValidToken_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                ClientId = "test-client-id"
            };

            // Act
            var isValid = tokens.IsValid;

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void OAuthTokens_IsValid_WithNullAccessToken_ShouldReturnFalse()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = null,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                ClientId = "test-client-id"
            };

            // Act
            var isValid = tokens.IsValid;

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void OAuthTokens_IsValid_WithEmptyAccessToken_ShouldReturnFalse()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                ClientId = "test-client-id"
            };

            // Act
            var isValid = tokens.IsValid;

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void OAuthTokens_IsValid_WithExpiredToken_ShouldReturnFalse()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
                ClientId = "test-client-id"
            };

            // Act
            var isValid = tokens.IsValid;

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void OAuthTokens_IsValid_WithDefaultExpiryTime_ShouldReturnFalse()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = default(DateTime),
                ClientId = "test-client-id"
            };

            // Act
            var isValid = tokens.IsValid;

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void OAuthTokens_IsExpired_WithFutureExpiryTime_ShouldReturnFalse()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                ClientId = "test-client-id"
            };

            // Act
            var isExpired = tokens.IsExpired;

            // Assert
            Assert.IsFalse(isExpired);
        }

        [TestMethod]
        public void OAuthTokens_IsExpired_WithPastExpiryTime_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
                ClientId = "test-client-id"
            };

            // Act
            var isExpired = tokens.IsExpired;

            // Assert
            Assert.IsTrue(isExpired);
        }

        [TestMethod]
        public void OAuthTokens_IsExpired_WithDefaultExpiryTime_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = default(DateTime),
                ClientId = "test-client-id"
            };

            // Act
            var isExpired = tokens.IsExpired;

            // Assert
            Assert.IsTrue(isExpired);
        }

        [TestMethod]
        public void OAuthTokens_NeedsRefresh_WithTokenExpiringSoon_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(2), // Less than 5 minutes
                ClientId = "test-client-id"
            };

            // Act
            var needsRefresh = tokens.NeedsRefresh;

            // Assert
            Assert.IsTrue(needsRefresh);
        }

        [TestMethod]
        public void OAuthTokens_NeedsRefresh_WithTokenNotExpiringSoon_ShouldReturnFalse()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(10), // More than 5 minutes
                ClientId = "test-client-id"
            };

            // Act
            var needsRefresh = tokens.NeedsRefresh;

            // Assert
            Assert.IsFalse(needsRefresh);
        }

        [TestMethod]
        public void OAuthTokens_NeedsRefresh_WithExpiredToken_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
                ClientId = "test-client-id"
            };

            // Act
            var needsRefresh = tokens.NeedsRefresh;

            // Assert
            Assert.IsTrue(needsRefresh);
        }

        [TestMethod]
        public void OAuthTokens_NeedsRefresh_WithNoRefreshToken_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(2),
                RefreshToken = null,
                ClientId = "test-client-id"
            };

            // Act
            var needsRefresh = tokens.NeedsRefresh;

            // Assert
            Assert.IsTrue(needsRefresh); // NeedsRefresh is based on expiry time, not refresh token presence
        }

        [TestMethod]
        public void OAuthTokens_NeedsRefresh_WithEmptyRefreshToken_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(2),
                RefreshToken = "",
                ClientId = "test-client-id"
            };

            // Act
            var needsRefresh = tokens.NeedsRefresh;

            // Assert
            Assert.IsTrue(needsRefresh); // NeedsRefresh is based on expiry time, not refresh token presence
        }

        [TestMethod]
        public void OAuthTokens_NeedsRefresh_WithValidRefreshToken_ShouldReturnTrue()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(2),
                RefreshToken = "test-refresh-token",
                ClientId = "test-client-id"
            };

            // Act
            var needsRefresh = tokens.NeedsRefresh;

            // Assert
            Assert.IsTrue(needsRefresh);
        }

        [TestMethod]
        public void OAuthTokens_ToString_ShouldReturnTypeName()
        {
            // Arrange
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                RefreshToken = "test-refresh-token",
                OrganizationUid = "test-org-uid",
                UserUid = "test-user-uid",
                ClientId = "test-client-id",
                AppId = "test-app-id",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            // Act
            var result = tokens.ToString();

            // Assert
            Assert.IsTrue(result.Contains("OAuthTokens")); // Default ToString returns type name
        }

        [TestMethod]
        public void OAuthTokens_WithAllProperties_ShouldSetCorrectly()
        {
            // Arrange
            var accessToken = "test-access-token";
            var refreshToken = "test-refresh-token";
            var organizationUid = "test-org-uid";
            var userUid = "test-user-uid";
            var clientId = "test-client-id";
            var appId = "test-app-id";
            var expiresAt = DateTime.UtcNow.AddHours(1);

            // Act
            var tokens = new OAuthTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                OrganizationUid = organizationUid,
                UserUid = userUid,
                ClientId = clientId,
                AppId = appId,
                ExpiresAt = expiresAt
            };

            // Assert
            Assert.AreEqual(accessToken, tokens.AccessToken);
            Assert.AreEqual(refreshToken, tokens.RefreshToken);
            Assert.AreEqual(organizationUid, tokens.OrganizationUid);
            Assert.AreEqual(userUid, tokens.UserUid);
            Assert.AreEqual(clientId, tokens.ClientId);
            Assert.AreEqual(appId, tokens.AppId);
            Assert.AreEqual(expiresAt, tokens.ExpiresAt);
        }
    }
}
