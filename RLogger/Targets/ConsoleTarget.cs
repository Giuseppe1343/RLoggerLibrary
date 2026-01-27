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
        private readonly ILogFormatter<string> _formatter;
        public ConsoleTarget(ILogFormatter<string>? formatter = null)
        {
            _formatter = formatter ?? DefaultStringFormatter.Instance;
        }
        public void Log(LogMessage logMessage)
        {
            Console.WriteLine(_formatter.ApplyFormat(logMessage));
        }
    }
}
