using System;

namespace RLoggerLib.Settings
{
    public enum CreationInterval
    {
        Infinite = 0,
        Yearly = 1,
        Monthly = 2,
        Daily = 3,
        Hourly = 4,
        Minutely = 5
    }
    public enum SizeLimitPolicy
    {
        StopLoggingAndWaitForNextInterval = 0,
        CreateNewFileAndContinueLogging = 1
    }
    public class TextFileSettings
    {
        /// <summary>
        /// The directory where the log files will be saved.
        /// </summary>
        public string Directory { get; set; } = "logs";

        /// <summary>
        /// The log file name.
        /// </summary>
        public string LogName { get; set; }

        /// <summary>
        /// The log file name builder for dynamic log names.
        /// </summary>
        public Func<string>? LogNameBuilder { get; set; }

        /// <summary>
        /// The creation interval of the log file.
        /// </summary>
        public CreationInterval FileCreationInterval { get; set; } = CreationInterval.Daily;

        /// <summary>
        /// The maximum number of log files to retain. <b>0</b> means no limit.
        /// </summary>
        public int RetainedFileCountLimit { get; set; } = 0;

        #region Not Implemented Yet
        /// <summary>
        /// The maximum size of the log file in bytes. <b>0</b> means no limit.
        /// </summary>
        // public long FileSizeLimit { get; set; } = 0;

        /// <summary>
        /// The policy to apply when the log file reaches the <see cref="FileSizeLimit"/>. <br/>
        /// </summary>
        /// <remarks>
        /// If <see cref="FileSizeLimit"/> is <b>0</b>, this property is ignored.
        /// </remarks>
        // public SizeLimitPolicy FileSizeLimitPolicy { get; set; } = SizeLimitPolicy.CreateNewFileAndContinueLogging;
        #endregion

        /// <summary>
        /// Initializes a new text file settings with the specified log name.
        /// </summary>
        /// <param name="logName"> Log name </param>
        public TextFileSettings(string logName) 
        {
            LogName = logName;
            LogNameBuilder = null;
        }

        /// <summary>
        /// Initializes a new text file settings with the specified log name builder for dynamic log naming.
        /// </summary>
        /// <param name="logNameBuilder"> Function that returns the log name. </param>
        public TextFileSettings(Func<string> logNameBuilder)
        {
            LogNameBuilder = logNameBuilder;
            LogName = "";
        }
    }
}
