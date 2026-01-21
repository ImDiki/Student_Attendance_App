using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System.Views;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;

namespace Student_Attendance_System
{
    public partial class MainWindow : Window
    {
        private bool isSideBarExpanded = true;
        private DispatcherTimer? _timer; // Fixed CS8618

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, ev) => {
                UpdateClockDisplay(!isSideBarExpanded);
                txtDate.Text = LanguageSettings.Language
                    ? DateTime.Now.ToString("yyyy年MM月dd日 (ddd)")
                    : DateTime.Now.ToString("ddd, MMM dd, yyyy").ToUpper();
            };
            _timer.Start();

            UpdateLanguage();
            MainFrame.Navigate(new LoginPage());
        }

        private void UpdateClockDisplay(bool isCollapsed)
        {
            if (isCollapsed)
                txtDigitalClock.Text = DateTime.Now.ToString("HH\nmm\nss");
            else
                txtDigitalClock.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void btnHamburger_Click(object sender, RoutedEventArgs e)
        {
            if (isSideBarExpanded)
            {
                SidebarBorder.Width = 85;
                SideInfoPanel.Visibility = Visibility.Collapsed;
                txtDigitalClock.FontSize = 20;
                txtDate.Visibility = Visibility.Collapsed;

                // Vertical Header Layout
                Grid.SetColumn(btnHomeLogo, 0); Grid.SetColumnSpan(btnHomeLogo, 2);
                btnHomeLogo.HorizontalAlignment = HorizontalAlignment.Center;

                Grid.SetRow(btnHamburger, 1); Grid.SetColumn(btnHamburger, 0); Grid.SetColumnSpan(btnHamburger, 2);
                btnHamburger.HorizontalAlignment = HorizontalAlignment.Center;
                btnHamburger.Margin = new Thickness(0, 10, 0, 0);

                ToggleSidebarUI(Visibility.Collapsed, HorizontalAlignment.Center);
                UpdateClockDisplay(true);
            }
            else
            {
                SidebarBorder.Width = 260;
                SideInfoPanel.Visibility = Visibility.Visible;
                txtDigitalClock.FontSize = 35;
                txtDate.Visibility = Visibility.Visible;

                // Restore Horizontal Layout
                Grid.SetColumn(btnHomeLogo, 0); Grid.SetColumnSpan(btnHomeLogo, 1);
                btnHomeLogo.HorizontalAlignment = HorizontalAlignment.Left;

                Grid.SetRow(btnHamburger, 0); Grid.SetColumn(btnHamburger, 1); Grid.SetColumnSpan(btnHamburger, 1);
                btnHamburger.HorizontalAlignment = HorizontalAlignment.Right;
                btnHamburger.Margin = new Thickness(0, 5, 15, 5);

                ToggleSidebarUI(Visibility.Visible, HorizontalAlignment.Left);
                UpdateClockDisplay(false);
            }
            isSideBarExpanded = !isSideBarExpanded;
        }

        // Fix for image_f4f7a5.png: Defined HandleLoginSuccess
        public void HandleLoginSuccess(User user)
        {
            if (user == null) return;
            txtWelcome.Text = user.Username.ToUpper();
            btnLoginMenu.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Visible;

            if (user.Role == "Admin") MainFrame.Navigate(new AdminDashboard());
            else if (user.Role == "Teacher") MainFrame.Navigate(new TeacherDashboard());
            else MainFrame.Navigate(new StudentDashboardPage());
        }

        // Fix for image_f39ae3.png: Defined btnLanguage_Click
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
            lblScan.Text = isJp ? "スキャン" : "Scan";
            lblTable.Text = isJp ? "タイムテーブル" : "Timetable";
            lblReport.Text = isJp ? "レポート" : "Report";
            lblLogout.Text = isJp ? "ログアウト" : "Logout";
            lblAbout.Text = isJp ? "情報" : "About";
            lblDev.Text = isJp ? "開発者" : "Developer";
            lblLang.Text = isJp ? "言語: JP / ENG" : "Language: ENG / JP";
            if (MainFrame.Content is ILanguageSwitchable page) page.ChangeLanguage(isJp);
        }

        private void ToggleSidebarUI(Visibility vis, HorizontalAlignment align)
        {
            lblLang.Visibility = lblLogin.Visibility = lblReg.Visibility =
            lblScan.Visibility = lblTable.Visibility = lblReport.Visibility =
            lblLogout.Visibility = lblAbout.Visibility = lblDev.Visibility = vis;

            pnlLang.HorizontalAlignment = pnlLogin.HorizontalAlignment = pnlReg.HorizontalAlignment =
            pnlScan.HorizontalAlignment = pnlTable.HorizontalAlignment = pnlReport.HorizontalAlignment =
            pnlLogout.HorizontalAlignment = pnlAbout.HorizontalAlignment = pnlDev.HorizontalAlignment = align;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e) => btnGlobalBack.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        private void btnGlobalBack_Click(object sender, RoutedEventArgs e) { if (MainFrame.CanGoBack) MainFrame.GoBack(); }
        private void btnHomeLogo_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new LoginPage());
        private void btnLoginMenu_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new LoginPage());
        private void btnRegister_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegisterPage());
        private void btnScanMode_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ScanPage());
        private void btnTimeTable_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new TimetablePage());
        private void btnReport_Click(object sender, RoutedEventArgs e) { }
        private void btnAbout_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ProjectOverviewPage());
        private void btnDevTeam_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Developer Team: OWL-SYS");
        private void btnLogout_Click(object sender, RoutedEventArgs e) { UserData.UserData.CurrentUser = null; UpdateLanguage(); MainFrame.Navigate(new LoginPage()); }
    }
}