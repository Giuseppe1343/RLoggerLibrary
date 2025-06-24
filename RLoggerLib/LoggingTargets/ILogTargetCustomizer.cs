namespace RLoggerLib.LoggingTargets
{
    public interface ILogTargetCustomizer
    {
        /// <summary>
        /// Adds a log target to the logger.
        /// </summary>
        /// <param name="loggingTarget"> The logging target to be added. </param>
        /// <returns> Itself for chaining. </returns>
        ILogTargetCustomizer LogToTarget(ILogTarget loggingTarget);

        /// <summary>
        /// Adds debug output target to the logger. <b>NOTE:</b> Multiple calls will be ignored.
        /// </summary>
        /// <returns> Itself for chaining. </returns>
        ILogTargetCustomizer LogToDebug();

        /// <summary>
        /// Adds console output target to the logger. <b>NOTE:</b> Multiple calls will be ignored.
        /// </summary>
        /// <param name="settings"> Console settings </param>
        /// <returns> Itself for chaining. </returns>
        ILogTargetCustomizer LogToConsole(ConsoleSettings settings);

        /// <summary>
        /// Adds windows event output target to the logger. <b>NOTE:</b> Multiple calls will be ignored.
        /// </summary>
        /// <param name="settings"> Windows event log settings. </param>
        /// <returns> Itself for chaining. </returns>
        ILogTargetCustomizer LogToWindowsEvent(WindowsEventSettings settings);
    }
}
