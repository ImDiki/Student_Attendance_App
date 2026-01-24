using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Views;

namespace Student_Attendance_System.Services
{
    class AdminService
    {
        // 🔹 ကျောင်းသားအရေအတွက် တွက်ချက်ခြင်း
        public static int GetTotalStudents()
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            string sql = "SELECT COUNT(*) FROM Students";
            using SqlCommand cmd = new SqlCommand(sql, con);
            return (int)cmd.ExecuteScalar();
        }

        // 🔹 ဆရာမအရေအတွက် တွက်ချက်ခြင်း
        public static int GetTotalTeachers()
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            string sql = "SELECT COUNT(*) FROM Teachers";
            using SqlCommand cmd = new SqlCommand(sql, con);
            return (int)cmd.ExecuteScalar();
        }
    }
}
