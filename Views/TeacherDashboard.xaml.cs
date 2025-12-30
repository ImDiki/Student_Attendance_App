using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Student_Attendance_System.Views
{
    public partial class TeacherDashboard : Page
    {
        // Scan Page ကနေ ပြန်လာရင် Tab 2 ကိုဖွင့်ခိုင်းဖို့ Variable
        public bool OpenManagerTab = false;

        public TeacherDashboard()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtCurrentDate.Text = DateTime.Now.ToString("D");
            LoadTodaySchedule();
            RefreshList();

            // Scan Page ကနေ ပြန်လာတာဆိုရင် Manager Tab ကို အလိုလိုဖွင့်မယ်
            if (OpenManagerTab)
            {
                MainTabControl.SelectedIndex = 1;
            }
        }

        // ==========================================
        // TAB 1: TIMETABLE & START CLASS
        // ==========================================
        private void LoadTodaySchedule()
        {
            DayOfWeek today = DateTime.Now.DayOfWeek;
            // today = DayOfWeek.Monday; // Debug Mode

            string p1 = "No Class", p2 = "No Class", p3 = "No Class", p4 = "No Class", p5 = "No Class";

            switch (today)
            {
                case DayOfWeek.Monday:
                    p1 = "Network & Security"; p2 = "Network & Security"; p3 = "Exam Prep"; p4 = "Exam Prep"; p5 = "Self Study"; break;
                case DayOfWeek.Tuesday:
                    p1 = "Business App"; p2 = "Programming I (C#)"; p3 = "Data Structure"; p4 = "Seminar"; p5 = "Job Guidance"; break;
                case DayOfWeek.Wednesday:
                    p1 = "Comp Systems"; p2 = "Comp Systems"; p3 = "PG Practice"; p4 = "PG Practice"; p5 = "Comm Skills"; break;
                case DayOfWeek.Thursday:
                    p1 = "Management"; p2 = "Japanese"; p3 = "Exam Prep"; p4 = "Exam Prep"; p5 = "Exam Prep"; break;
                case DayOfWeek.Friday:
                    p1 = "Data Structure"; p2 = "Management"; p3 = "Programming I (C#)"; p4 = "Programming I (C#)"; p5 = "Database"; break;
            }

            txtSubjP1.Text = p1; btnP1.Tag = p1; btnP1.IsEnabled = (p1 != "No Class");
            txtSubjP2.Text = p2; btnP2.Tag = p2; btnP2.IsEnabled = (p2 != "No Class");
            txtSubjP3.Text = p3; btnP3.Tag = p3; btnP3.IsEnabled = (p3 != "No Class");
            txtSubjP4.Text = p4; btnP4.Tag = p4; btnP4.IsEnabled = (p4 != "No Class");
            txtSubjP5.Text = p5; btnP5.Tag = p5; btnP5.IsEnabled = (p5 != "No Class");
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string subject = btn.Tag.ToString();

            // 1. Session Start
            App.IsClassActive = true;
            App.CurrentActiveSessionStart = DateTime.Now;
            App.CurrentSubject = subject;

            // 2. Scan Page ကို သွားမယ်
            NavigationService.Navigate(new ScanPage());
        }

        // ==========================================
        // TAB 2: ATTENDANCE MANAGER (EDIT)
        // ==========================================
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            dgAttendance.ItemsSource = null;
            dgAttendance.ItemsSource = App.TempAttendanceList;
        }

        private bool IsReasonValid()
        {
            string reason = txtReason.Text.Trim();
            if (string.IsNullOrEmpty(reason) || reason.Length < 3)
            {
                MessageBox.Show("Please enter a valid reason (e.g., 'Train Delay').", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void btnMarkPresent_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendance.SelectedItem is AttendanceRecord record)
            {
                if (!IsReasonValid()) return;
                record.Status = "Present";
                record.Note = txtReason.Text.Trim();
                MessageBox.Show($"Updated {record.StudentID} to Present.");
                txtReason.Clear();
                RefreshList();
            }
            else MessageBox.Show("Select a student first.");
        }

        private void btnMarkAbsent_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendance.SelectedItem is AttendanceRecord record)
            {
                if (!IsReasonValid()) return;
                record.Status = "Absent";
                record.Note = txtReason.Text.Trim();
                MessageBox.Show($"Updated {record.StudentID} to Absent.");
                txtReason.Clear();
                RefreshList();
            }
            else MessageBox.Show("Select a student first.");
        }
    }
}