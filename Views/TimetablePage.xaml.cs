using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Student_Attendance_System.Interfaces;

namespace Student_Attendance_System.Views
{
    public partial class TimetablePage : Page, ILanguageSwitchable
    {
        private string[] mockSubjects = { "PG実践", "DB開発", "WebDesign", "SysDev", "資格対策", "日本事情", "ビジネス" };

        public TimetablePage()
        {
            InitializeComponent();
            ChangeLanguage(LanguageSettings.Language);
            CheckUserAccess();
            LoadTimetable();
        }

        private void CheckUserAccess()
        {
            var user = UserData.UserData.CurrentUser;
            if (user != null && user.Role == "Student")
            {
                cboYear.IsEnabled = false;
                cboClass.IsEnabled = false;
            }
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
            txtTodayInfo.Text = isJapanese ? today.ToString("本日: yyyy年MM月dd日 (ddd)") : today.ToString("Today: dddd, MMM dd");
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e) { if (IsLoaded) LoadTimetable(); }

        private void LoadTimetable()
        {
            // Clear items with Linq safely
            var itemsToRemove = TimetableGrid.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) > 0 || Grid.GetColumn(x) >= 0).ToList();
            foreach (var item in itemsToRemove) TimetableGrid.Children.Remove(item);

            AddHeaders();

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

        private void AddHeaders()
        {
            string[] headers = LanguageSettings.Language ? new[] { "時限", "月", "火", "水", "木", "金" } : new[] { "Period", "MON", "TUE", "WED", "THU", "FRI" };
            for (int i = 0; i < headers.Length; i++)
            {
                var lbl = new TextBlock { Text = headers[i], Foreground = Brushes.White, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                var border = new Border { BorderBrush = new SolidColorBrush(Color.FromArgb(34, 255, 255, 255)), BorderThickness = new Thickness(0, 0, 1, 1), Child = lbl };
                Grid.SetRow(border, 0); Grid.SetColumn(border, i); TimetableGrid.Children.Add(border);
            }
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
                // CS0117 Fix: FontWeights (with 's') Medium ကို သုံးထားပါသည်
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