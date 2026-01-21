using System;

namespace Student_Attendance_System.Models
{
    public class LeaveRequest
    {
        public int RequestID { get; set; }
        public string StudentID { get; set; }
        public string Type { get; set; } 
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
