using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using System.Drawing;
using ZXing.Windows.Compatibility;

namespace Student_Attendance_System.Views
{
    public partial class ScanPage : Page
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private DispatcherTimer timer;

        public ScanPage()
        {
            InitializeComponent();
            SetupCamera();
            UpdateStatistics(); // အောက်ခြေက Card ဂဏန်းလေးတွေ Update လုပ်ရန်
        }

        private void SetupCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
            }
            else
            {
                MessageBox.Show("No camera found.");
            }
        }

        // Camera ကလာတဲ့ Frame တွေကို Image control မှာ ပြပေးခြင်း
        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                Dispatcher.Invoke(() =>
                {
                    CameraPreview.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                });

                // QR Code ကို စစ်ဆေးခြင်း
                BarcodeReader reader = new BarcodeReader();
                var result = reader.Decode(bitmap);
                if (result != null)
                {
                    Dispatcher.Invoke(() => HandleScanSuccess(result.Text));
                }
            }
            catch { }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (videoSource != null && !videoSource.IsRunning)
            {
                videoSource.Start();
                ((Button)sender).Content = "Stop Scanning";
            }
            else if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.Stop();
                ((Button)sender).Content = "Start Scanning";
            }
        }

        // Scan အောင်မြင်သွားတဲ့အခါ လုပ်ဆောင်မည့် Logic
        private void HandleScanSuccess(string scannedText)
        {
            videoSource.Stop();
            MessageBox.Show($"出席を確認しました: {scannedText}", "Success");

            // Dashboard ကို အလိုလိုပြန်သွားစေခြင်း (Scan တန်းလန်းကြီး မကျန်ခဲ့အောင်)
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void UpdateStatistics()
        {
            // ဒီနေရာမှာ ကျောင်းသားစာရင်းနဲ့ Attendance status အလိုက် ဂဏန်းတွေကို Update လုပ်ပေးပါ
            // ဥပမာ- CheckedInCount.Text = App.AttendanceToday.Count.ToString();
        }

        // Page ကနေ ထွက်သွားရင် Camera ကို ပိတ်ပေးခြင်း
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.Stop();
            }
        }
    }
}