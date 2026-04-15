using System.Security.Cryptography;
using System.Text;

namespace SchoolERP.Net.Utilities
{
    public static class SecurityHelper
    {
        private const int SaltBytes = 32;        // 256-bit salt
        private const int HashBytes = 32;        // 256-bit hash
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName Algo = HashAlgorithmName.SHA256;

        /// <summary>
        /// Hashes a password using PBKDF2-SHA256.
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Tuple of (Base64 Hash, Base64 Salt)</returns>
        public static (string Hash, string Salt) HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) 
                throw new ArgumentException("Password cannot be empty", nameof(password));

            byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltBytes);
            byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                Iterations,
                Algo,
                HashBytes);

            return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
        }

        /// <summary>
        /// Verifies a password against a stored hash and salt.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            try
            {
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(password),
                    saltBytes,
                    Iterations,
                    Algo,
                    HashBytes);

                return CryptographicOperations.FixedTimeEquals(hashBytes, Convert.FromBase64String(storedHash));
            }
            catch
            {
                return false;
            }
        }
    }
}
