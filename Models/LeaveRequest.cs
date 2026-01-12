//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Student_Attendance_System.Models;

//namespace Student_Attendance_System.Models
//{
//    public class LeaveRequest
//    {
//        public string StudentID { get; set; }
//        public string Reason { get; set; }
//        public string Date { get; set; }
//    }
//}

namespace Student_Attendance_System.Models
{
    public class LeaveRequest
    {
        public int RequestID { get; set; }
        public string StudentID { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public string TeacherComment { get; set; }
    }
}
