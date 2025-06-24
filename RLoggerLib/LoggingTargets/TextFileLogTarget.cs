using RLoggerLib.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RLoggerLib.LoggingTargets
{
    /// <summary>
    /// Default logging target implementation for the text file logging.
    /// </summary>
    internal class TextFileLogTarget : ILogTarget
    {
        private static int _logTargetCreatedCount = 0;
        public string UniqueName => string.IsNullOrWhiteSpace(_options.LogName) ? $"TextDynamicFileLogTarget{_logTargetCreatedCount}" : _options.LogName;

        private readonly TextFileSettings _options;

        private readonly bool _useDynamicFileName;
        private DateTime _nextInterval;
        private string _currentInternalStr = "";

        public TextFileLogTarget(TextFileSettings options)
        {
            _options = options;

            if (_options.RetainedFileCountLimit < 0/* || _options.FileSizeLimit < 0*/)
                throw new ArgumentOutOfRangeException(MessageUtils.OUT_OF_RANGE_TEXT_FILE_SETTINGS);

            if (!Enum.IsDefined(typeof(CreationInterval), _options.FileCreationInterval)/* || !Enum.IsDefined(typeof(SizeLimitPolicy), _options.FileSizeLimitPolicy)*/)
                throw new InvalidEnumArgumentException(MessageUtils.ENUM_NOT_DEFINED_TEXT_FILE_SETTINGS);

            if (string.IsNullOrWhiteSpace(_options.LogName) && (_options.LogNameBuilder is null || string.IsNullOrWhiteSpace(_options.LogNameBuilder.Invoke())))
                throw new ArgumentException(MessageUtils.INVALID_TEXT_FILE_SETTINGS);

            // Get the full path of the directory
            _options.Directory = IOUtils.GetFullPath(_options.Directory);

            // Ensure the directory exists
            IOUtils.EnsureDirectory(_options.Directory);

            _useDynamicFileName = string.IsNullOrWhiteSpace(_options.LogName);

            // Create temp file that includes the log name
            var tempFile = _useDynamicFileName ? _options.LogNameBuilder!.Invoke() : _options.LogName;

            // Append a random file name to the temp file
            tempFile = Path.Combine(tempFile, Path.GetRandomFileName());

            // Test file operations (C W D)
            IOUtils.TestFileOperations(_options.Directory, tempFile);

            // Checkpoint
            CheckPoint(_options.FileCreationInterval, ref _currentInternalStr, ref _nextInterval);

            // Increment the log target created count
            _logTargetCreatedCount++;
        }

        /// <inheritdoc/>
        public void Log(RLog log)
        {
            // Get the log name
            string logName = _useDynamicFileName ? _options.LogNameBuilder!.Invoke() : _options.LogName;

            // Get the log file path
            string logFilePath = GetLogFilePath(IOUtils.GetFullPath(logName,_options.Directory));

            // Append the log to the file
            File.AppendAllText(logFilePath, log.ToFormattedString() + Environment.NewLine);
        }

        // this function should move to Log(RLog log) function
        private string GetLogFilePath(string logName)
        {
            var directory = Path.GetDirectoryName(logName);
            var fileName = Path.GetFileName(logName);

            IOUtils.EnsureDirectory(directory);

            if (TimeUtils.Now > _nextInterval)
            {
                CheckPoint(_options.FileCreationInterval, ref _currentInternalStr, ref _nextInterval);
                RemoveOldFiles(directory, fileName);
            }

            /* Not implemented yet
            else if (_fileSizeLimit > 0)
            {
                // calculate the size of the file

                // if the size exceeds the limit

                // apply the policy

                RemoveOldFiles(directory, fileName);
            }
            */

            return CreateLogFileName(fileName);
        }

        private string CreateLogFileName(string fileName/*, int section = 0*/)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                extension = ".log";

            //var sectionStr = section > 0 ? $"_{section}" : string.Empty;

            return $"{name}{_currentInternalStr}{extension}"/*{sectionStr}*/;
        }


        private void RemoveOldFiles(in string directory, in string fileName)
        {
            if (_options.RetainedFileCountLimit == 0)
                return;

            var directoryInfo = new DirectoryInfo(directory);
            var oldfiles = directoryInfo.GetFiles(fileName + "*").OrderByDescending(f => f.CreationTime).Skip(_options.RetainedFileCountLimit);

            foreach (var file in oldfiles)
                file.Delete();
        }

        private static void CheckPoint(in CreationInterval creationInterval, ref string currentInternalStr, ref DateTime nextInterval)
        {
            DateTime now = TimeUtils.Now;

            switch (creationInterval)
            {
                case CreationInterval.Infinite:
                    currentInternalStr = string.Empty;
                    nextInterval = DateTime.MaxValue;
                    break;
                case CreationInterval.Yearly:
                    currentInternalStr = now.ToString("yyyy");
                    nextInterval = new DateTime(now.Year, 1, 1, 0, 0, 0, 0, now.Kind).AddYears(1);
                    break;
                case CreationInterval.Monthly:
                    currentInternalStr = now.ToString("yyyyMM");
                    nextInterval = new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, now.Kind).AddMonths(1);
                    break;
                case CreationInterval.Daily:
                    currentInternalStr = now.ToString("yyyyMMdd");
                    nextInterval = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, now.Kind).AddDays(1);
                    break;
                case CreationInterval.Hourly:
                    currentInternalStr = now.ToString("yyyyMMddHH");
                    nextInterval = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, 0, now.Kind).AddHours(1);
                    break;
                case CreationInterval.Minutely:
                    currentInternalStr = now.ToString("yyyyMMddHHmm");
                    nextInterval = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 0, now.Kind).AddMinutes(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
