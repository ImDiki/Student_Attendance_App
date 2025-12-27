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

            // Default Dates (Optional)
            dpEnrollDate.SelectedDate = DateTime.Today; // ဒီနေ့ရက်စွဲ အလိုလိုပေါ်မယ်
            dpBirthDate.DisplayDate = new DateTime(2003, 1, 1); // 2003 ခုနှစ်ဝန်းကျင်ကို ပြထားမယ်
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            // Webcam ဖွင့်မယ့် Code (နောက်မှထည့်မယ်)
            MessageBox.Show("Webcam capture logic will be here!");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // ၁။ Form Validation (မဖြည့်ရသေးရင် သတိပေးမယ်)
            if (string.IsNullOrWhiteSpace(txtStudentID.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please fill in Student ID and Name!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpBirthDate.SelectedDate == null)
            {
                MessageBox.Show("Please select Birth Date!", "Warning");
                return;
            }

            // ၂။ Data တွေကို ဆွဲထုတ်ခြင်း
            string studentID = txtStudentID.Text;  // e.g., C5292
            string fullName = txtName.Text;        // e.g., MYAT THADAR LINN
            string department = cboDepartment.Text; // e.g., 情報システム開発
            DateTime birthDate = dpBirthDate.SelectedDate.Value;
            DateTime enrollDate = dpEnrollDate.SelectedDate?? DateTime.Now;
            string password = txtPassword.Password;

            // ---------------------------------------------------------
            // bo sann's code (Database Insert)  
            // ---------------------------------------------------------
            // TODO: Send these values to SQL Database      >> d name dwe a tine database lod pr bro 
            // INSERT INTO Users (Username, FullName, Major, BirthDate, ...) 
            // VALUES (studentID, fullName, department, birthDate, ...);

            MessageBox.Show($"Student Registered Successfully!\nID: {studentID}\nName: {fullName}", "Success");

            // ပြီးရင် Login Page ကို ပြန်ပို့မယ်
            NavigationService.GoBack();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}