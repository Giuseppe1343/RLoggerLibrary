using RLoggerLib.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;

namespace RLoggerLib.LoggingTargets
{
    internal class LogTargetCustomizer : ILogTargetCustomizer
    {
        private readonly Dictionary<string, ILogTarget> _logTargets = [];

        public ILogTargetCustomizer LogToTarget(ILogTarget loggingTarget)
        {
            if (_logTargets.ContainsKey(loggingTarget.UniqueName))
            {
                var message = $"LogToTarget: The logging target with the name {loggingTarget.UniqueName} is already added.";

                if (!RLogger.UseInternalLogging)
                    throw new ArgumentException(message);

                LoggerInternal.LogWarning(message);
                return this;
            }

            _logTargets.Add(loggingTarget.UniqueName, loggingTarget);
            return this;
        }

        public ILogTargetCustomizer LogToDebug()
        {
            // Check for multiple calls
            if (_logTargets.ContainsKey(DebugLogTarget.UNIQUE_NAME))
            {
                Debug.WriteLine("Warning: The LogToDebug method is called multiple times. First call is used.");
                return this;
            }

            // Add the debug log target
            _logTargets.Add(DebugLogTarget.UNIQUE_NAME, new DebugLogTarget());
            return this;
        }

        public ILogTargetCustomizer LogToConsole(ConsoleSettings settings)
        {
            // Check for multiple calls
            if (_logTargets.ContainsKey(ConsoleLogTarget.UNIQUE_NAME))
            {
                Debug.WriteLine("Warning: The LogToConsole method is called multiple times. First call is used.");
                return this;
            }

            // Check for null settings
            if (settings is null)
            {
                if (!RLogger.UseInternalLogging)
                    throw new ArgumentNullException(nameof(settings));

                LoggerInternal.LogWarning($"LogToConsole: The settings argument is null.");
                return this;
            }

            // Add the console log target
            _logTargets.Add(ConsoleLogTarget.UNIQUE_NAME, new ConsoleLogTarget(settings));
            return this;
        }

        public ILogTargetCustomizer LogToWindowsEvent(WindowsEventSettings settings)
        {
            // Check for multiple calls
            if (_logTargets.ContainsKey(WindowsEventLogTarget.UNIQUE_NAME))
            {
                Debug.WriteLine("Warning: The LogToWindowsEvent method is called multiple times. First call is used.");
                return this;
            }

            // Check for null settings
            if (settings is null)
            {
                if (!RLogger.UseInternalLogging)
                    throw new ArgumentNullException(nameof(settings));

                LoggerInternal.LogWarning($"LogToWindowsEvent: The settings argument is null.");
                return this;
            }

            // Try to add the Windows event log target
            try
            {
                _logTargets.Add(WindowsEventLogTarget.UNIQUE_NAME, new WindowsEventLogTarget(settings));
            }
            catch (Exception ex) when (RLogger.UseInternalLogging)
            {
                LoggerInternal.LogError($"LogToWindowsEvent: The WindowsEventLogTarget could not be created with this settings.", ex, settings);
            }

            return this;
        }

        public ILogTargetCustomizer LogToTextFile(TextFileSettings settings)
        {
            // Check for null settings
            if (settings is null)
            {
                if (!RLogger.UseInternalLogging)
                    throw new ArgumentNullException(nameof(settings));

                LoggerInternal.LogWarning($"LogToTextFile: The settings argument is null.");
                return this;
            }

            // Try to create the text file log target
            TextFileLogTarget textFileLogTarget;
            try
            {
                textFileLogTarget = new TextFileLogTarget(settings);
            }
            catch (Exception ex) when (RLogger.UseInternalLogging)
            {
                LoggerInternal.LogError($"LogToTextFile: The TextFileLogTarget could not be created with this settings.", ex, settings);
                return this;
            }

            // Check for multiple calls
            if (_logTargets.ContainsKey(textFileLogTarget.UniqueName))
            {
                var message = $"LogToTextFile: The TextFileLogTarget with the name {textFileLogTarget.UniqueName} is already added.";

                if (!RLogger.UseInternalLogging)
                    throw new ArgumentException(message);

                LoggerInternal.LogWarning(message);
                return this;
            }

            // Add the text file log target
            _logTargets.Add(textFileLogTarget.UniqueName, textFileLogTarget);
            return this;
        }
    }
}
