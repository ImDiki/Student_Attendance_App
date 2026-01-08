using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{


    public partial class TeacherDashboard : Page
    {
        public bool OpenManagerTab { get; set; } = false;
        public TeacherDashboard() { InitializeComponent(); }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtCurrentDate.Text = DateTime.Now.ToString("yyyy年MM月dd日 (ddd)");
            LoadDailyPeriods();
            RefreshList();
        }

        private void LoadDailyPeriods()
        {
            pnlDailyPeriods.Children.Clear();
            var day = DateTime.Now.DayOfWeek;
            var schedule = GetLatterTermSchedule(day);

            for (int i = 0; i < schedule.Count; i++)
            {
                int period = i + 1;
                string subject = schedule[i].Subj;
                string time = schedule[i].Time;

                if (subject == "-" || string.IsNullOrEmpty(subject)) continue;

                string key = $"{DateTime.Now:yyyyMMdd}_{day}_{period}";
                bool isStarted = App.StartedPeriods.Contains(key);

                Border b = new Border { Background = Brushes.White, CornerRadius = new CornerRadius(10), Padding = new Thickness(15), Margin = new Thickness(0, 0, 0, 10) };
                Grid g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                StackPanel sp = new StackPanel();
                sp.Children.Add(new TextBlock { Text = $"{period}限目 ({time})", FontWeight = FontWeights.Bold, Foreground = Brushes.DodgerBlue });
                sp.Children.Add(new TextBlock { Text = subject, FontSize = 18 });

                Button btn = new Button
                {
                    Content = isStarted ? "開始済み" : "授業開始",
                    IsEnabled = !isStarted,
                    Width = 110,
                    Height = 40,
                    Background = isStarted ? Brushes.Gray : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")),
                    Foreground = Brushes.White,
                    Tag = new { Subj = subject, P = period, Key = key }
                };
                btn.Click += StartClass_Click;

                Grid.SetColumn(btn, 1);
                g.Children.Add(sp); g.Children.Add(btn);
                b.Child = g; pnlDailyPeriods.Children.Add(b);
            }
        }

        private void StartClass_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            dynamic data = btn.Tag;

            if (MessageBox.Show($"{data.Subj} の授業を開始しますか？", "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.IsClassActive = true;
                App.CurrentActiveSessionStart = DateTime.Now;
                App.CurrentSubject = data.Subj;
                App.StartedPeriods.Add(data.Key);
                this.NavigationService.Navigate(new ScanPage());
            }
        }

        private List<(string Time, string Subj)> GetLatterTermSchedule(DayOfWeek day)
        {
            // Latter Term (後期) schedule based on image_57b450.jpg
            return day switch
            {
                DayOfWeek.Monday => new List<(string, string)> { ("09:10-10:40", "テスト技法"), ("10:50-12:20", "テスト技法"), ("13:10-14:40", "データ構造 II") },
                DayOfWeek.Tuesday => new List<(string, string)> { ("09:10-10:40", "ビジネスアプリ II"), ("10:50-12:20", "システム開発基礎") },
                DayOfWeek.Wednesday => new List<(string, string)> { ("09:10-10:40", "PG実践 I"), ("10:50-12:20", "PG実践 I"), ("13:10-14:40", "コミ技") },
                DayOfWeek.Thursday => new List<(string, string)> { ("09:10-10:40", "プログラミング II"), ("10:50-12:20", "プログラミング II"), ("13:10-14:40", "キャリアデザイン") },
                DayOfWeek.Friday => new List<(string, string)> { ("09:10-10:40", "ゼミナール I"), ("10:50-12:20", "データベース技術"), ("13:10-14:40", "データベース技術") },
                _ => new List<(string, string)>()
            };
        }

        private void RefreshList() { dgAttendance.ItemsSource = null; dgAttendance.ItemsSource = App.TempAttendanceList; }
        private void btnMarkPresent_Click(object sender, RoutedEventArgs e) { /* Update Present Logic */ RefreshList(); }
        private void btnMarkAbsent_Click(object sender, RoutedEventArgs e) { /* Update Absent Logic */ RefreshList(); }
    }
}