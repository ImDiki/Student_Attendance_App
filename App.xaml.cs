using System.Collections.Generic;
using System.Windows;
using Student_Attendance_System.Models;


namespace Student_Attendance_System
{
    public partial class App : Application
    {
        // အတန်းတက်စာရင်းကို ယာယီသိမ်းထားမည့်နေရာ

        public static int SelectedTermIndex { get; set; } = 1;
        public static List<string> StartedPeriods { get; set; } = new List<string>();
        public static List<AttendanceRecord> TempAttendanceList { get; set; } = new List<AttendanceRecord>();

        public static List<LeaveRequest> GlobalLeaveRequests { get; set; } = new List<LeaveRequest>();

        // လက်ရှိ အတန်းချိန် အခြေအနေ
        public static bool IsClassActive { get; set; } = false;
        public static string CurrentSubject { get; set; } = "";
        public static DateTime CurrentActiveSessionStart { get; set; }
    }
}