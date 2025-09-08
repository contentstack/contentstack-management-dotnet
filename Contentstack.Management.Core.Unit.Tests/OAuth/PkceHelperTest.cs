using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Unit.Tests.OAuth
{
    [TestClass]
    public class PkceHelperTest
    {
        [TestMethod]
        public void PkceHelper_GenerateCodeVerifier_ShouldReturnValidVerifier()
        {
            // Act
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Assert
            Assert.IsNotNull(codeVerifier);
            Assert.IsTrue(codeVerifier.Length >= 43);
            Assert.IsTrue(codeVerifier.Length <= 128);
            Assert.IsTrue(PkceHelper.IsValidCodeVerifier(codeVerifier));
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
        public void PkceHelper_GenerateCodeChallenge_ShouldReturnValidChallenge()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Act
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

            // Assert
            Assert.IsNotNull(codeChallenge);
            Assert.AreEqual(43, codeChallenge.Length);
            Assert.IsTrue(PkceHelper.IsValidCodeChallenge(codeChallenge));
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeChallenge_SameVerifier_ShouldReturnSameChallenge()
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
        public void PkceHelper_GenerateCodeChallenge_DifferentVerifiers_ShouldReturnDifferentChallenges()
        {
            // Arrange
            var verifier1 = PkceHelper.GenerateCodeVerifier();
            var verifier2 = PkceHelper.GenerateCodeVerifier();

            // Act
            var challenge1 = PkceHelper.GenerateCodeChallenge(verifier1);
            var challenge2 = PkceHelper.GenerateCodeChallenge(verifier2);

            // Assert
            Assert.AreNotEqual(challenge1, challenge2);
        }

        [TestMethod]
        public void PkceHelper_VerifyCodeChallenge_WithValidPair_ShouldReturnTrue()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

            // Act & Assert
            Assert.IsTrue(PkceHelper.VerifyCodeChallenge(codeVerifier, codeChallenge));
        }

        [TestMethod]
        public void PkceHelper_VerifyCodeChallenge_WithInvalidChallenge_ShouldReturnFalse()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var invalidChallenge = "invalid-challenge";

            // Act & Assert
            Assert.IsFalse(PkceHelper.VerifyCodeChallenge(codeVerifier, invalidChallenge));
        }

        [TestMethod]
        public void PkceHelper_VerifyCodeChallenge_WithInvalidVerifier_ShouldReturnFalse()
        {
            // Arrange
            var invalidVerifier = "invalid-verifier";
            var codeChallenge = PkceHelper.GenerateCodeChallenge(PkceHelper.GenerateCodeVerifier());

            // Act & Assert
            Assert.IsFalse(PkceHelper.VerifyCodeChallenge(invalidVerifier, codeChallenge));
        }

        [TestMethod]
        public void PkceHelper_GeneratePkcePair_ShouldReturnValidPair()
        {
            // Act
            var (verifier, challenge) = PkceHelper.GeneratePkcePair();

            // Assert
            Assert.IsNotNull(verifier);
            Assert.IsNotNull(challenge);
            Assert.IsTrue(PkceHelper.IsValidCodeVerifier(verifier));
            Assert.IsTrue(PkceHelper.IsValidCodeChallenge(challenge));
            Assert.IsTrue(PkceHelper.VerifyCodeChallenge(verifier, challenge));
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithValidVerifier_ShouldReturnTrue()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Act & Assert
            Assert.IsTrue(PkceHelper.IsValidCodeVerifier(codeVerifier));
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithTooShortVerifier_ShouldReturnFalse()
        {
            // Arrange
            var shortVerifier = "short";

            // Act & Assert
            Assert.IsFalse(PkceHelper.IsValidCodeVerifier(shortVerifier));
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithTooLongVerifier_ShouldReturnFalse()
        {
            // Arrange
            var longVerifier = new string('a', 129); // 129 characters

            // Act & Assert
            Assert.IsFalse(PkceHelper.IsValidCodeVerifier(longVerifier));
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithInvalidCharacters_ShouldReturnFalse()
        {
            // Arrange
            var invalidVerifier = "invalid+characters!@#";

            // Act & Assert
            Assert.IsFalse(PkceHelper.IsValidCodeVerifier(invalidVerifier));
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithValidChallenge_ShouldReturnTrue()
        {
            // Arrange
            var codeChallenge = PkceHelper.GenerateCodeChallenge(PkceHelper.GenerateCodeVerifier());

            // Act & Assert
            Assert.IsTrue(PkceHelper.IsValidCodeChallenge(codeChallenge));
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithWrongLength_ShouldReturnFalse()
        {
            // Arrange
            var wrongLengthChallenge = "wrong-length";

            // Act & Assert
            Assert.IsFalse(PkceHelper.IsValidCodeChallenge(wrongLengthChallenge));
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithInvalidCharacters_ShouldReturnFalse()
        {
            // Arrange
            var invalidChallenge = "invalid+characters!@#";

            // Act & Assert
            Assert.IsFalse(PkceHelper.IsValidCodeChallenge(invalidChallenge));
        }

        [TestMethod]
        public void PkceHelper_CodeVerifier_ShouldContainOnlyValidCharacters()
        {
            // Arrange & Act
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Assert - Should only contain URL-safe base64 characters
            var validPattern = @"^[A-Za-z0-9\-._~]+$";
            Assert.IsTrue(Regex.IsMatch(codeVerifier, validPattern), 
                $"Code verifier contains invalid characters: {codeVerifier}");
        }

        [TestMethod]
        public void PkceHelper_CodeChallenge_ShouldContainOnlyValidCharacters()
        {
            // Arrange
            var codeVerifier = PkceHelper.GenerateCodeVerifier();

            // Act
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

            // Assert - Should only contain URL-safe base64 characters
            var validPattern = @"^[A-Za-z0-9\-._~]+$";
            Assert.IsTrue(Regex.IsMatch(codeChallenge, validPattern), 
                $"Code challenge contains invalid characters: {codeChallenge}");
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeVerifier_ShouldBeCryptographicallySecure()
        {
            // Arrange
            var verifiers = new string[100];

            // Act
            for (int i = 0; i < 100; i++)
            {
                verifiers[i] = PkceHelper.GenerateCodeVerifier();
            }

            // Assert - All verifiers should be unique
            var uniqueVerifiers = new System.Collections.Generic.HashSet<string>(verifiers);
            Assert.AreEqual(100, uniqueVerifiers.Count, "Generated code verifiers should be unique");
        }
    }
}


