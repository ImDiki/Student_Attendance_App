using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Student_Attendance_System.Interfaces;

namespace Student_Attendance_System.Views
{
    public partial class TimetablePage : Page, ILanguageSwitchable
    {
        private string[] mockSubjects = { "PG実践", "プログラミング", "データベース", "Webデザイン", "システム開発", "資格対策", "日本事情", "ビジネス" };

        public TimetablePage()
        {
            InitializeComponent();

            // ၁။ Language Setting ကို အရင်ယူမယ်
            ChangeLanguage(LanguageSettings.Language);

            // ၂။ Login Status ကို စစ်ပြီး UI ကို Lock/Unlock လုပ်မယ်
            CheckUserAccess();

            UpdateTodayInfo();
            LoadTimetable();
        }

        private void CheckUserAccess()
        {
            var user = UserData.UserData.CurrentUser;

            // ကျောင်းသား Login ဝင်ထားတယ်ဆိုရင်
            if (user != null && user.Role == "Student")
            {
                // သူနဲ့ဆိုင်တဲ့ Year Level ကို Select လုပ်မယ် (Tag သုံးပြီး ရှာတာပါ)
                foreach (ComboBoxItem item in cboYear.Items)
                {
                    if (item.Tag?.ToString() == user.YearLevel.ToString())
                    {
                        cboYear.SelectedItem = item;
                        break;
                    }
                }

                // သူ့ရဲ့ အခန်း (A, B, C...) ကို Select လုပ်မယ်
                foreach (ComboBoxItem item in cboClass.Items)
                {
                    if (item.Content.ToString() == user.AssignedClass)
                    {
                        cboClass.SelectedItem = item;
                        break;
                    }
                }

                // ကျောင်းသားဆိုရင် သူ့အတန်းပဲ သူကြည့်ရမယ် (ရွေးလို့မရအောင် ပိတ်ထားမယ်)
                cboYear.IsEnabled = false;
                cboClass.IsEnabled = false;
            }
            else
            {
                // Login မဝင်ထားရင် (Guest) အကုန်ရွေးကြည့်လို့ရအောင် ဖွင့်ပေးထားမယ်
                cboYear.IsEnabled = true;
                cboClass.IsEnabled = true;
            }
        }

        private void UpdateTodayInfo()
        {
            DateTime today = DateTime.Now;
            if (LanguageSettings.Language) // Japanese Mode
            {
                txtTodayInfo.Text = $"本日: {today.ToString("yyyy年MM月dd日 (dddd)")}";
            }
            else // English Mode
            {
                txtTodayInfo.Text = $"Today: {today.ToString("dddd, MMM dd, yyyy")}";
            }
        }

        public void ChangeLanguage(bool isJapanese)
        {
            if (isJapanese)
            {
                txtTitle.Text = "時間割 (Time Table)";
                lblClassSelect.Text = "クラス選択: ";
                lblTermSelect.Text = "学期: ";
                hPeriod.Text = "時限";
                hMon.Text = "月"; hTue.Text = "火"; hWed.Text = "水"; hThu.Text = "木"; hFri.Text = "金";
            }
            else
            {
                txtTitle.Text = "Time Table";
                lblClassSelect.Text = "Class Select: ";
                lblTermSelect.Text = "Term: ";
                hPeriod.Text = "Period";
                hMon.Text = "Mon"; hTue.Text = "Tue"; hWed.Text = "Wed"; hThu.Text = "Thu"; hFri.Text = "Fri";
            }
            UpdateTodayInfo();
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (TimetableGrid != null) LoadTimetable();
        }

        private void LoadTimetable()
        {
            // Clear current cards except headers
            for (int i = TimetableGrid.Children.Count - 1; i >= 0; i--)
            {
                if (Grid.GetRow(TimetableGrid.Children[i]) > 0)
                    TimetableGrid.Children.RemoveAt(i);
            }

            // Year, Class, Term ၃ ခုလုံးကို ပေါင်းပြီး Seed လုပ်မယ်
            // ဒါမှ 1A နဲ့ 4C ရဲ့ timetable တွေက လုံးဝမတူဘဲ ထွက်လာမှာပါ
            int yearIndex = cboYear.SelectedIndex;
            int classIndex = cboClass.SelectedIndex;
            int termIndex = cboTerm.SelectedIndex;

            Random rnd = new Random(yearIndex * 100 + classIndex * 10 + termIndex);

            string selectedClass = (cboClass.SelectedItem as ComboBoxItem)?.Content.ToString();

            AddRow(1, "09:10", rnd, selectedClass);
            AddRow(2, "10:50", rnd, selectedClass);
            AddRow(3, "13:10", rnd, selectedClass);
            AddRow(4, "14:50", rnd, selectedClass);
        }

        private void AddRow(int row, string startTime, Random rnd, string className)
        {
            var border = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0, 0, 1, 1) };
            var lbl = new TextBlock
            {
                Text = $"{row}\n{startTime}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 11,
                Foreground = Brushes.Gray,
                TextAlignment = TextAlignment.Center
            };
            border.Child = lbl;
            Grid.SetRow(border, row); Grid.SetColumn(border, 0);
            TimetableGrid.Children.Add(border);

            for (int col = 1; col <= 5; col++)
            {
                // Random Subject Generation
                string subject = rnd.Next(10) > 2 ? mockSubjects[rnd.Next(mockSubjects.Length)] : "-";
                AddCard(row, col, subject);
            }
        }

        private void AddCard(int row, int col, string subject)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E0")),
                BorderThickness = new Thickness(0, 0, 1, 1)
            };

            if (subject == "-")
            {
                Grid.SetRow(border, row); Grid.SetColumn(border, col);
                TimetableGrid.Children.Add(border);
                return;
            }

            // Button ကို သုံးထားပေမယ့် IsEnabled = false လုပ်ထားရင် Click လို့မရတော့ဘူး (Read-only view)
            Button btn = new Button
            {
                Content = subject,
                Margin = new Thickness(2),
                FontSize = 12,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GetColor(subject))),
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Arrow,
                IsHitTestVisible = false // Click လို့ လုံးဝမရအောင် လုပ်တာပါ
            };

            border.Child = btn;
            Grid.SetRow(border, row); Grid.SetColumn(border, col);
            TimetableGrid.Children.Add(border);
        }

        private string GetColor(string sub)
        {
            if (sub.Contains("プロ")) return "#EBF8FF";
            if (sub.Contains("資格")) return "#FFF5F5";
            if (sub.Contains("データ") || sub.Contains("DB")) return "#F0FFF4";
            if (sub.Contains("PG")) return "#FAF5FF";
            return "#FFFFFF";
        }
    }
}