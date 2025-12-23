using System.Linq; // List ရှာဖို့လိုတယ်
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System;

namespace Student_Attendance_System.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack) NavigationService.GoBack();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string inputUser = txtUsername.Text;
            string inputPass = txtPassword.Password;

            // MockDatabase ထဲမှာ အဲ့ဒီလူ ရှိမရှိ စစ်မယ်
            var user = MockDatabase.Users.FirstOrDefault(u => u.Username == inputUser && u.Password == inputPass);

            if (user != null)
            {
                // ရှိတယ်ဆိုရင် Role ကို စစ်မယ်
                if (user.Role == "Admin")
                {
                    //NavigationService.Navigate(new DashboardPage()); // Admin Page
                    Admin admin = new Admin();
                    admin.Show();
                    
                    
                }
                else if (user.Role == "Student")
                {
                    
                    NavigationService.Navigate(new StudentDashboardPage(user.FullName));
                }
            }
            else
            {
                MessageBox.Show("Invalid Username or Password!", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}