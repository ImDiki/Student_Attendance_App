using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Student_Attendance_System.Views
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

        void LoadCameras()
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
            {
                cboCamera.Items.Add(filterInfo.Name);
            }
            if (cboCamera.Items.Count > 0) cboCamera.SelectedIndex = 0;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (filterInfoCollection.Count == 0) return;

            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (var memory = new MemoryStream())
                {
                    eventArgs.Frame.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    Dispatcher.Invoke(() => imgWebcam.Source = bitmapImage);
                }
            }
            catch { }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopWebcam();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopWebcam();
        }

        private void StopWebcam()
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice = null;
            }
        }
    }
}