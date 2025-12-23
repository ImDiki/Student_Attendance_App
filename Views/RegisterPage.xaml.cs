using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Student_Attendance_System; //

namespace Student_Attendance_System.Views
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        // 1. Back Button 
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        // 2. Webcam Button 
        private void btnWebcam_Click(object sender, RoutedEventArgs e)
        {
            // WebcamWindow ကို သီးသန့် Window အနေနဲ့ ဖွင့်မယ်
            WebcamWindow cam = new WebcamWindow();
            cam.ShowDialog();
        }

        // 3. Register Button 
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // အရင်ဆုံး ID နဲ့ Name ဖြည့်ထားလား စစ်မယ်
            if (string.IsNullOrWhiteSpace(txtID.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter Student Name and ID!", "Missing Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // မဖြည့်ရသေးရင် ဆက်မလုပ်ဘူး
            }

            // User အသစ်တစ်ခု တည်ဆောက်မယ်
            User newUser = new User
            {
                Username = txtID.Text.Trim(), // ID ကို Username အဖြစ်သုံးမယ်
                Password = "123",             // Default Password "123" 
                Role = "Student",             // ကျောင်းသားမို့လို့ Student Role 
                FullName = txtName.Text.Trim()
            };

            // MockDatabase ထဲကို သိမ်း
            MockDatabase.Users.Add(newUser);

            // အောင်မြင်
            MessageBox.Show($"Registration Successful!\nLogin ID: {newUser.Username}\nPassword: 123",
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Login Page ကို တန်းပို့
            NavigationService.Navigate(new LoginPage());
        }
    }
}