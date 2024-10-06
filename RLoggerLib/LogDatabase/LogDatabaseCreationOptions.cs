using RLoggerLib.LoggingTargets;
using System;
using System.IO;

namespace RLoggerLib
{
    public enum DatabaseFileNameDateSuffix
    {
        /// <summary>
        /// Database file name is constant.
        /// </summary>
        None,

        /// <summary>
        /// Database file name will have the current year suffix.
        /// </summary>
        Year,

        /// <summary>
        /// Database file name will have the current year and month suffix.
        /// </summary>
        YearMonth,

        /// <summary>
        /// Database file name will have the current year, month and day suffix.
        /// </summary>
        YearMonthDay
    }
    public enum TableCreationInterval
    {
        /// <summary>
        /// The table will be created only once.
        /// </summary>
        None,

        /// <summary>
        /// The table will be created daily.
        /// </summary>
        Daily,

        /// <summary>
        /// The table will be created weekly.
        /// </summary>
        Weekly,

        /// <summary>
        /// The table will be created monthly.
        /// </summary>
        Monthly
    }
    /// <summary>
    /// Options for creating the <see cref="LogDatabase"/> in <see cref="RLogger"/>.
    /// </summary>
    public class LogDatabaseCreationOptions
    {
        public static LogDatabaseCreationOptions Default => new();

        /// <summary>
        /// The minimum required severity for saving the log to the database.
        /// </summary>
        public LogType MinRequiredSeverityForSaving { get; set; } = LogType.Warning;

        /// <summary>
        /// The directory where the log database will be saved.
        /// Relative and absolute paths are supported.
        /// </summary>
        public string Directory { get; set; } = Helpers.DefaultLogDirectory;

        /// <summary>
        /// The Database file name wihout extension.
        /// </summary>
        public string FileName { get; set; } = "LogDB";

        /// <summary>
        /// The Database file name's date suffix.
        /// </summary>
        public DatabaseFileNameDateSuffix DateSuffix { get; set; } = DatabaseFileNameDateSuffix.None;

        /// <summary>
        /// The interval for creating tables.
        /// </summary>
        public TableCreationInterval TableCreationInterval { get; set; } = TableCreationInterval.Daily;
    }
}
