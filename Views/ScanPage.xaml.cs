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

            // UI တစ်ခုလုံးကို နှိပ်လိုက်တိုင်း Input Box ဆီ Focus ပြန်သွားစေရန်
            this.Loaded += (s, e) => txtScannerInput.Focus();
            this.PreviewMouseDown += (s, e) => txtScannerInput.Focus();
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
                // ၁။ Weekend Check (စနေ၊ တနင်္ဂနွေ ဖြစ်ပါက Scan ပိတ်မည်)
                DayOfWeek today = DateTime.Today.DayOfWeek;
                if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
                {
                    lblStatusMessage.Text = _isJapanese ? "本日は休日です" : "Today is Weekend. No classes.";
                    lblStatusMessage.Foreground = Brushes.Orange;
                    txtScannerInput.Clear();
                    return;
                }

                // ၂။ Teacher Start Class Check (Teacher ဘက်က Class စတင်မှ Scan ခွင့်ပြုမည်)
                string id = txtScannerInput.Text.Trim().ToUpper();

                // Teacher Portal မှ Status ကို Database တွင် တိုက်ရိုက်စစ်ဆေးခြင်း
                if (!IsTeacherClassStarted())
                {
                    lblStatusMessage.Text = _isJapanese ? "授業はまだ開始されていません" : "Class not started by Teacher";
                    lblStatusMessage.Foreground = Brushes.Orange;
                    txtScannerInput.Clear();
                    return;
                }

                ProcessScan(id);
                txtScannerInput.Clear();
            }
        }

        // Teacher Dashboard အခြေအနေကို Database မှ တိုက်ရိုက်စစ်သည့် Method
        /* private bool IsTeacherClassStarted()
         {
             try
             {
                 using (SqlConnection con = DBConnection.GetConnection())
                 {
                     con.Open();

                     // ScanPage.xaml.cs ထဲက IsTeacherClassStarted method မှာ ဒီလိုလေး ပြောင်းစမ်းကြည့်ပါ
                     string query = @"SELECT COUNT(*) FROM Timetable 
                  WHERE CAST(Date AS DATE) = CAST(GETDATE() AS DATE) 
                  AND Status = 'Started'";
                     // အချိန်နဲ့ စစ်တဲ့အပိုင်းကို ခေတ္တ ဖျောက်ထားသည်





                     // ပိုမိုတိကျစေရန် ယနေ့ရက်စွဲသာမက လက်ရှိအချိန် (Current Time) အတွင်းမှာရှိတဲ့ Started class ကို စစ်ပါမယ်

                     // string query = @"SELECT COUNT(*) FROM Timetable 
                     //    WHERE CAST(Date AS DATE) = CAST(GETDATE() AS DATE) 
                     //  AND Status = 'Started' 
                     //AND @currentTime BETWEEN StartTime AND EndTime";

                     SqlCommand cmd = new SqlCommand(query, con);
                     // လက်ရှိအချိန်ကို Parameter အဖြစ် ထည့်ပေးပါ
                     cmd.Parameters.AddWithValue("@currentTime", DateTime.Now.ToString("HH:mm"));

                     int count = (int)cmd.ExecuteScalar();
                     return count > 0;
                 }
             }
             catch (Exception)
             {
                 return false;
             }
         } */
        private bool IsTeacherClassStarted()
        {
            try
            {
                using (SqlConnection con = DBConnection.GetConnection())
                {
                    con.Open();
                    // Period က 99 ဖြစ်နေရင် Teacher က Start Class နှိပ်ထားတယ်လို့ ယူဆပါမယ်
                    string query = @"SELECT COUNT(*) FROM Timetables 
                             WHERE SubjectName = @subj 
                             AND DayOfWeek = @day 
                             AND Period = 99";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@subj", App.CurrentSubject);
                    cmd.Parameters.AddWithValue("@day", (int)DateTime.Now.DayOfWeek);

                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch { return false; }
        }



        private void ProcessScan(string scannedCode)
        {
            try
            {
                using (SqlConnection con = DBConnection.GetConnection())
                {
                    con.Open();
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
                                System.Media.SystemSounds.Beep.Play(); // အောင်မြင်ပါက Beep မြည်မည်
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
            // ၃ စက္ကန့်အကြာတွင် LoginPage သို့ အလိုအလျောက်ပြန်ပို့မည်
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