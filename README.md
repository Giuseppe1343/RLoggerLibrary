# RLoggerLib

[![License: AGPL3.0](https://img.shields.io/badge/License-AGPL3.0-darkred.svg)](https://opensource.org/licenses/MIT) ![Environment: Windows](https://img.shields.io/badge/Environment-Windows-blue)  ![IDE: VisualStudio](https://img.shields.io/badge/IDE-VisualStudio-purple) 

![image](https://github.com/user-attachments/assets/190e7b40-ff01-4e09-90bd-ffb8525c0355)
```
  [04:03:14] Info: Program[MLog] - Test
  [04:03:14] Warning: Program[MLog] - Test
  [04:03:14] Error: Program[MLog] - Test
  [04:03:14] Critical: Program[MLog] - Test
```

RLoggerLib is a library that performs logging operations with minimal latency without blocking the main thread. It is designed to be thread-safe, and you can add your own logging targets.

## Features

- Minimal latency logging
- Thread-safe
- Debug Logging
- Colored Console Logging
- Text File Logging
- Mail Logging
- Windows Event Logging
- Event Logging
- Support for Custom logging targets

> [!TIP]
> If you need to know how many days a log is repeated in the same day, you need to save it to SQLite Database. The default logging threshold is Warning and above.

## Installation
NETCLI
> `dotnet add package RLoggerLib --version 2.0.0`

NuGet
> `Install-Package RLoggerLib -Version 2.0.0`

### .NET Framework
For installation on .NET Framework, you need to install the `Stub.System.Data.SQLite.Core.NetFramework` NuGet package.

### .NET Core / .NET 8
For .NET Core and .NET 8, no additional packages are required as the library brings its dependencies internally.

## Configuration

All classes with the `Options` suffix provide certain settings in the creation of the logger.

> [!WARNING]
> The `MailLoggingTargetOptions` doesn't have default instance. Make sure use properly initialized options.

## Usage

### Windows Forms Application
#### Program.cs
``` csharp
// Entry point of the application
[STAThread]
static void Main()
{
	// Register the main thread
	RLogger.RegisterMainThread();

	// Create empty logger
	RLogger.Create();

	// Late binding of logging targets
	RLogger.Instance.AddDebugLogging();

	RLogger.Instance.LogInfo("Application is starting");

	Application.EnableVisualStyles();
	Application.SetCompatibleTextRenderingDefault(false);
	Application.Run(new Form1());

	RLogger.Instance.LogInfo("Application is closing");
}
```
#### Form1.cs
``` csharp
public partial class Form1 : Form
{
	private readonly IRLogger _logger = RLogger.Instance;
	public Form1()
	{
		InitializeComponent();
		_logger.LogDebug("Form1 is initialized", "ExampleSource");
		_logger.AddTextFileLogging(new TextFileLoggingTargetOptions()
		{
			FileNamingConvention = LogFileNamingConvention.CustomDate,
			CustomName = "WindowsFormsApp",
			DateFormat = "yyyy-MM-dd",
		});
		_logger.LogTrace("Text file logging is added", "ExampleSource", "B2");
	}

	private void button1_Click(object sender, EventArgs e)
	{
		_logger.LogInfo("Button1 is clicked");
	}
}
```

### ASP.NET Core Application
#### Program.cs
``` csharp
var builder = WebApplication.CreateBuilder(args);

// Register the main thread
RLogger.RegisterMainThread();

// Create logger with adding some logging targets
RLogger.Create((logger) =>
{
	logger.AddDebugLogging()
		.AddConsoleLogging()
		.AddTextFileLogging(TextFileLoggingTargetOptions.Default)
		.AddMailLogging(new MailLoggingTargetOptions()
		{
			MinRequiredSeverity = LogType.Critical, //Default is LogType.Error
			MailServer = "smtp.gmail.com", // Example mail server
			MailPort = 587, // google mail port
			MailTo = new string[] { "RECEIVER1 ADDRESS", "RECEIVER2 ADDRESS" },
			MailUser = "YOUR MAIL ADDRESS",
			MailPassword = "YOUR MAIL PASSWORD",
		})
		// For Windows Event Logging, you need to run the application as an administrator on Windows.
		.AddWindowsEventLogging(WindowsEventLoggingTargetOptions.Default);
});

// Add logging to the DI container as a singleton service
builder.Services.AddSingleton<IRLogger, RLogger>((sp) => RLogger.Instance);
```
#### WeatherForecastController.cs
``` csharp
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IRLogger _rlogger;

    public WeatherForecastController(IRLogger rlogger)
    {
        _rlogger = rlogger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _rlogger.LogInfo("GetWeatherForecast is called");
        return //Result
    }
}
```

### Custom Logging Target Example
#### ConsoleApp/Program.cs
``` csharp
// Entry point of the application
static void Main(string[] args)
{
	// Register the main thread
	RLogger.RegisterMainThread();

	// Create logger with adding some logging targets
	RLogger.Create((logger) =>
	{
		// Add debug, console and custom logging targets
		logger.AddDebugLogging()
			.AddConsoleLogging(LogType.Debug)
			.AddCustomLoggingTarget(new DemoLoggingTarget());
	});

	RLogger.Instance.LogInfo("Application is running...", "ConsoleApp", "1");

	try
	{
		// Do some work
		RLogger.Instance.LogTrace("Job Starting...");
		MLog(RLogger.Instance);
		RLogger.Instance.LogTrace("Job Completed.");
	}
	catch (Exception ex)
	{
		RLogger.Instance.LogError("Job Failed. Ex:" + ex.Message);
	}

	RLogger.Instance.LogInfo("Application is exited.", "ConsoleApp", "0");
}

// Example of a implement custom logging target
public class DemoLoggingTarget : ILoggingTarget
{
	public void Log(LogEntity logEntity)
	{
		Console.WriteLine("DEMO");
	}
}
```

> [!NOTE]
> Calling `RegisterMainThread()` and `Create(any)` once is enough. Afterwards, you can access the created logger from anywhere in the Project using `RLogger.Instance`.

> [!CAUTION]
> `RegisterMainThread()` must be called only once in the main thread, otherwise it will throw an error.

## Dependencies

- `System.Data.SQLite`
- `System.Diagnostics.EventLog`

## Contributing

Contributions are welcome! After forking the project and making adjustments and improvements, please complete your documentation and tests and create a pull request.

## Project Status

Active Development

## License

This project is licensed under the AGPL-3.0 License.
