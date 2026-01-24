using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Services;
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
            // ၁။ Login ဝင်ထားသော User ကို ယူပါသည်
            var user = UserData.UserData.CurrentUser;

            if (user != null)
            {
                ChangeLanguage(LanguageSettings.Language);
                txtYearDisplay.Text = user.YearLevel;

                if (user.FacePhoto != null && user.FacePhoto.Length > 0)
                {
                    imgProfileDisplay.Source = ConvertByteArrayToImage(user.FacePhoto);
                }

                // ⚠️ ၂။ Attendance Stats Load လုပ်ခြင်း (SqlException မတက်စေရန် user.Username ကို သေချာလှမ်းပို့ပါသည်)
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
            btnViewProfile.Content = isJapanese ? "プロフィールを表示" : "View Profile";
            // အခြား Label များ...
        }

        private void LoadAttendanceStats(string studentID)
        {
            // Database မှ data များ တွက်ချက်ရယူခြင်း
            int total = AttendanceService.GetTotalClasses(studentID);
            int present = AttendanceService.GetPresentCount(studentID);
            double rate = AttendanceService.GetAttendancePercentage(studentID);

            txtTotal.Text = total.ToString();
            txtPresent.Text = present.ToString();
            txtPercent.Text = $"{rate:0.0}%";
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            
                this.NavigationService.Navigate(new StudentProfile());
            
        }

        private BitmapImage ConvertByteArrayToImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}