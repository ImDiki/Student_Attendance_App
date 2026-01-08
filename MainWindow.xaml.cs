using System;
using System.Windows;
using System.Windows.Threading; 
using Student_Attendance_System.Models;
using Student_Attendance_System.Views; // *** Views Folder ကို ခေါ်ထားမှ Page တွေ သုံးလို့ရမယ် ***

namespace Student_Attendance_System
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            
            // 1. နာရီစ run မယ်
            StartClock();

            // 2. App စဖွင့်ဖွင့်ချင်း Login Page ကို တန်းပြမယ်
            MainFrame.Navigate(new LoginPage());
        }

        // --- Date & Time Timer ---
        private void StartClock()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Format: Jan 08, 2026 | 01:20 AM
            txtDate.Text = DateTime.Now.ToString("MMM dd, yyyy | hh:mm tt");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Timer_Tick(null, null); 
        }

        // =======================================================
        // SIDEBAR BUTTON CLICK EVENTS (PAGE NAVIGATION)
        // =======================================================

        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            // Language လဲချင်ရင် ဒီမှာ Logic ရေးရမယ်
            MessageBox.Show("Language changed to Japanese (Mock)");
        }

        private void btnLoginMenu_Click(object sender, RoutedEventArgs e)
        {
            // LoginPage.xaml သို့
            MainFrame.Navigate(new LoginPage());
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // RegisterPage.xaml သို့
            MainFrame.Navigate(new RegisterPage());
        }

        private void btnScanMode_Click(object sender, RoutedEventArgs e)
        {
            // ScanPage.xaml သို့ (Webcam.xaml သုံးချင်ရင် ဒီနေရာမှာ new Webcam() ပြောင်းပါ)
            MainFrame.Navigate(new ScanPage());
        }

        private void btnTimeTable_Click(object sender, RoutedEventArgs e)
        {
            // TimetablePage.xaml သို့
            MainFrame.Navigate(new TimetablePage());
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            // ReportPage.xaml မရှိသေးလို့ Message Box ပြထားသည်
            // File ဆောက်ပြီးရင်: MainFrame.Navigate(new ReportPage()); လို့ပြောင်းပါ
            MessageBox.Show("Report Page is under construction.");
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Attendance System v1.0\nCreated for Students & Teachers.");
        }

        private void btnDevTeam_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Developer Team:\n1. [Name]\n2. [Name]");
        }

        // =======================================================
        // LOGIN SUCCESS HANDLER (Dashboard ခွဲမယ့်နေရာ)
        // =======================================================
        public void HandleLoginSuccess(User user)
        { 
            // User Role ပေါ်မူတည်ပြီး Dashboard အသီးသီးကို သွားမယ်
            if (user.Role == "Teacher" || user.Role == "Admin")
            {
                // TeacherDashboard.xaml သို့
                MainFrame.Navigate(new TeacherDashboard()); 
                // Admin ဖြစ်ရင် AdminDashboard.xaml သို့ သွားချင်ရင်လည်း ဒီမှာ စစ်လို့ရ
            }
            else if (user.Role == "Student")
            {
                // StudentDashboardPage.xaml သို့
                MainFrame.Navigate(new StudentDashboardPage());
            }
        }
        private void UpdateSidebarUI(string role)
        {
            if (role == "Student")
            {
                // ကျောင်းသားဆိုလျှင် ScanMode နှင့် Register ကို ဖျောက်ထားမည်
                btnScanMode.Visibility = Visibility.Collapsed;
                btnRegister.Visibility = Visibility.Collapsed;
                btnTimeTable.Visibility = Visibility.Visible;
            }
            else if (role == "Teacher")
            {
                // ဆရာဆိုအကုန်မြင်
                btnScanMode.Visibility = Visibility.Visible;
                btnRegister.Visibility = Visibility.Visible;
                btnTimeTable.Visibility = Visibility.Visible;
            }
        }
    }
}
