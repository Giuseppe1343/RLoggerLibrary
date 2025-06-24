using System;
using System.Collections.Generic;
using System.Text;

namespace RLoggerLib.Utils
{
    internal static class FormatUtils
    {
        private static bool showEventId => RLogger.Settings.ShowEventId;

        public static string ToFormattedString(this RLog log)
        {
            return $"[{log.DateTime.ToFormattedString()}] [{log.LogLevel.ToString().ToUpperInvariant()}] - {(showEventId && log.EventId != 0 ? $"EventId:{log.EventId} " : "")}Source:{log.Source} Line:{log.SourceLine}{Environment.NewLine}Message:{log.Body}";
        }
    }
}
