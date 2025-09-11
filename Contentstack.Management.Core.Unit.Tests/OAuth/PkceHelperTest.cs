using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Unit.Tests.OAuth
{
    [TestClass]
    public class PkceHelperTest
    {
        [TestMethod]
        public void PkceHelper_GenerateCodeVerifier_ShouldReturnValidCodeVerifier()
        {
            // Act
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Assert
            Assert.IsNotNull(codeVerifier);
            Assert.IsTrue(PkceHelper.IsValidCodeVerifier(codeVerifier));
            Assert.IsTrue(codeVerifier.Length >= 43);
            Assert.IsTrue(codeVerifier.Length <= 128);
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeVerifier_MultipleCalls_ShouldReturnDifferentValues()
        {
            // Act
            var verifier1 = PkceHelper.GenerateCodeVerifier();
            var verifier2 = PkceHelper.GenerateCodeVerifier();

            // Assert
            Assert.AreNotEqual(verifier1, verifier2);
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeChallenge_WithValidCodeVerifier_ShouldReturnValidChallenge()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Act
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

            // Assert
            Assert.IsNotNull(codeChallenge);
            Assert.IsTrue(PkceHelper.IsValidCodeChallenge(codeChallenge));
            Assert.AreEqual(43, codeChallenge.Length); // Base64URL encoded SHA256 hash is always 43 characters
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeChallenge_WithSameCodeVerifier_ShouldReturnSameChallenge()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Act
            var challenge1 = PkceHelper.GenerateCodeChallenge(codeVerifier);
            var challenge2 = PkceHelper.GenerateCodeChallenge(codeVerifier);

            // Assert
            Assert.AreEqual(challenge1, challenge2);
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeChallenge_WithDifferentCodeVerifiers_ShouldReturnDifferentChallenges()
        {
            // Arrange
            var codeVerifier1 = PkceHelper.GenerateCodeVerifier();
            var codeVerifier2 = PkceHelper.GenerateCodeVerifier();

            // Act
            var challenge1 = PkceHelper.GenerateCodeChallenge(codeVerifier1);
            var challenge2 = PkceHelper.GenerateCodeChallenge(codeVerifier2);

            // Assert
            Assert.AreNotEqual(challenge1, challenge2);
        }

        [TestMethod]
        public void PkceHelper_VerifyCodeChallenge_WithValidPair_ShouldReturnTrue()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

            // Act
            var isValid = PkceHelper.VerifyCodeChallenge(codeVerifier, codeChallenge);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void PkceHelper_VerifyCodeChallenge_WithInvalidPair_ShouldReturnFalse()
        {
            // Arrange
            var codeVerifier1 = PkceHelper.GenerateCodeVerifier();
            var codeVerifier2 = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier1);

            // Act
            var isValid = PkceHelper.VerifyCodeChallenge(codeVerifier2, codeChallenge);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_GeneratePkcePair_ShouldReturnValidPair()
        {
            // Act
            var pkcePair = PkceHelper.GeneratePkcePair();

            // Assert
            Assert.IsNotNull(pkcePair);
            Assert.IsNotNull(pkcePair.CodeVerifier);
            Assert.IsNotNull(pkcePair.CodeChallenge);
            Assert.IsTrue(PkceHelper.IsValidCodeVerifier(pkcePair.CodeVerifier));
            Assert.IsTrue(PkceHelper.IsValidCodeChallenge(pkcePair.CodeChallenge));
            Assert.IsTrue(PkceHelper.VerifyCodeChallenge(pkcePair.CodeVerifier, pkcePair.CodeChallenge));
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithValidVerifier_ShouldReturnTrue()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Act
            var isValid = PkceHelper.IsValidCodeVerifier(codeVerifier);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithNullVerifier_ShouldReturnFalse()
        {
            // Act
            var isValid = PkceHelper.IsValidCodeVerifier(null);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithEmptyVerifier_ShouldReturnFalse()
        {
            // Act
            var isValid = PkceHelper.IsValidCodeVerifier("");

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithTooShortVerifier_ShouldReturnFalse()
        {
            // Arrange
            var shortVerifier = "short"; // Less than 43 characters

            // Act
            var isValid = PkceHelper.IsValidCodeVerifier(shortVerifier);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithTooLongVerifier_ShouldReturnFalse()
        {
            // Arrange
            var longVerifier = new string('a', 129); // More than 128 characters

            // Act
            var isValid = PkceHelper.IsValidCodeVerifier(longVerifier);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithInvalidCharacters_ShouldReturnFalse()
        {
            // Arrange
            var invalidVerifier = "invalid-characters!@#$%^&*()"; // Contains invalid characters

            // Act
            var isValid = PkceHelper.IsValidCodeVerifier(invalidVerifier);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithValidChallenge_ShouldReturnTrue()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

            // Act
            var isValid = PkceHelper.IsValidCodeChallenge(codeChallenge);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithNullChallenge_ShouldReturnFalse()
        {
            // Act
            var isValid = PkceHelper.IsValidCodeChallenge(null);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithEmptyChallenge_ShouldReturnFalse()
        {
            // Act
            var isValid = PkceHelper.IsValidCodeChallenge("");

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithWrongLengthChallenge_ShouldReturnFalse()
        {
            // Arrange
            var wrongLengthChallenge = "wrong-length"; // Should be 43 characters

            // Act
            var isValid = PkceHelper.IsValidCodeChallenge(wrongLengthChallenge);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithInvalidCharacters_ShouldReturnFalse()
        {
            // Arrange
            var invalidChallenge = "invalid-characters!@#$%^&*()"; // Contains invalid characters

            // Act
            var isValid = PkceHelper.IsValidCodeChallenge(invalidChallenge);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_CodeVerifier_ShouldBeUrlSafe()
        {
            // Act
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Assert
            Assert.IsFalse(codeVerifier.Contains("+"));
            Assert.IsFalse(codeVerifier.Contains("/"));
            Assert.IsFalse(codeVerifier.Contains("="));
        }

        [TestMethod]
        public void PkceHelper_CodeChallenge_ShouldBeUrlSafe()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Act
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

            // Assert
            Assert.IsFalse(codeChallenge.Contains("+"));
            Assert.IsFalse(codeChallenge.Contains("/"));
            Assert.IsFalse(codeChallenge.Contains("="));
        }

        [TestMethod]
        public void PkceHelper_CodeVerifier_ShouldContainOnlyValidCharacters()
        {
            // Act
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Assert
            foreach (char c in codeVerifier)
            {
                Assert.IsTrue(
                    char.IsLetterOrDigit(c) || c == '-' || c == '.' || c == '_' || c == '~',
                    $"Character '{c}' is not valid in code verifier"
                );
            }
        }

        [TestMethod]
        public void PkceHelper_CodeChallenge_ShouldContainOnlyValidCharacters()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Act
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

            // Assert
            foreach (char c in codeChallenge)
            {
                Assert.IsTrue(
                    char.IsLetterOrDigit(c) || c == '-' || c == '.' || c == '_' || c == '~',
                    $"Character '{c}' is not valid in code challenge"
                );
            }
        }
    }
}