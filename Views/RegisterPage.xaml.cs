using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Interfaces;

namespace Student_Attendance_System.Views
{
    public partial class RegisterPage : Page, ILanguageSwitchable
    {
        private bool _isJapanese = false;

        public RegisterPage()
        {
            InitializeComponent();

            // App-wide language ကို စတင်စစ်ဆေးခြင်း
            ChangeLanguage(LanguageSettings.Language);

            dpEnrollDate.SelectedDate = DateTime.Today;
            dpBirthDate.DisplayDate = new DateTime(2005, 1, 1);
        }

        // --- ILanguageSwitchable Implementation ---
        public void ChangeLanguage(bool isJapanese)
        {
            _isJapanese = isJapanese;
            UpdatePageUI();
        }

        private void UpdatePageUI()
        {
            if (_isJapanese)
            {
                txtRegTitle.Text = "新規学生登録";
                txtRegSubTitle.Text = "学生証の通りに詳細を入力してください";
                lblStuId.Text = "学籍番号 (Student ID)";
             
                lblYear.Text = "学年 (Year Level)"; // အသစ်ထည့်ထားသော label
                lblName.Text = "氏名 (Full Name)";
                lblDept.Text = "学科 (Department)";
                lblBirth.Text = "生年月日 (Birth Date)";
                lblEnroll.Text = "入学日 (Enrollment Date)";
                lblPass.Text = "システムパスワード";
                btnCancel.Content = "キャンセル";
                btnSave.Content = "学生を登録する";
                btnCaptureText.Content = "顔写真を撮影する";
            }
            else
            {
                txtRegTitle.Text = "NEW STUDENT REGISTRATION";
                txtRegSubTitle.Text = "Fill in details as shown on Student ID Card";
                lblStuId.Text = "Student ID (学籍番号)";
           
                lblYear.Text = "Year (学年)"; // အသစ်ထည့်ထားသော label
                lblName.Text = "Full Name (氏名)";
                lblDept.Text = "Department (学科)";
                lblBirth.Text = "Birth Date (生年月日)";
                lblEnroll.Text = "Enrollment Date (入学日)";
                lblPass.Text = "System Password";
                btnCancel.Content = "Cancel";
                btnSave.Content = "Save Student";
                btnCaptureText.Content = "Capture Face Photo";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                string msg = _isJapanese ? "すべての必須項目を入力してください！" : "Please fill all required fields!";
                MessageBox.Show(msg, "Warning");
                return;
            }

            try
            {
                // DB Connection string ကို ကိုယ့် project အလိုက် ပြန်စစ်ပါ
                using (SqlConnection conn = DBConnection.GetConnection())
                {
                    // YearLevel နဲ့ AssignedClass (Major) ကိုပါ သိမ်းမယ်
                    string sql = @"INSERT INTO Students 
                                 (StudentID, FullName, YearLevel, AssignedClass, Department, BirthDate, EnrollmentDate, CardBarcode, PasswordHash) 
                                 VALUES 
                                 (@SID, @Name, @Year, @Class, @Dept, @Birth, @Enroll, @Barcode, @Pass)";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@SID", txtStudentID.Text.Trim());
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());

                    // ComboBox မှ Year Level ကို Tag (1, 2, 3, 4) အနေနဲ့ ယူခြင်း
                    int year = int.Parse(((ComboBoxItem)cboYear.SelectedItem).Tag.ToString());
                    cmd.Parameters.AddWithValue("@Year", year);

                    // Student ID ရဲ့ နောက်ဆုံးစာလုံး သို့မဟုတ် major ကို AssignedClass အဖြစ် သိမ်းခြင်း
                    // ဥပမာ- C ခန်း ဆိုရင် "C"
                    cmd.Parameters.AddWithValue("@Class", "C");

                    cmd.Parameters.AddWithValue("@Dept", ((ComboBoxItem)cboDepartment.SelectedItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@Birth", dpBirthDate.SelectedDate ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@Enroll", dpEnrollDate.SelectedDate ?? DateTime.Now);
                  
                    cmd.Parameters.AddWithValue("@Pass", txtPassword.Password);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                string successMsg = _isJapanese ? "登録が完了しました！" : "Student registered successfully!";
                MessageBox.Show(successMsg, "Success");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_isJapanese ? "カメラ機能を起動します..." : "Starting Camera...");
            // Webcam logic ကို ဒီမှာ ဆက်ရေးလို့ရပါပြီ
        }
    }
}