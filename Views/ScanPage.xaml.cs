using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading; // Timer အတွက်လိုအပ်
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;
using Microsoft.Data.SqlClient;

namespace Student_Attendance_System.Views
{
    public partial class ScanPage : Page, ILanguageSwitchable
    {
        private bool _isJapanese = false;
        private DispatcherTimer redirectTimer; // Redirect လုပ်ဖို့ Timer

        private List<StudentMock> mockStudentDb = new List<StudentMock>()
        {
            new StudentMock { BarcodeID = "C5292", Name = "MYAT THADAR LINN", StudentID = "C5292" },
            new StudentMock { BarcodeID = "123456789", Name = "DIKI", StudentID = "D001" }
        };

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
            txtBadgeStatus.Text = isJapanese ? "出席 ✅ PRESENT" : "PRESENT ✅ ATTENDED";

            if (lblScannedName.Text == "Waiting..." || lblScannedName.Text == "待機中...")
                lblScannedName.Text = isJapanese ? "待機中..." : "Waiting...";
        }

        private void txtScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string id = txtScannerInput.Text.Trim().ToUpper();
                ProcessScan(id);
                txtScannerInput.Clear();
            }
        }

        private void ProcessScan(string scannedCode)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

                string query = @"
SELECT StudentCode, FullName
FROM Students
WHERE StudentCode = @code";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@code", scannedCode);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string studentCode = reader["StudentCode"].ToString();
                    string name = reader["FullName"].ToString();


                    lblScannedName.Text = name;
                    lblScannedID.Text = scannedCode;

                    AttendanceBadge.Visibility = Visibility.Visible;
                    lblStatusMessage.Text = _isJapanese
                        ? "出席が記録されました"
                        : "Attendance recorded";

                    lblStatusMessage.Foreground = Brushes.LightGreen;
                    System.Media.SystemSounds.Beep.Play();

                    reader.Close();
                    //SaveAttendance(studentCode);

                    //StartRedirectTimer();
                    bool saved = SaveAttendance(studentCode);

                    if (saved)
                    {
                        lblStatusMessage.Text = _isJapanese
                            ? "出席が記録されました"
                            : "Attendance recorded";

                        lblStatusMessage.Foreground = Brushes.LightGreen;
                        System.Media.SystemSounds.Beep.Play();

                        StartRedirectTimer();
                    }
                    else
                    {
                        lblStatusMessage.Text = _isJapanese
                            ? "すでに出席済みです"
                            : "Already scanned";

                        lblStatusMessage.Foreground = Brushes.Orange;
                        System.Media.SystemSounds.Hand.Play();
                    }

                }
                else
                {
                    lblScannedName.Text = _isJapanese ? "未登録" : "Not Found";
                    lblScannedID.Text = "----";
                    AttendanceBadge.Visibility = Visibility.Collapsed;

                    lblStatusMessage.Text = _isJapanese
                        ? "無効なカードです"
                        : "Invalid Card";

                    lblStatusMessage.Foreground = Brushes.Salmon;
                }
            }
        }
        private bool SaveAttendance(string studentCode)
        {
            using (SqlConnection con = DBConnection.GetConnection())
            {
                con.Open();

                // 1️⃣ Check duplicate
                string checkQuery = @"
SELECT COUNT(*) FROM Attendance
WHERE StudentID = @sid
  AND AttendanceDate = CAST(GETDATE() AS DATE)
  AND Subject = @subject";

                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@sid", studentCode);
                checkCmd.Parameters.AddWithValue("@subject", App.CurrentSubject);

                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    return false; // ❌ Duplicate
                }

                // 2️⃣ Insert
                string insertQuery = @"
INSERT INTO Attendance
(StudentID, Subject, AttendanceDate, AttendanceTime, Status, CreatedAt)
VALUES
(@sid, @subject,
 CAST(GETDATE() AS DATE),
 CAST(GETDATE() AS TIME),
 'Present', GETDATE())";

                SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@sid", studentCode);
                insertCmd.Parameters.AddWithValue("@subject", App.CurrentSubject);

                insertCmd.ExecuteNonQuery();
                return true; // ✅ Success
            }
        }




        // --- ၃ စက္ကန့်အကြာတွင် LoginPage ဆီသို့ အလိုအလျောက်ပြန်သွားရန် ---
        private void StartRedirectTimer()
        {
            if (redirectTimer != null) redirectTimer.Stop();

            redirectTimer = new DispatcherTimer();
            redirectTimer.Interval = TimeSpan.FromSeconds(3); // ၃ စက္ကန့် စောင့်မယ်
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

    public class StudentMock
    {
        public string BarcodeID { get; set; }
        public string Name { get; set; }
        public string StudentID { get; set; }
    }
}