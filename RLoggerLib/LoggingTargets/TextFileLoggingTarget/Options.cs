using System;

namespace RLoggerLib.LoggingTargets
{
    [Flags]
    public enum LogFileNamingConvention
    {

        /// <summary>
        /// The log file name will be the [custom name].ext
        /// </summary>
        Custom = 1,

        /// <summary>
        /// The log file name will be the [source name].ext
        /// </summary>
        Source = 2,

        /// <summary>
        /// The log file name will be the [custom name][sep][source name].ext
        /// </summary>
        CustomSource = Custom | Source,

        /// <summary>
        /// The log file name will be the source id.
        /// </summary>
        SourceId = 4,

        /// <summary>
        /// The log file name will be the [custom name][sep][source id].ext
        /// </summary>
        CustomSourceId = Custom | SourceId,

        /// <summary>
        /// The log file name will be the [source name][sep][source id].ext
        /// </summary>
        SourceSourceId = Source | SourceId,

        /// <summary>
        /// The log file name will be the [custom name][sep][source name][sep][source id].ext
        /// </summary>
        CustomSourceSourceId = Custom | Source | SourceId,

        /// <summary>
        /// The log file name will be the date.
        /// </summary>
        Date = 8,

        /// <summary>
        /// The log file name will be the [custom name][sep][date].ext
        /// </summary>
        CustomDate = Custom | Date,

        /// <summary>
        /// The log file name will be the [source name][sep][date].ext
        /// </summary>
        SourceDate = Source | Date,

        /// <summary>
        /// The log file name will be the [source id][sep][date].ext
        /// </summary>
        SourceIdDate = SourceId | Date,

        /// <summary>
        /// The log file name will be the [custom name][sep][source name][sep][date].ext
        /// </summary>
        CustomSourceDate = Custom | Source | Date,

        /// <summary>
        /// The log file name will be the [custom name][sep][source id][sep][date].ext
        /// </summary>
        CustomSourceIdDate = Custom | SourceId | Date,

        /// <summary>
        /// The log file name will be the [source name][sep][source id][sep][date].ext
        /// </summary>
        SourceSourceIdDate = Source | SourceId | Date,

        /// <summary>
        /// The log file name will be the [custom name][sep][source name][sep][source id][sep][date].ext
        /// </summary>
        CustomSourceSourceIdDate = Source | SourceId | Date | Custom
    }
    public class TextFileLoggingTargetOptions
    {
        /// <summary>
        /// The default options for the <see cref="TextFileLoggingTarget"/>.
        /// </summary>
        public static TextFileLoggingTargetOptions Default => new();

        /// <summary>
        /// The minimum required severity for saving the log to the text file.
        /// </summary>
        public LogType MinRequiredSeverity { get; set; } = LogType.Info;

        /// <summary>
        /// The directory where the log files will be saved.
        /// Relative and absolute paths are supported.
        /// </summary>
        public string Directory { get; set; } = Helpers.DefaultLogDirectory;

        /// <summary>
        /// The naming convention of the log file.
        /// </summary>
        public LogFileNamingConvention FileNamingConvention { get; set; } = LogFileNamingConvention.SourceDate;

        /// <summary>
        /// The naming convention of the log file.
        /// </summary>
        public string CustomName { get; set; } = "";

        /// <summary>
        /// The separator for the name parts.
        /// </summary>
        public string NameSeparator { get; set; } = "_";

        /// <summary>
        /// The date format for the date part of the log file name.
        /// </summary>
        public string DateFormat { get; set; } = "yyyyMMdd";
    }
}
