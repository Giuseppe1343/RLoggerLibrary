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
        void AddLoggingTarget(ILogTarget loggingTarget);

        /// <summary>
        /// <overloads> Log the message with the specified <paramref name="logLevel"/> type, </overloads> source, and sourceId (optional).
        /// </summary>
        /// <param name="logLevel"> The <see cref="LogLevel"/> of the log. </param>
        /// <param name="message"> The message to be logged </param>
        /// <param name="source"> The source of the log. </param>
        /// <param name="sourceId"> The id of the source. </param>
        void Log(LogLevel logLevel, string message, string source, string sourceId = "");
    }

    /// <summary>
    /// Extension methods for the <see cref="IRLogger"/> interface.
    /// </summary>
    public static partial class IRLoggerExtensions
    {
        /// <summary>
        /// Log the message as type <c><see cref="LogLevel.Trace"/></c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogTrace(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogLevel.Trace, $"{message}{Environment.NewLine}{new StackTrace(1, true).ToString().TrimEnd()}", source, sourceId);

        /// <summary>
        /// Log the message as type <c><see cref="LogLevel.Trace"/></c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogTrace(this IRLogger logger, string message)
        {
            var stackTrace = new StackTrace(1, true);
            var currMethod = stackTrace.GetFrame(0).GetMethod();
            logger.Log(LogLevel.Trace, $"{message}{Environment.NewLine}{stackTrace.ToString().TrimEnd()}", currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Debug"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogDebug(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogLevel.Debug, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Debug"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogDebug(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogLevel.Debug, message, currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Info"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogInfo(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogLevel.Info, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Info"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogInfo(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogLevel.Info, message, currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Warning"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogWarning(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogLevel.Warning, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Warning"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogWarning(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogLevel.Warning, message, currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Error"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogError(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogLevel.Error, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Error"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogError(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogLevel.Error, message, currMethod.ReflectedType.Name, currMethod.Name);
        }

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Critical"/> </c> with <inheritdoc cref="IRLogger.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLogger.Log"/>
        public static void LogCritical(this IRLogger logger, string message, string source, string sourceId = "") => logger.Log(LogLevel.Critical, message, source, sourceId);

        /// <summary>
        /// Log the message as type <c> <see cref="LogLevel.Critical"/> </c> with source as caller type and sourceId as caller method name.
        /// </summary>
        /// <param name="message"> The message to be logged. </param>
        public static void LogCritical(this IRLogger logger, string message)
        {
            var currMethod = new StackFrame(1).GetMethod();
            logger.Log(LogLevel.Critical, message, currMethod.ReflectedType.Name, currMethod.Name);
        }
    }
}
