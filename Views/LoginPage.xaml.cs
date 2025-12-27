using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // MouseButtonEventArgs အတွက် ဒါမဖြစ်မနေလိုတယ်
using System.Windows.Navigation;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        // Login Button နှိပ်ရင် အလုပ်လုပ်မယ့်နေရာ
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // ---------------------------------------------------------
            // bo sann's code (Backend Logic)
            // ---------------------------------------------------------
            // Database စစ်ဆေးခြင်း (လောလောဆယ် Fake Data နဲ့ စစ်မယ်)
            User fakeUser = null;

            if (username == "student" && password == "123")
            {
                fakeUser = new User { Id = 1, Username = "std001", Role = "Student", FullName = "Mg Mg", Major = "IT" };
            }
            else if (username == "teacher" && password == "123")
            {
                fakeUser = new User { Id = 2, Username = "tr001", Role = "Teacher", FullName = "Daw Mya", Major = "IT" };
            }
            else if (username == "admin" && password == "123")
            {
                fakeUser = new User { Id = 99, Username = "admin", Role = "Admin", FullName = "Head Master" };
            }
            // ---------------------------------------------------------

            if (fakeUser != null)
            {
                // Login အောင်မြင်ရင် MainWindow ကို လှမ်းပြောပြီး Sidebar တွေဖွင့်ခိုင်းမယ်
                // Application.Current.MainWindow ဆိုတာ လက်ရှိ Run နေတဲ့ Window ကြီးကို လှမ်းခေါ်တာ
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.HandleLoginSuccess(fakeUser);
                }
            }
            else
            {
                MessageBox.Show("Login Failed! Incorrect username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Register စာသားကို နှိပ်ရင် အလုပ်လုပ်မယ့်နေရာ (Error တက်ခဲ့တဲ့နေရာ)
        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            // RegisterPage ကို ကူးမယ်
            NavigationService.Navigate(new RegisterPage());
        }
    }
}