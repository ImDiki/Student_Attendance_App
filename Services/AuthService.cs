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
                SELECT 
                    u.UserId,
                    u.Username,
                    u.Role,
                    s.FullName,
                    s.YearLevel,
                    s.Class
                FROM Users u
                LEFT JOIN Students s
                    ON UPPER(LTRIM(RTRIM(u.Username))) 
                     = UPPER(LTRIM(RTRIM(s.StudentCode)))
                WHERE u.Username = @u
                  AND u.PasswordHash = @p
                  AND u.IsActive = 1";

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
                    Role = dr.GetString(2),

                    FullName = dr.IsDBNull(3) ? "" : dr.GetString(3),

                    YearLevel = dr.IsDBNull(4)
                        ? 0
                        : int.Parse(dr.GetString(4)),

                    AssignedClass = dr.IsDBNull(5) ? "" : dr.GetString(5)
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
