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

                // 1️⃣ ADMIN LOGIN (CASE-SENSITIVE)
          
                string adminSql = @"
SELECT AdminID, Username, FullName, Role
FROM Admins
WHERE Username COLLATE Latin1_General_CS_AS = @login
  AND PasswordHash COLLATE Latin1_General_CS_AS = @password";

                User admin = TryLogin(con, adminSql, login, password, "AdminID");
                if (admin != null) return admin;

          
                // 2️⃣ TEACHER LOGIN (CASE-SENSITIVE)
               
                string teacherSql = @"
SELECT TeacherID, Username, FullName, Role
FROM Teachers
WHERE Username COLLATE Latin1_General_CS_AS = @login
  AND PasswordHash COLLATE Latin1_General_CS_AS = @password";

                User teacher = TryLogin(con, teacherSql, login, password, "TeacherID");
                if (teacher != null) return teacher;

             
                // 3️⃣ STUDENT LOGIN (CASE-SENSITIVE)
             
                string studentSql = @"
SELECT StudentID, StudentID AS Username, FullName, 'Student' AS Role
FROM Students
WHERE StudentID COLLATE Latin1_General_CS_AS = @login
  AND PasswordHash COLLATE Latin1_General_CS_AS = @password";

                User student = TryLogin(con, studentSql, login, password, "StudentID");
                if (student != null) return student;
            }

            return null;
        }

        // 🔁 COMMON LOGIN METHOD (SAFE)
        
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
                        UserID = reader[idColumn].ToString(),
                        Username = reader["Username"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Role = reader["Role"].ToString()
                    };
                }
            }
        }
    }
}
