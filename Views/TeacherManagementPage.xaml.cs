using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;


namespace Student_Attendance_System.Views
{
    public partial class TeacherManagementPage : Page
    {
        private int _selectedTeacherId = 0; // Users.UserId (also Teachers.TeacherId)

        public TeacherManagementPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTeachers();
        }

        // ===== Model for grid =====
        private class TeacherRow
        {
            public int UserId { get; set; }
            public string TeacherCode { get; set; }
            public string FullName { get; set; }
            public string Department { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        // ===== Load list =====
        private void LoadTeachers()
        {
            List<TeacherRow> list = new();

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = @"
SELECT t.TeacherId, t.TeacherCode, t.FullName, t.Department, t.Email, t.Phone
FROM Teachers t
INNER JOIN Users u ON u.UserId = t.TeacherId
WHERE u.Role = 'Teacher'
ORDER BY t.TeacherCode";

            using SqlCommand cmd = new SqlCommand(sql, con);
            using SqlDataReader r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new TeacherRow
                {
                    UserId = r.GetInt32(0),
                    TeacherCode = r.GetString(1),
                    FullName = r.GetString(2),
                    Department = r.IsDBNull(3) ? "" : r.GetString(3),
                    Email = r.IsDBNull(4) ? "" : r.GetString(4),
                    Phone = r.IsDBNull(5) ? "" : r.GetString(5)
                });
            }

            dgTeachers.ItemsSource = list;
        }
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
        public static event Action TeacherChanged;

        // ===== Add teacher =====
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string code = txtTeacherCode.Text.Trim();
            string name = txtFullName.Text.Trim();
            string dept = (cboDepartment.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string pass = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(code) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("TeacherCode, FullName, Password are required.");
                return;
            }

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            SqlTransaction tx = con.BeginTransaction();

            try
            {
                // 1) Users insert (Username = TeacherCode)
                string userSql = @"
INSERT INTO Users (Username, PasswordHash, Role)
OUTPUT INSERTED.UserId
VALUES (@u, @p, 'Teacher')";

                using SqlCommand userCmd = new SqlCommand(userSql, con, tx);
                userCmd.Parameters.AddWithValue("@u", code);
                userCmd.Parameters.AddWithValue("@p", HashPassword(pass));
                //userCmd.Parameters.AddWithValue("@f", name);

                int userId = (int)userCmd.ExecuteScalar();

                // 2) Teachers insert
                string teacherSql = @"
INSERT INTO Teachers (TeacherId, TeacherCode, FullName, Department, Email, Phone)
VALUES (@id, @code, @name, @dept, @email, @phone)";

                using SqlCommand tCmd = new SqlCommand(teacherSql, con, tx);
                tCmd.Parameters.AddWithValue("@id", userId);
                tCmd.Parameters.AddWithValue("@code", code);
                tCmd.Parameters.AddWithValue("@name", name);
                tCmd.Parameters.AddWithValue("@dept", (object)dept ?? DBNull.Value);
                tCmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email);
                tCmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(phone) ? DBNull.Value : phone);

                tCmd.ExecuteNonQuery();

                tx.Commit();

                TeacherChanged?.Invoke(); //notify admin dashboard

                MessageBox.Show("Teacher added!");
                ClearForm();
                LoadTeachers();

            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Add failed:\n" + ex.Message);
            }
        }

        // ===== Select from grid to edit =====
        private void dgTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTeachers.SelectedItem == null) return;

            dynamic row = dgTeachers.SelectedItem;

            _selectedTeacherId = (int)row.UserId;
            txtTeacherCode.Text = (string)row.TeacherCode;
            txtFullName.Text = (string)row.FullName;
            txtEmail.Text = (string)row.Email;
            txtPhone.Text = (string)row.Phone;

            // set department selection
            string dept = (string)row.Department;
            cboDepartment.SelectedIndex = -1;
            for (int i = 0; i < cboDepartment.Items.Count; i++)
            {
                if (cboDepartment.Items[i] is ComboBoxItem item &&
                    (item.Content?.ToString() ?? "") == dept)
                {
                    cboDepartment.SelectedIndex = i;
                    break;
                }
            }
        }

        // ===== Update teacher info =====
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTeacherId == 0)
            {
                MessageBox.Show("Select a teacher from the list first.");
                return;
            }

            string code = txtTeacherCode.Text.Trim();
            string name = txtFullName.Text.Trim();
            string dept = (cboDepartment.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("TeacherCode and FullName are required.");
                return;
            }

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            SqlTransaction tx = con.BeginTransaction();

            try
            {
                // Update Users (Username + FullName)
                string userSql = @"UPDATE Users SET Username = @u WHERE UserId = @id";

                using SqlCommand userCmd = new SqlCommand(userSql, con, tx);
                userCmd.Parameters.AddWithValue("@u", code);
                //userCmd.Parameters.AddWithValue("@f", name);
                userCmd.Parameters.AddWithValue("@id", _selectedTeacherId);
                userCmd.ExecuteNonQuery();

                // Update Teachers
                string tSql = @"UPDATE Teachers SET TeacherCode = @code, FullName = @name, Department = @dept, Email = @email, Phone = @phone WHERE TeacherId = @id";

                using SqlCommand tCmd = new SqlCommand(tSql, con, tx);
                tCmd.Parameters.AddWithValue("@code", code);
                tCmd.Parameters.AddWithValue("@name", name);
                tCmd.Parameters.AddWithValue("@dept", (object)dept ?? DBNull.Value);
                tCmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email);
                tCmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(phone) ? DBNull.Value : phone);
                tCmd.Parameters.AddWithValue("@id", _selectedTeacherId);
                tCmd.ExecuteNonQuery();

                tx.Commit();
                TeacherChanged?.Invoke();
                MessageBox.Show("Teacher updated!");
                LoadTeachers();

            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Update failed:\n" + ex.Message);
            }
        }

        // ===== Reset password (hashed) =====
        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTeacherId == 0)
            {
                MessageBox.Show("Select a teacher first.");
                return;
            }

            string pass = txtPassword.Password;
            if (string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Enter new password in Password box.");
                return;
            }

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string sql = "UPDATE Users SET PasswordHash = @p WHERE UserId = @id";
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@p", HashPassword(pass));
            cmd.Parameters.AddWithValue("@id", _selectedTeacherId);

            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Password reset successfully!");
                txtPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reset failed:\n" + ex.Message);
            }
        }

        // ===== Delete teacher =====
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTeacherId == 0)
            {
                MessageBox.Show("Select a teacher first.");
                return;
            }

            if (MessageBox.Show("Delete this teacher?", "Confirm",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using SqlConnection con = DBConnection.GetConnection();
            con.Open();
            SqlTransaction tx = con.BeginTransaction();

            try
            {
                // Delete from Teachers first (FK)
                string tSql = "DELETE FROM Teachers WHERE TeacherId = @id";
                using (SqlCommand tCmd = new SqlCommand(tSql, con, tx))
                {
                    tCmd.Parameters.AddWithValue("@id", _selectedTeacherId);
                    tCmd.ExecuteNonQuery();
                }

                // Delete from Users
                string uSql = "DELETE FROM Users WHERE UserId = @id";
                using (SqlCommand uCmd = new SqlCommand(uSql, con, tx))
                {
                    uCmd.Parameters.AddWithValue("@id", _selectedTeacherId);
                    uCmd.ExecuteNonQuery();
                }

                tx.Commit();

                TeacherChanged?.Invoke();

                MessageBox.Show("Teacher deleted!");

                ClearForm();
                LoadTeachers();

            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Delete failed:\n" + ex.Message);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTeachers();
        }

        private void ClearForm()
        {
            _selectedTeacherId = 0;
            txtTeacherCode.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtPassword.Clear();
            cboDepartment.SelectedIndex = -1;
            dgTeachers.SelectedItem = null;
        }
    }
}
