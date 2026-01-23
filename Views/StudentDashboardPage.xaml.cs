using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Student_Attendance_System.Models;
using Student_Attendance_System.Interfaces;
using Microsoft.Data.SqlClient;

namespace Student_Attendance_System.Views
{
    public partial class StudentDashboardPage : Page, ILanguageSwitchable
    {
        public StudentDashboardPage()
        {
            InitializeComponent();
            // Language ကို စတင်သတ်မှတ်ခြင်း
            ChangeLanguage(LanguageSettings.Language);
        }

        //private void Page_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // ၁။ User နာမည်ကို Dashboard မှာပြခြင်း
        //    if (UserData.UserData.CurrentUser != null)
        //    {
        //        string greeting = LanguageSettings.Language ? "さん、ようこそ！" : " ,Welcome back!";
        //        txtWelcome.Text = $"{UserData.UserData.CurrentUser.FullName}{greeting}";
        //    }

        //    // ၂။ Timetable ကို Frame ထဲမှာ Navigate လုပ်ခြင်း
        //    TimetableFrame.Navigate(new TimetablePage());

        //    // ၃။ ကိန်းဂဏန်းများ တွက်ချက်ခြင်း
        //    LoadAttendanceStats();
        //}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserData.UserData.CurrentUser;

            if (user == null)
            {
                MessageBox.Show("User session lost!");
                return;
            }

            if (user.YearLevel <= 0 || string.IsNullOrWhiteSpace(user.AssignedClass))
            {
                MessageBox.Show(
                    $"Student info error!\nYear={user.YearLevel}\nClass={user.AssignedClass}"
                );
                return;
            }

            txtWelcome.Text = $"{user.FullName} さん、ようこそ！";

            // 🔑 THIS loads timetable correctly
            TimetableFrame.Navigate(new TimetablePage());

            LoadAttendanceStats();
        }

        public void ChangeLanguage(bool isJapanese)
        {
            txtWelcomeSub.Text = isJapanese ? "出席状況を確認してください。" : "Check your attendance status.";
            lblPresent.Text = isJapanese ? "出席 (Present)" : "Present";
            lblAbsent.Text = isJapanese ? "欠席 (Absent)" : "Absent";
            lblRateTitle.Text = isJapanese ? "総合出席率" : "Overall Attendance Rate";
            lblRateSub.Text = isJapanese ? "進級には80%以上必要です。" : "You need 80% to pass.";
            lblTimetable.Text = isJapanese ? "今週の時間割" : "Weekly Timetable";
            lblLeaveTitle.Text = isJapanese ? "欠席・遅刻届" : "Leave / Late Request";
            lblReason.Text = isJapanese ? "申請理由:" : "Reason for Leave:";
            btnSubmit.Content = isJapanese ? "申請を送信する" : "Submit Request";
        }

        private void LoadAttendanceStats()
        {
            string studentId = UserData.UserData.CurrentUser?.Username;

            if (string.IsNullOrEmpty(studentId))
                return;

            int total = 0;
            int present = 0;

            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

                // Total classes attended
                string totalQuery = @"SELECT COUNT(*) FROM Attendance WHERE StudentID = @sid";

                SqlCommand totalCmd = new SqlCommand(totalQuery, con);
                totalCmd.Parameters.AddWithValue("@sid", studentId);
                total = (int)totalCmd.ExecuteScalar();

                // Present count
                string presentQuery = @"SELECT COUNT(*) FROM Attendance WHERE StudentID = @sid AND Status = 'Present'";

                SqlCommand presentCmd = new SqlCommand(presentQuery, con);
                presentCmd.Parameters.AddWithValue("@sid", studentId);
                present = (int)presentCmd.ExecuteScalar();
            }

            int absent = total - present;

            // UI update
            txtTotal.Text = total.ToString();
            txtPresent.Text = present.ToString();
            txtAbsent.Text = absent.ToString();

            if (total > 0)
            {
                double rate = (double)present / total * 100;
                txtPercent.Text = $"{rate:0.0}%";
                txtPercent.Foreground = rate < 80
                    ? Brushes.Red
                    : (Brush)new BrushConverter().ConvertFromString("#38BDF8");
            }
            else
            {
                txtPercent.Text = "0%";
                txtPercent.Foreground = Brushes.Gray;
            }
        }


        private void btnSubmitLeave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLeaveReason.Text))
            {
                string msg = LanguageSettings.Language ? "理由を入力してください" : "Please enter a reason.";
                MessageBox.Show(msg, "Warning");
                return;
            }

            // Global Leave Request စာရင်းထဲ Mock ထည့်သွင်းခြင်း
            App.GlobalLeaveRequests.Add(new LeaveRequest
            {
                StudentID = UserData.UserData.CurrentUser?.Username,
                Reason = txtLeaveReason.Text,
                //Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm")
            });

            string successMsg = LanguageSettings.Language ? "申請を送信しました。" : "Request submitted successfully!";
            MessageBox.Show(successMsg, "Success");
            txtLeaveReason.Clear();
        }
    }
}