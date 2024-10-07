using RLoggerLib;
using RLoggerLib.LoggingTargets;
using System;
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
            // Register the main thread
            RLogger.RegisterMainThread();

            // Create empty logger
            RLogger.Create();

            // Late binding of logging targets
            RLogger.Instance.AddDebugLogging();

            RLogger.Instance.LogInfo("Application is starting");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            RLogger.Instance.LogInfo("Application is closing");
        }
    }
}
