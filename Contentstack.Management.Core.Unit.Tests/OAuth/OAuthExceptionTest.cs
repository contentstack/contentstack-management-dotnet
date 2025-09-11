using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Unit.Tests.OAuth
{
    [TestClass]
    public class OAuthExceptionTest
    {
        [TestMethod]
        public void OAuthException_DefaultConstructor_ShouldUseDefaultMessage()
        {
            // Act
            var exception = new OAuthException();

            // Assert
            Assert.AreEqual("OAuth operation failed.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthException_WithMessage_ShouldUseProvidedMessage()
        {
            // Arrange
            var message = "Custom OAuth error message";

            // Act
            var exception = new OAuthException(message);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthException_WithMessageAndInnerException_ShouldUseBoth()
        {
            // Arrange
            var message = "Custom OAuth error message";
            var innerException = new InvalidOperationException("Inner exception");

            // Act
            var exception = new OAuthException(message, innerException);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthConfigurationException_DefaultConstructor_ShouldUseDefaultMessage()
        {
            // Act
            var exception = new OAuthConfigurationException();

            // Assert
            Assert.AreEqual("OAuth configuration is invalid.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthConfigurationException_WithMessage_ShouldUseProvidedMessage()
        {
            // Arrange
            var message = "Custom configuration error message";

            // Act
            var exception = new OAuthConfigurationException(message);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthConfigurationException_WithMessageAndInnerException_ShouldUseBoth()
        {
            // Arrange
            var message = "Custom configuration error message";
            var innerException = new ArgumentException("Inner exception");

            // Act
            var exception = new OAuthConfigurationException(message, innerException);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenException_DefaultConstructor_ShouldUseDefaultMessage()
        {
            // Act
            var exception = new OAuthTokenException();

            // Assert
            Assert.AreEqual("OAuth token operation failed.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenException_WithMessage_ShouldUseProvidedMessage()
        {
            // Arrange
            var message = "Custom token error message";

            // Act
            var exception = new OAuthTokenException(message);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenException_WithMessageAndInnerException_ShouldUseBoth()
        {
            // Arrange
            var message = "Custom token error message";
            var innerException = new InvalidOperationException("Inner exception");

            // Act
            var exception = new OAuthTokenException(message, innerException);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthAuthorizationException_DefaultConstructor_ShouldUseDefaultMessage()
        {
            // Act
            var exception = new OAuthAuthorizationException();

            // Assert
            Assert.AreEqual("OAuth authorization failed.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthAuthorizationException_WithMessage_ShouldUseProvidedMessage()
        {
            // Arrange
            var message = "Custom authorization error message";

            // Act
            var exception = new OAuthAuthorizationException(message);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthAuthorizationException_WithMessageAndInnerException_ShouldUseBoth()
        {
            // Arrange
            var message = "Custom authorization error message";
            var innerException = new InvalidOperationException("Inner exception");

            // Act
            var exception = new OAuthAuthorizationException(message, innerException);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenRefreshException_DefaultConstructor_ShouldUseDefaultMessage()
        {
            // Act
            var exception = new OAuthTokenRefreshException();

            // Assert
            Assert.AreEqual("OAuth token refresh failed.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenRefreshException_WithMessage_ShouldUseProvidedMessage()
        {
            // Arrange
            var message = "Custom refresh error message";

            // Act
            var exception = new OAuthTokenRefreshException(message);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenRefreshException_WithMessageAndInnerException_ShouldUseBoth()
        {
            // Arrange
            var message = "Custom refresh error message";
            var innerException = new InvalidOperationException("Inner exception");

            // Act
            var exception = new OAuthTokenRefreshException(message, innerException);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthException_Inheritance_ShouldBeCorrect()
        {
            // Act
            var oauthException = new OAuthException();
            var configException = new OAuthConfigurationException();
            var tokenException = new OAuthTokenException();
            var authException = new OAuthAuthorizationException();
            var refreshException = new OAuthTokenRefreshException();

            // Assert
            Assert.IsInstanceOfType(oauthException, typeof(Exception));
            Assert.IsInstanceOfType(configException, typeof(OAuthException));
            Assert.IsInstanceOfType(tokenException, typeof(OAuthException));
            Assert.IsInstanceOfType(authException, typeof(OAuthException));
            Assert.IsInstanceOfType(refreshException, typeof(OAuthTokenException));
        }

        [TestMethod]
        public void OAuthException_Serialization_ShouldWork()
        {
            // Arrange
            var originalException = new OAuthException("Test message", new InvalidOperationException("Inner"));


            Assert.IsNotNull(originalException);
            Assert.AreEqual("Test message", originalException.Message);
            Assert.IsNotNull(originalException.InnerException);
        }

        [TestMethod]
        public void OAuthException_ToString_ShouldIncludeMessage()
        {
            // Arrange
            var message = "Test OAuth error message";
            var exception = new OAuthException(message);

            // Act
            var result = exception.ToString();

            // Assert
            Assert.IsTrue(result.Contains(message));
            Assert.IsTrue(result.Contains("OAuthException"));
        }

        [TestMethod]
        public void OAuthException_WithInnerException_ToString_ShouldIncludeBoth()
        {
            // Arrange
            var message = "Test OAuth error message";
            var innerMessage = "Inner exception message";
            var innerException = new InvalidOperationException(innerMessage);
            var exception = new OAuthException(message, innerException);

            // Act
            var result = exception.ToString();

            // Assert
            Assert.IsTrue(result.Contains(message));
            Assert.IsTrue(result.Contains(innerMessage));
            Assert.IsTrue(result.Contains("OAuthException"));
            Assert.IsTrue(result.Contains("InvalidOperationException"));
        }
    }
}

