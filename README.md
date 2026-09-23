Student Attendance System

A Windows desktop project for managing student attendance and class information. It uses separate student, teacher, and administrator screens, with English and Japanese interface text. This project is under active development.

What the application currently includes

Students: sign in, view attendance totals and percentage, see a timetable, and view their profile.

Teachers: start a class session, view attendance records, and mark a selected record present or absent with a note.

Administrators: screens for managing teachers, classes, and timetables.

Attendance entry: a scan screen accepts a student code from keyboard-style input, checks the current session, and records attendance in SQL Server.

Registration: a camera window can capture a profile photo when a webcam is available.

The timetable screen reads database entries for first-year classes when available; otherwise it displays generated example subjects. Other years also use example subjects. The QR Scan menu item is currently a placeholder, and the application does not yet decode QR codes through the camera. These areas are still being developed.

Technology

C# and .NET 8 (Windows), WPF and XAML

SQL Server LocalDB via Microsoft.Data.SqlClient

AForge.Video for webcam capture

ZXing.Net is referenced by the project but is not yet used for QR decoding in the current code

Run locally

1.On Windows, install Visual Studio 2022 with the .NET desktop development workload and SQL Server LocalDB.

2.Clone this repository and open Student_Attendance_System.sln in Visual Studio.

3.Review the connection string in Services/DBConnection.cs. It currently points to a machine-specific absolute path; change AttachDbFilename to the location of your local database file before running. Use test data rather than personal student records.

4.Restore NuGet packages, then build and run the solution.

The repository includes a LocalDB database file, but the database setup is not automated. Running the application on a different machine may require additional database configuration. A Windows runtime and LocalDB are required for end-to-end testing.

Related project

Attendance MCP Server is a separate C#/.NET project that exposes student-record lookup from a local attendance database through the Model Context Protocol. It is not a component of this WPF application.

日本語

学生出席管理のための開発中の Windows デスクトップアプリです。学生・教員・管理者向けの画面と、英語・日本語の表示を備えています。時間割には一部サンプルデータを使用しています。QR コードのカメラ読み取り機能はまだ実装されていません。実行前に Services/DBConnection.cs の接続先をローカル環境に合わせて設定してください。
