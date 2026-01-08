using System;
using System.Data.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Services;

namespace Student_Attendance_System.Views
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
            dpEnrollDate.SelectedDate = DateTime.Today;
            dpBirthDate.DisplayDate = new DateTime(2003, 1, 1);
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Webcam Feature Coming Soon! (Needs Bo Sann's Help)");
        }


        

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Please fill all required fields!", "Warning");
                return;
            }

            try
            {
                using (SqlConnection conn = DBConnection.GetConnection())
                {
                    string sql = @"INSERT INTO Students(StudentID,FullName,Department,BirthDate,EnrollmentDate,PasswordHash,Photo,Role,CreatedAt)
                    VALUES(@StudentID,@FullName,@Department,@BirthDate,@EnrollDate,@Password,NULL,'Student',GETDATE());";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@StudentID", txtStudentID.Text.Trim());
                    cmd.Parameters.AddWithValue("@FullName", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Department",
                        (cboDepartment.SelectedItem as ComboBoxItem)?.Content?.ToString());
                    cmd.Parameters.AddWithValue("@BirthDate", dpBirthDate.SelectedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnrollDate", dpEnrollDate.SelectedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Password);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Student registered successfully!", "Success");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error");
            }
        }

        // =========================================================================
        // BO SANN'S CODE ZONE (Insert to Database)
        // =========================================================================
        // ACTION: INSERT INTO Users (...) VALUES (...)
        // IMPORTANT: Handle Photo as Base64 String or Byte Array
        // =========================================================================

        // --- MOCK ACTION ---
        //MessageBox.Show($"[MOCK] Student Registered Successfully!\nID: {studentID}", "Success");
        //    NavigationService.GoBack();
        //}

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
