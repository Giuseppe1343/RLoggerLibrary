using RLogger.Targets;

namespace RLogger.Loggers
{
    internal class SyncLogger : IRLogger, IDisposable
    {
        private readonly ILogTarget[] _targets;
        public SyncLogger(ILogTarget[] targets)
        {
            _targets = targets;
        }

        public void Log(LogMessage logMessage)
        {
            foreach (var target in _targets.AsSpan())
                target.Log(logMessage);
        }

        public void Dispose()
        {
            foreach (var target in _targets.AsSpan())
            {
                if (target is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch
                    {
                        // TODO: Internal logging or handle exceptions during disposal
                    }
                }
            }
        }
    }


}
