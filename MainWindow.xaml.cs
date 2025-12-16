using System;
using System.Drawing; // AForge နဲ့ QR အတွက်
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging; // WPF ပုံအတွက်
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
// အောက်ပါ Line က အရေးကြီးပါတယ် (Error ဖြေရှင်းဖို့)
using ZXing.Windows.Compatibility;

namespace Student_Attendance_System // Bro Project Name နဲ့ တူရမယ်
{
    public partial class MainWindow : Window
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        public MainWindow()
        {
            InitializeComponent();
            LoadCameras();
        }

        // 1. ကင်မရာများကို ရှာဖွေခြင်း
        void LoadCameras()
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
            {
                cboCamera.Items.Add(filterInfo.Name);
            }
            if (cboCamera.Items.Count > 0)
            {
                cboCamera.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No Camera Found!");
            }
        }

        // 2. Start Button
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (filterInfoCollection.Count == 0) return;
            StopCamera();
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        // 3. Stop Button
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopCamera();
            imgWebcam.Source = null;
        }

        // 4. ကင်မရာ Frame ဝင်တိုင်း QR စစ်မည့်နေရာ (Error ပြင်ထားသည်)
        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

                // --- FIX: Specific BarcodeReader for Windows ---
                // ဒီနေရာမှာ ကုဒ်ပြောင်းထားပါတယ်
                var reader = new ZXing.Windows.Compatibility.BarcodeReader();
                reader.Options.TryHarder = true; // QR ဖတ်ရခက်ရင် ပိုကြိုးစားဖို့
                var result = reader.Decode(bitmap);
                // ------------------------------------------------

                BitmapImage bitmapImage = BitmapToImageSource(bitmap);
                bitmapImage.Freeze();

                Dispatcher.Invoke(() =>
                {
                    imgWebcam.Source = bitmapImage;

                    if (result != null)
                    {
                        // QR တွေ့ပြီ
                        txtQRCode.Text = "QR Found: " + result.Text;
                        // TODO: Database code here
                    }
                    else
                    {
                        txtQRCode.Text = "Scanning...";
                    }
                });
            }
            catch (Exception ex)
            {
                // Error တက်ရင် ကျော်မယ်
            }
        }

        // Helper: Bitmap to BitmapImage
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
                return bitmapimage;
            }
        }

        private void StopCamera()
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.WaitForStop();
                videoCaptureDevice = null;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            StopCamera();
            Application.Current.Shutdown();
        }
    }
}