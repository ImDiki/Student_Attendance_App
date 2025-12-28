using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
// အောက်ပါ namespace အသစ်ကို ထည့်ဖို့ Package သွင်းထားရမယ်
using ZXing.Windows.Compatibility;

namespace Student_Attendance_System.Views
{
    public partial class ScanPage : Page
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        public ScanPage()
        {
            InitializeComponent();
            Loaded += ScanPage_Loaded;
            Unloaded += Page_Unloaded;
        }

        private void ScanPage_Loaded(object sender, RoutedEventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (filterInfoCollection == null || filterInfoCollection.Count == 0)
            {
                MessageBox.Show("No Camera Found!", "Error");
                return;
            }

            // Start ခလုတ်ကို ဖျောက်မယ်
            pnlStart.Visibility = Visibility.Collapsed;

            // --- CAMERA SELECTION LOGIC (အသစ်) ---
           
            string selectedCameraMoniker = "";

            foreach (FilterInfo device in filterInfoCollection)
            {
                // "USB" သို့မဟုတ် "WebCam" ပါတဲ့ကောင်ကို ဦးစားပေးရှာမယ်
                if (device.Name.Contains("USB") || device.Name.Contains("WebCam") || device.Name.Contains("Integrated"))
                {
                    selectedCameraMoniker = device.MonikerString;
                    break; // တွေ့ရင် Loop ရပ်လိုက်မယ်
                }
            }

            // ရှာလို့မတွေ့ရင် ပထမဆုံးတစ်ခုကိုပဲ ယူမယ် (Fallback)
            if (string.IsNullOrEmpty(selectedCameraMoniker))
            {
                selectedCameraMoniker = filterInfoCollection[0].MonikerString;
            }

            // Camera ဖွင့်မယ်
            videoCaptureDevice = new VideoCaptureDevice(selectedCameraMoniker);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();

            lblStatus.Text = "Scanning... Show your ID Card";
        }
        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                BitmapImage bitmapImage = BitmapToImageSource(bitmap);

                Dispatcher.Invoke(() => imgWebcam.Source = bitmapImage);

                // BarcodeReader (from ZXing.Windows.Compatibility)
                BarcodeReader reader = new BarcodeReader();
                reader.Options.TryHarder = true;
                var result = reader.Decode(bitmap);

                if (result != null)
                {
                    string decodedText = result.Text;
                    videoCaptureDevice.SignalToStop();
                    Dispatcher.Invoke(() => ProcessAttendance(decodedText));
                }
            }
            catch { }
        }

        // (ProcessAttendance, ShowResultUI, RestartCameraAfterDelay, BitmapToImageSource Code 


        private void ProcessAttendance(string studentID)
        {
            // ၁။ အတန်းချိန် ရှိမရှိ စစ်မယ်
            if (!App.IsClassActive)
            {
                ShowResultUI("No Active Class", "Please wait for teacher.", true);
                RestartCameraAfterDelay();
                return;
            }

            // ၂။ အချိန်ကွာခြားချက်ကို တွက်မယ်
            TimeSpan diff = DateTime.Now - App.CurrentActiveSessionStart;

            string status = "";
            string msg = "";
            bool isError = false;

            // =========================================================
            // NEW RULE: Strict 5 Minute Cutoff
            // =========================================================

            if (diff.TotalMinutes <= 5)
            {
                // ၅ မိနစ် အတွင်း (OK)
                status = "Present (出席)";
                msg = "Attendance Confirmed";
                isError = false; // အစိမ်းရောင်ပြမယ်
            }
            else
            {
                // ၅ မိနစ် ကျော်သွားပြီ (Absent)
                status = "Time Over (欠席)";
                msg = $"Late by {diff.Minutes} mins"; // ဘယ်နှစ်မိနစ်နောက်ကျလဲ ပြမယ်
                isError = true; // အနီရောင်ပြမယ်
            }

            // =========================================================
            // BO SANN'S CODE ZONE (Save to DB)
            // =========================================================
            // Insert into Attendance Table (Status: Present or Absent)
            // =========================================================

            // UI ပြမယ်
            ShowResultUI(status, $"{studentID}\n{msg}", isError);
            RestartCameraAfterDelay();
        }


        // Helper Functions...
        private void ShowResultUI(string status, string message, bool isError)
        {
            ResultBorder.Visibility = Visibility.Visible;
            lblResultStatus.Text = status;
            lblResultMessage.Text = message;
            // Color logic...
        }

        private void RestartCameraAfterDelay()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) => {
                ResultBorder.Visibility = Visibility.Collapsed;
                if (videoCaptureDevice != null && !videoCaptureDevice.IsRunning) videoCaptureDevice.Start();
                timer.Stop();
            };
            timer.Start();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.WaitForStop();
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmapimage.Freeze();
                return bitmapimage;
            }
        }
    }
}