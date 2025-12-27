using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System.UserData;

namespace Student_Attendance_System.Views
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();

            // ၁။ User နာမည်ကို Header မှာ ပြမယ်
            if (UserData.UserData.CurrentUser != null)
            {
                lblUserInfo.Text = "User: " + UserData.UserData.CurrentUser.Username;
            }

            // ၂။ စဖွင့်ဖွင့်ချင်း Dashboard ကို Default အနေနဲ့ ပြမယ်
            // (Bro ရှေ့မှာ StudentDashboardPage ဆောက်ထားတယ်မလား? အဲ့ကောင်ကို ခေါ်ထည့်လိုက်မယ်)
            ContentFrame.Navigate(new StudentDashboardPage());
            lblPageTitle.Text = "Dashboard";
        }

        // Dashboard Button
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new StudentDashboardPage());
            lblPageTitle.Text = "Dashboard";
        }

        // QR Scan Button (ခဏနေမှ ဆောက်မယ်၊ လောလောဆယ် Placeholder ထားမယ်)
        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            // ContentFrame.Navigate(new Webcam()); // Webcam page မရှိသေးရင် Error တက်မယ်
            MessageBox.Show("QR Scan Page Coming Soon!");
            lblPageTitle.Text = "QR Scanner";
        }

        // Profile Button
        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Profile Page Coming Soon!");
            lblPageTitle.Text = "My Profile";
        }

        // Logout -> Login Page ကို ပြန်သွားမယ်
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserData.UserData.CurrentUser = null;
            NavigationService.Navigate(new LoginPage());
        }
    }
}