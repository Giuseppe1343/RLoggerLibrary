using System.Diagnostics;

namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Default logging target implementation for the debug logging.
    /// </summary>
    internal class DebugLoggingTarget : ILoggingTarget
    {
        /// <inheritdoc/>
        public void Log(LogEntity logEntity)
        {
            Debug.WriteLine(logEntity.ToLogString());
        }
    }
    public static partial class IRLoggerExtensions
    {
        /// <summary>
        /// Adds debug logging to the logger.
        /// </summary>
        public static IRLogger AddDebugLogging(this IRLogger logger)
        {
            logger.AddLoggingTarget(new DebugLoggingTarget());
            return logger;
        }
    }
}
