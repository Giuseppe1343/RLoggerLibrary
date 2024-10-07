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
- Customizable logging targets

> [!TIP]
> If you need to know how many days a log is repeated in the same day, you need to save it to SQLite Database. The default logging threshold is Warning and above.

![image](https://github.com/user-attachments/assets/91a597e3-c26c-4056-bd43-69eca7a4a425)

## Installation

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
![image](https://github.com/user-attachments/assets/551a19d2-db85-4242-bae5-2c6c1418394d)
#### Form1.cs
![image](https://github.com/user-attachments/assets/66dae9c9-3f26-42bb-8689-0cc40c8f4e95)

### ASP.NET Core Application
#### Program.cs
![image](https://github.com/user-attachments/assets/4da6a47a-3393-48ee-96b8-d07ad24a8d8e)
#### WeatherForecastController.cs
![image](https://github.com/user-attachments/assets/e0ffce94-2ce4-48e1-80af-338dd10b2149)

### Custom Logging Target Example
#### ConsoleApp/Program.cs
![image](https://github.com/user-attachments/assets/65e70a00-65c7-448e-835c-85972c2bb5a8)

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
