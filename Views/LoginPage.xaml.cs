using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System.Data;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage() { InitializeComponent(); }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = DBConn.Login(txtUser.Text, txtPass.Password);

            if (user == null)
            {
                MessageBox.Show("Invalid username or password");
                return;
            }

            // ROLE CHECK
            if (user.Role == "Admin")
            {
                Admin admin = new Admin();
                admin.Show();   
            }
            else
            {
                NavigationService.Navigate(new StudentDashboardPage(user.FullName));
            }

            Application.Current.MainWindow.Close();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}