using RLogger.FluentBuilding;
using RLogger.Loggers;
using RLogger.Targets;

namespace RLogger
{
    public static class R
    {
        public static LogLevel GlobalLogLevel { get; set; } = LogLevel.Info;
        public static IRLogger Logger { get; } = NoOpLogger.Instance;
        public static IRLoggerBuilder Builder() => RLoggerBuilder.Create();

    }
}
