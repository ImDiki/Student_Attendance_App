using Student_Attendance_System.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Student_Attendance_System.Views
{
    /// <summary>
    /// Login_window.xaml の相互作用ロジック
    /// </summary>
    public partial class Login_window : Window
    {
        public Login_window()
        {
            InitializeComponent();

        }
        private bool isDarkMode = false;

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            if (!isDarkMode)
            {
                // DARK MODE
                Application.Current.Resources["WindowBackgroundBrush"] =
                    new SolidColorBrush(Color.FromRgb(18, 18, 18));
                Application.Current.Resources["CardBackgroundBrush"] =
                    new SolidColorBrush(Color.FromRgb(30, 30, 30));
                Application.Current.Resources["CardBorderBrush"] =
                    new SolidColorBrush(Color.FromRgb(70, 70, 70));

                Application.Current.Resources["TextBrush"] =
                    new SolidColorBrush(Colors.White);
                Application.Current.Resources["SubTextBrush"] =
                    new SolidColorBrush(Color.FromRgb(180, 180, 180));

                BtnThemeToggle.Content = "☀️";
                isDarkMode = true;
            }
            else
            {
                // LIGHT MODE
                Application.Current.Resources["WindowBackgroundBrush"] =
                    new SolidColorBrush(Color.FromRgb(245, 245, 245));
                Application.Current.Resources["CardBackgroundBrush"] =
                    new SolidColorBrush(Colors.White);
                Application.Current.Resources["CardBorderBrush"] =
                    new SolidColorBrush(Color.FromRgb(221, 221, 221));

                Application.Current.Resources["TextBrush"] =
                    new SolidColorBrush(Color.FromRgb(51, 51, 51));
                Application.Current.Resources["SubTextBrush"] =
                    new SolidColorBrush(Colors.Gray);

                BtnThemeToggle.Content = "🌙";
                isDarkMode = false;
            }
        }
    }
}
