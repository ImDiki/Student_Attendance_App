using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Student_Attendance_System.Models;
using Student_Attendance_System.Views;

namespace Student_Attendance_System.Services
{
    public static class AttendanceService
    {
        // 🔹 Insert Present
        public static void MarkPresent(string studentId, string subject)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = @"
                INSERT INTO Attendance
                (StudentID, Subject, AttendanceDate, AttendanceTime, Status, Note)
                VALUES
                (@sid, @subj, CAST(GETDATE() AS DATE), CAST(GETDATE() AS TIME), 'Present', NULL)
            ";

            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@sid", studentId);
            cmd.Parameters.AddWithValue("@subj", subject);

            cmd.ExecuteNonQuery();
        }

        // 🔹 Check existing attendance (for double scan)
        public static string GetTodayStatus(string studentId, string subject)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = @"
                SELECT Status FROM Attendance
                WHERE StudentID = @sid
                  AND Subject = @subj
                  AND AttendanceDate = CAST(GETDATE() AS DATE)
            ";

            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@sid", studentId);
            cmd.Parameters.AddWithValue("@subj", subject);

            object result = cmd.ExecuteScalar();
            return result?.ToString(); // null if not exists
        }

        // 🔹 GET TODAY ATTENDANCE (FOR TEACHER DASHBOARD)
        public static List<AttendanceRecord> GetTodayAttendance(string subject)
        {
            var list = new List<AttendanceRecord>();

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = @"
                SELECT * FROM Attendance
                WHERE Subject = @subj
                  AND AttendanceDate = CAST(GETDATE() AS DATE)
                ORDER BY AttendanceTime
            ";

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
