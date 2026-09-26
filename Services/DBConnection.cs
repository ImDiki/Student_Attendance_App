using System;
using System;
using System.IO;
using Microsoft.Data.SqlClient;

namespace Student_Attendance_System.Views
{
    public static class DBConnection
    {
        // Build a portable connection string that uses the Database\mainlineDB.mdf
        // located relative to the application's base directory.
        private static readonly string connectionString;

        static DBConnection()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
                string dbPath = Path.Combine(baseDir, "Database", "mainlineDB.mdf");

                // If the exact file isn't present in the output directory, also try repository-relative path
                if (!File.Exists(dbPath))
                {
                    // repository layout: when running from IDE the base dir may be bin/Debug/net.../,
                    // so walk up to find repository root that contains Database folder.
                    string dir = baseDir;
                    for (int i = 0; i < 6; i++)
                    {
                        string candidate = Path.Combine(dir, "Database", "mainlineDB.mdf");
                        if (File.Exists(candidate)) { dbPath = candidate; break; }
                        dir = Path.GetDirectoryName(dir) ?? dir;
                    }
                }

                connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True;Connect Timeout=30";
            }
            catch
            {
                // fallback to a minimal connection string; the caller will get exception when using it
                connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30";
            }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
