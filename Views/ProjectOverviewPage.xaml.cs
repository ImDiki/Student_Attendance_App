using System;
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

            // 🔄 Attendance Process Logic
            txtAttendanceContent.Text = isJapanese ?
                "1. 講師が授業を開始（Start Class）すると、出席スキャンが可能になります。\n" +
                "2. 学生は顔認証スキャンを行い、出席を確定させます。\n" +
                "3. 遅刻者に対して、講師は「備考（Remark）」を記入し、出席または欠席を個別に判断できます。" :
                "1. Attendance scanning is only enabled after the Teacher starts the class session.\n" +
                "2. Students must perform a facial scan to confirm their arrival.\n" +
                "3. For late arrivals, Teachers can add 'Remarks' and manually verify them as Present or Absent.";

            // User Roles
            txtRolesContent.Text = isJapanese ?
                "【管理者】全ユーザーの管理（登録・編集・削除）を行います。\n" +
                "【講師】クラスの開始、遅刻者の備考入力、出席データの確定を行います。\n" +
                "【学生】スキャンによる出席登録と自身の情報の閲覧のみ可能です。" :
                "【ADMINISTRATOR】Full control over adding, editing, and deleting all accounts.\n" +
                "【TEACHER】Starts class sessions, manages late remarks, and finalizes attendance.\n" +
                "【STUDENT】Limited to facial scanning and personal data viewing.";

            // Security & Privacy
            txtPrivacyContent.Text = isJapanese ?
                "・パスワードを忘れた場合は管理者にリセットを依頼してください。\n" +
                "・データは暗号化され、出席確認以外の目的には使用されません。" :
                "- FORGET PASSWORD: If forgotten, please contact the Admin for a manual reset.\n" +
                "- All biometric and credential data is encrypted and strictly used for school attendance.";

            txtAgreeLabel.Text = isJapanese ? "システムガイドと規約に同意します" : "I agree to the system guide and security terms.";
            btnProceed.Content = isJapanese ? "同意してログインへ" : "Agree & Go to Login";
        }

        private void chkAgree_Changed(object sender, RoutedEventArgs e)
        {
            btnProceed.IsEnabled = chkAgree.IsChecked == true;
        }

        private void btnProceed_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(new LoginPage());
            }
        }
    }
}