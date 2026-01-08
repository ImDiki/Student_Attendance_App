using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Student_Attendance_System.Models;
using Student_Attendance_System.Services;

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
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password",
                                "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 🔑 DATABASE AUTH
            AuthService authService = new AuthService();
            User user = authService.AuthenticateUser(username, password);

            if (user != null)
            {
                // Save login state
                UserData.CurrentUser = user;

                // Navigate based on role
                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.HandleLoginSuccess(user);
                }
            }
            else
            {
                MessageBox.Show("Login failed.\nInvalid username or password.",
                                "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}
