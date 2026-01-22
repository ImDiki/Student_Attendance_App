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
            ChangeLanguage(LanguageSettings.Language);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserData.UserData.CurrentUser;
            if (user != null)
            {
                UpdateWelcomeText(user.FullName, LanguageSettings.Language);
                txtYearDisplay.Text = user.YearLevel; // string တိုက်ရိုက်ပြခြင်း

                if (user.FacePhoto != null && user.FacePhoto.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(user.FacePhoto))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        imgProfileDisplay.Source = bitmap; // ပုံကို အဝိုင်း Border ထဲ ပြခြင်း
                    }
                }
                TimetableFrame.Navigate(new TimetablePage()); // Timetable အပြည့်ပေါ်ရန်
                LoadAttendanceStats(user.Username);
            }
        }

        public void ChangeLanguage(bool isJapanese)
        {
            var user = UserData.UserData.CurrentUser;
            if (user != null) UpdateWelcomeText(user.FullName, isJapanese);

            txtWelcomeSub.Text = isJapanese ? "出席状況を確認してください。" : "Check your attendance status.";
            lblTotal.Text = isJapanese ? "合計 (TOTAL)" : "TOTAL CLASSES";
            lblPresent.Text = isJapanese ? "出席 (PRESENT)" : "PRESENT";
            lblRate.Text = isJapanese ? "出席率 (RATE)" : "ATTENDANCE RATE";
            lblYear.Text = isJapanese ? "学年 (YEAR)" : "YEAR LEVEL";
            lblTimetable.Text = isJapanese ? "時間割 (Weekly Timetable)" : "Weekly Timetable";
        }

        private void UpdateWelcomeText(string name, bool isJapanese)
        {
            txtWelcome.Text = isJapanese ? $"{name} さん、ようこそ！" : $"{name}, Welcome back!";
        }

        private void LoadAttendanceStats(string studentID)
        {
            var myRecords = App.TempAttendanceList.Where(r => r.StudentID == studentID).ToList();
            txtTotal.Text = myRecords.Count.ToString();
            int presentCount = myRecords.Count(r => r.Status.Contains("Present") || r.Status.Contains("出席"));
            txtPresent.Text = presentCount.ToString();

            if (myRecords.Count > 0)
            {
                double rate = ((double)presentCount / myRecords.Count) * 100;
                txtPercent.Text = $"{rate:0.0}%";
            }
        }
    }
}