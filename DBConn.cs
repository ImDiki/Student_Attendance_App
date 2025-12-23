
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Data
{
    public static class DBConn
    {
        private static string conStr =
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\Si Thu Bo Sann\source\repos\version01\Database\Database.mdf"";Integrated Security=True";

        public static User Login(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                string sql = @"SELECT * FROM Users
                               WHERE Username=@u AND PasswordHash=@p";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    { 
                        if (rd.Read())
                        {
                            return new User
                            {
                                //Id = (int)rd["Id"],
                                Username = rd["Username"].ToString(),
                                Role = rd["Role"].ToString(),
                                FullName = rd["FullName"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        //STUDENT REGISTER
        public static bool RegisterStudent(string sid, string name, DateTime birthday, string subject)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();

                    string sql = @"INSERT INTO Students
                                   (StudentId, StudentName, Birthday, Subject)
                                   VALUES (@sid, @name, @birth, @subject)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue ("@sid", sid);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@birth", birthday);
                        cmd.Parameters.AddWithValue("@subject", subject);

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        internal static bool RegisterStudent(string name, DateTime value, string subject)
        {
            throw new NotImplementedException();
        }
    }
}
