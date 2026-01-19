using System;
using System.Windows;
using System.Windows.Threading;
using Student_Attendance_System.Views;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;

namespace Student_Attendance_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ၁။ အချိန်ကို Real-time ပြဖို့ Timer ပေးမယ်
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, ev) => {
                txtDate.Text = LanguageSettings.Language
                    ? DateTime.Now.ToString("yyyy年MM月dd日 (ddd) HH:mm:ss")
                    : DateTime.Now.ToString("yyyy/MM/dd (ddd) hh:mm:ss tt");
            };
            timer.Start();

            // ၂။ App စဖွင့်တာနဲ့ Login Page ကို တန်းပြမယ်
            MainFrame.Navigate(new LoginPage());
        }

        // --- Login အောင်မြင်ရင် LoginPage ကနေ ဒီကောင်ကို လှမ်းခေါ်မယ် ---
        public void HandleLoginSuccess(User user)
        {
            txtWelcome.Text = user.FullName.ToUpper();

            // UI Toggle Logic: Login ကို ဖျောက်ပြီး Logout ကို ဖော်မယ်
            btnLoginMenu.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Visible;

            // Role အလိုက် Dashboard ခွဲပို့မယ်
            if (user.Role == "Student")
            {
                MainFrame.Navigate(new TimetablePage());
            }
            else
            {
                MainFrame.Navigate(new ScanPage());
            }
        }

        // Sidebar က Login Button ကို နှိပ်ရင်
        private void btnLoginMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage());
        }

        // Report အောက်က Logout Button ကို နှိပ်ရင်
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Memory ထဲက User data ကို ရှင်းမယ်
            UserData.UserData.CurrentUser = null;

            // UI Toggle Logic: Login ကို ပြန်ဖော်ပြီး Logout ကို ပြန်ဖျောက်မယ်
            btnLoginMenu.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Collapsed;

            UpdateLanguage(); // Sidebar စာသားတွေ မူလအတိုင်း ပြန်ပြင်မယ်
            MainFrame.Navigate(new LoginPage());

            string msg = LanguageSettings.Language ? "ログアウトしました" : "Logged out successfully!";
            MessageBox.Show(msg, "Logout", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            LanguageSettings.Language = !LanguageSettings.Language;
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            bool isJp = LanguageSettings.Language;

            // Login မဝင်ထားရင်ပဲ Welcome ပြမယ်
            if (UserData.UserData.CurrentUser == null)
            {
                txtWelcome.Text = isJp ? "ようこそ" : "WELCOME";
            }

            // Button စာသားများ Update လုပ်ခြင်း
            btnLoginMenu.Content = isJp ? "🔑 ログイン" : "🔑 Login";
            btnRegister.Content = isJp ? "📝 新規登録" : "📝 Register";
            btnScanMode.Content = isJp ? "📷 スキャンモード" : "📷 Scan / Webcam";
            btnTimeTable.Content = isJp ? "📅 タイムテーブル" : "📅 Time Table";
            btnReport.Content = isJp ? "📊 レポート" : "📊 Report";
            btnLogout.Content = isJp ? "🚪 ログアウト" : "🚪 Logout";
            btnAbout.Content = isJp ? "ℹ️ アプリについて" : "ℹ️ About Us";
            btnDevTeam.Content = isJp ? "👨‍💻 開発チーム" : "👨‍💻 Developer Team";
            btnLanguage.Content = isJp ? "🌐 言語: JP / ENG" : "🌐 Language: ENG / JP";

            // လက်ရှိပွင့်နေတဲ့ Page ကိုပါ Language လှမ်းပြောင်းမယ်
            if (MainFrame.Content is ILanguageSwitchable currentPage)
            {
                currentPage.ChangeLanguage(isJp);
            }
        }

        // --- Navigation Methods ---
        private void btnRegister_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegisterPage());
        private void btnScanMode_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ScanPage());
        private void btnTimeTable_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new TimetablePage());
        private void btnReport_Click(object sender, RoutedEventArgs e) { /* Navigate to ReportPage */ }
        private void btnAbout_Click(object sender, RoutedEventArgs e) { /* Navigate to AboutPage */ }
        private void btnDevTeam_Click(object sender, RoutedEventArgs e) { /* Navigate to DevTeamPage */ }
    }
}