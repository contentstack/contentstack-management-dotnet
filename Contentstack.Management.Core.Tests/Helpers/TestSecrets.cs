using System;
using System.Linq;
using System.Security.Cryptography;

namespace Contentstack.Management.Core.Tests.Helpers
{
    /// <summary>
    /// Per-run generated values for tests that need credential-shaped inputs but must not
    /// commit literal secrets. None of these are real credentials.
    /// </summary>
    internal static class TestSecrets
    {
        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string Password => Guid.NewGuid().ToString("N");

        public static string Username => "user-" + Guid.NewGuid().ToString("N").Substring(0, 12) + "@example.com";

        public static string Token() => Guid.NewGuid().ToString("N");

        /// <summary>A stack API key-shaped value (blt + hex) that does not exist on any stack.</summary>
        public static string BltKey() => "blt" + Guid.NewGuid().ToString("N").Substring(0, 16);

        /// <summary>A valid RFC 4648 base32 string, suitable as a TOTP secret.</summary>
        public static string Base32Secret()
        {
            var bytes = new byte[10];
            RandomNumberGenerator.Fill(bytes);
            return new string(bytes.Select(b => Base32Alphabet[b % 32]).ToArray());
        }

        /// <summary>A string that is not valid base32 (contains characters outside the alphabet).</summary>
        public static string InvalidBase32Secret() => "!@#" + Guid.NewGuid().ToString("N");

        /// <summary>Alternates upper/lower case; used to check case-insensitive secret handling.</summary>
        public static string MixedCase(string value) =>
            new string(value.Select((c, i) => i % 2 == 0 ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c)).ToArray());
    }
}
