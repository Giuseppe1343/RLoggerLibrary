using System;
using System.Data.SQLite;
using System.IO;

namespace RLoggerThread
{
    /// <summary>
    /// The database class for storing logs.
    /// </summary>
    internal class LogDatabase
    {
        private readonly LogDatabaseCreationOptions _options;

        /// <summary>
        /// Create a new instance of <see cref="LogDatabase"/> with the <paramref name="options"/>.
        /// </summary>
        /// <param name="options"></param>
        public LogDatabase(LogDatabaseCreationOptions options) => _options = options;

        /// <summary>
        /// Create a valid SQLite connection with the <see cref="_options"/>.
        /// </summary>
        private SQLiteConnection ValidConnection 
        {
            get 
            {
                // Create the log directory if not exist
                Directory.CreateDirectory(_options.LogDBDirectory);

                //Open the database file
                var connection = new SQLiteConnection($"Data Source={_options.LogDBFilePath};");
                connection.Open();

                //Check table and create if not exists, tables are created daily (performance reasons)
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = $@"CREATE TABLE IF NOT EXISTS LogTable_{DateTime.Today:yyyyMMdd} (
                                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                        LogTime INTEGER NOT NULL,
                                                        LogType INTEGER NOT NULL,
                                                        Message TEXT NOT NULL,
                                                        Source TEXT NOT NULL );";
                createTableCommand.ExecuteNonQuery();

                return connection;
            } 
        }

        /// <summary>
        /// Add a new log to the database.
        /// </summary>
        /// <param name="log"></param>
        public void AddLog(LogModel log)
        {
            using (var connection = ValidConnection)
            {
                var insertCommand = connection.AddCommand(log);
                insertCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Add a new log to the database and check if the log exists today's table.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public int AddLogAndCheckIfExistsToday(LogModel log)
        {
            using (var connection = ValidConnection)
            {
                // Check if the log exists in today's table
                var selectCommand = connection.CheckCommand(log);
                var result = Convert.ToInt32(selectCommand.ExecuteScalar());

                // Insert anyway
                var insertCommand = connection.AddCommand(log);
                insertCommand.ExecuteNonQuery();

                return result;
            }
        }
    }

    /// <summary>
    /// Extension methods for the <see cref="SQLiteCommand"/> class.
    /// </summary>
    internal static class SQLiteCommandExtensions
    {
        /// <summary>
        /// Create a new command for adding a <see cref="LogModel"/> to the database.
        /// </summary>
        /// <param name="connection"> The connection to use in creating the command. </param>
        /// <param name="log"> The log to be added. </param>
        /// <returns> The created command. </returns>
        internal static SQLiteCommand AddCommand(this SQLiteConnection connection, LogModel log)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = $@"INSERT INTO LogTable_{DateTime.Today:yyyyMMdd} (LogTime, LogType, Message, Source) VALUES (@LogTime, @LogType, @Message, @Source);";
            insertCommand.Parameters.AddWithValue("@LogTime", log.LogTime.Ticks);
            insertCommand.Parameters.AddWithValue("@LogType", (byte)log.LogType);
            insertCommand.Parameters.AddWithValue("@Message", log.Message);
            insertCommand.Parameters.AddWithValue("@Source", log.Source);
            return insertCommand;
        }

        /// <summary>
        /// Create a new command for checking if a <see cref="LogModel"/> exists in the database.
        /// </summary>
        /// <param name="connection"> The connection to use in creating the command. </param>
        /// <param name="log"> The log to be checked. </param>
        /// <returns> The created command. </returns>
        internal static SQLiteCommand CheckCommand(this SQLiteConnection connection, LogModel log)
        {
            var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = $@"SELECT COUNT(*) FROM LogTable_{DateTime.Today:yyyyMMdd} WHERE LogType = @LogType AND Message = @Message AND Source = @Source;";
            checkCommand.Parameters.AddWithValue("@LogType", (byte)log.LogType);
            checkCommand.Parameters.AddWithValue("@Message", log.Message);
            checkCommand.Parameters.AddWithValue("@Source", log.Source);
            return checkCommand;
        }
    }
}
