using RLogger.Formatters;
using System.Diagnostics;
using System.Text;

namespace RLogger.Targets
{
    internal class TraceTarget : ILogTarget
    {
        private readonly LogLevel _minLogLevel;
        private readonly ILogFormatter _formatter;
        public TraceTarget(LogLevel? minLogLevel = null, ILogFormatter? formatter = null)
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
            Trace.WriteLine(builder);
        }
    }
}
