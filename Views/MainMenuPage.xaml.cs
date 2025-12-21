using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Student_Attendance_System.Views
{
    public partial class MainMenuPage : Page
    {
        private bool isDark = true;

        public MainMenuPage()
        {
            InitializeComponent();
            SetThemeColors(true); // Start with Dark Mode
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new LoginPage());
        private void btnRegister_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new RegisterPage());
        private void btnAdmin_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new DashboardPage());

        private void btnTheme_Click(object sender, RoutedEventArgs e)
        {
            isDark = !isDark;
            SetThemeColors(isDark);
            btnTheme.Content = isDark ? "☀️ Light Mode" : "🌙 Dark Mode";
        }

        private void SetThemeColors(bool dark)
        {
            if (dark)
            {
                // DARK MODE (Neon Style)
                SetResourceColor("AppBackground", Color.FromRgb(5, 5, 10));
                SetResourceColor("SurfaceColor", Color.FromRgb(20, 20, 25));
                SetResourceColor("PrimaryText", Colors.White);
                SetResourceColor("SecondaryText", Color.FromRgb(180, 180, 180));
                SetResourceColor("AccentColor", Color.FromRgb(0, 255, 255)); // Cyan
                SetResourceColor("AccentColor2", Color.FromRgb(255, 0, 85)); // Pink

                // Manually update the Glow Gradient Color
                if (GlowColorStop != null) GlowColorStop.Color = Color.FromRgb(0, 255, 255);
            }
            else
            {
                // LIGHT MODE (Clean Style)
                SetResourceColor("AppBackground", Color.FromRgb(240, 242, 245));
                SetResourceColor("SurfaceColor", Colors.White);
                SetResourceColor("PrimaryText", Color.FromRgb(40, 40, 40));
                SetResourceColor("SecondaryText", Color.FromRgb(100, 100, 100));
                SetResourceColor("AccentColor", Color.FromRgb(0, 86, 210));  // Royal Blue
                SetResourceColor("AccentColor2", Color.FromRgb(210, 0, 86)); // Deep Rose

                // Manually update the Glow Gradient Color
                if (GlowColorStop != null) GlowColorStop.Color = Color.FromRgb(0, 86, 210);
            }
        }

        private void SetResourceColor(string key, Color color)
        {
            Application.Current.Resources[key] = new SolidColorBrush(color);
        }
    }
}