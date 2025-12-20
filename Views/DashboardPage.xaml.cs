using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Student_Attendance_System.Views
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // နောက်ပြန်ဆုတ်ခြင်း Logic
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}