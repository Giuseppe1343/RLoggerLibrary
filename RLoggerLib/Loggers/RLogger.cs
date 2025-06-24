using RLoggerLib.Loggers;
using RLoggerLib.LoggingTargets;
using RLoggerLib.Utils;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using RLoggerLib.Settings;
using System.Threading;

namespace RLoggerLib
{
    /// <summary>
    /// The main class for the asynchronous logger.
    /// </summary>
    public static class RLogger
    {
        /// <summary>
        /// <see cref="RLogger"/> logs the error that occurs during any operation instead of throwing an exception. <br/>
        /// The default value is <c><b>!</b><see cref="Debugger.IsAttached"/></c>.
        /// </summary>
        public static bool UseInternalLogging { get; set; } = !Debugger.IsAttached;

        public static LogLevel GlobalLogLevel { get; set; } = LogLevel.Info;

        public static GeneralSettings Settings { get; set; } = new GeneralSettings();



        private static readonly object _lock = new();

        private static LoggerThread? _logger;

        public static bool IsActive => _logger is not null;

        /// <summary>
        /// Create a new instance of <see cref="RLogger"/> accessible through the <see cref="Instance"/> property. <br/>
        /// This also re-creates the instance if it's already created.
        /// </summary>
        /// <param name="options"> The options for the logger's database. </param>
        /// <param name="creationOptions"> An action to set the creation options for the logger. <br/>
        /// Example: <c> (logger) => { logger.AddDebugLogging(); } </c> </param>
        /// <exception cref="InvalidOperationException"> Main thread not registered or Instance already created. </exception>
        public static void Create(GlobalSettings settings, Action<ILogTargetCustomizer>)
        {
            if (_logger is not null)
                MessageUtils.ThrowIfInternalLoggingNotEnabled(new InvalidOperationException(MessageUtils.LOGGER_ALREADY_CREATED));

            lock (_lock)
            {
                if (_logger is not null)
                    return;

                ImportSettings(settings);

                _logger = Creator();
                return _dummy;
            }
        }

        /// <inheritdoc cref="Create(LogDatabaseCreationOptions, Action{IRLogger})"/>
        public static void Create(Action<IRLogger> creationOptions) => Create(null, creationOptions);

        /// <inheritdoc cref="Create(LogDatabaseCreationOptions, Action{IRLogger})"/>
        public static void Create(LogDatabaseCreationOptions options) => Create(options, null);

        /// <inheritdoc cref="Create(LogDatabaseCreationOptions, Action{IRLogger})"/>
        public static void Create() => Create(null, null);

        public static void Close()
        {
            if (_instance is null)
                return;

        }

    }
}
