using RLogger.Formatters;
using RLogger.Loggers;
using RLogger.Persistence;
using RLogger.Targets;
using System.ComponentModel;

namespace RLogger.FluentBuilding
{
    public interface IRLoggerBuilder : IFluentInterface
    {
        IRLoggerBuilder AddTraceTarget(TargetOptions? options = null);
        IRLoggerBuilder AddTraceTarget(Func<TargetOptions>? optionsFactory);
        IRLoggerBuilder AddDebugTarget(TargetOptions? options = null);
        IRLoggerBuilder AddDebugTarget(Func<TargetOptions>? optionsFactory);
        IRLoggerBuilder AddConsoleTarget(TargetOptions? options = null);
        IRLoggerBuilder AddConsoleTarget(Func<TargetOptions>? optionsFactory);
        IRLoggerBuilder AddColoredConsoleTarget(TargetOptions? options = null);
        IRLoggerBuilder AddColoredConsoleTarget(Func<TargetOptions>? optionsFactory);
        IRLoggerBuilder AddCustomTarget(Func<ILogTarget> targetFactory);
        IRLogger BuildLogger();
        IRLogger BuildAsyncLogger(ILogPersistence? logPersistence = null);
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
            public void Clear() => _targets.Clear();
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

        public IRLoggerBuilder AddTraceTarget(TargetOptions? options = null)
        {
            var target = new TraceTarget(options?.MinLogLevel, options?.Formatter);
            _shared.Add(target);
            return this;
        }
        public IRLoggerBuilder AddTraceTarget(Func<TargetOptions>? optionsFactory = null)
        {
            var options = optionsFactory?.Invoke();
            return AddTraceTarget(options);
        }

        public IRLoggerBuilder AddDebugTarget(TargetOptions? options = null)
        {
            var target = new DebugTarget(options?.MinLogLevel, options?.Formatter);
            _shared.Add(target);
            return this;
        }

        public IRLoggerBuilder AddDebugTarget(Func<TargetOptions>? optionsFactory = null)
        {
            var options = optionsFactory?.Invoke();
            return AddDebugTarget(options);
        }

        public IRLoggerBuilder AddConsoleTarget(TargetOptions? options = null)
        {
            var target = new ConsoleTarget(options?.MinLogLevel, options?.Formatter);
            _shared.Add(target);
            return this;
        }

        public IRLoggerBuilder AddConsoleTarget(Func<TargetOptions>? optionsFactory = null)
        {
            var options = optionsFactory?.Invoke();
            return AddConsoleTarget(options);
        }

        public IRLoggerBuilder AddColoredConsoleTarget(TargetOptions? options = null)
        {
            var target = new ColoredConsoleTarget(options?.MinLogLevel, options?.Formatter);
            _shared.Add(target);
            return this;
        }

        public IRLoggerBuilder AddColoredConsoleTarget(Func<TargetOptions>? optionsFactory = null)
        {
            var options = optionsFactory?.Invoke();
            return AddColoredConsoleTarget(options);
        }

        public IRLoggerBuilder AddCustomTarget(Func<ILogTarget> targetFactory)
        {
            ArgumentNullException.ThrowIfNull(targetFactory);
            var target = targetFactory() ?? throw new ArgumentNullException(nameof(targetFactory), "Target factory must return a valid ILogTarget instance.");
            _shared.Add(target);
            return this;
        }

        public IRLogger BuildLogger()
        {
            if (_shared.IsEmpty)
                return NoOpLogger.Instance;

            var targets = _shared.GetTargets();
            _shared.Clear();

            bool useAsync = targets.Any(t => t is IAsyncLogTarget);

            IRLogger logger = useAsync ? new AsyncLogger(NoOpLogPersistence.Instance, targets) : new SyncLogger(targets);

            // Set the global logger if it's not already set to avoid overwriting an existing logger.
            if (R.Logger == NoOpLogger.Instance)
                R.Logger = logger;

            return logger;
        }

        public IRLogger BuildAsyncLogger(ILogPersistence? logPersistence = null)
        {
            if (_shared.IsEmpty)
                return NoOpLogger.Instance;

            var targets = _shared.GetTargets();
            _shared.Clear();

            // TODO: mmf persistence options (e.g., batch size, retry policy, etc.)
            var logger = new AsyncLogger(logPersistence ?? NoOpLogPersistence.Instance, targets);

            // Set the global logger if it's not already set to avoid overwriting an existing logger.
            if (R.Logger == NoOpLogger.Instance)
                R.Logger = logger;

            return logger;
        }
    }
}
