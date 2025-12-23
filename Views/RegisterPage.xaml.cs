using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System.Data;
using System.Xml.Linq;

namespace Student_Attendance_System.Views
{
    public partial class RegisterPage : Page
    {
        public RegisterPage() { InitializeComponent(); }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string sid = txtId.Text;
            string name = txtName.Text.Trim();
            string subject = txtSubject.Text.Trim();

            if (string.IsNullOrEmpty(sid)||
                string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(subject) ||
                dpBirthday.SelectedDate == null)
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            bool success = DBConn.RegisterStudent(
                sid,name,dpBirthday.SelectedDate.Value,subject);

            if (success)
            {
                MessageBox.Show("Student registered successfully!");
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Database error!");
            }
        }
    }
}