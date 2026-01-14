using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; // <--- ဒါရှိမှ Brushes error ပျောက်မှာ
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Student_Attendance_System.Views
{
    public class StudentMock
    {
        public string BarcodeID { get; set; }
        public string Name { get; set; }
        public string StudentID { get; set; }
    }

    public partial class ScanPage : Page
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        private List<StudentMock> mockStudentDb = new List<StudentMock>()
        {
            new StudentMock { BarcodeID = "C5292", Name = "MYAT THADAR LINN", StudentID = "C5292" },
            new StudentMock { BarcodeID = "123456789", Name = "DIKI", StudentID = "D001" }
        };

        public ScanPage()
        {
            InitializeComponent();
            SetupCamera();
            this.Loaded += (s, e) => txtScannerInput.Focus(); // Page Load တာနဲ့ Focus ပေးမယ်
        }

        private void txtScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Scanner က Enter ခေါက်လိုက်တဲ့အခါ
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
                lblStatusMessage.Text = "Success!";
                lblStatusMessage.Foreground =System.Windows.Media. Brushes.Green; // Namespace ပါမှ အလုပ်လုပ်မယ်
                System.Media.SystemSounds.Beep.Play();
            }
            else
            {
                lblScannedName.Text = "Not Found!";
                lblScannedID.Text = id;
                AttendanceBadge.Visibility = Visibility.Collapsed;
                lblStatusMessage.Text = "Card not registered!";
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
            txtScannerInput.Focus(); // ခလုတ်နှိပ်ပြီးတိုင်း Focus ပြန်ယူမယ်
        }

        private void Page_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtScannerInput.Focus(); // တခြားနေရာနှိပ်မိရင် Focus ပြန်ယူမယ်
        }
    }
}