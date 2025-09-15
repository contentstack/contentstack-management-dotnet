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

            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            Assert.IsNotNull(codeVerifier);
            Assert.IsTrue(PkceHelper.IsValidCodeVerifier(codeVerifier));
            Assert.IsTrue(codeVerifier.Length >= 43);
            Assert.IsTrue(codeVerifier.Length <= 128);
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeVerifier_MultipleCalls_ShouldReturnDifferentValues()
        {

            var verifier1 = PkceHelper.GenerateCodeVerifier();
            var verifier2 = PkceHelper.GenerateCodeVerifier();
            Assert.AreNotEqual(verifier1, verifier2);
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeChallenge_WithValidCodeVerifier_ShouldReturnValidChallenge()
        {
            
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);
            Assert.IsNotNull(codeChallenge);
            Assert.IsTrue(PkceHelper.IsValidCodeChallenge(codeChallenge));
            Assert.AreEqual(43, codeChallenge.Length); // Base64URL encoded SHA256 hash is always 43 characters
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeChallenge_WithSameCodeVerifier_ShouldReturnSameChallenge()
        {
            
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var challenge1 = PkceHelper.GenerateCodeChallenge(codeVerifier);
            var challenge2 = PkceHelper.GenerateCodeChallenge(codeVerifier);
            Assert.AreEqual(challenge1, challenge2);
        }

        [TestMethod]
        public void PkceHelper_GenerateCodeChallenge_WithDifferentCodeVerifiers_ShouldReturnDifferentChallenges()
        {
            
            var codeVerifier1 = PkceHelper.GenerateCodeVerifier();
            var codeVerifier2 = PkceHelper.GenerateCodeVerifier();
            var challenge1 = PkceHelper.GenerateCodeChallenge(codeVerifier1);
            var challenge2 = PkceHelper.GenerateCodeChallenge(codeVerifier2);
            Assert.AreNotEqual(challenge1, challenge2);
        }

        [TestMethod]
        public void PkceHelper_VerifyCodeChallenge_WithValidPair_ShouldReturnTrue()
        {
            
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);
            var isValid = PkceHelper.VerifyCodeChallenge(codeVerifier, codeChallenge);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void PkceHelper_VerifyCodeChallenge_WithInvalidPair_ShouldReturnFalse()
        {
            
            var codeVerifier1 = PkceHelper.GenerateCodeVerifier();
            var codeVerifier2 = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier1);
            var isValid = PkceHelper.VerifyCodeChallenge(codeVerifier2, codeChallenge);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_GeneratePkcePair_ShouldReturnValidPair()
        {

            var pkcePair = PkceHelper.GeneratePkcePair();
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
            
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var isValid = PkceHelper.IsValidCodeVerifier(codeVerifier);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithNullVerifier_ShouldReturnFalse()
        {

            var isValid = PkceHelper.IsValidCodeVerifier(null);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithEmptyVerifier_ShouldReturnFalse()
        {

            var isValid = PkceHelper.IsValidCodeVerifier("");
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithTooShortVerifier_ShouldReturnFalse()
        {
            
            var shortVerifier = "short"; // Less than 43 characters
            var isValid = PkceHelper.IsValidCodeVerifier(shortVerifier);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithTooLongVerifier_ShouldReturnFalse()
        {
            
            var longVerifier = new string('a', 129); // More than 128 characters
            var isValid = PkceHelper.IsValidCodeVerifier(longVerifier);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeVerifier_WithInvalidCharacters_ShouldReturnFalse()
        {
            
            var invalidVerifier = "invalid-characters!@#$%^&*()"; // Contains invalid characters
            var isValid = PkceHelper.IsValidCodeVerifier(invalidVerifier);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithValidChallenge_ShouldReturnTrue()
        {
            
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);
            var isValid = PkceHelper.IsValidCodeChallenge(codeChallenge);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithNullChallenge_ShouldReturnFalse()
        {

            var isValid = PkceHelper.IsValidCodeChallenge(null);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithEmptyChallenge_ShouldReturnFalse()
        {

            var isValid = PkceHelper.IsValidCodeChallenge("");
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithWrongLengthChallenge_ShouldReturnFalse()
        {
            
            var wrongLengthChallenge = "wrong-length"; // Should be 43 characters
            var isValid = PkceHelper.IsValidCodeChallenge(wrongLengthChallenge);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_IsValidCodeChallenge_WithInvalidCharacters_ShouldReturnFalse()
        {
            
            var invalidChallenge = "invalid-characters!@#$%^&*()"; // Contains invalid characters
            var isValid = PkceHelper.IsValidCodeChallenge(invalidChallenge);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void PkceHelper_CodeVerifier_ShouldBeUrlSafe()
        {

            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            Assert.IsFalse(codeVerifier.Contains("+"));
            Assert.IsFalse(codeVerifier.Contains("/"));
            Assert.IsFalse(codeVerifier.Contains("="));
        }

        [TestMethod]
        public void PkceHelper_CodeChallenge_ShouldBeUrlSafe()
        {
            
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);
            Assert.IsFalse(codeChallenge.Contains("+"));
            Assert.IsFalse(codeChallenge.Contains("/"));
            Assert.IsFalse(codeChallenge.Contains("="));
        }

        [TestMethod]
        public void PkceHelper_CodeVerifier_ShouldContainOnlyValidCharacters()
        {

            var codeVerifier = PkceHelper.GenerateCodeVerifier();
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
            
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);
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