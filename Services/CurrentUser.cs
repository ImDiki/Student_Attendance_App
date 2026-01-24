using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_System.Services
{
    public static class CurrentUser
    {
        public static string UserID { get; set; }

        // လိုအပ်လျှင် User Role (Admin/Student) ပါ သိမ်းထားလို့ရပါတယ်
        public static string Role { get; set; }
    }
}
