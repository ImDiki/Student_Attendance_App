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
                // 🔹 အရေးကြီးဆုံးအပိုင်း: CurrentUser ထဲသို့ User Object တစ်ခုလုံး သိမ်းဆည်းခြင်း
                // ဤနေရာတွင် UserData.UserData.CurrentUser သို့မဟုတ် CurrentUser.UserID (ဘရိုဆောက်ထားသော Class ပေါ်မူတည်၍) သိမ်းပါ
                UserData.UserData.CurrentUser = loggedInUser;

                // Storyline အရ Default Password သုံးနေလျှင် သတိပေးချက်ပြခြင်း
                if (pass == "123456")
                {
                    string welcomeMsg = LanguageSettings.Language
                        ? "初期パスワードを使用しています。プロフィールで変更してください。"
                        : "Welcome! You are using a temporary password. Please change it in your Profile.";
                    MessageBox.Show(welcomeMsg, "Security Alert");
                }

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

        // 🔹 Forgot Password Storyline
        private void ForgetPassword_Click(object sender, MouseButtonEventArgs e)
        {
            string info = LanguageSettings.Language
                ? "教務課(Admin Office)でパスワードをリセットしてください。\nリセット後の初期パスワードは '123456' です。"
                : "Please contact the Admin Office to reset your password.\n\nYour default password after reset will be '123456'.";

            MessageBox.Show(info, "Forgot Password Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}