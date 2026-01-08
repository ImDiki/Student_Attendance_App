using System; // Exception အတွက်
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Data.SqlClient;
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
            // 1. Inputs ကို ရယူပြီး စစ်ဆေးခြင်း
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password", "Warning");
                return;
            }

            // 2. AuthService ကိုသုံးပြီး Login စစ်ဆေးခြင်း
            // (မှတ်ချက်: ဒီနေရာမှာ variable ထပ်မကြေညာတော့ဘဲ တန်းသုံးလိုက်ပါမယ်)
            var authService = new AuthService();
            User loggedInUser = authService.AuthenticateUser(username, password);

            // 3. Login အောင်မြင်မှု ရှိ/မရှိ စစ်ဆေးခြင်း
            if (loggedInUser != null)
            {
                // Global State (UserData) ထဲမှာ သိမ်းမယ်
                UserData.UserData.CurrentUser = loggedInUser;

                // Login အောင်ရင် MainWindow ကို လှမ်းပြောပြီး Dashboard Navigate လုပ်မယ်
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.HandleLoginSuccess(loggedInUser);
                }
            }
            else
            {
                // Login ကျရှုံးလျှင် ပြမည့် Message
                MessageBox.Show("Login Failed!\n\nCheck Credentials:\nStudent: C5292 / 1234\nTeacher: admin / admin",
                                "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
           
           
        // Register Link နှိပ်ရင် အလုပ်လုပ်နေရာ
        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}