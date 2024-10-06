using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace RLoggerThread
{
    /// <summary>
    /// Delegate for holding the log actions. <br/>
    /// If you want to add a new <see cref="LogTarget"/>, you should add a new method matching this delegate and add it to the <see cref="RLoggerThread._logAction"/> inside the constructor of <see cref="RLoggerThread"/>.
    /// </summary>
    internal delegate void LogAction(LogModel log, int repeatedErrorCount);

    internal class RLoggerThread : IRLoggerThread
    {
        private readonly RLoggerThreadCreationOptions _options; // Options for the logger

        private readonly LogDatabase _logDatabase; // SQLite Database

        private readonly LogAction _logAction; // Delegate for holding the log actions

        private readonly BlockingCollection<LogModel> _logBlockingQueue = []; // BlockingQueue for holding unprocessed logs

        //private readonly ManualResetEvent _logEvent = new(false); // Event for signaling the log thread
        private readonly Thread _mainThread; // The main thread for checking if it's alive
        private readonly Thread _loggerThread; // The log thread

        /// <summary>
        /// Create a new instance of <see cref="RLoggerThread"/> with the default options.
        /// </summary>
        public RLoggerThread() : this(new()) { }

        /// <summary>
        /// Create a new instance of <see cref="RLoggerThread"/> with the <paramref name="options"/>.
        /// </summary>
        /// <param name="options"> The options for the logger. </param>
        public RLoggerThread(RLoggerThreadCreationOptions options)
        {
            // Set the options
            _options = options;

            // Create the log database
            _logDatabase = new(_options.DatabaseCreationOptions);

            // Fill _logAction delegate with the specified targets' actions
            if (_options.LogTargets.HasFlag(LogTarget.Debug))
                _logAction += LogToDebug;

            if (_options.LogTargets.HasFlag(LogTarget.Console))
                _logAction += LogToConsole;

            if (_options.LogTargets.HasFlag(LogTarget.File))
                _logAction += LogToFile;

            if (_options.LogTargets.HasFlag(LogTarget.Mail))
                _logAction += LogToMail;

            // Get the main thread
            _mainThread = Thread.CurrentThread;

            // Create the log thread
            _loggerThread = new Thread(LoggerThreadMain)
            {
                Name = "LoggerThread",
                IsBackground = false // This thread should not be terminated by the main thread
            };

            // Start the log thread
            _loggerThread.Start();
        }

        /// <summary>
        /// The main method for the log thread.
        /// </summary>
        void LoggerThreadMain()
        {
            do
            {
                //// Wait for the event to be set
                //if (!_logEvent.WaitOne(1000))
                //    continue; // Timeout, check if the main thread is still alive

                //while (_logQueue.TryDequeue(out var log))

                // The log thread will wait for 1 second to take the log from the queue if the log taken, it will be processed.
                while (_logBlockingQueue.TryTake(out var log, 1000)) // while the logBlockingQueue is not empty
                {
                    int repeatedErrorCount = 0;
                    if (log.LogType >= _options.DbMinRequiredSeverity)
                        repeatedErrorCount = _logDatabase.AddLogAndCheckIfExistsToday(log);
                    _logAction?.Invoke(log, repeatedErrorCount);
                }

                //// Reset the event
                //_logEvent.Reset();


            } while (_mainThread.IsAlive); // If the main thread is not alive, the log thread will terminate
            _logBlockingQueue.Dispose();
        }


        /// <inheritdoc/>
        public void Log(string message, string source, string clientName, string clientId, LogType logType)
        {
            _logBlockingQueue.Add(new LogModel(message + (logType == LogType.Trace ? $"{new StackTrace(2, true)}" : ""), source, logType, clientName, clientId));
            //_logEvent.Set();
        }

        #region LogAction Implementations

        /// <summary>
        /// Implementation of <c><see cref="LogTarget.Debug"/></c> for send log to debug output. 
        /// </summary>
        /// <param name="log">The log to be sent</param>
        /// <param name="_"> discarded </param>
        void LogToDebug(LogModel log, int _)
        {
            Debug.WriteLine($"{log.Source}.log - {log.LogTime} | {log.LogType} => {log.Message}");
        }

        /// <summary>
        /// Implementation of <c><see cref="LogTarget.Console"/></c> for send log to console output.
        /// </summary>
        /// <param name="log">The log to be sent</param>
        /// <param name="_"> discarded </param>
        void LogToConsole(LogModel log, int _)
        {
            Console.WriteLine($"{log.Source}.log - {log.LogTime} | {log.LogType} => {log.Message}");
        }

        /// <summary>
        /// Implementation of <c><see cref="LogTarget.File"/></c> for send log to file.<br/><br/>
        /// Append the log to the file with the name of the client, source and date
        /// </summary>
        /// <param name="log">The log to be sent</param>
        /// <param name="_"> discarded </param>
        void LogToFile(LogModel log, int _)
        {
            // Create the log directory if it does not exist
            Directory.CreateDirectory(_options.LogFileDirectory);

            // Append the log to the file
            File.AppendAllText($"{_options.LogFileDirectory}{log.ClientName}{log.Source}{log.LogTime:yyMMdd}.log", $"{Environment.NewLine}{log.LogTime:HH:mm:ss} | {log.LogType} => {log.Message}{Environment.NewLine}");
        }

        /// <summary>
        /// Implementation of <c><see cref="LogTarget.Mail"/></c> for send log by mail.<br/><br/>
        /// Send mail if the log type is greater than the <c><see cref="_options.MinRequiredSevernity"/></c> and the repeated error count is less than the <c><see cref="_options.MaxRepeatedErrorCount"/></c>
        /// </summary>
        /// <param name="log">The log to be sent</param>
        /// <param name="repeatedErrorCount">The count of the repeated error.</param>
        void LogToMail(LogModel log, int repeatedErrorCount)
        {
            // If the log type is less than the minimum required severity or the repeated error count is greater than the maximum repeated error count, do not send the mail.
            if (log.LogType < _options.MailMinRequiredSeverity || repeatedErrorCount > _options.MailMaxRepeatedErrorCount)
                return;

            var color = log.LogType switch
            {
                LogType.Trace => "#8c8c94",
                LogType.Debug => "#4e3fa8",
                LogType.Information => "#1e90ff",
                LogType.Warning => "#ff8c00",
                LogType.Error => "#d40000",
                LogType.Critical => "#7c0a02",
                _ => throw new InvalidEnumArgumentException(nameof(log.LogType), (int)log.LogType, typeof(LogType))
            };
            try
            {
                // Create the mail message
                using var eMail = new MailMessage()
                {
                    From = new MailAddress(_options.MailUser, _options.MailSenderName),
                    Subject = $"{log.ClientName} - {log.Source} - Id:{log.ClientId} Date:{log.LogTime:dd/MM/yyyy}",
                    IsBodyHtml = true,
                    Body = $"<table cellspacing=\"20\" cellpadding=\"10\" style=\"width:100%;border-collapse:collapse;border:1px solid #a7a9ac;font-family:Tahoma;font-size:14px\" border=\"1px\"><tbody><tr style=\"background-color:{color};color:White;font-weight:bold;font-size:20px;height:70px\"><th style=\"width:75%;text-align:start;padding-left:30px;border:0\" colspan=\"3\">{log.ClientName} - {log.Source}</th><th style=\"text-align:right;padding-right:30px;border:0\">{log.LogType}</th></tr><tr style=\"background-color:#e3e3e3;font-weight:bold;font-size:16px\"><td style=\"width:20%\">Client Id</td><td style=\"width:20%\">Date</td><td style=\"width:20%\">Time</td><td>Log Name</td></tr><tr><td>{log.ClientId}</td><td>{log.LogTime:d}</td><td>{log.LogTime:T}</td><td>{log.ClientName}{log.Source}{log.LogTime:yyMMdd}.log</td></tr><tr><td style=\"background-color:#a7a9ac;padding:1\" colspan=\"4\"></td></tr><tr><td style=\"background-color:#f4f4f4;font-weight:bold;font-size:16px;color:#626262\" colspan=\"4\">Log Message</td></tr><tr><td colspan=\"4\">{log.Message.Replace(Environment.NewLine,"<br>")}</td></tr></tbody></table><p style=\"padding-left:5;font-family:Tahoma;font-size:12px;color:#626262\">MTLogger 2.0.0 V20240928</p>"
                };

                // Add the receivers
                foreach (var receiver in _options.MailTo) eMail.To.Add(receiver);

                // Send the mail
                using var smtp = new SmtpClient(_options.MailServer, _options.MailPort);
                smtp.Credentials = new NetworkCredential(_options.MailUser, _options.MailPassword);
                smtp.EnableSsl = true;
                smtp.Send(eMail);
            }
            catch (Exception ex)
            {
                //If the mail could not be sent, log the error in database
                _logDatabase.AddLog(new LogModel($"An error occured while sending the mail.{Environment.NewLine}Error: {ex.Message}", "MTLogger", LogType.Error));
            }
        }
        #endregion
    }
}
