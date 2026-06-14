using RLogger.FluentBuilding;
using RLogger.Targets;

namespace RLogger.Loggers
{
    //TODO: Consider making this interface implement IDisposable and IAsyncDisposable, and then implement those in the concrete loggers, so that the user can dispose the logger when done, and it will dispose the targets as well.
    public interface IRLogger //: IDisposable, IAsyncDisposable
    {
        void Log(LogMessage logMessage);
    }
}
