using System; // Exception အတွက်
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Data.SqlClient;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password", "Warning");
                return;
            }

            // =========================================================================
            // BO SANN'S CODE ZONE (Database Check - ACTIVATED)
            // =========================================================================

            User user = null;

            try
            {
                user = new User { Username = "C5292", FullName = "Myat Thadar Linn", Role = "Student", Major = "IT" };
            }
            else if (username == "admin" && password == "admin")
            {
                user = new User { Username = "admin", FullName = "Head Master", Role = "Teacher" };
            }

                            using (SqlDataReader r = cmd.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    user = new User
                                    {
                                        Username = r["StudentID"].ToString(),
                                        FullName = r["FullName"].ToString(),
                                        Role = "Student",
                                        Major = r["Department"].ToString()
                                    };
                                }
                            }
                        }
                    }
                }
                // RESULT
                if (user != null)
                {
                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.HandleLoginSuccess(user);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password", "Login Failed",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            // =========================================================================
            // DATABASE ZONE (Commented Out / CLOSED)
            // =========================================================================

            /* User user = null;

            try
            {
                using (SqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    // 1️⃣ ADMIN
                    string adminSql = @"SELECT Username, FullName, Role
                                FROM Admins
                                WHERE Username=@U AND PasswordHash=@P";

                    using (SqlCommand cmd = new SqlCommand(adminSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@U", username);
                        cmd.Parameters.AddWithValue("@P", password);

                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                user = new User
                                {
                                    Username = r["Username"].ToString(),
                                    FullName = r["FullName"].ToString(),
                                    Role = "Admin",
                                    Major = "Administration"
                                };
                            }
                        }
                    }
                    // 2️⃣ TEACHER

                    if (user == null)
                    {
                        string teacherSql = @"SELECT Username, FullName, Department
                                            FROM Teachers
                                            WHERE Username=@U AND PasswordHash=@P";

                        using (SqlCommand cmd = new SqlCommand(teacherSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@U", username);
                            cmd.Parameters.AddWithValue("@P", password);

                            using (SqlDataReader r = cmd.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    user = new User
                                    {
                                        Username = r["Username"].ToString(),
                                        FullName = r["FullName"].ToString(),
                                        Role = "Teacher",
                                        Major = r["Department"].ToString()
                                    };
                                }
                            }
                        }
                    }
                    // 3️⃣ STUDENT
                    if (user == null)
                    {
                        string studentSql = @"SELECT StudentID, FullName, Department
                                            FROM Students
                                            WHERE StudentID=@U AND PasswordHash=@P";

                        using (SqlCommand cmd = new SqlCommand(studentSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@U", username);
                            cmd.Parameters.AddWithValue("@P", password);

                            using (SqlDataReader r = cmd.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    user = new User
                                    {
                                        Username = r["StudentID"].ToString(),
                                        FullName = r["FullName"].ToString(),
                                        Role = "Student",
                                        Major = r["Department"].ToString()
                                    };
                                }
                            }
                        }
                    }
                }
                // RESULT
                if (user != null)
                {
                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.HandleLoginSuccess(user);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password", "Login Failed",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            */
        }

        // =========================================================================
        // BO SANN'S CODE ZONE (Database Check)
        // =========================================================================

        //User user = null;

        //    // --- MOCK DATA (အစမ်း) ---
        //    if (username == "C5292" && password == "1234")
        //    {
        //        user = new User { Id = 1, Username = "C5292", FullName = "Myat Thadar Linn", Role = "Student", Major = "IT" };
        //    }
        //    else if (username == "admin" && password == "admin")
        //    {
        //        user = new User { Id = 99, Username = "admin", FullName = "Head Master", Role = "Teacher" };
        //    }

        //    if (user != null)
        //    {
        //        // Login အောင်ရင် MainWindow ကို လှမ်းပြောပြီး Sidebar တွေဖွင့်ခိုင်းမယ်
        //        if (Application.Current.MainWindow is MainWindow mainWindow)
        //        {
        //            mainWindow.HandleLoginSuccess(user);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Login Failed!\n\nUse:\nStudent: C5292 / 1234\nTeacher: admin / admin", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        // Register Link နှိပ်ရင် အလုပ်လုပ်နေရာ
        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}