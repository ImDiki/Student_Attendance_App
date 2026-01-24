namespace Student_Attendance_System.Models
{
    public class Timetable
    {
        public int YearLevel { get; set; }
        public string ClassName { get; set; }
        public string Term { get; set; }

        public int DayOfWeek { get; set; } // 1–5
        public int Period { get; set; }    // 1–4
        public string StartTime { get; set; }
        public string SubjectName { get; set; }
    }
}
