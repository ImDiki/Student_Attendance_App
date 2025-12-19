using System;
using System.Windows;

namespace Student_Attendance_System.Views
{
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
            ダッシュボード読み込み();
        }

        private void ダッシュボード読み込み()
        {
            // today
            txtDate.Text = DateTime.Now.ToString("yyyy年MM月dd日");

            // fake facts
            txtAttendance.Text = "120 人 出席";
            txtClasses.Text = "6 クラス";
        }

        //mode dark light
        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            Resources["WindowBg"] = Resources["DarkWindowBg"];
            Resources["CardBg"] = Resources["DarkCardBg"];
            Resources["TextColor"] = Resources["DarkText"];
            ThemeToggle.Content = "☀";
        }

        //mode light dark
        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Resources["WindowBg"] = Resources["LightWindowBg"];
            Resources["CardBg"] = Resources["LightCardBg"];
            Resources["TextColor"] = Resources["LightText"];
            ThemeToggle.Content = "🌙";
        }

        //logout
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "ログアウトしますか？",
                "確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 例：
                // new LoginWindow().Show();
                this.Close();
            }
        }
    }
}
