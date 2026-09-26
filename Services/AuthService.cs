using Microsoft.Data.SqlClient;
using Student_Attendance_System.Models;
using System.Security.Cryptography;
using System.Text;
using System;
using Student_Attendance_System.Views;
using Student_Attendance_System.Services;
using System.Data;

namespace Student_Attendance_System.Services
{
    public class AuthService
    {
        // AuthenticateUser now supports bcrypt and legacy SHA256 migration.
        public User? AuthenticateUser(string username, string password)
        {
            try
            {
                using SqlConnection con = DBConnection.GetConnection();
                con.Open();

                string sql = @"
                    SELECT u.UserId, u.Username, u.Role, u.PasswordHash, s.FullName, s.YearLevel, s.Class, s.FacePhoto
                    FROM Users u
                    LEFT JOIN Students s ON UPPER(LTRIM(RTRIM(u.Username))) = UPPER(LTRIM(RTRIM(s.StudentCode)))
                    WHERE u.Username = @u AND u.IsActive = 1";

                using SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = username;

                // Read the row into locals, then close the reader before performing any UPDATE
                int userId;
                string dbUsername;
                string role;
                string storedHash;
                string fullName;
                string yearLevel;
                string assignedClass;
                byte[]? facePhoto;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return null;

                    userId = dr.GetInt32(0);
                    dbUsername = dr.IsDBNull(1) ? "" : dr.GetString(1);
                    role = dr.IsDBNull(2) ? "" : dr.GetString(2);
                    storedHash = dr.IsDBNull(3) ? "" : dr.GetString(3);
                    fullName = dr.IsDBNull(4) ? "" : dr.GetValue(4).ToString() ?? "";
                    yearLevel = dr.IsDBNull(5) ? "" : dr.GetValue(5).ToString() ?? "";
                    assignedClass = dr.IsDBNull(6) ? "" : dr.GetValue(6).ToString() ?? "";
                    facePhoto = dr.IsDBNull(7) ? null : (byte[])dr["FacePhoto"];
                }

                bool verified = false;

                if (PasswordHasher.IsBcryptHash(storedHash))
                {
                    verified = PasswordHasher.Verify(password, storedHash);
                }
                else
                {
                    string sha = ComputeSha256(password);
                    if (string.Equals(sha, storedHash, StringComparison.OrdinalIgnoreCase))
                    {
                        verified = true;
                        // Re-hash with bcrypt and update DB after reader is closed
                        try
                        {
                            string newHash = PasswordHasher.Hash(password);
                            using SqlCommand up = new SqlCommand("UPDATE Users SET PasswordHash = @p WHERE UserId = @id", con);
                            up.Parameters.Add("@p", SqlDbType.NVarChar, 200).Value = newHash;
                            up.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                            up.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Failed to migrate password hash for user " + dbUsername, ex);
                        }
                    }
                }

                if (verified)
                {
                    return new User
                    {
                        UserId = userId,
                        Username = dbUsername,
                        Role = role,
                        FullName = fullName,
                        YearLevel = yearLevel,
                        AssignedClass = assignedClass,
                        FacePhoto = facePhoto
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Login Error", ex);
                System.Windows.MessageBox.Show("Login Error: " + ex.Message);
            }
            return null;
        }

        public bool RegisterStudent(string studentID, string fullName, string password, string dept, string year, string className, byte[] photoBytes)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            using SqlTransaction trans = con.BeginTransaction();
            try
            {
                string passwordHash = PasswordHasher.Hash(password);
                string userSql = "INSERT INTO Users (Username, PasswordHash, Role, IsActive) OUTPUT INSERTED.UserId VALUES (@u, @p, 'Student', 1)";
                using SqlCommand userCmd = new SqlCommand(userSql, con, trans);
                userCmd.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = studentID;
                userCmd.Parameters.Add("@p", SqlDbType.NVarChar, 200).Value = passwordHash;
                int NewuserId = Convert.ToInt32(userCmd.ExecuteScalar());

                string studentSql = "INSERT INTO Students (StudentId, StudentCode, FullName, Department, YearLevel, Class, FacePhoto, EnrollmentDate) VALUES (@id, @sc, @fn, @dept, @yl, @class, @img, @date)";
                using SqlCommand stuCmd = new SqlCommand(studentSql, con, trans);
                stuCmd.Parameters.Add("@id", SqlDbType.Int).Value = NewuserId;
                stuCmd.Parameters.Add("@sc", SqlDbType.NVarChar, 100).Value = studentID;
                stuCmd.Parameters.Add("@fn", SqlDbType.NVarChar, 200).Value = fullName;
                stuCmd.Parameters.Add("@dept", SqlDbType.NVarChar, 100).Value = (object)dept ?? DBNull.Value;
                stuCmd.Parameters.Add("@yl", SqlDbType.NVarChar, 50).Value = (object)year ?? DBNull.Value;
                stuCmd.Parameters.Add("@class", SqlDbType.NVarChar, 100).Value = (object)className ?? DBNull.Value;
                stuCmd.Parameters.Add("@date", SqlDbType.Date).Value = DateTime.Today;
                stuCmd.Parameters.Add("@img", System.Data.SqlDbType.VarBinary).Value = (object)photoBytes ?? DBNull.Value;

                stuCmd.ExecuteNonQuery();
                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                try { trans.Rollback(); } catch { }
                Logger.LogError("RegisterStudent failed", ex);
                System.Windows.MessageBox.Show("Reg Error: " + ex.Message);
                return false;
            }
        }

        private static string ComputeSha256(string password)
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
