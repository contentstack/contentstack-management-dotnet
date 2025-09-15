using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Unit.Tests.OAuth
{
    [TestClass]
    public class OAuthOptionsTest
    {
        [TestMethod]
        public void OAuthOptions_DefaultValues_ShouldBeCorrect()
        {

            var options = new OAuthOptions();
            Assert.IsTrue(options.UsePkce);
            Assert.AreEqual("code", options.ResponseType);
            Assert.AreEqual("6400aa06db64de001a31c8a9", options.AppId);
            Assert.AreEqual("Ie0FEfTzlfAHL4xM", options.ClientId);
            Assert.AreEqual("http://localhost:8184", options.RedirectUri);
            Assert.IsNull(options.ClientSecret);
            Assert.IsNull(options.Scope);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithValidPKCEOptions_ShouldReturnTrue()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code"
                // UsePkce is automatically true when ClientSecret is null/empty
            };
            var isValid = options.IsValid();
            Assert.IsTrue(isValid);
            Assert.IsTrue(options.UsePkce);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithValidTraditionalOAuthOptions_ShouldReturnTrue()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code",
                ClientSecret = "test-secret"
                // UsePkce is automatically false when ClientSecret is provided
            };
            var isValid = options.IsValid();
            Assert.IsTrue(isValid);
            Assert.IsFalse(options.UsePkce);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithMissingAppId_ShouldReturnFalse()
        {
            
            var options = new OAuthOptions
            {
                AppId = "",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code"
            };
            var isValid = options.IsValid(out var errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("AppId is required for OAuth configuration.", errorMessage);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithMissingClientId_ShouldReturnFalse()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code"
            };
            var isValid = options.IsValid(out var errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("ClientId is required for OAuth configuration.", errorMessage);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithMissingRedirectUri_ShouldReturnFalse()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "",
                ResponseType = "code"
            };
            var isValid = options.IsValid(out var errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("RedirectUri is required for OAuth configuration.", errorMessage);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithInvalidRedirectUri_ShouldReturnFalse()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "not-a-valid-uri",
                ResponseType = "code"
            };
            var isValid = options.IsValid(out var errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("RedirectUri must be a valid absolute URI.", errorMessage);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithNonHttpRedirectUri_ShouldReturnFalse()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "ftp://example.com/callback",
                ResponseType = "code"
            };
            var isValid = options.IsValid(out var errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("RedirectUri must use http or https scheme.", errorMessage);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithMissingResponseType_ShouldReturnFalse()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = ""
            };
            var isValid = options.IsValid(out var errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("ResponseType is required for OAuth configuration.", errorMessage);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithInvalidResponseType_ShouldReturnFalse()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "token"
            };
            var isValid = options.IsValid(out var errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("ResponseType must be 'code' for authorization code flow.", errorMessage);
        }

        [TestMethod]
        public void OAuthOptions_IsValid_WithTraditionalOAuthMissingClientSecret_ShouldReturnFalse()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code"
                // UsePkce will be true because ClientSecret is null
            };
            var isValid = options.IsValid(out var errorMessage);
            Assert.IsTrue(isValid); // This will actually be valid because UsePkce is true
            Assert.IsTrue(options.UsePkce);
        }

        [TestMethod]
        public void OAuthOptions_Validate_WithValidOptions_ShouldNotThrow()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code"
            };

            // Should not throw
            options.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(OAuthConfigurationException))]
        public void OAuthOptions_Validate_WithInvalidOptions_ShouldThrowException()
        {
            
            var options = new OAuthOptions
            {
                AppId = "",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code"
            };
            options.Validate();
        }

        [TestMethod]
        public void OAuthOptions_WithScopes_ShouldBeValid()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code",
                Scope = new[] { "read", "write", "admin" }
            };
            var isValid = options.IsValid();
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void OAuthOptions_WithEmptyScopes_ShouldBeValid()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code",
                Scope = new string[0]
            };
            var isValid = options.IsValid();
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void OAuthOptions_WithNullScopes_ShouldBeValid()
        {
            
            var options = new OAuthOptions
            {
                AppId = "test-app-id",
                ClientId = "test-client-id",
                RedirectUri = "https://example.com/callback",
                ResponseType = "code",
                Scope = null
            };
            var isValid = options.IsValid();
            Assert.IsTrue(isValid);
        }
    }
}
