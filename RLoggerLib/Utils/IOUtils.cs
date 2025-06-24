using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RLoggerLib.Utils
{
    internal static class IOUtils
    {
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string InternalLogDirectory = Path.Combine(BaseDirectory, "logs", "internal");
        public static string InternalLogFile
        {
            get
            {
                Directory.CreateDirectory(InternalLogDirectory);
                return Path.Combine(InternalLogDirectory, $"internal_{TimeUtils.Today:yyyy-MM-dd}.log");
            }
        }
        public static string GetFullPath(string filePath) => Path.Combine(BaseDirectory, filePath);
        public static string GetFullPath(string filePath, string directory) => Path.Combine(BaseDirectory, directory, filePath);

        public static void EnsureDirectory(string directory) => Directory.CreateDirectory(GetFullPath(directory));

        public static void TestFileOperations(string directory, string fileName)
        {
            var path = Path.Combine(directory, fileName);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.WriteByte(0);
                fs.Close();
            }
            File.Delete(path);
        }
    }
}
