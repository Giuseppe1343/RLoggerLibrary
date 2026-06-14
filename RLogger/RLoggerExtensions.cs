using RLogger.Loggers;
using System.Runtime.CompilerServices;

namespace RLogger
{
    public static class RLoggerExtensions
    {
        public static void LogTrace(this IRLogger logger, string category, int eventId, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Trace, category, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogTrace(this IRLogger logger, string category, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
             => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Trace, category, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogTrace(this IRLogger logger, int eventId, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
             => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Trace, string.Empty, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogTrace(this IRLogger logger, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Trace, string.Empty, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));

        public static void LogDebug(this IRLogger logger, string category, int eventId, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Debug, category, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogDebug(this IRLogger logger, string category, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
             => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Debug, category, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogDebug(this IRLogger logger, int eventId, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
             => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Debug, string.Empty, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogDebug(this IRLogger logger, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Debug, string.Empty, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));

        public static void LogInformation(this IRLogger logger, string category, int eventId, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Info, category, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogInformation(this IRLogger logger, string category, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
             => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Info, category, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogInformation(this IRLogger logger, int eventId, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Info, string.Empty, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogInformation(this IRLogger logger, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Info, string.Empty, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));

        public static void LogWarning(this IRLogger logger, string category, int eventId, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Warning, category, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogWarning(this IRLogger logger, string category, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
             => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Warning, category, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogWarning(this IRLogger logger, int eventId, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Warning, string.Empty, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));
        public static void LogWarning(this IRLogger logger, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Warning, string.Empty, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber));

        public static void LogError(this IRLogger logger, string category, int eventId, string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Error, category, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber).WithException(exception));
        public static void LogError(this IRLogger logger, string category, string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
             => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Error, category, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber).WithException(exception));
        public static void LogError(this IRLogger logger, int eventId, string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Error, string.Empty, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber).WithException(exception));
        public static void LogError(this IRLogger logger, string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Error, string.Empty, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber).WithException(exception));

        public static void LogCritical(this IRLogger logger, string category, int eventId, string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Critical, category, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber).WithException(exception));
        public static void LogCritical(this IRLogger logger, string category, string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
             => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Critical, category, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber).WithException(exception));
        public static void LogCritical(this IRLogger logger, int eventId, string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Critical, string.Empty, eventId, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber).WithException(exception));
        public static void LogCritical(this IRLogger logger, string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            => logger.Log(new LogMessage(DateTimeOffset.Now, LogLevel.Critical, string.Empty, 0, message).WithCallerInfo(callerMemberName, callerFilePath, callerLineNumber).WithException(exception));
    }
}
