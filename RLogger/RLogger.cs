using RLogger.Loggers;
using RLogger.Targets;

namespace RLogger
{
    public static class RLogger
    {
        public enum LogLevel
        {
            Trace,
            Debug,
            Information,
            Warning,
            Error,
            Critical,
        }
        public static IRLogger Logger { get; set; }
    }

    
}
