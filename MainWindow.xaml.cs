using System.Windows;
using Student_Attendance_System.Views; // Views Folder ကို လှမ်းချိတ်မှ ရမယ်

namespace Student_Attendance_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login_window login = new Login_window();
            login.Show();
            this.Close();

        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Close();
        }
    }
}