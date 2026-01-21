using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using Student_Attendance_System.Views;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;

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

            UpdateLanguage(); // Start with correct language display
            MainFrame.Navigate(new LoginPage());
        }

        private void btnHomeLogo_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage());
        }

        private void btnHamburger_Click(object sender, RoutedEventArgs e)
        {
            if (isSideBarExpanded)
            {
                SidebarBorder.Width = 85;
                SideInfoPanel.Visibility = Visibility.Collapsed;
                ToggleSidebarUI(Visibility.Collapsed, HorizontalAlignment.Center);
                Grid.SetRow(btnHamburger, 0);
                Grid.SetColumn(btnHamburger, 1);

                Grid.SetRow(btnHomeLogo, 1);
                Grid.SetColumn(btnHomeLogo, 1);

                btnHamburger.HorizontalAlignment = HorizontalAlignment.Center;
                btnHomeLogo.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                SidebarBorder.Width = 260;
                SideInfoPanel.Visibility = Visibility.Visible;
                ToggleSidebarUI(Visibility.Visible, HorizontalAlignment.Left);
                Grid.SetRow(btnHomeLogo, 0);
                Grid.SetColumn(btnHomeLogo, 0);

                Grid.SetRow(btnHamburger, 0);
                Grid.SetColumn(btnHamburger, 2);

                btnHamburger.HorizontalAlignment = HorizontalAlignment.Right;
                btnHomeLogo.HorizontalAlignment = HorizontalAlignment.Left;
            }

            isSideBarExpanded = !isSideBarExpanded;
        }



        private void ToggleSidebarUI(Visibility vis, HorizontalAlignment align)
        {
            lblLang.Visibility = lblLogin.Visibility = lblReg.Visibility =
            lblScan.Visibility = lblTable.Visibility = lblReport.Visibility =
            lblLogout.Visibility = lblAbout.Visibility = lblDev.Visibility = vis;

            pnlLang.HorizontalAlignment = pnlLogin.HorizontalAlignment =
            pnlReg.HorizontalAlignment = pnlScan.HorizontalAlignment =
            pnlTable.HorizontalAlignment = pnlReport.HorizontalAlignment =
            pnlLogout.HorizontalAlignment = pnlAbout.HorizontalAlignment =
            pnlDev.HorizontalAlignment = align;
        }

        public void HandleLoginSuccess(User user)
        {
            txtWelcome.Text = user.Username.ToUpper();
            btnLoginMenu.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Visible;

            switch (user.Role)
            {
                case "Admin":
                    MainFrame.Navigate(new AdminDashboardPage());
                    break;

                case "Teacher":
                    MainFrame.Navigate(new TeacherDashboard());
                    break;

                case "Student":
                    MainFrame.Navigate(new StudentDashboardPage());
                    break;

                default:
                    MessageBox.Show("Unknown role: " + user.Role);
                    MainFrame.Navigate(new LoginPage());
                    break;
            }
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

        // Methods that were missing in your original CS causing errors:
        private void btnLoginMenu_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new LoginPage());
        private void btnRegister_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegisterPage());
        private void btnScanMode_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ScanPage());
        private void btnTimeTable_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new TimetablePage());
        private void btnReport_Click(object sender, RoutedEventArgs e) { }
        private void btnAbout_Click(object sender, RoutedEventArgs e) => MessageBox.Show(LanguageSettings.Language ? "情報" : "About System");
        private void btnDevTeam_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Developer Team Console");
    }
}