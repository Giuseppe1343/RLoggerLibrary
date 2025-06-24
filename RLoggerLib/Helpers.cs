using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RLoggerLib
{
    internal static class Helpers
    {
        internal const string MULTIPLE_REGISTER_EXCEPTION_MESSAGE = "The logger can be registered by calling it only once.";
        internal const string REGISTERED_FROM_NON_MAIN_THREAD_EXCEPTION_MESSAGE = "The logger must be registered from the main thread. Thread.CurrentThread.ManagedThreadId must be 1.";
        internal const string UNREGISTERED_EXCEPTION_MESSAGE = "To create the logger you need to call RegisterMainThread() from the main thread.";
        internal const string INSTANCE_NOT_CREATED_EXCEPTION_MESSAGE = "The logger is not created. Please Create() it first.";
        internal const string INSTANCE_TERMINATED_EXCEPTION_MESSAGE = "This logger instance has been terminated. Make sure to create a new instance and get the logger from RLoggerThread.Instance property.";

        public static string DefaultLogDirectory => AppDomain.CurrentDomain.BaseDirectory + "Logs";
        public static string InternalLogDirectory => AppDomain.CurrentDomain.BaseDirectory + "Logs" + Path.DirectorySeparatorChar + "InternalLogs";


        public static void TestFilePath(string directoryPath, string filePath, string extension = ".txt")
        {
            Directory.CreateDirectory(directoryPath);
            string pathForValidityCheck = directoryPath + filePath + "__" + extension;
            File.WriteAllText(pathForValidityCheck, "Path And Name Validity Check");
            File.Delete(pathForValidityCheck);
        }

        public static string NormalizeDirectoryPath(this string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                return DefaultLogDirectory;

            string result = Path.GetFullPath(directoryPath);
            if (result[result.Length - 1] == Path.DirectorySeparatorChar)
                return result;
            return result + Path.DirectorySeparatorChar;
        }

        public static string NormalizeFileName(this string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "Log";

            int iLen = fileName.Length;
            char[] oChars = new char[iLen];
            int oLen = 0;
            for (int i = 0; i < iLen; i++)
            {
                char c = fileName[i];

                if (!Path.GetInvalidFileNameChars().Contains(c))
                    oChars[oLen++] = c;
            }
            return new string(oChars, 0, oLen);
        }
    }
}
