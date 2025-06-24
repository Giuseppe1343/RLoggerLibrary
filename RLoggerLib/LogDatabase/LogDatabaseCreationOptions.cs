namespace RLoggerLib
{
    public enum DatabaseFileCreationInterval
    {
        /// <summary>
        /// The database file creation interval is not specified. Default value is <see cref="Monthly"/>.
        /// </summary>
        None,

        /// <summary>
        /// The database file will be created daily.
        /// </summary>
        Daily,

        /// <summary>
        /// The database file will be created weekly.
        /// </summary>
        Weekly,

        /// <summary>
        /// The database file will be created monthly.
        /// </summary>
        Monthly,

        /// <summary>
        /// The database file will be created yearly.
        /// </summary>
        Yearly,

        /// <summary>
        /// The database file will be created only once.
        /// </summary>
        Never
    }
    /// <summary>
    /// Options for creating the <see cref="SQLiteLogDatabase"/> in <see cref="RLogger"/>.
    /// </summary>
    public class LogDatabaseCreationOptions
    {
        public static LogDatabaseCreationOptions Default => new();

        /// <summary>
        /// The minimum required severity for saving the log to the database.
        /// </summary>
        public LogLevel MinRequiredSeverityForSaving { get; set; } = LogLevel.Info;

        /// <summary>
        /// The Database file name's date suffix.
        /// </summary>
        public DatabaseFileCreationInterval CreationInterval { get; set; }
    }
}
