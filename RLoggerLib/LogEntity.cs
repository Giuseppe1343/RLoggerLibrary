using System;

namespace RLoggerLib
{
    /// <summary>
    /// Model for the log.
    /// </summary>
    public class LogEntity
    {
        public DateTime LogDateTime { get; set; }
        public LogType LogType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string SourceId { get; set; }
        public long TodaysRepetitionCount { get; internal set; }

        public virtual string ToLogString() => $"[{LogDateTime:T}] {LogType}: {Source}[{SourceId}] - {Message}";
    }
}
