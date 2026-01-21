using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System.Models;
using Student_Attendance_System.Interfaces;
using Microsoft.Data.SqlClient;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.Security.Cryptography;
using System.Text;


namespace Student_Attendance_System.Views
{
    public partial class RegisterPage : Page, ILanguageSwitchable
    {
        public RegisterPage()
        {
            InitializeComponent();
            // Enrollment Date ကို ယနေ့ရက်စွဲ အလိုအလျောက် သတ်မှတ်ပေးခြင်း
            dpEnrollDate.SelectedDate = DateTime.Today;

            // App-wide Language settings နှင့် ချိတ်ဆက်ခြင်း
            ChangeLanguage(LanguageSettings.Language);
        }

        // --- ILanguageSwitchable Implementation: ENG/JP ပြောင်းလဲခြင်း ---
        public void ChangeLanguage(bool isJapanese)
        {
            // Title & Labels
            txtRegTitle.Text = isJapanese ? "新規学生登録" : "NEW STUDENT REGISTRATION";
           
            lblStuId.Text = isJapanese ? "学籍番号" : "Student ID";
            lblYear.Text = isJapanese ? "学年" : "Year (Grade)";
            lblClass.Text = isJapanese ? "クラス" : "Class";
            lblName.Text = isJapanese ? "氏名" : "Full Name";
            lblDept.Text = isJapanese ? "専攻 / 学科" : "Department / Major";
            lblBirth.Text = isJapanese ? "生年月日" : "Birth Date";
            lblEnroll.Text = isJapanese ? "入学日" : "Enrollment Date";
            lblPass.Text = isJapanese ? "パスワード" : "System Password";

            // Buttons
            btnSave.Content = isJapanese ? "登録する" : "Save Student";
            btnCancel.Content = isJapanese ? "キャンセル" : "Cancel";
            btnCaptureText.Content = isJapanese ? "顔写真を撮影" : "Capture Photo";

            // Major List Language Sync (Duration များ အလိုအလျောက်ပြောင်းရန်)
            UpdateMajorList(isJapanese);
        }

        private void UpdateMajorList(bool isJapanese)
        {
            // ComboBox ထဲက Major များကို ဘရိုပေးထားသော စာရင်းအတိုင်း Language ညှိပေးခြင်း
            int selectedIndex = cboDepartment.SelectedIndex != -1 ? cboDepartment.SelectedIndex : 0;
            cboDepartment.Items.Clear();

            string y4 = isJapanese ? "4年" : "4 Years";
            string y3 = isJapanese ? "3年" : "3 Years";
            string y2 = isJapanese ? "2年" : "2 Years";

            cboDepartment.Items.Add(new ComboBoxItem { Content = $"ITスペシャリスト専攻 (IT Specialist) - {y4}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"ネットワークセキュリティ専攻 (Network Security) - {y4}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"システムエンジニア専攻 (Systems Engineer) - {y3}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"ネットワークエンジニア専攻 (Network Engineer) - {y3}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"Webエンジニア専攻 (Web Engineer) - {y3}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"テクニカルコース (Technical Course) - {y2}" });
            cboDepartment.Items.Add(new ComboBoxItem { Content = $"ネットワークシステムコース (Network Systems) - {y2}" });

            cboDepartment.SelectedIndex = selectedIndex;
        }

        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    // Front-end Validation (အခြေခံစစ်ဆေးချက်)
        //    if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtStudentID.Text))
        //    {
        //        string msg = LanguageSettings.Language ? "必要事項を入力してください" : "Please fill required fields.";
        //        MessageBox.Show(msg, "Validation");
        //        return;
        //    }

        //    // Create Mock User for Redirect (Database မပါဘဲ Dashboard သို့ တိုက်ရိုက်ပို့ရန်)
        //    var mockUser = new User
        //    {
        //        FullName = txtName.Text.Trim(),
        //        Username = txtStudentID.Text.Trim(),
        //        Role = "Student"
        //    };

        //    // MainWindow ရှိ HandleLoginSuccess ကို လှမ်းခေါ်ခြင်း
        //    if (Application.Current.MainWindow is MainWindow main)
        //    {
        //        main.HandleLoginSuccess(mockUser);
        //    }
        //}
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show(
                    LanguageSettings.Language
                        ? "必要事項を入力してください"
                        : "Please fill required fields.",
                    "Validation");
                return;
            }

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            SqlTransaction tx = con.BeginTransaction();

            try
            {
                //INSERT INTO Users (ROLE = Student)
                string userSql = @"INSERT INTO Users (Username, PasswordHash, Role) 
                OUTPUT INSERTED.UserId VALUES (@username, @password, 'Student')";

                SqlCommand userCmd = new SqlCommand(userSql, con, tx);
                userCmd.Parameters.AddWithValue("@username", txtStudentID.Text.Trim());
                string hashedPassword = HashPassword(txtPassword.Password.Trim());
                userCmd.Parameters.AddWithValue("@password", hashedPassword);


                int userId = (int)userCmd.ExecuteScalar();

                //INSERT INTO Students
                string studentSql = @"INSERT INTO Students(StudentId, StudentCode, FullName, YearLevel, Class, Department,
                BirthDate, EnrollmentDate, FacePhoto)VALUES(@id, @code, @name, @year, @class, @dept, @birth, @enroll, @photo)";

                SqlCommand stuCmd = new SqlCommand(studentSql, con, tx);
                stuCmd.Parameters.AddWithValue("@id", userId);
                stuCmd.Parameters.AddWithValue("@code", txtStudentID.Text.Trim());
                stuCmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                stuCmd.Parameters.AddWithValue("@year",
                    (cboYear.SelectedItem as ComboBoxItem)?.Content.ToString());
                stuCmd.Parameters.AddWithValue("@class",
                    (cboClass.SelectedItem as ComboBoxItem)?.Content.ToString());
                stuCmd.Parameters.AddWithValue("@dept",
                    (cboDepartment.SelectedItem as ComboBoxItem)?.Content.ToString());
                stuCmd.Parameters.AddWithValue("@birth",
                    dpBirthDate.SelectedDate ?? (object)DBNull.Value);
                stuCmd.Parameters.AddWithValue("@enroll",
                    dpEnrollDate.SelectedDate ?? DateTime.Today);

                byte[] photoBytes = GetCapturedPhotoBytes();
                stuCmd.Parameters.AddWithValue("@photo",
                    (object)photoBytes ?? DBNull.Value);

                stuCmd.ExecuteNonQuery();



                //COMMIT
                tx.Commit();

                MessageBox.Show(
                    LanguageSettings.Language
                        ? "学生登録が完了しました"
                        : "Student registered successfully!");

                //Go back to Login page
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Database Error:\n" + ex.Message);
            }
        }


        private void btnBack_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap capturedBitmap;
        private bool isCameraRunning = false;

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (!isCameraRunning)
            {
                // OPEN CAMERA
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("No camera detected");
                    return;
                }
                
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();

                isCameraRunning = true;
                btnCaptureText.Content = LanguageSettings.Language ? "撮影" : "Capture";
            }
            else
            {
                // CAPTURE & STOP CAMERA
                videoSource.SignalToStop();
                videoSource.NewFrame -= VideoSource_NewFrame;

                isCameraRunning = false;
                btnCaptureText.Content = LanguageSettings.Language ? "再撮影" : "Retake";
            }
        }
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            capturedBitmap = (Bitmap)eventArgs.Frame.Clone();

            Dispatcher.Invoke(() =>
            {
                imgCamera.Source = ConvertBitmapToBitmapImage(capturedBitmap);
            });
        }
        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            ms.Position = 0;

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = ms;
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            return img;
        }

        private byte[] GetCapturedPhotoBytes()
        {
            if (capturedBitmap == null)
                return null;

            using MemoryStream ms = new MemoryStream();
            capturedBitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
