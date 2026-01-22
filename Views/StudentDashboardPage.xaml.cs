using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class StudentDashboardPage : Page
    {
        public StudentDashboardPage() { InitializeComponent(); }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserData.UserData.CurrentUser;
            if (user == null) return;

            // နာမည်ပြခြင်း
            txtWelcome.Text = $"{user.FullName} さん、ようこそ！";
            txtYearLevel.Text = user.YearLevel;

            // ဓာတ်ပုံပြန်ပြခြင်း logic
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

            TimetableFrame.Navigate(new TimetablePage());
            LoadAttendanceStats();
        }

        private void LoadAttendanceStats()
        {
            var user = UserData.UserData.CurrentUser;
            if (user == null) return;
            var myRecords = App.TempAttendanceList.Where(r => r.StudentID == user.Username).ToList();
            txtTotal.Text = myRecords.Count.ToString();
            txtPresent.Text = myRecords.Count(r => r.Status.Contains("Present") || r.Status.Contains("出席")).ToString();
            if (myRecords.Count > 0)
            {
                double rate = ((double)int.Parse(txtPresent.Text) / myRecords.Count) * 100;
                txtPercent.Text = $"{rate:0.0}%";
            }
        }
    }
}