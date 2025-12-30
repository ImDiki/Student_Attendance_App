using System;
using System.Windows;
using System.Windows.Controls;
using Student_Attendance_System.UserData; // Role စစ်ဖို့

namespace Student_Attendance_System.Views
{
    public partial class TimetablePage : Page
    {
        public TimetablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSchedule();
            CheckUserRole();
        }

        private void CheckUserRole()
        {
            // ဆရာမ ဖြစ်မှ Start Button တွေကို ဖော်မယ်
            if (UserData.UserData.CurrentUser != null && UserData.UserData.CurrentUser.Role == "Teacher")
            {
                btnP1.Visibility = Visibility.Visible;
                btnP2.Visibility = Visibility.Visible;
                btnP3.Visibility = Visibility.Visible;
            }
            else
            {
                // ကျောင်းသားဆိုရင် ပိတ်ထားမယ် (မြင်တော့မြင်ရမယ်၊ နှိပ်မရတာ)
                btnP1.Visibility = Visibility.Collapsed;
                btnP2.Visibility = Visibility.Collapsed;
                btnP3.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadSchedule()
        {
            DayOfWeek today = DateTime.Now.DayOfWeek;
            txtToday.Text = "Today: " + today.ToString();

            // (Schedule Logic - 
            // Example:
            if (today == DayOfWeek.Tuesday)
            {
                txtP1.Text = "Business Application I"; btnP1.Tag = txtP1.Text;
                txtP2.Text = "Programming I (C#)"; btnP2.Tag = txtP2.Text;
                txtP3.Text = "Data Structure"; btnP3.Tag = txtP3.Text;
            }
            // ... Other days
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string subject = btn.Tag.ToString();

            App.IsClassActive = true;
            App.CurrentActiveSessionStart = DateTime.Now;
            App.CurrentSubject = subject;

            MessageBox.Show($"Class Started: {subject}", "Success");
            btn.Background = System.Windows.Media.Brushes.Green;
            btn.Content = "IN PROGRESS";
        }
    }
}