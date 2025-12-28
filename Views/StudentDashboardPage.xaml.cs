using System.Windows.Controls;
using Student_Attendance_System.UserData; // UserData ကို သုံးမယ်

namespace Student_Attendance_System.Views
{
    public partial class StudentDashboardPage : Page
    {
        public StudentDashboardPage()
        {
            InitializeComponent();
            LoadStudentData();
        }

        private void LoadStudentData()
        {
            // ၁။ ကျောင်းသား နာမည် ပြမယ်
            if (UserData.UserData.CurrentUser != null)
            {
                txtWelcome.Text = $"Welcome back, {UserData.UserData.CurrentUser.FullName}!";
            }

            // =========================================================================
            // BO SANN'S CODE ZONE (Get Stats from DB)
            // =========================================================================
            // ACTION: SELECT Count(*) FROM Attendance WHERE StudentID = @id AND Status = 'Present' ...
            // OUTPUT: totalClass, presentCount, absentCount
            // =========================================================================

            // --- MOCK DATA (အစမ်းထည့်ထားသော ဂဏန်းများ) ---
            int total = 20;
            int present = 18;
            int absent = 2;

            // UI မှာ ပြမယ်
            txtTotal.Text = total.ToString();
            txtPresent.Text = present.ToString();
            txtAbsent.Text = absent.ToString();

            // ရာခိုင်နှုန်း တွက်မယ်
            if (total > 0)
            {
                double percentage = ((double)present / total) * 100;
                txtPercent.Text = $"{percentage:0.0}%";
            }
            else
            {
                txtPercent.Text = "0%";
            }
        }
    }
}