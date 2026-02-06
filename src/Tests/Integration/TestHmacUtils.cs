using System.Security.Cryptography;
using System.Text;

namespace Tests.Integration
{
    public static class TestHmacUtils
    {
        // Single place to define the test secret. Keep it simple and stable.
        public const string TestHmacSecret = "test-hmac-secret-key";

        /// <summary>
        /// Generates a HMAC SHA-256 signature string formatted like the incoming header: "sha256={hex}".
        /// </summary>
        public static string GenerateHmacSignature(string payload)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(TestHmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var hex = Convert.ToHexString(hash).ToLowerInvariant();
            return $"sha256={hex}";
        }
    }
}
