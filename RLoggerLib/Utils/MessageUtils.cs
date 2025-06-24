using System;
using System.Collections.Generic;
using System.Text;

namespace RLoggerLib.Utils
{
    internal class MessageUtils
    {
        public const string DEFAULT_MESSAGE = "An error occurred in the RLoggerLib.";
        public const string SETTINGS_FILE_EMPTY = "Settings file is empty.";
        public const string LOGGER_ALREADY_CREATED = "Logger has already been created. If you want to create a new logger, please Close() the current logger first.";
        public const string GLOBAL_SETTINGS_NULL = "Global settings is null.";

        public const string CONSOLETARGET_ALREADY_ADDED = "Console target has already been added.";
        public const string INVALID_TEXT_FILE_SETTINGS = "Invalid TextFileSettings. You must provide a valid log name or a log name builder that returns valid log name. Valid log name must be a not null or empty string.";
        public const string OUT_OF_RANGE_TEXT_FILE_SETTINGS = "TextFileSettings parameter(s) are out of range. File size limit, retained file count limit must be greater than or equal to 0.";
        public const string ENUM_NOT_DEFINED_TEXT_FILE_SETTINGS = "TextFileSettings parameter(s) are not defined in the enum. File creation interval, file size limit policy must be defined in the enum.";

    }
}
namespace RLoggerLib
{
    public class RLoggerLibException : Exception
    {
        public RLoggerLibException() { }
        public RLoggerLibException(string message) : base(message) { }
        public RLoggerLibException(string message, Exception inner) : base(message, inner) { }
        protected RLoggerLibException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
