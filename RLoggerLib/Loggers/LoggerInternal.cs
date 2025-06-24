using Newtonsoft.Json;
using RLoggerLib.Utils;
using System;
using System.Diagnostics;
using System.IO;

namespace RLoggerLib
{
    internal static class LoggerInternal
    {
        private const string CRITICAL_MESSAGE_HEADER = "------------------------------- CRITICAL -----------------------------";
        private const string ERROR_MESSAGE_HEADER =    "-------------------------------- ERROR -------------------------------";
        private const string WARNING_MESSAGE_HEADER =  "------------------------------- WARNING ------------------------------";
        private const string MESSAGE_FOOTER = "----------------------------------------------------------------------";
        private static readonly string NewLine = Environment.NewLine;

        private readonly static bool _enabled = true;

        static LoggerInternal()
        {
            try
            {
                var file = Path.Combine(IOUtils.InternalLogDirectory, "dummy.log");
                File.WriteAllText(file, "dummy-text");
                var readed = File.ReadAllText(file);
                File.Delete(file);
                if (readed != "dummy-text")
                    throw new RLoggerLibException("Internal logger is not working properly.");
            }
            catch when (RLogger.UseInternalLogging)
            {
                _enabled = false;
            }

            if (!_enabled)
                return;

            // If logger enabled, attach to unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                if (IsLibraryException(ex))
                    Log(CRITICAL_MESSAGE_HEADER, "An Unhandled Exception Occured.", ex);
            };
        }
        public static void Log(string header, string message, Exception? ex = null, object? data = null)
        {
            if (!_enabled)
                return;

            File.AppendAllText
             (
                IOUtils.InternalLogFile,

                $"{header}{NewLine
                }Date: {TimeUtils.Today:d}{NewLine
                }Time: {TimeUtils.Now:HH:mm:ss.fff}{NewLine
                }Message: {message}{NewLine
                }{(ex is not null ? $"Exception: {NewLine}{ex}{NewLine}" : "")
                }{(data is not null ? $"Data: {NewLine}{JsonConvert.SerializeObject(data, Formatting.Indented)}{NewLine}" : "")
                }{MESSAGE_FOOTER}{NewLine}{NewLine}"
             );
        }

        public static void LogError(string message, Exception? ex = null, object? data = null) => Log(ERROR_MESSAGE_HEADER, message, ex, data);
        public static void LogWarning(string message, Exception? ex = null, object? data = null) => Log(WARNING_MESSAGE_HEADER, message, ex, data);

        private static bool IsLibraryException(Exception? ex)
        {
            if (ex is null)
                return false;

            var stackTrace = new StackTrace(ex, true);

            foreach (var frame in stackTrace.GetFrames())
            {
                var method = frame.GetMethod();
                if (method.DeclaringType.Namespace.StartsWith("RLoggerLib"))
                    return true;
            }

            return false;
        }
    }
}
