using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading; // Timer အတွက်လိုအပ်
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class ScanPage : Page, ILanguageSwitchable
    {
        private bool _isJapanese = false;
        private DispatcherTimer redirectTimer; // Redirect လုပ်ဖို့ Timer

        private List<StudentMock> mockStudentDb = new List<StudentMock>()
        {
            new StudentMock { BarcodeID = "C5292", Name = "MYAT THADAR LINN", StudentID = "C5292" },
            new StudentMock { BarcodeID = "123456789", Name = "DIKI", StudentID = "D001" }
        };

        public ScanPage()
        {
            InitializeComponent();
            ChangeLanguage(LanguageSettings.Language);
            this.Loaded += (s, e) => txtScannerInput.Focus();
        }

        public void ChangeLanguage(bool isJapanese)
        {
            _isJapanese = isJapanese;
            txtTitle.Text = isJapanese ? "出席管理スキャン" : "Attendance Scanning";
            lblHeaderName.Text = isJapanese ? "氏名" : "Name";
            lblHeaderID.Text = isJapanese ? "学籍番号" : "Student ID";
            txtBadgeStatus.Text = isJapanese ? "出席 ✅ PRESENT" : "PRESENT ✅ ATTENDED";

            if (lblScannedName.Text == "Waiting..." || lblScannedName.Text == "待機中...")
                lblScannedName.Text = isJapanese ? "待機中..." : "Waiting...";
        }

        private void txtScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string id = txtScannerInput.Text.Trim().ToUpper();
                ProcessScan(id);
                txtScannerInput.Clear();
            }
        }

        private void ProcessScan(string id)
        {
            var student = mockStudentDb.FirstOrDefault(s => s.BarcodeID.ToUpper() == id);

            if (student != null)
            {
                lblScannedName.Text = student.Name;
                lblScannedID.Text = student.StudentID;
                AttendanceBadge.Visibility = Visibility.Visible;
                lblStatusMessage.Text = _isJapanese ? "成功しました。間もなく戻ります..." : "Success. Redirecting soon...";
                lblStatusMessage.Foreground = Brushes.LightGreen;
                System.Media.SystemSounds.Beep.Play();

                StartRedirectTimer(); // Card ဖတ်ပြီးတာနဲ့ Timer စမယ်
            }
            else
            {
                lblScannedName.Text = _isJapanese ? "未登録" : "Not Found";
                lblStatusMessage.Text = _isJapanese ? "無効なカードです" : "Invalid Card ID";
                lblStatusMessage.Foreground = Brushes.Salmon;
            }
        }

        // --- ၃ စက္ကန့်အကြာတွင် LoginPage ဆီသို့ အလိုအလျောက်ပြန်သွားရန် ---
        private void StartRedirectTimer()
        {
            if (redirectTimer != null) redirectTimer.Stop();

            redirectTimer = new DispatcherTimer();
            redirectTimer.Interval = TimeSpan.FromSeconds(3); // ၃ စက္ကန့် စောင့်မယ်
            redirectTimer.Tick += (s, e) =>
            {
                redirectTimer.Stop();
                this.NavigationService.Navigate(new LoginPage());
            };
            redirectTimer.Start();
        }

        private void Page_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtScannerInput.Focus();
        }
    }

    public class StudentMock
    {
        public string BarcodeID { get; set; }
        public string Name { get; set; }
        public string StudentID { get; set; }
    }
}