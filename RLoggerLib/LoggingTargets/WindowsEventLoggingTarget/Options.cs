using System;

namespace RLoggerLib.LoggingTargets
{
    public class WindowsEventLoggingTargetOptions
    {
        /// <summary>
        /// The default options for the <see cref="WindowsEventLoggingTargetOptions"/>.
        /// </summary>
        public static WindowsEventLoggingTargetOptions Default => new();

        /// <summary>
        /// The minimum required severity for log to the windows event log.
        /// </summary>
        public LogType MinRequiredSeverity { get; set; } = LogType.Info;

        /// <summary>
        /// The source of the event. If UseLogSourceAsEventSource is true, this will be ignored.
        /// </summary>
        public string EventSource { get; set; } = AppDomain.CurrentDomain.FriendlyName;

        /// <summary>
        /// If true, the <see cref="LogEntity.Source"/> will be used as as the <see cref="EventSource"/>.
        /// </summary>
        public bool UseLogSourceAsEventSource { get; set; } = false;

        /// <summary>
        /// The event id will be used if UseLogSourceIdAsEventId is false or SourceId is not valid integer.
        /// </summary>
        public int EventId { get; set; } = 0;

        /// <summary>
        /// If true, the <see cref="LogEntity.SourceId"/> will be used as the <see cref="EventId"/>. If SourceId is not valid integer, EventId will be used.
        /// </summary>
        public bool UseLogSourceIdAsEventId { get; set; } = false;
    }
}
