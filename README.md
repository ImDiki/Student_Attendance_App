> [!IMPORTANT]
> Status: This project is currently under active development. Some features are being finalized and optimized.
> ステータス: このプロジェクトは現在開発中です。一部の機能は現在最適化および最終調整を行っています。
> >feedback and contributions are welcome!
現在開発段階のため、フィードバックや改善案がございましたらぜひご連絡ください！

****************************************************************************************************************************************************************************
Student Attendance System (WPF)
A modern, desktop-based attendance management system designed for educational institutions. This application features a role-based dashboard for both teachers and students, real-time QR code scanning for attendance marking, and automated timetable management.

 Key Features
📅 Advanced Timetable System
Automated Color Logic: Class periods are dynamically highlighted. Orange indicates an ongoing class, while Brown signifies completed or already started sessions.

Horizontal Grid Layout: A clean, card-based interface providing a full weekly view (Monday to Friday).

Term Sync: Synchronized data management for both Former (前期) and Latter (後期) terms across all user dashboards.

 Teacher Portal
Session Control: Teachers can initiate a specific class period, which automatically marks it as "Started" globally.

QR Code Management: Generates/activates the scanning system for students to mark their attendance.

Leave Requests: Teachers can review and track student absence requests stored in a global system.

 Student Dashboard
Real-time Statistics: Students can view their attendance percentages, total classes attended, and absent counts.

QR Scanning: A modern, web-style scanning interface to register attendance quickly.

Leave Submission: A built-in form to submit reasons for absence (欠席届) directly to the teacher.

🚀 Technical Stack
Framework: .NET / WPF (C#)

UI Design: XAML with Glassmorphism and Acrylic Sidebar accents

Libraries:

ZXing.Net: For QR code decoding and generation

AForge.Video: For camera and webcam integration

🛠️ Installation & Setup
Clone the repository: git clone https://github.com/ImDiki/Student_Attendance_App.git

Open the solution file .sln in Visual Studio 2022.

Ensure all NuGet packages (ZXing, AForge) are restored.

If a StaticResourceException occurs, ensure App.xaml contains the necessary brush resources.

Build > Rebuild Solution and Run.

Bug Fixes & Optimization (Recent Updates)
Navigation Fix: Resolved the issue where the app stayed on the scan screen; it now automatically returns to the Dashboard after a successful scan.

Resource Optimization: Fixed the InitializeComponent() crash by correcting resource keys and replacing invalid markup with {x:Null}.

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
学生出席管理システム (Student Attendance System)
学校や教育機関向けに開発された、WPF ベースのモダンな出席管理システムです。教師と学生それぞれのダッシュボード、QRコードによるリアルタイム出席確認、自動時間割管理機能を備えています。

 主な機能 (Key Features)
 高機能時間割システム (Timetable System)
自動色分けロジック: 現在の時刻に合わせて、授業の色が自動的に変化します。

オレンジ色: 現在進行中の授業。

茶色: 終了した授業、または既に開始された授業。

グリッドレイアウト: 月曜日から金曜日までの一週間分を横並びのカード形式で表示します。

学期同期: 前期と後期の切り替えが全ユーザー間で同期されます。

 教師用ポータル (Teacher Portal)
授業開始コントロール: 特定の時限の授業を開始し、全システムで「開始済み」としてマークできます。

出席状況の監視: 学生の出席、遅刻、欠席をリアルタイムで確認・修正できます。

欠席届の確認: 学生から提出された欠席理由を一覧で確認できます。

 学生用ダッシュボード (Student Dashboard)
出席統計: 自分の出席率、出席日数、欠席日数をグラフや数値で視覚的に確認できます。

QRコードスキャン: Webカメラを使用して、素早く出席登録ができるモダンなUIです。

欠席届の提出: フォームから直接、教師へ欠席理由を送信できます。

🚀 技術スタック (Technical Stack)
フレームワーク: .NET / WPF (C#)

UIデザイン: XAML (グラスモーフィズムとアクリルサイドバーアクセントを採用)

使用ライブラリ:

ZXing.Net: QRコードのデコードおよび生成

AForge.Video: カメラおよびウェブカメラの制御

🛠️ セットアップ (Setup)
リポジトリをクローン: git clone https://github.com/ImDiki/Student_Attendance_App.git

Visual Studio 2022 で .sln ファイルを開きます。

App.xaml に必要なリソース（WindowBackgroundBrush等）が含まれていることを確認してください。

ビルド > ソリューションの再ビルド を実行し、デバッグを開始します。

📝 最近の更新事項 (Recent Updates)
ナビゲーションの改善: スキャン成功後、自動的にダッシュボードに戻るように修正しました。

リソースの最適化: InitializeComponent() でのクラッシュを修正し、無効なマークアップを {x:Null} に置き換えました。
