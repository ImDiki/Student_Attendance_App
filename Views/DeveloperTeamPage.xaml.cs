using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Student_Attendance_System.Views
{
    public partial class DeveloperTeamPage : Page
    {
        public DeveloperTeamPage()
        {
            InitializeComponent();
        }

        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl((sender as Button)?.Tag?.ToString());
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl((sender as Button)?.Tag?.ToString());
        }

        private void OpenUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show("Could not open the link.");
                }
            }
        }
    }
}