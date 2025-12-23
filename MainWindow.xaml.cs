using System.Windows;
using Student_Attendance_System.Views; // Views folder ကို လှမ်းချိတ်မှ MainMenuPage ကို သိမယ်

namespace Student_Attendance_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // App စဖွင့်ရင် MainMenuPage ကို သွားမယ်
            RootFrame.Navigate(new MainMenuPage());
        }
    }
}