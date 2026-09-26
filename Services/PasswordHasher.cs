using BCrypt.Net;

namespace Student_Attendance_System.Services
{
    public static class PasswordHasher
    {
        private const int WorkFactor = 12; // reasonable default

        public static string Hash(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
        }

        public static bool Verify(string plainPassword, string hashed)
        {
            if (string.IsNullOrEmpty(hashed)) return false;
            try
            {
                return BCrypt.Net.BCrypt.Verify(plainPassword, hashed);
            }
            catch
            {
                return false;
            }
        }

        // Helper to detect if a stored hash looks like a bcrypt hash
        public static bool IsBcryptHash(string hash)
        {
            return !string.IsNullOrEmpty(hash) && hash.StartsWith("$2");
        }
    }
}
