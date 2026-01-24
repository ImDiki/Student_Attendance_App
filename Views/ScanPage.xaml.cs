using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Services;
using Microsoft.Data.SqlClient;

namespace Student_Attendance_System.Views
{
    public partial class ScanPage : Page, ILanguageSwitchable
    {
        private bool _isJapanese = false;
        private DispatcherTimer redirectTimer;

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
            lblHeaderMajor.Text = isJapanese ? "専攻" : "Major";
            txtBadgeStatus.Text = isJapanese ? "出席 ✅ PRESENT" : "PRESENT ✅ ATTENDED";

            if (lblScannedName.Text == "Waiting..." || lblScannedName.Text == "待機中...")
                lblScannedName.Text = isJapanese ? "待機中..." : "Waiting...";
        }

        private void txtScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // ၁။ Weekend Check
                DayOfWeek today = DateTime.Today.DayOfWeek;
                if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
                {
                    lblStatusMessage.Text = _isJapanese ? "本日は休日です" : "Today is Weekend. No classes.";
                    lblStatusMessage.Foreground = Brushes.Orange;
                    txtScannerInput.Clear();
                    return;
                }

                // ၂။ Teacher Start Class Check
                string id = txtScannerInput.Text.Trim().ToUpper();
                if (!AttendanceService.IsClassActive(App.CurrentSubject))
                {
                    lblStatusMessage.Text = _isJapanese ? "授業はまだ開始されていません" : "Class not started or unavailable";
                    lblStatusMessage.Foreground = Brushes.Orange;
                    txtScannerInput.Clear();
                    return;
                }

                ProcessScan(id);
                txtScannerInput.Clear();
            }
        }

        private void ProcessScan(string scannedCode)
        {
            try
            {
                using (SqlConnection con = DBConnection.GetConnection())
                {
                    con.Open();
                    // Department (Major) ကိုပါ SELECT လုပ်ပါတယ်
                    string query = "SELECT StudentCode, FullName, Department FROM Students WHERE StudentCode = @code";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@code", scannedCode);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string studentCode = reader["StudentCode"].ToString();
                            string name = reader["FullName"].ToString();
                            string major = reader["Department"]?.ToString() ?? "N/A";

                            lblScannedName.Text = name;
                            lblScannedID.Text = studentCode;
                            lblScannedMajor.Text = major;
                            AttendanceBadge.Visibility = Visibility.Visible;

                            // Attendance Logic
                            string currentStatus = AttendanceService.GetTodayStatus(studentCode, App.CurrentSubject);

                            if (string.IsNullOrEmpty(currentStatus))
                            {
                                AttendanceService.MarkPresent(studentCode, App.CurrentSubject);
                                lblStatusMessage.Text = _isJapanese ? "出席が記録されました" : "Attendance recorded";
                                lblStatusMessage.Foreground = Brushes.LightGreen;
                                System.Media.SystemSounds.Beep.Play();
                                StartRedirectTimer();
                            }
                            else
                            {
                                lblStatusMessage.Text = _isJapanese ? "すでに出席済みです" : "Already scanned today";
                                lblStatusMessage.Foreground = Brushes.Orange;
                                System.Media.SystemSounds.Hand.Play();
                            }
                        }
                        else
                        {
                            ShowInvalidCard();
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblStatusMessage.Text = "Database Error!";
                lblStatusMessage.Foreground = Brushes.Salmon;
            }
        }

        private void ShowInvalidCard()
        {
            lblScannedName.Text = _isJapanese ? "未登録" : "Not Found";
            lblScannedID.Text = "----";
            lblScannedMajor.Text = "----";
            AttendanceBadge.Visibility = Visibility.Collapsed;
            lblStatusMessage.Text = _isJapanese ? "無効なカードです" : "Invalid Card";
            lblStatusMessage.Foreground = Brushes.Salmon;
        }

        private void StartRedirectTimer()
        {
            if (redirectTimer != null) redirectTimer.Stop();
            redirectTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
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
}