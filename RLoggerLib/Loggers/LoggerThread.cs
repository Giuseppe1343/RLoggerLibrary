using RLoggerLib.LoggingTargets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RLoggerLib.Loggers
{
    internal class LoggerThread
    {
        private readonly SQLiteLogDatabase _logDatabase; // SQLite Database

        private readonly ConcurrentBag<ILogTarget> _loggingTargets = []; // Logging targets

        private readonly BlockingCollection<RLog> _logBlockingQueue = []; // BlockingQueue for holding unprocessed logs

        private bool _terminateCalled = false; // If true, the logger will exit after processing the logs in the queue.

        /// <summary>
        /// Create a new instance of <see cref="RLogger"/> with the <paramref name="options"/>.
        /// </summary>
        /// <param name="options"> The options for the logger's database. </param>
        internal LoggerThread(LogDatabaseCreationOptions options)
        {
            // Create the log database
            _logDatabase = new(options);

            // Create the log thread
            _loggerThread = new Thread(LoggerThreadMain)
            {
                Name = "LoggerThread",
                IsBackground = false // This thread should not be terminated by the main thread
            };

            // Start the log thread
            _loggerThread.Start();
        }

        /// <inheritdoc/>
        public void AddLoggingTarget(ILogTarget loggingTarget)
        {
            if (_terminateCalled)
                throw new InvalidOperationException(Helpers.INSTANCE_TERMINATED_EXCEPTION_MESSAGE);

            _loggingTargets.Add(loggingTarget);
        }

        /// <inheritdoc/>
        public void Log(LogLevel logLevel, string message, string source, string sourceId = "")
        {
            if (_terminateCalled)
                throw new InvalidOperationException(Helpers.INSTANCE_TERMINATED_EXCEPTION_MESSAGE);

            _logBlockingQueue.Add(new LogEntity()
            {
                LogDateTime = DateTime.Now,
                LogLevel = logLevel,
                Message = message,
                Source = source,
                SourceId = sourceId
            });
        }

        /// <summary>
        /// The main method for the log thread.
        /// </summary>
        private void LoggerThreadMain()
        {
            do
            {
                // The log thread will wait for 1 second to take the log from the queue if the log taken, it will be processed.
                while (_logBlockingQueue.TryTake(out var log, 1000)) // while the logBlockingQueue is not empty
                {
                    // Get the count of the logs for today and add the log to the database
                    _logDatabase.GetTodaysCountAndAddLog(log);

                    // Log the log to the logging targets
                    foreach (var target in _loggingTargets)
                        target.Log(log);
                }

            } while (_mainThread!.IsAlive && !_terminateCalled); // If the main thread is not alive or terminate is called, the logger thread will exit.

            if (!_mainThread!.IsAlive) // If main thread is not alive, self dispose the resources.
            {
                InternalDispose();
                _lock.Dispose();
                _creationCompleted.Dispose();
            }
        }

        /// <summary>
        /// Dispose the logger's resources.
        /// </summary>
        private void InternalDispose()
        {
            foreach (var logTarget in _loggingTargets)
                if (logTarget is IDisposable disposable)
                    disposable.Dispose();
            _logBlockingQueue.Dispose();
        }
    }
}
