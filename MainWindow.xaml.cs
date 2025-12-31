using System.Windows;
using Student_Attendance_System.Models;
using Student_Attendance_System.Views;
using Student_Attendance_System.UserData;

namespace Student_Attendance_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Software စဖွင့်တာနဲ့ Scan Page ကို အရင်ဆုံး ပြမယ်
            MainFrame.Navigate(new Views.ScanPage());
        }

        // Login Page ကနေ လှမ်းခေါ်မယ့် Function
        public void HandleLoginSuccess(User user)
        {
            UserData.UserData.CurrentUser = user;

            // ၁။ Login ခလုတ်ကို ဖျောက်မယ်
            btnLogin.Visibility = Visibility.Collapsed;

            // ၂။ Dashboard/Logout ခလုတ်တွေကို ဖော်မယ်
            pnlUserMenu.Visibility = Visibility.Visible;
            lblUserTitle.Text = user.Role + " Menu";

            // ၃။ သက်ဆိုင်ရာ Dashboard ကို ပို့မယ်
            if (user.Role == "Student")
            {
                MainFrame.Navigate(new StudentDashboardPage());
            }
            else if (user.Role == "Teacher")
            {
                MainFrame.Navigate(new TeacherDashboard());
            }
            else if (user.Role == "Admin")
            {
                MainFrame.Navigate(new AdminDashboard());
            }
        }

        // --- EVENT HANDLERS (Error တက်နေတဲ့ Function များ) ---

        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (btnLanguage.Content.ToString() == "Language: EN")
                btnLanguage.Content = "Language: JP";
            else
                btnLanguage.Content = "Language: EN";
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Student Attendance System\nVersion 1.0\nDeveloped by our Team.", "About Us");
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage());
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (UserData.UserData.CurrentUser == null) return;

            if (UserData.UserData.CurrentUser.Role == "Student")
                MainFrame.Navigate(new StudentDashboardPage());
            else if (UserData.UserData.CurrentUser.Role == "Teacher")
                MainFrame.Navigate(new TeacherDashboard());
            else if (UserData.UserData.CurrentUser.Role == "Admin")
                MainFrame.Navigate(new TeacherDashboard());
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserData.UserData.CurrentUser = null;
            pnlUserMenu.Visibility = Visibility.Collapsed;
            btnLogin.Visibility = Visibility.Visible;
            MainFrame.Navigate(new LoginPage());
        }
    }
}