namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Interface for the logging target. <br/>
    /// You can create custom logging targets by implementing this interface.
    /// </summary>
    public interface ILoggingTarget
    {
        /// <summary>
        /// Log the specified log entity.
        /// </summary>
        /// <param name="logEntity"> The log entity to be logged. </param>
        void Log(LogEntity logEntity);
    }
    public static partial class IRLoggerExtensions
    {
        /// <summary>
        /// Add a new logging target to the logger.
        /// </summary>
        /// <param name="loggingTarget"> The logging target to be added. </param>
        public static IRLogger AddCustomLoggingTarget(this IRLogger logger, ILoggingTarget loggingTarget)
        {
            logger.AddLoggingTarget(loggingTarget);
            return logger;
        }
    }
}
