using RLogger.Formatters;
using RLogger.Loggers;
using RLogger.Targets;
using System.ComponentModel;

namespace RLogger.FluentBuilding
{
    public interface IRLoggerBuilder : IFluentInterface
    {
        IRLoggerBuilder AddTraceTarget(Func<ILogFormatter<string>>? opt = null);
        IRLoggerBuilder AddDebugTarget(Func<ILogFormatter<string>>? opt = null);
        IRLoggerBuilder AddConsoleTarget(Func<ILogFormatter<string>>? opt = null);
        IRLoggerBuilder AddColoredConsoleTarget(Func<ILogFormatter<(string DateTime, string Level, string Id, string Message)>>? opt = null);
        IRLoggerBuilder AddCustomSyncTarget(Func<ISyncLogTarget> targetFactory);
        IRLoggerBuilder AddCustomAsyncTarget(Func<IAsyncLogTarget> targetFactory);
        IRLogger Build(bool preferSync = true);
    }

    internal class RLoggerBuilder : IRLoggerBuilder
    {
        internal class Shared
        {
            private readonly List<ILogTarget> _targets = [];
            public bool IsEmpty => _targets.Count == 0;
            public void Add(ILogTarget target) => _targets.Add(target);
            public ILogTarget Current => _targets[^1];
            public ILogTarget[] GetTargets() => [.. _targets];
        }

        protected readonly Shared _shared;

        public static IRLoggerBuilder Create()
        {
            return new RLoggerBuilder();
        }
        private RLoggerBuilder()
        {
            _shared = new();
        }
        protected RLoggerBuilder(Shared shared)
        {
            _shared = shared;
        }

        public IRLoggerBuilder AddTraceTarget(Func<ILogFormatter<string>>? opt = null)
        {
            var target = new TraceTarget(opt?.Invoke());
            _shared.Add(target);
            return this;
        }

        public IRLoggerBuilder AddDebugTarget(Func<ILogFormatter<string>>? opt = null)
        {
            var target = new DebugTarget(opt?.Invoke());
            _shared.Add(target);
            return this;
        }

        public IRLoggerBuilder AddConsoleTarget(Func<ILogFormatter<string>>? opt = null)
        {
            var target = new ConsoleTarget(opt?.Invoke());
            _shared.Add(target);
            return this;
        }

        public IRLoggerBuilder AddColoredConsoleTarget(Func<ILogFormatter<(string DateTime, string Level, string Id, string Message)>>? opt = null)
        {
            var target = new ColoredConsoleTarget(opt?.Invoke());
            _shared.Add(target);
            return this;
        }

        public IRLoggerBuilder AddCustomSyncTarget(Func<ISyncLogTarget> targetFactory)
            => AddCustomTarget(targetFactory);

        public IRLoggerBuilder AddCustomAsyncTarget(Func<IAsyncLogTarget> targetFactory)
            => AddCustomTarget(targetFactory);

        private RLoggerBuilder AddCustomTarget(Func<ILogTarget> targetFactory)
        {
            ArgumentNullException.ThrowIfNull(targetFactory);
            var target = targetFactory() ?? throw new ArgumentNullException(nameof(targetFactory), "Target factory must return a valid ILogTarget instance.");
            _shared.Add(target);
            return this;
        }

        public IRLogger Build(bool preferSync = true)
        {
            if (_shared.IsEmpty)
                return NullLogger.Instance;

            var targets = _shared.GetTargets();

            bool useAsync = targets.Any(t => t is IAsyncLogTarget) || !preferSync;

            return useAsync ? new AsyncLogger(targets) : new SyncLogger(targets);
        }
    }
}
