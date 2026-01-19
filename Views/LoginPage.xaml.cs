using System;
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
        // Language state ကို သိဖို့ variable တစ်ခု ထားပါမယ်


        public LoginPage()
        {
            InitializeComponent();
            ChangeLanguage(LanguageSettings.Language);
        }

        // ILanguageSwitchable ကနေလာတဲ့ Method ကို အကောင်အထည်ဖော်ခြင်း
        public void ChangeLanguage(bool isJapanese)
        {


            if (isJapanese)
            {
                // Japanese UI
                txtTitle.Text = "ようこそ";
                txtTitle.Focus();
                txtSubTitle.Text = "サインインして続行してください";
                lblUsername.Text = "ユーザー名";
                lblPassword.Text = "パスワード";
                btnLogin.Content = "ログイン";
                txtRegisterLink.Text = "アカウントをお持ちでない方はこちら";
            }
            else
            {
                // English UI
                txtTitle.Text = "WELCOME";
                txtTitle.Focus();
                txtSubTitle.Text = "Sign in to Continue";
                lblUsername.Text = "Username";
                lblPassword.Text = "Password";
                btnLogin.Content = "Login";
                txtRegisterLink.Text = "Don't have an account? Register here";
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                string msg = LanguageSettings.Language ? "ユーザー名とパスワードを入力してください" : "Please enter username and password";
                MessageBox.Show(msg, "Warning");
                return;
            }

            var authService = new AuthService();
            User loggedInUser = authService.AuthenticateUser(username, password);

            if (loggedInUser != null)
            {
                UserData.UserData.CurrentUser = loggedInUser;

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    // MainWindow ကနေ Dashboard ကို သွားမယ့် method ကို လှမ်းခေါ်ခြင်း
                    // mainWindow.HandleLoginSuccess(loggedInUser);
                }
            }
            else
            {
                string errorTitle = LanguageSettings.Language ? "ログインエラー" : "Login Error";
                string errorMsg = LanguageSettings.Language
                    ? "ログインに失敗しました！\n資格情報を確認してください。"
                    : "Login Failed!\n\nCheck Credentials:\nStudent: C5292 / 1234\nTeacher: admin / admin";

                MessageBox.Show(errorMsg, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}