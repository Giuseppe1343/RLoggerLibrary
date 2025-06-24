namespace RLoggerLib.Settings
{
    public class MailLogTargetSettings
    {
        /// <summary>
        /// The lower limit of the log level to send mail.
        /// </summary>
        public LogLevel RequiredLevel { get; set; } = LogLevel.Error;

        /// <summary>
        /// Limit on the maximum number of duplicate logs that can be sent per day.
        /// </summary>
        /// <remarks>
        /// If the log level is lower than the <see cref="LogLevel.Warning"/>, this limit is <b>ignored</b>.
        /// </remarks>
        public int DailyLimitForDuplicateLogs { get; set; } = 1;

        /// <summary>
        /// The name of the sender that will be shown in the mail.
        /// </summary>
        public string MailSenderName { get; set; } = "RLogger";

        /// <summary>
        /// The mail addresses that the mail will be sent.
        /// </summary>
        public string[] MailTo { get; set; }

        /// <summary>
        /// The mail address that will be used to send the mail.
        /// </summary>
        public string MailUser { get; set; }

        /// <summary>
        /// The password of the mail address that will be used to send the mail.
        /// </summary>
        public string MailPassword { get; set; }

        /// <summary>
        /// The mail server (host) that will be used to send the mail.
        /// </summary>
        public string MailServer { get; set; }

        /// <summary>
        /// The port of the mail server that will be used to send the mail.
        /// </summary>
        public int MailPort { get; set; }

        /// <summary>
        /// Use SSL for the mail server.
        /// </summary>
        public bool UseSsl { get; set; } = true;
    }
}
