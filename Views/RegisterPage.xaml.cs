using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System.Models;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Services;

namespace Student_Attendance_System.Views
{
    public partial class RegisterPage : Page, ILanguageSwitchable
    {
        private byte[] capturedPhotoBytes = null;

        public RegisterPage()
        {
            InitializeComponent();
            // Enrollment Date ကို ယနေ့ရက်စွဲ သတ်မှတ်ခြင်း
            dpEnrollDate.SelectedDate = DateTime.Today;
            ChangeLanguage(LanguageSettings.Language);
        }

        public void ChangeLanguage(bool isJapanese)
        {
            txtRegTitle.Text = isJapanese ? "新規学生登録" : "NEW STUDENT REGISTRATION";
            lblStuId.Text = isJapanese ? "学籍番号" : "Student ID";
            lblYear.Text = isJapanese ? "学年" : "Year (Grade)";
            lblClass.Text = isJapanese ? "クラス" : "Class";
            lblName.Text = isJapanese ? "氏名" : "Full Name";
            lblDept.Text = isJapanese ? "専攻 / 学科" : "Department / Major";
            lblPass.Text = isJapanese ? "パスワード" : "System Password";

            btnSave.Content = isJapanese ? "登録する" : "Save Student";
            btnCancel.Content = isJapanese ? "キャンセル" : "Cancel";
            btnCaptureText.Content = isJapanese ? "写真撮影" : "Capture Photo";

            UpdateMajorList(isJapanese);
        }

        private void UpdateMajorList(bool isJapanese)
        {
            int savedIndex = cboDepartment.SelectedIndex;
            cboDepartment.Items.Clear();

            string y4 = isJapanese ? "4年" : "4 Years";
            string y3 = isJapanese ? "3年" : "3 Years";
            string y2 = isJapanese ? "2年" : "2 Years";

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
                cboDepartment.Items.Add(new ComboBoxItem
                {
                    Content = $"{majorNames[i]} - {durations[i]}",
                    Foreground = System.Windows.Media.Brushes.White
                });
            }
            cboDepartment.SelectedIndex = savedIndex != -1 ? savedIndex : 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // ၁။ Validation စစ်ဆေးခြင်း
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtStudentID.Text))
            {
                MessageBox.Show(LanguageSettings.Language ? "入力してください" : "Please fill all fields.");
                return;
            }

            // ၂။ UI မှ Data များရယူခြင်း
            string id = txtStudentID.Text.Trim();
            string name = txtName.Text.Trim();
            string pass = txtPassword.Password;
            string dept = ((ComboBoxItem)cboDepartment.SelectedItem)?.Content.ToString();
            string year = ((ComboBoxItem)cboYear.SelectedItem)?.Content.ToString();
            string className = ((ComboBoxItem)cboClass.SelectedItem)?.Content.ToString();

            AuthService auth = new AuthService();
            // ၃။ Database ထဲသို့ သိမ်းဆည်းခြင်း
            bool success = auth.RegisterStudent(id, name, pass, dept, year, className, capturedPhotoBytes);

            if (success)
            {
                
                UserData.UserData.CurrentUser = new User
                {
                    Username = id,
                    FullName = name,
                    YearLevel = year,
                    AssignedClass = className,
                    FacePhoto = capturedPhotoBytes // ရိုက်လိုက်သောပုံ
                };

                MessageBox.Show(LanguageSettings.Language ? "登録が完了しました！" : "Registration Successful!");

                // ၅။ Dashboard ကို သွားမည်
                NavigationService.Navigate(new StudentDashboardPage());
            }
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            Webcam cameraWindow = new Webcam();
            if (cameraWindow.ShowDialog() == true)
            {
                capturedPhotoBytes = cameraWindow.CapturedImageBytes;
                imgProfile.Source = cameraWindow.CapturedImageSource;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => ClearFields();

        private void btnBack_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private void ClearFields()
        {
            txtStudentID.Clear();
            txtName.Clear();
            txtPassword.Clear();
            cboDepartment.SelectedIndex = 0;
            cboYear.SelectedIndex = 0;
            cboClass.SelectedIndex = 0;
            dpEnrollDate.SelectedDate = DateTime.Today;
            imgProfile.Source = null;
            capturedPhotoBytes = null;
        }
    }
}