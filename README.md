Student Attendance System

A desktop application for managing classes and attendance

C# · .NET 8 · WPF · SQL Server LocalDB

Student Attendance System provides separate screens for students, teachers, and administrators. Its interface includes English and Japanese text. The project is under active development.

Features

Student

Sign in and view attendance totals, attendance percentage, timetable, and profile.

Teacher

Start a class session and view attendance records.

Mark a selected record present or absent and add a note.

Administrator

Access screens for teacher, class, and timetable management.

Attendance and registration

Enter a student code through the scan screen to record attendance for an active session.

Capture a profile photo during registration when a webcam is available.

Current limitations

The timetable shows database entries for first-year classes when available. It otherwise generates example subjects; other years also use example subjects.

The QR Scan menu item is a placeholder. The application does not yet decode QR codes through a camera. Attendance entry currently accepts keyboard-style student-code input.

The database connection uses a machine-specific file path, so local setup is required before the app can run on another computer.

Tech stack

Desktop: C#, .NET 8 for Windows, WPF, XAML

Database: SQL Server LocalDB, Microsoft.Data.SqlClient

Camera: AForge.Video for profile-photo capture

QR library: ZXing.Net is referenced but is not yet used for QR decoding

Run locally

On Windows, install Visual Studio 2022 with the .NET desktop development workload and SQL Server LocalDB.

Clone the repository and open Student_Attendance_System.sln in Visual Studio.

In Services/DBConnection.cs, set AttachDbFilename to the location of your local database file. Use test data instead of personal student records.

Restore NuGet packages, then build and run the solution.

The repository contains a LocalDB database file, but database setup is not automated. Additional local configuration may be needed.

Related project

Attendance MCP Server is a separate C#/.NET project for looking up student records in a local attendance database through the Model Context Protocol. It is not part of this WPF application.

日本語概要

学生・教員・管理者向けの画面を備えた、開発中の出席管理デスクトップアプリです。出席記録、時間割表示、プロフィール写真の撮影に対応しています。時間割の一部はサンプルデータを使用しており、カメラによる QR コード読み取りはまだ実装されていません。実行前に Services/DBConnection.cs の接続先をローカル環境に合わせて設定してください。
