using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class TimetablePage : Page
    {
        public TimetablePage()
        {
            InitializeComponent();
            cboTerm.SelectedIndex = App.SelectedTermIndex;
            LoadTimetable();
        }

        private void cboTerm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TimetableGrid == null) return;
            App.SelectedTermIndex = cboTerm.SelectedIndex;
            LoadTimetable();
        }

        private void LoadTimetable()
        {
            // Clear current cards (except headers)
            for (int i = TimetableGrid.Children.Count - 1; i >= 0; i--)
            {
                if (Grid.GetRow(TimetableGrid.Children[i]) > 0)
                    TimetableGrid.Children.RemoveAt(i);
            }

            // Data based on 2025年度 1Cクラス
            if (App.SelectedTermIndex == 1) // Latter Term (後期)
            {
                AddRow(1, "09:10-10:40", "テスト技法", "ビジネスアプリ II", "PG実践 I", "プログラミング II", "ゼミナール I", 10, 40);
                AddRow(2, "10:50-12:20", "テスト技法", "システム開発基礎", "PG実践 I", "プログラミング II", "データベース技術", 12, 20);
                AddRow(3, "13:10-14:40", "データ構造 II", "-", "コミ技", "キャリアデザイン", "データベース技術", 14, 40);
                AddRow(4, "14:50-16:20", "資格対策/日本 IV", "-", "-", "資格対策/日本 II", "就職特別指導", 16, 20);
            }
            else // Former Term (前期)
            {
                AddRow(1, "09:10-10:40", "ネット＆セキュ", "ビジネスアプリ I", "コンシステム", "マネジメント", "データ構造 I", 10, 40);
                AddRow(2, "10:50-12:20", "ネット＆セキュ", "プログラミング I", "コンシステム", "日本表現法", "マネジメント", 12, 20);
            }
        }

        private void AddRow(int row, string time, string mon, string tue, string wed, string thu, string fri, int endH, int endM)
        {
            // Time Label
            var lbl = new TextBlock { Text = $"{row}限\n{time}", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 12, Foreground = Brushes.Gray };
            Grid.SetRow(lbl, row); Grid.SetColumn(lbl, 0);
            TimetableGrid.Children.Add(lbl);

            // Add Cards for each day
            AddCard(row, 1, mon, "Monday", endH, endM);
            AddCard(row, 2, tue, "Tuesday", endH, endM);
            AddCard(row, 3, wed, "Wednesday", endH, endM);
            AddCard(row, 4, thu, "Thursday", endH, endM);
            AddCard(row, 5, fri, "Friday", endH, endM);
        }

        private void AddCard(int row, int col, string subject, string day, int endH, int endM)
        {
            if (subject == "-" || string.IsNullOrEmpty(subject))
            {
                // အတန်းမရှိတဲ့နေရာမှာ အဖြူကွက်လပ်လေးတွေ ဖြစ်စေရန်
                var blank = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5) };
                Grid.SetRow(blank, row); Grid.SetColumn(blank, col);
                TimetableGrid.Children.Add(blank);
                return;
            }

            // ဘာသာရပ်အလိုက် အရောင်သတ်မှတ်ခြင်း
            Brush cardColor = Brushes.White; // Default
            if (subject.Contains("プログラミング")) cardColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EBF8FF")); // Light Blue
            else if (subject.Contains("テスト")) cardColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5")); // Light Red
            else if (subject.Contains("DB")) cardColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0FFF4")); // Light Green
            else if (subject.Contains("PG")) cardColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FAF5FF")); // Light Purple

            var btn = new Button
            {
                Content = subject,
                Style = (Style)Application.Current.Resources["TimetableCardStyle"],
                Background = cardColor, // ကတ်တစ်ခုလုံးကို အရောင်ခြယ်ပါသည်
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            btn.Click += Subject_Click;

            Grid.SetRow(btn, row); Grid.SetColumn(btn, col);
            TimetableGrid.Children.Add(btn);
        }

        private void Subject_Click(object sender, RoutedEventArgs e)
        {
            if (UserData.CurrentUser?.Role != "Teacher") return;

            var btn = sender as Button;
            if (btn.Background.ToString() == "#FF8D6E63") return; // ပြီးသွားလျှင် နှိပ်မရ

            if (MessageBox.Show($"{btn.Content} を開始しますか？", "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.IsClassActive = true;
                App.CurrentSubject = btn.Content.ToString();
                App.StartedPeriods.Add(btn.Tag.ToString()); // Mark as started
                this.NavigationService.Navigate(new ScanPage()); // Dashboard navigation fix included
            }
        }
    }
}