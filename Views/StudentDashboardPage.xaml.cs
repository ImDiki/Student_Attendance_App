using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Models;
using Student_Attendance_System.Services;

namespace Student_Attendance_System.Views
{
    public partial class StudentDashboardPage : Page
    {
        public StudentDashboardPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // ၁။ User နာမည်ပြောင်းခြင်း
            if (UserData.CurrentUser != null)
            {
                txtWelcome.Text = $"{UserData.CurrentUser.FullName} さん、ようこそ！";
            }

            // ၂။ Timetable ကို Dashboard ထဲမှာ Navigate လုပ်ခြင်း
            TimetableFrame.Navigate(new TimetablePage());

            // ၃။ ကိန်းဂဏန်းများ တွက်ချက်ခြင်း
            LoadAttendanceStats();
        }

        //private void LoadAttendanceStats()
        //{
        //    // App.TempAttendanceList ထဲကနေ လက်ရှိ Login ဝင်ထားတဲ့ကျောင်းသားရဲ့ data ပဲယူမယ်
        //    string currentStudentID = UserData.CurrentUser?.Username;
        //    var myRecords = App.TempAttendanceList.Where(r => r.StudentID == currentStudentID).ToList();

        //    int total = myRecords.Count;
        //    int present = myRecords.Count(r => r.Status.Contains("Present") || r.Status.Contains("出席"));
        //    int absent = total - present;

        //    txtTotal.Text = total.ToString();
        //    txtPresent.Text = present.ToString();
        //    txtAbsent.Text = absent.ToString();

        //    if (total > 0)
        //    {
        //        double rate = ((double)present / total) * 100;
        //        txtPercent.Text = $"{rate:0.0}%";

        //        // 80% အောက်ရောက်ရင် စာသားအနီပြောင်းမယ်
        //        txtPercent.Foreground = rate < 80 ? System.Windows.Media.Brushes.Red : (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#2980b9");
        //    }
        //}
        private void LoadAttendanceStats()
        {
            if (UserData.CurrentUser == null) return;

            string studentId = UserData.CurrentUser.Username; // StudentID

            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

                string sql = @"
SELECT TotalClasses, PresentClasses
FROM Students
WHERE StudentID COLLATE Latin1_General_CS_AS = @StudentID";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return;

                        int total = (int)r["TotalClasses"];
                        int present = (int)r["PresentClasses"];
                        int absent = total - present;

                        txtTotal.Text = total.ToString();
                        txtPresent.Text = present.ToString();
                        txtAbsent.Text = absent.ToString();

                        double rate = total > 0 ? (double)present / total * 100 : 0;
                        txtPercent.Text = $"{rate:0.0}%";
                        txtPercent.Foreground = rate < 80 ? Brushes.Red : (Brush)new BrushConverter().ConvertFromString("#2980b9");
                    }
                }
            }
        }




        private void btnSubmitLeave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLeaveReason.Text))
            {
                MessageBox.Show("申請理由を入力してください (Please enter a reason).", "Warning");
                return;
            }

            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

                string sql = @"
            INSERT INTO LeaveRequests (StudentID, Reason)
            VALUES (@StudentID, @Reason)";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", UserData.CurrentUser.Username);
                    cmd.Parameters.AddWithValue("@Reason", txtLeaveReason.Text);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("申請を送信しました。先生の確認をお待ちください。", "Success");
            txtLeaveReason.Clear();
        }
    }
}