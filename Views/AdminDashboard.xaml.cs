using System;
using System.Windows;
using System.Windows.Controls;
using Student_Attendance_System.Services; // AdminService ကို သုံးရန်
using Student_Attendance_System.Interfaces;

namespace Student_Attendance_System.Views
{
    public partial class AdminDashboard : Page, ILanguageSwitchable
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage(LanguageSettings.Language);

            // Database မှ Stats များ Load လုပ်ခြင်း
            txtStudents.Text = AdminService.GetTotalStudents().ToString();
            txtTeachers.Text = AdminService.GetTotalTeachers().ToString();

            // Timetable Page ကို အောက်က Frame ထဲမှာ Navigate လုပ်ပါတယ်
            AdminFrame.Navigate(new AdminClassTimetablePage());
        }

        public void ChangeLanguage(bool isJapanese)
        {
            txtWelcome.Text = isJapanese ? "管理者ダッシュボード" : "Admin Dashboard";
            txtWelcomeSub.Text = isJapanese ? "システム全体を管理します。" : "Manage the entire system.";
            lblStudents.Text = isJapanese ? "全学生数" : "TOTAL STUDENTS";
            lblTeachers.Text = isJapanese ? "全教員数" : "TOTAL TEACHERS";
            lblMgmtMenu.Text = isJapanese ? "管理メニュー" : "Management Menu";
            lblTimetableTitle.Text = isJapanese ? "クラス時間割管理" : "CLASS TIMETABLE (ADMIN)";
        }

        // Teacher Management ခလုတ်နှိပ်လျှင်
        private void ManageTeachers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // AdminFrame ဆိုတဲ့ Frame ထဲမှာ Teacher Management Page ကို ဖွင့်ပြပါတယ်
                AdminFrame.Navigate(new TeacherManagementPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error navigating to Teacher Management: " + ex.Message);
            }
        }

        // Class Management ခလုတ်နှိပ်လျှင်
        private void ManageClasses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // AdminFrame ထဲမှာ Class Management Page ကို ဖွင့်ပြပါတယ်
                AdminFrame.Navigate(new ClassManagementPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error navigating to Class Management: " + ex.Message);
            }
        }
    }
}