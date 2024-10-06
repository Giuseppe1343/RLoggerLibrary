using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Default logging target implementation for the text file logging. <br/>
    /// </summary>
    internal class TextFileLoggingTarget : ILoggingTarget
    {
        private const string LogExtension = ".log";
        private readonly TextFileLoggingTargetOptions _options;
        public TextFileLoggingTarget(TextFileLoggingTargetOptions options)
        {
            _options = options;

            // Normalize the directory path
            _options.Directory = _options.Directory.NormalizeDirectoryPath();

            // Normalize the custom name
            _options.CustomName = _options.CustomName.NormalizeFileName();

            // Test the path validity
            Helpers.TestFilePath(_options.Directory, _options.CustomName + _options.NameSeparator + DateTime.Now.ToString(_options.DateFormat));
        }

        /// <inheritdoc/>
        public void Log(LogEntity logEntity)
        {
            // Check the log type
            if (logEntity.LogType < _options.MinRequiredSeverity)
                return;

            // Create the log directory if it does not exist
            Directory.CreateDirectory(_options.Directory);

            // Prepare log path
            var logPathBuilder = new StringBuilder();

            logPathBuilder.Append(_options.Directory);
            if (_options.FileNamingConvention.HasFlag(LogFileNamingConvention.Custom))
            {
                logPathBuilder.Append(_options.CustomName);
                logPathBuilder.Append(_options.NameSeparator);
            }
            if (_options.FileNamingConvention.HasFlag(LogFileNamingConvention.Source))
            {
                logPathBuilder.Append(logEntity.Source.NormalizeFileName());
                logPathBuilder.Append(_options.NameSeparator);
            }
            if (_options.FileNamingConvention.HasFlag(LogFileNamingConvention.SourceId))
            {
                logPathBuilder.Append(logEntity.SourceId.NormalizeFileName());
                logPathBuilder.Append(_options.NameSeparator);
            }
            if (_options.FileNamingConvention.HasFlag(LogFileNamingConvention.Date))
            {
                logPathBuilder.Append(logEntity.LogDateTime.ToString(_options.DateFormat, CultureInfo.InvariantCulture));
            }

            logPathBuilder.Append(LogExtension);

            // Append the log to the file
            File.AppendAllText(logPathBuilder.ToString(), logEntity.ToLogString() + Environment.NewLine);
        }
    }
    public static partial class IRLoggerExtensions
    {
        /// <summary>
        /// Adds text file logging to the logger.
        /// </summary>
        /// <param name="options"> The options for the text file logging. </param>
        public static IRLogger AddTextFileLogging(this IRLogger logger, TextFileLoggingTargetOptions options)
        {
            logger.AddLoggingTarget(new TextFileLoggingTarget(options));
            return logger;
        }
    }
}
