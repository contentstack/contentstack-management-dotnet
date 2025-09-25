using System;
using System.Security.Cryptography;
using System.Text;

namespace Contentstack.Management.Core.Utils
{
    /// <summary>
    /// Helper class for PKCE (Proof Key for Code Exchange) operations in OAuth 2.0.
    /// PKCE enhances security for OAuth flows, especially for public clients that cannot securely store client secrets.
    /// </summary>
    public static class PkceHelper
    {
        /// <summary>
        /// Generates a cryptographically random code verifier for PKCE.
        /// The code verifier is a high-entropy cryptographic random string.
        /// </summary>
        /// <returns>A URL-safe base64-encoded code verifier.</returns>
        /// <exception cref="CryptographicException">Thrown when cryptographic operations fail.</exception>
        public static string GenerateCodeVerifier()
        {
            try
            {
                // Generate 32 random bytes (256 bits)
                var bytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }

                // Convert to URL-safe base64 string
                return Convert.ToBase64String(bytes)
                    .TrimEnd('=')  // Remove padding
                    .Replace('+', '-')  // Replace + with -
                    .Replace('/', '_'); // Replace / with _
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to generate code verifier", ex);
            }
        }

        /// <summary>
        /// Generates a code challenge from a code verifier using SHA256.
        /// The code challenge is the SHA256 hash of the code verifier, base64url-encoded.
        /// </summary>
        /// <param name="codeVerifier">The code verifier to hash.</param>
        /// <returns>A URL-safe base64-encoded code challenge.</returns>
        /// <exception cref="ArgumentNullException">Thrown when codeVerifier is null or empty.</exception>
        /// <exception cref="CryptographicException">Thrown when cryptographic operations fail.</exception>
        public static string GenerateCodeChallenge(string codeVerifier)
        {
            if (string.IsNullOrEmpty(codeVerifier))
                throw new ArgumentNullException(nameof(codeVerifier), "Code verifier cannot be null or empty.");

            try
            {
                // Compute SHA256 hash of the code verifier
                using (var sha256 = SHA256.Create())
                {
                    var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                    
                    // Convert to URL-safe base64 string
                    return Convert.ToBase64String(challengeBytes)
                        .TrimEnd('=')  // Remove padding
                        .Replace('+', '-')  // Replace + with -
                        .Replace('/', '_'); // Replace / with _
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to generate code challenge", ex);
            }
        }

        /// <summary>
        /// Validates a code verifier format.
        /// A valid code verifier must be 43-128 characters long and contain only URL-safe characters.
        /// </summary>
        /// <param name="codeVerifier">The code verifier to validate.</param>
        /// <returns>True if the code verifier is valid, false otherwise.</returns>
        public static bool IsValidCodeVerifier(string codeVerifier)
        {
            if (string.IsNullOrEmpty(codeVerifier))
                return false;

            // Check length (43-128 characters as per RFC 7636)
            if (codeVerifier.Length < 43 || codeVerifier.Length > 128)
                return false;

            // Check for URL-safe characters only (A-Z, a-z, 0-9, -, _, .)
            foreach (char c in codeVerifier)
            {
                if (!IsUrlSafeCharacter(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validates a code challenge format.
        /// A valid code challenge must be 43 characters long and contain only URL-safe characters.
        /// </summary>
        /// <param name="codeChallenge">The code challenge to validate.</param>
        /// <returns>True if the code challenge is valid, false otherwise.</returns>
        public static bool IsValidCodeChallenge(string codeChallenge)
        {
            if (string.IsNullOrEmpty(codeChallenge))
                return false;

            // SHA256 hash in base64url should be exactly 43 characters
            if (codeChallenge.Length != 43)
                return false;

            // Check for URL-safe characters only
            foreach (char c in codeChallenge)
            {
                if (!IsUrlSafeCharacter(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies that a code challenge matches a code verifier.
        /// This is used during the token exchange to ensure the client possesses the original code verifier.
        /// </summary>
        /// <param name="codeVerifier">The original code verifier.</param>
        /// <param name="codeChallenge">The code challenge to verify against.</param>
        /// <returns>True if the code challenge matches the code verifier, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either parameter is null or empty.</exception>
        public static bool VerifyCodeChallenge(string codeVerifier, string codeChallenge)
        {
            if (string.IsNullOrEmpty(codeVerifier))
                throw new ArgumentNullException(nameof(codeVerifier), "Code verifier cannot be null or empty.");

            if (string.IsNullOrEmpty(codeChallenge))
                throw new ArgumentNullException(nameof(codeChallenge), "Code challenge cannot be null or empty.");

            try
            {
                // Generate the expected code challenge from the verifier
                var expectedChallenge = GenerateCodeChallenge(codeVerifier);
                
                // Compare using constant-time comparison to prevent timing attacks
                return ConstantTimeEquals(expectedChallenge, codeChallenge);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a complete PKCE pair (code verifier and code challenge).
        /// </summary>
        /// <returns>A tuple containing the code verifier and code challenge.</returns>
        /// <exception cref="CryptographicException">Thrown when cryptographic operations fail.</exception>
        public static (string CodeVerifier, string CodeChallenge) GeneratePkcePair()
        {
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            return (codeVerifier, codeChallenge);
        }

        /// <summary>
        /// Checks if a character is URL-safe according to RFC 3986.
        /// URL-safe characters are: A-Z, a-z, 0-9, -, _, ., ~
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is URL-safe, false otherwise.</returns>
        private static bool IsUrlSafeCharacter(char c)
        {
            return (c >= 'A' && c <= 'Z') ||  // A-Z
                   (c >= 'a' && c <= 'z') ||  // a-z
                   (c >= '0' && c <= '9') ||  // 0-9
                   c == '-' || c == '_' || c == '.' || c == '~';  // Special URL-safe characters
        }

        /// <summary>
        /// Performs a constant-time string comparison to prevent timing attacks.
        /// </summary>
        /// <param name="a">First string to compare.</param>
        /// <param name="b">Second string to compare.</param>
        /// <returns>True if strings are equal, false otherwise.</returns>
        private static bool ConstantTimeEquals(string a, string b)
        {
            if (a == null || b == null)
                return a == b;

            if (a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }
}


