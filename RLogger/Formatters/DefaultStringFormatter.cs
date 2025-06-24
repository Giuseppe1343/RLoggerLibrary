using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Formatters
{
    internal class DefaultStringFormatter : ILogFormatter<string>
    {
        public readonly static DefaultStringFormatter Instance = new();
        public string ApplyFormat(LogMessage logMessage)
            => string.Format("{0:dd.MM.yyyy HH:mm:ss}: [{1}] (Id: {2}): {3}",
                            logMessage.DateTime, logMessage.Level, logMessage.Id, logMessage.Message);
    }
}
