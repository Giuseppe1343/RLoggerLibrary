using RLoggerLib.Utils;
using System;

namespace RLoggerLib
{
    /// <summary>
    /// Model for the log.
    /// </summary>
    public class RLog
    {
        /// <summary>
        /// The event id of the log. This is used to group logs together. If the event id is 0, it means that this log is not part of any event.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// T
        /// </summary>
        public DateTime DateTime { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Source { get; set; }
        public int SourceLine { get; set; }
        public string Body { get; set; } // Formatting is done in the log targets

        //[BsonIgnore]
        public int TodayDuplicateCount { get; internal set; }
    }
}
