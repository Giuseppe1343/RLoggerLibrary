using System;

namespace RLoggerThread
{
    /// <summary>
    /// Defines the targets of the log <br/>
    /// Can be combined with the <c>|</c> operator
    /// </summary>
    [Flags]
    public enum LogTarget : byte
    {
        /// <summary>
        /// Enable logging to the debug output
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Enable logging to the console
        /// </summary>
        Console = 2,

        /// <summary>
        /// Enable logging to a file
        /// </summary>
        File = 4,

        /// <summary>
        /// Enable logging to an email
        /// </summary>
        Mail = 8,
    }

    /// <summary>
    /// Defines the type of the log
    /// </summary>
    public enum LogType : byte
    {
        /// <summary>
        /// Logs that contain the most detailed messages. These messages may contain sensitive application data. These messages should never be enabled in a production environment.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Logs that are used for interactive investigation during development.  These logs should primarily contain information useful for debugging and have no long-term value.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Logs that track the general flow of the application. These logs should have long-term value.
        /// </summary>
        Information = 2,

        /// <summary>
        /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a failure in the current activity, not an application-wide failure.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires immediate attention.
        /// </summary>
        Critical = 5,
    }
}
