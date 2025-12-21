using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Student_Attendance_System.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage() { InitializeComponent(); }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;

            if (user == "admin")
            {
                // Admin Dashboard ကို သွားမယ်
                NavigationService.Navigate(new DashboardPage());
            }
            else if (user == "student")
            {
                // Student Dashboard (အသစ်ဆောက်ရမယ်) ကို သွားမယ်
                MessageBox.Show("Welcome Student!");
                // NavigationService.Navigate(new StudentDashboardPage());
            }
            else
            {
                MessageBox.Show("User not found!");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); // Main Menu ပြန်သွားမယ်
        }
    }
}