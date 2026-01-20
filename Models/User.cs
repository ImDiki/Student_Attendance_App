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
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Student", "Teacher", "Admin"
        public string FullName { get; set; }
        public string Major { get; set; }
        public int YearLevel { get; set; }
        public string AssignedClass { get; set; }
    }
}
