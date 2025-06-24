using RLogger.Formatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Targets
{
    internal class ColoredConsoleTarget : ISyncLogTarget
    {
        private readonly ILogFormatter<(string DateTime, string Level, string Id, string Message)> _formatter;
        public ColoredConsoleTarget(ILogFormatter<(string DateTime, string Level, string Id, string Message)>? formatter = null)
        {
            _formatter = formatter ?? SplittedStringFormatter.Instance;
        }
        public void Log(LogMessage logMessage)
        {
            var (dateTime, level, id, message) = _formatter.ApplyFormat(logMessage);
            // Set console color based on log level
            Console.Write(dateTime);
            switch (logMessage.Level)
            {
                case RLogger.LogLevel.Trace:
                case RLogger.LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case RLogger.LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case RLogger.LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case RLogger.LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case RLogger.LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.Write(level);
            Console.ResetColor();
            Console.Write(id);
            Console.WriteLine(message);
        }
    }
}
