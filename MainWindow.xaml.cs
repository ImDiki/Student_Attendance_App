using System;
using System.Windows;
using System.Windows.Threading;
using Student_Attendance_System.Views;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;
using System.Xml.Serialization;

namespace Student_Attendance_System
{
    public partial class MainWindow : Window
    {
        private bool isSideBarExpanded = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, ev) => {
                txtDate.Text = LanguageSettings.Language
                    ? DateTime.Now.ToString("yyyy年MM月dd日 (ddd) HH:mm:ss")
                    : DateTime.Now.ToString("yyyy/MM/dd (ddd) hh:mm:ss tt");
            };
            timer.Start();

            MainFrame.Navigate(new LoginPage());
        }
        private void btnHomeLogo_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage());

        }
        // --- Hamburger Click Logic ---
        private void btnHamburger_Click(object sender, RoutedEventArgs e)
        {
            if (isSideBarExpanded)
            {
                // Sidebar ကို ကျုံ့မယ် (Icon mode)
                SidebarBorder.Width = 75;
                SideInfoPanel.Visibility = Visibility.Collapsed;
                BottomPanel.Visibility = Visibility.Visible; // Developer button တွေ မပျောက်အောင်
                ToggleLabels(Visibility.Collapsed);
            }
            else
            {
                // Sidebar ကို ပြန်ချဲ့မယ် (Full mode)
                SidebarBorder.Width = 255;
                SideInfoPanel.Visibility = Visibility.Visible;
                ToggleLabels(Visibility.Visible);
            }
            isSideBarExpanded = !isSideBarExpanded;
        }

        private void ToggleLabels(Visibility visibility)
        {
            lblLang.Visibility = visibility;
            lblLogin.Visibility = visibility;
            lblReg.Visibility = visibility;
            lblScan.Visibility = visibility;
            lblTable.Visibility = visibility;
            lblReport.Visibility = visibility;
            lblLogout.Visibility = visibility;
            lblAbout.Visibility = visibility;
            lblDev.Visibility = visibility;
        }

        public void HandleLoginSuccess(User user)
        {
            txtWelcome.Text = user.FullName.ToUpper();
            btnLoginMenu.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Visible;

            if (user.Role == "Student") MainFrame.Navigate(new TimetablePage());
            else MainFrame.Navigate(new ScanPage());
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserData.UserData.CurrentUser = null;
            btnLoginMenu.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Collapsed;
            UpdateLanguage();
            MainFrame.Navigate(new LoginPage());
            MessageBox.Show(LanguageSettings.Language ? "ログアウトしました" : "Logged out successfully!");
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

            lblLogin.Text = isJp ? "ログイン" : "Login";
            lblReg.Text = isJp ? "新規登録" : "Register";
            lblScan.Text = isJp ? "スキャン" : "Scan / Webcam";
            lblTable.Text = isJp ? "タイムテーブル" : "Time Table";
            lblReport.Text = isJp ? "レポート" : "Report";
            lblLogout.Text = isJp ? "ログアウト" : "Logout";
            lblAbout.Text = isJp ? "アプリについて" : "About Us";
            lblDev.Text = isJp ? "開発チーム" : "Developer Team";
            lblLang.Text = isJp ? "言語: JP / ENG" : "Language: ENG / JP";

            if (MainFrame.Content is ILanguageSwitchable page) page.ChangeLanguage(isJp);
        }

        private void btnLoginMenu_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new LoginPage());
        private void btnRegister_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegisterPage());
        private void btnScanMode_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ScanPage());
        private void btnTimeTable_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new TimetablePage());
        private void btnReport_Click(object sender, RoutedEventArgs e) { /* Navigate to Report */ }
        private void btnAbout_Click(object sender, RoutedEventArgs e) { /* Navigate to About */ }
        private void btnDevTeam_Click(object sender, RoutedEventArgs e) { /* Navigate to DevTeam */ }
    }
}