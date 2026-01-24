using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Student_Attendance_System.Models;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Services; // Service ကို သုံးရန်

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
            // ၁။ လက်ရှိ Login ဝင်ထားတဲ့ User ကို ယူပါတယ်
            var user = UserData.UserData.CurrentUser;

            if (user != null)
            {
                // ၂။ Language အရင်ပြောင်းပါတယ်
                ChangeLanguage(LanguageSettings.Language);

                txtYearDisplay.Text = user.YearLevel;

                // ၃။ Profile Image Loading (Byte array မှ Bitmap ပြောင်းခြင်း)
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

                // ၄။ Real Database မှ Attendance Stats များကို Load လုပ်ပါတယ်
                LoadAttendanceStats(user.Username);

                // ၅။ Timetable ကို Navigate လုပ်ပါတယ်
                TimetableFrame.Navigate(new TimetablePage());
            }
        }

        public void ChangeLanguage(bool isJapanese)
        {
            var user = UserData.UserData.CurrentUser;
            if (user != null)
            {
                // Welcome Message ကို နာမည်နဲ့တကွ bilingual ပြောင်းပေးပါတယ်
                txtWelcome.Text = isJapanese ? $"{user.FullName} さん၊ おかえりなさい！" : $"{user.FullName}, Welcome back!";
            }

            // Dashboard UI Label များကို ဘာသာစကားပြောင်းလဲခြင်း
            txtWelcomeSub.Text = isJapanese ? "出席状況を確認してください。" : "Check your attendance status.";
            lblTotal.Text = isJapanese ? "合計" : "TOTAL";
            lblPresent.Text = isJapanese ? "出席" : "PRESENT";
            lblRate.Text = isJapanese ? "出席率" : "ATTENDANCE RATE";
            lblYear.Text = isJapanese ? "学年" : "YEAR LEVEL";
            lblTimetable.Text = isJapanese ? "週間時間割" : "Weekly Timetable";
        }

        private void LoadAttendanceStats(string studentID)
        {
            // 🔹 Database Records မှ Real-time တွက်ချက်ခြင်း
            // AttendanceService ရှိ Method များကို အသုံးပြုပါတယ်

            int total = AttendanceService.GetTotalClasses(studentID);
            int present = AttendanceService.GetPresentCount(studentID);
            double rate = AttendanceService.GetAttendancePercentage(studentID);

            // UI Counters များကို Update လုပ်ခြင်း
            txtTotal.Text = total.ToString();
            txtPresent.Text = present.ToString();
            txtPercent.Text = $"{rate:0.0}%";

            // 80% အောက်ရောက်ပါက သတိပေးအရောင် (Salmon) ပြောင်းလဲခြင်း
            if (rate < 80)
            {
                txtPercent.Foreground = System.Windows.Media.Brushes.Salmon;
            }
        }
    }
}