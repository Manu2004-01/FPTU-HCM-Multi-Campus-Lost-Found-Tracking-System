using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            // Check if hash is a valid BCrypt hash (starts with $2a$, $2b$, $2x$, or $2y$)
            if (hash != null && (hash.StartsWith("$2a$") || hash.StartsWith("$2b$") || hash.StartsWith("$2x$") || hash.StartsWith("$2y$")))
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(password, hash);
                }
                catch
                {
                    // If BCrypt verification fails, fall back to plain text comparison
                    return password == hash;
                }
            }
            // If not a BCrypt hash, compare as plain text (for migration purposes)
            return password == hash;
        }
    }
}
