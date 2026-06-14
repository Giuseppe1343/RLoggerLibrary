using RLogger.Persistence;
using RLogger.Targets;
using System.Threading.Channels;

namespace RLogger.Loggers
{
    internal class AsyncLogger : IRLogger, IAsyncDisposable
    {
        private readonly ILogPersistence _logPersistence;
        private readonly ILogTarget[] _targets;

        private readonly Task _loggerTask;
        private readonly Channel<LogMessage> _logMessageChannel;
        private ChannelReader<LogMessage> Reader => _logMessageChannel.Reader;
        private ChannelWriter<LogMessage> Writer => _logMessageChannel.Writer;

        public AsyncLogger(ILogPersistence logPersistence, ILogTarget[] targets)
        {
            _logPersistence = logPersistence;
            _targets = targets;
            _logMessageChannel = Channel.CreateUnbounded<LogMessage>(new UnboundedChannelOptions
            {
                AllowSynchronousContinuations = false,
                SingleReader = true,
                SingleWriter = false
            });
            _loggerTask = LogTask();
            return;

            async Task LogTask()
            {
                // First, recover any pending log messages from the persistence layer and process them
                foreach (var recoveredLogMessage  in _logPersistence.GetPending())
                {
                    foreach (var target in _targets)
                    {
                        // If the target supports async logging, log asynchronously
                        if (target is IAsyncLogTarget asyncTarget)
                        {
                            await asyncTarget.LogAsync(recoveredLogMessage);
                            continue;
                        }

                        // If the target does not support async logging, log synchronously
                        target.Log(recoveredLogMessage);
                    }
                    _logPersistence.Acknowledge(recoveredLogMessage.LogId);
                }

                // Continuously read log messages from the channel and process them
                while (await Reader.WaitToReadAsync()) 
                {
                    while (Reader.TryRead(out LogMessage? logMessage))
                    {
                        foreach (var target in _targets)
                        {
                            // If the target supports async logging, log asynchronously
                            if (target is IAsyncLogTarget asyncTarget)
                            {
                                await asyncTarget.LogAsync(logMessage);
                                continue;
                            }
                            
                            // If the target does not support async logging, log synchronously
                            target.Log(logMessage);
                        }
                        _logPersistence.Acknowledge(logMessage.LogId);
                    }
                }
            }
        }

        public void Log(LogMessage logMessage)
        {
            _logPersistence.Write(logMessage);
            Writer.TryWrite(logMessage);
        }

        public async ValueTask DisposeAsync()
        {
            // Signal that no more messages will be written to the channel
            Writer.Complete();
            await _loggerTask.ConfigureAwait(false);

            if (_logPersistence is IDisposable disposable)
                disposable.Dispose();

            foreach (var target in _targets)
            {
                switch (target)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync();
                        break;
                    case IDisposable syncDisposable:
                        syncDisposable.Dispose();
                        break;
                    default:
                        continue;
                }
            }

        }
    }
}
