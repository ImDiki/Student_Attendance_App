using Microsoft.Data.SqlClient;
using Student_Attendance_System.Models;
using System.Security.Cryptography;
using System.Text;
using System;
using Student_Attendance_System.Views;

namespace Student_Attendance_System.Services
{
    public class AuthService
    {
        public User? AuthenticateUser(string username, string password)
        {
            try
            {
                // DBConnection error ကို ဖြေရှင်းရန်
                using SqlConnection con = DBConnection.GetConnection();
                con.Open();
                string hashedPassword = HashPassword(password);

                string sql = @"
                    SELECT u.UserId, u.Username, u.Role, s.FullName, s.YearLevel, s.Class, s.FacePhoto
                    FROM Users u
                    LEFT JOIN Students s ON UPPER(LTRIM(RTRIM(u.Username))) = UPPER(LTRIM(RTRIM(s.StudentCode)))
                    WHERE u.Username = @u AND u.PasswordHash = @p AND u.IsActive = 1";

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
                        FullName = dr.IsDBNull(3) ? "" : dr.GetValue(3).ToString() ?? "",
                        YearLevel = dr.IsDBNull(4) ? "" : dr.GetValue(4).ToString() ?? "", // Format fix
                        AssignedClass = dr.IsDBNull(5) ? "" : dr.GetValue(5).ToString() ?? "",
                        FacePhoto = dr.IsDBNull(6) ? null : (byte[])dr["FacePhoto"]
                    };
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("Login Error: " + ex.Message); }
            return null; // Return value fix
        }

        public bool RegisterStudent(string studentID, string fullName, string password, string dept, string year, string className, byte[] photoBytes)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            using SqlTransaction trans = con.BeginTransaction();
            try
            {
                string passwordHash = HashPassword(password);
                string userSql = "INSERT INTO Users (Username, PasswordHash, Role, IsActive) OUTPUT INSERTED.UserId VALUES (@u, @p, 'Student', 1)";
                using SqlCommand userCmd = new SqlCommand(userSql, con, trans);
                userCmd.Parameters.AddWithValue("@u", studentID);
                userCmd.Parameters.AddWithValue("@p", passwordHash);
                int NewuserId = Convert.ToInt32(userCmd.ExecuteScalar());

                string studentSql = "INSERT INTO Students (StudentId, StudentCode, FullName, Department, YearLevel, Class, FacePhoto, EnrollmentDate) VALUES (@id, @sc, @fn, @dept, @yl, @class, @img, @date)";
                using SqlCommand stuCmd = new SqlCommand(studentSql, con, trans);
                stuCmd.Parameters.AddWithValue("@id", NewuserId);
                stuCmd.Parameters.AddWithValue("@sc", studentID);
                stuCmd.Parameters.AddWithValue("@fn", fullName);
                stuCmd.Parameters.AddWithValue("@dept", dept);
                stuCmd.Parameters.AddWithValue("@yl", year);
                stuCmd.Parameters.AddWithValue("@class", className);
                stuCmd.Parameters.AddWithValue("@date", DateTime.Today);
                stuCmd.Parameters.Add("@img", System.Data.SqlDbType.VarBinary).Value = (object)photoBytes ?? DBNull.Value;

                stuCmd.ExecuteNonQuery();
                trans.Commit();
                return true;
            }
            catch (Exception ex) { trans.Rollback(); System.Windows.MessageBox.Show("Reg Error: " + ex.Message); return false; }
        }

        private string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}