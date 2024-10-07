using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Default logging target implementation for the mail logging.
    /// </summary>
    public class MailLoggingTarget : ILoggingTarget, IDisposable
    {
        private readonly string _internalErrorLogFilePath = $"{Helpers.DefaultLogDirectory.NormalizeDirectoryPath()}MailSendError_{DateTime.Today:yyyyMMdd}.log";

        private readonly MailLoggingTargetOptions _options;

        private readonly MailMessage _eMail;
        private readonly SmtpClient _smtpClient;
        private bool _disposed;

        public MailLoggingTarget(MailLoggingTargetOptions options)
        {
            _options = options;

            // Create the mail message
            _eMail = new MailMessage()
            {
                From = new MailAddress(_options.MailUser, _options.MailSenderName),
                IsBodyHtml = true,
            };
            foreach (var receiver in _options.MailTo) _eMail.To.Add(new MailAddress(receiver));

            // Create the smtp client
            _smtpClient = new SmtpClient(_options.MailServer, _options.MailPort)
            {
                Credentials = new NetworkCredential(_options.MailUser, _options.MailPassword),
                EnableSsl = true
            };
        }

        /// <inheritdoc/>
        public void Log(LogEntity logEntity)
        {

            // If the log type is less than the minimum required severity or the repeated error count is greater than the maximum repeated error count, do not send the mail.
            if (logEntity.LogType < _options.MinRequiredSeverity || logEntity.TodaysRepetitionCount > _options.MailMaxRepeatedErrorCount)
                return;

            try
            {
                // Create the mail message
                _eMail.Subject = GetMailSubject(logEntity);
                _eMail.Body = GetMailBody(logEntity);

                // Send the mail
                _smtpClient.Send(_eMail);
            }
            catch (Exception ex)
            {
                Directory.CreateDirectory(Helpers.DefaultLogDirectory);

                // Append the error to the mail send error log
                File.AppendAllText(_internalErrorLogFilePath, $"[{DateTime.Now:T}] {LogType.Error}: RLogger.LoggingTargets.MailLoggingTarget - An error occured while sending the mail for this LogEntity ({logEntity.ToLogString()}).{Environment.NewLine}ExceptionMessage: {ex.Message}{Environment.NewLine}");
            }
        }

        protected virtual string GetMailSubject(LogEntity logEntity)
        {
            return $"{logEntity.Source} - {(string.IsNullOrWhiteSpace(logEntity.SourceId) ? "" : $"Id:{logEntity.SourceId} ")}Date:{logEntity.LogDateTime:d}";
        }

        protected virtual string GetMailBody(LogEntity logEntity)
        {
            var color = logEntity.LogType switch
            {
                LogType.Trace => "#8c8c94",
                LogType.Debug => "#8c8c94", //4e3fa8 - VS Purple
                LogType.Info => "#1e90ff",
                LogType.Warning => "#ff8c00",
                LogType.Error => "#d40000",
                LogType.Critical => "#7c0a02",
                _ => throw new InvalidEnumArgumentException(nameof(logEntity.LogType), (int)logEntity.LogType, typeof(LogType))
            };

            return $"<table cellspacing=\"20\" cellpadding=\"10\" style=\"width:100%;border-collapse:collapse;border:1px solid #a7a9ac;font-family:Tahoma;font-size:14px\" border=\"1px\"><tbody><tr style=\"background-color:{color};color:White;font-weight:bold;font-size:20px;height:70px\"><th style=\"width:75%;text-align:start;padding-left:30px;border:0\" colspan=\"3\">{logEntity.Source}</th><th style=\"text-align:right;padding-right:30px;border:0\">{logEntity.LogType}</th></tr><tr style=\"background-color:#e3e3e3;font-weight:bold;font-size:16px\"><td style=\"width:25%\">Source Id</td><td style=\"width:25%\">Date</td><td style=\"width:25%\">Time</td><td>Repetition Count</td></tr><tr><td>{logEntity.SourceId}</td><td>{logEntity.LogDateTime:d}</td><td>{logEntity.LogDateTime:T}</td><td>{logEntity.TodaysRepetitionCount}</td></tr><tr><td style=\"background-color:#a7a9ac;padding:1\" colspan=\"4\"></td></tr><tr><td style=\"background-color:#f4f4f4;font-weight:bold;font-size:16px;color:#626262\" colspan=\"4\">Log Message</td></tr><tr><td colspan=\"4\">{logEntity.Message.Replace(Environment.NewLine, "<br>")}</td></tr></tbody></table><p style=\"padding-left:5;font-family:Tahoma;font-size:12px;color:#626262\">RLogger 3.0.0 V20241007</p>";
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _eMail.Dispose();
                    _smtpClient.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    public static partial class IRLoggerExtensions
    {
        /// <summary>
        /// Adds mail logging to the logger.
        /// </summary>
        /// <param name="options"> The options for the mail logging. </param>
        public static IRLogger AddMailLogging(this IRLogger logger, MailLoggingTargetOptions options)
        {
            logger.AddLoggingTarget(new MailLoggingTarget(options));
            return logger;
        }
    }
}
