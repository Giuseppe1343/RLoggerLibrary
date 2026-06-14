using RLogger.Formatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Targets
{
    internal class ColoredConsoleTarget : ILogTarget
    {
        private readonly LogLevel _minLogLevel;
        private readonly ILogFormatter _formatter;
        public ColoredConsoleTarget(LogLevel? minLogLevel = null, ILogFormatter? formatter = null)
        {
            _minLogLevel = minLogLevel ?? R.GlobalLogLevel;
            _formatter = formatter ?? LogFormatter.DefaultFormatter;
        }
        public void Log(LogMessage logMessage)
        {
            if (logMessage.Level < _minLogLevel)
                return;

            var builder = new StringBuilder();

            // Format timestamp
            _formatter.FormatTimestamp(builder, logMessage.Timestamp);
            Console.Out.Write(builder);
            builder.Length = 0;

            // Format log level and set console color
            _formatter.FormatLevel(builder, logMessage.Level);
            switch (logMessage.Level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.Out.Write(builder);
            builder.Length = 0;
            Console.ResetColor();

            // Format rest of the log message
            if (!string.IsNullOrEmpty(logMessage.Category))
            {
                builder.Append(' ');
                _formatter.FormatCategory(builder, logMessage.Category);
            }
            if (logMessage.EventId != 0)
            {
                builder.Append(' ');
                _formatter.FormatEventId(builder, logMessage.EventId);
            }
            builder.Append(' ');
            _formatter.FormatMessage(builder, logMessage.Message);
            Console.Out.Write(builder);
            Console.Out.WriteLine();
        }
    }
}
