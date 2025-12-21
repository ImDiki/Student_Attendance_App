using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Student_Attendance_System.Views
{
    public partial class RegisterPage : Page
    {
        public RegisterPage() { InitializeComponent(); }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}