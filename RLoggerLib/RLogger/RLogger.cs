using RLoggerLib.LoggingTargets;
using System;
using System.Collections.Concurrent;
using System.Threading;
using static System.Collections.Specialized.BitVector32;

namespace RLoggerLib
{
    /// <summary>
    /// The main class for the asynchronous logger.
    /// </summary>
    public sealed class RLogger : IRLogger
    {
        /// <summary>
        /// The <see cref="Log"/> call can be made by multiple threads.<br/> <br/>
        /// <see cref="ILoggingTarget.Log"/> to be called only by the <see cref="_loggerThread"/> itself.
        /// </summary>
        public const bool IsThreadSafe = true;

        /// <summary>
        /// Only one instance of the logger can be created.
        /// </summary>
        public const bool IsSupportMultipleInstance = false;

        #region Static Members
        private static Thread? _mainThread; // The main thread for checking if it's alive

        /// <summary>
        /// Register the main thread for determine the logger continue to log or not.
        /// <b>This method must be called only once and from the main thread.</b>
        /// </summary>
        /// <remarks>
        /// <see cref="_loggerThread"/> is not a background thread because we do not want it to be terminated by the main thread before it completes logging. In order not to prevent the application from closing, the logger should exit gracefully. For this, we need a reference to the Main Thread.
        /// </remarks>
        /// <exception cref="InvalidOperationException"> Multiple register or registered from non-main thread. </exception>
        public static void RegisterMainThread()
        {
            if (_mainThread is not null)
                throw new InvalidOperationException(Helpers.MULTIPLE_REGISTER_EXCEPTION_MESSAGE);

            if (Thread.CurrentThread.ManagedThreadId != 1)
                throw new InvalidOperationException(Helpers.REGISTERED_FROM_NON_MAIN_THREAD_EXCEPTION_MESSAGE);

            _mainThread = Thread.CurrentThread;
        }

        private static RLogger? _instance = null;

        /// <summary>
        /// The <see cref="Instance"/>'s alive status
        /// </summary>
        public static bool IsAlive => _instance is not null && _instance._loggerThread.IsAlive;

        private static readonly ManualResetEventSlim _creationCompleted = new(true);

        /// <summary>
        /// The logger instance can only be accessed from here. Object creation is not allowed.
        /// </summary>
        /// <exception cref="InvalidOperationException"> When Instance is not created </exception>
        public static RLogger Instance
        {
            get
            {
                // Wait for the creation to be completed.
                _creationCompleted.Wait();

                if (_instance is null)
                        throw new InvalidOperationException(Helpers.INSTANCE_NOT_CREATED_EXCEPTION_MESSAGE);

                return _instance;
            }
        }

        private static readonly Mutex _lock = new();

        /// <summary>
        /// Create a new instance of <see cref="RLogger"/> accessible through the <see cref="Instance"/> property. <br/>
        /// This also re-creates the instance if it's already created.
        /// </summary>
        /// <param name="options"> The options for the logger's database. </param>
        /// <param name="creationOptions"> An action to set the creation options for the logger. <br/>
        /// Example: <c> (logger) => { logger.AddDebugLogging(); } </c> </param>
        /// <exception cref="InvalidOperationException"> Main thread not registered or Instance already created. </exception>
        public static void Create(LogDatabaseCreationOptions options, Action<IRLogger> creationOptions)
        {
            if (_mainThread is null)
                throw new InvalidOperationException(Helpers.UNREGISTERED_EXCEPTION_MESSAGE);

            // If mutex lock cannot be acquired, return.
            if (!_lock.WaitOne(0))
                return;

            // Start of the critical section.

            // Start blocking the RLogger.Instance consumers until the creation is completed.
            _creationCompleted.Reset();

            // If the instance is not null, terminate it and create a new one.
            if (_instance is not null)
            {
                _instance._terminateCalled = true;
                if (_instance._loggerThread.IsAlive)
                    _instance._loggerThread.Join();
                _instance.InternalDispose();
                _instance = null;
            }

            _instance = new RLogger(options ?? LogDatabaseCreationOptions.Default);

            if (creationOptions is not null)
                creationOptions(_instance);

            // Stop blocking the RLogger.Instance consumers.
            _creationCompleted.Set();

            // End of the critical section.
            _lock.ReleaseMutex();
        }

        /// <inheritdoc cref="Create(LogDatabaseCreationOptions, Action{IRLogger})"/>
        public static void Create(Action<IRLogger> creationOptions) => Create(null, creationOptions);

        /// <inheritdoc cref="Create(LogDatabaseCreationOptions, Action{IRLogger})"/>
        public static void Create(LogDatabaseCreationOptions options) => Create(options, null);

        /// <inheritdoc cref="Create(LogDatabaseCreationOptions, Action{IRLogger})"/>
        public static void Create() => Create(null, null);

        #endregion

        #region Instance Members

        private readonly LogDatabase _logDatabase; // SQLite Database

        private readonly ConcurrentBag<ILoggingTarget> _loggingTargets = []; // Logging targets

        private readonly BlockingCollection<LogEntity> _logBlockingQueue = []; // BlockingQueue for holding unprocessed logs

        private readonly Thread _loggerThread; // The log thread

        private bool _terminateCalled = false; // If true, the logger will exit after processing the logs in the queue.

        /// <summary>
        /// Create a new instance of <see cref="RLogger"/> with the <paramref name="options"/>.
        /// </summary>
        /// <param name="options"> The options for the logger's database. </param>
        internal RLogger(LogDatabaseCreationOptions options)
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
        public void AddLoggingTarget(ILoggingTarget loggingTarget)
        {
            if (_terminateCalled)
                throw new InvalidOperationException(Helpers.INSTANCE_TERMINATED_EXCEPTION_MESSAGE);

            _loggingTargets.Add(loggingTarget);
        }

        /// <inheritdoc/>
        public void Log(LogType logType, string message, string source, string sourceId = "")
        {
            if (_terminateCalled)
                throw new InvalidOperationException(Helpers.INSTANCE_TERMINATED_EXCEPTION_MESSAGE);

            _logBlockingQueue.Add(new LogEntity()
            {
                LogDateTime = DateTime.Now,
                LogType = logType,
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
        #endregion
    }
}
