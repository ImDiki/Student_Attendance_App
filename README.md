# Student Attendance System

A WPF desktop application for recording and viewing student attendance intended for small school/classroom use. The app uses a LocalDB (SQL Server LocalDB) database and provides role-based views for Students, Teachers, and Administrators. The primary attendance workflow in the current codebase records attendance when a student code is scanned or entered into the scanning page.

Project Status
- Preserved portfolio snapshot. The repository contains working build artifacts and code verified to compile under .NET 8 (WPF). The UI and database interactions are implemented as code-behind in XAML pages.
- Build-verified: `dotnet build Student_Attendance_System.sln` completes successfully (see warnings below).
- Runtime-verified: none of the runtime flows (login, teacher CRUD, scanning) were executed by the maintainer during this snapshot; manual runtime testing is required.
- Partially implemented or referenced features: camera-based QR decoding or video capture components are referenced (AForge, ZXing) but may require additional configuration or platform compatibility. The scanning page currently supports typed or scanned student codes via a focused text input box.

Features by role (implemented in code)
- Student
  - The codebase contains models and student profile UI pages. Student dashboard code reads attendance counts and percentages from the `Attendance` table.
- Teacher
  - The Teacher dashboard and teacher management UI exist. Teachers can be created, updated, and deleted via `TeacherManagementPage` (teacher creation inserts a `Users` row and `Teachers` row).
  - Teachers can 'start' a class (the code uses timetable entries with Period = 99 to indicate started class) and then the `ScanPage` is used to mark attendance.
- Administrator
  - Admin pages and timetable management exist in the Views. Administrators can manage classes and teachers via the provided UI.

Attendance workflow (current implementation)
- The active `ScanPage` accepts input in a text box (intended for barcode/QR scanners that act as keyboard devices or manual entry).
- When a student code is submitted, the page looks up the `Students` table by `StudentCode`. If found and today's attendance for the current subject is not already present, the app calls `AttendanceService.MarkPresent(studentId, subject)` which inserts a row into `Attendance` with Status = 'Present'.
- Attendance statistics (present/absent counts, percentage) are computed by `AttendanceService` via SQL queries.

Screenshots
- Placeholders: add screenshots into `Assets/` (or the README) and replace the placeholders below.
  - `screenshots/login.png` — Login screen
  - `screenshots/scan.png` — Scanner input and attendance badge
  - `screenshots/teacher_management.png` — Teacher management grid

Tech stack
- C# 12 (net8.0-windows)
- WPF / XAML desktop UI
- SQL Server LocalDB via `Microsoft.Data.SqlClient`
- Password hashing: `BCrypt.Net-Next` (bcrypt)
- Camera/QR libraries referenced: `AForge`, `ZXing.Net` (these are included as package references; camera integration may require platform dependencies)
- Other packages: `System.Drawing.Common`, `ZXing.Net.Bindings.Windows.Compatibility`

Code structure
- `Views/` — XAML pages and code-behind implementing UI and direct database calls.
- `Services/` — Helper classes for database connection, authentication (`AuthService`), attendance logic (`AttendanceService`), and utilities added in Phase 1 (`PasswordHasher`, `Logger`).
- `Models/` — Plain data models used by the UI and services.
- The application follows a code-behind style: database access is synchronous and performed directly in UI event handlers and service methods. There is no formal DI container or repository pattern in the current snapshot.

Security notes
- New and reset passwords are hashed with bcrypt via `Services/PasswordHasher.cs`.
- The application also supports migrating legacy SHA256-stored password hashes to bcrypt on successful login: the authentication flow compares a computed SHA256 value to the stored hash and, if matched, re-hashes the supplied password with bcrypt and updates the `Users.PasswordHash` column. The migration update is executed after the original reader is closed to avoid concurrency issues.
- SQL queries use parameterized `SqlCommand` parameters in many places; some remaining calls still use `AddWithValue`. Not every query has been exhaustively converted to typed parameters.
- The DB connection uses a local `AttachDbFilename` resolved at runtime relative to the app base directory. Do not assume the repository history is free of large or sensitive artifacts: older commits in this repository tracked `Database/mainlineDB.mdf` and `Database/mainlineDB_log.ldf`. Review history before publishing and remove any secrets if present.

Local setup (developer)
Prerequisites
- .NET SDK 8.0 (matching `TargetFramework` net8.0-windows)
- Visual Studio 2022/2023 or `dotnet` CLI for building WPF projects

Clone and build
1. git clone https://github.com/ImDiki/Student_Attendance_App.git
2. cd Student_Attendance_App
3. dotnet restore
4. dotnet build Student_Attendance_System.sln

Database file and local configuration
 - The project expects a LocalDB `.mdf` file at `Database/mainlineDB.mdf` relative to the app base directory. Note: older commits in this repository tracked `Database/mainlineDB.mdf` and `Database/mainlineDB_log.ldf`; a pending cleanup commit will remove those files from the current branch. A fresh clone will therefore need a locally supplied LocalDB database with the expected schema. Runtime setup and login on a fresh clone have not been verified and require manual configuration and testing.
 - Do not commit personal database files or credentials.

Run
- Launch from Visual Studio or run the project using `dotnet run --project Student_Attendance_System.csproj` (ensure LocalDB is accessible and the database file path resolves).

Testing and verification
- Build command used: `dotnet build Student_Attendance_System.sln` (build succeeded during this snapshot).
- Known warning categories observed during build:
  - NU1701: package compatibility warnings for `AForge` packages targeting .NET Framework
  - CS86xx series: nullable-reference warnings in several code-behind files (existing codebase warnings)
- Runtime behaviors (login, teacher CRUD, scanning badge) require manual testing since credentials and local DB were not executed during this snapshot.

Known limitations and future work
- Camera-based QR scanning is referenced but may not be fully integrated or tested on all platforms.
- Many database calls are synchronous and in code-behind; refactoring to async/await and a repository or service layer would improve maintainability.
- Nullable-reference warnings should be addressed to reduce runtime null reference risks.
- Verify all SQL usage is typed (avoid `AddWithValue`) and ensure database column sizes are sufficient for bcrypt hashes.

License / Attribution
- This repository contains original code by the author. Third-party libraries are referenced via NuGet (see project file). Do not redistribute third-party binaries without their licenses.

---

If you want, run the project locally and follow the `ScanPage` to test marking attendance with an existing `Students` table and student codes.
