using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System.Models;
using Student_Attendance_System.Interfaces;

namespace Student_Attendance_System.Views
{
    public partial class RegisterPage : Page, ILanguageSwitchable
    {
        public RegisterPage()
        {
            InitializeComponent();
            // Enrollment Date ကို ယနေ့ရက်စွဲ (Jan 20, 2026) သတ်မှတ်ခြင်း
            dpEnrollDate.SelectedDate = DateTime.Today;

            // ဘရိုရဲ့ Language Setting အလိုက် UI ကို ပြင်ဆင်ခြင်း
            ChangeLanguage(LanguageSettings.Language);
        }

        public void ChangeLanguage(bool isJapanese)
        {
            // Labels and Titles
            txtRegTitle.Text = isJapanese ? "新規学生登録" : "NEW STUDENT REGISTRATION";
            lblStuId.Text = isJapanese ? "学籍番号" : "Student ID";
            lblYear.Text = isJapanese ? "学年" : "Year (Grade)";
            lblClass.Text = isJapanese ? "クラス" : "Class";
            lblName.Text = isJapanese ? "氏名" : "Full Name";
            lblDept.Text = isJapanese ? "専攻 / 学科" : "Department / Major";
            lblPass.Text = isJapanese ? "パスワード" : "System Password";

            // Buttons
            btnSave.Content = isJapanese ? "登録する" : "Save Student";
            btnCancel.Content = isJapanese ? "キャンセル" : "Cancel";
            btnCaptureText.Content = isJapanese ? "写真撮影" : "Capture Photo";

            // Choice တွေ ပြန်ပေါ်လာစေရန် Major List ကို Update လုပ်ခြင်း
            UpdateMajorList(isJapanese);
        }

        private void UpdateMajorList(bool isJapanese)
        {
            // လက်ရှိ ရွေးထားတဲ့ Index ကို မှတ်ထားပါ (Choice ပျောက်မသွားစေရန်)
            int savedIndex = cboDepartment.SelectedIndex;
            cboDepartment.Items.Clear();

            string y4 = isJapanese ? "4年" : "4 Years";
            string y3 = isJapanese ? "3年" : "3 Years";
            string y2 = isJapanese ? "2年" : "2 Years";

            // ComboBox ထဲသို့ Choice များ ပြန်ထည့်ခြင်း
            string[] majorNames = {
                "ITスペシャリスト専攻 (IT Specialist)",
                "ネットワークセキュリティ専攻 (Network Security)",
                "システムエンジニア専攻 (Systems Engineer)",
                "ネットワークエンジニア専攻 (Network Engineer)",
                "Webエンジニア専攻 (Web Engineer)",
                "テクニカルコース (Technical Course)",
                "ネットワークシステムコース (Network Systems)"
            };

            string[] durations = { y4, y4, y3, y3, y3, y2, y2 };

            for (int i = 0; i < majorNames.Length; i++)
            {
                // UI မှာ Choice တွေကို အဖြူရောင်စာသားနဲ့ မြင်ရအောင်လုပ်ခြင်း
                cboDepartment.Items.Add(new ComboBoxItem
                {
                    Content = $"{majorNames[i]} - {durations[i]}",
                    Foreground = System.Windows.Media.Brushes.White
                });
            }

            // အရင်ရွေးထားတာရှိရင် ပြန်သတ်မှတ်ပေးပါ
            cboDepartment.SelectedIndex = savedIndex != -1 ? savedIndex : 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation စစ်ဆေးခြင်း
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtStudentID.Text))
            {
                MessageBox.Show(LanguageSettings.Language ? "入力してください" : "Please fill all fields.");
                return;
            }

            // Register လုပ်ပြီးသည်နှင့် Student Dashboard သို့ ပို့ရန် Mock User တည်ဆောက်ခြင်း
            var mockUser = new User
            {
                FullName = txtName.Text.Trim(),
                Username = txtStudentID.Text.Trim(),
                Role = "Student"
            };

            // Dashboard သို့ Redirect လုပ်ခြင်း
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.HandleLoginSuccess(mockUser);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
        private void btnCapture_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Camera Starting...");
    }
}