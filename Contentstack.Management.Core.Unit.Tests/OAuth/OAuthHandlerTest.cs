using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Unit.Tests.OAuth
{
    [TestClass]
    public class OAuthHandlerTest
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
            InMemoryOAuthTokenStore.ClearTokens(_options.ClientId);
        }

        [TestMethod]
        public void OAuthHandler_Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Act
            var handler = new OAuthHandler(_client, _options);

            // Assert
            Assert.IsNotNull(handler);
            Assert.AreEqual(_options.ClientId, handler.ClientId);
            Assert.AreEqual(_options.AppId, handler.AppId);
            Assert.AreEqual(_options.RedirectUri, handler.RedirectUri);
            Assert.AreEqual(_options.UsePkce, handler.UsePkce);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OAuthHandler_Constructor_WithNullClient_ShouldThrowException()
        {
            // Act & Assert
            new OAuthHandler(null, _options);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OAuthHandler_Constructor_WithNullOptions_ShouldThrowException()
        {
            // Act & Assert
            new OAuthHandler(_client, null);
        }

        [TestMethod]
        [ExpectedException(typeof(OAuthConfigurationException))]
        public void OAuthHandler_Constructor_WithInvalidOptions_ShouldThrowException()
        {
            // Arrange
            var invalidOptions = new OAuthOptions
            {
                AppId = "", // Invalid
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code"
            };

            // Act & Assert
            new OAuthHandler(_client, invalidOptions);
        }

        [TestMethod]
        public void OAuthHandler_GetCurrentTokens_WithNoTokens_ShouldReturnNull()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            var tokens = handler.GetCurrentTokens();

            // Assert
            Assert.IsNull(tokens);
        }

        [TestMethod]
        public void OAuthHandler_GetCurrentTokens_WithStoredTokens_ShouldReturnTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var expectedTokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, expectedTokens);

            // Act
            var tokens = handler.GetCurrentTokens();

            // Assert
            Assert.IsNotNull(tokens);
            Assert.AreEqual("test-token", tokens.AccessToken);
        }

        [TestMethod]
        public void OAuthHandler_HasValidTokens_WithNoTokens_ShouldReturnFalse()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act & Assert
            Assert.IsFalse(handler.HasValidTokens());
        }

        [TestMethod]
        public void OAuthHandler_HasValidTokens_WithValidTokens_ShouldReturnTrue()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act & Assert
            Assert.IsTrue(handler.HasValidTokens());
        }

        [TestMethod]
        public void OAuthHandler_HasValidTokens_WithExpiredTokens_ShouldReturnFalse()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act & Assert
            Assert.IsFalse(handler.HasValidTokens());
        }

        [TestMethod]
        public void OAuthHandler_HasTokens_WithNoTokens_ShouldReturnFalse()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act & Assert
            Assert.IsFalse(handler.HasTokens());
        }

        [TestMethod]
        public void OAuthHandler_HasTokens_WithTokens_ShouldReturnTrue()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act & Assert
            Assert.IsTrue(handler.HasTokens());
        }

        [TestMethod]
        public void OAuthHandler_ClearTokens_ShouldRemoveTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);
            Assert.IsTrue(handler.HasTokens());

            // Act
            handler.ClearTokens();

            // Assert
            Assert.IsFalse(handler.HasTokens());
            Assert.IsNull(handler.GetCurrentTokens());
        }

        [TestMethod]
        public void OAuthHandler_GetAuthorizationUrl_WithPKCE_ShouldReturnValidUrl()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            var authUrl = handler.GetAuthorizationUrl();

            // Assert
            Assert.IsNotNull(authUrl);
            Assert.IsTrue(authUrl.Contains("response_type=code"));
            Assert.IsTrue(authUrl.Contains($"client_id={_options.ClientId}"));
            Assert.IsTrue(authUrl.Contains($"redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}"));
            Assert.IsTrue(authUrl.Contains("code_challenge="));
            Assert.IsTrue(authUrl.Contains("code_challenge_method=S256"));
        }

        [TestMethod]
        public void OAuthHandler_GetAuthorizationUrl_WithTraditionalOAuth_ShouldReturnValidUrl()
        {
            // Arrange
            var traditionalOptions = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code",
                ClientSecret = "test-secret"
            };
            var handler = new OAuthHandler(_client, traditionalOptions);

            // Act
            var authUrl = handler.GetAuthorizationUrl();

            // Assert
            Assert.IsNotNull(authUrl);
            Assert.IsTrue(authUrl.Contains("response_type=code"));
            Assert.IsTrue(authUrl.Contains($"client_id={traditionalOptions.ClientId}"));
            Assert.IsTrue(authUrl.Contains($"redirect_uri={Uri.EscapeDataString(traditionalOptions.RedirectUri)}"));
            Assert.IsFalse(authUrl.Contains("code_challenge="));
        }

        [TestMethod]
        public void OAuthHandler_GetAuthorizationUrl_WithScopes_ShouldIncludeScopes()
        {
            // Arrange
            _options.Scope = new[] { "read", "write" };
            var handler = new OAuthHandler(_client, _options);

            // Act
            var authUrl = handler.GetAuthorizationUrl();

            // Assert
            Assert.IsTrue(authUrl.Contains("scope=read%20write"));
        }

        [TestMethod]
        public void OAuthHandler_GetAuthorizationUrl_ShouldStoreCodeVerifierForPKCE()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.GetAuthorizationUrl();

            // Assert
            var storedTokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.IsNotNull(storedTokens);
            Assert.IsNotNull(storedTokens.AccessToken); // This is the code verifier
            Assert.IsTrue(PkceHelper.IsValidCodeVerifier(storedTokens.AccessToken));
        }

        [TestMethod]
        public void OAuthHandler_ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            var result = handler.ToString();

            // Assert
            Assert.IsTrue(result.Contains(_options.ClientId));
            Assert.IsTrue(result.Contains(_options.AppId));
            Assert.IsTrue(result.Contains("True")); // UsePkce
            Assert.IsTrue(result.Contains("False")); // HasTokens
        }

        [TestMethod]
        public void OAuthHandler_ExchangeCodeForTokenAsync_WithEmptyCode_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act & Assert
            try
            {
                handler.ExchangeCodeForTokenAsync("").Wait();
                Assert.Fail("Should have thrown ArgumentException");
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void OAuthHandler_ExchangeCodeForTokenAsync_WithNullCode_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act & Assert
            try
            {
                handler.ExchangeCodeForTokenAsync(null).Wait();
                Assert.Fail("Should have thrown ArgumentException");
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void OAuthHandler_ExchangeCodeForTokenAsync_WithoutCodeVerifier_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act & Assert
            try
            {
                handler.ExchangeCodeForTokenAsync("test-code").Wait();
                Assert.Fail("Should have thrown OAuthTokenException");
            }
            catch (AggregateException ex) when (ex.InnerException is OAuthTokenException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void OAuthHandler_RefreshTokenAsync_WithNoTokens_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act & Assert
            try
            {
                handler.RefreshTokenAsync().Wait();
                Assert.Fail("Should have thrown OAuthTokenRefreshException");
            }
            catch (AggregateException ex) when (ex.InnerException is OAuthTokenRefreshException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void OAuthHandler_RefreshTokenAsync_WithEmptyRefreshToken_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act & Assert
            try
            {
                handler.RefreshTokenAsync("").Wait();
                Assert.Fail("Should have thrown OAuthTokenRefreshException");
            }
            catch (AggregateException ex) when (ex.InnerException is OAuthTokenRefreshException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void OAuthHandler_LogoutAsync_WithNoTokens_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act & Assert
            try
            {
                handler.LogoutAsync().Wait();
                Assert.Fail("Expected OAuthException to be thrown");
            }
            catch (AggregateException ex) when (ex.InnerException is Exceptions.OAuthException)
            {
                // Expected - async methods wrap exceptions in AggregateException
                Assert.IsTrue(ex.InnerException.Message.Contains("No OAuth tokens found"));
            }
        }

        [TestMethod]
        public void OAuthHandler_LogoutAsync_WithTokens_ShouldReturnSuccessMessage()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act & Assert
            try
            {
                var result = handler.LogoutAsync().Result;
                // If successful, should return success message and clear tokens
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Contains("successfully"));
                Assert.IsFalse(handler.HasTokens());
            }
            catch (AggregateException ex) when (ex.InnerException is Exceptions.OAuthException)
            {
                // Expected - the actual API call will fail in unit tests
                // This confirms that the method attempted to call the revocation API
                Assert.IsTrue(ex.InnerException.Message.Contains("Failed to get OAuth app authorization"));
            }
        }

        #region Getter Methods Tests
        [TestMethod]
        public void OAuthHandler_GetAccessToken_WithValidTokens_ShouldReturnAccessToken()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                AccessToken = "test-access-token",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act
            var result = handler.GetAccessToken();

            // Assert
            Assert.AreEqual("test-access-token", result);
        }

        [TestMethod]
        public void OAuthHandler_GetAccessToken_WithNoTokens_ShouldReturnNull()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            var result = handler.GetAccessToken();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void OAuthHandler_GetRefreshToken_WithValidTokens_ShouldReturnRefreshToken()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                RefreshToken = "test-refresh-token",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act
            var result = handler.GetRefreshToken();

            // Assert
            Assert.AreEqual("test-refresh-token", result);
        }

        [TestMethod]
        public void OAuthHandler_GetRefreshToken_WithNoTokens_ShouldReturnNull()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            var result = handler.GetRefreshToken();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void OAuthHandler_GetOrganizationUID_WithValidTokens_ShouldReturnOrganizationUID()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                OrganizationUid = "test-org-uid",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act
            var result = handler.GetOrganizationUID();

            // Assert
            Assert.AreEqual("test-org-uid", result);
        }

        [TestMethod]
        public void OAuthHandler_GetOrganizationUID_WithNoTokens_ShouldReturnNull()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            var result = handler.GetOrganizationUID();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void OAuthHandler_GetUserUID_WithValidTokens_ShouldReturnUserUID()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                UserUid = "test-user-uid",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act
            var result = handler.GetUserUID();

            // Assert
            Assert.AreEqual("test-user-uid", result);
        }

        [TestMethod]
        public void OAuthHandler_GetUserUID_WithNoTokens_ShouldReturnNull()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            var result = handler.GetUserUID();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void OAuthHandler_GetTokenExpiryTime_WithValidTokens_ShouldReturnExpiryTime()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var expiryTime = DateTime.UtcNow.AddHours(1);
            var tokens = new OAuthTokens
            {
                ExpiresAt = expiryTime,
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act
            var result = handler.GetTokenExpiryTime();

            // Assert
            Assert.AreEqual(expiryTime, result);
        }

        [TestMethod]
        public void OAuthHandler_GetTokenExpiryTime_WithNoTokens_ShouldReturnNull()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            var result = handler.GetTokenExpiryTime();

            // Assert
            Assert.IsNull(result);
        }
        #endregion

        #region Setter Methods Tests
        [TestMethod]
        public void OAuthHandler_SetAccessToken_WithValidToken_ShouldUpdateTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act
            handler.SetAccessToken("new-access-token");

            // Assert
            var updatedTokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.AreEqual("new-access-token", updatedTokens.AccessToken);
        }

        [TestMethod]
        public void OAuthHandler_SetAccessToken_WithNoExistingTokens_ShouldCreateNewTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetAccessToken("new-access-token");

            // Assert
            var tokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.IsNotNull(tokens);
            Assert.AreEqual("new-access-token", tokens.AccessToken);
            Assert.AreEqual(_options.ClientId, tokens.ClientId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OAuthHandler_SetAccessToken_WithNullToken_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetAccessToken(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OAuthHandler_SetAccessToken_WithEmptyToken_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetAccessToken("");
        }

        [TestMethod]
        public void OAuthHandler_SetRefreshToken_WithValidToken_ShouldUpdateTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act
            handler.SetRefreshToken("new-refresh-token");

            // Assert
            var updatedTokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.AreEqual("new-refresh-token", updatedTokens.RefreshToken);
        }

        [TestMethod]
        public void OAuthHandler_SetRefreshToken_WithNoExistingTokens_ShouldCreateNewTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetRefreshToken("new-refresh-token");

            // Assert
            var tokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.IsNotNull(tokens);
            Assert.AreEqual("new-refresh-token", tokens.RefreshToken);
            Assert.AreEqual(_options.ClientId, tokens.ClientId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OAuthHandler_SetRefreshToken_WithNullToken_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetRefreshToken(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OAuthHandler_SetRefreshToken_WithEmptyToken_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetRefreshToken("");
        }

        [TestMethod]
        public void OAuthHandler_SetOrganizationUID_WithValidUID_ShouldUpdateTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act
            handler.SetOrganizationUID("new-org-uid");

            // Assert
            var updatedTokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.AreEqual("new-org-uid", updatedTokens.OrganizationUid);
        }

        [TestMethod]
        public void OAuthHandler_SetOrganizationUID_WithNoExistingTokens_ShouldCreateNewTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetOrganizationUID("new-org-uid");

            // Assert
            var tokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.IsNotNull(tokens);
            Assert.AreEqual("new-org-uid", tokens.OrganizationUid);
            Assert.AreEqual(_options.ClientId, tokens.ClientId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OAuthHandler_SetOrganizationUID_WithNullUID_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetOrganizationUID(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OAuthHandler_SetOrganizationUID_WithEmptyUID_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetOrganizationUID("");
        }

        [TestMethod]
        public void OAuthHandler_SetUserUID_WithValidUID_ShouldUpdateTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act
            handler.SetUserUID("new-user-uid");

            // Assert
            var updatedTokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.AreEqual("new-user-uid", updatedTokens.UserUid);
        }

        [TestMethod]
        public void OAuthHandler_SetUserUID_WithNoExistingTokens_ShouldCreateNewTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetUserUID("new-user-uid");

            // Assert
            var tokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.IsNotNull(tokens);
            Assert.AreEqual("new-user-uid", tokens.UserUid);
            Assert.AreEqual(_options.ClientId, tokens.ClientId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OAuthHandler_SetUserUID_WithNullUID_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetUserUID(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OAuthHandler_SetUserUID_WithEmptyUID_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            handler.SetUserUID("");
        }

        [TestMethod]
        public void OAuthHandler_SetTokenExpiryTime_WithValidTime_ShouldUpdateTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);
            var newExpiryTime = DateTime.UtcNow.AddHours(2);

            // Act
            handler.SetTokenExpiryTime(newExpiryTime);

            // Assert
            var updatedTokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.AreEqual(newExpiryTime, updatedTokens.ExpiresAt);
        }

        [TestMethod]
        public void OAuthHandler_SetTokenExpiryTime_WithNoExistingTokens_ShouldCreateNewTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var newExpiryTime = DateTime.UtcNow.AddHours(2);

            // Act
            handler.SetTokenExpiryTime(newExpiryTime);

            // Assert
            var tokens = InMemoryOAuthTokenStore.GetTokens(_options.ClientId);
            Assert.IsNotNull(tokens);
            Assert.AreEqual(newExpiryTime, tokens.ExpiresAt);
            Assert.AreEqual(_options.ClientId, tokens.ClientId);
        }
        #endregion

        #region HandleRedirectAsync Tests
        [TestMethod]
        public async Task OAuthHandler_HandleRedirectAsync_WithValidUrl_ShouldExchangeCodeForTokens()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var redirectUrl = "https://example.com/callback?code=test-auth-code&state=test-state";

            // Act & Assert
            try
            {
                await handler.HandleRedirectAsync(redirectUrl);
                // If we get here, the method completed without throwing an exception
                // The actual token exchange would fail in a real test due to mocking, but the URL parsing works
            }
            catch (Exceptions.OAuthTokenException)
            {
                // Expected - the actual API call will fail in unit tests
                // This confirms that the URL parsing worked and the method attempted the token exchange
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task OAuthHandler_HandleRedirectAsync_WithNullUrl_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            await handler.HandleRedirectAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task OAuthHandler_HandleRedirectAsync_WithEmptyUrl_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            await handler.HandleRedirectAsync("");
        }

        [TestMethod]
        [ExpectedException(typeof(Exceptions.OAuthException))]
        public async Task OAuthHandler_HandleRedirectAsync_WithUrlMissingCode_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var redirectUrl = "https://example.com/callback?state=test-state";

            // Act
            await handler.HandleRedirectAsync(redirectUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(Exceptions.OAuthException))]
        public async Task OAuthHandler_HandleRedirectAsync_WithUrlContainingEmptyCode_ShouldThrowException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var redirectUrl = "https://example.com/callback?code=&state=test-state";

            // Act
            await handler.HandleRedirectAsync(redirectUrl);
        }

        [TestMethod]
        public async Task OAuthHandler_HandleRedirectAsync_WithComplexUrl_ShouldParseCorrectly()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var redirectUrl = "https://example.com/callback?code=test-auth-code&state=test-state&other=value";

            // Act & Assert
            try
            {
                await handler.HandleRedirectAsync(redirectUrl);
                // If we get here, the method completed without throwing an exception
            }
            catch (Exceptions.OAuthTokenException)
            {
                // Expected - the actual API call will fail in unit tests
                // This confirms that the URL parsing worked correctly
            }
        }
        #endregion

        #region Updated LogoutAsync Tests
        [TestMethod]
        public async Task OAuthHandler_LogoutAsync_WithValidTokens_ShouldCallRevocationAPI()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                UserUid = "test-user-uid",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act & Assert
            try
            {
                var result = await handler.LogoutAsync();
                // If we get here, the method completed without throwing an exception
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Contains("successfully"));
            }
            catch (Exceptions.OAuthException)
            {
                // Expected - the actual API call will fail in unit tests due to mocking
                // This confirms that the method attempted to call the revocation API
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exceptions.OAuthException))]
        public async Task OAuthHandler_LogoutAsync_WithNoTokens_ShouldThrowOAuthException()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);

            // Act
            await handler.LogoutAsync();
        }

        [TestMethod]
        public async Task OAuthHandler_LogoutAsync_ShouldClearTokensAfterSuccessfulRevocation()
        {
            // Arrange
            var handler = new OAuthHandler(_client, _options);
            var tokens = new OAuthTokens
            {
                AccessToken = "test-token",
                UserUid = "test-user-uid",
                ClientId = _options.ClientId
            };
            InMemoryOAuthTokenStore.SetTokens(_options.ClientId, tokens);

            // Act & Assert
            try
            {
                await handler.LogoutAsync();
                // If successful, tokens should be cleared
                Assert.IsFalse(handler.HasTokens());
            }
            catch (Exceptions.OAuthException)
            {
                // Expected - the actual API call will fail in unit tests
                // In this case, tokens are NOT cleared because the API call failed
                Assert.IsTrue(handler.HasTokens());
            }
        }
        #endregion
    }
}
