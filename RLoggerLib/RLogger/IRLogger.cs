using RLoggerLib.LoggingTargets;
using System;
using System.Diagnostics;

namespace RLoggerLib
{
    /// <summary>
    /// Public interface for using <see cref="RLogger"/> in a dependency injection scenario.
    /// Service lifetime: Singleton (for the same logger instance to be used throughout the application)
    /// </summary>
    public interface IRLogger
    {
        /// <summary>
        /// Add a new logging target to the logger. <br/>
        /// You can add custom logging targets by implementing <see cref="ILoggingTarget"/>.
        /// </summary>
        /// <param name="loggingTarget"> The logging target to be added. </param>
        void AddLoggingTarget(ILoggingTarget loggingTarget);

        /// <summary>
        /// <overloads> Log the message with the specified <paramref name="logType"/> type, </overloads> source, and sourceId (optional).
        /// </summary>
        /// <param name="logType"> The <see cref="LogType"/> of the log. </param>
        /// <param name="message"> The message to be logged </param>
        /// <param name="source"> The source of the log. </param>
        /// <param name="sourceId"> The id of the source. </param>
        void Log(LogType logType, string message, string source, string sourceId = "");
    }

    /// <summary>
    /// Extension methods for the <see cref="IRLogger"/> interface.
    /// </summary>
    public static partial class IRLoggerExtensions
    {
        /// <summary>
        /// Log the message as type <c><see cref="LogType.Trace"/></c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogTrace(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogType.Trace, $"{message}{Environment.NewLine}{new StackTrace(1, true).ToString().TrimEnd()}", source, sourceId);

        /// <summary>
        /// Log the message as type <c><see cref="LogType.Trace"/></c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogTrace(this IRLogger logger, string message)
        {
            var stackTrace = new StackTrace(1, true);
            var currMethod = stackTrace.GetFrame(0).GetMethod();
            logger.Log(LogType.Trace, $"{message}{Environment.NewLine}{stackTrace.ToString().TrimEnd()}", currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Debug"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogDebug(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogType.Debug, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Debug"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogDebug(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogType.Debug, message, currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Info"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogInfo(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogType.Info, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Info"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogInfo(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogType.Info, message, currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Warning"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogWarning(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogType.Warning, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Warning"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogWarning(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogType.Warning, message, currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Error"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogError(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogType.Error, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Error"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogError(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogType.Error, message, currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Critical"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogCritical(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogType.Critical, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogType.Critical"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogCritical(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogType.Critical, message, currMethod.ReflectedType.Name, currMethod.Name);
        }
    }
}
