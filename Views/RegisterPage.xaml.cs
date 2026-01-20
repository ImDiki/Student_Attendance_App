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
            // Enrollment Date ကို ယနေ့ရက်စွဲ အလိုအလျောက် သတ်မှတ်ပေးခြင်း
            dpEnrollDate.SelectedDate = DateTime.Today;

            // App-wide Language settings နှင့် ချိတ်ဆက်ခြင်း
            ChangeLanguage(LanguageSettings.Language);
        }

        // --- ILanguageSwitchable Implementation: ENG/JP ပြောင်းလဲခြင်း ---
        public void ChangeLanguage(bool isJapanese)
        {
            // Title & Labels
            txtRegTitle.Text = isJapanese ? "新規学生登録" : "NEW STUDENT REGISTRATION";
           
            lblStuId.Text = isJapanese ? "学籍番号" : "Student ID";
            lblYear.Text = isJapanese ? "学年" : "Year (Grade)";
            lblClass.Text = isJapanese ? "クラス" : "Class";
            lblName.Text = isJapanese ? "氏名" : "Full Name";
            lblDept.Text = isJapanese ? "専攻 / 学科" : "Department / Major";
            lblBirth.Text = isJapanese ? "生年月日" : "Birth Date";
            lblEnroll.Text = isJapanese ? "入学日" : "Enrollment Date";
            lblPass.Text = isJapanese ? "パスワード" : "System Password";

            // Buttons
            btnSave.Content = isJapanese ? "登録する" : "Save Student";
            btnCancel.Content = isJapanese ? "キャンセル" : "Cancel";
            btnCaptureText.Content = isJapanese ? "顔写真を撮影" : "Capture Photo";

            // Major List Language Sync (Duration များ အလိုအလျောက်ပြောင်းရန်)
            UpdateMajorList(isJapanese);
        }

        private void UpdateMajorList(bool isJapanese)
        {
            // ComboBox ထဲက Major များကို ဘရိုပေးထားသော စာရင်းအတိုင်း Language ညှိပေးခြင်း
            int selectedIndex = cboDepartment.SelectedIndex != -1 ? cboDepartment.SelectedIndex : 0;
            cboDepartment.Items.Clear();

            string y4 = isJapanese ? "4年" : "4 Years";
            string y3 = isJapanese ? "3年" : "3 Years";
            string y2 = isJapanese ? "2年" : "2 Years";

            cboDepartment.Items.Add(new ComboBoxItem { Content = $"ITスペシャリスト専攻 (IT Specialist) - {y4}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"ネットワークセキュリティ専攻 (Network Security) - {y4}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"システムエンジニア専攻 (Systems Engineer) - {y3}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"ネットワークエンジニア専攻 (Network Engineer) - {y3}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"Webエンジニア専攻 (Web Engineer) - {y3}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"テクニカルコース (Technical Course) - {y2}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"ネットワークシステムコース (Network Systems) - {y2}" });

            cboDepartment.SelectedIndex = selectedIndex;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Front-end Validation (အခြေခံစစ်ဆေးချက်)
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtStudentID.Text))
            {
                string msg = LanguageSettings.Language ? "必要事項を入力してください" : "Please fill required fields.";
                MessageBox.Show(msg, "Validation");
                return;
            }

            // Create Mock User for Redirect (Database မပါဘဲ Dashboard သို့ တိုက်ရိုက်ပို့ရန်)
            var mockUser = new User
            {
                FullName = txtName.Text.Trim(),
                Username = txtStudentID.Text.Trim(),
                Role = "Student"
            };

            // MainWindow ရှိ HandleLoginSuccess ကို လှမ်းခေါ်ခြင်း
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.HandleLoginSuccess(mockUser);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            // Camera Mock Logic
            string msg = LanguageSettings.Language ? "カメラを起動しています..." : "Starting Camera for Mock Capture...";
            MessageBox.Show(msg, "Device");
        }
    }
}