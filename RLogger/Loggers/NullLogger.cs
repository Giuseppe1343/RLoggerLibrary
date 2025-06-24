using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Loggers
{
    internal class NullLogger : IRLogger
    {
        public static readonly NullLogger Instance = new();
        public void Log(LogMessage logMessage) { }
    }
}
