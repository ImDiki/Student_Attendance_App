using System;
using System.IO;

namespace Student_Attendance_System.Services
{
    public static class Logger
    {
        private static readonly object _sync = new object();
        private static readonly string _logFile;

        static Logger()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
            string logDir = Path.Combine(baseDir, "logs");
            try
            {
                Directory.CreateDirectory(logDir);
            }
            catch { /* ignore */ }
            _logFile = Path.Combine(logDir, "app_errors.log");
        }

        public static void LogError(string message, Exception? ex = null)
        {
            try
            {
                lock (_sync)
                {
                    File.AppendAllText(_logFile, $"[{DateTime.UtcNow:O}] {message} {ex?.ToString()}{Environment.NewLine}");
                }
            }
            catch
            {
                // swallowing errors - logging must not bring down the app
            }
        }
    }
}
