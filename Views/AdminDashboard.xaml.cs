using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Services;

namespace Student_Attendance_System.Views
{
    public partial class AdminDashboardPage : Page
    {
        public AdminDashboardPage()
        {
            InitializeComponent();

            // listen for teacher changes
            TeacherManagementPage.TeacherChanged += RefreshTeacherCount;
        }

        private void RefreshTeacherCount()
        {
            LoadTeacherCount();
        }
        private void LoadTeacherCount()
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = "SELECT COUNT(*) FROM Users WHERE Role = 'Teacher'";
            using SqlCommand cmd = new SqlCommand(sql, con);

            int count = (int)cmd.ExecuteScalar();

            txtTeachers.Text = count.ToString(); // or label/content
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDashboardCounts();
        }

        private void LoadDashboardCounts()
        {
            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            txtStudents.Text = GetCount(con, "Student");
            txtTeachers.Text = GetCount(con, "Teacher");
            txtClasses.Text = "0";        // placeholder (future)
            txtAttendance.Text = "0";     // placeholder (future)
        }

        private string GetCount(SqlConnection con, string role)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE Role = @r";
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@r", role);
            return cmd.ExecuteScalar().ToString();
        }

        private void ManageTeachers_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new TeacherManagementPage());
        }
        private void ManageClasses_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Class management coming soon");
        }
        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reports coming soon");
        }
    }
}
