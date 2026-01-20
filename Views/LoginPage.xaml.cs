using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;
using Student_Attendance_System.Services;

namespace Student_Attendance_System.Views
{
    public partial class LoginPage : Page, ILanguageSwitchable
    {
        public LoginPage()
        {
            InitializeComponent();
            // App-wide language settings ကို စတင်အသုံးပြုခြင်း
            ChangeLanguage(LanguageSettings.Language);
        }

        // --- ILanguageSwitchable Implementation ---
        public void ChangeLanguage(bool isJapanese)
        {
            txtTitle.Text = isJapanese ? "ようこそ" : "WELCOME";
            txtSubTitle.Text = isJapanese ? "サインインして続行してください" : "Sign in to Continue";
            lblUsername.Text = isJapanese ? "ユーザー名" : "Username";
            lblPassword.Text = isJapanese ? "パスワード" : "Password";
            btnLogin.Content = isJapanese ? "ログイン" : "LOGIN";
            txtForgetPass.Text = isJapanese ? "パスワードをお忘れですか？" : "Forgot Password?";
            txtRegisterLink.Text = isJapanese ? "登録はこちら" : "No account? Register here.";
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                string msg = LanguageSettings.Language ? "すべての項目を入力してください" : "Please fill in all fields.";
                MessageBox.Show(msg, "System Status");
                return;
            }

            var auth = new AuthService();
            User loggedInUser = auth.AuthenticateUser(user, pass);

            if (loggedInUser != null)
            {
                UserData.UserData.CurrentUser = loggedInUser;

                // MainWindow ဆီသို့ Success ဖြစ်ကြောင်း လှမ်းခေါ်ပြီး Role အလိုက် Redirect လုပ်ခိုင်းခြင်း
                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.HandleLoginSuccess(loggedInUser);
                }
            }
            else
            {
                MessageBox.Show(LanguageSettings.Language ? "ログインに失敗しました" : "Login Failed!", "Auth Error");
            }
        }

        private void ForgetPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(LanguageSettings.Language ? "教務課に連絡してください" : "Please contact the admin office.");
        }

        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}