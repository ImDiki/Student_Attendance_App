using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using Student_Attendance_System.Models; // LanguageSettings သုံးရန်
using Student_Attendance_System.Interfaces; // ILanguageSwitchable သုံးရန်

namespace Student_Attendance_System.Views
{
    public partial class Webcam : Window, ILanguageSwitchable
    {
        // Properties for RegisterPage
        public byte[] CapturedImageBytes { get; private set; }
        public BitmapSource CapturedImageSource { get; private set; }

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap lastCapturedFrame;

        public Webcam()
        {
            InitializeComponent();

            // ၁။ Language Setting အလိုက် UI ပြောင်းခြင်း
            ChangeLanguage(LanguageSettings.Language);

            // ၂။ Camera ကို အလိုအလျောက် စတင်ခြင်း
            this.Loaded += (s, e) => StartCameraAutomatically();
        }

        public void ChangeLanguage(bool isJapanese)
        {
            this.Title = isJapanese ? "カメラスキャン" : "Camera Scan";
            btnCapture.Content = isJapanese ? "📷 撮影する" : "📷 Take Photo";
            btnRetake.Content = isJapanese ? "🔄 撮り直す" : "🔄 Retake";
            btnConfirm.Content = isJapanese ? "✅ この写真を使う" : "✅ Use This Photo";
        }

        private void StartCameraAutomatically()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count > 0)
                {
                    // USB Camera ကို ဦးစားပေးရှာဖွေခြင်း
                    int index = 0;
                    for (int i = 0; i < videoDevices.Count; i++)
                    {
                        if (videoDevices[i].Name.ToUpper().Contains("USB")) { index = i; break; }
                    }

                    videoSource = new VideoCaptureDevice(videoDevices[index].MonikerString);
                    videoSource.NewFrame += (s, e) => {
                        Bitmap frame = (Bitmap)e.Frame.Clone();
                        lastCapturedFrame = (Bitmap)e.Frame.Clone();
                        Dispatcher.Invoke(() => {
                            imgWebcam.Source = ConvertBitmapToBitmapSource(frame);
                        });
                    };
                    videoSource.Start();
                }
                else
                {
                    string msg = LanguageSettings.Language ? "カメラが見つかりません" : "No camera detected.";
                    MessageBox.Show(msg);
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop(); // Preview ကို Freeze လုပ်လိုက်ခြင်း
                btnCapture.Visibility = Visibility.Collapsed;
                btnRetake.Visibility = Visibility.Visible;
                btnConfirm.Visibility = Visibility.Visible;
            }
        }

        private void btnRetake_Click(object sender, RoutedEventArgs e)
        {
            btnCapture.Visibility = Visibility.Visible;
            btnRetake.Visibility = Visibility.Collapsed;
            btnConfirm.Visibility = Visibility.Collapsed;
            if (videoSource != null) videoSource.Start(); // ကင်မရာပြန်ပွင့်လာမည်
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (lastCapturedFrame != null)
            {
                CapturedImageSource = ConvertBitmapToBitmapSource(lastCapturedFrame);
                using (MemoryStream ms = new MemoryStream())
                {
                    lastCapturedFrame.Save(ms, ImageFormat.Jpeg);
                    CapturedImageBytes = ms.ToArray();
                }
                StopCamera();
                this.DialogResult = true;
                this.Close();
            }
        }

        private void StopCamera()
        {
            if (videoSource != null)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var bitmapSource = BitmapSource.Create(bitmapData.Width, bitmapData.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution, System.Windows.Media.PixelFormats.Bgr24, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => StopCamera();
    }
}