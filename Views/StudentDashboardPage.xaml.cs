using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Student_Attendance_System.Models;
using Student_Attendance_System.Interfaces;

namespace Student_Attendance_System.Views
{
    public partial class StudentDashboardPage : Page, ILanguageSwitchable
    {
        public StudentDashboardPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserData.UserData.CurrentUser;
            if (user != null)
            {
                // Language အရင်ပြောင်းပါ
                ChangeLanguage(LanguageSettings.Language);

                txtYearDisplay.Text = user.YearLevel;

                // Profile Image Loading
                if (user.FacePhoto != null && user.FacePhoto.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(user.FacePhoto))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        imgProfileDisplay.Source = bitmap;
                    }
                }

                LoadAttendanceStats(user.Username);
                TimetableFrame.Navigate(new TimetablePage());
            }
        }

        public void ChangeLanguage(bool isJapanese)
        {
            var user = UserData.UserData.CurrentUser;
            if (user != null)
            {
                txtWelcome.Text = isJapanese ? $"{user.FullName} さん、おかえりなさい！" : $"{user.FullName}, Welcome back!";
            }

            txtWelcomeSub.Text = isJapanese ? "出席状況を確認してください。" : "Check your attendance status.";
            lblTotal.Text = isJapanese ? "合計" : "TOTAL";
            lblPresent.Text = isJapanese ? "出席" : "PRESENT";
            lblRate.Text = isJapanese ? "出席率" : "ATTENDANCE RATE";
            lblYear.Text = isJapanese ? "学年" : "YEAR LEVEL";
            lblTimetable.Text = isJapanese ? "週間時間割" : "Weekly Timetable";
        }

        private void LoadAttendanceStats(string studentID)
        {
            // Database records မှ တွက်ချက်ခြင်း
            var myRecords = App.TempAttendanceList.Where(r => r.StudentID == studentID).ToList();
            int total = myRecords.Count;
            int present = myRecords.Count(r => r.Status.Contains("Present") || r.Status.Contains("出席"));

            txtTotal.Text = total.ToString();
            txtPresent.Text = present.ToString();
            if (total > 0)
            {
                double rate = ((double)present / total) * 100;
                txtPercent.Text = $"{rate:0.0}%";
            }
        }
    }
}