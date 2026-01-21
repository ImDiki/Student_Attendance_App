using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class TimetablePage : Page, ILanguageSwitchable
    {
        public TimetablePage()
        {
            InitializeComponent();
            ChangeLanguage(LanguageSettings.Language);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaultSelections();   // ✅ sets Year/Class/Term so they are never null
            CheckUserAccess();        // ✅ if student, overwrite Year/Class to his own
            UpdateTodayInfo();
            LoadTimetable();
        }

        // ===================== DEFAULTS (IMPORTANT) =====================
        private void SetDefaultSelections()
        {
            // Year default
            if (cboYear.SelectedItem == null && cboYear.Items.Count > 0)
                cboYear.SelectedIndex = 0;

            // Class default
            if (cboClass.SelectedItem == null && cboClass.Items.Count > 0)
                cboClass.SelectedIndex = 0;

            // Term default (後期)
            if (cboTerm.SelectedItem == null && cboTerm.Items.Count > 1)
                cboTerm.SelectedIndex = 1;
        }

        // ===================== ACCESS CONTROL =====================
        private void CheckUserAccess()
        {
            var user = UserData.UserData.CurrentUser;

            // safety check
            if (user == null)
            {
                MessageBox.Show("User session lost.", "Error");
                return;
            }

            // STUDENT ONLY
            if (user.Role == "Student")
            {
                // validate user info first
                if (user.YearLevel <= 0 || string.IsNullOrWhiteSpace(user.AssignedClass))
                {
                    MessageBox.Show(
                        $"Student info is missing!\n\n" +
                        $"YearLevel = {user.YearLevel}\n" +
                        $"AssignedClass = '{user.AssignedClass}'\n\n" +
                        "Fix this in DB / AuthService.",
                        "Timetable Error");

                    return; // ⛔ STOP here
                }

                bool yearOk = SelectComboByTag(cboYear, user.YearLevel.ToString());
                bool classOk = SelectComboByContent(cboClass, user.AssignedClass);

                if (!yearOk || !classOk)
                {
                    MessageBox.Show(
                        $"ComboBox mismatch!\n\n" +
                        $"Expected:\nTag = '{user.YearLevel}'\nClass = '{user.AssignedClass}'\n\n" +
                        $"Check ComboBox Items EXACTLY.",
                        "Timetable Error");

                    return; // ⛔ STOP here
                }

                // 🔒 lock AFTER success
                cboYear.IsEnabled = false;
                cboClass.IsEnabled = false;
            }
            else
            {
                // ADMIN / TEACHER
                cboYear.IsEnabled = true;
                cboClass.IsEnabled = true;
            }
        }
        private bool SelectComboByTag(ComboBox combo, string tag)
        {
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Tag?.ToString() == tag)
                {
                    combo.SelectedItem = item;
                    return true;
                }
            }
            return false;
        }

        private bool SelectComboByContent(ComboBox combo, string value)
        {
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content.ToString() == value)
                {
                    combo.SelectedItem = item;
                    return true;
                }
            }
            return false;
        }

        // ===================== LANGUAGE =====================
        public void ChangeLanguage(bool isJapanese)
        {
            txtTitle.Text = isJapanese ? "時間割" : "Time Table";
            lblYearSelect.Text = isJapanese ? "学年:" : "Year:";
            lblClassSelect.Text = isJapanese ? "クラス:" : "Class:";
            lblTermSelect.Text = isJapanese ? "学期:" : "Term:";
            hPeriod.Text = isJapanese ? "時限" : "Period";
            hMon.Text = isJapanese ? "月" : "Mon";
            hTue.Text = isJapanese ? "火" : "Tue";
            hWed.Text = isJapanese ? "水" : "Wed";
            hThu.Text = isJapanese ? "木" : "Thu";
            hFri.Text = isJapanese ? "金" : "Fri";
        }

        private void UpdateTodayInfo()
        {
            DateTime today = DateTime.Now;
            txtTodayInfo.Text = LanguageSettings.Language
                ? today.ToString("yyyy/MM/dd dddd")
                : today.ToString("MMM dd, yyyy (dddd)");
        }

        // ===================== FILTER =====================
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) LoadTimetable();
        }

        // ===================== LOAD =====================
        private void LoadTimetable()
        {
            if (cboYear.SelectedItem == null ||
                cboClass.SelectedItem == null ||
                cboTerm.SelectedItem == null)
                return;

            int year = int.Parse(((ComboBoxItem)cboYear.SelectedItem).Tag.ToString());
            string className = ((ComboBoxItem)cboClass.SelectedItem).Content.ToString();

            LoadTimetableFromDB(year, className);
        }

        // ===================== DATABASE =====================
        private void LoadTimetableFromDB(int year, string className)
        {
            ClearGrid();

            string termText = ((ComboBoxItem)cboTerm.SelectedItem).Content.ToString();
            string term = termText.Contains("前期") ? "前期" : "後期";

            List<Timetable> list = new();

            using (SqlConnection con = DBConnection.GetConnection())
            {
                string sql = @"SELECT YearLevel, ClassName, Term, DayOfWeek, Period, StartTime, SubjectName
                               FROM Timetables
                               WHERE YearLevel = @Year
                                 AND ClassName = @ClassName
                                 AND Term = @Term";

                using SqlCommand cmd = new(sql, con);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@ClassName", className);
                cmd.Parameters.AddWithValue("@Term", term);

                con.Open();
                using SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    list.Add(new Timetable
                    {
                        YearLevel = (int)rd["YearLevel"],
                        ClassName = rd["ClassName"].ToString(),
                        Term = rd["Term"].ToString(),
                        DayOfWeek = (int)rd["DayOfWeek"],
                        Period = (int)rd["Period"],
                        StartTime = rd["StartTime"]?.ToString(),
                        SubjectName = rd["SubjectName"]?.ToString()
                    });
                }
            }

            for (int p = 1; p <= 5; p++)
            {
                string time = list.Find(x => x.Period == p)?.StartTime ?? "";
                AddRowFromDB(p, time, list);
            }
        }

        // ===================== GRID =====================
        private void ClearGrid()
        {
            for (int i = TimetableGrid.Children.Count - 1; i >= 0; i--)
            {
                if (Grid.GetRow(TimetableGrid.Children[i]) > 0)
                    TimetableGrid.Children.RemoveAt(i);
            }
        }

        private void AddRowFromDB(int row, string time, List<Timetable> data)
        {
            Border left = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 0, 1, 1)
            };

            left.Child = new TextBlock
            {
                Text = $"{row}\n{time}",
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.Gray
            };

            Grid.SetRow(left, row);
            Grid.SetColumn(left, 0);
            TimetableGrid.Children.Add(left);

            for (int day = 1; day <= 5; day++)
            {
                var item = data.Find(x => x.Period == row && x.DayOfWeek == day);
                AddCard(row, day, item?.SubjectName ?? "-");
            }
        }

        private void AddCard(int row, int col, string subject)
        {
            Border border = new Border
            {
                BorderThickness = new Thickness(0, 0, 1, 1),
                BorderBrush = Brushes.Gray
            };

            if (subject == "-" || string.IsNullOrWhiteSpace(subject))
            {
                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);
                TimetableGrid.Children.Add(border);
                return;
            }

            Button btn = new Button
            {
                Content = subject,
                Margin = new Thickness(2),
                FontSize = 12,
                IsHitTestVisible = false,
                Background = Brushes.White,
                BorderThickness = new Thickness(0)
            };

            border.Child = btn;
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            TimetableGrid.Children.Add(border);
        }
    }
}
