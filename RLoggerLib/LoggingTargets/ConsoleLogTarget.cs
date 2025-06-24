using System;

namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Default log target implementation for the console logging. <br/>
    /// </summary>
    internal class ConsoleLogTarget : ILogTarget
    {
        public const string UNIQUE_NAME = "DefaultConsoleLogTarget";
        public string UniqueName => UNIQUE_NAME;

        private readonly ConsoleSettings _consoleSettings;
        public ConsoleLogTarget(ConsoleSettings consoleSettings)
        {
            _consoleSettings = consoleSettings;
        }

        public void Log(RLog log)
        {
            if (_consoleSettings.UseColoredLogLevels)
                WriteColored(log);
            else
                Write(log);
        }

        private void Write(RLog log)
        {
            Console.WriteLine(log.ToFormattedString());
        }
        private void WriteColored(RLog log)
        {
            Console.Write($"[{log.DateTime.ToFormattedString()}] [");
            Console.BackgroundColor = ConsoleColor.Black;
            switch (log.LogLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Critical:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
            }
            Console.Write(log.LogLevel.ToString().ToUpperInvariant());
            Console.ResetColor();
            Console.WriteLine($"] - {(RLogger.Settings.ShowEventId && log.EventId != 0 ? $"EventId:{log.EventId} " : "")}Source:{log.Source} Line:{log.SourceLine}{Environment.NewLine}Message:{log.Body}");
        }

    }
}
