using System.Text.RegularExpressions;
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

            // Empty check
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password",
                                "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Username validation (A–Z, a–z, 0–9 only)
            if (!Regex.IsMatch(username, @"^[A-Za-z0-9]+$"))
            {
                MessageBox.Show(
                    "Username can contain only letters and numbers.\nCapital and small letters are different.",
                    "Invalid Username",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Password validation
            if (password.Length < 8)
            {
                MessageBox.Show(
                    "Password must be at least 8 characters long.",
                    "Weak Password",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Must contain at least 1 letter and 1 number
            if (!Regex.IsMatch(password, @"^(?=.*[A-Za-z])(?=.*\d).+$"))
            {
                MessageBox.Show(
                    "Password must contain at least one letter and one number.",
                    "Weak Password",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // 🔑 DATABASE AUTH
            AuthService authService = new AuthService();
            User user = authService.AuthenticateUser(username, password);

            if (user != null)
            {
                UserData.CurrentUser = user;

                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.HandleLoginSuccess(user);
                }
            }
            else
            {
                MessageBox.Show(
                    "Login failed.\nInvalid username or password.",
                    "Login Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}
