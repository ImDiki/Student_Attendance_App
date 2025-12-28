using System;
using System.Windows;

namespace Student_Attendance_System
{
    public partial class App : Application
    {
        // Global Variables for Session Management
        public static bool IsClassActive = false;
        public static DateTime CurrentActiveSessionStart;
        public static string CurrentSubject = "";

        // Session ID (Backend က ပြန်ပေးရင် သိမ်းဖို့)
        public static int CurrentSessionID = 0;
    }
}