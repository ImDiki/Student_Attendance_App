using System.Collections.Generic;


namespace Student_Attendance_System
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
    }

    public static class MockDatabase
    {
        public static List<User> Users = new List<User>()
        {
            new User { Username = "admin", Password = "123", Role = "Admin", FullName = "System Administrator" },
            new User { Username = "st001", Password = "123", Role = "Student", FullName = "DI KI(Student)" }
        };
    }
}





