using RLoggerLib;
using RLoggerLib.LoggingTargets;

namespace ConsoleApp
{
    internal class Program
    {

        // Entry point of the application
        static void Main(string[] args)
        {
            // Register the main thread
            RLogger.RegisterMainThread();

            // Create logger with adding some logging targets
            RLogger.Create((logger) =>
            {
                // Add debug, console and custom logging targets
                logger.AddDebugLogging()
                    .AddConsoleLogging(LogType.Debug)
                    .AddCustomLoggingTarget(new DemoLoggingTarget());
            });

            RLogger.Instance.LogInfo("Application is running...", "ConsoleApp", "1");

            try
            {
                // Do some work
                RLogger.Instance.LogTrace("Job Starting...");
                MLog(RLogger.Instance);
                RLogger.Instance.LogTrace("Job Completed.");
            }
            catch (Exception ex)
            {
                RLogger.Instance.LogError("Job Failed. Ex:" + ex.Message);
            }

            RLogger.Instance.LogInfo("Application is exited.", "ConsoleApp", "0");
        }

        // Example of a implement custom logging target
        public class DemoLoggingTarget : ILoggingTarget
        {
            public void Log(LogEntity logEntity)
            {
                Console.WriteLine("DEMO");
            }
        }

        static void MLog(RLogger logger)
        {
            logger.LogTrace("Test");
            RLogger.Instance.LogDebug("Test");
            RLogger.Instance.LogInfo("Test");
            RLogger.Instance.LogWarning("Test");
            RLogger.Instance.LogError("Test");
            RLogger.Instance.LogCritical("Test");
        }

    }
}
