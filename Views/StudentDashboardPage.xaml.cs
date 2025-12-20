using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Student_Attendance_System.Views
{
    public partial class StudentDashboardPage : Page
    {
        public StudentDashboardPage(string studentName)
        {
            InitializeComponent();
            txtWelcome.Text = $"Welcome, {studentName} 🎓";
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Logout လုပ်ရင် Main Menu ပြန်သွားမယ်
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}