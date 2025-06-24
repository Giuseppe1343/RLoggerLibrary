using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Formatters
{
    internal class SplittedStringFormatter : ILogFormatter<(string DateTime, string Level, string Id, string Message)>
    {
        public readonly static SplittedStringFormatter Instance = new();

        public (string DateTime, string Level, string Id, string Message) ApplyFormat(LogMessage logMessage)
        {
            return ($"{logMessage.DateTime:dd.MM.yyyy HH:mm:ss}: ",
                    $"[{logMessage.Level}]",
                    $" (Id: {logMessage.Id}): ",
                    logMessage.Message);
        }
    }
}
