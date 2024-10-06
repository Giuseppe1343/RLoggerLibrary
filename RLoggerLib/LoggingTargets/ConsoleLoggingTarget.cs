using System;

namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Default logging target implementation for the console logging. <br/>
    /// </summary>
    internal class ConsoleLoggingTarget : ILoggingTarget
    {
        private readonly LogType _minimumLogLevel;
        public ConsoleLoggingTarget(LogType minimumLogLevel)
        {
            _minimumLogLevel = minimumLogLevel;
        }

        /// <inheritdoc/>
        public void Log(LogEntity logEntity)
        {
            // Do not log if the log level is less than the minimum log level
            if (logEntity.LogType < _minimumLogLevel)
                return;

            // Write the log to the console
            Console.Write($"[{logEntity.LogDateTime:T}] ");
            WriteLogType(logEntity.LogType);
            Console.WriteLine($": {logEntity.Source}[{logEntity.SourceId}] - {logEntity.Message}");
        }

        /// <summary>
        /// Writes colored log type to the console.
        /// </summary>
        private void WriteLogType(LogType logType)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            switch (logType)
            {
                case LogType.Trace:
                case LogType.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogType.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Critical:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
            }
            Console.Write(logType.ToString().ToUpperInvariant());
            Console.ResetColor();
        }
    }

    public static partial class IRLoggerExtensions
    {
        /// <summary>
        /// Adds console logging to the logger.
        /// </summary>
        /// <param name="minimumLogLevel"> The minimum log level to be logged on the console. </param>
        public static IRLogger AddConsoleLogging(this IRLogger logger, LogType minimumLogLevel = LogType.Info)
        {
            logger.AddLoggingTarget(new ConsoleLoggingTarget(minimumLogLevel));
            return logger;
        }
    }
}
