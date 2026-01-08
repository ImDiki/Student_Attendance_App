using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Models
{
    public class LeaveRequest
    {
        public string StudentID { get; set; }
        public string Reason { get; set; }
        public string Date { get; set; }
    }
}
