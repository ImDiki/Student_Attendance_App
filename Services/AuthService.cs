using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Services
{
    class AuthService
    {
        public User AuthenticateUser(string username, string password)
        {
            // Backend မလာခင် Mock အနေနဲ့ စစ်မယ်
            if (username == "admin" && password == "admin")
                return new User { Username = "admin", FullName = "Head Master", Role = "Teacher" };

            if (username == "C5292" && password == "1234")
                return new User { Username = "C5292", FullName = "Myat Thadar Linn", Role = "Student", Major = "IT" };

            return null;
        }
    }
}
