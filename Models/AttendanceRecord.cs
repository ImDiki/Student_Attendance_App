using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_System.Models
{
    public class AttendanceRecord
    {
        public string StudentID { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Status { get; set; } // Present, Absent, Late
        public string Note { get; set; }
    }
}
