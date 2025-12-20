using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System.Views; // 🔥 ဒါလေးပါမှ Views ထဲက Page တွေကို သိမှာ

namespace Student_Attendance_System.Views
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
        }

        // Login ခလုတ်နှိပ်ရင်
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
           

           
            NavigationService.Navigate(new LoginPage());
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DashboardPage());
        }
    }
}