using RLogger.Targets;
using System.Threading.Channels;

namespace RLogger.Loggers
{
    internal class AsyncLogger : IRLogger, IAsyncDisposable
    {
        private readonly ILogTarget[] _targets;

        private readonly Task _loggerTask;
        private readonly Channel<LogMessage> _logMessageChannel;
        private ChannelReader<LogMessage> Reader => _logMessageChannel.Reader;
        private ChannelWriter<LogMessage> Writer => _logMessageChannel.Writer;

        public AsyncLogger(ILogTarget[] targets)
        {
            _targets = targets;
            _loggerTask = Task.Factory.StartNew(LogTask, TaskCreationOptions.LongRunning);
            _logMessageChannel = System.Threading.Channels.Channel.CreateUnbounded<LogMessage>(new()
            {
                AllowSynchronousContinuations = true,
                SingleReader = true,
                SingleWriter = false
            });

            async Task LogTask()
            {
                while (await Reader.WaitToReadAsync()) 
                {
                    while (Reader.TryRead(out LogMessage? logMessage))
                    {
                        foreach (ILogTarget target in _targets)
                        {
                            // If the target supports async logging, log asynchronously
                            if (target is IAsyncLogTarget asyncTarget)
                            {
                                await asyncTarget.LogAsync(logMessage);
                            }
                            else if (target is ILogTarget syncTarget)
                            {
                                // If the target supports synchronous logging, log synchronously
                                syncTarget.Log(logMessage);
                            }
                        }
                    }
                }
            }
        }

        public void Log(LogMessage logMessage)
        {
            Writer.TryWrite(logMessage);
        }

        public async ValueTask DisposeAsync()
        {
            // Signal that no more messages will be written to the channel
            Writer.Complete();
            await _loggerTask.ConfigureAwait(false);

            foreach (var target in _targets)
            {
                switch (target)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync();
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}
