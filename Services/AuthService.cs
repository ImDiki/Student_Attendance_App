using System;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Services
{
    class AuthService
    {
        public User AuthenticateUser(string username, string password)
        {
            // --- Teacher Login (Mock) ---
            if (username == "admin" && password == "admin")
            {
                return new User
                {
                    Username = "admin",
                    FullName = "Head Master",
                    Role = "Teacher"
                };
            }

            // --- Student Login (Mock) ---
            // ဥပမာ - Myat Thadar Linn က 2nd Year, C Class ဖြစ်တယ်ဆိုပါစို့
            if (username == "C5292" && password == "1234")
            {
                return new User
                {
                    Username = "C5292",
                    FullName = "Myat Thadar Linn",
                    Role = "Student",
                    YearLevel = 2,          // ၂ နှစ်မြောက်ကျောင်းသား
                    AssignedClass = "C"      // C ခန်း
                };
            }

            return null; // Login မှားရင် null ပြန်မယ်
        }
    }
}