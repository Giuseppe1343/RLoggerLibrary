using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger
{
    public class LogMessage
    {
        public static readonly LogMessage Empty = new(default, default, default, string.Empty);

        public DateTime DateTime { get; }
        public RLogger.LogLevel Level { get; }
        public int Id { get; }
        public string Message { get; }

        public LogMessage(DateTime dateTime, RLogger.LogLevel logLevel, int logId, string message)
        {
            DateTime = dateTime;
            Level = logLevel;
            Id = logId;
            Message = message;
        }
    }
}
