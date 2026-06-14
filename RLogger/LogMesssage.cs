using RLogger.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger
{
    public class LogMessage
    {
        public static readonly LogMessage Empty = new(Guid.Empty, DateTimeOffset.MinValue, LogLevel.None, string.Empty, 0, string.Empty);
        internal Guid LogId { get; }
        /// <summary>When the message was created, with timezone context.</summary>
        public DateTimeOffset Timestamp { get; }
        /// <summary>The severity level of the log message.</summary>
        public LogLevel Level { get; }
        /// <summary>The event id of the log message. This is used to group log messages together.</summary>
        public string Category { get; }
        /// <summary>The content of the log message. This should be a pre-formatted string, can contain exception, stack traces or other information. The log targets will not do any additional formatting to this message.</summary>
        public int EventId { get; }
        /// <summary>The category of the log message. This is used to group log messages together. It can be the source of the log message, such as the class or method name.</summary>
        public string Message { get; internal set; }
        internal LogMessage(Guid logId, DateTimeOffset timestamp, LogLevel logLevel, string category, int eventId, string message)
        {
            LogId = logId;
            Timestamp = timestamp;
            Level = logLevel;
            EventId = eventId;
            Category = category;
            Message = message;
        }
        public LogMessage(DateTimeOffset timestamp, LogLevel logLevel, string category, int eventId, string message) : this(Guid.NewGuid(), timestamp, logLevel, category, eventId, message)
        {
        }
    }
    public static class LogMessageExtensions
    {
        public static LogMessage WithCallerInfo(this LogMessage logMessage, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            logMessage.Message += Environment.NewLine + LogFormatter.CallerInfoFormatter(callerMemberName, callerFilePath, callerLineNumber);
            return logMessage;
        }
        public static LogMessage WithException(this LogMessage logMessage, Exception? ex)
        {
            if (ex != null)
            {
                logMessage.Message += Environment.NewLine + LogFormatter.ExceptionFormatter(ex);
            }
            return logMessage;
        }
    }
}
