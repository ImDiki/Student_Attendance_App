using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using AForge.Video;
using AForge.Video.DirectShow;
using Student_Attendance_System.Interfaces; // Interface သိအောင်

namespace Student_Attendance_System.Views
{
    public class StudentMock
    {
        public string BarcodeID { get; set; }
        public string Name { get; set; }
        public string StudentID { get; set; }
    }

    public partial class ScanPage : Page, ILanguageSwitchable
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private bool _isJapanese = false;

        private List<StudentMock> mockStudentDb = new List<StudentMock>()
        {
            new StudentMock { BarcodeID = "C5292", Name = "MYAT THADAR LINN", StudentID = "C5292" },
            new StudentMock { BarcodeID = "123456789", Name = "DIKI", StudentID = "D001" }
        };

        public ScanPage()
        {
            InitializeComponent();
            SetupCamera();

            // App-wide language ကို စဖွင့်တာနဲ့ စစ်မယ်
            ChangeLanguage(LanguageSettings.Language);

            this.Loaded += (s, e) => txtScannerInput.Focus();
        }

        // --- ILanguageSwitchable Implementation ---
        public void ChangeLanguage(bool isJapanese)
        {
            _isJapanese = isJapanese;
            if (isJapanese)
            {
                txtTitle.Text = "出席管理スキャン";
                lblScannerLabel.Text = "スキャナー入力: ";
                txtStudentInfo.Text = "学生情報";
                lblHeaderName.Text = "氏名 (Name)";
                lblHeaderID.Text = "学籍番号 (Student ID)";
                btnStart.Content = "システム開始";
                txtBadgeStatus.Text = "出席 ✅ PRESENT";
                if (lblScannedName.Text == "Waiting...") lblScannedName.Text = "待機中...";
            }
            else
            {
                txtTitle.Text = "Attendance Scanning";
                lblScannerLabel.Text = "Scanner Input: ";
                txtStudentInfo.Text = "Student Information";
                lblHeaderName.Text = "Name (氏名)";
                lblHeaderID.Text = "Student ID (学籍番号)";
                btnStart.Content = "START SYSTEM";
                txtBadgeStatus.Text = "出席 ✅ PRESENT";
                if (lblScannedName.Text == "待機中...") lblScannedName.Text = "Waiting...";
            }
        }

        private void txtScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string id = txtScannerInput.Text.Trim().ToUpper();
                ProcessScan(id);
                txtScannerInput.Clear();
                txtScannerInput.Focus();
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
                lblStatusMessage.Text = _isJapanese ? "成功しました！" : "Success!";
                lblStatusMessage.Foreground = System.Windows.Media.Brushes.Green;
                System.Media.SystemSounds.Beep.Play();
            }
            else
            {
                lblScannedName.Text = _isJapanese ? "見つかりません！" : "Not Found!";
                lblScannedID.Text = id;
                AttendanceBadge.Visibility = Visibility.Collapsed;
                lblStatusMessage.Text = _isJapanese ? "カードが登録されていません！" : "Card not registered!";
                lblStatusMessage.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void SetupCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += (s, e) => {
                    try
                    {
                        Bitmap bitmap = (Bitmap)e.Frame.Clone();
                        Dispatcher.Invoke(() => {
                            CameraPreview.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        });
                    }
                    catch { }
                };
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (videoSource != null && !videoSource.IsRunning) videoSource.Start();
            txtScannerInput.Focus();
        }

        private void Page_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtScannerInput.Focus();
        }
    }
}