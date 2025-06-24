using System.Diagnostics;

namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Default log target implementation for the debug logging.
    /// </summary>
    internal class DebugLogTarget : ILogTarget
    {
        public const string UNIQUE_NAME = "DefaultDebugLogTarget";
        public string UniqueName => UNIQUE_NAME;

        /// <inheritdoc/>
        public void Log(RLog log)
        {
            Debug.WriteLine(log.ToFormattedString());
        }
    }
}
