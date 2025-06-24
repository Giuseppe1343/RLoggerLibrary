namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Interface for the logging target. <br/>
    /// You can create custom logging targets by implementing this interface.
    /// </summary>
    public interface ILogTarget
    {
        /// <summary>
        /// Gets the unique name of the logging target.
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// Log the specified log entity.
        /// </summary>
        /// <param name="log"> The log entity to be logged. </param>
        void Log(RLog log);
    }
}
