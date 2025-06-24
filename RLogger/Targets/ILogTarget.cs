using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Targets
{
    public interface ILogTarget { }
    public interface IAsyncLogTarget : ILogTarget
    {
        ValueTask LogAsync(LogMessage logMessage);
    }
    public interface ISyncLogTarget : ILogTarget
    {
        void Log(LogMessage logMessage);
    }
}
