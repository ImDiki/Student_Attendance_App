using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Services;
using System.Collections.Generic;


namespace Student_Attendance_System.Views
{
    public partial class AdminClassTimetablePage : Page
    {
        private readonly string[] Subjects =
{
    "-",
    "PG実践",
    "プログラミング",
    "データベース",
    "Webデザイン",
    "システム開発",
    "資格対策",
    "日本語表現法",
    "ネットワークとセキュリティ",
    "データ構造とアルゴリズム",
    "ビジネスアプリケーション",
    "コンピュータシステム",
    "コミュニケーション技法",
    "ITリテラシー",
    "ゼミナール",
    "テスト技法",
    "日本語",
    "AI",
    "クラウド"
};
        private readonly Dictionary<int, string> PeriodTimes = new()
{
    { 1, "09:10 - 10:40" },
    { 2, "10:50 - 12:20" },
    { 3, "13:10 - 14:40" },
    { 4, "14:50 - 16:20" },
    { 5, "16:30 - 18:00" }
};

        public AdminClassTimetablePage()
        {
            InitializeComponent();
            CheckAdmin();
            CreatePeriodLabels();   // ✅ ADD
            CreateEmptyTimetable();
        }


        private void CreatePeriodLabels()
        {
            // Remove old labels
            for (int i = TimetableGrid.Children.Count - 1; i >= 0; i--)
            {
                if (TimetableGrid.Children[i] is StackPanel sp &&
                    Grid.GetColumn(sp) == 0 &&
                    Grid.GetRow(sp) > 0)
                {
                    TimetableGrid.Children.RemoveAt(i);
                }
            }

            // Create new labels
            foreach (var kv in PeriodTimes)
            {
                int period = kv.Key;
                string time = kv.Value;

                StackPanel sp = new StackPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                sp.Children.Add(new TextBlock
                {
                    Text = period.ToString(),
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold
                });

                sp.Children.Add(new TextBlock
                {
                    Text = time,
                    FontSize = 11,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#94A3B8")
                });

                Grid.SetRow(sp, period);
                Grid.SetColumn(sp, 0);
                TimetableGrid.Children.Add(sp);
            }
        }

        // ===================== SECURITY =====================
        private void CheckAdmin()
        {
            if (UserData.UserData.CurrentUser.Role != "Admin")
            {
                MessageBox.Show("Access denied");
                NavigationService.GoBack();
            }
        }

        // ===================== UI =====================
        private void CreateEmptyTimetable()
        {
            for (int i = TimetableGrid.Children.Count - 1; i >= 0; i--)
            {
                if (TimetableGrid.Children[i] is ComboBox)
                    TimetableGrid.Children.RemoveAt(i);
            }

            for (int row = 1; row <= 5; row++)
            {
                for (int col = 1; col <= 5; col++)
                {
                    ComboBox cb = new ComboBox
                    {
                        ItemsSource = Subjects,
                        SelectedIndex = 0,
                        Margin = new Thickness(3),
                        Background = Brushes.White
                    };

                    Grid.SetRow(cb, row);
                    Grid.SetColumn(cb, col);
                    TimetableGrid.Children.Add(cb);
                }
            }
        }

        // ===================== LOAD =====================
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSelection()) return;

            CreateEmptyTimetable();

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = @"SELECT DayOfWeek, Period, SubjectName
                           FROM Timetables
                           WHERE YearLevel=@y AND ClassName=@c AND Term=@t";

            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@y", cboYear.Text);
            cmd.Parameters.AddWithValue("@c", cboClass.Text);
            cmd.Parameters.AddWithValue("@t", cboTerm.Text);

            using SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                int col = Convert.ToInt32(rd["DayOfWeek"]);
                int row = Convert.ToInt32(rd["Period"]);
                string subject = rd["SubjectName"].ToString();

                foreach (var item in TimetableGrid.Children)
                {
                    if (item is ComboBox cb &&
                        Grid.GetRow(cb) == row &&
                        Grid.GetColumn(cb) == col)
                    {
                        cb.SelectedItem = subject;
                        break;
                    }
                }
            }
        }

        // ===================== SAVE =====================
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSelection()) return;

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string deleteSql = @"DELETE FROM Timetables
                                 WHERE YearLevel=@y AND ClassName=@c AND Term=@t";

            using (SqlCommand del = new SqlCommand(deleteSql, con))
            {
                del.Parameters.AddWithValue("@y", cboYear.Text);
                del.Parameters.AddWithValue("@c", cboClass.Text);
                del.Parameters.AddWithValue("@t", cboTerm.Text);
                del.ExecuteNonQuery();
            }

            foreach (var item in TimetableGrid.Children)
            {
                if (item is ComboBox cb)
                {
                    int row = Grid.GetRow(cb);
                    int col = Grid.GetColumn(cb);
                    string subject = cb.SelectedItem?.ToString();

                    if (row == 0 || col == 0 || subject == "-" || string.IsNullOrEmpty(subject))
                        continue;

                    string time = PeriodTimes.ContainsKey(row)
    ? PeriodTimes[row]
    : "";

                    string insertSql = @"INSERT INTO Timetables
(YearLevel, ClassName, Term, Period, DayOfWeek, StartTime, SubjectName)
VALUES (@y,@c,@t,@p,@d,@time,@s)";

                    using SqlCommand ins = new SqlCommand(insertSql, con);
                    ins.Parameters.AddWithValue("@y", cboYear.Text);
                    ins.Parameters.AddWithValue("@c", cboClass.Text);
                    ins.Parameters.AddWithValue("@t", cboTerm.Text);
                    ins.Parameters.AddWithValue("@p", row);
                    ins.Parameters.AddWithValue("@d", col);
                    ins.Parameters.AddWithValue("@time", PeriodTimes[row]);
                    ins.Parameters.AddWithValue("@s", subject);
                    ins.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Timetable saved successfully");
        }

        // ===================== HELPERS =====================
        private bool ValidateSelection()
        {
            if (cboYear.SelectedItem == null ||
                cboClass.SelectedItem == null ||
                cboTerm.SelectedItem == null)
            {
                MessageBox.Show("Please select Year, Class and Term");
                return false;
            }
            return true;
        }
    }
}
