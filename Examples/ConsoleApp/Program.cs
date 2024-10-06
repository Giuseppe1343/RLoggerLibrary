using RLoggerLib;
using RLoggerLib.LoggingTargets;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RLogger.RegisterMainThread();
            RLogger.Create(LogDatabaseCreationOptions.Default, (logger) =>
            {
                logger.AddDebugLogging()
                    .AddConsoleLogging()
                    .AddTextFileLogging(new TextFileLoggingTargetOptions() { 
                        FileNamingConvention = LogFileNamingConvention.SourceSourceIdDate 
                    })
                    .AddWindowsEventLogging(WindowsEventLoggingTargetOptions.Default)
                    .AddCustomLoggingTarget(new DemoLoggingTarget());
            });

            RLogger.Instance.LogInfo("Application is running");
            MLog(RLogger.Instance);
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

        // Example of a implement custom logging target
        public class DemoLoggingTarget : ILoggingTarget
        {
            public void Log(LogEntity logEntity)
            {
                Console.WriteLine("DEMO");
            }
        }
    }
}
