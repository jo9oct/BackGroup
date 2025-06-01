// WARNING: This is a very basic example. For production, use a strong library like BCrypt.Net-Next.
using System.Security.Cryptography;
using System.Text;

namespace LibraryWebAPI.Services
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // In a real app, you would also generate and store a salt per user.
            // For simplicity, this example uses a static "salt" concept (not recommended for production).
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "STATIC_SALT_FOR_DEMO_ONLY"));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }
}