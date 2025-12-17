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

        // WebcamWindow ကို ဖွင့်မယ်
        private void btnOpenWebcam_Click(object sender, RoutedEventArgs e)
        {
            WebcamWindow webcamWin = new WebcamWindow();
            webcamWin.Show();
            // this.Close(); // လိုချင်ရင် Main ကို ပိတ်လိုက်လို့ရတယ်
        }

     /*
        private void btnOpenRegister_Click(object sender, RoutedEventArgs e)
        {
            Register_window regWin = new Register_window();
            regWin.Show();
        }

        // Dashboard ကို ဖွင့်မယ်
        private void btnOpenDashboard_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashWin = new Dashboard();
            dashWin.Show();
        }
     */
    }
}