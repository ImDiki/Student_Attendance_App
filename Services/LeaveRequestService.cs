using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Student_Attendance_System.Models;
using Student_Attendance_System.Views;

namespace Student_Attendance_System.Services
{
    public static class LeaveRequestService
    {
        public static List<LeaveRequest> GetPendingRequests()
        {
            var list = new List<LeaveRequest>();

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = "SELECT * FROM LeaveRequests ORDER BY RequestDate DESC";

            using SqlCommand cmd = new SqlCommand(sql, con);
            using SqlDataReader r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new LeaveRequest
                {
                    RequestID = (int)r["RequestID"],
                    StudentID = r["StudentID"].ToString(),
                    Type = r["Type"].ToString(),
                    Reason = r["Reason"].ToString(),
                    Status = r["Status"].ToString(),
                    RequestDate = (DateTime)r["RequestDate"]
                });
            }

            return list;
        }

        public static void UpdateStatus(int id, string status)
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = "UPDATE LeaveRequests SET Status=@st WHERE RequestID=@id";

            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@st", status);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
