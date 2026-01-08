using Microsoft.Data.SqlClient;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Services
{
    public class AuthService
    {
        public User AuthenticateUser(string login, string password)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

                // ======================
                // 1️⃣ ADMIN LOGIN
                // ======================
                string adminSql = @"
                    SELECT AdminID, Username, FullName, Role
                    FROM Admins
                    WHERE Username = @login AND PasswordHash = @password";

                User admin = TryLogin(con, adminSql, login, password, "AdminID");
                if (admin != null) return admin;

                // ======================
                // 2️⃣ TEACHER LOGIN
                // ======================
                string teacherSql = @"
                    SELECT TeacherID, Username, FullName, Role
                    FROM Teachers
                    WHERE Username = @login AND PasswordHash = @password";

                User teacher = TryLogin(con, teacherSql, login, password, "TeacherID");
                if (teacher != null) return teacher;

                // ======================
                // 3️⃣ STUDENT LOGIN (StudentID)
                // ======================
                string studentSql = @"
                    SELECT StudentID, FullName, Role
                    FROM Students
                    WHERE StudentID = @login AND PasswordHash = @password";

                User student = TryLogin(con, studentSql, login, password, "StudentID");
                if (student != null) return student;
            }

            return null;
        }

        // ==================================================
        // 🔁 COMMON LOGIN METHOD
        // ==================================================
        private User TryLogin(SqlConnection con, string sql, string login, string password, string idColumn)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@password", password);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    return new User
                    {
                        UserID = reader[idColumn].ToString(), // ✅ NO int.Parse
                        Username = login,
                        FullName = reader["FullName"].ToString(),
                        Role = reader["Role"].ToString()
                    };
                }
            }
        }
    }
}