using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Student_Attendance_System.Models;
using Student_Attendance_System.Views;

namespace Student_Attendance_System.Services
{
    public static class AttendanceService
    {
        // 🔹 ၁။ ကျောင်းသား Scan ဖတ်သည့်အခါ စာရင်းသွင်းခြင်း
        public static void MarkPresent(string studentId, string subject)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = @"
                INSERT INTO Attendance
                (StudentID, Subject, AttendanceDate, AttendanceTime, Status, Note, CreatedAt)
                VALUES
                (@sid, @subj, CAST(GETDATE() AS DATE), CAST(GETDATE() AS TIME), 'Present', NULL, GETDATE())
            ";

            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@sid", studentId);
            cmd.Parameters.AddWithValue("@subj", subject);

            cmd.ExecuteNonQuery();
        }

        // 🔹 ၂။ Attendance Percentage (%) ကို တွက်ချက်ခြင်း
        public static double GetAttendancePercentage(string studentId)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = @"
                SELECT 
                    CASE 
                        WHEN COUNT(*) = 0 THEN 0 
                        ELSE (CAST(SUM(CASE WHEN Status = 'Present' THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*)) * 100 
                    END
                FROM Attendance
                WHERE StudentID = @sid
            ";

            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@sid", studentId);

            object result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToDouble(result) : 0;
        }

        // 🔹 ၃။ Total Classes Count တွက်ခြင်း (Dashboard အတွက်)
        public static int GetTotalClasses(string studentId)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            string sql = "SELECT COUNT(*) FROM Attendance WHERE StudentID = @sid";
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@sid", studentId);
            return (int)cmd.ExecuteScalar();
        }

        // 🔹 ၄။ Present Count တွက်ခြင်း (Dashboard အတွက်)
        public static int GetPresentCount(string studentId)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            string sql = "SELECT COUNT(*) FROM Attendance WHERE StudentID = @sid AND Status = 'Present'";
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@sid", studentId);
            return (int)cmd.ExecuteScalar();
        }

        // 🔹 ၅။ Absent Count တွက်ခြင်း (Dashboard အတွက်)
        public static int GetAbsentCount(string studentId)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            string sql = "SELECT COUNT(*) FROM Attendance WHERE StudentID = @sid AND Status = 'Absent'";
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@sid", studentId);
            return (int)cmd.ExecuteScalar();
        }

        // 🔹 ၆။ အတန်းစ၊ မစ စစ်ဆေးခြင်း
        public static bool IsClassActive(string subject)
        {
            try
            {
                using SqlConnection con = DBConnection.GetConnection();
                con.Open();
                string sql = "SELECT IsActive FROM Timetables WHERE SubjectName = @subj";
                using SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@subj", subject);
                object result = cmd.ExecuteScalar();
                return result != null && Convert.ToBoolean(result);
            }
            catch (Exception) { return false; }
        }

        // 🔹 ၇။ ထပ်ခါတလဲလဲ Scan ဖတ်ခြင်းကို ကာကွယ်ရန်
        public static string GetTodayStatus(string studentId, string subject)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            string sql = "SELECT Status FROM Attendance WHERE StudentID = @sid AND Subject = @subj AND AttendanceDate = CAST(GETDATE() AS DATE)";
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@sid", studentId);
            cmd.Parameters.AddWithValue("@subj", subject);
            return cmd.ExecuteScalar()?.ToString();
        }

        // 🔹 ၈။ ဆရာမ Dashboard အတွက် ယနေ့တက်ရောက်သူစာရင်း
        public static List<AttendanceRecord> GetTodayAttendance(string subject)
        {
            var list = new List<AttendanceRecord>();
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            string sql = "SELECT * FROM Attendance WHERE Subject = @subj AND AttendanceDate = CAST(GETDATE() AS DATE) ORDER BY AttendanceTime";
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@subj", subject);
            using SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new AttendanceRecord
                {
                    AttendanceID = (int)r["AttendanceID"],
                    StudentID = r["StudentID"].ToString(),
                    Subject = r["Subject"].ToString(),
                    AttendanceDate = (DateTime)r["AttendanceDate"],
                    AttendanceTime = (TimeSpan)r["AttendanceTime"],
                    Status = r["Status"].ToString(),
                    Note = r["Note"]?.ToString()
                });
            }
            return list;
        }
    }
}