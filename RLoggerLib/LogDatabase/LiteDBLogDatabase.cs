using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using LiteDB;

namespace RLoggerLib
{
    /// <summary>
    /// The database class for storing logs.
    /// </summary>
    internal class LiteDBLogDatabase

    {
        private const string DbExtension = ".db";
        private readonly LogDatabaseCreationOptions _options;

        // Cache the if file path is constant
        private readonly string _constantFilePath;

        /// <summary>
        /// Create a new instance of <see cref="SQLiteLogDatabase"/> with the <paramref name="options"/>.
        /// </summary>
        /// <param name="options"></param>
        public LiteDBLogDatabase(LogDatabaseCreationOptions options)
        {
            _options = options;

            // Normalize the directory path
            //_options.Directory = _options.Directory.NormalizeDirectoryPath();

            // Normalize the file name
            //_options.FileName = _options.FileName.NormalizeFileName();

            // Test the path validity
            //Helpers.TestFilePath(_options.Directory, _options.FileName);

            // Cache the constant file path
            _constantFilePath = "";// $"{_options.Directory}{_options.FileName}{DbExtension}";

            BsonMapper.Global.EnumAsInteger = true;
        }

        /// <summary>
        /// The absolute path of the database file.
        /// </summary>
        private string AbsoluteFilePath
        {
            get
            {
                return "";
                //return _options.DateSuffix switch
                //{
                //    DatabaseFileNameDateSuffix.Year => $"{_options.Directory}{_options.FileName}_{DateTime.Today:yyyy}{DbExtension}",
                //    DatabaseFileNameDateSuffix.YearMonth => $"{_options.Directory}{_options.FileName}_{DateTime.Today:yyyyMM}{DbExtension}",
                //    DatabaseFileNameDateSuffix.YearMonthDay => $"{_options.Directory}{_options.FileName}_{DateTime.Today:yyyyMMdd}{DbExtension}",
                //    _ => _constantFilePath,
                //};
            }
        }

        /// <summary>
        /// Create a valid LiteDB database with the <see cref="_options"/>.
        /// </summary>
        private LiteDatabase ValidDatabase
        {
            get
            {
                // Create the log directory if not exist
                //Directory.CreateDirectory(_options.Directory);

                //Open the database file
                return new LiteDatabase(new ConnectionString(){ Filename = AbsoluteFilePath, Upgrade = true });
            }
        }

        /// <summary>
        /// Add a new log to the database and check if the log exists today's table.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public void GetTodaysCountAndAddLog(LogEntity log)
        {
            // Create the database log entity
            var dbLog = new DatabaseLogEntity(log);

            if (dbLog.LogLevel < _options.MinRequiredSeverityForSaving)
                return;

            using (var db = ValidDatabase)
            {
                // Get the collection
                var collection = db.GetCollection<DatabaseLogEntity>($"LogTable_{DateTime.Today:yyyyMMdd}");

                // Ensure the indexes
                collection.EnsureIndex("LogHashCode");

                // Set the todays repetition count
                dbLog.TodaysRepetitionCount = collection.Count(Query.EQ("LogHashCode", dbLog.LogHashCode));

                // Insert anyway
                collection.Insert(dbLog);
            }
        }
        private class DatabaseLogEntity(LogEntity logEntity)
        {
            public DateTime LogDateTime => logEntity.LogDateTime;
            public LogLevel LogLevel => logEntity.LogLevel;
            public string Message => logEntity.Message;
            public string Source => logEntity.Source;
            public string SourceId => logEntity.SourceId;
            public int TodaysRepetitionCount { set => logEntity.TodaysRepetitionCount = value; }

            public int LogHashCode
            {
                get
                {
                    int hash = (int)logEntity.LogLevel;
                    unchecked
                    {
                        foreach (var c in logEntity.Message)
                        {
                            hash += c;
                            hash += (hash << 10);
                            hash ^= (hash >> 6);
                        }
                        foreach (var c in logEntity.Source)
                        {
                            hash += c;
                            hash += (hash << 10);
                            hash ^= (hash >> 6);
                        }
                        foreach (var c in logEntity.SourceId)
                        {
                            hash += c;
                            hash += (hash << 10);
                            hash ^= (hash >> 6);
                        }
                        hash += (hash << 3);
                        hash ^= (hash >> 11);
                        hash += (hash << 15);
                    }
                    return hash;
                }
            }

        }
    }
}
