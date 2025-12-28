using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Student_Attendance_System.Views
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
            dpEnrollDate.SelectedDate = DateTime.Today;
            dpBirthDate.DisplayDate = new DateTime(2003, 1, 1);
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Webcam Feature Coming Soon! (Needs Bo Sann's Help)");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtStudentID.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please fill in Student ID and Name!", "Warning");
                return;
            }

            string studentID = txtStudentID.Text;
            string fullName = txtName.Text;

            // =========================================================================
            // BO SANN'S CODE ZONE (Insert to Database)
            // =========================================================================
            // ACTION: INSERT INTO Users (...) VALUES (...)
            // IMPORTANT: Handle Photo as Base64 String or Byte Array
            // =========================================================================

            // --- MOCK ACTION ---
            MessageBox.Show($"[MOCK] Student Registered Successfully!\nID: {studentID}", "Success");
            NavigationService.GoBack();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
