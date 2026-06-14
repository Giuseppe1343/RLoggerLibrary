using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Formatters
{
    public interface ILogFormatter
    {
        void FormatTimestamp(StringBuilder builder, DateTimeOffset timestamp);
        void FormatLevel(StringBuilder builder, LogLevel level);
        void FormatCategory(StringBuilder builder, string category);
        void FormatEventId(StringBuilder builder, int eventId);
        void FormatMessage(StringBuilder builder, string message);
        void FormatLogMessage(StringBuilder builder, LogMessage logMessage);
    }
}
