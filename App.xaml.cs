using System.Collections.Generic;
using System.Windows;
using Student_Attendance_System.Models;


namespace Student_Attendance_System
{
    public partial class App : Application
    {
        // အတန်းတက်စာရင်းကို ယာယီသိမ်းထားမည့်နေရာ
        public static List<AttendanceRecord> TempAttendanceList { get; set; } = new List<AttendanceRecord>();

        // လက်ရှိ အတန်းချိန် အခြေအနေ
        public static bool IsClassActive { get; set; } = false;
        public static string CurrentSubject { get; set; } = "";
        public static DateTime CurrentActiveSessionStart { get; set; }
    }
}