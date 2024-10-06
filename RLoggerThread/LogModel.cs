using System;

namespace RLoggerThread
{
    /// <summary>
    /// Model for the log.
    /// </summary>
    internal class LogModel
    {
        public long Id { get; set; }
        public DateTime LogTime { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public LogType LogType { get; set; }

        public LogModel(string message, string source, LogType logType)
        {
            LogTime = DateTime.Now;
            Message = message;
            Source = source;
            LogType = logType;
        }

        #region For Mail Target
        // These are the properties that are not saved in the database but are used for the mail target
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public LogModel(string message, string source, LogType logType, string clientName, string clientId)
        {
            LogTime = DateTime.Now;
            Message = message;
            Source = source;
            LogType = logType;
            ClientName = clientName;
            ClientId = clientId;
        }
        #endregion
    }
}
