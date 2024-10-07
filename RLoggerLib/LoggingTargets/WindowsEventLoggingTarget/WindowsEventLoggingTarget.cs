using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace RLoggerLib.LoggingTargets
{
    internal class WindowsEventLoggingTarget : ILoggingTarget
    {
        private readonly string _internalErrorLogFilePath = $"{Helpers.DefaultLogDirectory.NormalizeDirectoryPath()}WindowsEventLogError_{DateTime.Today:yyyyMMdd}.log";

        private readonly string _logName = AppDomain.CurrentDomain.FriendlyName;

        private readonly WindowsEventLoggingTargetOptions _options;
        public WindowsEventLoggingTarget(WindowsEventLoggingTargetOptions options)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException("WindowsEventLoggingTarget is only supported on Windows.");

            _options = options;
        }

        public void Log(LogEntity logEntity)
        {
            // If the log type is less than the minimum required severity, do not log.
            if (logEntity.LogType < _options.MinRequiredSeverity)
                return;

            try
            {
                // Create the event source if it does not exist
                string source = _options.UseLogSourceAsEventSource ? logEntity.Source : _options.EventSource;
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, _logName);

                // Set the event id
                int eventId = _options.EventId;
                if (_options.UseLogSourceIdAsEventId && int.TryParse(logEntity.SourceId, out int sourceId))
                    eventId = sourceId;

                // Write the log to the event log
                EventLog.WriteEntry(source, logEntity.ToLogString(), ConvertEventLogEntryType(logEntity.LogType), eventId);
            }
            catch (Exception ex)
            {
                Directory.CreateDirectory(Helpers.DefaultLogDirectory);

                // Append the error to the mail send error log
                File.AppendAllText(_internalErrorLogFilePath, $"[{DateTime.Now:T}] {LogType.Error}: RLogger.LoggingTargets.WindowsEventLoggingTarget - An error occured while logging this LogEntity ({logEntity.ToLogString()}).{Environment.NewLine}ExceptionMessage: {ex.Message}{Environment.NewLine}");
            }

        }
        private static EventLogEntryType ConvertEventLogEntryType(LogType type)
        {
            return type switch
            {
                LogType.Trace => EventLogEntryType.Information,
                LogType.Debug => EventLogEntryType.Information,
                LogType.Info => EventLogEntryType.Information,
                LogType.Warning => EventLogEntryType.Warning,
                LogType.Error => EventLogEntryType.Error,
                LogType.Critical => EventLogEntryType.Error,
                _ => EventLogEntryType.Information,
            };
        }
    }
    public static partial class IRLoggerExtensions
    {
        /// <summary>
        /// Adds windows event logging to the logger. <br/>
        /// <b>Only supported on Windows.</b>
        /// </summary>
        /// <param name="options"> The options for the windows event logging. </param>
        /// <exception cref="PlatformNotSupportedException"> Thrown when the current platform is not Windows. </exception>
        public static IRLogger AddWindowsEventLogging(this IRLogger logger, WindowsEventLoggingTargetOptions options)
        {
            logger.AddLoggingTarget(new WindowsEventLoggingTarget(options));
            return logger;
        }
    }
}
