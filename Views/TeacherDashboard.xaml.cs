using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Student_Attendance_System.Models;
using Student_Attendance_System.Interfaces;
using Microsoft.Data.SqlClient;

namespace Student_Attendance_System.Views
{
    public partial class TeacherDashboard : Page, ILanguageSwitchable
    {
        public TeacherDashboard() { InitializeComponent(); ChangeLanguage(LanguageSettings.Language); }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtCurrentDate.Text = LanguageSettings.Language
                ? DateTime.Now.ToString("yyyy年MM月dd日 (ddd)")
                : DateTime.Now.ToString("yyyy/MM/dd (ddd)");
            LoadDailyPeriods();
            RefreshList();
        }

        public void ChangeLanguage(bool isJapanese)
        {
            txtTitle.Text = isJapanese ? "先生用ポータル" : "Teacher Portal";
            tabClass.Header = isJapanese ? " 📅 本日の授業開始 " : " 📅 Start Today's Class ";
            tabManage.Header = isJapanese ? " 📝 出席管理 " : " 📝 Attendance Manage ";
            txtListTitle.Text = isJapanese ? "リアルタイム出席リスト" : "Real-time Attendance List";
            lblNote.Text = isJapanese ? "修正理由（備考）：" : "Modification Reason (Note):";
            btnPresent.Content = isJapanese ? "出席にする" : "Mark Present";
            btnAbsent.Content = isJapanese ? "欠席にする" : "Mark Absent";
            LoadDailyPeriods(); // Refresh dynamic cards language
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

                // --- Glass Card UI Creation ---
                Border b = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(20, 255, 255, 255)),
                    CornerRadius = new CornerRadius(15),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                    Padding = new Thickness(20),
                    Margin = new Thickness(0, 0, 0, 12)
                };

                Grid g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                StackPanel sp = new StackPanel();
                sp.Children.Add(new TextBlock { Text = $"{period}限目 ({time})", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38BDF8")) });
                sp.Children.Add(new TextBlock { Text = subject, FontSize = 20, Foreground = Brushes.White, Margin = new Thickness(0, 5, 0, 0) });

                Button btn = new Button
                {
                    Content = isStarted ? (LanguageSettings.Language ? "開始済み" : "Started") : (LanguageSettings.Language ? "授業開始" : "Start Class"),
                    IsEnabled = !isStarted,
                    Width = 130,
                    Height = 45,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Background = isStarted ? Brushes.DimGray : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
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
            string confirmMsg = LanguageSettings.Language ? $"{data.Subj} の授業を開始しますか？" : $"Start {data.Subj} class?";

            if (MessageBox.Show(confirmMsg, "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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

        private void RefreshList()
        {
            App.TempAttendanceList.Clear();

            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

                string query = @"
SELECT AttendanceID, StudentID, Status, Note
FROM Attendance
WHERE AttendanceDate = CAST(GETDATE() AS DATE)
  AND Subject = @subject";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@subject", App.CurrentSubject);

                SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    App.TempAttendanceList.Add(new AttendanceRecord
                    {
                        AttendanceID = (int)r["AttendanceID"],
                        StudentID = r["StudentID"].ToString(),
                        Status = r["Status"].ToString(),
                        Note = r["Note"]?.ToString()
                    });
                }
            }

            dgAttendance.ItemsSource = null;
            dgAttendance.ItemsSource = App.TempAttendanceList;
        }

        private void btnMarkPresent_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendance.SelectedItem is not AttendanceRecord selected) return;

            UpdateAttendance(selected.AttendanceID, "Present", txtReason.Text);
            RefreshList();
        }

        private void btnMarkAbsent_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendance.SelectedItem is not AttendanceRecord selected) return;

            UpdateAttendance(selected.AttendanceID, "Absent", txtReason.Text);
            RefreshList();
        }
        private void UpdateAttendance(int attendanceId, string status, string note)
{
    using (SqlConnection con = DBConnection.GetConnection())
    {
        con.Open();

        string query = @"
UPDATE Attendance
SET Status = @status,
    Note = @note
WHERE AttendanceID = @id";

        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@status", status);
        cmd.Parameters.AddWithValue("@note", note ?? "");
        cmd.Parameters.AddWithValue("@id", attendanceId);

        cmd.ExecuteNonQuery();
    }
}

    }
}