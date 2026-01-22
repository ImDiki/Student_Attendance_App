using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_System.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // YearLevel ကို string ပြောင်းထားပါသည်
        public string YearLevel { get; set; } = string.Empty;
        public string AssignedClass { get; set; } = string.Empty;

        // Profile ပုံ သိမ်းဆည်းရန်
        public byte[]? FacePhoto { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
