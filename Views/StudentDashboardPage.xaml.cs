using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Student_Attendance_System.UserData;

namespace Student_Attendance_System.Views
{
    public partial class StudentDashboardPage : Page
    {
        public StudentDashboardPage()
        {
            InitializeComponent();
            LoadStudentData();
        }

        private void LoadStudentData()
        {
            // ၁။ နာမည်ပြမယ်
            if (UserData.UserData.CurrentUser != null)
            {
                txtWelcome.Text = $"Welcome back, {UserData.UserData.CurrentUser.FullName}!";
            }

            // =========================================================
            // DATA LOADING FROM MEMORY
            // =========================================================

            // List ထဲမှာ Data ရှိမရှိ စစ်မယ် (မရှိရင် 0 တွေပဲ ပြမယ်)
            var dataList = App.TempAttendanceList;

            int total = dataList.Count;
            int present = 0;
            int absent = 0;

            // List ကို ရှင်းပြီးမှ ပြန်ထည့်မယ် (History List Update)
            pnlHistoryList.Children.Clear();

            // Header ပြန်ထည့်မယ်
            AddHeaderRow();

            // Loop ပတ်ပြီး ရေတွက်မယ် + စာရင်းထုတ်မယ်
            foreach (var record in dataList)
            {
                if (record.Status.Contains("Present")) present++;
                else absent++;

                // History စာရင်းထဲ အတန်းလိုက်ထည့်မယ်
                AddHistoryRow(record.Date, record.Subject, record.Status);
            }

            // UI ဂဏန်းတွေ ပြမယ်
            txtTotal.Text = total.ToString();
            txtPresent.Text = present.ToString();
            txtAbsent.Text = absent.ToString();

            // ရာခိုင်နှုန်း
            if (total > 0)
            {
                double percentage = ((double)present / total) * 100;
                txtPercent.Text = $"{percentage:0.0}%";
            }
            else
            {
                txtPercent.Text = "0%";
            }
        }

        // C# Code နဲ့ UI (Grid/TextBlock) ဆောက်ပြီး ထည့်ခြင်း
        private void AddHistoryRow(string date, string subject, string status)
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(0, 5, 0, 5);
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock txtDate = new TextBlock { Text = date };
            TextBlock txtSubject = new TextBlock { Text = subject, FontWeight = FontWeights.SemiBold }; // Grid.Column=1 Auto

            TextBlock txtStatus = new TextBlock { Text = status, FontWeight = FontWeights.Bold };
            if (status.Contains("Present")) txtStatus.Foreground = Brushes.Green;
            else txtStatus.Foreground = Brushes.Red;

            Grid.SetColumn(txtDate, 0);
            Grid.SetColumn(txtSubject, 1);
            Grid.SetColumn(txtStatus, 2);

            grid.Children.Add(txtDate);
            grid.Children.Add(txtSubject);
            grid.Children.Add(txtStatus);

            pnlHistoryList.Children.Add(grid);
        }

        private void AddHeaderRow()
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(0, 0, 0, 10);
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock t1 = new TextBlock { Text = "Date", FontWeight = FontWeights.Bold, Foreground = Brushes.Gray };
            TextBlock t2 = new TextBlock { Text = "Subject", FontWeight = FontWeights.Bold, Foreground = Brushes.Gray };
            TextBlock t3 = new TextBlock { Text = "Status", FontWeight = FontWeights.Bold, Foreground = Brushes.Gray };

            Grid.SetColumn(t1, 0);
            Grid.SetColumn(t2, 1);
            Grid.SetColumn(t3, 2);

            grid.Children.Add(t1);
            grid.Children.Add(t2);
            grid.Children.Add(t3);

            pnlHistoryList.Children.Add(grid);
            pnlHistoryList.Children.Add(new Separator { Margin = new Thickness(0, 0, 0, 10) });
        }
    }
}