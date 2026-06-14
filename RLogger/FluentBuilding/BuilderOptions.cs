
using RLogger.Formatters;

namespace RLogger.FluentBuilding
{
    public class TargetOptions
    {
        public LogLevel? MinLogLevel { get; set; }
        public ILogFormatter? Formatter { get; set; }
    }
}
