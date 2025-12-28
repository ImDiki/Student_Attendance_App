using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // MouseButtonEventArgs အတွက် လိုအပ်သည်
using System.Windows.Navigation;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // =========================================================================
            // BO SANN'S CODE ZONE (Database Check)
            // =========================================================================

            User user = null;

            // --- MOCK DATA (အစမ်း) ---
            if (username == "C5292" && password == "1234")
            {
                user = new User { Id = 1, Username = "C5292", FullName = "Myat Thadar Linn", Role = "Student", Major = "IT" };
            }
            else if (username == "admin" && password == "admin")
            {
                user = new User { Id = 99, Username = "admin", FullName = "Head Master", Role = "Teacher" };
            }

            if (user != null)
            {
                // Login အောင်ရင် MainWindow ကို လှမ်းပြောပြီး Sidebar တွေဖွင့်ခိုင်းမယ်
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.HandleLoginSuccess(user);
                }
            }
            else
            {
                MessageBox.Show("Login Failed!\n\nUse:\nStudent: C5292 / 1234\nTeacher: admin / admin", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Register Link နှိပ်ရင် အလုပ်လုပ်နေရာ
        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}