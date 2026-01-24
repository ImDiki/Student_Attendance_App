using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class TimetablePage : Page, ILanguageSwitchable
    {
        private string[] mockSubjects = { "PG実践", "DB開発", "WebDesign", "SysDev", "資格対策", "日本事情", "ビジネス" };

        public TimetablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage(LanguageSettings.Language);

            // Login ဝင်ထားတဲ့ User ရှိမရှိ စစ်ပါတယ်
            var user = UserData.UserData.CurrentUser;

            if (user != null && user.Role == "Student")
            {
                // ကျောင်းသားဆိုရင် သူ့ Major/Class ကို auto-select လုပ်ပြီး ပိတ်ထားမယ်
                SetComboBoxValue(cboYear, user.YearLevel);
                SetComboBoxValue(cboClass, user.AssignedClass);

                cboYear.IsEnabled = false;
                cboClass.IsEnabled = false;
            }

            LoadTimetable();
        }

        private void LoadTimetable()
        {
            // Grid ကို အရင်ရှင်းပါတယ်
            var itemsToRemove = TimetableGrid.Children.Cast<UIElement>()
                .Where(x => Grid.GetRow(x) > 0).ToList();
            foreach (var item in itemsToRemove) TimetableGrid.Children.Remove(item);

            string selectedYear = (cboYear.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "1st Year";
            string selectedClass = (cboClass.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "A";

            // Hybrid Logic: 1st Year အတွက်ဆိုရင် DB စစ်မယ်၊ မဟုတ်ရင် Mock ပြမယ်
            if (selectedYear == "1st Year")
            {
                bool hasData = FetchFromDatabase(selectedYear, selectedClass);
                if (!hasData) GenerateMockTimetable();
            }
            else
            {
                GenerateMockTimetable();
            }
        }

        private bool FetchFromDatabase(string year, string className)
        {
            bool dataFound = false;
            try
            {
                using (SqlConnection con = DBConnection.GetConnection())
                {
                    con.Open();
                    string sql = "SELECT * FROM Timetables WHERE YearLevel = @year AND ClassID = @class";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@class", className);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        string[] times = { "09:10", "10:50", "13:10", "14:50" };
                        for (int i = 1; i <= 4; i++) AddTimeLabel(i, times[i - 1]);

                        while (r.Read())
                        {
                            dataFound = true;
                            string subject = r["SubjectName"].ToString();
                            int row = Convert.ToInt32(r["Period"]);
                            int col = GetDayColumn(r["DayOfWeek"].ToString());
                            AddSubjectCard(row, col, subject);
                        }
                    }
                }
            }
            catch { return false; }
            return dataFound;
        }

        private void GenerateMockTimetable()
        {
            Random rnd = new Random(cboYear.SelectedIndex + cboClass.SelectedIndex + cboTerm.SelectedIndex);
            string[] times = { "09:10", "10:50", "13:10", "14:50" };

            for (int r = 1; r <= 4; r++)
            {
                AddTimeLabel(r, times[r - 1]);
                for (int c = 1; c <= 5; c++)
                {
                    string sub = rnd.Next(10) > 1 ? mockSubjects[rnd.Next(mockSubjects.Length)] : "-";
                    AddSubjectCard(r, c, sub);
                }
            }
        }

        // --- Helper Methods ---

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) LoadTimetable();
        }

        private void SetComboBoxValue(ComboBox combo, string value)
        {
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content.ToString() == value) { combo.SelectedItem = item; break; }
            }
        }

        private int GetDayColumn(string day)
        {
            day = day.ToUpper();
            if (day.Contains("MON") || day.Contains("月")) return 1;
            if (day.Contains("TUE") || day.Contains("火")) return 2;
            if (day.Contains("WED") || day.Contains("水")) return 3;
            if (day.Contains("THU") || day.Contains("木")) return 4;
            if (day.Contains("FRI") || day.Contains("金")) return 5;
            return 1;
        }

        public void ChangeLanguage(bool isJapanese)
        {
            txtTitle.Text = isJapanese ? "時間割 (TIMETABLE)" : "TIME TABLE";
            lblYearSelect.Text = isJapanese ? "学年: " : "Year: ";
            lblClassSelect.Text = isJapanese ? "クラス: " : "Class: ";
            lblTermSelect.Text = isJapanese ? "学期: " : "Term: ";
            UpdateTodayInfo(isJapanese);
        }

        private void UpdateTodayInfo(bool isJapanese)
        {
            DateTime today = DateTime.Now;
            txtTodayInfo.Text = isJapanese ? today.ToString("本日: yyyy年MM月dd日 (ddd)") : today.ToString(" 'Today: 'dddd, MMM dd,yyyy");
        }

        private void AddTimeLabel(int row, string time)
        {
            var lbl = new TextBlock { Text = $"{row}\n{time}", Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, TextAlignment = TextAlignment.Center, FontSize = 12 };
            var border = new Border { BorderBrush = new SolidColorBrush(Color.FromArgb(34, 255, 255, 255)), BorderThickness = new Thickness(0, 0, 1, 1), Child = lbl };
            Grid.SetRow(border, row); Grid.SetColumn(border, 0); TimetableGrid.Children.Add(border);
        }

        private void AddSubjectCard(int row, int col, string subject)
        {
            var border = new Border { BorderBrush = new SolidColorBrush(Color.FromArgb(34, 255, 255, 255)), BorderThickness = new Thickness(0, 0, 1, 1) };
            if (subject != "-")
            {
                var card = new Border { Background = GetSubjectBrush(subject), CornerRadius = new CornerRadius(8), Margin = new Thickness(4) };
                card.Child = new TextBlock { Text = subject, Foreground = Brushes.White, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Medium };
                border.Child = card;
            }
            Grid.SetRow(border, row); Grid.SetColumn(border, col); TimetableGrid.Children.Add(border);
        }

        private Brush GetSubjectBrush(string sub)
        {
            if (sub.Contains("PG") || sub.Contains("Sys")) return new SolidColorBrush(Color.FromArgb(60, 37, 99, 235));
            if (sub.Contains("DB")) return new SolidColorBrush(Color.FromArgb(60, 16, 185, 129));
            return new SolidColorBrush(Color.FromArgb(40, 255, 255, 255));
        }
    }
}