using RLogger.FluentBuilding;
using RLogger.Targets;

namespace RLogger.Loggers
{
    public interface IRLogger
    {
        void Log(LogMessage logMessage);
    }


}
