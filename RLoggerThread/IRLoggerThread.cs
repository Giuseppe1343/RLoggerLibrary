namespace RLoggerThread
{
    /// <summary>
    /// Public interface for the use logger
    /// </summary>
    public interface IRLoggerThread
    {
        /// <summary>
        /// <overloads>Log the </overloads>message with the source, client name, client id<overloads> and log type </overloads>. <br/> <br/>
        /// If you want to use <see cref="LogTarget.Mail"/> target, you <b>should</b> fill the <paramref name="clientName"/> and <paramref name="clientId"/> parameters.
        /// </summary>
        /// <param name="message"> The message to be logged </param>
        /// <param name="source"> The source of the log. </param>
        /// <param name="clientName"> The name of the client.</param>
        /// <param name="clientId"> The id of the client.</param>
        /// <param name="logType"> The <see cref="LogType"/> of the log. </param>
        void Log(string message, string source, string clientName, string clientId, LogType logType);
    }

    /// <summary>
    /// Extension methods for the <see cref="IRLoggerThread"/> interface.
    /// </summary>
    public static class IRLoggerThreadExtensions
    {
        /// <summary>
        /// Log the <c><see cref="LogType.Trace"/></c> <inheritdoc cref="IRLoggerThread.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLoggerThread.Log"/>
        public static void LogTrace(this IRLoggerThread logger, string message, string source, string clientName = "", string clientId = "") => logger.Log(message, source, clientName, clientId, LogType.Trace);

        /// <summary>
        /// Log the <c><see cref="LogType.Debug"/></c> <inheritdoc cref="IRLoggerThread.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLoggerThread.Log"/>
        public static void LogDebug(this IRLoggerThread logger, string message, string source, string clientName = "", string clientId = "") => logger.Log(message, source, clientName, clientId, LogType.Debug);

        /// <summary>
        /// Log the <c><see cref="LogType.Information"/></c> <inheritdoc cref="IRLoggerThread.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLoggerThread.Log"/>
        public static void LogInformation(this IRLoggerThread logger, string message, string source, string clientName = "", string clientId = "") => logger.Log(message, source, clientName, clientId, LogType.Information);

        /// <summary>
        /// Log the <c><see cref="LogType.Warning"/></c> <inheritdoc cref="IRLoggerThread.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLoggerThread.Log"/>
        public static void LogWarning(this IRLoggerThread logger, string message, string source, string clientName = "", string clientId = "") => logger.Log(message, source, clientName, clientId, LogType.Warning);

        /// <summary>
        /// Log the <c><see cref="LogType.Error"/></c> <inheritdoc cref="IRLoggerThread.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLoggerThread.Log"/>
        public static void LogError(this IRLoggerThread logger, string message, string source, string clientName = "", string clientId = "") => logger.Log(message, source, clientName, clientId, LogType.Error);

        /// <summary>
        /// Log the <c><see cref="LogType.Critical"/></c> <inheritdoc cref="IRLoggerThread.Log"/>
        /// </summary>
        /// <inheritdoc cref="IRLoggerThread.Log"/>
        public static void LogCritical(this IRLoggerThread logger, string message, string source, string clientName = "", string clientId = "") => logger.Log(message, source, clientName, clientId, LogType.Critical);
    }
}
