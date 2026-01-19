using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Models;
using Student_Attendance_System.Services;

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

            LoadLeaveRequests(); 
        }
        private void ApproveLeave_Click(object sender, RoutedEventArgs e)
        {
            UpdateLeaveStatus("Approved");
        }

        private void RejectLeave_Click(object sender, RoutedEventArgs e)
        {
            UpdateLeaveStatus("Rejected");
        }

        //private void UpdateLeaveStatus(string status)
        //{
        //    if (dgLeaveRequests.SelectedItem is not LeaveRequest req)
        //    {
        //        MessageBox.Show("申請を選択してください");
        //        return;
        //    }

        //    // 1️⃣ Update LeaveRequest status
        //    using (SqlConnection con = DBConnection.GetConnection())
        //    {
        //        con.Open();
        //        string sql = "UPDATE LeaveRequests SET Status=@Status WHERE RequestID=@ID";

        //        using (SqlCommand cmd = new SqlCommand(sql, con))
        //        {
        //            cmd.Parameters.AddWithValue("@Status", status);
        //            cmd.Parameters.AddWithValue("@ID", req.RequestID);
        //            cmd.ExecuteNonQuery();
        //        }
        //    }

        //    // 2️⃣ IF APPROVED
        //    if (status == "Approved")
        //    {
        //        string today = DateTime.Now.ToString("yyyy-MM-dd");

        //        // 1️⃣ CHECK: already exists in 出席管理?
        //        var existing = App.TempAttendanceList.FirstOrDefault(a =>
        //            a.StudentID == req.StudentID &&
        //            a.Subject == App.CurrentSubject &&
        //            a.Date.StartsWith(today)
        //        );

        //        // 2️⃣ IF NOT EXISTS → CREATE IT (公欠)
        //        if (existing == null)
        //        {
        //            // 👉 ADD TO 出席管理 (MEMORY)
        //            App.TempAttendanceList.Add(new AttendanceRecord
        //            {
        //                StudentID = req.StudentID,
        //                Subject = App.CurrentSubject,
        //                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
        //                Status = "Present",
        //                Note = "欠席届 承認（公欠）"
        //            });

        //            // 👉 SAVE TO Attendance TABLE
        //            using (SqlConnection con = DBConnection.GetConnection())
        //            {
        //                con.Open();
        //                string sql = @"INSERT INTO Attendance
        //                   (StudentID, Subject, ScanTime, Status)
        //                   VALUES (@StudentID, @Subject, @Time, @Status)";

        //                using (SqlCommand cmd = new SqlCommand(sql, con))
        //                {
        //                    cmd.Parameters.AddWithValue("@StudentID", req.StudentID);
        //                    cmd.Parameters.AddWithValue("@Subject", App.CurrentSubject);
        //                    cmd.Parameters.AddWithValue("@Time", DateTime.Now);
        //                    cmd.Parameters.AddWithValue("@Status", "Present");

        //                    cmd.ExecuteNonQuery();
        //                }
        //            }

        //            // 👉 UPDATE Students TABLE (attendance counters)
        //            using (SqlConnection con = DBConnection.GetConnection())
        //            {
        //                con.Open();
        //                string sql = @"UPDATE Students SET TotalClasses = TotalClasses + 1, PresentClasses = PresentClasses + 1 WHERE StudentID = @StudentID";

        //                using (SqlCommand cmd = new SqlCommand(sql, con))
        //                {
        //                    cmd.Parameters.AddWithValue("@StudentID", req.StudentID);
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }

        //            // 👉 UPDATE Students TABLE (attendance counters)
        //            using (SqlConnection con = DBConnection.GetConnection())
        //            {
        //                con.Open();
        //                string sql = @"
        //        UPDATE Students
        //        SET 
        //            TotalClasses = TotalClasses + 1,
        //            PresentClasses = PresentClasses + 1
        //        WHERE StudentID = @StudentID";

        //                using (SqlCommand cmd = new SqlCommand(sql, con))
        //                {
        //                    cmd.Parameters.AddWithValue("@StudentID", req.StudentID);
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //    }


        //    // 🔹 C. UPDATE STUDENT ATTENDANCE COUNTERS
        //    using (SqlConnection con = DBConnection.GetConnection())
        //        {
        //            con.Open();
        //            string sql = @"
        //        UPDATE Students
        //        SET 
        //            TotalClasses = TotalClasses + 1,
        //            PresentClasses = PresentClasses + 1
        //        WHERE StudentID = @StudentID";

        //            using (SqlCommand cmd = new SqlCommand(sql, con))
        //            {
        //                cmd.Parameters.AddWithValue("@StudentID", req.StudentID);
        //                cmd.ExecuteNonQuery();
        //            }
        //        }
        //    }


        private void UpdateLeaveStatus(string status)
        {
            if (dgLeaveRequests.SelectedItem is not LeaveRequest req)
            {
                MessageBox.Show("申請を選択してください");
                return;
            }

            // 1️⃣ Update LeaveRequest status
            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();
                string sql = "UPDATE LeaveRequests SET Status=@Status WHERE RequestID=@ID";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ID", req.RequestID);
                    cmd.ExecuteNonQuery();
                }
            }

            // ======================
            // APPROVED → PRESENT
            // ======================
            if (status == "Approved")
            {
                // Prevent duplicate
                var exists = App.TempAttendanceList.FirstOrDefault(a =>
                    a.StudentID == req.StudentID &&
                    a.Subject == App.CurrentSubject &&
                    a.Date.StartsWith(DateTime.Now.ToString("yyyy-MM-dd"))
                );

                if (exists == null)
                {
                    // 出席管理 (memory)
                    App.TempAttendanceList.Add(new AttendanceRecord
                    {
                        StudentID = req.StudentID,
                        Subject = App.CurrentSubject,
                        Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                        Status = "Present",
                        Note = "欠席届 承認（公欠）"
                    });

                    // DB → PresentClasses +1 ONLY
                    using (SqlConnection con = DBConnection.GetConnection())
                    {
                        con.Open();
                        string sql = @"
UPDATE Students
SET PresentClasses = PresentClasses + 1
WHERE StudentID = @StudentID";

                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@StudentID", req.StudentID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            // ======================
            // REJECTED → ABSENT
            // ======================
            if (status == "Rejected")
            {
                // 出席管理 (ABSENT only, no DB counter change)
                App.TempAttendanceList.Add(new AttendanceRecord
                {
                    StudentID = req.StudentID,
                    Subject = App.CurrentSubject,
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    Status = "Absent",
                    Note = "欠席届 却下"
                });
            }

            LoadLeaveRequests();
            RefreshList();
        }


        private void IncreaseTotalClassesForAllStudents()
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();
                string sql = "UPDATE Students SET TotalClasses = TotalClasses + 1";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
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

        //private void StartClass_Click(object sender, RoutedEventArgs e)
        //{
        //    var btn = sender as Button;
        //    dynamic data = btn.Tag;

        //    if (MessageBox.Show($"{data.Subj} の授業を開始しますか？", "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //    {
        //        App.IsClassActive = true;
        //        App.CurrentActiveSessionStart = DateTime.Now;
        //        App.CurrentSubject = data.Subj;
        //        App.StartedPeriods.Add(data.Key);
        //        this.NavigationService.Navigate(new ScanPage());
        //    }
        //}

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

                // ✅ IMPORTANT: TotalClasses +1 (everyone absent by default)
                IncreaseTotalClassesForAllStudents();

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
        private void LoadLeaveRequests()
        {
            List<LeaveRequest> list = new List<LeaveRequest>();

            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();
                string sql = @"SELECT * FROM LeaveRequests
                       WHERE Status = 'Pending'
                       ORDER BY RequestDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LeaveRequest
                        {
                            RequestID = (int)reader["RequestID"],
                            StudentID = reader["StudentID"].ToString(),
                            Reason = reader["Reason"].ToString(),
                            Status = reader["Status"].ToString(),
                            RequestDate = Convert.ToDateTime(reader["RequestDate"])
                        });
                    }
                }
            }

            dgLeaveRequests.ItemsSource = list;
        }



        private void RefreshList() { dgAttendance.ItemsSource = null; dgAttendance.ItemsSource = App.TempAttendanceList; }
        private void btnMarkPresent_Click(object sender, RoutedEventArgs e) { /* Update Present Logic */ RefreshList(); }
        private void btnMarkAbsent_Click(object sender, RoutedEventArgs e) { /* Update Absent Logic */ RefreshList(); }
    }
}