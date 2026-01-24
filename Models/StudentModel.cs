using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_System.Models
{
   public  class StudentModel
    {
        public string StudentCode { get; set; }
        public string FullName { get; set; }
        public string YearLevel { get; set; }
        public string ClassName { get; set; }
        public string Department { get; set; }
    }
}
