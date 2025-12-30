using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Navigation; // For Navigation
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Windows.Compatibility;

namespace Student_Attendance_System.Views
{
    public partial class ScanPage : Page
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        private bool _isProcessing = false;
        private string _scannerBuffer = "";
        private DateTime _lastKeystroke = DateTime.Now;

        public ScanPage()
        {
            InitializeComponent();
            Loaded += ScanPage_Loaded;
            Unloaded += Page_Unloaded;
        }

        private void ScanPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        // --- KEYBOARD INPUT ---
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isProcessing) return;
            TimeSpan elapsed = DateTime.Now - _lastKeystroke;
            if (elapsed.TotalMilliseconds > 100) _scannerBuffer = "";
            _lastKeystroke = DateTime.Now;

            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(_scannerBuffer))
                {
                    StartVerificationProcess(_scannerBuffer);
                    _scannerBuffer = "";
                }
            }
            else { _scannerBuffer += GetCharFromKey(e.Key); }
        }

        private string GetCharFromKey(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9) return ((int)key - (int)Key.D0).ToString();
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return ((int)key - (int)Key.NumPad0).ToString();
            if (key >= Key.A && key <= Key.Z) return key.ToString();
            return "";
        }

        // --- WEBCAM & BUTTONS ---
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            this.Focus();
            if (filterInfoCollection == null || filterInfoCollection.Count == 0)
            {
                MessageBox.Show("No Camera Found!", "Error");
                return;
            }
            pnlStart.Visibility = Visibility.Collapsed;

            string selectedCameraMoniker = "";
            foreach (FilterInfo device in filterInfoCollection)
            {
                if (device.Name.Contains("USB") || device.Name.Contains("WebCam"))
                {
                    selectedCameraMoniker = device.MonikerString;
                    break;
                }
            }
            if (string.IsNullOrEmpty(selectedCameraMoniker)) selectedCameraMoniker = filterInfoCollection[0].MonikerString;

            videoCaptureDevice = new VideoCaptureDevice(selectedCameraMoniker);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        // *** NEW: RETURN TO DASHBOARD ***
        private void btnReview_Click(object sender, RoutedEventArgs e)
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.WaitForStop();
            }

            // Dashboard ကို ပြန်ခေါ်မယ် (Tab 2 ဖွင့်ခိုင်းမယ်)
            TeacherDashboard dashboard = new TeacherDashboard();
            dashboard.OpenManagerTab = true;
            NavigationService.Navigate(dashboard);
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                BitmapImage bitmapImage = BitmapToImageSource(bitmap);
                Dispatcher.Invoke(() => imgWebcam.Source = bitmapImage);

                if (_isProcessing) return;

                BarcodeReader reader = new BarcodeReader();
                reader.Options.TryHarder = true;
                var result = reader.Decode(bitmap);

                if (result != null)
                {
                    string decodedText = result.Text;
                    Dispatcher.Invoke(() => StartVerificationProcess(decodedText));
                }
            }
            catch { }
        }

        private void StartVerificationProcess(string studentID)
        {
            if (_isProcessing) return;
            _isProcessing = true;

            if (!App.IsClassActive)
            {
                ShowResultUI("No Active Class", "Wait for teacher", true);
                ResetSystemAfterDelay();
                return;
            }

            ResultBorder.Visibility = Visibility.Visible;
            ResultBorder.Background = System.Windows.Media.Brushes.LightBlue;
            lblResultStatus.Text = "Card Detected";
            lblResultStatus.Foreground = System.Windows.Media.Brushes.Black;
            lblResultMessage.Text = $"ID: {studentID}\nVerifying Identity...";

            DispatcherTimer faceCheckTimer = new DispatcherTimer();
            faceCheckTimer.Interval = TimeSpan.FromSeconds(2);
            faceCheckTimer.Tick += (s, args) =>
            {
                faceCheckTimer.Stop();
                CalculateAttendance(studentID); // Mock Success
            };
            faceCheckTimer.Start();
        }

        private void CalculateAttendance(string studentID)
        {
            TimeSpan diff = DateTime.Now - App.CurrentActiveSessionStart;
            string status = "";
            string msg = "";
            bool isError = false;

            if (diff.TotalMinutes <= 5)
            {
                status = "Present (出席)";
                msg = "Identity Verified";
                isError = false;
            }
            else
            {
                status = "Absent (欠席)";
                msg = "Time Over";
                isError = true;
            }

            App.TempAttendanceList.Add(new AttendanceRecord
            {
                StudentID = studentID,
                Status = status.Contains("Present") ? "Present" : "Absent",
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            });

            ShowResultUI(status, $"{studentID}\n{msg}", isError);
            ResetSystemAfterDelay();
        }

        private void ShowResultUI(string status, string message, bool isError)
        {
            ResultBorder.Visibility = Visibility.Visible;
            lblResultStatus.Text = status;
            lblResultMessage.Text = message;
            lblCurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");

            if (isError)
            {
                ResultBorder.Background = System.Windows.Media.Brushes.Red;
                lblResultStatus.Foreground = System.Windows.Media.Brushes.White;
            }
            else
            {
                ResultBorder.Background = System.Windows.Media.Brushes.LightGreen;
                lblResultStatus.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void ResetSystemAfterDelay()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                ResultBorder.Visibility = Visibility.Collapsed;
                this.Focus();
                _isProcessing = false;
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