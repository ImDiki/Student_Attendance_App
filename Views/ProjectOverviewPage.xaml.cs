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
            // လက်ရှိစနစ်၏ ဘာသာစကားအတိုင်း ပြသမည်
            ChangeLanguage(LanguageSettings.Language);
        }

        public void ChangeLanguage(bool isJapanese)
        {
            // Title & Sections
            txtOverTitle.Text = isJapanese ? "利用規約とプライバシーポリシー" : "TERMS & PRIVACY POLICY";
            lblHowToUse.Text = isJapanese ? "📖 システムの使用方法" : "📖 SYSTEM USAGE GUIDE";
            lblPrivacy.Text = isJapanese ? "🔒 データ保護とセキュリティ" : "🔒 DATA PROTECTION & SECURITY";

            // Usage Guide
            txtHowToUseContent.Text = isJapanese ?
                "1. 学生証番号と顔情報を正しく登録してください。\n2. 登校時、メイン画面のカメラで出席確認を行います。\n3. 出席データはリアルタイムで管理者に送信されます。" :
                "1. Register your Student ID and facial data correctly.\n2. Use the main camera interface for daily attendance tracking.\n3. Attendance data is securely synced with the admin dashboard.";

            // Privacy & Security Facts (Added Student Card & Anti-Spoofing)
            txtPrivacyContent.Text = isJapanese ?
                "【学生証とデータの保護】\n" +
                "・学生証は一人一枚のみ有効であり、他人のカードでのなりすましを防止します。\n" +
                "・顔認証と組み合わせることで、カード紛失時も不正利用を防ぎます。\n" +
                "【なりすまし防止技術】\n" +
                "・写真や動画による不正登録を防止する「生体検知技術」を搭載しています。\n" +
                "・データは暗号化され、本人確認の目的以外には使用されません。" :
                "【STUDENT CARD PROTECTION】\n" +
                "- Each Student Card is uniquely bound to one user to prevent identity theft.\n" +
                "- Face recognition ensures security even if the physical card is lost or stolen.\n" +
                "【ANTI-SPOOFING TECHNOLOGY】\n" +
                "- Features 'Liveness Detection' to block fraudulent attempts using photos or videos.\n" +
                "- All biometric data is encrypted and used strictly for identity verification.";

            // Agreement Labels
            txtAgreeLabel.Text = isJapanese ? "上記の規約とセキュリティ対策に同意します" : "I agree to the terms and security measures mentioned above.";
            btnProceed.Content = isJapanese ? "同意してログインへ" : "Agree & Go to Login";
        }

        private void chkAgree_Changed(object sender, RoutedEventArgs e)
        {
            btnProceed.IsEnabled = chkAgree.IsChecked == true;
        }

        private void btnProceed_Click(object sender, RoutedEventArgs e)
        {
            string title = LanguageSettings.Language ? "完了" : "Success";
            string msg = LanguageSettings.Language ?
                "登録と同意が完了しました。ログイン画面に戻ります。" :
                "Registration and Agreement completed. Returning to Login page.";

            MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Information);

            if (this.NavigationService != null)
            {
                // ဘရိုရဲ့ Login Page class နာမည်ကို သေချာစစ်ဆေးပါ
                this.NavigationService.Navigate(new LoginPage());
            }
        }
    }
}