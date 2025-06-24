namespace RLoggerLib.Settings
{
    public class ConsoleSettings
    {
        public static ConsoleSettings Default { get; } = new ConsoleSettings();

        /// <summary>
        /// Log levels will be colored on the console.
        /// </summary>
        public bool UseColoredLogLevels { get; set; } = true;
    }
}
