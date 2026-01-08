using System;
using System.Windows;
using System.Windows.Controls;


namespace Student_Attendance_System.Views
{
    public partial class AdminDashboard : Page
    {
        public AdminDashboard()
        {
            InitializeComponent();

            txtDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            txtAttendance.Text = "0";
            txtClasses.Text = "0";
        }

        // 🌙 Dark Mode
        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["WindowBg"] = Resources["DarkWindowBg"];
            Application.Current.Resources["CardBg"] = Resources["DarkCardBg"];
            Application.Current.Resources["TextColor"] = Resources["DarkText"];
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["WindowBg"] = Resources["LightWindowBg"];
            Application.Current.Resources["CardBg"] = Resources["LightCardBg"];
            Application.Current.Resources["TextColor"] = Resources["LightText"];
        }

        // 🚪 Logout
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            //UserData.CurrentUser = null;

            if (Application.Current.MainWindow is MainWindow mw)
            {
                mw.MainFrame.Navigate(new LoginPage());
            }
        }
    }
}
