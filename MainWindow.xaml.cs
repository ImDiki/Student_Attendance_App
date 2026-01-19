using System;
using System.Windows;
using System.Windows.Threading;
using Student_Attendance_System.Views;
using Student_Attendance_System.Interfaces;// Page တွေရှိတဲ့ Folder Path ကို သေချာအောင် စစ်ပေးပါ

namespace Student_Attendance_System
{
    public partial class MainWindow : Window
    {
        private bool _isJapanese = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ၁။ အချိန်ကို Real-time ပြဖို့ Timer ပေးမယ်
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            // ၂။ App စဖွင့်တာနဲ့ Login Page ကို တန်းပြမယ်
            MainFrame.Navigate(new LoginPage());
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // ဂျပန်စာဆိုရင် ဂျပန် format နဲ့ပြမယ်
            if (_isJapanese)
                txtDate.Text = DateTime.Now.ToString("yyyy年MM月dd日 (ddd) HH:mm:ss");
            else
                txtDate.Text = DateTime.Now.ToString("yyyy/MM/dd (ddd) hh:mm:ss tt");
        }

        // --- Navigation ---
        private void btnLoginMenu_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new LoginPage());
        private void btnRegister_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegisterPage());
        private void btnScanMode_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ScanPage());
        private void btnTimeTable_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new TimetablePage());
        private void btnReport_Click(object sender, RoutedEventArgs e) { /* MainFrame.Navigate(new ReportPage()); */ }
        private void btnAbout_Click(object sender, RoutedEventArgs e) { /* MainFrame.Navigate(new AboutPage()); */ }
        private void btnDevTeam_Click(object sender, RoutedEventArgs e) { /* MainFrame.Navigate(new DevTeamPage()); */ }

        // --- Language Toggle ---
        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            LanguageSettings.Language =!LanguageSettings.Language;
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            if (_isJapanese)
            {
                txtWelcome.Text = "ようこそ";
                btnLoginMenu.Content = "🔑 ログイン (Login)";
                btnRegister.Content = "📝 新規登録 (Register)";
                btnScanMode.Content = "📷 スキャンモード";
                btnTimeTable.Content = "📅 タイムテーブル";
                btnReport.Content = "📊 レポート";
                btnAbout.Content = "ℹ️ アプリについて";
                btnDevTeam.Content = "👨‍💻 開発チーム";
                btnLanguage.Content = "🌐 言語: JP / ENG";
            }
            else
            {
                txtWelcome.Text = "WELCOME";
                btnLoginMenu.Content = "🔑 Login";
                btnRegister.Content = "📝 Register";
                btnScanMode.Content = "📷 Scan / Webcam";
                btnTimeTable.Content = "📅 Time Table";
                btnReport.Content = "📊 Report";
                btnAbout.Content = "ℹ️ About Us";
                btnDevTeam.Content = "👨‍💻 Developer Team";
                btnLanguage.Content = "🌐 Language: ENG / JP";
            }
            if(MainFrame.Content is ILanguageSwitchable  Currentpage)
            {
               Currentpage.ChangeLanguage(LanguageSettings.Language);
            }
        }
    }
}