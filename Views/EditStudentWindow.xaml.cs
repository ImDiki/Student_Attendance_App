using System;
using System.Windows;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class EditStudentWindow : Window, ILanguageSwitchable
    {
        private StudentModel student;

        // 🔹 Constructor မှာ StudentModel ကို Parameter အနေနဲ့ လက်ခံရပါမယ်
        public EditStudentWindow(StudentModel selectedStudent)
        {
            InitializeComponent();

            // 🔹 လက်ခံရရှိတဲ့ data ကို local variable ထဲ သိမ်းလိုက်ပါတယ်
            this.student = selectedStudent;

            // Language setting ကို ခေါ်ယူပြီး UI ပြောင်းလဲခြင်း
            // (LanguageSettings.Language ဟု spelling ပြန်စစ်ပေးပါ)
            ChangeLanguage(LanguageSettings.Language);

            // TextBox များထဲသို့ ကျောင်းသားအချက်အလက်ဟောင်းများ ထည့်သွင်းခြင်း
            if (student != null)
            {
                lblFullName.Text = student.FullName;
                lblYear.Text = student.YearLevel;
                lblClass.Text = student.ClassName;
                lblMajor.Text = student.Department;
            }
        }

        public void ChangeLanguage(bool isJapanese)
        {
            EditWin.Content = isJapanese ? "学生プロフィールの編集" : "EDIT STUDENT PROFILE";
            lblFullName.Text = isJapanese ? "氏名" : "Full Name";
            lblYear.Text = isJapanese ? "学年" : "Year Level";
            lblClass.Text = isJapanese ? "クラス" : "Class";
            lblMajor.Text = isJapanese ? "専攻 / 学科" : "Department (Major)";
            btnSave.Content = isJapanese ? "変更を保存" : "SAVE CHANGES";
            btnDelete.Content = isJapanese ? "学生を削除" : "DELETE STUDENT";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var con = DBConnection.GetConnection())
                {
                    con.Open();
                    // Database ထဲက column နာမည် 'Class' ဖြစ်နေတာ သတိထားပါ
                    string sql = "UPDATE Students SET FullName=@name, YearLevel=@year, Class=@class, Department=@dept WHERE StudentCode=@code";
                    var cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@name", lblFullName.Text);
                    cmd.Parameters.AddWithValue("@year", lblYear.Text);
                    cmd.Parameters.AddWithValue("@class", lblClass.Text);
                    cmd.Parameters.AddWithValue("@dept", lblMajor.Text);
                    cmd.Parameters.AddWithValue("@code", student.StudentCode);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Update Successful!");
                    this.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save Error: " + ex.Message);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this student?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var con = DBConnection.GetConnection())
                    {
                        con.Open();
                        string sql = "DELETE FROM Students WHERE StudentCode=@code";
                        var cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@code", student.StudentCode);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Student Deleted.");
                        this.DialogResult = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete Error: " + ex.Message);
                }
            }
        }
    }
}