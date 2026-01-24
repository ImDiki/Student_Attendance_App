using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Models; // StudentModel ကို သိရှိစေရန်
using Student_Attendance_System.Services;

namespace Student_Attendance_System.Views
{
    public partial class ClassManagementPage : Page
    {
        public ClassManagementPage()
        {
            InitializeComponent();
            LoadStudents(); // စာမျက်နှာဖွင့်လျှင် data အလိုအလျောက်ဆွဲထုတ်မည်
        }

        // 🔹 ၁။ Database မှ ကျောင်းသားစာရင်းအားလုံးကို ဆွဲထုတ်ခြင်း
        private void LoadStudents()
        {
            List<StudentModel> list = new List<StudentModel>();
            try
            {
                using (var con = DBConnection.GetConnection())
                {
                    con.Open();
                    // Database column 'Class' ကို သုံးထားပါသည်
                    var cmd = new SqlCommand("SELECT StudentCode, FullName, YearLevel, Class, Department FROM Students", con);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new StudentModel
                            {
                                StudentCode = r["StudentCode"].ToString(),
                                FullName = r["FullName"].ToString(),
                                YearLevel = r["YearLevel"].ToString(),
                                ClassName = r["Class"].ToString(),
                                Department = r["Department"].ToString()
                            });
                        }
                    }
                }
                dgStudents.ItemsSource = list; // DataGrid သို့ data ပို့ခြင်း
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        // 🔹 ၂။ Row ကို နှိပ်လျှင် Form ထဲသို့ Data ပြန်ဖြည့်ပေးခြင်း (FIXED Logic)
        private void dgStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgStudents.SelectedItem is StudentModel selected)
            {
                txtSID.Text = selected.StudentCode;
                txtSID.IsEnabled = false; // ID ကို Update လုပ်ချိန်တွင် ပြင်ခွင့်မပြုပါ
                txtSName.Text = selected.FullName;
                txtSClass.Text = selected.ClassName;

                // Department ComboBox ကို Matching လုပ်ခြင်း
                foreach (ComboBoxItem item in cboSDpt.Items)
                {
                    if (item.Content.ToString() == selected.Department)
                    {
                        cboSDpt.SelectedItem = item;
                        break;
                    }
                }

                // Year Level ComboBox ကို Matching လုပ်ခြင်း
                foreach (ComboBoxItem item in cboYear.Items)
                {
                    if (item.Content.ToString() == selected.YearLevel)
                    {
                        cboYear.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        // 🔹 ၃။ ကျောင်းသားသစ်ထည့်ခြင်း သို့မဟုတ် ရှိပြီးသားကို Update လုပ်ခြင်း
        private void AddOrUpdateStudent_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSID.Text))
            {
                MessageBox.Show("Please enter Student ID.");
                return;
            }

            try
            {
                using (var con = DBConnection.GetConnection())
                {
                    con.Open();
                    // StudentId NULL error အတွက် Auto-calculation သုံးထားပါသည်
                    string sql = @"IF EXISTS (SELECT 1 FROM Students WHERE StudentCode = @code)
                                   UPDATE Students SET FullName=@name, YearLevel=@year, Class=@class, Department=@dept WHERE StudentCode=@code
                                   ELSE
                                   INSERT INTO Students (StudentId, StudentCode, FullName, YearLevel, Class, Department) 
                                   VALUES ((SELECT ISNULL(MAX(StudentId),0)+1 FROM Students), @code, @name, @year, @class, @dept)";

                    var cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@code", txtSID.Text);
                    cmd.Parameters.AddWithValue("@name", txtSName.Text);
                    cmd.Parameters.AddWithValue("@year", (cboYear.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@class", txtSClass.Text);
                    // ComboBox မှ Department ကို ယူခြင်း
                    cmd.Parameters.AddWithValue("@dept", (cboSDpt.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "");

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Database Updated Successfully!");
                    LoadStudents();
                    ClearFields();
                }
            }
            catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
        }

        // 🔹 ၄။ ကျောင်းသားဖျက်ခြင်း
        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSID.Text)) return;
            if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var con = DBConnection.GetConnection())
                    {
                        con.Open();
                        var cmd = new SqlCommand("DELETE FROM Students WHERE StudentCode=@code", con);
                        cmd.Parameters.AddWithValue("@code", txtSID.Text);
                        cmd.ExecuteNonQuery();
                        LoadStudents();
                        ClearFields();
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        // 🔹 ၅။ Form ရှင်းထုတ်ခြင်း
        private void ClearFields_Click(object sender, RoutedEventArgs e) => ClearFields();

        private void ClearFields()
        {
            txtSID.Clear();
            txtSID.IsEnabled = true;
            txtSName.Clear();
            txtSClass.Clear();
            cboYear.SelectedIndex = -1;
            cboSDpt.SelectedIndex = -1; // Dropdown အား Reset လုပ်ခြင်း
        }
    }
}