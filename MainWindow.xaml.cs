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
            // စဖွင့်ရင် Login Page ကို အရင်ပြမယ်
            MainFrame.Navigate(new LoginPage());
        }

        // ဒီ Function ကို Login Page ကနေ လှမ်းခေါ်မယ်
        public void HandleLoginSuccess(User user)
        {
            // ၁။ User Data သိမ်းမယ်
            UserData.UserData.CurrentUser = user;

            // ၂။ Sidebar က Menu တွေကို ဖော်မယ်
            pnlUserMenu.Visibility = Visibility.Visible;

            // Login ခလုတ်တွေ၊ About Us တွေ ဖျောက်ချင်ရင်လည်း ရတယ် (Optional)
            // btnAbout.Visibility = Visibility.Collapsed; 

            // ၃။ Role ပေါ်မူတည်ပြီး Dashboard ပြောင်းမယ်
            if (user.Role == "Student")
            {
                MainFrame.Navigate(new StudentDashboardPage());
            }
            else if (user.Role == "Teacher")
            {
                // Teacher Dashboard မရှိသေးရင် Message ပြ
                MessageBox.Show("Welcome Teacher! Dashboard coming soon.");
            }
            else if (user.Role == "Admin")
            {
                MessageBox.Show("Welcome Admin!");
            }
        }

        // Language Change Logic
        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (btnLanguage.Content.ToString() == "Language: EN")
            {
                btnLanguage.Content = "Language: JP";
                // Japan စာသားတွေ ပြောင်းမယ့် Code ဒီမှာရေး
            }
            else
            {
                btnLanguage.Content = "Language: EN";
                // English ပြန်ပြောင်း
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Logout လုပ်ရင် မူလအခြေအနေ ပြန်ထားမယ်
            UserData.UserData.CurrentUser = null;
            pnlUserMenu.Visibility = Visibility.Collapsed; // Menu ပြန်ဖျောက်
            MainFrame.Navigate(new LoginPage()); // Login Page ပြန်ခေါ်
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            // About Us page ကို Frame ထဲမှာ ပြချင်ရင်
            // MainFrame.Navigate(new AboutUsPage()); 
            MessageBox.Show("Team Profile: \nDev 1: Bo Sann (Backend)\nDev 2: DI KI (Frontend)");
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            // လက်ရှိ User ပေါ်မူတည်ပြီး Dashboard ပြန်ခေါ်တာ
            if (UserData.UserData.CurrentUser.Role == "Student")
                MainFrame.Navigate(new StudentDashboardPage());
        }
    }
}