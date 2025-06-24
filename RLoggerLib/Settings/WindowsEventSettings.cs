using System;

namespace RLoggerLib.Settings
{
    public class WindowsEventSettings
    {
        public static WindowsEventSettings Default { get; } = new WindowsEventSettings();

        /// <summary>
        /// The name of the event log.
        /// </summary>
        public string EventLogName { get; set; } = "Application";

        /// <summary>
        /// The source of the event.
        /// </summary>
        public string EventSource { get; set; } = AppDomain.CurrentDomain.FriendlyName;

    }
}
