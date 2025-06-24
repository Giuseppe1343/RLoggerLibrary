using System;
using System.Collections.Generic;
using System.Text;

namespace RLoggerLib.Settings
{
    public class GeneralSettings
    {
        public bool UseUtcTime { get; set; } = false;
        public bool ShowDate { get; set; } = true;
        public bool ShowMilliseconds { get; set; } = true;
        public bool ShowEventId { get; set; } = true;
    }
}
