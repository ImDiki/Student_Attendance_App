using System;
using System.Collections.Generic;
using System.Windows;

namespace Student_Attendance_System
{
    // Data သိမ်းဖို့ ပုံစံခွက် (Model)
    public class AttendanceRecord
    {
        public string StudentID { get; set; }
        public string Status { get; set; } // "Present" or "Absent"
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Note { get; set; } = "-";
    }

    public partial class App : Application
    {
        // Session Variables
        public static bool IsClassActive = false;
        public static DateTime CurrentActiveSessionStart;
        public static string CurrentSubject = "";

        // Error CS0117 အတွက် ဒါလေးပြန်ထည့်ပေးရမယ်
        public static int CurrentSessionID = 0;

        // Dashboard မှာပြဖို့ ယာယီ Database
        public static List<AttendanceRecord> TempAttendanceList = new List<AttendanceRecord>();
    }
}