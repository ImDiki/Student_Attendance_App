using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Student_Attendance_System.Views
{
    // Team Member Data Model
    public class TeamMember
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string ImagePath { get; set; }
        public string Major { get; set; }
        public string ClassInfo { get; set; }
    }

    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadStaticTeamMembers();
        }

        private void LoadStaticTeamMembers()
        {
            List<TeamMember> members = new List<TeamMember>();

            // 1. MYAT THADAR LINN
            members.Add(new TeamMember
            {
                Name = "MYAT THADAR LINN",
                Role = "Lead Developer",
                ImagePath = "/Resources/diki.jpg",
                Major = "システムエンジニア専攻 3年",
                ClassInfo = "1C11KS"
            });

            // 2. KHINE ZIN MAR LYNN
            members.Add(new TeamMember
            {
                Name = "KHINE ZIN MAR LYNN",
                Role = "UI/UX Designer",
                ImagePath = "/Resources/khine.jpg",
                Major = "Web・グラフィックデザイン専攻 3年",
                ClassInfo = "1A25KS"
            });

            // 3. SI THU BO SANN
            members.Add(new TeamMember
            {
                Name = "SI THU BO SANN",
                Role = "Backend Developer",
                ImagePath = "/Resources/bosann.jpg",
                Major = "システムエンジニア専攻 3年",
                ClassInfo = "1C04KS"
            });

            TeamItemsControl.ItemsSource = members;
        }

        // Back Button Logic (Go back to Main Menu)
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // MainMenuPage 
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}