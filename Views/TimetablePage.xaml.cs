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
            // Initial load အနေနဲ့ နောက်ဆုံးတက်နေတဲ့ 後期 (Latter Term) ကို ပြမယ်
            LoadLatterTerm();
        }

        private void cboTerm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgFullTimetable == null) return;

            if (cboTerm.SelectedIndex == 1) LoadLatterTerm();
            else LoadFormerTerm();
        }

        private void LoadLatterTerm()
        {
            // image_57b450.jpg ထဲက 2025年度 1Cクラス 後期時間割
            var list = new List<TimetableRow>();

            // CreateRow(အချိန်စာသား, တနင်္လာ, အင်္ဂါ, ဗုဒ္ဓဟူး, ကြာသပတေး, သောကြာ, အချိန်ပိုင်းနံပါတ်, စမည့်နာရီ, မိနစ်, ပြီးမည့်နာရီ, မိနစ်)
            list.Add(CreateRow("09:10-10:40\n(1限目)", "テスト技法", "ビジネスアプリ II", "PG実践 I", "プログラミング II", "ゼミナール I", 1, 9, 10, 10, 40));
            list.Add(CreateRow("10:50-12:20\n(2限目)", "テスト技法", "システム開発基礎", "PG実践 I", "プログラミング II", "データベース技術", 2, 10, 50, 12, 20));
            list.Add(CreateRow("13:10-14:40\n(3限目)", "データ構造 II", "-", "コミ技", "キャリアデザイン", "データベース技術", 3, 13, 10, 14, 40));
            list.Add(CreateRow("14:50-16:20\n(4限目)", "資格対策/日本 IV", "-", "-", "資格対策/日本 II", "就職特別指導", 4, 14, 50, 16, 20));

            dgFullTimetable.ItemsSource = list;
        }

        private void LoadFormerTerm()
        {
            // image_57b450.jpg ထဲက 2025年度 1Cクラス 前期時間割
            var list = new List<TimetableRow>();
            list.Add(CreateRow("09:10-10:40\n(1限目)", "ネット＆セキュ", "ビジネスアプリ I", "コンシステム", "マネジメント", "データ構造 I", 1, 9, 10, 10, 40));
            list.Add(CreateRow("10:50-12:20\n(2限目)", "ネット＆セキュ", "プログラミング I", "コンシステム", "日本表現法", "マネジメント", 2, 10, 50, 12, 20));
            dgFullTimetable.ItemsSource = list;
        }

        private TimetableRow CreateRow(string time, string mon, string tue, string wed, string thu, string fri, int p, int h1, int m1, int h2, int m2)
        {
            var row = new TimetableRow { Time = time, Monday = mon, Tuesday = tue, Wednesday = wed, Thursday = thu, Friday = fri };

            // တစ်နေ့ချင်းစီအတွက် လက်ရှိအချိန်နဲ့ တိုက်စစ်ပြီး အရောင်သတ်မှတ်မယ်
            row.MonColor = GetStatusColor(mon, "Monday", p, h1, m1, h2, m2);
            row.IsMonEnabled = CanStart(row.MonColor);

            row.TueColor = GetStatusColor(tue, "Tuesday", p, h1, m1, h2, m2);
            row.IsTueEnabled = CanStart(row.TueColor);

            row.WedColor = GetStatusColor(wed, "Wednesday", p, h1, m1, h2, m2);
            row.IsWedEnabled = CanStart(row.WedColor);

            row.ThuColor = GetStatusColor(thu, "Thursday", p, h1, m1, h2, m2);
            row.IsThuEnabled = CanStart(row.ThuColor);

            row.FriColor = GetStatusColor(fri, "Friday", p, h1, m1, h2, m2);
            row.IsFriEnabled = CanStart(row.FriColor);

            return row;
        }
        private Brush GetStatusColor(string subject, string day, int p, int h1, int m1, int h2, int m2)
        {
            if (subject == "-" || string.IsNullOrEmpty(subject)) return Brushes.Transparent;

            DateTime now = DateTime.Now;
            string currentDay = now.DayOfWeek.ToString(); // Monday, Tuesday...

            // ၁။ လက်ရှိနေ့ (ဥပမာ- 木曜日) ဖြစ်ပါက Column တစ်ခုလုံးကို Highlight မှိန်မှိန်လေးပြမယ်
            if (currentDay == day)
            {
                // လက်ရှိတက်နေဆဲ အချိန်ပိုင်းဖြစ်ပါက ပိုလင်းသောအရောင်ပြမယ်
                DateTime start = new DateTime(now.Year, now.Month, now.Day, h1, m1, 0);
                DateTime end = new DateTime(now.Year, now.Month, now.Day, h2, m2, 0);

                if (now >= start && now <= end)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE0B2")); // လက်ရှိအတန်း (လိမ္မော်နုရောင်)

                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")); // ယနေ့ Column (မီးခိုးနုရောင်မှိန်မှိန်)
            }

            // ၂။ ကျန်တဲ့နေ့တွေနဲ့ ပြီးသွားတဲ့အတန်းတွေအားလုံးကို အဖြူရောင်ပဲထားမယ်
            return Brushes.White;
        }

        private bool CanStart(Brush color)
        {
            // အညိုရောင် (ပြီးသွားပြီ) ဆိုရင် ထပ်နှိပ်လို့မရတော့ဘူး
            if (color.ToString() == "#FF8D6E63") return false;
            // ဆရာမဖြစ်မှ နှိပ်ခွင့်ရှိမယ်
            return UserData.CurrentUser?.Role == "Teacher";
        }

        private void Subject_Click(object sender, RoutedEventArgs e)
        {
            if (UserData.CurrentUser?.Role != "Teacher") return;

            Button btn = sender as Button;
            string subject = btn.Content.ToString();

            if (MessageBox.Show($"{subject} の授業を開始しますか？", "授業開始", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.IsClassActive = true;
                App.CurrentActiveSessionStart = DateTime.Now;
                App.CurrentSubject = subject;
                // နဂို logic အတိုင်း ScanPage ကို သွားမယ်
                this.NavigationService.Navigate(new ScanPage());
            }
        }
    }

    public class TimetableRow
    {
        public string Time { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public bool IsMonEnabled { get; set; }
        public bool IsTueEnabled { get; set; }
        public bool IsWedEnabled { get; set; }
        public bool IsThuEnabled { get; set; }
        public bool IsFriEnabled { get; set; }
        public Brush MonColor { get; set; }
        public Brush TueColor { get; set; }
        public Brush WedColor { get; set; }
        public Brush ThuColor { get; set; }
        public Brush FriColor { get; set; }
    }
}