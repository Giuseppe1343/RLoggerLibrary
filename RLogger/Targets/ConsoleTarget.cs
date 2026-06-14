using RLogger.Formatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Targets
{
    internal class ConsoleTarget : ILogTarget
    {
        private readonly LogLevel _minLogLevel;
        private readonly ILogFormatter _formatter;
        public ConsoleTarget(LogLevel? minLogLevel = null, ILogFormatter? formatter = null)
        {
            _minLogLevel = minLogLevel ?? R.GlobalLogLevel;
            _formatter = formatter ?? LogFormatter.DefaultFormatter;
        }
        public void Log(LogMessage logMessage)
        {
            if (logMessage.Level < _minLogLevel)
                return;
            var builder = new StringBuilder();
            _formatter.FormatLogMessage(builder, logMessage);
            Console.Out.Write(builder);
            Console.Out.WriteLine();
        }
    }
}
