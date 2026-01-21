using System;

namespace Student_Attendance_System.Models
{
    public class AttendanceRecord
    {
        public int AttendanceID { get; set; }
        public string StudentID { get; set; }
        public string Subject { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan AttendanceTime { get; set; }
        public string Status { get; set; }   // Present / Absent / Late
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
