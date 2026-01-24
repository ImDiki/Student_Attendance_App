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
        private DispatcherTimer? _timer;

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
                ToggleSidebarUI(Visibility.Collapsed, HorizontalAlignment.Center);
                UpdateClockDisplay(true);
            }
            else
            {
                SidebarBorder.Width = 260;
                SideInfoPanel.Visibility = Visibility.Visible;
                txtDigitalClock.FontSize = 35;
                txtDate.Visibility = Visibility.Visible;
                ToggleSidebarUI(Visibility.Visible, HorizontalAlignment.Left);
                UpdateClockDisplay(false);
            }
            isSideBarExpanded = !isSideBarExpanded;
        }

        // ✅ LoginPage ကနေ လှမ်းခေါ်မယ့် Login Success Logic
        public void HandleLoginSuccess(User user)
        {
            if (user == null) return;

            // ၁။ Sidebar ကို Login State သို့ ပြောင်းပါ
            UpdateSidebar(true);

            // ၂။ Role အလိုက် Dashboard ခွဲပို့ပါ
            if (user.Role == "Admin")
                MainFrame.Navigate(new AdminDashboard());
            else if (user.Role == "Teacher")
                MainFrame.Navigate(new TeacherDashboard());
            else
                MainFrame.Navigate(new StudentDashboardPage());
        }

        // ✅ Sidebar Toggle Logic
        public void UpdateSidebar(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                pnlLogin.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Visible;
                lblLogout.Visibility = Visibility.Visible;

                pnlReg.Visibility = Visibility.Visible;
                pnlScan.Visibility = Visibility.Visible;
                pnlTable.Visibility = Visibility.Visible;

                if (UserData.UserData.CurrentUser != null)
                    txtWelcome.Text = UserData.UserData.CurrentUser.Username.ToUpper();
            }
            else
            {
                btnLogout.Visibility = Visibility.Collapsed;
                lblLogout.Visibility = Visibility.Collapsed;

                pnlLogin.Visibility = Visibility.Visible;
                lblLogin.Visibility = Visibility.Visible;

                txtWelcome.Text = LanguageSettings.Language ? "ようこそ" : "WELCOME";
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserData.UserData.CurrentUser = null;
            UpdateSidebar(false);
            UpdateLanguage();
            MainFrame.Navigate(new LoginPage());
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
            lblScan.Text = isJp ? "スキャン" : "Scan";
            lblTable.Text = isJp ? "タイムテーブル" : "Timetable";
            lblLogout.Text = isJp ? "ログアウト" : "Logout";
            lblAbout.Text = isJp ? "情報" : "About";
            lblDev.Text = isJp ? "開発者" : "Developer";
            lblLang.Text = isJp ? "言語: JP / ENG" : "Language: ENG / JP";
            if (MainFrame.Content is ILanguageSwitchable page) page.ChangeLanguage(isJp);
        }

        private void ToggleSidebarUI(Visibility vis, HorizontalAlignment align)
        {
            lblLang.Visibility = lblLogin.Visibility = lblReg.Visibility =
            lblScan.Visibility = lblTable.Visibility =
            lblLogout.Visibility = lblAbout.Visibility = lblDev.Visibility = vis;

            pnlLang.HorizontalAlignment = pnlLogin.HorizontalAlignment = pnlReg.HorizontalAlignment =
            pnlScan.HorizontalAlignment = pnlTable.HorizontalAlignment =
            pnlLogout.HorizontalAlignment = pnlAbout.HorizontalAlignment = pnlDev.HorizontalAlignment = align;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e) => btnGlobalBack.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        private void btnGlobalBack_Click(object sender, RoutedEventArgs e) { if (MainFrame.CanGoBack) MainFrame.GoBack(); }
        private void btnHomeLogo_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new LoginPage());
        private void btnLoginMenu_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new LoginPage());
        private void btnRegister_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegisterPage());
        private void btnScanMode_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ScanPage());
        private void btnTimeTable_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new TimetablePage());
        private void btnAbout_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ProjectOverviewPage());
        private void btnDevTeam_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Developer Team: OWL-SYS");
    }
}