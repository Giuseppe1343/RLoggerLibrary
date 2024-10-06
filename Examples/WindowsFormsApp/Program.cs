using RLoggerLib;
using RLoggerLib.LoggingTargets;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RLogger.RegisterMainThread();
            RLogger.Create(LogDatabaseCreationOptions.Default, (logger) =>
            {
                logger.AddDebugLogging();
                logger.AddTextFileLogging(new TextFileLoggingTargetOptions()
                {
                    FileNamingConvention = LogFileNamingConvention.SourceDate,
                    CustomName = "WindowsFormsApp",
                    DateFormat = "yyyy-MM-dd",
                });
                logger.AddMailLogging(new MailLoggingTargetOptions()
                {
                    //Defaults are predefined in MailLoggingTargetOptions

                    MailServer = "smtp.gmail.com", // Example mail server
                    MailPort = 587, // google mail port
                    MailTo = new string[] { "RECEIVER1 ADDRESS", "RECEIVER2 ADDRESS" }, // Example receiver mail addresses, you can add more or use only one
                    MailUser = "YOUR MAIL ADDRESS",
                    MailPassword = "YOUR MAIL PASSWORD",

                });
            });
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            RLogger.Instance.LogInfo("Application is closed");
        }
    }
}
