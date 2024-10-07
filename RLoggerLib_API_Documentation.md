# RLoggerLib v2.0.0.0 API documentation

# All types

|   |   |   |
|---|---|---|
| [RLogger Class](#rlogger-class) | [IRLogger Interface](#irlogger-interface) | [IRLoggerExtensions Class](#irloggerextensions-class) |
| [LogDatabaseCreationOptions Class](#logdatabasecreationoptions-class) | [DatabaseFileNameDateSuffix Enum](#databasefilenamedatesuffix-enum) | [TableCreationInterval Enum](#tablecreationinterval-enum) |
| [LogEntity Class](#logentity-class) | [LogType Enum](#logtype-enum) | [ILoggingTarget Interface](#iloggingtarget-interface) |
| [IRLoggerExtensions Class](#irloggerextensions-class) | [TextFileLoggingTargetOptions Class](#textfileloggingtargetoptions-class) | [LogFileNamingConvention Enum](#logfilenamingconvention-enum) |
| [WindowsEventLoggingTargetOptions Class](#windowseventloggingtargetoptions-class) | [MailLoggingTargetOptions Class](#mailloggingtargetoptions-class) |  |
|   |   |   |

# RLogger Class

Namespace: RLoggerLib

The main class for the asynchronous logger.

## Constants

| Name | Type | Summary |
|---|---|---|
| **IsThreadSafe** | bool = true | This library is designed to be thread-safe. Also, logging on ILoggingTargets is done by a single thread, so they do not need to be thread safe. |
| **IsSupportMultipleInstance** | bool = false | Only one instance of the logger can be created and it is accessed via Instance. |
## Properties

| Name | Type | Summary |
|---|---|---|
| **Instance** | [RLogger](#rlogger-class) | The logger instance can only be accessed from here. Object creation is not allowed. |
| **IsAlive** | bool | The **Instance**'s alive status |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **RegisterMainThread()** | void | Register the main thread for determine the logger continue to log or not.<br><b>This method must be called only once and from the main thread.</b> |
| **Create()** | void | Create a default instance of **RLoggerLib.RLogger**. |
| **Create([Action](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)\<[IRLogger](#irlogger-interface)\> creationOptions)** | void | Create a default instance of **RLoggerLib.RLogger** with creationOptions action. |
| **Create([LogDatabaseCreationOptions](#logdatabasecreationoptions-class) options)** | void | Create a customized instance of **RLoggerLib.RLogger**. |
| **Create([LogDatabaseCreationOptions](#logdatabasecreationoptions-class) options, [Action](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)\<[IRLogger](#irlogger-interface)\> creationOptions)** | void | Create a customized instance of **RLoggerLib.RLogger** with creationOptions action. |
| **AddLoggingTarget([ILoggingTarget](#irlogger-interface) loggingTarget)** | void | Add a new logging target to the logger.<br>You can add custom logging targets by implementing **RLoggerLib.LoggingTargets.ILoggingTarget**. |
| **Log([LogType](#logtype-enum) logType, string message, string source, string sourceId)** | void | Log the message with the specified **logType** type, source, and sourceId (optional). |

# IRLogger Interface

Namespace: RLoggerLib

Public interface for using **RLoggerLib.RLogger** in a dependency injection scenario.
> [!TIP]
> Suggested service lifetime is Singleton (for the same logger instance to be used throughout the application)

## Methods

| Name | Returns | Summary |
|---|---|---|
| **AddLoggingTarget([ILoggingTarget](#irlogger-interface) loggingTarget)** | void | Add a new logging target to the logger. |
| **Log([LogType](#logtype-enum) logType, string message, string source, string sourceId)** | void | Log the message with the specified **logType** type, source, and sourceId. |

# IRLoggerExtensions Class

Namespace: RLoggerLib

Extension methods for the **RLoggerLib.IRLogger** interface.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **LogTrace([IRLogger](#irlogger-interface) logger, string message)** | void | Log the message as type `LogType.Trace` with source as caller type and sourceId as caller method name. |
| **LogTrace([IRLogger](#irlogger-interface) logger, string message, string source, string sourceId)** | void | Log the message as type `LogType.Trace` with source, and sourceId (optional). 
| **LogDebug([IRLogger](#irlogger-interface) logger, string message)** | void | Log the message as type `LogType.Debug` with source as caller type and sourceId as caller method name. |
| **LogDebug([IRLogger](#irlogger-interface) logger, string message, string source, string sourceId)** | void | Log the message as type `LogType.Debug` with source, and sourceId (optional). |
| **LogInfo([IRLogger](#irlogger-interface) logger, string message)** | void | Log the message as type `LogType.Info` with source as caller type and sourceId as caller method name. |
| **LogInfo([IRLogger](#irlogger-interface) logger, string message, string source, string sourceId)** | void | Log the message as type `LogType.Info` with source, and sourceId (optional). |
| **LogWarning([IRLogger](#irlogger-interface) logger, string message)** | void | Log the message as type `LogType.Warning` with source as caller type and sourceId as caller method name. |
| **LogWarning([IRLogger](#irlogger-interface) logger, string message, string source, string sourceId)** | void | Log the message as type `LogType.Warning` with source, and sourceId (optional). |
| **LogError([IRLogger](#irlogger-interface) logger, string message)** | void | Log the message as type `LogType.Error` with source as caller type and sourceId as caller method name. |
| **LogError([IRLogger](#irlogger-interface) logger, string message, string source, string sourceId)** | void | Log the message as type `LogType.Error` with source, and sourceId (optional). |
| **LogCritical([IRLogger](#irlogger-interface) logger, string message)** | void | Log the message as type `LogType.Critical` with source as caller type and sourceId as caller method name. |
| **LogCritical([IRLogger](#irlogger-interface) logger, string message, string source, string sourceId)** | void | Log the message as type `LogType.Critical` with source, and sourceId (optional). |

# LogDatabaseCreationOptions Class

Namespace: RLoggerLib

Options for creating the **RLoggerLib.LogDatabase** in **RLoggerLib.RLogger**.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Default** | [LogDatabaseCreationOptions](#logdatabasecreationoptions-class) | Options with default values |
| **MinRequiredSeverityForSaving** | [LogType](#logtype-enum) | The minimum required severity for saving the log to the database. |
| **Directory** | string | The directory where the log database will be saved.<br>Relative and absolute paths are supported. |
| **FileName** | string | The Database file name wihout extension. |
| **DateSuffix** | [DatabaseFileNameDateSuffix](#databasefilenamedatesuffix-enum) | The Database file name's date suffix. |
| **TableCreationInterval** | [TableCreationInterval](#tablecreationnterval-enum) | The interval for creating tables. |

# DatabaseFileNameDateSuffix Enum

Namespace: RLoggerLib

Option for date suffix of the log database file

## Values

| Name | Summary |
|---|---|
| **None** | Database file name is constant. |
| **Year** | Database file name will have the current year suffix. |
| **YearMonth** | Database file name will have the current year and month suffix. |
| **YearMonthDay** | Database file name will have the current year, month and day suffix. |

# TableCreationInterval Enum

Namespace: RLoggerLib

Option to specify the period of creating log tables in the log database

## Values

| Name | Summary |
|---|---|
| **None** | The table will be created only once. |
| **Daily** | The table will be created daily. |
| **Weekly** | The table will be created weekly. |
| **Monthly** | The table will be created monthly. |

# LogEntity Class

Namespace: RLoggerLib

Model for the log.

## Properties

| Name | Type | Summary |
|---|---|---|
| **LogDateTime** | DateTime | Date of creation of the log |
| **LogType** | [LogType](#logtype-enum) | Type (level) of the log |
| **Message** | string | Message of the log |
| **Source** | string | Source of the log |
| **SourceId** | string | Source Id of the log |
| **TodaysRepetitionCount** | long | Indicates how many times the log message has been logged this day. |
## Methods

| Name | Returns | Summary |
|---|---|---|
| **ToLogString()** | string | Returns the log as a string. |

# LogType Enum

Namespace: RLoggerLib

Defines the type (level) of the log

## Values

| Name | Summary |
|---|---|
| **Trace** | Logs that contain the most detailed messages. These messages may contain sensitive application data. These messages should never be enabled in a production environment. |
| **Debug** | Logs that are used for interactive investigation during development.  These logs should primarily contain information useful for debugging and have no long-term value. |
| **Info** | Logs that track the general flow of the application. These logs should have long-term value. |
| **Warning** | Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop. |
| **Error** | Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a failure in the current activity, not an application-wide failure. |
| **Critical** | Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires immediate attention. |

# ILoggingTarget Interface

Namespace: RLoggerLib.LoggingTargets

Interface for the logging target. <br>
You can create custom logging targets by implementing this interface.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **Log([LogEntity](#logentity-class) logEntity)** | void | Log the specified log entity. |

# IRLoggerExtensions Class

Namespace: RLoggerLib.LoggingTargets

Extension methods for the **RLoggerLib.IRLogger** interface.

## Methods

| Name | Returns | Summary |
|---|---|---|
| **AddCustomLoggingTarget([IRLogger](#irlogger-interface) logger, [ILoggingTarget](#iloggingtarget-interface) loggingTarget)** | [IRLogger](#irlogger-interface) | Add a new custom logging target to the logger. |
| **AddConsoleLogging([IRLogger](#irlogger-interface) logger, [LogType](#logtype-enum) minimumLogLevel)** | [IRLogger](#irlogger-interface) | Adds console logging to the logger. |
| **AddDebugLogging([IRLogger](#irlogger-interface) logger)** | [IRLogger](#irlogger-interface) | Adds debug logging to the logger. |
| **AddMailLogging([IRLogger](#irlogger-interface) logger, [MailLoggingTargetOptions](#mailloggingtargetoptions-class) options)** | [IRLogger](#irlogger-interface) | Adds mail logging to the logger. |
| **AddTextFileLogging([IRLogger](#irlogger-interface) logger, [TextFileLoggingTargetOptions](#textfileloggingtargetoptions-class) options)** | [IRLogger](#irlogger-interface) | Adds text file logging to the logger. |
| **AddWindowsEventLogging([IRLogger](#irlogger-interface) logger, [WindowsEventLoggingTargetOptions](#windowseventloggingtargetoptions-class) options)** | [IRLogger](#irlogger-interface) | Adds windows event logging to the logger.<br><b>Only supported on Windows.</b> |

# TextFileLoggingTargetOptions Class

Namespace: RLoggerLib.LoggingTargets

Options for creating the **TextFileLoggingTarget**.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Default** | [TextFileLoggingTargetOptions](#textfileloggingtargetoptions-class) | Options with default values. |
| **MinRequiredSeverity** | [LogType](#logtype-enum) | The minimum required severity for saving the log to the text file. |
| **Directory** | string | The directory where the log files will be saved.<br>Relative and absolute paths are supported. |
| **FileNamingConvention** | [LogFileNamingConvention](#logfilenamingconvention-enum) | The naming convention of the log file. |
| **CustomName** | string | The custom name part of file name. |
| **NameSeparator** | string | The separator for the file name parts. |
| **DateFormat** | string | The date format for the date part of the log file name. |

# LogFileNamingConvention Enum

Namespace: RLoggerLib.LoggingTargets

Options for naming the log file.

## Values

| Name | Summary |
|---|---|
| **Custom** | The log file name will be the [custom name].ext |
| **Source** | The log file name will be the [source name].ext |
| **CustomSource** | The log file name will be the [custom name][sep][source name].ext |
| **SourceId** | The log file name will be the source id. |
| **CustomSourceId** | The log file name will be the [custom name][sep][source id].ext |
| **SourceSourceId** | The log file name will be the [source name][sep][source id].ext |
| **CustomSourceSourceId** | The log file name will be the [custom name][sep][source name][sep][source id].ext |
| **Date** | The log file name will be the date. |
| **CustomDate** | The log file name will be the [custom name][sep][date].ext |
| **SourceDate** | The log file name will be the [source name][sep][date].ext |
| **CustomSourceDate** | The log file name will be the [custom name][sep][source name][sep][date].ext |
| **SourceIdDate** | The log file name will be the [source id][sep][date].ext |
| **CustomSourceIdDate** | The log file name will be the [custom name][sep][source id][sep][date].ext |
| **SourceSourceIdDate** | The log file name will be the [source name][sep][source id][sep][date].ext |
| **CustomSourceSourceIdDate** | The log file name will be the [custom name][sep][source name][sep][source id][sep][date].ext |

# WindowsEventLoggingTargetOptions Class

Namespace: RLoggerLib.LoggingTargets

Options for creating the **WindowsEventLoggingTarget**.

## Properties

| Name | Type | Summary |
|---|---|---|
| **Default** | [WindowsEventLoggingTargetOptions](#windowseventloggingtargetoptions-class) | Options with default values. |
| **MinRequiredSeverity** | [LogType](#logtype-enum) | The minimum required severity for log to the windows event log. |
| **EventSource** | string | The source of the event. If UseLogSourceAsEventSource is true, this will be ignored. |
| **UseLogSourceAsEventSource** | bool | If true, the **RLoggerLib.LogEntity.Source** will be used as as the **RLoggerLib.LoggingTargets.WindowsEventLoggingTargetOptions.EventSource**. |
| **EventId** | int | The event id will be used if UseLogSourceIdAsEventId is false or SourceId is not valid integer. |
| **UseLogSourceIdAsEventId** | bool | If true, the **RLoggerLib.LogEntity.SourceId** will be used as the **RLoggerLib.LoggingTargets.WindowsEventLoggingTargetOptions.EventId**. If SourceId is not valid integer, EventId will be used. |

# MailLoggingTargetOptions Class

Namespace: RLoggerLib.LoggingTargets

Options for creating the **MailLoggingTarget**.

## Properties

| Name | Type | Summary |
|---|---|---|
| **MinRequiredSeverity** | [LogType](#logtype-enum) | The minimum required severity to send mail. |
| **MailMaxRepeatedErrorCount** | int | The maximum repeated error count to send mail. After this count, the mail will not be sent in the same day. |
| **MailServer** | string | The mail server (host) that will be used to send the mail. |
| **MailPort** | int | The port of the mail server that will be used to send the mail. |
| **MailTo** | [string[]](https://docs.microsoft.com/en-us/dotnet/api/system.string[]) | The mail addresses that the mail will be sent. |
| **MailSenderName** | string | The name of the sender that will be shown in the mail. |
| **MailUser** | string | The mail address that will be used to send the mail. |
| **MailPassword** | string | The password of the mail address that will be used to send the mail. |

