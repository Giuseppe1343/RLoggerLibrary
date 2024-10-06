using System;
using System.IO;

namespace RLoggerThread
{
    public class RLoggerThreadCreationOptions
    {
        /// <summary>
        /// The log targets that the logger will use.
        /// </summary>
        public LogTarget LogTargets { get; set; } = LogTarget.Debug | LogTarget.Console;

        /// <summary>
        /// The minimum required severity to save the log to the database.
        /// </summary>
        public LogType DbMinRequiredSeverity { get; set; } = LogType.Warning;

        /// <summary>
        /// The directory where the log database will be created.
        /// </summary>
        public string DbDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "Log" + Path.DirectorySeparatorChar;

        /// <summary>
        /// The name of the log database file without extension.
        /// </summary>
        public string DbFileName { get; set; } = "log";


        #region File Settings
        /// <summary>
        /// The directory where the log files will be saved.
        /// </summary>
        public string LogFileDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "Log" + Path.DirectorySeparatorChar;
        #endregion

        #region Mail Settings
        /// <summary>
        /// The minimum required severity to send mail.
        /// </summary>
        public LogType MailMinRequiredSeverity { get; set; } = LogType.Error;

        /// <summary>
        /// The maximum repeated error count to send mail. After this count, the mail will not be sent in the same day.
        /// </summary>
        public int MailMaxRepeatedErrorCount { get; set; } = 2;

        /// <summary>
        /// The mail server (host) that will be used to send the mail.
        /// </summary>
        public string MailServer { get; set; }

        /// <summary>
        /// The port of the mail server that will be used to send the mail.
        /// </summary>
        public int MailPort { get; set; }

        /// <summary>
        /// The mail addresses that the mail will be sent.
        /// </summary>
        public string[] MailTo { get; set; } 

        /// <summary>
        /// The name of the sender that will be shown in the mail.
        /// </summary>
        public string MailSenderName { get; set; } = "RLogger";

        /// <summary>
        /// The mail address that will be used to send the mail.
        /// </summary>
        public string MailUser { get; set; }

        /// <summary>
        /// The password of the mail address that will be used to send the mail.
        /// </summary>
        public string MailPassword { get; set; }
        #endregion

        /// <summary>
        /// The options for creating the log database.
        /// </summary>
        internal LogDatabaseCreationOptions DatabaseCreationOptions => new(DbDirectory, DbFileName);
    }
}
