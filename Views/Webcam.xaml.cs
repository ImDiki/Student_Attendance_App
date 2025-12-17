using System;
using System.Drawing; // AForge & QR
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging; // WPF Image
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Windows.Compatibility; // For .NET 8 fix

namespace Student_Attendance_System.Views // Views Folder ထဲမို့လို့
{
    public partial class WebcamWindow : Window
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        public WebcamWindow()
        {
            InitializeComponent();
            LoadCameras();
        }

        // 1. ကင်မရာ List ရှာခြင်း
        void LoadCameras()
        {
            try
            {
                filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo filterInfo in filterInfoCollection)
                {
                    cboCamera.Items.Add(filterInfo.Name);
                }
                if (cboCamera.Items.Count > 0)
                    cboCamera.SelectedIndex = 0;
                else
                    MessageBox.Show("No Camera Found!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cameras: " + ex.Message);
            }
        }

        // 2. Start Button
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (filterInfoCollection == null || filterInfoCollection.Count == 0) return;

            StopCamera(); // အရင်ဟာရှိရင် ပိတ်မယ်

            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        // 3. Stop Button
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopCamera();
            imgWebcam.Source = null;
            txtQRCode.Text = "Camera Stopped";
        }

        // 4. Frame ဝင်တိုင်း အလုပ်လုပ်မည့် Function
        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

                // --- QR Code Reading ---
                var reader = new BarcodeReader(); // Using Compatibility package
                reader.Options.TryHarder = true;
                var result = reader.Decode(bitmap);
                // -----------------------

                BitmapImage bitmapImage = BitmapToImageSource(bitmap);
                bitmapImage.Freeze();

                Dispatcher.Invoke(() =>
                {
                    imgWebcam.Source = bitmapImage;

                    if (result != null)
                    {
                        // QR ဖတ်မိလျှင်
                        txtQRCode.Text = "QR: " + result.Text;

                        // TODO: Database Check Logic ကို ဒီနေရာမှာ ခေါ်ရမယ်
                        // CheckAttendance(result.Text);
                    }
                    else
                    {
                        txtQRCode.Text = "Scanning...";
                    }
                });
            }
            catch (Exception) { }
        }

        // Helper: Bitmap to WPF Image
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

        // Stop Camera Function
        private void StopCamera()
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.WaitForStop();
                videoCaptureDevice = null;
            }
        }

        // Window ပိတ်ရင် ကင်မရာပါ ပိတ်မယ်
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            StopCamera();
        }
    }
}
