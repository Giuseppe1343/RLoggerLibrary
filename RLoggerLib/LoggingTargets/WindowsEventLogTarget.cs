using System;
using System.Diagnostics;

namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Default log target implementation for the Windows event logging.
    /// </summary>
    internal class WindowsEventLogTarget : ILogTarget
    {
        public const string UNIQUE_NAME = "DefaultWindowsEventLogTarget";
        public string UniqueName => UNIQUE_NAME;

        private readonly WindowsEventSettings _windowsEventSettings;
        public WindowsEventLogTarget(WindowsEventSettings windowsEventSettings)
        {
            _windowsEventSettings = windowsEventSettings;

            // Create the event source if it does not exist
            if (!EventLog.SourceExists(_windowsEventSettings.EventSource))
            {
                EventLog.CreateEventSource(_windowsEventSettings.EventSource, _windowsEventSettings.EventLogName);
            }
            // If the event source exists but is not in the correct log, delete it and create it again
            else if (EventLog.LogNameFromSourceName(_windowsEventSettings.EventSource, ".") != _windowsEventSettings.EventLogName)
            {
                EventLog.DeleteEventSource(_windowsEventSettings.EventSource);
                EventLog.CreateEventSource(_windowsEventSettings.EventSource, _windowsEventSettings.EventLogName);
            }
        }

        public void Log(RLog log)
        {
            // Write the log to the event log
            EventLog.WriteEntry(_windowsEventSettings.EventSource, log.ToFormattedString(), ConvertEventLogEntryType(log.LogLevel), log.EventId);

        }
        private static EventLogEntryType ConvertEventLogEntryType(LogLevel type)
        {
            return type switch
            {
                LogLevel.Trace => EventLogEntryType.Information,
                LogLevel.Debug => EventLogEntryType.Information,
                LogLevel.Info => EventLogEntryType.Information,
                LogLevel.Warning => EventLogEntryType.Warning,
                LogLevel.Error => EventLogEntryType.Error,
                LogLevel.Critical => EventLogEntryType.Error,
                _ => EventLogEntryType.Information,
            };
        }
    }
}
