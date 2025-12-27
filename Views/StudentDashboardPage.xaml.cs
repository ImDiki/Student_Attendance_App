using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System.UserData;

namespace Student_Attendance_System.Views
{
    public partial class StudentDashboardPage : Page
    {
        public StudentDashboardPage()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (UserData.UserData.CurrentUser != null)
            {
                lblWelcome.Text = $"Welcome, {UserData.UserData.CurrentUser.FullName}!";

                // ---------------------------------------------------------
                // bo sann's code
                // ---------------------------------------------------------
                // Database ကနေ Attendance % ကို ဆွဲထုတ်ပြီး lblPercent မှာ ပြပေးရမယ်
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserData.UserData.CurrentUser = null; // Clear Data
            NavigationService.Navigate(new LoginPage()); // Go back to Login
        }
    }
}