using System;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

namespace RLoggerLib
{
    /// <summary>
    /// The database class for storing logs.
    /// </summary>
    internal class LogDatabase
    {
        private const string DbExtension = ".db";
        private readonly LogDatabaseCreationOptions _options;

        // Cache the if file path is constant
        private readonly string  _constantFilePath;

        /// <summary>
        /// Create a new instance of <see cref="LogDatabase"/> with the <paramref name="options"/>.
        /// </summary>
        /// <param name="options"></param>
        public LogDatabase(LogDatabaseCreationOptions options)
        {
            _options = options;

            // Normalize the directory path
            _options.Directory = _options.Directory.NormalizeDirectoryPath();

            // Normalize the file name
            _options.FileName = _options.FileName.NormalizeFileName();

            // Test the path validity
            Helpers.TestFilePath(_options.Directory, _options.FileName);

            // Cache the constant file path
            _constantFilePath = $"{_options.Directory}{_options.FileName}{DbExtension}";
        }

        /// <summary>
        /// The absolute path of the database file.
        /// </summary>
        private string AbsoluteFilePath
        {
            get
            {
                return _options.DateSuffix switch
                {
                    DatabaseFileNameDateSuffix.Year => $"{_options.Directory}{_options.FileName}_{DateTime.Today:yyyy}{DbExtension}",
                    DatabaseFileNameDateSuffix.YearMonth => $"{_options.Directory}{_options.FileName}_{DateTime.Today:yyyyMM}{DbExtension}",
                    DatabaseFileNameDateSuffix.YearMonthDay => $"{_options.Directory}{_options.FileName}_{DateTime.Today:yyyyMMdd}{DbExtension}",
                    _ => _constantFilePath,
                };
            }
        }

        /// <summary>
        /// The name of the table to use.
        /// </summary>
        private string TableName
        {
            get
            {
                return _options.TableCreationInterval switch
                {
                    TableCreationInterval.Daily => $"LogTable_{DateTime.Today:yyyyMMdd}",
                    TableCreationInterval.Weekly => $"LogTable_{DateTime.Today:yyyy}_{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)}",
                    TableCreationInterval.Monthly => $"LogTable_{DateTime.Today:yyyyMM}",
                    _ => "LogTable",
                };
            }
        }

        /// <summary>
        /// Create a valid SQLite connection with the <see cref="_options."/>.
        /// </summary>
        private SQLiteConnection ValidConnection
        {
            get
            {
                // Create the log directory if not exist
                Directory.CreateDirectory(_options.Directory);

                //Open the database file
                var connection = new SQLiteConnection($"Data Source={AbsoluteFilePath};");
                connection.Open();

                //Check table and create if not exists
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = $@"CREATE TABLE IF NOT EXISTS {TableName} (
                                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                        LogDateTime INTEGER NOT NULL,
                                                        LogType INTEGER NOT NULL,
                                                        Message TEXT NOT NULL,
                                                        Source TEXT NOT NULL,
                                                        SourceId TEXT NOT NULL );";
                createTableCommand.ExecuteNonQuery();

                return connection;
            }
        }

        /// <summary>
        /// Add a new log to the database.
        /// </summary>
        /// <param name="log"></param>
        public void AddLog(LogEntity log)
        {
            if (log.LogType < _options.MinRequiredSeverityForSaving)
                return;

            using (var connection = ValidConnection)
            {
                var insertCommand = AddCommand(connection, log);
                insertCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Add a new log to the database and check if the log exists today's table.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public void GetTodaysCountAndAddLog(LogEntity log)
        {
            if (log.LogType < _options.MinRequiredSeverityForSaving)
                return;

            using (var connection = ValidConnection)
            {
                // Check if the log exists in today's table
                var selectCommand = GetTodaysCountCommand(connection, log);
                log.TodaysRepetitionCount = (long)selectCommand.ExecuteScalar();

                // Insert anyway
                var insertCommand = AddCommand(connection, log);
                insertCommand.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// Create a new command for adding a <see cref="LogEntity"/> to the database.
        /// </summary>
        /// <param name="connection"> The connection to use in creating the command. </param>
        /// <param name="log"> The log to be added. </param>
        /// <returns> The created command. </returns>
        public SQLiteCommand AddCommand(SQLiteConnection connection, LogEntity log)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = $@"INSERT INTO {TableName} (LogDateTime, LogType, Message, Source, SourceId) VALUES (@LogDateTime, @LogType, @Message, @Source, @SourceId);";
            insertCommand.Parameters.AddWithValue("@LogDateTime", log.LogDateTime.Ticks);
            insertCommand.Parameters.AddWithValue("@LogType", (byte)log.LogType);
            insertCommand.Parameters.AddWithValue("@Message", log.Message);
            insertCommand.Parameters.AddWithValue("@Source", log.Source);
            insertCommand.Parameters.AddWithValue("@SourceId", log.SourceId);
            return insertCommand;
        }

        /// <summary>
        /// Create a new command for getting the count of the same <see cref="LogEntity"/> in the database (<see cref="LogEntity.LogDateTime"/> is ignored).
        /// </summary>
        /// <param name="connection"> The connection to use in creating the command. </param>
        /// <param name="log"> The log to be checked. </param>
        /// <returns> The created command. </returns>
        public SQLiteCommand GetTodaysCountCommand(SQLiteConnection connection, LogEntity log)
        {
            var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = $@"SELECT COUNT(*) FROM {TableName} WHERE LogDateTime >= @TodayStart AND LogType = @LogType AND Message = @Message AND Source = @Source AND SourceId = @SourceId;";
            checkCommand.Parameters.AddWithValue("@TodayStart", DateTime.Today.Ticks);
            checkCommand.Parameters.AddWithValue("@LogType", (byte)log.LogType);
            checkCommand.Parameters.AddWithValue("@Message", log.Message);
            checkCommand.Parameters.AddWithValue("@Source", log.Source);
            checkCommand.Parameters.AddWithValue("@SourceId", log.SourceId);
            return checkCommand;
        }
    }

}
