namespace RLoggerThread
{
    /// <summary>
    /// Options for creating a new <see cref="LogDatabase"/>.
    /// </summary>
    internal class LogDatabaseCreationOptions
    {
        private const string DbExtension = ".db";

        /// <summary>
        /// The directory where the log database will be saved.
        /// </summary>
        public string LogDBDirectory { get; }

        /// <summary>
        /// The absolute path of the log database file with extension.
        /// </summary>
        public string LogDBFilePath { get; }

        /// <summary>
        /// Create a new instance of <see cref="LogDatabaseCreationOptions"/>.
        /// </summary>
        /// <param name="logDBDirectory"> The directory where the log database will be saved. </param>
        /// <param name="logDBFileName"> The name of the log database file without extension.  </param>

        public LogDatabaseCreationOptions(string logDBDirectory, string logDBFileName)
        {
            LogDBDirectory = logDBDirectory;
            LogDBFilePath = $"{LogDBDirectory}{logDBFileName}{DbExtension}";
        }
    }
}
