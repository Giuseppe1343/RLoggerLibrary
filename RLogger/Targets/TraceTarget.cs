using RLogger.Formatters;
using System.Diagnostics;

namespace RLogger.Targets
{
    internal class TraceTarget : ISyncLogTarget
    {
        private readonly ILogFormatter<string> _formatter;
        public TraceTarget(ILogFormatter<string>? formatter = null)
        {
            _formatter = formatter ?? DefaultStringFormatter.Instance;
        }

        public void Log(LogMessage logMessage)
        {
            Trace.WriteLine(_formatter.ApplyFormat(logMessage));
        }
    }
}
