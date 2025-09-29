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

            var exception = new OAuthException();
            Assert.AreEqual("OAuth operation failed.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthException_WithMessage_ShouldUseProvidedMessage()
        {
            
            var message = "Custom OAuth error message";
            var exception = new OAuthException(message);
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthException_WithMessageAndInnerException_ShouldUseBoth()
        {
            
            var message = "Custom OAuth error message";
            var innerException = new InvalidOperationException("Inner exception");
            var exception = new OAuthException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthConfigurationException_DefaultConstructor_ShouldUseDefaultMessage()
        {

            var exception = new OAuthConfigurationException();
            Assert.AreEqual("OAuth configuration is invalid.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthConfigurationException_WithMessage_ShouldUseProvidedMessage()
        {
            
            var message = "Custom configuration error message";
            var exception = new OAuthConfigurationException(message);
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthConfigurationException_WithMessageAndInnerException_ShouldUseBoth()
        {
            
            var message = "Custom configuration error message";
            var innerException = new ArgumentException("Inner exception");
            var exception = new OAuthConfigurationException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenException_DefaultConstructor_ShouldUseDefaultMessage()
        {

            var exception = new OAuthTokenException();
            Assert.AreEqual("OAuth token operation failed.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenException_WithMessage_ShouldUseProvidedMessage()
        {
            
            var message = "Custom token error message";
            var exception = new OAuthTokenException(message);
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenException_WithMessageAndInnerException_ShouldUseBoth()
        {
            
            var message = "Custom token error message";
            var innerException = new InvalidOperationException("Inner exception");
            var exception = new OAuthTokenException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthAuthorizationException_DefaultConstructor_ShouldUseDefaultMessage()
        {

            var exception = new OAuthAuthorizationException();
            Assert.AreEqual("OAuth authorization failed.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthAuthorizationException_WithMessage_ShouldUseProvidedMessage()
        {
            
            var message = "Custom authorization error message";
            var exception = new OAuthAuthorizationException(message);
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthAuthorizationException_WithMessageAndInnerException_ShouldUseBoth()
        {
            
            var message = "Custom authorization error message";
            var innerException = new InvalidOperationException("Inner exception");
            var exception = new OAuthAuthorizationException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenRefreshException_DefaultConstructor_ShouldUseDefaultMessage()
        {

            var exception = new OAuthTokenRefreshException();
            Assert.AreEqual("OAuth token refresh failed.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenRefreshException_WithMessage_ShouldUseProvidedMessage()
        {
            
            var message = "Custom refresh error message";
            var exception = new OAuthTokenRefreshException(message);
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void OAuthTokenRefreshException_WithMessageAndInnerException_ShouldUseBoth()
        {
            
            var message = "Custom refresh error message";
            var innerException = new InvalidOperationException("Inner exception");
            var exception = new OAuthTokenRefreshException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void OAuthException_Inheritance_ShouldBeCorrect()
        {

            var oauthException = new OAuthException();
            var configException = new OAuthConfigurationException();
            var tokenException = new OAuthTokenException();
            var authException = new OAuthAuthorizationException();
            var refreshException = new OAuthTokenRefreshException();
            Assert.IsInstanceOfType(oauthException, typeof(Exception));
            Assert.IsInstanceOfType(configException, typeof(OAuthException));
            Assert.IsInstanceOfType(tokenException, typeof(OAuthException));
            Assert.IsInstanceOfType(authException, typeof(OAuthException));
            Assert.IsInstanceOfType(refreshException, typeof(OAuthTokenException));
        }

        [TestMethod]
        public void OAuthException_Serialization_ShouldWork()
        {
            
            var originalException = new OAuthException("Test message", new InvalidOperationException("Inner"));
            Assert.IsNotNull(originalException);
            Assert.AreEqual("Test message", originalException.Message);
            Assert.IsNotNull(originalException.InnerException);
        }

        [TestMethod]
        public void OAuthException_ToString_ShouldIncludeMessage()
        {
            
            var message = "Test OAuth error message";
            var exception = new OAuthException(message);
            var result = exception.ToString();
            Assert.IsTrue(result.Contains(message));
            Assert.IsTrue(result.Contains("OAuthException"));
        }

        [TestMethod]
        public void OAuthException_WithInnerException_ToString_ShouldIncludeBoth()
        {
            
            var message = "Test OAuth error message";
            var innerMessage = "Inner exception message";
            var innerException = new InvalidOperationException(innerMessage);
            var exception = new OAuthException(message, innerException);
            var result = exception.ToString();
            Assert.IsTrue(result.Contains(message));
            Assert.IsTrue(result.Contains(innerMessage));
            Assert.IsTrue(result.Contains("OAuthException"));
            Assert.IsTrue(result.Contains("InvalidOperationException"));
        }
    }
}
