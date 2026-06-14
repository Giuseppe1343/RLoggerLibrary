using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Loggers
{
    internal class NoOpLogger : IRLogger
    {
        public static readonly NoOpLogger Instance = new();
        private NoOpLogger() { }
        void IRLogger.Log(LogMessage logMessage) { }
    }
}
