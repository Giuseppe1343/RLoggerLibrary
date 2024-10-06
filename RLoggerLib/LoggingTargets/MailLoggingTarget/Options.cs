namespace RLoggerLib.LoggingTargets
{
    public class MailLoggingTargetOptions
    {
        /// <summary>
        /// The minimum required severity to send mail.
        /// </summary>
        public LogType MinRequiredSeverity { get; set; } = LogType.Error;

        /// <summary>
        /// The maximum repeated error count to send mail. After this count, the mail will not be sent in the same day.
        /// </summary>
        public int MailMaxRepeatedErrorCount { get; set; } = 1;

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
    }
}
