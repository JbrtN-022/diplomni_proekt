using System;
using System.IO;

namespace rccs_new.MyClass
{
    internal class HistoryLogger
    {
        private static readonly string folderPath =
            Path.Combine(
                Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)
                .Parent
                .Parent
                .FullName,
                "history");

        private static readonly string filePath =
            Path.Combine(folderPath, "history.txt");

        public static void Log(string message)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string logMessage =
                    $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}";

                File.AppendAllText(
                    filePath,
                    logMessage + Environment.NewLine,
                    System.Text.Encoding.UTF8);
            }
            catch
            {

            }
        }
    }
}