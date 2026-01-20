using Microsoft.Data.SqlClient;
using Student_Attendance_System.Models;
using Student_Attendance_System.Views;
using System.Security.Cryptography;
using System.Text;

namespace Student_Attendance_System.Services
{
    public class AuthService
    {
        public User AuthenticateUser(string username, string password)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string hashedPassword = HashPassword(password);

            string sql = @"
                SELECT UserId, Username, Role
                FROM Users
                WHERE Username = @u
                  AND PasswordHash = @p
                  AND IsActive = 1";

            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", hashedPassword);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new User
                {
                    UserId = dr.GetInt32(0),
                    Username = dr.GetString(1),
                    Role = dr.GetString(2)
                };
            }

            return null;
        }

        private string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
