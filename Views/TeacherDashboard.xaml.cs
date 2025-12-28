using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Student_Attendance_System.Views
{
    public partial class TeacherDashboard : Page
    {
        public TeacherDashboard()
        {
            InitializeComponent();
        }

        private void btnStartClass_Click(object sender, RoutedEventArgs e)
        {
            // =========================================================================
            // BO SANN'S CODE ZONE (Start Session)
            // =========================================================================
            // ACTION: INSERT INTO ClassSessions (TeacherID, Subject, StartTime)
            // OUTPUT: Get the new SessionID
            // =========================================================================

            // --- MOCK ACTION ---
            App.IsClassActive = true;
            App.CurrentActiveSessionStart = DateTime.Now;
            App.CurrentSubject = "C# Programming"; // ဥပမာ Subject
            App.CurrentSessionID = 101; // Mock Session ID

            MessageBox.Show($"Class Started: {App.CurrentSubject}\nTime: {DateTime.Now:HH:mm:ss}", "Success");
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ScanPage());
        }
    }
}
