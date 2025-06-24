using System;
using System.Collections.Generic;
using System.Text;

namespace RLoggerLib.Utils
{
    internal static class TimeUtils
    {
        private static bool useUtcTime => RLogger.Settings.UseUtcTime;
        private static bool showDate => RLogger.Settings.ShowDate;
        private static bool showMilliseconds => RLogger.Settings.ShowMilliseconds;
        public static DateTime Now => useUtcTime ? DateTime.UtcNow : DateTime.Now;
        public static DateTime Today => useUtcTime ? DateTime.UtcNow.Date : DateTime.Today;

        public static string ToFormattedString(this DateTime time) => time.ToString($"{(showDate ? "yyyy-MM-dd " : "")}HH:mm:ss{(showMilliseconds ? ".fff" : "")}");
    }
}
