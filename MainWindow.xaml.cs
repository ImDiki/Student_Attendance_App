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
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, ev) => {
                txtDate.Text = LanguageSettings.Language ? DateTime.Now.ToString("yyyy年MM月dd日 (ddd) HH:mm:ss") : DateTime.Now.ToString("yyyy/MM/dd (ddd) hh:mm:ss tt");
            };
            timer.Start();
            MainFrame.Navigate(new LoginPage());
        }

        public void HandleLoginSuccess(User user)
        {
            txtWelcome.Text = user.FullName.ToUpper();
            btnLoginMenu.Content = LanguageSettings.Language ? "🚪 ログアウト" : "🚪 Logout";

            if (user.Role == "Student") MainFrame.Navigate(new TimetablePage()); // သို့မဟုတ် StudentDashboard
            else MainFrame.Navigate(new ScanPage()); // သို့မဟုတ် TeacherDashboard
        }

        private void btnLoginMenu_Click(object sender, RoutedEventArgs e)
        {
            if (UserData.UserData.CurrentUser != null)
            {
                UserData.UserData.CurrentUser = null;
                UpdateLanguage();
                MainFrame.Navigate(new LoginPage());
                MessageBox.Show(LanguageSettings.Language ? "ログアウトしました" : "Logged out successfully!");
            }
            else MainFrame.Navigate(new LoginPage());
        }

        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            LanguageSettings.Language = !LanguageSettings.Language;
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            bool isJp = LanguageSettings.Language;
            if (UserData.UserData.CurrentUser == null) txtWelcome.Text = isJp ? "ようこそ" : "WELCOME";
            btnLoginMenu.Content = UserData.UserData.CurrentUser != null ? (isJp ? "🚪 ログアウト" : "🚪 Logout") : (isJp ? "🔑 ログイン" : "🔑 Login");
            btnRegister.Content = isJp ? "📝 新規登録" : "📝 Register";
            btnScanMode.Content = isJp ? "📷 スキャンモード" : "📷 Scan / Webcam";
            btnTimeTable.Content = isJp ? "📅 タイムテーブル" : "📅 Time Table";
            btnLanguage.Content = isJp ? "🌐 言語: JP / ENG" : "🌐 Language: ENG / JP";

            if (MainFrame.Content is ILanguageSwitchable page) page.ChangeLanguage(isJp);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegisterPage());
        private void btnScanMode_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ScanPage());
        private void btnTimeTable_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new TimetablePage());
        private void btnReport_Click(object sender, RoutedEventArgs e) { }
        private void btnAbout_Click(object sender, RoutedEventArgs e) { }
        private void btnDevTeam_Click(object sender, RoutedEventArgs e) { }
    }
}