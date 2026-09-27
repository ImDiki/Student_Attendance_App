using System.Windows;
using System.Windows.Controls;
using Student_Attendance_System.Interfaces;
using Student_Attendance_System.Models;

namespace Student_Attendance_System.Views
{
    public partial class ProjectOverviewPage : Page, ILanguageSwitchable
    {
        public ProjectOverviewPage()
        {
            InitializeComponent();
            ChangeLanguage(LanguageSettings.Language);
        }

        public void ChangeLanguage(bool isJapanese)
        {
            txtOverTitle.Text = isJapanese ? "システム権限とガイド" : "SYSTEM GUIDE & PRIVACY";
            lblAttendance.Text = isJapanese ? "✅ 出席確認プロセス" : "✅ ATTENDANCE CONFIRMATION PROCESS";
            lblRoles.Text = isJapanese ? "👥 ユーザー権限" : "👥 USER ROLES & PERMISSIONS";
            lblPrivacy.Text = isJapanese ? "🔒 セキュリティとプライバシー" : "🔒 DATA PRIVACY & SECURITY";

            txtAttendanceContent.Text = isJapanese
                ? "1. 講師が授業を開始すると、出席入力が可能になります。\n" +
                  "2. 学生は学籍番号をスキャナーまたは入力欄から送信して出席を記録します。\n" +
                  "3. 登録済みの出席は重複して記録されません。"
                : "1. Attendance input is enabled after the teacher starts the class session.\n" +
                  "2. Students submit their student code using a keyboard-style scanner or the input field.\n" +
                  "3. Attendance already recorded for the current class is not recorded twice.";

            txtRolesContent.Text = isJapanese
                ? "【管理者】学生・講師・時間割などの管理画面を利用します。\n" +
                  "【講師】授業の開始と出席情報の確認・管理を行います。\n" +
                  "【学生】自分の情報を確認し、学籍番号による出席登録を行います。"
                : "【ADMINISTRATOR】Uses management screens for students, teachers, and timetable data.\n" +
                  "【TEACHER】Starts class sessions and reviews or manages attendance information.\n" +
                  "【STUDENT】Views personal information and records attendance using a student code.";

            txtPrivacyContent.Text = isJapanese
                ? "・パスワードを忘れた場合は管理者にリセットを依頼してください。\n" +
                  "・パスワードはハッシュ化して保存されます。学生写真を登録する場合は、ローカルの学校用データベースに保存されます。"
                : "- FORGOT PASSWORD: Contact the administrator for a manual reset.\n" +
                  "- Passwords are stored as hashes. If a student photo is registered, it is stored in the local school database.";

            txtAgreeLabel.Text = isJapanese ? "システムガイドと規約に同意します" : "I agree to the system guide and security terms.";
            btnProceed.Content = isJapanese ? "同意してログインへ" : "Agree & Go to Login";
        }

        private void chkAgree_Changed(object sender, RoutedEventArgs e)
        {
            btnProceed.IsEnabled = chkAgree.IsChecked == true;
        }

        private void btnProceed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginPage());
        }
    }
}
