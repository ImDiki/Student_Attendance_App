using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Services;
using Student_Attendance_System.Interfaces;

namespace Student_Attendance_System.Views
{
    public partial class StudentProfile : Page, ILanguageSwitchable
    {
        public StudentProfile()
        {
            InitializeComponent();
            ChangeLanguage(LanguageSettings.Language);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => LoadProfile();

        public void ChangeLanguage(bool isJapanese)
        {
            txtTitle.Text = isJapanese ? "マイプロフィール" : "MY PROFILE";
            lblID.Text = isJapanese ? "学籍番号" : "Student ID";
            lblName.Text = isJapanese ? "氏名" : "Full Name";
            lblYear.Text = isJapanese ? "学年" : "Year Level";
            lblClass.Text = isJapanese ? "クラス" : "Class";
            lblDept.Text = isJapanese ? "専攻" : "Department";
            lblSecurity.Text = isJapanese ? "セキュリティ" : "Account Security";
            lblPassword.Text = isJapanese ? "パスワード" : "Password";
        }

        private void LoadProfile()
        {
            try
            {
                var user = UserData.UserData.CurrentUser;
                if (user == null) return;

                using (var con = DBConnection.GetConnection())
                {
                    con.Open();

                    // ⚠️ JOIN သုံးပြီး PasswordHash column ကို ယူပါသည်
                    string sql = @"SELECT s.StudentCode, s.FullName, s.YearLevel, s.Class, s.Department, s.FacePhoto, u.PasswordHash 
                                   FROM Students s 
                                   INNER JOIN Users u ON s.StudentCode = u.Username 
                                   WHERE s.StudentCode = @id";

                    var cmd = new SqlCommand(sql, con);

                    // ✅ image_32d5eb error အတွက် Parameter ပို့ပေးခြင်း
                    cmd.Parameters.AddWithValue("@id", user.Username);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            txtID.Text = r["StudentCode"].ToString();
                            txtName.Text = r["FullName"].ToString();
                            txtYear.Text = r["YearLevel"].ToString();
                            txtClass.Text = r["Class"].ToString();
                            txtDept.Text = r["Department"].ToString();

                            // 🖼️ FacePhoto ဖော်ပြခြင်း
                            if (r["FacePhoto"] != DBNull.Value)
                            {
                                imgProfile.Source = ConvertByteArrayToImage((byte[])r["FacePhoto"]);
                            }

                            // 🔒 Password Stars logic (PasswordHash column ကို သုံးပါသည်)
                            // Hash ဖြစ်နေသော်လည်း စာလုံးအရေအတွက်အတိုင်း အစက်ပြပါမည်
                            string pHash = r["PasswordHash"].ToString();
                            txtPassStars.Text = new string('•', pHash.Length > 8 ? 8 : pHash.Length);

                           
                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Profile Error: " + ex.Message);
            }
        }

        private BitmapImage ConvertByteArrayToImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}