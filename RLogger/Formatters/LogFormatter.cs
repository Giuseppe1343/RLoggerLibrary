using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Formatters
{
    public delegate string ExceptionFormatterDelegate(Exception exception);
    public delegate string CallerInfoFormatterDelegate(string callerMemberName, string callerFilePath, int callerLineNumber);
    public class LogFormatter : ILogFormatter
    {
        /// <summary>
        /// The default formatter used by the logger. <br/>
        /// When a log target does not specify a formatter, this formatter will be used to format the log message. <br/>
        /// </summary>
        public static ILogFormatter DefaultFormatter { get; set; } = new LogFormatter();
        public static ExceptionFormatterDelegate ExceptionFormatter { get; set; } = static exception => $"{exception.GetType().FullName}: {exception}";
        public static CallerInfoFormatterDelegate CallerInfoFormatter { get; set; } = static (callerMemberName, callerFilePath, callerLineNumber) => $"   at {callerMemberName} in {callerFilePath}:line {callerLineNumber})";
        private LogFormatter() { }

        public virtual void FormatTimestamp(StringBuilder builder, DateTimeOffset timestamp)
        {
            builder.Append($"{timestamp:O}");
        }

        public virtual void FormatLevel(StringBuilder builder, LogLevel level)
        {
            var levelString = level switch
            {
                LogLevel.Trace => "TRACE",
                LogLevel.Debug => "DEBUG",
                LogLevel.Info => "INFO",
                LogLevel.Warning => "WARNING",
                LogLevel.Error => "ERROR",
                LogLevel.Critical => "CRITICAL",
                LogLevel.None => "NONE",
                _ => level.ToString()
            };
            builder.Append($"[{levelString}]");
        }

        public virtual void FormatCategory(StringBuilder builder, string category)
        {
            builder.Append($"({category})");
        }

        public virtual void FormatEventId(StringBuilder builder, int eventId)
        {
            builder.Append($"(Id: {eventId})");

        }

        public virtual void FormatMessage(StringBuilder builder, string message)
        {
            builder.Append(message);
        }

        public virtual void FormatLogMessage(StringBuilder builder, LogMessage logMessage)
        {
            FormatTimestamp(builder, logMessage.Timestamp);
            builder.Append(' ');
            FormatLevel(builder, logMessage.Level);
            if (!string.IsNullOrEmpty(logMessage.Category))
            {
                builder.Append(' ');
                FormatCategory(builder, logMessage.Category);
            }
            if (logMessage.EventId != 0)
            {
                builder.Append(' ');
                FormatEventId(builder, logMessage.EventId);
            }
            builder.Append(' ');
            FormatMessage(builder, logMessage.Message);
        }
    }
}
