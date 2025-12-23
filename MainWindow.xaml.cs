using System.Windows;
using Student_Attendance_System.Views;

namespace Student_Attendance_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // ဆော့ဝဲလ်စဖွင့်ရင် MainMenuPage ကို စပြ
            MainFrame.Navigate(new MainMenuPage());
        }
    }
}